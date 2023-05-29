using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using System;

namespace Chart
{
    public class WoodAwakeningDataChart : IChart<int>
    {
        public ObscuredInt Id;
        public Grade Grade;
        public ObscuredInt OpenCost;
        public ObscuredInt AwakeningCost;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["WoodAwakening_Id"].ToString(), out int id);
            Id = id;

            Enum.TryParse(jsonData["Open_Grade"].ToString(), out Grade);

            int.TryParse(jsonData["Open_Cost"].ToString(), out int openCost);
            OpenCost = openCost;

            int.TryParse(jsonData["Awakening_Cost"].ToString(), out int awakeningCost);
            AwakeningCost = awakeningCost;

            ChartManager.WoodAwakeningDataCharts.TryAdd(GetID(), this);
        }
    }
}

