using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class MissionChart : IChart<int>
    {
        public ObscuredInt MissionId;
        public MissionType MissionType;
        public ObscuredInt SubLevel;
        public ObscuredInt MissionValue;
        public ResetType ResetType;
        public ItemType[] RewardItemTypes;
        public ObscuredInt[] RewardItemIds;
        public ObscuredInt[] RewardItemValues;
        
        public int GetID()
        {
            return MissionId;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Mission_Id"].ToString(), out int missionId);
            MissionId = missionId;
            
            Enum.TryParse(jsonData["Mission_Type"].ToString(), out MissionType);
            
            int.TryParse(jsonData["Sub_Level"].ToString(), out int subLevel);
            SubLevel = subLevel;
            
            int.TryParse(jsonData["Mission_Value"].ToString(), out int missionValue);
            MissionValue = missionValue;
            
            Enum.TryParse(jsonData["Reset_Type"].ToString(), out ResetType);
            
            RewardItemTypes = Array.ConvertAll(jsonData["Reward_Item_Type"].ToString().Trim().Split(','), Enum.Parse<ItemType>);
            
            int[] rewardItemIds = Array.ConvertAll(jsonData["Reward_Item_Id"].ToString().Trim().Split(','), int.Parse);
            RewardItemIds = new ObscuredInt[rewardItemIds.Length];
            for (int i = 0; i < rewardItemIds.Length; i++)
                RewardItemIds[i] = rewardItemIds[i];
            
            int[] rewardItemValues = Array.ConvertAll(jsonData["Reward_Item_Value"].ToString().Trim().Split(','), int.Parse);
            RewardItemValues = new ObscuredInt[rewardItemValues.Length];
            for (int i = 0; i < rewardItemValues.Length; i++)
                RewardItemValues[i] = rewardItemValues[i];

            ChartManager.MissionCharts[GetID()] = this;
        }
    }
}