using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class UnlimitedPointUpgradeLevelChart : IChart<int>
    {
        public ObscuredInt StatId;
        public ObscuredInt UpgradeLevel;
        public ObscuredDouble LevelIncreaseValue;
        public ObscuredInt UnlimitedPointIncreaseGap;
        
        public int GetID()
        {
            return StatId;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Stat_Id"].ToString(), out int statId);
            StatId = statId;

            int.TryParse(jsonData["Upgrade_Level"].ToString(), out int upgradeLevel);
            UpgradeLevel = upgradeLevel;

            double.TryParse(jsonData["Level_Increase_Value"].ToString(), out double levelIncreaseValue);
            LevelIncreaseValue = levelIncreaseValue;

            int.TryParse(jsonData["UnlimitedPoint_Increase_Gap"].ToString(), out int unlimitedPointIncreaseGap);
            UnlimitedPointIncreaseGap = unlimitedPointIncreaseGap;

            ChartManager.UnlimitedPointUpgradeLevelCharts.TryAdd(GetID(), this);
        }
    }
}