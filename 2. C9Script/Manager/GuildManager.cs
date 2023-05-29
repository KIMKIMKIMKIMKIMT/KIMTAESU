using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using BackEnd.Tcp;
using CodeStage.AntiCheat.ObscuredTypes;
using GameData;
using LitJson;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

public class GuildMetaData
{
    private ObscuredString _desc;

    public string Desc
    {
        get => _desc;
        set => _desc = value;
    }

    private ObscuredInt _grade;

    public int Grade
    {
        get => _grade;
        set
        {
            _grade = value;
            Managers.Game.CalculateStat();
        }
    }

    private ObscuredInt _mark;

    public int Mark
    {
        get => _mark;
        set => _mark = value;
    }

    private ObscuredInt _memberMaxCount;

    public int MemberMaxCount
    {
        get => _memberMaxCount;
        set => _memberMaxCount = value;
    }

    private ObscuredInt _server;

    public int Server
    {
        get => _server;
        set => _server = value;
    }

    public DateTime? ChangeMarkCoolTime;
    public DateTime? ChangeDescCoolTime;
}

public class EnemyGuildMetaData
{
    private ObscuredString _desc;

    public string Desc
    {
        get => _desc;
        set => _desc = value;
    }

    private ObscuredInt _grade;

    public int Grade
    {
        get => _grade;
        set
        {
            _grade = value;
        }
    }

    private ObscuredInt _mark;

    public int Mark
    {
        get => _mark;
        set => _mark = value;
    }

    private ObscuredInt _memberMaxCount;

    public int MemberMaxCount
    {
        get => _memberMaxCount;
        set => _memberMaxCount = value;
    }

    private ObscuredInt _server;

    public int Server
    {
        get => _server;
        set => _server = value;
    }

    public DateTime? ChangeMarkCoolTime;
    public DateTime? ChangeDescCoolTime;
}

public class GuildData
{
    public GuildMetaData GuildMetaData = new();

    private ObscuredInt _memberCount;

    public int MemberCount
    {
        get => _memberCount;
        set => _memberCount = value;
    }

    private ObscuredString _masterNickname;

    public string MasterNickname
    {
        get => _masterNickname;
        set => _masterNickname = value;
    }

    private ObscuredString _inDate;

    public string InDate
    {
        get => _inDate;
        set => _inDate = value;
    }

    private ObscuredString _guildName;

    public string GuildName
    {
        get => _guildName;
        set => _guildName = value;
    }

    private ObscuredString _guildMasterInDate;

    public string GuildMasterInDate
    {
        get => _guildMasterInDate;
        set => _guildMasterInDate = value;
    }

    private ObscuredBool _immediateRegistration;

    public bool ImmediateRegistration
    {
        get => _immediateRegistration;
        set => _immediateRegistration = value;
    }

    public Dictionary<int, int> GoodsData = new();
}

public class EnemyGuildData
{
    public EnemyGuildMetaData EnemyGuildMetaData = new();

    private ObscuredInt _memberCount;

    public int MemberCount
    {
        get => _memberCount;
        set => _memberCount = value;
    }

    private ObscuredString _inDate;

    public string InDate
    {
        get => _inDate;
        set => _inDate = value;
    }

    private ObscuredString _guildName;

    public string GuildName
    {
        get => _guildName;
        set => _guildName = value;
    }

    private ObscuredString _guildMasterInDate;

    public string GuildMasterInDate
    {
        get => _guildMasterInDate;
        set => _guildMasterInDate = value;
    }

    private ObscuredBool _immediateRegistration;

    public bool ImmediateRegistration
    {
        get => _immediateRegistration;
        set => _immediateRegistration = value;
    }

    public int GoodsData;
}

public class GuildMemberData
{
    private ObscuredString _inDate;

    public string InDate
    {
        get => _inDate;
        set => _inDate = value;
    }

    private ObscuredInt _lv;

    public int Lv
    {
        get => _lv;
        set => _lv = value;
    }

    private ObscuredString _nickname;

    public string Nickname
    {
        get => _nickname;
        set => _nickname = value;
    }

    private ObscuredInt _promoGrade;

    public int PromoGrade
    {
        get => _promoGrade;
        set => _promoGrade = value;
    }

    private ObscuredDouble _power;

    public double Power
    {
        get => _power;
        set => _power = value;
    }

    public int Passive_Godgod_Level;

    private ObscuredString _position;

    public string Position
    {
        get => _position;
        set => _position = value;
    }

    public DateTime LastConnectTime;
    public DateTime AttendanceTime;
    public DateTime GuildJoinTime;

    public readonly Dictionary<int, int> GoodsDic = new()
    {
        {1, 0},
        {2, 0},
        {3, 0},
        {4, 0},
        {5, 0},
        {6, 0},
        {7, 0},
        {8, 0},
        {9, 0},
        {10, 0},
    };
    public Dictionary<int, double> StatDatas = new();
    public Dictionary<EquipType, int> EquipData = new();

