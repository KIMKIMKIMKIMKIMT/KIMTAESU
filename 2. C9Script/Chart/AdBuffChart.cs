using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class AdBuffChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt BuffStatType;
        public ObscuredFloat BuffValue;
        public ObscuredInt Duration;
        public ObscuredString Icon;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["AD_Buff_Id"].ToString(), out int id);
            Id = id;
            
            int.TryParse(jsonData["Ad_Buff_Stat_Type"].ToString(), out int buffStatType);
            BuffStatType = buffStatType;
            
            float.TryParse(jsonData["Buff_Value"].ToString(), out float buffValue);
            BuffValue = buffValue;
            
            int.TryParse(jsonData["Duration"].ToString(), out int duration);
            Duration = duration;
            
            Icon = jsonData["Icon"].ToString();

            ChartManager.AdBuffCharts[GetID()] = this;
        }
    }
}