using System;
using System.Runtime.ConstrainedExecution;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace GameData
{
    [Serializable]
    public class XMasEventData
    {
        private ObscuredInt _xMasDungeonHighestScore;
        public int XMasDungeonHighestScore
        {
            get => _xMasDungeonHighestScore;
            set => _xMasDungeonHighestScore = value;
        }

        private ObscuredInt _xMasDungeonEntryCount;
        public int XMasDungeonEntryCount
        {
            get => _xMasDungeonEntryCount;
            set => _xMasDungeonEntryCount = value;
        }
    }
    
    public class XMasEventGameData : BaseGameData
    {
        public override string TableName => "XMasEvent";
        protected override string InDate { get; set; }
        
        private const string XMasDungeonHighestScore = "XMasDungeonHighestScore";
        private const string XMasDungeonEntryCount = "XMasDungeonEntryCount";
        
        protected override Param MakeInitData()
        {
            var param = new Param
            {
                { XMasDungeonHighestScore, 0 },
                { XMasDungeonEntryCount, ChartManager.XMasEventDungeonCharts[1].DailyEntryCount.GetDecrypted() }
            };

            return param;
        }

        protected override Param MakeSaveData()
        {
            var param = new Param
            {
                { XMasDungeonHighestScore, Managers.Game.XMasEventData.XMasDungeonHighestScore },
                { XMasDungeonEntryCount, Managers.Game.XMasEventData.XMasDungeonEntryCount }
            };

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            var param = new Param();
            
            if (jsonData.ContainsKey(XMasDungeonHighestScore))
            {
                int.TryParse(jsonData[XMasDungeonHighestScore].ToString(), out var xMasDungeonHighestScore);
                Managers.Game.XMasEventData.XMasDungeonHighestScore = xMasDungeonHighestScore;
            }
            else
            {
                Managers.Game.XMasEventData.XMasDungeonHighestScore = 0;
                param.Add(XMasDungeonHighestScore, 0);
            }

            if (jsonData.ContainsKey(XMasDungeonEntryCount))
            {
                int.TryParse(jsonData[XMasDungeonEntryCount].ToString(), out var xmasDungeonEntryCount);
                Managers.Game.XMasEventData.XMasDungeonEntryCount = xmasDungeonEntryCount;
            }
            else
            {
                if (ChartManager.XMasEventDungeonCharts.TryGetValue(1, out var xMasEventDungeonChart))
                {
                    Managers.Game.XMasEventData.XMasDungeonEntryCount = xMasEventDungeonChart.DailyEntryCount;
                    param.Add(XMasDungeonEntryCount, Managers.Game.XMasEventData.XMasDungeonEntryCount);
                }
                else
                {
                    Debug.LogError("Fail Load XMasEventDungeonChart : 1");
                }
            }
            
            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}