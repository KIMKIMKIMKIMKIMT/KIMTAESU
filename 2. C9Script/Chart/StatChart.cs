using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class StatChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString Name;
        public ObscuredInt DefaultValue;
        public ValueType ValueType;
        public ObscuredFloat FeverValue;
        public ObscuredString Icon;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Stat_Id"].ToString(), out int id);
            Id = id;
            
            Name = jsonData["Stat_Name"].ToString();
            
            int.TryParse(jsonData["Default_Value"].ToString(), out int defaultValue);
            DefaultValue = defaultValue;
            
            Enum.TryParse(jsonData["Value_Type"].ToString(), out ValueType);
            
            float.TryParse(jsonData["Fever"].ToString(), out float feverValue);
            FeverValue = feverValue;
            
            Icon = jsonData["Icon"].ToString();

            ChartManager.StatCharts[GetID()] = this;
        }
    }
}