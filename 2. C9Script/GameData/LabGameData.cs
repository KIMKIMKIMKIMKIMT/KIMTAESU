using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;

namespace GameData
{
    public class LabResearchData
    {
        public LabSkillType LabSkillType;

        private ObscuredInt _level;

        public int Level
        {
            get => _level;
            set => _level = value;
        }

        private ObscuredBool _isResearch;

        public bool IsResearch
        {
            get => _isResearch;
            set => _isResearch = value;
        }

        public DateTime StartResearchTime;
        public DateTime EndResearchTime;
        public DateTime CoolTime;

        public LabResearchData(LabSkillType labSkillType)
        {
            LabSkillType = labSkillType;
            Level = 0;
            IsResearch = false;
            StartResearchTime = new DateTime();
            EndResearchTime = new DateTime();
            CoolTime = new DateTime();
        }
    }

    public class LabAwakeningData
    {
        private ObscuredInt _id;

        public int Id
        {
            get => _id;
            private set => _id = value;
        }

        private ObscuredInt _patternId;

        public int PatternId
        {
            get => _patternId;
            set => _patternId = value;
        }

        public Grade Grade;

        private ObscuredInt _statId;

        public int StatId
        {
            get => _statId;
            set => _statId = value;
        }

        private ObscuredDouble _statValue;

        public double StatValue
        {
            get => _statValue;
            set => _statValue = value;
        }

        private ObscuredBool _isLock;

        public bool IsLock
        {
            get => _isLock;
            set => _isLock = value;
        }

        public LabAwakeningData(int id)
        {
            Id = id;
            PatternId = 0;
            StatId = 0;
            StatValue = 0;
            IsLock = false;
        }
    }

    public class LabGameData : BaseGameData
    {
        public override string TableName => "Lab";
        protected override string InDate { get; set; }

        private string ResearchKey(LabSkillType labSkillType) => $"Research_{labSkillType.ToString()}";
        private string AwakeningKey(int presetId, int awakeningId) => $"Awakening_{presetId}_{awakeningId}";

        private const int PresetCount = 4;

        private string EquipPreset => "EquipPreset";

        protected override Param MakeInitData()
        {
            var param = new Param();

            foreach (var chartData in ChartManager.LabSkillCharts.Values)
            {
                var labResearchData = new LabResearchData(chartData.LabSkillType);
                param.Add(ResearchKey(labResearchData.LabSkillType), labResearchData);
            }


            for (var presetId = 0; presetId < PresetCount; presetId++)
            {
                foreach (var chartData in ChartManager.LabAwakeningCharts.Values)
                {
                    var labAwakeningData = new LabAwakeningData(chartData.Id);

                    param.Add(AwakeningKey(presetId, chartData.Id), labAwakeningData);
                }
            }

            param.Add(EquipPreset, 0);

            return param;
        }

        protected override Param MakeSaveData()
        {
            var param = new Param();

            foreach (var labResearchData in Managers.Game.LabResearchDatas)
                param.Add(ResearchKey(labResearchData.Key), labResearchData.Value);

            for (int presetId = 0; presetId < PresetCount; presetId++)
            {
                if (!Managers.Game.LabAwakeningDatas.TryGetValue(presetId, out var labAwakeningDatas))
                    continue;

                foreach (var labAwakeningData in labAwakeningDatas.Values)
                {
                    param.Add(AwakeningKey(presetId, labAwakeningData.Id), labAwakeningData);
                }
            }

            param.Add(EquipPreset, Managers.Game.LabEquipPresetId.GetDecrypted());

            return param;
        }

        public void SaveEquipPresetGameData()
        {
            var param = new Param()
            {
                { EquipPreset, Managers.Game.LabEquipPresetId.GetDecrypted() }
            };

            SaveGameData(param);
        }

