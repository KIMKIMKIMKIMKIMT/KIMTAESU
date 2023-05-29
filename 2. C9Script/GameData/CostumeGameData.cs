using System;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;
using UniRx;

namespace GameData
{
    [Serializable]
    public class CostumeData
    {
        public int Id;
        private ObscuredInt _awakening;

        public int Awakening
        {
            get => _awakening;
            set
            {
                _awakening = value;
                OnChangeAwakening?.OnNext(_awakening);
            }
        }

        private ObscuredInt _quantity;

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;

                if (_awakening == -1 && value > 0)
                {
                    _awakening = 0;
                    Managers.Game.CalculateStat();
                }
            }
        }

        [JsonIgnore]
        public bool IsAcquired => _awakening >= 0;
        
        [JsonIgnore] 
        public Subject<int> OnChangeAwakening = new();
    }

    public class CostumeSetData
    {
        public int Id;
        public bool Active;
    }
    
    public class CostumeGameData : BaseGameData
    {
        public override string TableName => "Costume";
        protected override string InDate { get; set; }

        private string CostumeKey(int id) => $"Costume_{id.ToString()}";
        private string CostumSetKey(int id) => $"CostumeSet_{id.ToString()}";
        
        protected override Param MakeInitData()
        {
            Param param = new Param();

            foreach (var chartData in ChartManager.CostumeCharts.Values)
            {
                CostumeData data = new CostumeData()
                {
                    Id = chartData.Id,
                    Awakening = chartData.Id == ChartManager.SystemCharts[SystemData.Default_Costume].Value ? 0 : -1,
                    Quantity = 0
                };
                
                param.Add(CostumeKey(data.Id), JsonConvert.SerializeObject(data));
            }
            foreach (var data in ChartManager.CostumCollectionCharts.Values)
            {
                CostumeSetData setData = new CostumeSetData()
                {
                    Id = data.Id,
                    Active = false
                };

                param.Add(CostumSetKey(setData.Id), setData.Active);
            }

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            foreach (var costumeData in Managers.Game.CostumeDatas.Values)
            {
                param.Add(CostumeKey(costumeData.Id), JsonConvert.SerializeObject(costumeData));
            }

            foreach (var costumeSetData in Managers.Game.CostumeSetDatas)
            {
                param.Add(CostumSetKey(costumeSetData.Key), costumeSetData.Value);
            }
            
            return param;
        }

        protected override Param MakeSaveData(int id)
        {
            Param param = new Param();
            
            if (Managers.Game.CostumeDatas.TryGetValue(id, out var costumeData))
                param.Add(CostumeKey(id), JsonConvert.SerializeObject(costumeData));

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            foreach (int costumeId in ChartManager.CostumeCharts.Keys)
            {
                CostumeData costumeData;

                if (!jsonData.ContainsKey(CostumeKey(costumeId)))
                {
                    costumeData = new CostumeData()
                    {
                        Id = costumeId,
                        Awakening = -1
                    };

                    param.Add(CostumeKey(costumeId), JsonConvert.SerializeObject(costumeData));
                }
                else
                {
                    costumeData =
                        JsonConvert.DeserializeObject<CostumeData>(jsonData[CostumeKey(costumeId)].ToString());
                }

                if (!Managers.Game.CostumeDatas.ContainsKey(costumeId))
                    Managers.Game.CostumeDatas.Add(costumeId, costumeData);
                else
                    Managers.Game.CostumeDatas[costumeId] = costumeData;
            }

            foreach (var costumeSetId in ChartManager.CostumCollectionCharts.Keys)
            {
                CostumeSetData costumeSetData;

                if (!jsonData.ContainsKey(CostumSetKey(costumeSetId)))
                {
                    costumeSetData = new CostumeSetData()
                    {
                        Id = costumeSetId,
                        Active = false
                    };

                    param.Add(CostumSetKey(costumeSetData.Id), costumeSetData.Active);
                }
                else
                {
                    costumeSetData = new CostumeSetData()
                    {
                        Id = costumeSetId,
                        Active = bool.Parse(jsonData[CostumSetKey(costumeSetId)].ToString())
                    };
                }

                if (!Managers.Game.CostumeSetDatas.ContainsKey(costumeSetId))
                    Managers.Game.CostumeSetDatas.Add(costumeSetId, costumeSetData.Active);
                else
                    Managers.Game.CostumeSetDatas[costumeSetId] = costumeSetData.Active;
            }
            
            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}