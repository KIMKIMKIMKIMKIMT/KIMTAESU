using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class LabAwakeningChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt OpenLv;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Awakening_Id"].ToString(), out int id);
            Id = id;

            int.TryParse(jsonData["Awakening_Level"].ToString(), out int openLv);
            OpenLv = openLv;

            ChartManager.LabAwakeningCharts.TryAdd(GetID(), this);
        }
    }
}