    public bool IsConnect()
    {
        return (Utils.GetNow() - LastConnectTime).TotalMinutes <= 10;
    }

    public string GetPositionString()
    {
        switch (Position)
        {
            case "master":
                return "총장";
            case "viceMaster":
                return "교수";
            default:
                return "학생";
        }
    }

    public int GetTotalContribution()
    {
        return GoodsDic.Values.Sum();
    }
}

public class EnemyGuildMemberData
{
    private ObscuredString _inDate;

    public string InDate
    {
        get => _inDate;
        set => _inDate = value;
    }

    private ObscuredInt _lv;

    public int Lv
    {
        get => _lv;
        set => _lv = value;
    }

    private ObscuredString _nickname;

    public string Nickname
    {
        get => _nickname;
        set => _nickname = value;
    }

    private ObscuredInt _promoGrade;

    public int PromoGrade
    {
        get => _promoGrade;
        set => _promoGrade = value;
    }

    private ObscuredDouble _power;

    public double Power
    {
        get => _power;
        set => _power = value;
    }

    public int Passive_Godgod_Level;

    public Dictionary<int, double> EnemyStatDatas = new();
    public Dictionary<EquipType, int> EquipData = new();
}

public class GuildManager
{
    private readonly Dictionary<int, string> _guildRandomInDateDic = new()
    {
        { 1, "776542f0-8a5d-11ed-b687-b57734d5662d" },
        { 2, "7d073790-8a5d-11ed-8036-bb5c9ed6a463" },
        { 100, "b9ff6eb0-8a5d-11ed-b502-bb6a14a0402f" }
    };

    // 내 길드 정보
    public GuildData GuildData;
    public bool IsBelongGuild => GuildData != null;

    // Key : GamerInDate, Value : GuildMemberData
    public Dictionary<string, GuildMemberData> GuildMemberDatas = new();
    public Dictionary<string, EnemyGuildMemberData> EnemyGuildMemberDatas = new();

    // Key : InDate, Value : GuildMemberData
    public readonly Dictionary<string, GuildMemberData> GuildApplicantsDatas = new();

    // 검색한 길드 정보
    public readonly List<GuildData> GuildList = new();
    public readonly List<EnemyGuildData> EnemyGuildList = new();
    public EnemyGuildData EnemyGuild;

    private bool _isGuildDataFlag = true;
    private bool _isGuildMemberFlag = true;
    private bool _isGuildGoodsFlag = true;

    public GuildMemberData MyGuildMemberData =>
        GuildMemberDatas.TryGetValue(Backend.UserInDate, out var myGuildMemberData) ? myGuildMemberData : null;

    

    public void ResetGuildMemberFlag()
    {
        _isGuildMemberFlag = true;
        _memberComposite.Clear();
    }

    public void ResetGuildGoodsFlag()
    {
        _isGuildGoodsFlag = true;
        _goodsComposite.Clear();
    }

    private int _memberCount;
    private int _enemyMemberCount;
    private int _readCompleteCount;
    private int _readCompleteCount_Sports;

    private readonly CompositeDisposable _guildDataComposite = new();
    private readonly CompositeDisposable _memberComposite = new();
    private readonly CompositeDisposable _goodsComposite = new();

    public void Init(bool isRefreshUI = false)
    {
        var bro = Backend.Guild.GetMyGuildInfoV3();

        if (!bro.IsSuccess())
        {
            Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

            // 길드가 없는 유저
            if (statusCode.Equals("412") && errorCode.Equals("PreconditionFailed") &&
                message.Contains("notGuildMember"))
            {
                GuildData = null;
            }
            else
                Managers.Backend.FailLog(bro, "Fail GetMyGuildInfo");

            return;
        }

        GuildData = ParseGuildData(bro);

        GetMyGuildMember(null, false);

        GetMyGuildGoodsData(null, false);

        // 멤버가 아닌 총장, 교수직은 가입자 리스트까지 로드
        if (MyGuildMemberData != null && !MyGuildMemberData.Position.Equals("member"))
            GetGuildApplicants(null, false);

        _isGuildDataFlag = false;

        _guildDataComposite.Clear();
        Observable.Timer(TimeSpan.FromMinutes(5)).Subscribe(_ => { _isGuildDataFlag = true; })
            .AddTo(_guildDataComposite);

        if (isRefreshUI)
        {
            var guildPopup = Managers.UI.FindPopup<UI_GuildPopup>();
            if (guildPopup != null)
            {
                guildPopup.SetGuildUI();
            }
        }
    }

