using System;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;

namespace GameData
{
    public class GuildRaidClearInfo
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

        private ObscuredDouble _highestGuildPoint;
        public double HighestGuildPoint
        {
            get => _highestGuildPoint;
            set => _highestGuildPoint = value;
        }
    }
    
    public class GuildGameData : BaseGameData
    {
        public override string TableName => "Guild";
        protected override string InDate { get; set; }

        private const string GuildRaidEntryCount = "GuildRaidEntryCount";
        private const string GuildRaidClearInfo = "GuildRaidClearInfo";

        protected override Param MakeInitData()
        {
            var param = new Param();
            
            param.Add(GuildRaidEntryCount, 1);
            param.Add(GuildRaidClearInfo, new GuildRaidClearInfo());

            return param;
        }

        protected override Param MakeSaveData()
        {
            var param = new Param();
            
            param.Add(GuildRaidEntryCount, Managers.GuildRaid.EntryCount.GetDecrypted());
            param.Add(GuildRaidClearInfo, Managers.GuildRaid.GuildRaidClearInfo);

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            var param = new Param();

            if (jsonData.ContainsKey(GuildRaidEntryCount))
                Managers.GuildRaid.EntryCount = int.Parse(jsonData[GuildRaidEntryCount].ToString());
            else
            {
                Managers.GuildRaid.EntryCount = 1;
                param.Add(GuildRaidEntryCount, Managers.GuildRaid.EntryCount.GetDecrypted());
            }
            
            if (jsonData.ContainsKey(GuildRaidClearInfo))
                Managers.GuildRaid.GuildRaidClearInfo = JsonConvert.DeserializeObject<GuildRaidClearInfo>(jsonData[GuildRaidClearInfo].ToJson());
            else
            {
                Managers.GuildRaid.GuildRaidClearInfo = new GuildRaidClearInfo();
                param.Add(GuildRaidClearInfo, Managers.GuildRaid.GuildRaidClearInfo);
            }

            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}