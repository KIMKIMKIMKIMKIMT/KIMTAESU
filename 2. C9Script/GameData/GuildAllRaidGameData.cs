using System;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;

namespace GameData
{
    public class GuildAllRaidClearInfo
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


    public class GuildAllRaidGameData : BaseGameData
    {
        public override string TableName => "GuildAllRaid";

        protected override string InDate { get; set; }

        private const string GuildAllRaidEntryCount = "GuildAllRaidEntryCount";
        private const string GuildAllRaidClearInfo = "GuildAllRaidClearInfo";

        protected override Param MakeInitData()
        {
            Param param = new Param();

            param.Add(GuildAllRaidEntryCount, 1);
            param.Add(GuildAllRaidClearInfo, new GuildAllRaidClearInfo());

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            param.Add(GuildAllRaidEntryCount, Managers.AllRaid.EntryCount.GetDecrypted());
            param.Add(GuildAllRaidClearInfo, Managers.AllRaid.GuildAllRaidClearInfo);

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            if (jsonData.ContainsKey(GuildAllRaidEntryCount))
            {
                Managers.AllRaid.EntryCount = int.Parse(jsonData[GuildAllRaidEntryCount].ToString());
            }
            else
            {
                Managers.AllRaid.EntryCount = 1;
                param.Add(GuildAllRaidEntryCount, Managers.AllRaid.EntryCount.GetDecrypted());
            }

            if (jsonData.ContainsKey(GuildAllRaidClearInfo))
            {
                Managers.AllRaid.GuildAllRaidClearInfo = JsonConvert.DeserializeObject<GuildAllRaidClearInfo>(jsonData[GuildAllRaidClearInfo].ToJson());
            }
            else
            {
                Managers.AllRaid.GuildAllRaidClearInfo = new GuildAllRaidClearInfo();
                param.Add(GuildAllRaidClearInfo, Managers.AllRaid.GuildAllRaidClearInfo);
            }

            if (param.Count > 0)
                SaveGameData(param);
        }
    }

}