using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using GameData;
using LitJson;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

public class RankData
{
    public readonly RankType RankType;
    public string GamerInDate;
    public string Nickname;
    public double Score;
    public int Rank;
    public float Time;

    public RankData()
    {
        GamerInDate = string.Empty;
        Nickname = string.Empty;
        Score = 0;
        Rank = -1;
    }

    public RankData(RankType rankType, JsonData jsonData)
    {
        RankType = rankType;
        GamerInDate = jsonData["gamerInDate"].ToString();
        Nickname = jsonData["nickname"].ToString();
        Rank = int.Parse(jsonData["rank"].ToString());
        Score = double.Parse(jsonData["score"].ToString());

        switch (RankType)
        {
            case RankType.Stage:
            {
                if (jsonData.ContainsKey("StageReachInfo"))
                {
                    RankStageExtraData data =
                        JsonConvert.DeserializeObject<RankStageExtraData>(jsonData["StageReachInfo"].ToString());

                    if (data != null)
                        Score = data.Stage;
                }
            }
                break;
            case RankType.Raid:
            {
                if (jsonData.ContainsKey("ClearInfo"))
                {
                    RaidClearInfo raidClearInfo =
                        JsonConvert.DeserializeObject<RaidClearInfo>(jsonData["ClearInfo"].ToString());

                    if (raidClearInfo != null)
                    {
                        Score = raidClearInfo.ClearStep;
                        Time = raidClearInfo.ClearTime;
                    }
                }
            }
                break;
            case RankType.Pvp:
                break;
            case RankType.Guild:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public class GuildRankData
{
    public string GuildName;
    public string GuildInDate;
    public double Score;
    public int Rank;

    public GuildRankData()
    {
        GuildName = string.Empty;
        GuildInDate = string.Empty;
        Score = 0;
        Rank = 0;
    }

    public GuildRankData(JsonData jsonData)
    {
        GuildName = jsonData["guildName"].ToString();
        GuildInDate = jsonData["guildInDate"].ToString();
        Score = double.Parse(jsonData["score"].ToString());
        Rank = int.Parse(jsonData["rank"].ToString());
    }
}

public class GuildAllRaidRankData
{
    public string GuildName;
    public string GuildInDate;
    public double Score;
    public int Rank;

    public GuildAllRaidRankData()
    {
        GuildName = string.Empty;
        GuildInDate = string.Empty;
        Score = 0;
        Rank = 0;
    }

    public GuildAllRaidRankData(JsonData jsonData)
    {
        GuildName = jsonData["guildName"].ToString();
        GuildInDate = jsonData["guildInDate"].ToString();
        Score = double.Parse(jsonData["score"].ToString());
        Rank = int.Parse(jsonData["rank"].ToString());
    }
}

public class GuildSportsRankData
{
    public string GuildName;
    public string GuildInDate;
    public double Score;
    public int Rank;

    public GuildSportsRankData()
    {
        GuildName = string.Empty;
        GuildInDate = string.Empty;
        Score = 0;
        Rank = 0;
    }

    public GuildSportsRankData(JsonData jsonData)
    {
        GuildName = jsonData["guildName"].ToString();
        GuildInDate = jsonData["guildInDate"].ToString();
        Score = double.Parse(jsonData["score"].ToString());
        Rank = int.Parse(jsonData["rank"].ToString());
    }
}

public class RankRewardData
{
    public int StartRank;
    public int EndRank;
    public ItemType ItemType;
    public int ItemId;
    public double ItemValue;

    public RankRewardData(JsonData rewardJsonData)
    {
        StartRank = int.Parse(rewardJsonData["startRank"].ToString());
        EndRank = int.Parse(rewardJsonData["endRank"].ToString());
        ItemType = Enum.Parse<ItemType>(rewardJsonData["rewardItems"]["Item_Type"].ToString());
        ItemId = int.Parse(rewardJsonData["rewardItems"]["Item_Id"].ToString());
        ItemValue = int.Parse(rewardJsonData["rewardItemCount"].ToString());
    }

    public string GetRankString()
    {
        if (StartRank == 0 && EndRank == 0)
            return "전체 보상";

        if (StartRank == EndRank)
            return $"{StartRank}위";

        return $"{StartRank}~{EndRank}위";
    }
}

public class RankManager
{
    // Key : (RankType, Server)
    public readonly Dictionary<(RankType, int), string> RankUUIDDictionary = new()
    {
        { (RankType.Stage, 1), "ea73a1a0-34cd-11ed-834b-03bc80c6df49" },
        { (RankType.Stage, 2), "ccc5e490-64db-11ed-b7c1-df738fed23fb" },
        { (RankType.Pvp, 1), "170312c0-4377-11ed-b91a-25fdc7e5cc6c" },
        { (RankType.Pvp, 2), "22da3b50-4377-11ed-b91a-25fdc7e5cc6c" },
        { (RankType.Raid, 1), "6b5341f0-752d-11ed-a50e-5beec811ecdf" },
        { (RankType.Raid, 2), "ff54fca0-a295-11ed-9369-0b81f15a74bf" },
        { (RankType.Guild, 1), "6986ab70-a073-11ed-a8b3-fd673406d951" },
        { (RankType.Guild, 2), "466f0b30-a296-11ed-9369-0b81f15a74bf" },
        { (RankType.GuildAllRaid, 1), "c999dc00-d8d9-11ed-8b0a-6fa2f1120298" },
        { (RankType.GuildAllRaid, 2), "dd986b90-d8d9-11ed-b063-319cfd6951d8" },
        { (RankType.GuildSports, 1), "951ad810-f9d8-11ed-9b75-33bc5626c5fa" },
        { (RankType.GuildSports, 2), "a6904bc0-f9d8-11ed-b14c-5d3404a48f30" }
    };

    public readonly List<RankRewardData> RankRewardDatas = new();
    public readonly List<RankRewardData> GuildRankRewardDatas = new();
    public readonly List<RankRewardData> GuildSportsRankRewardDatas = new();

    public readonly Dictionary<RankType, RankData> MyRankDatas = new();
    public readonly Dictionary<RankType, List<RankData>> RankDatas = new();
    public readonly List<GuildRankData> GuildRankDatas = new();
    public readonly List<GuildAllRaidRankData> GuildAllRaidRankDatas = new();
    public readonly List<GuildSportsRankData> GuildSportsRankDatas = new();
    public readonly Dictionary<int, GuildData> RankerGuildData = new();
    public readonly Dictionary<int, GuildData> RankerGuildAllRaidData = new();
    public readonly Dictionary<int, GuildData> RankerGuildSportsData = new();

    public readonly Dictionary<(RankType, int, EquipType), int> RankerEquipDatas = new();

    public readonly Dictionary<string, string> RankerGuildDic = new();
    public readonly Dictionary<RankType, bool> RankerGuildLoadFlagDic = new();
    public readonly Dictionary<RankType, CompositeDisposable> RankerGuildLoadCompositeDic = new();
    
    private bool _isRefreshRankingFlag = true;
    private bool _isRefreshGuildRankFlag = true;
    private bool _isRefreshGuildAllRaidRankFlag = true;
    private bool _isRefreshGuildSportsFlag = true;

    public bool IsRefreshRankStageFlag = true;
    public bool IsRefreshRankRaidFlag = true;

    public void CheckDev()
    {
        if (Managers.Manager.ProjectType == ProjectType.Dev)
        {
            // 주간
            RankUUIDDictionary.TryAdd((RankType.Stage, 100), "fa030090-5424-11ed-828e-d9926994bc8e");

            // 일간
            RankUUIDDictionary.TryAdd((RankType.Pvp, 100), "784d6fa0-6959-11ed-80c7-9597a2b5a986");
            RankUUIDDictionary.TryAdd((RankType.Raid, 100), "46f94a10-7123-11ed-8276-bb59c839bc65");
            RankUUIDDictionary.TryAdd((RankType.Guild, 100), "498c6510-a057-11ed-882c-e1afaf991840");
            RankUUIDDictionary.TryAdd((RankType.GuildAllRaid, 100), "f9d3f770-d46f-11ed-a8d7-850c514a6b0c");
            RankUUIDDictionary.TryAdd((RankType.GuildSports, 100), "27191920-f0b0-11ed-bb95-69510e72dda2");
        }
    }

    public void Init()
    {
        //UpdateAllRankData();

        // Observable.Timer(TimeSpan.FromMinutes(10)).RepeatUntilDestroy(Managers.Manager)
        //     .Subscribe(_ => UpdateAllRankData());

        // UpdateMyRankScore(RankType.Stage);
        // UpdateMyRankScore(RankType.Pvp);
        MainThreadDispatcher.StartCoroutine(CoRefreshRankScore());
        
        Backend.URank.User.GetRankRewardList(RankUUIDDictionary[(RankType.Stage, 1)], bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog("Fail GetRankRewardList", bro);
                return;
            }

            JsonData jsonData = bro.GetFlattenJSON();

            for (int i = 0; i < jsonData["rows"].Count; i++)
            {
                RankRewardData rankRewardData = new RankRewardData(jsonData["rows"][i]);
                RankRewardDatas.Add(rankRewardData);
            }
        });

        Backend.URank.User.GetRankRewardList(RankUUIDDictionary[(RankType.Guild, 1)], bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog("Fail GetRankRewardList", bro);
                return;
            }

            JsonData jsonData = bro.GetFlattenJSON();

            for (int i = 0; i < jsonData["rows"].Count; i++)
            {
                RankRewardData rankRewardData = new RankRewardData(jsonData["rows"][i]);
                GuildRankRewardDatas.Add(rankRewardData);
            }
        });

