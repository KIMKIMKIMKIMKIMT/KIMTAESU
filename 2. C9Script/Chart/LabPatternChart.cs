using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class LabPatternChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString Icon;
        public ObscuredString Name;
        public ObscuredDouble Rate;
        public ObscuredString RateString;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Pattern_Id"].ToString(), out int id);
            Id = id;

            Icon = jsonData["Icon"].ToString();

            Name = jsonData["Name"].ToString();

            double.TryParse(jsonData["Gotcha_Rate"].ToString(), out double rate);
            Rate = rate;
            RateString = jsonData["Gotcha_Rate"].ToString();
            
            ChartManager.LabPatternCharts.TryAdd(GetID(), this);
        }
    }
}