        public void SaveResearchGameData(LabSkillType labSkillType, bool isSaveImmediately = false)
        {
            if (!Managers.Game.LabResearchDatas.TryGetValue(labSkillType, out var labResearchData))
                return;

            var param = new Param();

            param.Add(ResearchKey(labSkillType), labResearchData);

            SaveGameData(param, isSaveImmediately);
        }

        public void SaveAwakeningGameData(int presetId)
        {
            if (!Managers.Game.LabAwakeningDatas.TryGetValue(presetId, out var labAwakeningDatas))
                return;

            var param = new Param();

            foreach (var labAwakeningData in labAwakeningDatas)
                param.Add(AwakeningKey(presetId, labAwakeningData.Value.Id), labAwakeningData);

            if (param.Count > 0)
                SaveGameData(param);
        }

        public void SaveAwakeningGameData(List<(int, int)> presetIdAndAwakeningIds, bool isPresetSave = false)
        {
            var param = new Param();

            presetIdAndAwakeningIds.ForEach(tuple =>
            {
                var presetId = tuple.Item1;
                var awakeningId = tuple.Item2;

                if (!Managers.Game.LabAwakeningDatas.TryGetValue(presetId, out var labAwakeningDatas))
                    return;

                if (!labAwakeningDatas.TryGetValue(awakeningId, out var labAwakeningData))
                    return;

                if (param.ContainsKey(AwakeningKey(presetId, awakeningId)))
                    return;

                param.Add(AwakeningKey(presetId, awakeningId), labAwakeningData);
            });
            
            if (isPresetSave)
                param.Add(EquipPreset, Managers.Game.LabEquipPresetId.GetDecrypted());

            SaveGameData(param);
        }

        protected override void SetGameData(JsonData jsonData)
        {
            var param = new Param();

            foreach (var chartData in ChartManager.LabSkillCharts.Values)
            {
                var key = ResearchKey(chartData.LabSkillType);

                LabResearchData labResearchData;

                if (jsonData.ContainsKey(key))
                    labResearchData = JsonConvert.DeserializeObject<LabResearchData>(jsonData[key].ToJson());
                else
                {
                    labResearchData = new LabResearchData(chartData.LabSkillType);
                    param.Add(key, labResearchData);
                }

                if (Managers.Game.LabResearchDatas.ContainsKey(labResearchData.LabSkillType))
                    Managers.Game.LabResearchDatas[labResearchData.LabSkillType] = labResearchData;
                else
                    Managers.Game.LabResearchDatas.Add(labResearchData.LabSkillType, labResearchData);
            }

            for (var presetId = 0; presetId < PresetCount; presetId++)
            {
                foreach (var chartData in ChartManager.LabAwakeningCharts.Values)
                {
                    var key = AwakeningKey(presetId, chartData.Id);

                    LabAwakeningData labAwakeningData;

                    if (jsonData.ContainsKey(key))
                        labAwakeningData = JsonConvert.DeserializeObject<LabAwakeningData>(jsonData[key].ToJson());
                    else
                    {
                        labAwakeningData = new LabAwakeningData(chartData.Id);
                        param.Add(key, labAwakeningData);
                    }

                    if (!Managers.Game.LabAwakeningDatas.ContainsKey(presetId))
                        Managers.Game.LabAwakeningDatas.Add(presetId, new Dictionary<int, LabAwakeningData>());

                    if (Managers.Game.LabAwakeningDatas[presetId].ContainsKey(chartData.Id))
                        Managers.Game.LabAwakeningDatas[presetId][chartData.Id] = labAwakeningData;
                    else
                        Managers.Game.LabAwakeningDatas[presetId].Add(chartData.Id, labAwakeningData);
                }
            }

            if (jsonData.ContainsKey(EquipPreset))
            {
                int.TryParse(jsonData[EquipPreset].ToString(), out int equipPreset);
                Managers.Game.LabEquipPresetId = equipPreset;
            }
            else
            {
                Managers.Game.LabEquipPresetId = 0;
                param.Add(EquipPreset, Managers.Game.LabEquipPresetId.GetDecrypted());
            }

            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}