using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class LevelChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredDouble Exp;
        public ItemType RewardItemType;
        public ObscuredInt[] RewardItemIds;
        public ObscuredInt[] RewardItemValues;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["LevelTable_Id"].ToString(), out int id);
            Id = id;
            
            double.TryParse(jsonData["Exp"].ToString(), out double exp);
            Exp = exp;
            
            Enum.TryParse(jsonData["Reward_Type"].ToString(), out RewardItemType);
            
            int[] rewardItemIds = Array.ConvertAll(jsonData["Reward_Id"].ToString().Trim().Split(','), int.Parse);
            RewardItemIds = new ObscuredInt[rewardItemIds.Length];
            for (int i = 0; i < rewardItemIds.Length; i++)
                RewardItemIds[i] = rewardItemIds[i];
            
            int[] rewardItemValues = Array.ConvertAll(jsonData["Reward_Value"].ToString().Trim().Split(','), int.Parse);
            RewardItemValues = new ObscuredInt[rewardItemValues.Length];
            for (int i = 0; i < rewardItemValues.Length; i++)
                RewardItemValues[i] = rewardItemValues[i];

            ChartManager.LevelCharts[GetID()] = this;
        }
    }
}