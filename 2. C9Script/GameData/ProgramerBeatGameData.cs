using System;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace GameData
{
    public class ProgramerBeatData
    {
        private ObscuredDouble _programerBeatHighestScore;
        public double ProgramerBeatHighestScore
        {
            get => _programerBeatHighestScore;
            set => _programerBeatHighestScore = value;
        }

        private ObscuredInt _programerBeatEntryCount;
        public int ProgramerBeatEntryCount
        {
            get => _programerBeatEntryCount;
            set => _programerBeatEntryCount = value;
        }
    }

    public class ProgramerBeatGameData : BaseGameData
    {
        public override string TableName => "ProgramerBeat";
        protected override string InDate { get; set; }

        private const string ProgramerBeatHighestScore = "ProgramerBeatHighestScore";
        private const string ProgramerBeatEntryCount = "ProgramerBeatEntryCount";
        

        protected override Param MakeInitData()
        {
            var param = new Param();

            if (!ChartManager.WorldCupEventDungeonCharts.TryGetValue(1, out var worldCupEventDungeonChart))
                return param;

            param.Add(ProgramerBeatHighestScore, 0);
            param.Add(ProgramerBeatEntryCount, worldCupEventDungeonChart.DailyEntryCount.GetDecrypted());

            return param;
        }

        protected override Param MakeSaveData()
        {
            var param = new Param();

            param.Add(ProgramerBeatHighestScore, Managers.Game.ProgramerBeatData.ProgramerBeatHighestScore);
            param.Add(ProgramerBeatEntryCount, Managers.Game.ProgramerBeatData.ProgramerBeatEntryCount);

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            var param = new Param();

            if (jsonData.ContainsKey(ProgramerBeatHighestScore))
            {
                double.TryParse(jsonData[ProgramerBeatHighestScore].ToString(), out var worldCupDungeonHighestScore);
                Managers.Game.ProgramerBeatData.ProgramerBeatHighestScore = worldCupDungeonHighestScore;
            }
            else
            {
                Managers.Game.ProgramerBeatData.ProgramerBeatHighestScore = 0;
                param.Add(ProgramerBeatHighestScore, 0);
            }

            if (jsonData.ContainsKey(ProgramerBeatEntryCount))
            {
                int.TryParse(jsonData[ProgramerBeatEntryCount].ToString(), out var worldCupDungeonEntryCount);
                Managers.Game.ProgramerBeatData.ProgramerBeatEntryCount = worldCupDungeonEntryCount;
            }
            else
            {
                int entryCount =
                    ChartManager.WorldCupEventDungeonCharts.TryGetValue(1, out var worldCupEventDungeonChart)
                        ? worldCupEventDungeonChart.DailyEntryCount
                        : 0;
                Managers.Game.ProgramerBeatData.ProgramerBeatEntryCount = entryCount;
                param.Add(ProgramerBeatEntryCount, entryCount);
            }

            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}

