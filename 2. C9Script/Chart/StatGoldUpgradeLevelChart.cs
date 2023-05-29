using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class StatGoldUpgradeLevelChart : IChart<(int, int)>
    {
        public ObscuredInt StatId;
        public ObscuredInt UpgradeLevel;
        public ObscuredFloat IncreaseValue;
        public ItemType UpgradeItemType;
        public ObscuredInt UpgradeItemId;
        public ObscuredDouble UpgradeItemValue;
        public ObscuredFloat UpgradeItemIncreaseValue;
        
        public (int, int) GetID()
        {
            return (StatId, UpgradeLevel);
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Stat_Id"].ToString(), out int statId);
            StatId = statId;
            
            int.TryParse(jsonData["Upgrade_Level"].ToString(), out int upgradeLevel);
            UpgradeLevel = upgradeLevel;
            
            float.TryParse(jsonData["Level_Increase_Value"].ToString(), out float increaseValue);
            IncreaseValue = increaseValue;
            
            Enum.TryParse(jsonData["Upgrade_Item_Type"].ToString(), out UpgradeItemType);
            
            int.TryParse(jsonData["Upgrade_Item_Id"].ToString(), out int upgradeItemId);
            UpgradeItemId = upgradeItemId;
            
            double.TryParse(jsonData["Upgrade_Item_Value"].ToString(), out double upgradeItemValue);
            UpgradeItemValue = upgradeItemValue;
            
            float.TryParse(jsonData["Upgrade_Item_Increase_Value"].ToString(), out float upgradeItemIncreaseValue);
            UpgradeItemIncreaseValue = upgradeItemIncreaseValue;

            ChartManager.StatGoldUpgradeLevelCharts[GetID()] = this;
        }
    }
}