    public GuildData ParseGuildData(BackendReturnObject bro)
    {
        var jsonData = bro.GetFlattenJSON()["guild"];

        var guildData = new GuildData
        {
            GuildName = jsonData["guildName"].ToString(),
            MasterNickname = jsonData["masterNickname"].ToString(),
            MemberCount = int.Parse(jsonData["memberCount"].ToString()),
            InDate = jsonData["inDate"].ToString(),
            GuildMasterInDate = jsonData["masterInDate"].ToString(),
            ImmediateRegistration = false,
            GuildMetaData =
            {
                Desc = jsonData["Desc"].ToString(),
                Grade = int.Parse(jsonData["Grade"].ToString()),
                Mark = int.Parse(jsonData["Mark"].ToString()),
                MemberMaxCount = int.Parse(jsonData["MemberMaxCount"].ToString()),
                Server = int.Parse(jsonData["Server"].ToString())
            }
        };

        if (jsonData.ContainsKey("ChangeMarkCoolTime"))
            guildData.GuildMetaData.ChangeMarkCoolTime = DateTime.Parse(jsonData["ChangeMarkCoolTime"].ToString());

        if (jsonData.ContainsKey("ChangeDescCoolTime"))
            guildData.GuildMetaData.ChangeDescCoolTime = DateTime.Parse(jsonData["ChangeDescCoolTime"].ToString());

        return guildData;
    }

    public EnemyGuildData ParseEnemyGuildData(BackendReturnObject bro)
    {
        JsonData jsonData = bro.GetFlattenJSON()["guild"];
        //JsonData guildJson = jsonData[0];

        var enemyGuildData = new EnemyGuildData
        {
            GuildName = jsonData["guildName"].ToString(),
            MemberCount = int.Parse(jsonData["memberCount"].ToString()),
            InDate = jsonData["inDate"].ToString(),
            GuildMasterInDate = jsonData["masterInDate"].ToString(),
            ImmediateRegistration = false,
            GoodsData = 0,
            
            EnemyGuildMetaData =
            {
                Desc = jsonData["Desc"].ToString(),
                Grade = int.Parse(jsonData["Grade"].ToString()),
                Mark = int.Parse(jsonData["Mark"].ToString()),
                MemberMaxCount = int.Parse(jsonData["MemberMaxCount"].ToString()),
                Server = int.Parse(jsonData["Server"].ToString())
            }
        };
        Debug.Log("AsyncEnd : " + enemyGuildData.GuildName.ToString() + " " + enemyGuildData.GoodsData.ToString());

        //if (jsonData.ContainsKey("ChangeMarkCoolTime"))
        //    enemyGuildData.EnemyGuildMetaData.ChangeMarkCoolTime = DateTime.Parse(jsonData["ChangeMarkCoolTime"].ToString());

        //if (jsonData.ContainsKey("ChangeDescCoolTime"))
        //    enemyGuildData.EnemyGuildMetaData.ChangeDescCoolTime = DateTime.Parse(jsonData["ChangeDescCoolTime"].ToString());

        return enemyGuildData;
    }

    //public EnemyGuildData GetEnemyGuildGoods(EnemyGuildData enemyGuildData)
    //{
    

//    Debug.Log("AsyncEnd : "+enemyGuildData.GuildName.ToString() + " " + enemyGuildData.GoodsData.ToString());
//    return enemyGuildData;
//}



public void GetMyGuildData(Action callback, bool isCheckFlag = true)
    {
        if (!isCheckFlag || _isGuildDataFlag)
        {
            if (isCheckFlag)
            {
                _isGuildDataFlag = false;

                _guildDataComposite.Clear();
                Observable.Timer(TimeSpan.FromMinutes(5)).Subscribe(_ => { _isGuildDataFlag = true; })
                    .AddTo(_guildDataComposite);
            }

            FadeScreen.Instance.OnLoadingScreen();

            Backend.Guild.GetMyGuildInfoV3(bro =>
            {
                FadeScreen.Instance.OffLoadingScreen();

                if (!bro.IsSuccess())
                {
                    Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                    if (statusCode.Contains("412") && errorCode.Contains("PreconditionFailed"))
                    {
                        if (message.Contains("guild's version"))
                            Managers.Message.ShowMessage("잘못된 호출 입니다.");
                        else if (message.Contains("notGuildMember"))
                        {
                            GuildData = null;
                            GuildMemberDatas.Clear();

                            if (Backend.Chat.IsChatConnect(ChannelType.Guild))
                                Backend.Chat.LeaveChannel(ChannelType.Guild);
                        }
                    }
                    else
                    {
                        Managers.Backend.FailLog("Fail GetMyGuildInfo", bro);
                    }

                    callback?.Invoke();
                    Managers.Game.CalculateStat();
                    return;
                }

                GuildData = ParseGuildData(bro);
                Managers.Game.CalculateStat();
                _isGuildMemberFlag = true;
                _isGuildGoodsFlag = true;
                GetMyGuildMember(() => GetMyGuildGoodsData(callback));
            });
        }
        else
        {
            callback?.Invoke();
        }
    }

