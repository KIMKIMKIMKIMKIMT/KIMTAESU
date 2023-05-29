using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class DungeonChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString Name;
        public ObscuredString Desc;
        public ObscuredInt WorldId;
        public ObscuredInt EntryItemId;
        public ObscuredInt RewardItemId;
        public ObscuredInt MaxStep;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Dungeon_Id"].ToString(), out int id);
            Id = id;
            
            Name = jsonData["Dungeon_Name"].ToString();
            
            Desc = jsonData["Dungeon_Desc"].ToString();
            
            int.TryParse(jsonData["World_Id"].ToString(), out int worldId);
            WorldId = worldId;
            
            int.TryParse(jsonData["EntryGoodsType"].ToString(), out int entryItemId);
            EntryItemId = entryItemId;
            
            int.TryParse(jsonData["RewardGoodsType"].ToString(), out int rewardItemId);
            RewardItemId = rewardItemId;
            
            int.TryParse(jsonData["Max_Step"].ToString(), out int maxStep);
            MaxStep = maxStep;

            ChartManager.DungeonCharts[GetID()] = this;
        }
    }
}