using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public record WeaponChart : IChart<int>
    {
        public ObscuredInt Id;
        public WeaponType Type;
        public Grade Grade;
        public ObscuredInt SubGrade;
        public ObscuredInt EquipStatType;
        public ObscuredDouble EquipStatValue;
        public ObscuredDouble EquipStatUpgradeValue;
        public ObscuredInt HaveStatType;
        public ObscuredDouble HaveStatValue;
        public ObscuredDouble HaveStatUpgradeValue;
        public ObscuredInt MaxLevel;
        public ItemType LevelUpItemType;
        public ObscuredInt LevelUpItemId;
        public ObscuredDouble LevelUpItemValue;
        public ObscuredDouble LevelUpItemIncreaseValue;
        public ObscuredInt CombineCount;
        public ObscuredInt CombineResultId;
        public ObscuredString Name;
        public ObscuredString Icon;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["WeaponTable_Id"].ToString(), out int id);
            Id = id;
            
            Enum.TryParse(jsonData["Weapon_Attribute"].ToString(), out Type);
            
            Enum.TryParse(jsonData["Grade"].ToString(), out Grade);
            
            int.TryParse(jsonData["Sub_Grade"].ToString(), out int subGrade);
            SubGrade = subGrade;
            
            int.TryParse(jsonData["EquipStat_Type"].ToString(), out int equipStatType);
            EquipStatType = equipStatType;
            
            double.TryParse(jsonData["EquipStat_Value"].ToString(), out double equipStatValue);
            EquipStatValue = equipStatValue;
            
            double.TryParse(jsonData["EquipStat_Upgrade_Value"].ToString(), out double equipStatUpgradeValue);
            EquipStatUpgradeValue = equipStatUpgradeValue;
            
            int.TryParse(jsonData["HaveStat_Type"].ToString(), out int haveStatType);
            HaveStatType = haveStatType;
            
            double.TryParse(jsonData["HaveStat_Value"].ToString(), out double haveStatValue);
            HaveStatValue = haveStatValue;
            
            double.TryParse(jsonData["HaveStat_Upgrade_Value"].ToString(), out double haveStatUpgradeValue);
            HaveStatUpgradeValue = haveStatUpgradeValue;
            
            int.TryParse(jsonData["Max_Level"].ToString(), out int maxLevel);
            MaxLevel = maxLevel;
            
            Enum.TryParse(jsonData["Level_Up_Item_Type"].ToString(), out LevelUpItemType);
            
            int.TryParse(jsonData["Level_Up_Item_Id"].ToString(), out int levelUpItemId);
            LevelUpItemId = levelUpItemId;
            
            double.TryParse(jsonData["Level_Up_Item_Value"].ToString(), out double levelUpItemValue);
            LevelUpItemValue = levelUpItemValue;

            double.TryParse(jsonData["Level_Up_Item_Increase_Value"].ToString(), out double levelUpItemIncreaseValue);
            LevelUpItemIncreaseValue = levelUpItemIncreaseValue;
            
            int.TryParse(jsonData["Combine_Count"].ToString(), out int combineCount);
            CombineCount = combineCount;
            
            int.TryParse(jsonData["Combine_Result"].ToString(), out int combineResultId);
            CombineResultId = combineResultId;
            
            Name = jsonData["Weapon_Name"].ToString();
            Icon = jsonData["Icon"].ToString();

            ChartManager.WeaponCharts[GetID()] = this;
        }
    }
}