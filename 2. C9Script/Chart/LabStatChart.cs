using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class LabStatChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt StatId;
        public Grade Grade;
        public ObscuredDouble MinValue;
        public ObscuredDouble MaxValue;
        public ObscuredString MinValueString;
        public ObscuredString MaxValueString;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["SkillAwakening_Id"].ToString(), out int id);
            Id = id;

            int.TryParse(jsonData["Stat_Type"].ToString(), out int statId);
            StatId = statId;

            Enum.TryParse(jsonData["Grade"].ToString(), out Grade);

            double.TryParse(jsonData["Min_Value"].ToString(), out double minValue);
            MinValue = minValue;
            MinValueString = jsonData["Min_Value"].ToString();
            
            double.TryParse(jsonData["Max_Value"].ToString(), out double maxValue);
            MaxValue = maxValue;
            MaxValueString = jsonData["Max_Value"].ToString();
            
            ChartManager.LabStatCharts.TryAdd(GetID(), this);
        }
    }
}