using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;


namespace Chart
{
    public class WoodGradeRateChart : IChart<int>
    {
        public ObscuredInt Id;
        public Grade Grade;
        public ObscuredDouble Rate;
        public ObscuredString RateString;
        public ObscuredString Bg;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["WoodAwakeningGradeRate_Id"].ToString(), out int id);
            Id = id;

            Enum.TryParse(jsonData["Grade"].ToString(), out Grade);

            double.TryParse(jsonData["Rate"].ToString(), out double rate);
            Rate = rate;
            RateString = jsonData["Rate"].ToString();

            Bg = jsonData["Bg"].ToString();

            ChartManager.WoodGradeRateCharts.TryAdd(GetID(), this);
        }
    }
}