    public void GetGuildList(Action callback, bool isPvp = false)
    {
        FadeScreen.Instance.OnLoadingScreen();

        Backend.RandomInfo.GetRandomData(RandomType.Guild, _guildRandomInDateDic[Managers.Server.CurrentServer],
            Managers.Server.CurrentServer, 0, 10,
            bro =>
            {
                if (!bro.IsSuccess())
                {
                    FadeScreen.Instance.OffLoadingScreen();
                    string statusCode = bro.GetStatusCode();
                    string errorCode = bro.GetErrorCode();
                    string message = bro.GetMessage();

                    if (statusCode.Equals("400"))
                    {
                        // 입력한 uuid가 null 혹은 string.Empty일 경우
                        if (errorCode.Equals("ValidationException") && message.Contains("uuid"))
                        {
                            Managers.Message.ShowMessage("에러 코드 400 uuid");
                            callback?.Invoke();
                            return;
                        }

                        // 랜덤 조회의 유형이 다를 경우(유저 유형인데 RandomType.Guild으로 지정했을 경우)
                        if (errorCode.Equals("BadParameterException") && message.Contains("bad type"))
                        {
                            Managers.Message.ShowMessage("에러 코드 400 bad type");
                            callback?.Invoke();
                            return;
                        }
                    }
                    else if (statusCode.Equals("404"))
                    {
                        // 존재하지 않는 uuid일 경우
                        if (errorCode.Equals("NotFoundException") && message.Contains("randomPool"))
                        {
                            Managers.Message.ShowMessage("에러 코드 404 uuid");
                            callback?.Invoke();
                            return;
                        }
                    }
                    else
                    {
                        Managers.Backend.FailLog("Fail Guild Random", bro);
                        callback?.Invoke();
                        return;
                    }
                }

                JsonData jsonData = bro.GetFlattenJSON()["rows"];

                if (jsonData == null)
                {
                    FadeScreen.Instance.OffLoadingScreen();
                    callback?.Invoke();
                    return;
                }

                GuildList.Clear();
                EnemyGuildList.Clear();

                List<string> guildInDates = new();

                for (int i = 0; i < jsonData.Count; i++)
                {
                    string guildInDate = jsonData[i]["guildInDate"].ToString();
                    guildInDates.Add(guildInDate);
                }

                int completeCount = 0;

                if (guildInDates.Count > 0)
                {
                    guildInDates.ForEach(guildInDate =>
                    {
                        Backend.Guild.GetGuildInfoV3(guildInDate, guildBro =>
                        {
                            void UpdateComplete()
                            {
                                ++completeCount;

                                if (completeCount < guildInDates.Count)
                                    return;

                                if (isPvp)
                                {
                                    GetNearEnemyGuild(callback);
                                }
                                else
                                {
                                    FadeScreen.Instance.OffLoadingScreen();
                                    callback?.Invoke();
                                }

                                
                            }

                            if (!guildBro.IsSuccess())
                            {
                                Managers.Backend.FailLog("Fail GetGuildInfo", guildBro);
                                UpdateComplete();
                                return;
                            }

                            if (isPvp)
                            {
                                EnemyGuildData enemyGuildData = ParseEnemyGuildData(guildBro);
                                EnemyGuildList.Add(enemyGuildData);
                            }
                            else
                            {
                                var guildData = ParseGuildData(guildBro);
                                GuildList.Add(guildData);
                            }

                            UpdateComplete();
                        });
                    });
                }
                else
                {
                    FadeScreen.Instance.OffLoadingScreen();
                    callback?.Invoke();
                }
            });

        
    }

    public void GetNearEnemyGuild(Action callback = null)
    {
        int count = 0;

        EnemyGuildList.ForEach(enemyGuildData =>
        {
            Backend.Guild.GetGuildGoodsByIndateV3(enemyGuildData.InDate, bro =>
            {
                if (!bro.IsSuccess())
                {
                    Debug.Log("FailGetGoods");
                    return;
                }

                JsonData jsonData = bro.GetFlattenJSON()["goods"];

                enemyGuildData.GoodsData = int.Parse(jsonData["totalGoods3Amount"].ToString());

                SearchNearEnemy();
            });
        });

        void SearchNearEnemy()
        {
            count++;

            if (count < EnemyGuildList.Count)
                return;

            int myGuildGoodsData = GuildData.GoodsData[3];

            var near = EnemyGuildList.OrderBy(x => Math.Abs(myGuildGoodsData - x.GoodsData)).First();

            EnemyGuild = near;

            GetEnemyGuildMemberList(callback);
        }

        

    }

