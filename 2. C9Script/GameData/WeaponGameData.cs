using System;
using System.Collections.Generic;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;
using UniRx;

namespace GameData
{
    [Serializable]
    public class WeaponData
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
                if (!IsAcquired&& _quantity > 0)
                {
                    Level = 1;
                    OnAcquired?.OnNext(1);
                    Managers.Game.CalculateStatFlag = true;
                }

                OnChangeQuantity?.OnNext(_quantity);
            }
        }


        [JsonIgnore]
        public bool IsAcquired => Level > 0;

        [JsonIgnore]
        public Subject<int> OnChangeQuantity = new();

        //[JsonIgnore]
        [NonSerialized]
        public Subject<int> OnAcquired = new();
    }

    public class WeaponGameData : BaseGameData
    {
        public override string TableName => "Weapon";
        protected override string InDate { get; set; }

        private string WeaponKey(int id) => $"Weapon_{id.ToString()}";

        protected override Param MakeInitData()
        {
            Param param = new Param();

            foreach (var weaponData in ChartManager.WeaponCharts.Values)
            {
                WeaponData data = new WeaponData()
                {
                    Id = weaponData.Id,
                    Quantity = 0,
                    Level = weaponData.Id == ChartManager.SystemCharts[SystemData.Default_Weapon].Value ? 1 : 0
                };

                param.Add(WeaponKey(data.Id), JsonConvert.SerializeObject(data));
            }

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            foreach (var weaponData in Managers.Game.WeaponDatas.Values)
            {
                param.Add(WeaponKey(weaponData.Id), JsonConvert.SerializeObject(weaponData));
            }

            return param;
        }

        protected override Param MakeSaveData(int id)
        {
            Param param = new Param();

            if (Managers.Game.WeaponDatas.TryGetValue(id, out var weaponData))
                param.Add(WeaponKey(weaponData.Id), JsonConvert.SerializeObject(weaponData));

            return param;
        }

        protected override Param MakeSaveData(List<int> ids)
        {
            var param = new Param();

            ids.ForEach(id =>
            {
                string weaponKey = WeaponKey(id);

                if (param.Contains(weaponKey))
                    return;

                if (!Managers.Game.WeaponDatas.TryGetValue(id, out var weaponData))
                    return;

                param.Add(weaponKey, weaponData);
            });

            return param;
        }


        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            foreach (int weaponId in ChartManager.WeaponCharts.Keys)
            {
                WeaponData weaponData;

                if (!jsonData.ContainsKey(WeaponKey(weaponId)))
                {
                    weaponData = new WeaponData()
                    {
                        Id = weaponId,
                        Quantity = 0,
                        Level = 0
                    };

                    param.Add(WeaponKey(weaponId), JsonConvert.SerializeObject(weaponData));
                }
                else
                {
                    string weaponJsonData = jsonData[WeaponKey(weaponId)].ToString();

                    if (weaponJsonData.Contains("Json"))
                    {
                        weaponData = JsonConvert.DeserializeObject<WeaponData>(jsonData[WeaponKey(weaponId)].ToJson());
                        Managers.Game.IsWeaponJson = true;
                        param.Add(WeaponKey(weaponId), JsonConvert.SerializeObject(weaponData));
                    }
                    else
                        weaponData = JsonConvert.DeserializeObject<WeaponData>(jsonData[WeaponKey(weaponId)].ToString());
                }

                if (!Managers.Game.WeaponDatas.ContainsKey(weaponId))
                    Managers.Game.WeaponDatas.Add(weaponId, weaponData);
                else
                    Managers.Game.WeaponDatas[weaponId] = weaponData;
            }

            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}