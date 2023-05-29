using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class DpsDungeonChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredDouble StageClearDps;
        public ItemType ClearRewardItemType;
        public ObscuredInt ClearRewardItemId;
        public ObscuredDouble ClearRewardItemValue;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Dps_Dungeon_Id"].ToString(), out int id);
            Id = id;
            
            double.TryParse(jsonData["Stage_Clear_Dps"].ToString(), out double stageClearDps);
            StageClearDps = stageClearDps;
            
            Enum.TryParse(jsonData["Stage_Clear_Reward_Item_Type"].ToString(), out ClearRewardItemType);
            
            int.TryParse(jsonData["Stage_Clear_Reward_Item_Id"].ToString(), out int clearRewardItemId);
            ClearRewardItemId = clearRewardItemId;
            
            double.TryParse(jsonData["Stage_Clear_Reward_Item_Value"].ToString(), out double clearRewardItemValue);
            ClearRewardItemValue = clearRewardItemValue;

            ChartManager.DpsDungeonCharts[GetID()] = this;
        }
    }
}