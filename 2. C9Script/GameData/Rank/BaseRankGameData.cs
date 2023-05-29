using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;

namespace GameData
{
    public class RankRewardData
    {
        public int StartRank;
        public int EndRank;
    }

    public abstract class BaseRankGameData : BaseGameData
    {
        protected string RankUUID => Managers.Rank.RankUUIDDictionary[(RankType, Managers.Server.CurrentServer)];
        public abstract RankType RankType { get; }
        protected string RankingServerTableName => $"{TableName}_Server{Managers.Server.CurrentServer}";
        protected string ServerTableNameKey(int server) => $"{TableName}_Server{server}";

        protected override void InitGameData()
        {
            InitGameData(RankingServerTableName, MakeInitData());
        }

        public void GetMyRankData(Action<RankData> endCallback)
        {
            Backend.URank.User.GetMyRank(RankUUID, bro =>
            {
                if (!bro.IsSuccess())
                {
                    // 자신의 랭킹이 존재하지 않는 경우
                    if (bro.GetStatusCode().Equals("404") &&
                        bro.GetErrorCode().Equals("NotFoundException") &&
                        bro.GetMessage().Contains("userRank not found"))
                    {
                        if (Utils.IsCalculateRankTime() || Utils.IsWhiteList())
                        {
                            if ((Managers.Manager.ProjectType == ProjectType.Live &&
                                 Utils.GetNow().DayOfWeek == DayOfWeek.Monday) ||
                                Managers.Manager.ProjectType == ProjectType.Dev)
                            {
                                if (TableName.Contains("PVP"))
                                    Managers.Pvp.PvpScore = 0;

                                endCallback?.Invoke(new RankData()
                                {
                                    GamerInDate = Backend.UserInDate,
                                    Nickname = Backend.UserNickName,
                                    Rank = 0,
                                    Score = 0,
                                });
                                return;
                            }
                        }

                        LoadGameDataNow();
                        UpdateMyRankData(false);

                        if (Utils.IsWhiteList())
                        {
                            endCallback?.Invoke(new RankData()
                            {
                                GamerInDate = Backend.UserInDate,
                                Nickname = Backend.UserNickName,
                                Rank = 0,
                                Score = 0,
                            });
                        }
                        else
                            GetMyRankData(endCallback);
                        
                        return;
                    }

                    Managers.Backend.FailLog("Fail GetMyRank", bro);
                    return;
                }

                var jsonData = bro.GetFlattenJSON()["rows"];

                RankData rankData = new RankData(RankType, jsonData[0]);

                if (Managers.Rank.MyRankDatas.ContainsKey(RankType))
                    Managers.Rank.MyRankDatas[RankType] = rankData;
                else
                    Managers.Rank.MyRankDatas.Add(RankType, rankData);

                endCallback?.Invoke(rankData);
            });
        }

        public List<RankData> GetRankList(int limit, int startRank)
        {
            var bro = Backend.URank.User.GetRankList(RankUUID, limit, startRank);

            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog($"Fail GetRankData {RankingServerTableName}", bro);
                return new List<RankData>();
            }

            JsonData jsonData = bro.GetFlattenJSON();

            List<RankData> rankDatas = new List<RankData>();

            for (int i = 0; i < jsonData["rows"].Count; i++)
            {
                RankData rankData = new RankData(RankType, jsonData["rows"][i]);
                rankDatas.Add(rankData);
            }

            return rankDatas;
        }

        public List<RankData> GetRankList(int server, RankType rankType, int limit, int startRank)
        {
            if (!Managers.Rank.RankUUIDDictionary.TryGetValue((rankType, server), out var uuid))
                return new List<RankData>();

            var bro = Backend.URank.User.GetRankList(uuid, limit, startRank);

            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog($"Fail GetRankData {uuid}", bro);
                return new List<RankData>();
            }

            JsonData jsonData = bro.GetFlattenJSON();

            List<RankData> rankDatas = new List<RankData>();

            for (int i = 0; i < jsonData["rows"].Count; i++)
            {
                RankData rankData = new RankData(RankType, jsonData["rows"][i]);

                rankDatas.Add(rankData);
            }

