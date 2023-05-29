using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using BackEnd.Tcp;
using GameData;
using LitJson;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

public class GameDataManager
{
    public static readonly EquipGameData EquipGameData = new();
    public static readonly WeaponGameData WeaponGameData = new();
    public static readonly GoodsGameData GoodsGameData = new();
    public static readonly SkillGameData SkillGameData = new();
    public static readonly CostumeGameData CostumeGameData = new();
    public static readonly PetGameData PetGameData = new();
    public static readonly WorldWoodGameData WoodGameData = new();
    public static readonly UserGameData UserGameData = new();
    public static readonly QuestGameData QuestGameData = new();
    public static readonly StatLevelGameData StatLevelGameData = new();
    public static readonly PvpGameData PvpGameData = new();
    public static readonly ShopGameData ShopGameData = new();
    public static readonly MissionGameData MissionGameData = new();
    public static readonly CollectionGameData CollectionGameData = new();
    public static readonly NewYearEventGameData NewYearEventGameData = new();
    public static readonly XMasEventGameData XMasEventGameData = new();
    public static readonly UnlimitedStatLevelGameData UnlimitedStatLevelGameData = new();
    public static readonly LabGameData LabGameData = new();
    public static readonly GuildGameData GuildGameData = new();
    public static readonly GuildAllRaidGameData GuildAllRaidGameData = new();
    public static readonly AccountGameData AccountGameData = new();
    public static readonly ChatCouponGameData ChatCouponGameData = new();
    public static readonly ProgramerBeatGameData ProgramerBeatGameData = new();

    public static readonly RankStageGameData RankStageGameData = new();
    public static readonly RankPvpGameData RankPvpGameData = new();
    public static readonly RankRaidGameData RankRaidGameData = new();

    public static readonly List<BaseGameData> GameDatas = new()
    {
        EquipGameData,
        WeaponGameData,
        GoodsGameData,
        SkillGameData,
        CostumeGameData,
        PetGameData,
        WoodGameData,
        UserGameData,
        QuestGameData,
        StatLevelGameData,
        PvpGameData,
        ShopGameData,
        MissionGameData,
        CollectionGameData,
        NewYearEventGameData,
        UnlimitedStatLevelGameData,
        LabGameData,
        GuildGameData,
        GuildAllRaidGameData,
        AccountGameData,
        ChatCouponGameData,
        ProgramerBeatGameData,
    };

    private static readonly List<BaseRankGameData> RankGameDatas = new()
    {
        RankStageGameData,
        RankPvpGameData,
        RankRaidGameData,
    };

    public double TotalSize = 0;

    public static BaseRankGameData GetRankGameData(RankType rankType) =>
        RankGameDatas.Find(rankGameData => rankGameData.RankType == rankType);

    public bool IsLoadFail = false;

    public static int TotalLoadGameDataCount => GameDatas.Count;
    private static int TotalLoadRankGameDataCount => RankGameDatas.Count;
    public int LoadCompleteGameDataCount;

    public static readonly List<TransactionValue> TransactionValues = new();

    public void InitGameData()
    {
        Managers.Game.SetServerTime(true, () =>
        {
            MainThreadDispatcher.StartCoroutine(CheckLoadingData());
            GameDatas.ForEach(gameData => gameData.LoadGameData());
        });
    }

    private IEnumerator CheckLoadingData()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (IsLoadFail)
            {
                // 로딩 실패 처리
            }

