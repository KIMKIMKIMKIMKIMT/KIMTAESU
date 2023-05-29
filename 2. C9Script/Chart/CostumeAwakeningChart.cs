using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class CostumeAwakeningChart : IChart<(int, int)>
    {
        public ObscuredInt AwakeningId;
        public ObscuredInt AwakeningLv;
        public ObscuredInt AwakeningItemId;
        public ObscuredInt AwakeningItemValue;
        public ObscuredInt StatType;
        public ObscuredDouble StatValue;

        public (int, int) GetID()
        {
            return (AwakeningItemId, AwakeningLv);
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Awakening_Id"].ToString(), out int awakeningId);
            AwakeningId = awakeningId;
            
            int.TryParse(jsonData["Awakening_Level"].ToString(), out int awakeningLv);
            AwakeningLv = awakeningLv;
            
            int.TryParse(jsonData["Awakening_Item_Id"].ToString(), out int awakeningItemId);
            AwakeningItemId = awakeningItemId;
            
            int.TryParse(jsonData["Awakening_Item_Value"].ToString(), out int awakeningItemValue);
            AwakeningItemValue = awakeningItemValue;
            
            int.TryParse(jsonData["Awakening_Stat_Type"].ToString(), out int statType);
            StatType = statType;
            
            double.TryParse(jsonData["Awakening_Stat_Value"].ToString(), out double statValue);
            StatValue = statValue;

            ChartManager.CostumeAwakeningCharts[GetID()] = this;
        }
    }
}