using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;

namespace GameData
{
    public class RaidClearInfo
    {
        private ObscuredInt _clearStep;
        public int ClearStep
        {
            get => _clearStep;
            set => _clearStep = value;
        }

        private ObscuredFloat _clearTime;
        public float ClearTime
        {
            get => _clearTime;
            set => _clearTime = value;
        }

        public double GetScore()
        {
            return ClearStep + 1 - (ClearTime * 0.0000001);
        }

        private ObscuredDouble _highestGold;
        public double HighestGold
        {
            get => _highestGold;
            set => _highestGold = value;
        }

        private ObscuredDouble _highestGoldBar;
        public double HighestGoldBar
        {
            get => _highestGoldBar;
            set => _highestGoldBar = value;
        }

        private ObscuredDouble _highestSkillStone;
        public double HighestSkillStone
        {
            get => _highestSkillStone;
            set => _highestSkillStone = value;
        }

    }
    
    public class RankRaidGameData : BaseRankGameData
    {
        public override string TableName => "Ranking_Raid";
        public override RankType RankType => RankType.Raid;
        protected override string InDate { get; set; }

        private const string Score = "Score";
        private const string ClearInfo = "ClearInfo";
        protected override Param MakeInitData()
        {
            Param param = new Param();
            
            param.Add(Score, 0);
            param.Add(ClearInfo, string.Empty);

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();
            
            param.Add(Score, Managers.Raid.RaidClearInfo != null ?
                Managers.Raid.RaidClearInfo.GetScore() :
                0);
            param.Add(ClearInfo,
                Managers.Raid.RaidClearInfo != null
                    ? JsonConvert.SerializeObject(Managers.Raid.RaidClearInfo)
                    : string.Empty);
            
            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            if (jsonData.ContainsKey(ClearInfo) && !string.IsNullOrEmpty(jsonData[ClearInfo].ToString()))
                Managers.Raid.RaidClearInfo = JsonConvert.DeserializeObject<RaidClearInfo>(jsonData[ClearInfo].ToString());
        }
    }
}