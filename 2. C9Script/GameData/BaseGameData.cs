using System;
using System.Collections.Generic;
using System.Text;
using BackEnd;
using LitJson;
using Newtonsoft.Json;
using UniRx;

public abstract class BaseGameData
{
    public abstract string TableName { get; }
    protected abstract string InDate { get; set; }

    private const string Server = "Server";

    private readonly CompositeDisposable _saveTimerCompositeDisposable = new();

    protected virtual void InitGameData()
    {
        Debug.Log($"Start {TableName} Init");
        var param = MakeInitData();
        param.Add(Server, Managers.Server.CurrentServer);
        Debug.Log($"End {TableName} Init");
        InitGameData(TableName, param);
    }

    protected void InitGameData(string tableName, Param param)
    {
        Backend.GameData.Insert(tableName, param, bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog($"Fail Init {TableName}_GameData", bro);
                Managers.GameData.IsLoadFail = true;
                return;
            }

            InDate = bro.GetInDate();
            LoadGameData();
        });
    }

    public void InsertGameData(Action<bool> callback)
    {
        FadeScreen.Instance.OnLoadingScreen();
        
        var param = MakeInitData();
        param.Add(Server, Managers.Server.CurrentServer);
        Backend.GameData.Insert(TableName, param, bro =>
        {
            FadeScreen.Instance.OffLoadingScreen();

            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog($"Fail Insert {TableName}", bro);
                callback?.Invoke(false);
                return;
            }
            
            callback?.Invoke(true);
        });
    }

    public virtual void LoadGameData()
    {
        if (string.IsNullOrEmpty(InDate))
        {
            Where where = new Where();
            where.Equal(Server, Managers.Server.CurrentServer);
            
            Backend.GameData.GetMyData(TableName, where, GetDataCallback);
        }
        else
        {
            Backend.GameData.GetMyData(TableName, InDate, GetDataCallback);
        }
    }
    
    protected void GetDataCallback(BackendReturnObject bro)
    {
        if (!bro.IsSuccess())
        {
            Managers.Backend.FailLog($"Fail Load {TableName}_GameData", bro);
            Managers.GameData.IsLoadFail = true;
            return;
        }

        var jsonData = bro.GetFlattenJSON();

        if (jsonData.ContainsKey("row"))
            jsonData = jsonData["row"];
        else if (jsonData.ContainsKey("rows"))
            jsonData = jsonData["rows"];

        if (jsonData.Count <= 0)
        {
            InitGameData();
            return;
        }

        if (string.IsNullOrEmpty(InDate))
            InDate = bro.GetInDate();

        var gameData = jsonData.IsArray ? jsonData[0] : jsonData;
        
        Debug.Log($"{TableName} Data Size : {Encoding.UTF8.GetBytes(gameData.ToJson()).Length / 1024f}kb");

        Managers.GameData.TotalSize += Encoding.UTF8.GetBytes(gameData.ToJson()).Length;

        SetGameData(jsonData.IsArray ? jsonData[0] : jsonData);

        Managers.GameData.LoadCompleteGameDataCount++;
    }
    
    public virtual void SaveGameData(Param param, bool isSaveImmediately = false, Action callback = null)
    {
        if (isSaveImmediately)
        {
            Backend.GameData.UpdateV2(TableName, InDate, Backend.UserInDate, param, bro =>
            {
                if (!bro.IsSuccess())
                {
                    Managers.Backend.FailLog($"Fail Save {TableName}_GameData", bro);
                    return;
                }

                Debug.Log($"Success Save {TableName}_GameData");
                callback?.Invoke();
            });
            
            Debug.Log($"Send SaveGameData {TableName}");
        }
        else
        {
            // 한 프레임 내 중복 테이블 저장 방지
            var transactionValue = GameDataManager.TransactionValues.Find(data => data.table == TableName);
            if (transactionValue != null)
            {
                GameDataManager.TransactionValues.Remove(transactionValue);
                
                foreach (var paramKey in transactionValue.param.Keys)
                {
                    if (param.ContainsKey(paramKey))
                        continue;
                    
                    param.Add(paramKey, transactionValue.param[paramKey]);
                }
            }

            Debug.Log($"{TableName} Size : {Encoding.Default.GetByteCount(JsonConvert.SerializeObject(param.GetValue()))}");

            GameDataManager.TransactionValues.Add(TransactionValue.SetUpdateV2(TableName, InDate, Backend.UserInDate, param));
        }
    }

    public TransactionValue GetTransactionValue()
    {
        return TransactionValue.SetUpdateV2(TableName, InDate, Backend.UserInDate, MakeSaveData());
    }

    public void SaveGameData(bool isSaveImmediately = false, Action callback = null)
    {
        SaveGameData(MakeSaveData(), isSaveImmediately, callback);
    }
    
    public void SaveGameData(int index, bool isSaveImmediately = false)
    {
        SaveGameData(MakeSaveData(index), isSaveImmediately);
    }
    public void SaveLog()
    {
        Backend.GameLog.InsertLog(TableName, MakeSaveData(), bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog($"Fail SaveLog {TableName}", bro);
                //Firebase.Analytics.FirebaseAnalytics.LogEvent();
                return;
            }
            
            Debug.Log($"Success SaveLog {TableName}");
        });
    }

    protected abstract Param MakeInitData();
    protected abstract Param MakeSaveData();
    protected abstract void SetGameData(JsonData jsonData);

    protected virtual Param MakeSaveData(int id)
    {
        return new Param();
    }

    protected virtual Param MakeSaveData(List<int> ids)
    {
        return new Param();
    }
    
    public virtual void GetGameDatas(List<string> ownerInDates, Action<JsonData> endCallback)
    {
        List<TransactionValue> transactionActions = new List<TransactionValue>();
        
        ownerInDates.ForEach(inDate =>
        {
            Where where = new Where();
            where.Equal("owner_inDate", inDate);
            where.Equal(Server, Managers.Server.CurrentServer);

            transactionActions.Add(TransactionValue.SetGet(TableName, where));
        });

        if (transactionActions.Count <= 0)
        {
            endCallback?.Invoke(null);
            return;
        }

        Backend.GameData.TransactionReadV2(transactionActions, bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog("Fail GetGameDatas", bro);
                return;
            }
            
            endCallback?.Invoke(bro.GetFlattenJSON());
        });
    }

    public virtual void GetGameDatas(int server, List<string> ownerInDates, Action<JsonData> endCallback)
    {
        List<TransactionValue> transactionActions = new List<TransactionValue>();
        
        ownerInDates.ForEach(inDate =>
        {
            Where where = new Where();
            where.Equal("owner_inDate", inDate);
            where.Equal(Server, server);

            transactionActions.Add(TransactionValue.SetGet(TableName, where));
        });

        Backend.GameData.TransactionReadV2(transactionActions, bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog("Fail GetGameDatas", bro);
                return;
            }
            
            endCallback?.Invoke(bro.GetFlattenJSON());
        });
    }
    
    public virtual JsonData GetGameDatas(int server, List<string> ownerInDates)
    {
        List<TransactionValue> transactionActions = new List<TransactionValue>();
        
        ownerInDates.ForEach(inDate =>
        {
            Where where = new Where();
            where.Equal("owner_inDate", inDate);
            where.Equal(Server, server);
            
            transactionActions.Add(TransactionValue.SetGet(TableName, where));
        });

        if (transactionActions.Count <= 0)
            return null;
        
        var bro = Backend.GameData.TransactionReadV2(transactionActions);

        if (!bro.IsSuccess())
        {
            Managers.Backend.FailLog($"Fail GetGameDatas {TableName}", bro);
            return null;
        }

        return bro.GetFlattenJSON();
    }

    public Param GetLog()
    {
        return MakeSaveData();
    }

    public void SetSaveTimer(float time = 5f)
    {
        _saveTimerCompositeDisposable.Clear();

        Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ => SaveGameData())
            .AddTo(_saveTimerCompositeDisposable);
    }
}