        Backend.URank.User.GetRankRewardList(RankUUIDDictionary[(RankType.GuildSports, 1)], bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog("Fail GetRankRewardList", bro);
                return;
            }

            JsonData jsonData = bro.GetFlattenJSON();

            for (int i = 0; i < jsonData["rows"].Count; i++)
            {
                RankRewardData rankRewardData = new RankRewardData(jsonData["rows"][i]);
                GuildSportsRankRewardDatas.Add(rankRewardData);
            }
        });
    }

    public List<RankData> GetRankList(int server, RankType rankType, int limit = 10, int startRank = 0)
    {
        var rankDatas = new List<RankData>();
        var baseRankGameData = GameDataManager.GetRankGameData(rankType);

        if (baseRankGameData != null)
            rankDatas = baseRankGameData.GetRankList(server, rankType, limit, startRank);

        if (RankDatas.ContainsKey(rankType))
            RankDatas[rankType] = rankDatas;
        else
            RankDatas.Add(rankType, rankDatas);

        return rankDatas;
    }

    public void GetRankList(RankType rankType, Action<List<RankData>> endCallback, int limit = 10,
        int startRank = 0, bool isUpdateList = true)
    {
        GameDataManager.GetRankGameData(rankType)?.GetRankList(limit, startRank, endCallback, isUpdateList);
    }

    public void UpdateRankingList(RankType rankType, Action endCallback, int limit = 10,
        int startRank = 0)
    {
        GetRankList(rankType, rankData => endCallback.Invoke(), limit, startRank);
    }

    /// <summary>
    /// 내 랭킹 데이터 조회
    /// </summary>
    /// <param name="rankType"></param>
    /// <param name="endCallback"></param>
    public void UpdateMyRanking(RankType rankType, Action endCallback)
    {
        GameDataManager.GetRankGameData(rankType)?.GetMyRankData(rankData => endCallback?.Invoke());
    }

    public void UpdateRankerEquipData(RankType rankType, Action endCallback)
    {
        List<string> ownerInDates = new List<string>();

        for (int i = 0; i < 3; i++)
        {
            if (RankDatas[rankType].Count <= i)
                continue;

            ownerInDates.Add(RankDatas[rankType][i].GamerInDate);
        }

        GameDataManager.EquipGameData.GetGameDatas(ownerInDates, jsonData =>
        {
            if (jsonData == null)
            {
                endCallback?.Invoke();
                return;
            }

            for (int i = 0; i < jsonData["Responses"].Count; i++)
            {
                foreach (EquipType equipType in Enum.GetValues(typeof(EquipType)))
                {
                    int equipIndex = 0;

                    if (!jsonData["Responses"][i].ContainsKey(equipType.ToString()) ||
                        !int.TryParse(jsonData["Responses"][i][equipType.ToString()].ToString(), out equipIndex))
                    {
                        switch (equipType)
                        {
                            case EquipType.Weapon:
                                equipIndex = (int)ChartManager.SystemCharts[SystemData.Default_Weapon].Value;
                                break;
                            case EquipType.Costume:
                                equipIndex = (int)ChartManager.SystemCharts[SystemData.Default_Costume].Value;
                                break;
                            case EquipType.ShowCostume:
                            {
                                if (RankerEquipDatas.TryGetValue((rankType, i, EquipType.Costume),
                                        out var rankerEquipData))
                                    equipIndex = rankerEquipData;
                                else if (jsonData["Responses"][i].ContainsKey(EquipType.Costume.ToString()))
                                    equipIndex = int.Parse(jsonData["Responses"][i][EquipType.Costume.ToString()]
                                        .ToString());
                                else
                                    equipIndex = (int)ChartManager.SystemCharts[SystemData.Default_Costume].Value;
                            }
                                break;
                            case EquipType.Pet:
                                equipIndex = (int)ChartManager.SystemCharts[SystemData.Default_Pet].Value;
                                break;
                            
                        }
                    }

                    if (RankerEquipDatas.ContainsKey((rankType, i, equipType)))
                        RankerEquipDatas[(rankType, i, equipType)] = 0;
                    else
                        RankerEquipDatas.Add((rankType, i, equipType), equipIndex);
                }
            }

            endCallback?.Invoke();
        });
    }

    public void RefreshRankingListData(Action callback)
    {
        if (!_isRefreshRankingFlag)
        {
            callback?.Invoke();
            return;
        }

        _isRefreshRankingFlag = false;
        Observable.Timer(TimeSpan.FromMinutes(10)).Subscribe(_ => _isRefreshRankingFlag = true);

        FadeScreen.Instance.OnLoadingScreen();

        RefreshRankingListData(RankType.Stage, () =>
        {
            RefreshRankingListData(RankType.Pvp, () =>
            {
                RefreshRankingListData(RankType.Raid, () =>
                {
                    
                    FadeScreen.Instance.OffLoadingScreen();
                    callback?.Invoke();
                });
            });
        });
    }

    private void RefreshRankingListData(RankType rankType, Action onSuccess)
    {
        Managers.Rank.UpdateRankingList(rankType, () =>
        {
            var myRankData = RankDatas[rankType].Find(rankData => rankData.GamerInDate == Backend.UserInDate);
            if (myRankData == null)
            {
                Managers.Rank.UpdateMyRanking(rankType, () =>
                {
                    Managers.Rank.UpdateRankerEquipData(rankType,
                        () =>
                        {
                            onSuccess?.Invoke();
                        }); 
                });
            }
            else
            {
                if (MyRankDatas.ContainsKey(rankType))
                    MyRankDatas[rankType] = myRankData;
                else
                    MyRankDatas.Add(rankType, myRankData);
                
                Managers.Rank.UpdateRankerEquipData(rankType,
                    () =>
                    {
                        onSuccess?.Invoke();
                    }); 
            }
        }, 100);
    }

    private void UpdateMyRankScore(RankType rankType, Action callback)
    {
        var rankGameData = GameDataManager.GetRankGameData(rankType);
        if (rankGameData == null)
        {
            Debug.Log($"Fail Find RankGameData - {rankType.ToString()}");
            return;
        }

        rankGameData.UpdateMyRankScore(callback);
    }

    public void GetRankerDic(RankType rankType, Action callback)
    {
        if (RankerGuildLoadFlagDic.TryGetValue(rankType, out bool loadFlag))
        {
            if (!loadFlag)
            {
                callback?.Invoke();
                return;
            }

            SetFlag();
        }
        else
            SetFlag();

        void SetFlag()
        {
            if (RankerGuildLoadCompositeDic.TryGetValue(rankType, out var compositeDisposable))
                compositeDisposable.Clear();
            else
            {
                compositeDisposable = new CompositeDisposable();
                RankerGuildLoadCompositeDic.Add(rankType, compositeDisposable);
            }

            RankerGuildLoadFlagDic[rankType] = false;
            Observable.Timer(TimeSpan.FromMinutes(10)).Subscribe(_ => RankerGuildLoadFlagDic[rankType] = true)
                .AddTo(compositeDisposable);
        }

        List<string> loadInDateList = new();

        for (int rank = 1; rank <= 3; rank++)
        {
            var userRankData = RankDatas[rankType].Find(rankData => rankData.Rank == rank);

            if (userRankData != null)
                loadInDateList.Add(userRankData.GamerInDate);
        }

        int loadCompleteCount = 0;

        if (loadInDateList.Count <= 0)
        {
            callback?.Invoke();
            return;
        }

        foreach (var inDate in loadInDateList)
        {
            Backend.Social.GetUserInfoByInDate(inDate, bro =>
            {
                if (bro.IsSuccess())
                {
                    var jsonData = bro.GetReturnValuetoJSON();

                    string userInDate = jsonData["row"]["inDate"].ToString();
                    string userGuild = jsonData["row"]["guildName"] != null
                        ? jsonData["row"]["guildName"].ToString()
                        : string.Empty;

                    if (RankerGuildDic.ContainsKey(userInDate))
                        RankerGuildDic[userInDate] = userGuild;
                    else
                        RankerGuildDic.Add(userInDate, userGuild);
                }
                else
                    Managers.Backend.FailLog("Fail GetUserInfoByInDate", bro);

                loadCompleteCount += 1;
                if (loadCompleteCount < loadInDateList.Count)
                    return;

                callback?.Invoke();
            });
        }
    }

    public void GetGuildRankList(Action callback, RankType type = RankType.Guild)
    {
        if (_isRefreshGuildRankFlag)
        {
            _isRefreshGuildRankFlag = false;
            
            Observable.Timer(TimeSpan.FromMinutes(10)).Subscribe(_ => _isRefreshGuildRankFlag = true);

            FadeScreen.Instance.OnLoadingScreen();

            Backend.URank.Guild.GetRankList(RankUUIDDictionary[(type, Managers.Server.CurrentServer)],
                bro =>
                {
                    if (!bro.IsSuccess())
                    {
                        Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                        // uuid가 null 혹은 string.Empty인 경우
                        if (statusCode.Contains("400") && errorCode.Contains("ValidationException") &&
                            message.Contains("rankUuid"))
                            Managers.Message.ShowMessage("잘못된 호출 입니다.");
                        // 존재하지 않는 uuid 일 때
                        else if (statusCode.Contains("404") && errorCode.Contains("NotFoundException") &&
                                 message.Contains("rank not found"))
                            Managers.Message.ShowMessage("존재하지 않는 랭킹 입니다.");
                        else
                            Managers.Backend.FailLog(bro, "Fail Guild GetRankList");

                        FadeScreen.Instance.OffLoadingScreen();
                        callback?.Invoke();
                        return;
                    }

                    var jsonData = bro.GetFlattenJSON()["rows"];

                    GuildRankDatas.Clear();

                    List<(int, string)> rankerGuildList = new();

                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        var guildRankData = new GuildRankData(jsonData[i]);
                        GuildRankDatas.Add(guildRankData);
                        
                        if (guildRankData.Rank >= 1 && guildRankData.Rank <= 3)
                            rankerGuildList.Add((guildRankData.Rank, guildRankData.GuildInDate));
                    }

                    int loadCompleteCount = 0;

                    if (rankerGuildList.Count > 0)
                    {
                        foreach (var (rank, guildInDate) in rankerGuildList)
                        {
                            Backend.Guild.GetGuildInfoV3(guildInDate, bro2 =>
                            {
                                if (bro2.IsSuccess())
                                {
                                    GuildData guildData = Managers.Guild.ParseGuildData(bro2);
                                
                                    if (RankerGuildData.ContainsKey(rank))
                                        RankerGuildData[rank] = guildData;
                                    else
                                        RankerGuildData.Add(rank, guildData);
                                }
                                else
                                    Managers.Backend.FailLog(bro2, "Fail GetGuildInfo");

                                loadCompleteCount += 1;
                                
                                if (loadCompleteCount < rankerGuildList.Count)
                                    return;

                                EndCallback();
                            });
                        }
                    }
                    else
                    {
                        EndCallback();
                    }

                    void EndCallback()
                    {
                        FadeScreen.Instance.OffLoadingScreen();
                        callback?.Invoke();
                    }
                });
        }
        else
            callback?.Invoke();
    }

    public void GetGuildAllRaidRankList(Action callback, RankType type = RankType.GuildAllRaid)
    {
        if (_isRefreshGuildAllRaidRankFlag)
        {
            _isRefreshGuildAllRaidRankFlag = false;

            Observable.Timer(TimeSpan.FromMinutes(10)).Subscribe(_ => _isRefreshGuildAllRaidRankFlag = true);

            FadeScreen.Instance.OnLoadingScreen();

            Backend.URank.Guild.GetRankList(RankUUIDDictionary[(type, Managers.Server.CurrentServer)],
                bro =>
                {
                    if (!bro.IsSuccess())
                    {
                        Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                        // uuid가 null 혹은 string.Empty인 경우
                        if (statusCode.Contains("400") && errorCode.Contains("ValidationException") &&
                            message.Contains("rankUuid"))
                            Managers.Message.ShowMessage("잘못된 호출 입니다.");
                        // 존재하지 않는 uuid 일 때
                        else if (statusCode.Contains("404") && errorCode.Contains("NotFoundException") &&
                                 message.Contains("rank not found"))
                            Managers.Message.ShowMessage("존재하지 않는 랭킹 입니다.");
                        else
                            Managers.Backend.FailLog(bro, "Fail Guild GetRankList");

                        FadeScreen.Instance.OffLoadingScreen();
                        callback?.Invoke();
                        return;
                    }

                    var jsonData = bro.GetFlattenJSON()["rows"];

                    GuildAllRaidRankDatas.Clear();

                    List<(int, string)> rankerGuildAllRaidList = new();

                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        var guildAllRaidRankData = new GuildAllRaidRankData(jsonData[i]);
                        GuildAllRaidRankDatas.Add(guildAllRaidRankData);

                        if (guildAllRaidRankData.Rank >= 1 && guildAllRaidRankData.Rank <= 3)
                            rankerGuildAllRaidList.Add((guildAllRaidRankData.Rank, guildAllRaidRankData.GuildInDate));
                    }

                    int loadCompleteCount = 0;

                    if (rankerGuildAllRaidList.Count > 0)
                    {
                        foreach (var (rank, guildInDate) in rankerGuildAllRaidList)
                        {
                            Backend.Guild.GetGuildInfoV3(guildInDate, bro2 =>
                            {
                                if (bro2.IsSuccess())
                                {
                                    GuildData guildData = Managers.Guild.ParseGuildData(bro2);

                                    if (RankerGuildAllRaidData.ContainsKey(rank))
                                        RankerGuildAllRaidData[rank] = guildData;
                                    else
                                        RankerGuildAllRaidData.Add(rank, guildData);
                                }
                                else
                                    Managers.Backend.FailLog(bro2, "Fail GetGuildInfo");

                                loadCompleteCount += 1;

                                if (loadCompleteCount < rankerGuildAllRaidList.Count)
                                    return;

                                EndCallback();
                            });
                        }
                    }
                    else
                    {
                        EndCallback();
                    }

                    void EndCallback()
                    {
                        FadeScreen.Instance.OffLoadingScreen();
                        callback?.Invoke();
                    }
                });
        }
        else
            callback?.Invoke();
    }

    public void GetGuildSportsRankList(Action callback, RankType type = RankType.GuildSports)
    {
        if (_isRefreshGuildSportsFlag)
        {
            _isRefreshGuildSportsFlag = false;

            Observable.Timer(TimeSpan.FromMinutes(10)).Subscribe(_ => _isRefreshGuildSportsFlag = true);

            FadeScreen.Instance.OnLoadingScreen();

            Backend.URank.Guild.GetRankList(RankUUIDDictionary[(type, Managers.Server.CurrentServer)],
                bro =>
                {
                    if (!bro.IsSuccess())
                    {
                        Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                        // uuid가 null 혹은 string.Empty인 경우
                        if (statusCode.Contains("400") && errorCode.Contains("ValidationException") &&
                            message.Contains("rankUuid"))
                            Managers.Message.ShowMessage("잘못된 호출 입니다.");
                        // 존재하지 않는 uuid 일 때
                        else if (statusCode.Contains("404") && errorCode.Contains("NotFoundException") &&
                                 message.Contains("rank not found"))
                            Managers.Message.ShowMessage("존재하지 않는 랭킹 입니다.");
                        else
                            Managers.Backend.FailLog(bro, "Fail Guild GetRankList");

                        FadeScreen.Instance.OffLoadingScreen();
                        callback?.Invoke();
                        return;
                    }

                    var jsonData = bro.GetFlattenJSON()["rows"];

                    GuildSportsRankDatas.Clear();

                    List<(int, string)> rankerGuildSportsList = new();

                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        var guildSportsRankData = new GuildSportsRankData(jsonData[i]);
                        GuildSportsRankDatas.Add(guildSportsRankData);

                        if (guildSportsRankData.Rank >= 1 && guildSportsRankData.Rank <= 3)
                            rankerGuildSportsList.Add((guildSportsRankData.Rank, guildSportsRankData.GuildInDate));
                    }

                    int loadCompleteCount = 0;

                    if (rankerGuildSportsList.Count > 0)
                    {
                        foreach (var (rank, guildInDate) in rankerGuildSportsList)
                        {
                            Backend.Guild.GetGuildInfoV3(guildInDate, bro2 =>
                            {
                                if (bro2.IsSuccess())
                                {
                                    GuildData guildData = Managers.Guild.ParseGuildData(bro2);

                                    if (RankerGuildSportsData.ContainsKey(rank))
                                        RankerGuildSportsData[rank] = guildData;
                                    else
                                        RankerGuildSportsData.Add(rank, guildData);
                                }
                                else
                                    Managers.Backend.FailLog(bro2, "Fail GetGuildInfo");

                                loadCompleteCount += 1;

                                if (loadCompleteCount < rankerGuildSportsList.Count)
                                    return;

                                EndCallback();
                            });
                        }
                    }
                    else
                    {
                        EndCallback();
                    }

                    void EndCallback()
                    {
                        FadeScreen.Instance.OffLoadingScreen();
                        callback?.Invoke();
                    }
                });
        }
        else
            callback?.Invoke();
    }

    // 10분 단위로 변화플래그 체크해서 랭킹 갱신
    private IEnumerator CoRefreshRankScore()
    {
        var delay = new WaitForSeconds(600f);

        while (true)
        {
            if (IsRefreshRankStageFlag && !Utils.IsWeeklyCalculateRankTime())
            {
                UpdateMyRankScore(RankType.Stage, () => UpdateMyRanking(RankType.Stage, null));
            }

            if (IsRefreshRankRaidFlag && !Utils.IsWeeklyCalculateRankTime())
            {
                UpdateMyRankScore(RankType.Raid, () => UpdateMyRanking(RankType.Raid, null));
            }

            yield return delay;
        }
    }
}