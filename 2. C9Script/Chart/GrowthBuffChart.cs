using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class GrowthBuffChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString BuffName;
        public ObscuredInt ApplyMaxLv;
        public ObscuredInt[] BuffStatIds;
        public ObscuredDouble[] BuffStatValues;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Buff_Id"].ToString(), out var id);
            Id = id;

            BuffName = jsonData["Buff_Name"].ToString();

            int.TryParse(jsonData["Char_Level"].ToString(), out var charLevel);
            ApplyMaxLv = charLevel;

            int[] buffStatIds = Array.ConvertAll(jsonData["Buff_Stat_Ids"].ToString().Trim().Split(','), int.Parse);
            BuffStatIds = new ObscuredInt[buffStatIds.Length];
            for (int i = 0; i < buffStatIds.Length; i++)
                BuffStatIds[i] = buffStatIds[i];
            
            
            double[] buffStatValues = Array.ConvertAll(jsonData["Buff_Stat_Values"].ToString().Trim().Split(','), double.Parse);
            BuffStatValues = new ObscuredDouble[buffStatValues.Length];
            for (int i = 0; i < buffStatValues.Length; i++)
                BuffStatValues[i] = buffStatValues[i];

            ChartManager.GrowthBuffCharts.TryAdd(GetID(), this);
        }
    }
}