            if (LoadCompleteGameDataCount >= TotalLoadGameDataCount)
                break;
        }

        LoadCompleteGameDataCount = 0;

        RankGameDatas.ForEach(rankGameData => rankGameData.LoadGameData());


        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (IsLoadFail)
            {
                // 로딩 실패 처리
            }

            if (LoadCompleteGameDataCount >= TotalLoadRankGameDataCount)
                break;
        }

        while (true)
        {
            if (ChartManager.StageDataController.DataInit && ChartManager.StageBossDataController.DataInit)
            {
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }
        
        Debug.Log($"Total Size : {TotalSize / 1024 / 1024}Mb");

        bool isCutScene = PlayerPrefs.GetInt("CutScene", 0) == 0;
        if (isCutScene)
        {
            PlayerPrefs.SetInt("CutScene", 1);
            MessageBroker.Default.Publish(new ViewCutScene());
        }
        else
            FadeScreen.FadeOut(LoadComplete, 0);
    }

    public void LoadComplete()
    {
        Managers.Scene.ChangeScene(SceneType.Game);
        MainThreadDispatcher.LateUpdateAsObservable().Where(_ => TransactionValues.Count > 0)
            .Subscribe(_ => SaveUpdateGameData());
    }

    public void SaveAllGameData(bool isSaveImmediately = false, Action callback = null)
    {
        GameDatas.ForEach(gameData => { gameData.SaveGameData(isSaveImmediately, callback); });
    }

    public void SaveAllGameDataTransaction(Action callback = null)
    {
        int totalSize = 0;
        
        TransactionValues.ForEach(value =>
        {
            string paramToString = Newtonsoft.Json.JsonConvert.SerializeObject(value.param.GetValue());
            int count = System.Text.Encoding.Default.GetByteCount(paramToString);
            totalSize += count;
            Debug.Log($"Save Transaction {value.table} Param Size : {count}");
        });
        
        TransactionValues.Clear();
        
        GameDatas.ForEach(gameData =>
        {
            gameData.SaveGameData();
        });

        if (TransactionValues.Count > 1)
        {
            List<List<TransactionValue>> saveTransactionValues = new List<List<TransactionValue>>();
            
            Debug.Log($"Save Data Size : {totalSize / 1024f}");

            var paramCount = 0;
            var saveListIndex = 0;

            saveTransactionValues.Add(new List<TransactionValue>());
            
            foreach (var transactionValue in TransactionValues)
            {
                // https://developer.thebackend.io/unity3d/guide/gameDataV3/transaction/transactionWriteV2/
                // 트랜잭션 한번 호출에 보낼 수 있는 최대값 체크
                // 컬럼 290개
                // 10개의 테이블
                if (paramCount + transactionValue.param.Count >= 290 || saveTransactionValues[saveListIndex].Count >= 10)
                {
                    paramCount = 0;
                    saveListIndex += 1;
                    saveTransactionValues.Add(new List<TransactionValue>());
                }
                
                saveTransactionValues[saveListIndex].Add(transactionValue);
                paramCount += transactionValue.param.Count;
            }

            TransactionValues.Clear();

            int completeCount = 0;
            int index = 0;

            foreach (var saveList in saveTransactionValues)
            {
                int cacheIndex = index++;
                
                Backend.GameData.TransactionWriteV2(saveList, bro =>
                {
                    completeCount += 1;
                    
                    if (!bro.IsSuccess())
                    {
                        Managers.Backend.FailLog($"Fail SaveUpdateGameData - {cacheIndex}", bro);
                        
                        saveList.ForEach(transactionValue =>
                        {
                            Backend.GameData.UpdateV2(transactionValue.table, transactionValue.inDate,
                                transactionValue.ownerInDate, transactionValue.param);
                        });
                        
                        if (completeCount >= saveTransactionValues.Count)
                            callback?.Invoke();
                        return;
                    }

                    Debug.Log($"Success SaveUpdateGameData - {cacheIndex}");
                    if (completeCount >= saveTransactionValues.Count)
                        callback?.Invoke();
                });
            }
        }
        else if (TransactionValues.Count == 1)
        {
            var gameData = GameDatas.Find(gameData => gameData.TableName == TransactionValues[0].table) ??
                           RankGameDatas.Find(gameData => gameData.TableName == TransactionValues[0].table);

            var param = TransactionValues[0].param;
            
            TransactionValues.Clear();

            if (gameData == null)
                return;
            
            gameData.SaveGameData(param, true);
        }
    }
    
    public static void SaveItemData(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
            {
                WeaponGameData.SaveGameData();
            }
                break;
            case ItemType.Goods:
            {
                GoodsGameData.SaveGameData();
            }
                break;
            case ItemType.Costume:
            {
                    Managers.CostumeSet.CostumeCollectActive();
                    CostumeGameData.SaveGameData();
            }
                break;
            case ItemType.Pet:
            {
                PetGameData.SaveGameData();
            }
                break;
            case ItemType.Collection:
            {
                CollectionGameData.SaveGameData();
            }
                break;
        }
    }
    
    private void SaveUpdateGameData()
    {
        // 2개 이상이면 트랜잭션, 1개라면 테이블로 저장
        // 참조 : https://developer.thebackend.io/outline/manual/dbguide/dbUse/

        int totalSize = 0;
        
        TransactionValues.ForEach(value =>
        {
            string paramToString = Newtonsoft.Json.JsonConvert.SerializeObject(value.param.GetValue());
            int count = System.Text.Encoding.Default.GetByteCount(paramToString);
            totalSize += count;
            Debug.Log($"Save Transaction {value.table} Param Size : {count}");
        });
        
        if (TransactionValues.Count > 1)
        {
            List<List<TransactionValue>> saveTransactionValues = new List<List<TransactionValue>>();
            
            Debug.Log($"Save Data Size : {totalSize / 1024f}");

            var paramCount = 0;
            var saveListIndex = 0;

            // for (int i = 0; i < TransactionValues.Count; i += 10)
            //     saveLists.Add(TransactionValues.GetRange(i, Mathf.Min(TransactionValues.Count - i, 10)));
            
            saveTransactionValues.Add(new List<TransactionValue>());
            
            foreach (var transactionValue in TransactionValues)
            {
                // https://developer.thebackend.io/unity3d/guide/gameDataV3/transaction/transactionWriteV2/
                // 트랜잭션 한번 호출에 보낼 수 있는 최대값 체크
                // 컬럼 290개
                // 10개의 테이블
                if (paramCount + transactionValue.param.Count >= 290 || saveTransactionValues[saveListIndex].Count >= 10)
                {
                    paramCount = 0;
                    saveListIndex += 1;
                    saveTransactionValues.Add(new List<TransactionValue>());
                }
                
                saveTransactionValues[saveListIndex].Add(transactionValue);
                paramCount += transactionValue.param.Count;
            }

            TransactionValues.Clear();

            int index = 0;

            foreach (var saveList in saveTransactionValues)
            {
                int saveSize = 0;
                
                saveList.ForEach(value =>
                {
                    string paramToString = Newtonsoft.Json.JsonConvert.SerializeObject(value.param.GetValue());
                    saveSize += System.Text.Encoding.Default.GetByteCount(paramToString);
                    Debug.Log($"Save Transaction {index} Table : {value.table}");
                });
                
                Debug.Log($"Save Transaction {index} Param Count : {saveList.Sum(t => t.param.Count)}개");
                Debug.Log($"Save Transaction {index++} Param Size : {saveSize / 1024f}kb");
                
                Backend.GameData.TransactionWriteV2(saveList, bro =>
                {
                    if (!bro.IsSuccess())
                    {
                        saveList.ForEach(transactionValue =>
                        {
                            Backend.GameData.UpdateV2(transactionValue.table, transactionValue.inDate,
                                transactionValue.ownerInDate, transactionValue.param, bro2 =>
                                {
                                    if (bro2.IsSuccess())
                                        Debug.Log($"Success SaveUpdateGameData {transactionValue.table}");
                                });
                        });
                        
                        Managers.Backend.FailLog("Fail SaveUpdateGameData", bro);
                        return;
                    }

                    Debug.Log("Success SaveUpdateGameData");
                });
            }
        }
        else if (TransactionValues.Count == 1)
        {
            var gameData = GameDatas.Find(gameData => gameData.TableName == TransactionValues[0].table) ??
                           RankGameDatas.Find(gameData => gameData.TableName == TransactionValues[0].table);

            var param = TransactionValues[0].param;
            
            TransactionValues.Clear();

            if (gameData == null)
                return;
            
            gameData.SaveGameData(param, true);
        }
    }

    public static void SaveImmediatelyAllGameData()
    {
        List<TransactionValue> transactionValues = new();
        
        GameDatas.ForEach(gameData => transactionValues.Add(gameData.GetTransactionValue()));

        List<List<TransactionValue>> saveLists = new();
        
        for (int i = 0; i < transactionValues.Count; i += 10)
            saveLists.Add(transactionValues.GetRange(i, Mathf.Min(transactionValues.Count - i, 10)));

        saveLists.ForEach(saveData => Backend.GameData.TransactionWriteV2(saveData));
        Debug.Log("SaveImmediatelyAllGameData");
    }

    public static void GetAllDataLog(ref Param param)
    {
        param.Add("BuildVersion", Application.version);
        
        foreach (var gameData in GameDatas)
            param.Add(gameData.TableName, gameData.GetLog());
    }
}