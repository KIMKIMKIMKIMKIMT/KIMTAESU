using System.Collections.Generic;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;
using UniRx;

namespace GameData
{
    public class SkillData
    {
        public int Id;

        private ObscuredInt _level;

        public int Level
        {
            get => _level;
            set
            {
                _level = value;
                OnChangeLevel?.OnNext(_level);
            }
        }

        [JsonIgnore]
        public bool IsOpen => Level > 0;

        [JsonIgnore] public Subject<int> OnChangeLevel = new();
    }

    public class SkillGameData : BaseGameData
    {
        public override string TableName => "Skill";
        protected override string InDate { get; set; }

        private string SkillKey(int id) => $"Skill_{id.ToString()}";
        private string EquipSkillKey(int presetId, int slotId) => $"EquipSkill_{presetId}_{slotId}";

        protected override Param MakeInitData()
        {
            Param param = new Param();

            foreach (var skillChart in ChartManager.SkillCharts.Values)
            {
                SkillData data = new SkillData()
                {
                    Id = skillChart.Id,
                    Level = 0
                };

                param.Add(SkillKey(data.Id), JsonConvert.SerializeObject(data));
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    param.Add(EquipSkillKey(i, j), 0);
                }
            }

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            foreach (var skillData in Managers.Game.SkillDatas.Values)
            {
                param.Add(SkillKey(skillData.Id), JsonConvert.SerializeObject(skillData));
            }
            
            for (int presetId = 0; presetId < 3; presetId++)
            {
                for (int slotId = 0; slotId < 5; slotId++)
                {
                    param.Add(EquipSkillKey(presetId, slotId), Managers.Game.EquipSkillList[presetId][slotId].Value);
                }
            }

            return param;
        }

        protected override Param MakeSaveData(int id)
        {
            Param param = new Param();

            if (Managers.Game.SkillDatas.TryGetValue(id, out var skillData))
                param.Add(SkillKey(skillData.Id), JsonConvert.SerializeObject(skillData));

            return param;
        }
        
        // (프리셋, 슬롯)
        public void SaveEquipSkillData()
        {
            Param param = new Param();
            
            for (int presetId = 0; presetId < 3; presetId++)
            {
                for (int slotId = 0; slotId < 5; slotId++)
                {
                    param.Add(EquipSkillKey(presetId, slotId), Managers.Game.EquipSkillList[presetId][slotId].Value);
                }
            }
            
            SaveGameData(param);
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            foreach (int skillId in ChartManager.SkillCharts.Keys)
            {
                SkillData skillData;

                if (!jsonData.ContainsKey(SkillKey(skillId)))
                {
                    skillData = new SkillData()
                    {
                        Id = skillId,
                        Level = 0
                    };

                    param.Add(SkillKey(skillId), JsonConvert.SerializeObject(skillData));
                }
                else
                {
                    skillData = JsonConvert.DeserializeObject<SkillData>(jsonData[SkillKey(skillId)].ToString());
                }

                if (!Managers.Game.SkillDatas.ContainsKey(skillId))
                    Managers.Game.SkillDatas.Add(skillId, skillData);
                else
                    Managers.Game.SkillDatas[skillId] = skillData;
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (jsonData.ContainsKey(EquipSkillKey(i, j)))
                        Managers.Game.EquipSkillList[i][j].Value = int.Parse(jsonData[EquipSkillKey(i, j)].ToString());
                    else
                        param.Add(EquipSkillKey(i, j), 0);
                }
            }

            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}