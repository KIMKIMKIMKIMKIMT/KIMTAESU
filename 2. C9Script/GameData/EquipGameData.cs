using System;
using System.Runtime.ConstrainedExecution;
using BackEnd;
using LitJson;
using UnityEngine;

namespace GameData
{
    public enum EquipType
    {
        Weapon,
        Costume,
        Pet,
        ShowCostume
    }

    public class EquipGameData : BaseGameData
    {
        public override string TableName => "Equip";
        protected override string InDate { get; set; }

        protected override Param MakeInitData()
        {
            Param param = new Param
            {
                { EquipType.Weapon.ToString(), ChartManager.SystemCharts[SystemData.Default_Weapon].Value.GetDecrypted() },
                { EquipType.Pet.ToString(), ChartManager.SystemCharts[SystemData.Default_Pet].Value.GetDecrypted() },
                { EquipType.Costume.ToString(), ChartManager.SystemCharts[SystemData.Default_Costume].Value.GetDecrypted() },
                { EquipType.ShowCostume.ToString(), ChartManager.SystemCharts[SystemData.Default_Costume].Value.GetDecrypted() }
                
            };

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            foreach (EquipType equipType in Enum.GetValues(typeof(EquipType)))
            {
                if (!Managers.Game.EquipDatas.ContainsKey(equipType))
                    continue;

                param.Add(equipType.ToString(), Managers.Game.EquipDatas[equipType]);
            }

            return param;
        }

        protected override Param MakeSaveData(int id)
        {
            EquipType equipType = (EquipType)id;

            Param param = new Param()
            {
                { equipType.ToString(), Managers.Game.EquipDatas[equipType] }
            };

            return param;
        }

        public void SaveGameData(EquipType equipType)
        {
            SaveGameData((int)equipType);
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            foreach (EquipType equipType in Enum.GetValues(typeof(EquipType)))
            {
                int equipItemIndex;

                if (!jsonData.ContainsKey(equipType.ToString()))
                {
                    int defaultValue = 0;

                    switch (equipType)
                    {
                        case EquipType.Weapon:
                            defaultValue = (int)ChartManager.SystemCharts[SystemData.Default_Weapon].Value.GetDecrypted();
                            break;
                        case EquipType.Costume:
                        case EquipType.ShowCostume:
                            defaultValue = (int)ChartManager.SystemCharts[SystemData.Default_Costume].Value.GetDecrypted();
                            break;
                        case EquipType.Pet:
                            defaultValue = (int)ChartManager.SystemCharts[SystemData.Default_Pet].Value.GetDecrypted();
                            break;
                        
                    }

                    param.Add(equipType.ToString(), defaultValue);
                    equipItemIndex = defaultValue;
                }
                else
                {
                    int.TryParse(jsonData[equipType.ToString()].ToString(),
                        out equipItemIndex);
                }

                if (!Managers.Game.EquipDatas.ContainsKey(equipType))
                    Managers.Game.EquipDatas.Add(equipType, equipItemIndex);
                else
                    Managers.Game.EquipDatas[equipType] = equipItemIndex;
            }
            
            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}