    public void GetEnemyGuildMemberList(Action callback = null)
    {
        var bro = Backend.Guild.GetGuildMemberListV3(EnemyGuild.InDate);
        OnBackendGetEnemyGuildMemberList(bro);

        void OnBackendGetEnemyGuildMemberList(BackendReturnObject bro)
        {
            if (!bro.IsSuccess())
            {
                callback?.Invoke();
                Debug.LogError("OnBackendGetEnemyGuildMemberList_False");
                FadeScreen.Instance.OffLoadingScreen();
                return;
            }

            var jsonData = bro.GetFlattenJSON()["rows"];

            EnemyGuildMemberDatas.Clear();

            List<TransactionValue> transactionValues = new List<TransactionValue>();

            for (int i = 0; i < jsonData.Count; i++)
            {
                var userJson = jsonData[i];

                string nickname = userJson["nickname"].ToString();
                string inDate = userJson["gamerInDate"].ToString();
                //string position = userJson["position"].ToString();
                //string lastConnectTime = userJson["lastLogin"].ToString();
                //string guildJoinTime = userJson["inDate"].ToString();

                var enemyGuildMemberData = new EnemyGuildMemberData()
                {
                    Nickname = nickname,
                    InDate = inDate,
                    //Position = position,
                    //LastConnectTime = DateTime.Parse(lastConnectTime).ToKstTime(),
                    //GuildJoinTime = DateTime.Parse(guildJoinTime).ToKstTime()
                };

                if (EnemyGuildMemberDatas.ContainsKey(inDate))
                    EnemyGuildMemberDatas[inDate] = enemyGuildMemberData;
                else
                    EnemyGuildMemberDatas.Add(inDate, enemyGuildMemberData);

                var where = new Where();

                where.Equal("owner_inDate", inDate);
                where.Equal("Server", Managers.Server.CurrentServer);

                transactionValues.Add(TransactionValue.SetGet("PvpInfo", where));
            }

            _enemyMemberCount = jsonData.Count;
            _readCompleteCount_Sports = 0;

            if (transactionValues.Count <= 0)
            {
                FadeScreen.Instance.OffLoadingScreen();
                callback?.Invoke();
                return;
            }

            for (int i = 0; i < transactionValues.Count; i += 10)
            {
                var transactionList = transactionValues.GetRange(i, Mathf.Min(transactionValues.Count - i, 10));
                int readCount = transactionList.Count;

                Backend.GameData.TransactionReadV2(transactionList, readBro =>
                {
                    if (readBro.IsSuccess())
                    {
                        OnBackendEnemyGuildMemberDataTransactionRead(readBro);
                    }
                    else
                    {
                        Managers.Backend.FailLog("Fail Transaction Read", readBro);
                    }

                    _readCompleteCount_Sports += readCount;

                    if (!CheckCompleteTransactionRead_Sports())
                        return;

                    FadeScreen.Instance.OffLoadingScreen();
                    callback?.Invoke();
                });
            }
        }
    }

