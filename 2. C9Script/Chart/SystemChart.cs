using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class SystemChart : IChart<SystemData>
    {
        public SystemData Id;
        public ObscuredFloat Value;
        
        public SystemData GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            Enum.TryParse(jsonData["System_Id"].ToString(), out Id);
            
            float.TryParse(jsonData["Value"].ToString(), out float value);
            Value = value;

            ChartManager.SystemCharts[GetID()] = this;
        }
    }
}