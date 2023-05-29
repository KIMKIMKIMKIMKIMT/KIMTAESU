using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;
using UniRx;

namespace GameData
{
    [Serializable]
    public class PetData
    {
        public int Id;

        private ObscuredInt _level;

        public int Level
        {
            get => _level;
            set => _level = value;
        }

        private ObscuredInt _quantity;

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;

                if (Level == 0 && _quantity > 0)
                {
                    Level = 1;
                    Managers.Game.CalculateStatFlag = true;
                }
            }
        }
        
        [JsonIgnore]
        public bool IsAcquired => Level > 0;
    }
    
    public class PetGameData : BaseGameData
    {
        public override string TableName => "Pet";
        protected override string InDate { get; set; }

        private string PetKey(int id) => $"Pet_{id.ToString()}";

        protected override Param MakeInitData()
        {
            Param param = new Param();

            foreach (var chartData in ChartManager.PetCharts.Values)
            {
                PetData data = new PetData()
                {
                    Id = chartData.Id,
                    Level = chartData.Id == (int)ChartManager.SystemCharts[SystemData.Default_Pet].Value ? 1 : 0,
                    Quantity = 0
                };
                
                param.Add(PetKey(data.Id), data.ToJson());
            }
            
            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            foreach (var petData in Managers.Game.PetDatas.Values)
                param.Add(PetKey(petData.Id), petData.ToJson());

            return param;
        }

        protected override Param MakeSaveData(int id)
        {
            Param param = new Param();
            
            if (Managers.Game.PetDatas.TryGetValue(id, out var petData))
                param.Add(PetKey(petData.Id), petData.ToJson());

            return param;
        }

        protected override Param MakeSaveData(List<int> ids)
        {
            var param = new Param();

            ids.ForEach(id =>
            {
                string petKey = PetKey(id);

                if (param.Contains(petKey))
                    return;

                if (!Managers.Game.PetDatas.TryGetValue(id, out var petData))
                    return;
                
                param.Add(petKey, petData.ToJson());
            });

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            foreach (int petId in ChartManager.PetCharts.Keys)
            {
                PetData petData;

                if (!jsonData.ContainsKey(PetKey(petId)))
                {
                    petData = new PetData()
                    {
                        Id = petId,
                        Level = 0,
                        Quantity = 0,
                    };
                    
                    param.Add(PetKey(petId), petData.ToJson());
                }
                else
                {
                    petData = jsonData[PetKey(petId)].ToData<PetData>();
                }

                if (!Managers.Game.PetDatas.ContainsKey(petId))
                    Managers.Game.PetDatas.Add(petId, petData);
                else
                    Managers.Game.PetDatas[petId] = petData;
            }
            
            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}