    public void GetMyGuildMember(Action callback, bool isAsync = true)
    {
        if (_isGuildMemberFlag || GuildMemberDatas.Count <= 0)
        {
            _isGuildMemberFlag = false;

            _memberComposite.Clear();
            Observable.Timer(TimeSpan.FromMinutes(5)).ObserveOnMainThread().Subscribe(_ =>
            {
                _isGuildMemberFlag = true;
            }).AddTo(_memberComposite);

            FadeScreen.Instance.OnLoadingScreen();

            void OnBackendGetGuildMemberList(BackendReturnObject bro)
            {
                if (!bro.IsSuccess())
                {
                    FadeScreen.Instance.OffLoadingScreen();

                    if (bro.GetStatusCode().Contains("404") && bro.GetErrorCode().Contains("NotFoundException") &&
                        bro.GetMessage().Contains("guild not found"))
                        Managers.Message.ShowMessage("잘못된 학교 데이터 입니다.");
                    else
                        Managers.Backend.FailLog("Fail GetGuildMemberList", bro);

                    callback?.Invoke();
                    return;
                }

                var jsonData = bro.GetFlattenJSON()["rows"];

                GuildMemberDatas.Clear();

                List<TransactionValue> transactionValues = new();

                for (int i = 0; i < jsonData.Count; i++)
                {
                    var userJson = jsonData[i];

                    string nickname = userJson["nickname"].ToString();
                    string inDate = userJson["gamerInDate"].ToString();
                    string position = userJson["position"].ToString();
                    string lastConnectTime = userJson["lastLogin"].ToString();
                    string guildJoinTime = userJson["inDate"].ToString();

                    var guildMemberData = new GuildMemberData()
                    {
                        Nickname = nickname,
                        InDate = inDate,
                        Position = position,
                        LastConnectTime = DateTime.Parse(lastConnectTime).ToKstTime(),
                        GuildJoinTime = DateTime.Parse(guildJoinTime).ToKstTime()
                    };

                    for (int j = 1; j <= 10; j++)
                    {
                        string goodsKey = $"totalGoods{j}Amount";

                        if (guildMemberData.GoodsDic.ContainsKey(j))
                            guildMemberData.GoodsDic[j] = int.Parse(userJson[goodsKey].ToString());
                        else
                            guildMemberData.GoodsDic.Add(j, int.Parse(userJson[goodsKey].ToString()));
                    }

                    if (GuildMemberDatas.ContainsKey(inDate))
                        GuildMemberDatas[inDate] = guildMemberData;
                    else
                        GuildMemberDatas.Add(inDate, guildMemberData);

                    var where = new Where();

                    where.Equal("owner_inDate", inDate);
                    where.Equal("Server", Managers.Server.CurrentServer);

                    transactionValues.Add(TransactionValue.SetGet("PvpInfo", where));
                }

                _memberCount = jsonData.Count;
                _readCompleteCount = 0;

                if (transactionValues.Count <= 0)
                {
                    FadeScreen.Instance.OffLoadingScreen();
                    callback?.Invoke();
                    return;
                }

                for (int i = 0; i < transactionValues.Count; i += 10)
                {
                    var transactionList = transactionValues.GetRange(i, Mathf.Min(transactionValues.Count - i, 10));
                    int readCount = transactionList.Count;

                    Backend.GameData.TransactionReadV2(transactionList, readBro =>
                    {
                        if (readBro.IsSuccess())
                        {
                            OnBackendMyGuildMemberDataTransactionRead(readBro);
                        }
                        else
                        {
                            Managers.Backend.FailLog("Fail Transaction Read", readBro);
                        }

                        _readCompleteCount += readCount;

                        if (!CheckCompleteTransactionRead())
                            return;

                        GuildMemberDatas = GuildMemberDatas
                            .OrderBy(guildMemberData => guildMemberData.Value.Position == "member")
                            .ThenBy(guildMemberData => guildMemberData.Value.Position == "viceMaster")
                            .ThenBy(guildMemberData => guildMemberData.Value.Position == "master")
                            .ToDictionary(guildMemberData => guildMemberData.Key,
                                guildMemberData => guildMemberData.Value);

                        FadeScreen.Instance.OffLoadingScreen();
                        callback?.Invoke();
                    });
                }
            }

            if (isAsync)
            {
                Backend.Guild.GetGuildMemberListV3(GuildData.InDate, OnBackendGetGuildMemberList);
            }
            else
            {
                var bro = Backend.Guild.GetGuildMemberListV3(GuildData.InDate);
                OnBackendGetGuildMemberList(bro);
                GuildMemberDatas = GuildMemberDatas
                    .OrderBy(guildMemberData => guildMemberData.Value.Position == "member")
                    .ThenBy(guildMemberData => guildMemberData.Value.Position == "viceMaster")
                    .ThenBy(guildMemberData => guildMemberData.Value.Position == "master")
                    .ToDictionary(guildMemberData => guildMemberData.Key, guildMemberData => guildMemberData.Value);
            }
        }
        else
            callback?.Invoke();
    }



    public void SetGuildGoodsFlag(bool isFlag)
    {
        _isGuildGoodsFlag = isFlag;
    }

