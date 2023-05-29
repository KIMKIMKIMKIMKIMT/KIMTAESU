using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Newtonsoft.Json;
using UniRx;

namespace GameData
{
    [Serializable]
    public class MissionProgressData
    {
        public long DailyPlayTime;
        public long DailyKillCount;

        public void Init()
        {
            MessageBroker.Default.Receive<QuestMessage>().Subscribe(questMessage =>
            {
                switch (questMessage.ProgressType)
                {
                    case QuestProgressType.PlayTime:
                        DailyPlayTime += questMessage.Value;
                        break;
                    case QuestProgressType.KillMonster:
                        DailyKillCount += questMessage.Value;
                        break;
                }
            });
        }

        public long GetMissionData(MissionDataType missionDataType)
        {
            switch (missionDataType)
            {
                case MissionDataType.DailyPlayTime:
                    return DailyPlayTime;
                case MissionDataType.DailyKillCount:
                    return DailyKillCount;
                default:
                    return 0;
            }
        }
    }

    public class MissionGameData : BaseGameData
    {
        public override string TableName => "Mission";
        protected override string InDate { get; set; }

        private string MissionKey(int missionId) => $"Mission_{missionId}";
        private string MissionProgressKey => "MissionProgressData";

        protected override Param MakeInitData()
        {
            Param param = new Param();

            foreach (var missionChart in ChartManager.MissionCharts.Values)
                param.Add(MissionKey(missionChart.MissionId), (0, 0, 0));

            param.Add(MissionProgressKey, new MissionProgressData());

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            foreach (var missionData in Managers.Game.MissionDatas)
                param.Add(MissionKey(missionData.Key), missionData.Value);

            param.Add(MissionProgressKey, Managers.Game.MissionProgressData);

            return param;
        }

        protected override Param MakeSaveData(int id)
        {
            return new Param()
            {
                { MissionKey(id), Managers.Game.MissionDatas[id] }
            };
        }

        protected override Param MakeSaveData(List<int> ids)
        {
            var param = new Param();
            
            ids.ForEach(id =>
            {
                string missionKey = MissionKey(id);

                if (param.Contains(missionKey))
                    return;

                if (!Managers.Game.MissionDatas.TryGetValue(id, out var missionData))
                    return;
                
                param.Add(missionKey, missionData);
            });
            
            param.Add(MissionProgressKey, Managers.Game.MissionProgressData);

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            foreach (var missionChart in ChartManager.MissionCharts.Values)
            {
                (byte, byte, byte) missionValue = (0, 0, 0);

                if (!jsonData.ContainsKey(MissionKey(missionChart.MissionId)))
                {
                    param.Add(MissionKey(missionChart.MissionId), (0, 0, 0));
                }
                else
                {
                    missionValue = ((byte)jsonData[MissionKey(missionChart.MissionId)]["Item1"],
                        (byte)jsonData[MissionKey(missionChart.MissionId)]["Item2"],
                        (byte)jsonData[MissionKey(missionChart.MissionId)]["Item3"]);
                }

                Managers.Game.MissionDatas[missionChart.MissionId] = missionValue;
            }

            if (jsonData.ContainsKey(MissionProgressKey))
                Managers.Game.MissionProgressData = JsonMapper.ToObject<MissionProgressData>(jsonData[MissionProgressKey].ToJson());
            else
            {
                Managers.Game.MissionProgressData = new MissionProgressData();
                param.Add(MissionProgressKey, Managers.Game.MissionProgressData);
            }
            
            Debug.Log($"Mission Game Data Json Size : {JsonConvert.SerializeObject(Managers.Game.MissionDatas).Length / 1024f}kb");

            Managers.Game.MissionProgressData.Init();

            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}