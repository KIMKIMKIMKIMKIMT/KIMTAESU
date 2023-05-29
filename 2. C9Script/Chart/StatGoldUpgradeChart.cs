using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class StatGoldUpgradeChart : IChart<int>
    {
        public ObscuredInt StatId;
        public ObscuredInt MaxLevel;

        public int GetID()
        {
            return StatId;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Stat_Id"].ToString(), out int statId);
            StatId = statId;
            
            int.TryParse(jsonData["Max_Level"].ToString(), out int maxLevel);
            MaxLevel = maxLevel;

            ChartManager.StatGoldUpgradeCharts[GetID()] = this;
        }
    }
}