            return rankDatas;
        }

        public void GetRankList(int limit, int startRank, Action<List<RankData>> endCallback, bool isUpdateList = true)
        {
            Backend.URank.User.GetRankList(RankUUID, limit, startRank, bro =>
            {
                if (!bro.IsSuccess())
                {
                    Managers.Backend.FailLog($"Fail GetRankData {RankingServerTableName}", bro);
                    return;
                }

                JsonData jsonData = bro.GetFlattenJSON();

                List<RankData> rankDatas = new List<RankData>();

                for (int i = 0; i < jsonData["rows"].Count; i++)
                {
                    RankData rankData = new RankData(RankType, jsonData["rows"][i]);
                    rankDatas.Add(rankData);
                }

                if (isUpdateList)
                {
                    if (Managers.Rank.RankDatas.ContainsKey(RankType))
                        Managers.Rank.RankDatas[RankType].Clear();
                    
                    if (Managers.Rank.RankDatas.ContainsKey(RankType))
                        Managers.Rank.RankDatas[RankType] = rankDatas;
                    else
                        Managers.Rank.RankDatas.Add(RankType, rankDatas);
                }

                endCallback?.Invoke(rankDatas);
            });
        }

        public override void LoadGameData()
        {
            if (string.IsNullOrEmpty(InDate))
            {
                Backend.GameData.GetMyData(RankingServerTableName, new Where(), GetDataCallback);
            }
            else
            {
                Backend.GameData.GetMyData(RankingServerTableName, InDate, GetDataCallback);
            }
        }
        
        public void LoadGameDataNow()
        {
            if (string.IsNullOrEmpty(InDate))
            {
                var bro = Backend.GameData.GetMyData(RankingServerTableName, new Where());
                GetDataCallback(bro);
            }
            else
            {
                var bro = Backend.GameData.GetMyData(RankingServerTableName, InDate);
                GetDataCallback(bro);
            }
        }

        public override void SaveGameData(Param param, bool isSaveImmediately = false, Action callback = null)
        {
            if (Utils.IsWhiteList())
                return;
            
            if (!Utils.IsCalculateRankTime())
                Backend.URank.User.UpdateUserScore(RankUUID, RankingServerTableName, InDate, param);
        }

        public void UpdateMyRankData(bool isAsync = true, Action endCallback = null)
        {
            if (Utils.IsWhiteList())
            {
                endCallback?.Invoke();
                return;
            }
            
            if (isAsync)
            {
                Backend.URank.User.UpdateUserScore(RankUUID, RankingServerTableName, InDate, MakeSaveData(), bro =>
                {
                    if (!bro.IsSuccess())
                    {
                        if (Utils.IsBackendCalculateRankTimeError(bro))
                        {
                            if (TableName.Contains("Ranking_PVP") && Utils.GetNow().DayOfWeek == DayOfWeek.Monday)
                                LoadGameData();
                            
                            endCallback?.Invoke();
                            return;
                        }
                        
                        Managers.Backend.FailLog($"Fail UpdateMyRankData {RankingServerTableName}", bro);
                        return;
                    }

                    Debug.Log($"Success {RankingServerTableName} UpdateMyRankData");
                    endCallback?.Invoke();
                });
            }
            else
            {
                var bro = Backend.URank.User.UpdateUserScore(RankUUID, RankingServerTableName, InDate, MakeSaveData());

                if (!bro.IsSuccess())
                {
                    if (Utils.IsBackendCalculateRankTimeError(bro))
                    {
                        if (TableName.Contains("Ranking_PVP") && Utils.GetNow().DayOfWeek == DayOfWeek.Monday)
                            LoadGameData();
                        
                        endCallback?.Invoke();
                        return;
                    }
                    
                    Managers.Backend.FailLog($"Fail UpdateMyRankData {RankingServerTableName}", bro);
                    return;
                }

                Debug.Log($"Success {RankingServerTableName} UpdateMyRankData");
            }
        }

        public void UpdateMyRankScore(Action callback)
        {
            if (Utils.IsWhiteList())
                return;
            
            Backend.URank.User.UpdateUserScore(
                Managers.Rank.RankUUIDDictionary[(RankType, Managers.Server.CurrentServer)],
                RankingServerTableName,
                InDate,
                MakeSaveData(),
                bro =>
                {
                    if (bro.IsSuccess())
                    {
                        switch (RankType)
                        {
                            case RankType.Stage:
                                Managers.Rank.IsRefreshRankStageFlag = false;
                                break;
                            case RankType.Raid:
                                Managers.Rank.IsRefreshRankRaidFlag = false;
                                break;
                        }
                        
                        callback?.Invoke();
                    }
                }
            );
        }

        public override void GetGameDatas(int server, List<string> ownerInDates, Action<JsonData> endCallback)
        {
            List<TransactionValue> transactionActions = new List<TransactionValue>();

            ownerInDates.ForEach(inDate =>
            {
                Where where = new Where();
                where.Equal("owner_inDate", inDate);

                transactionActions.Add(TransactionValue.SetGet(ServerTableNameKey(server), where));
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

        public override JsonData GetGameDatas(int server, List<string> ownerInDates)
        {
            List<TransactionValue> transactionActions = new List<TransactionValue>();

            ownerInDates.ForEach(inDate =>
            {
                Where where = new Where();
                where.Equal("owner_inDate", inDate);

                transactionActions.Add(TransactionValue.SetGet(ServerTableNameKey(server), where));
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


        protected double GetTimeKey(DateTime dateTime)
        {
            return double.Parse($"0.{Math.Round((dateTime - new DateTime(2012, 9, 1)).TotalMilliseconds)}");
        }
    }
}