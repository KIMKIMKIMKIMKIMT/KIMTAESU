using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class WorldChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt WorldType;
        public ObscuredString BgName;
        public ObscuredString BgDesc;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["World_Id"].ToString(), out int id);
            Id = id;

            int.TryParse(jsonData["World_Type"].ToString(), out int worldType);
            WorldType = worldType;
            
            BgName = jsonData["World_Res"].ToString();
            BgDesc = jsonData["World_Desc"].ToString();

            ChartManager.WorldCharts[GetID()] = this;
        }
    }
}