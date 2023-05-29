using System;
using BackEnd;
using Chart;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;
using UniRx;

namespace GameData
{
    public record QuestMessage
    {
        public QuestProgressType ProgressType;
        public int Id;
        public long Value;

        public QuestMessage(QuestProgressType progressType)
        {
            ProgressType = progressType;
            Id = 0;
            Value = 1;
        }

        public QuestMessage(QuestProgressType progressType, long value)
        {
            ProgressType = progressType;
            Id = 0;
            Value = value;
        }

        public QuestMessage(QuestProgressType progressType, int id, long value)
        {
            ProgressType = progressType;
            Id = id;
            Value = value;
        }
    }
    
    public record GuideQuestMessage
    {
        public QuestProgressType ProgressType;
        public int Id;
        public long Value;

        public GuideQuestMessage(QuestProgressType progressType)
        {
            ProgressType = progressType;
            Id = 0;
            Value = 1;
        }

        public GuideQuestMessage(QuestProgressType progressType, long value)
        {
            ProgressType = progressType;
            Id = 0;
            Value = value;
        }

        public GuideQuestMessage(QuestProgressType progressType, int id, long value)
        {
            ProgressType = progressType;
            Id = id;
            Value = value;
        }
    }


    [Serializable]
    public class QuestData
    {
        public int Id;

        private ObscuredLong _progressValue;

        public long ProgressValue
        {
            get => _progressValue;
            set
            {                
                _questChart ??= ChartManager.QuestCharts[Id];

                if (value >= _questChart.CompleteValue && _questChart.Type != QuestType.Repeat)
                    _progressValue = _questChart.CompleteValue;
                else
                    _progressValue = value;

                OnChangeProgressValue?.OnNext(_progressValue);
            }
        }

        private ObscuredBool _isReceiveReward;

        public bool IsReceiveReward
        {
            get => _isReceiveReward;
            set
            {
                _isReceiveReward = value;
                OnChangeIsReceiveReward?.OnNext(_isReceiveReward);
            }
        }

        private QuestChart _questChart;

        [JsonIgnore] public Subject<long> OnChangeProgressValue = new();

        [JsonIgnore] public Subject<bool> OnChangeIsReceiveReward = new();

        [JsonIgnore] public bool IsComplete => ProgressValue >= _questChart.CompleteValue;

        public QuestData()
        {
        }

        public QuestData(int id, int progressValue, bool isReceiveReward)
        {
            _questChart = ChartManager.QuestCharts[id];

            Id = id;
            ProgressValue = progressValue;
            IsReceiveReward = isReceiveReward;

            MessageBroker.Default.Receive<QuestMessage>()
                .Where(questMessage => !IsReceiveReward && questMessage.ProgressType == _questChart.ProgressType)
                .Subscribe(
                    questMessage =>
                    {
                        ProgressValue += questMessage.Value;
                    });
        }

        public void Init()
        {
            _questChart = ChartManager.QuestCharts[Id];

            MessageBroker.Default.Receive<QuestMessage>()
                .Where(questMessage => !IsReceiveReward && questMessage.ProgressType == _questChart.ProgressType)
                .Subscribe(
                    questMessage => { ProgressValue += questMessage.Value; });
        }
    }

    [Serializable]
    public class GuideQuestData
    {
        public int Id;

        private int _progressId;
        private long _progressValue;

        public long ProgressValue
        {
            get => _progressValue;
            set
            {
                _guideQuestChart ??= ChartManager.GuideQuestCharts[Id];

                if (value >= _guideQuestChart.QuestCompleteValue)
                    _progressValue = _guideQuestChart.QuestCompleteValue;
                else
                    _progressValue = value;

                OnChangeProgressValue?.OnNext(_progressValue);
            }
        }

        private GuideQuestChart _guideQuestChart;

        [JsonIgnore] public Subject<long> OnChangeProgressValue = new();
        [JsonIgnore] public bool IsComplete => ProgressValue >= _guideQuestChart.QuestCompleteValue;

        public GuideQuestData()
        {
        }

