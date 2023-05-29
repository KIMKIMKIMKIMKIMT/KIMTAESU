using System;
using BackEnd;
using LitJson;

namespace GameData
{
    [Serializable]
    public class RankStageExtraData
    {
        public int Stage;
        public DateTime StageReachTime;

        public static string ToJsonData(int stage, DateTime stageReachTime)
        {
            var data = new RankStageExtraData()
            {
                Stage = stage,
                StageReachTime = stageReachTime
            };
            
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }
    }
    
    public class RankStageGameData : BaseRankGameData
    {
        public override string TableName => $"Ranking_Stage";
        protected override string InDate { get; set; }

        public override RankType RankType => RankType.Stage;
        
        private string Stage => "StageRankValue";
        private int MaxReachStage => Managers.Game.UserData.MaxReachStage + 1;

        protected override Param MakeInitData()
        {
            Param param = new Param()
            {
                {
                    Stage, MaxReachStage - GetTimeKey(Utils.GetNow())
                },
                {
                    "StageReachInfo", RankStageExtraData.ToJsonData(Managers.Game.UserData.MaxReachStage, Managers.Game.UserData.MaxReachStageTime)
                }
            };

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param()
            {
                {
                    Stage, MaxReachStage - GetTimeKey(Managers.Game.UserData.MaxReachStageTime)
                },
                {
                    "StageReachInfo", RankStageExtraData.ToJsonData(Managers.Game.UserData.MaxReachStage, Managers.Game.UserData.MaxReachStageTime)
                 }
            };

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            
        }
    }
}