    public void GetMyGuildGoodsData(Action callback, bool isAsync = true)
    {
        if (_isGuildGoodsFlag)
        {
            _goodsComposite.Clear();

            _isGuildGoodsFlag = false;
            Observable.Timer(TimeSpan.FromMinutes(5)).Subscribe(_ => _isGuildGoodsFlag = true).AddTo(_goodsComposite);

            if (isAsync)
            {
                FadeScreen.Instance.OnLoadingScreen();
                Backend.Guild.GetMyGuildGoodsV3(OnBackendGetMyGuildGoods);
            }
            else
            {
                var bro = Backend.Guild.GetMyGuildGoodsV3();
                OnBackendGetMyGuildGoods(bro);
            }
        }
        else
            callback?.Invoke();

        void OnBackendGetMyGuildGoods(BackendReturnObject bro)
        {
            if (!bro.IsSuccess())
            {
                Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                // 길드가 없는 유저
                if (statusCode.Equals("412") && errorCode.Equals("PreconditionFailed") &&
                    message.Contains("notGuildMember"))
                {
                    GuildData = null;
                    if (Backend.Chat.IsChatConnect(ChannelType.Guild))
                        Backend.Chat.LeaveChannel(ChannelType.Guild);
                }
                else
                    Managers.Backend.FailLog(bro, "Fail GetMyGuildInfo");

                FadeScreen.Instance.OffLoadingScreen();
                callback?.Invoke();
                return;
            }

            JsonData jsonData = bro.GetFlattenJSON()["goods"];

            for (int i = 1; i <= 10; i++)
            {
                string goodsAmount = jsonData[$"totalGoods{i}Amount"].ToString();

                if (GuildData.GoodsData.ContainsKey(i))
                    GuildData.GoodsData[i] = int.Parse(goodsAmount);
                else
                    GuildData.GoodsData.Add(i, int.Parse(goodsAmount));

                var userJsonData = jsonData[$"goods{i}UserList"];
                for (int j = 0; j < userJsonData.Count; j++)
                {
                    string nickname = userJsonData[j]["nickname"].ToString();
                    string totalAmount = userJsonData[j]["totalAmount"].ToString();

                    var guildMemberData = GuildMemberDatas.Values.ToList().Find(guildMemberData => guildMemberData.Nickname == nickname);
                    if (guildMemberData != null)
                    {
                        if (GuildMemberDatas.ContainsKey(guildMemberData.InDate))
                        {
                            DateTime attendanceTime = DateTime.Parse(userJsonData[j]["updatedAt"].ToString());
                            attendanceTime = attendanceTime.ToKstTime();
                            GuildMemberDatas[guildMemberData.InDate].AttendanceTime = attendanceTime;

                            if (GuildMemberDatas[guildMemberData.InDate].GoodsDic.ContainsKey(i))
                                GuildMemberDatas[guildMemberData.InDate].GoodsDic[i] = int.Parse(totalAmount);
                            else
                                GuildMemberDatas[guildMemberData.InDate].GoodsDic.Add(i, int.Parse(totalAmount));
                        }
                    }
                }
            }

            FadeScreen.Instance.OffLoadingScreen();
            callback?.Invoke();
        }
    }

    private void OnBackendMyGuildMemberDataTransactionRead(BackendReturnObject bro)
    {
        var jsonData = bro.GetFlattenJSON()["Responses"];

        for (int i = 0; i < jsonData.Count; i++)
        {
            var userJson = jsonData[i];

            var inDate = userJson["owner_inDate"].ToString();

            // BaseStat
            var statData = JsonConvert.DeserializeObject<Dictionary<int, double>>(userJson["BaseStat"].ToString());

            // EquipItems
            var equipData =
                JsonConvert.DeserializeObject<Dictionary<EquipType, int>>(userJson["EquipItems"].ToString());

            // PromoGrade
            var promoGrade = int.Parse(userJson["PromoGrade"].ToString());

            // Passive Level
            var passiveLevel = 0;
            if (userJson.ContainsKey("Passive_Godgod_Lv"))
                passiveLevel = int.Parse(userJson["Passive_Godgod_Lv"].ToString());
            

            // Lv
            var lv = 0;

            if (userJson.ContainsKey("Lv"))
                lv = int.Parse(userJson["Lv"].ToString());

            var lastUpdateTime = DateTime.Parse(userJson["updatedAt"].ToString()).ToKstTime();

            if (!GuildMemberDatas.ContainsKey(inDate))
                Debug.LogError("Guild Member Not Contains Key");
            else
            {
                GuildMemberDatas[inDate].StatDatas = statData;
                GuildMemberDatas[inDate].EquipData = equipData;
                GuildMemberDatas[inDate].PromoGrade = promoGrade;
                GuildMemberDatas[inDate].Lv = lv;
                GuildMemberDatas[inDate].Passive_Godgod_Level = passiveLevel;
                GuildMemberDatas[inDate].LastConnectTime = lastUpdateTime;
            }
        }
    }

    private void OnBackendEnemyGuildMemberDataTransactionRead(BackendReturnObject bro)
    {
        var jsonData = bro.GetFlattenJSON()["Responses"];

        for (int i = 0; i < jsonData.Count; i++)
        {
            var userJson = jsonData[i];

            var inDate = userJson["owner_inDate"].ToString();

            // BaseStat
            var statData = JsonConvert.DeserializeObject<Dictionary<int, double>>(userJson["BaseStat"].ToString());

            // EquipItems
            var equipData =
                JsonConvert.DeserializeObject<Dictionary<EquipType, int>>(userJson["EquipItems"].ToString());

            // PromoGrade
            var promoGrade = int.Parse(userJson["PromoGrade"].ToString());

            // Lv
            var lv = 0;

            if (userJson.ContainsKey("Lv"))
                lv = int.Parse(userJson["Lv"].ToString());

            var passiveLevel = 0;
            if (userJson.ContainsKey("Passive_Godgod_Lv"))
                passiveLevel = int.Parse(userJson["Passive_Godgod_Lv"].ToString());

            if (!EnemyGuildMemberDatas.ContainsKey(inDate))
                Debug.LogError("Guild Member Not Contains Key");
            else
            {
                EnemyGuildMemberDatas[inDate].EnemyStatDatas = statData;
                EnemyGuildMemberDatas[inDate].EquipData = equipData;
                EnemyGuildMemberDatas[inDate].PromoGrade = promoGrade;
                EnemyGuildMemberDatas[inDate].Lv = lv;
                EnemyGuildMemberDatas[inDate].Passive_Godgod_Level = passiveLevel;
            }
        }
    }

