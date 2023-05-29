using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using UnityEngine.UIElements;

namespace Chart
{
    public class CostumeChart : IChart<int>
    {
        public ObscuredInt Id;
        public string Name;
        public ObscuredInt Sort;
        public Grade Grade;
        public ObscuredInt EquipStatType1;
        public ObscuredDouble EquipStatValue1;
        public ObscuredInt EquipStatType2;
        public ObscuredDouble EquipStatValue2;
        public ObscuredInt HaveStatType1;
        public ObscuredDouble HaveStatValue1;
        public ObscuredInt HaveStatType2;
        public ObscuredDouble HaveStatValue2;
        public ObscuredInt MaxAwakening;
        public ObscuredString Icon;
        public ObscuredInt SetId;
        
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["CostumeTable_Id"].ToString(), out var id);
            Id = id;
            
            Name = jsonData["Costume_Name"].ToString();

            int.TryParse(jsonData["Sort"].ToString(), out var sort);
            Sort = sort;
            
            Enum.TryParse(jsonData["Grade"].ToString(), out Grade);
            
            int.TryParse(jsonData["Equip_Stat_Type_1"].ToString(), out int equipStatType1);
            EquipStatType1 = equipStatType1;
            
            double.TryParse(jsonData["Equip_Stat_Type_1_Value"].ToString(), out double equipStatValue1);
            EquipStatValue1 = equipStatValue1;
            
            int.TryParse(jsonData["Equip_Stat_Type_2"].ToString(), out int equipStatType2);
            EquipStatType2 = equipStatType2;
            
            double.TryParse(jsonData["Equip_Stat_Type_2_Value"].ToString(), out double equipStatValue2);
            EquipStatValue2 = equipStatValue2;
            
            int.TryParse(jsonData["HaveStat_Type_1"].ToString(), out int haveStatType1);
            HaveStatType1 = haveStatType1;
            
            double.TryParse(jsonData["HaveStat_Value_1"].ToString(), out double haveStatValue1);
            HaveStatValue1 = haveStatValue1;

            int.TryParse(jsonData["HaveStat_Type_2"].ToString(), out int haveStatType2);
            HaveStatType2 = haveStatType2;
            
            double.TryParse(jsonData["HaveStat_Value_2"].ToString(), out double haveStatValue2);
            HaveStatValue2 = haveStatValue2;

            int.TryParse(jsonData["Max_Awakening"].ToString(), out int maxAwakening);
            MaxAwakening = maxAwakening;
            
            Icon = jsonData["Icon"].ToString();

            int.TryParse(jsonData["Set_Id"].ToString(), out int setId);
            SetId = setId;

            ChartManager.CostumeCharts[GetID()] = this;
        }
    }
}