        public GuideQuestData(int id, int progressValue)
        {
            _guideQuestChart = ChartManager.GuideQuestCharts[id];

            Id = id;
            ProgressValue = progressValue;

            MessageBroker.Default.Receive<QuestMessage>()
                .Where(questMessage =>
                    questMessage.ProgressType == _guideQuestChart.QuestProgressType
                                     && (_guideQuestChart.QuestProgressId == 0 ||
                                         _guideQuestChart.QuestProgressId == questMessage.Id))
                .Subscribe(
                    questMessage => { ProgressValue += questMessage.Value; });
            
            MessageBroker.Default.Receive<GuideQuestMessage>()
                .Where(questMessage =>
                    questMessage.ProgressType == _guideQuestChart.QuestProgressType
                    && (_guideQuestChart.QuestProgressId == 0 ||
                        _guideQuestChart.QuestProgressId == questMessage.Id))
                .Subscribe(
                    questMessage => { ProgressValue += questMessage.Value; });
        }

        public void Init()
        {
            _guideQuestChart = ChartManager.GuideQuestCharts[Id];

            MessageBroker.Default.Receive<QuestMessage>()
                .Where(questMessage =>
                   questMessage.ProgressType == _guideQuestChart.QuestProgressType)
                .Subscribe(
                    questMessage => { ProgressValue += questMessage.Value; });
            
            MessageBroker.Default.Receive<GuideQuestMessage>()
                .Where(questMessage =>
                    questMessage.ProgressType == _guideQuestChart.QuestProgressType)
                .Subscribe(
                    questMessage => { ProgressValue += questMessage.Value; });
        }
    }

    public class QuestGameData : BaseGameData
    {
        public override string TableName => "Quest";
        protected override string InDate { get; set; }

        private string QuestKey(int id) => $"Quest_{id.ToString()}";

        protected override Param MakeInitData()
        {
            Param param = new Param();

            foreach (var chartData in ChartManager.QuestCharts.Values)
            {
                QuestData questData = new QuestData(chartData.Id, 0, false);
                param.Add(QuestKey(questData.Id), JsonConvert.SerializeObject(questData));
            }

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            foreach (var questData in Managers.Game.QuestDatas.Values)
            {
                param.Add(QuestKey(questData.Id), JsonConvert.SerializeObject(questData));
            }

            return param;
        }

        protected override Param MakeSaveData(int id)
        {
            Param param = new Param();

            if (Managers.Game.QuestDatas.TryGetValue(id, out var questData))
                param.Add(QuestKey(questData.Id), JsonConvert.SerializeObject(questData));

            return param;
        }

        public void SaveNonReceiveQuestData()
        {
            Param param = new Param();

            foreach (var questData in Managers.Game.QuestDatas.Values)
            {
                if (questData.IsReceiveReward)
                    continue;

                param.Add(QuestKey(questData.Id), JsonConvert.SerializeObject(questData));
            }

            SaveGameData(param);
        }

        public void SaveSummonQuestData()
        {
            var param = new Param();
            
            foreach (var questData in Managers.Game.QuestDatas.Values)
            {
                if (questData.IsReceiveReward)
                    continue;

                if (ChartManager.QuestCharts[questData.Id].ProgressType != QuestProgressType.Summon)
                    continue;
                
                param.Add(QuestKey(questData.Id), JsonConvert.SerializeObject(questData));
            }
            
            SaveGameData(param);
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            foreach (int questId in ChartManager.QuestCharts.Keys)
            {
                QuestData questData;

                if (!jsonData.ContainsKey(QuestKey(questId)))
                {
                    questData = new QuestData(questId, 0, false);
                    param.Add(QuestKey(questId), questData);
                }
                else
                {
                    questData = JsonConvert.DeserializeObject<QuestData>(jsonData[QuestKey(questId)].ToString());
                    questData?.Init();
                }

                if (!Managers.Game.QuestDatas.ContainsKey(questId))
                    Managers.Game.QuestDatas.Add(questId, questData);
                else
                    Managers.Game.QuestDatas[questId] = questData;
            }

            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}