    private bool CheckCompleteTransactionRead()
    {
        return _readCompleteCount >= _memberCount;
    }

    private bool CheckCompleteTransactionRead_Sports()
    {
        return _readCompleteCount_Sports >= _enemyMemberCount;
    }

    public void SetGuildRandomData()
    {
        Backend.RandomInfo.SetRandomData(RandomType.Guild, _guildRandomInDateDic[Managers.Server.CurrentServer],
            Managers.Server.CurrentServer,
            bro =>
            {
                if (!bro.IsSuccess())
                {
                    Managers.Backend.FailLog("Fail SetRandomData Guild", bro);
                    return;
                }
            });
    }

    public void GetGuildApplicants(Action callback, bool isAsync = true)
    {
        FadeScreen.Instance.OnLoadingScreen();

        if (isAsync)
        {
            Backend.Guild.GetApplicantsV3(OnBackendGetApplicants);
        }
        else
        {
            var bro = Backend.Guild.GetApplicantsV3();
            OnBackendGetApplicants(bro);
        }

        void OnBackendGetApplicants(BackendReturnObject bro)
        {
            if (!bro.IsSuccess())
            {
                Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                if (statusCode.Contains("403") && errorCode.Contains("ForbiddenException") &&
                    message.Contains("Forbidden selectApplicant"))
                    Managers.Message.ShowMessage("권한이 없습니다.");
                else
                {
                    Managers.Backend.FailLog("Fail Applicants", bro);
                }

                FadeScreen.Instance.OffLoadingScreen();
                callback?.Invoke();
                return;
            }

            var jsonData = bro.GetFlattenJSON()["rows"];

            if (jsonData.Count <= 0)
            {
                FadeScreen.Instance.OffLoadingScreen();
                callback?.Invoke();
                return;
            }

            List<TransactionValue> transactionValues = new();
            GuildApplicantsDatas.Clear();

            for (int i = 0; i < jsonData.Count; i++)
            {
                string inDate = jsonData[i]["inDate"].ToString();
                string nickname = jsonData[i]["nickname"].ToString();

                var where = new Where();

                where.Equal("owner_inDate", inDate);
                where.Equal("Server", Managers.Server.CurrentServer);

                transactionValues.Add(TransactionValue.SetGet("PvpInfo", where));

                GuildApplicantsDatas.TryAdd(inDate, new GuildMemberData()
                {
                    Nickname = nickname,
                    InDate = inDate
                });
            }

            int completeReadCount = 0;
            int checkReadCount = jsonData.Count;

            for (int i = 0; i < transactionValues.Count; i += 10)
            {
                var transactionList = transactionValues.GetRange(i, Mathf.Min(transactionValues.Count - i, 10));
                int readCount = transactionList.Count;

                Backend.GameData.TransactionReadV2(transactionList, readBro =>
                {
                    if (readBro.IsSuccess())
                    {
                        OnBackendApplicantsMemberDataTransactionRead(readBro);
                    }
                    else
                    {
                        Managers.Backend.FailLog("Fail Transaction Read", readBro);
                    }

                    completeReadCount += readCount;

                    if (completeReadCount < checkReadCount)
                        return;

                    FadeScreen.Instance.OffLoadingScreen();
                    callback?.Invoke();
                });
            }
        }
    }

    private void OnBackendApplicantsMemberDataTransactionRead(BackendReturnObject bro)
    {
        var jsonData = bro.GetFlattenJSON()["Responses"];

        for (int i = 0; i < jsonData.Count; i++)
        {
            var userJson = jsonData[i];

            var inDate = userJson["owner_inDate"].ToString();

            // BaseStat
            var statData = JsonConvert.DeserializeObject<Dictionary<int, double>>(userJson["BaseStat"].ToString());

            // EquipItems
            var equipData =
                JsonConvert.DeserializeObject<Dictionary<EquipType, int>>(userJson["EquipItems"].ToString());

            // PromoGrade
            var promoGrade = int.Parse(userJson["PromoGrade"].ToString());

            // Lv
            var lv = 0;

            if (userJson.ContainsKey("Lv"))
                lv = int.Parse(userJson["Lv"].ToString());

            if (!GuildApplicantsDatas.ContainsKey(inDate))
                GuildApplicantsDatas.Add(inDate, new GuildMemberData());

            GuildApplicantsDatas[inDate].StatDatas = statData;
            GuildApplicantsDatas[inDate].EquipData = equipData;
            GuildApplicantsDatas[inDate].PromoGrade = promoGrade;
            GuildApplicantsDatas[inDate].Lv = lv;
        }
    }
}