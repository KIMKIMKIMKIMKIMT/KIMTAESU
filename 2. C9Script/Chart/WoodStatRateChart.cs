using LitJson;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Chart
{
    public class WoodStatRateChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt StatId;
        public ObscuredDouble Rate;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["StatRate_Id"].ToString(), out int id);
            Id = id;

            int.TryParse(jsonData["Stat_Type"].ToString(), out int statId);
            StatId = statId;

            double.TryParse(jsonData["Rate"].ToString(), out double rate);
            Rate = rate;

            ChartManager.WoodStatRateCharts.TryAdd(GetID(), this);
        }
    }
}


