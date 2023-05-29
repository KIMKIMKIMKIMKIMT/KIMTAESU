using System;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace GameData
{
    [Serializable]
    public class NewYearEventData
    {
        private ObscuredDouble _newYearDungeonHighestScore;
        public double NewYearDungeonHighestScore
        {
            get => _newYearDungeonHighestScore;
            set => _newYearDungeonHighestScore = value;
        }

        private ObscuredInt _newYearDungeonEntryCount;
        public int NewYearDungeonEntryCount
        {
            get => _newYearDungeonEntryCount;
            set => _newYearDungeonEntryCount = value;
        }


    }
    
    public class NewYearEventGameData : BaseGameData
    {
        public override string TableName => "NewYearEvent";
        protected override string InDate { get; set; }

        private const string NewYearDungeonHighestScore = "NewYearDungeonHighestScore";
        private const string NewYearDungeonEntryCount = "NewYearDungeonEntryCount";

        protected override Param MakeInitData()
        {
            var param = new Param();

            if (!ChartManager.WorldCupEventDungeonCharts.TryGetValue(1, out var worldCupEventDungeonChart))
                return param;
            
            param.Add(NewYearDungeonHighestScore, 0);
            param.Add(NewYearDungeonEntryCount, worldCupEventDungeonChart.DailyEntryCount.GetDecrypted());

            return param;
        }

        protected override Param MakeSaveData()
        {
            var param = new Param();
            
            param.Add(NewYearDungeonHighestScore, Managers.Game.NewYearEventData.NewYearDungeonHighestScore);
            param.Add(NewYearDungeonEntryCount, Managers.Game.NewYearEventData.NewYearDungeonEntryCount);

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            var param = new Param();
            
            // NewYearDungeonHighestScore
            if (jsonData.ContainsKey(NewYearDungeonHighestScore))
            {
                double.TryParse(jsonData[NewYearDungeonHighestScore].ToString(), out var worldCupDungeonHighestScore);
                Managers.Game.NewYearEventData.NewYearDungeonHighestScore = worldCupDungeonHighestScore;
            }
            else
            {
                Managers.Game.NewYearEventData.NewYearDungeonHighestScore = 0;
                param.Add(NewYearDungeonHighestScore, 0);
            }
            
            // NewYearDungeonEntryCount
            if (jsonData.ContainsKey(NewYearDungeonEntryCount))
            {
                int.TryParse(jsonData[NewYearDungeonEntryCount].ToString(), out var worldCupDungeonEntryCount);
                Managers.Game.NewYearEventData.NewYearDungeonEntryCount = worldCupDungeonEntryCount;
            }
            else
            {
                int entryCount =
                    ChartManager.WorldCupEventDungeonCharts.TryGetValue(1, out var worldCupEventDungeonChart)
                        ? worldCupEventDungeonChart.DailyEntryCount
                        : 0;
                Managers.Game.NewYearEventData.NewYearDungeonEntryCount = entryCount;
                param.Add(NewYearDungeonEntryCount, entryCount);
            }
            
            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}