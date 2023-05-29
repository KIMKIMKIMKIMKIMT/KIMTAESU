using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class RankingRewardChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString Desc;
        public ItemType RewardItemType;
        public ObscuredInt RewardItemId;
        public ObscuredInt RewardItemValue;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Ranking_Id"].ToString(), out int id);
            Id = id;
            
            Desc = jsonData["Ranking_Desc"].ToString();
            
            Enum.TryParse(jsonData["Ranking_Reward_Type"].ToString(), out RewardItemType);
            
            int.TryParse(jsonData["Ranking_Reward_Id"].ToString(), out int rewardItemId);
            RewardItemId = rewardItemId;
            
            int.TryParse(jsonData["Ranking_Reward_Value"].ToString(), out int rewardItemValue);
            RewardItemValue = rewardItemValue;
        }
    }
}