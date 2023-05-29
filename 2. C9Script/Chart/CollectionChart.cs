using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class CollectionChart : IChart<int>
    {
        public ObscuredInt CollectionId;
        public ObscuredString CollectionName;
        public ObscuredInt MaxLevel;
        public ObscuredInt StatType;
        public ObscuredDouble StatIncreaseValue;
        public ObscuredInt UpgradeValue;
        public ObscuredInt UpgradeIncreaseGap;
        public ObscuredString Icon;
        
        public int GetID()
        {
            return CollectionId;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Collection_Id"].ToString(), out int collectionId);
            CollectionId = collectionId;
            
            CollectionName = jsonData["Collection_Name"].ToString();
            
            int.TryParse(jsonData["Max_Level"].ToString(), out int maxLevel);
            MaxLevel = maxLevel;
            
            int.TryParse(jsonData["StatIncrease_Type"].ToString(), out int statType);
            StatType = statType;
            
            double.TryParse(jsonData["StatIncrease_Value"].ToString(), out double statIncreaseValue);
            StatIncreaseValue = statIncreaseValue;
            
            int.TryParse(jsonData["Upgrade_Item_Value"].ToString(), out int upgradeValue);
            UpgradeValue = upgradeValue;
            
            int.TryParse(jsonData["Upgrade_Item_Increase_Gap"].ToString(), out int upgradeIncreaseGap);
            UpgradeIncreaseGap = upgradeIncreaseGap;
            
            Icon = jsonData["Icon"].ToString();

            ChartManager.CollectionCharts[GetID()] = this;
        }
    }
}