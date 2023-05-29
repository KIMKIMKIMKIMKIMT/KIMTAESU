using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class StringChart : IChart<string>
    {
        public ObscuredString Id;
        public ObscuredString Value;
        
        public string GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            Id = jsonData["String_Id"].ToString();
            Value = jsonData["KR"].ToString();

            ChartManager.StringCharts[GetID()] = this;
        }
    }
}