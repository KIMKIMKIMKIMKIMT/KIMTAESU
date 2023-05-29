using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class PetChart : IChart<int>
    {
        public ObscuredInt Id;
        public PetType Type;
        public Grade Grade;
        public ObscuredInt SubGrade;
        public ObscuredInt EquipStatType1;
        public ObscuredDouble EquipStatValue1;
        public ObscuredDouble EquipStatUpgradeValue1;
        public ObscuredInt EquipStatType2;
        public ObscuredDouble EquipStatValue2;
        public ObscuredDouble EquipStatUpgradeValue2;
        public ObscuredInt EquipStatType3;
        public ObscuredDouble EquipStatValue3;
        public ObscuredDouble EquipStatUpgradeValue3;
        public ObscuredInt EquipStatType4;
        public ObscuredDouble EquipStatValue4;
        public ObscuredDouble EquipStatUpgradeValue4;
        public ObscuredInt HaveStatType1;
        public ObscuredDouble HaveStatValue1;
        public ObscuredDouble HaveStatUpgradeValue1;
        public ObscuredInt HaveStatType2;
        public ObscuredDouble HaveStatValue2;
        public ObscuredDouble HaveStatUpgradeValue2;
        public ObscuredInt HaveStatType3;
        public ObscuredDouble HaveStatValue3;
        public ObscuredDouble HaveStatUpgradeValue3;
        public ObscuredInt HaveStatType4;
        public ObscuredDouble HaveStatValue4;
        public ObscuredDouble HaveStatUpgradeValue4;
        public ObscuredInt MaxLevel;
        public ItemType LevelUpItemType;
        public ObscuredInt LevelUpItemId;
        public ObscuredDouble LevelUpItemValue;
        public ObscuredDouble LevelUpItemIncreaseValue;
        public ObscuredInt CombineCount;
        public ObscuredInt CombineResult;
        public ObscuredString PetName;
        public ObscuredString Icon;


        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Pet_Id"].ToString(), out int id);
            Id = id;
            
            Enum.TryParse(jsonData["Pet_Type"].ToString(), out Type);
            
            Enum.TryParse(jsonData["Pet_Grade"].ToString(), out Grade);
            
            int.TryParse(jsonData["Pet_Sub_Grade"].ToString(), out int subGrade);
            SubGrade = subGrade;
            
            int.TryParse(jsonData["Stat_Type_1"].ToString(), out int equipStatType1);
            EquipStatType1 = equipStatType1;
            
            double.TryParse(jsonData["Stat_Value_1"].ToString(), out double equipStatValue1);
            EquipStatValue1 = equipStatValue1;
            
            double.TryParse(jsonData["Stat_Upgrade_Value_1"].ToString(), out double equipStatUpgradeValue1);
            EquipStatUpgradeValue1 = equipStatUpgradeValue1;
            
            int.TryParse(jsonData["Stat_Type_2"].ToString(), out int equipStatType2);
            EquipStatType2 = equipStatType2;
            
            double.TryParse(jsonData["Stat_Value_2"].ToString(), out double equipStatValue2);
            EquipStatValue2 = equipStatValue2;
            
            double.TryParse(jsonData["Stat_Upgrade_Value_2"].ToString(), out double equipStatUpgradeValue2);
            EquipStatUpgradeValue2 = equipStatUpgradeValue2;
            
            int.TryParse(jsonData["Stat_Type_3"].ToString(), out int equipStatType3);
            EquipStatType3 = equipStatType3;
            
            double.TryParse(jsonData["Stat_Value_3"].ToString(), out double equipStatValue3);
            EquipStatValue3 = equipStatValue3;
            
            double.TryParse(jsonData["Stat_Upgrade_Value_3"].ToString(), out double equipStatUpgradeValue3);
            EquipStatUpgradeValue3 = equipStatUpgradeValue3;
            
            int.TryParse(jsonData["Stat_Type_4"].ToString(), out int equipStatType4);
            EquipStatType4 = equipStatType4;

            double.TryParse(jsonData["Stat_Value_4"].ToString(), out double equipStatValue4);
            EquipStatValue4 = equipStatValue4;
            
            double.TryParse(jsonData["Stat_Upgrade_Value_4"].ToString(), out double equipStatUpgradeValue4);
            EquipStatUpgradeValue4 = equipStatUpgradeValue4;

            int.TryParse(jsonData["HaveStat_Type_1"].ToString(), out int haveStatType1);
            HaveStatType1 = haveStatType1;
            
            double.TryParse(jsonData["HaveStat_Value_1"].ToString(), out double haveStatValue1);
            HaveStatValue1 = haveStatValue1;
            
            double.TryParse(jsonData["HaveStat_Upgrade_Value_1"].ToString(), out double haveStatUpgradeValue1);
            HaveStatUpgradeValue1 = haveStatUpgradeValue1;
            
            int.TryParse(jsonData["HaveStat_Type_2"].ToString(), out int haveStatType2);
            HaveStatType2 = haveStatType2;
            
            double.TryParse(jsonData["HaveStat_Value_2"].ToString(), out double haveStatValue2);
            HaveStatValue2 = haveStatValue2;
            
            double.TryParse(jsonData["HaveStat_Upgrade_Value_2"].ToString(), out double haveStatUpgradeValue2);
            HaveStatUpgradeValue2 = haveStatUpgradeValue2;
            
            int.TryParse(jsonData["HaveStat_Type_3"].ToString(), out int haveStatType3);
            HaveStatType3 = haveStatType3;
            
            double.TryParse(jsonData["HaveStat_Value_3"].ToString(), out double haveStatValue3);
            HaveStatValue3 = haveStatValue3;
            
            double.TryParse(jsonData["HaveStat_Upgrade_Value_3"].ToString(), out double haveStatUpgradeValue3);
            HaveStatUpgradeValue3 = haveStatUpgradeValue3;
            
            int.TryParse(jsonData["HaveStat_Type_4"].ToString(), out int haveStatType4);
            HaveStatType4 = haveStatType4;
            
            double.TryParse(jsonData["HaveStat_Value_4"].ToString(), out double haveStatValue4);
            HaveStatValue4 = haveStatValue4;
            
            double.TryParse(jsonData["HaveStat_Upgrade_Value_4"].ToString(), out double haveStatUpgradeValue4);
            HaveStatUpgradeValue4 = haveStatUpgradeValue4;

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
            
            int.TryParse(jsonData["Combine_Result"].ToString(), out int combineResult);
            CombineResult = combineResult;
            
            PetName = jsonData["Pet_Name"].ToString();
            Icon = jsonData["Icon"].ToString();

            ChartManager.PetCharts[GetID()] = this;
        }
    }
}