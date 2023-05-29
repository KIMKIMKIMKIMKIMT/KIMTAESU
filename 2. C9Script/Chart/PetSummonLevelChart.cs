using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class PetSummonLevelChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString NormalRate;
        public ObscuredString RareRate;
        public ObscuredString UniqueRate;
        public ObscuredString LegendRate;
        public ObscuredString LegenoRate;
        public ObscuredInt Exp;
        public ItemType RewardItemType;
        public ObscuredInt RewardItemId;
        public ObscuredInt RewardItemValue;
        
        public int GetID()
        {
            return Id;  
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Summon_Level"].ToString(), out int id);
            Id = id;
            
            NormalRate = jsonData["Normal"].ToString();
            RareRate = jsonData["Rare"].ToString();
            UniqueRate = jsonData["Unique"].ToString();
            LegendRate = jsonData["Legend"].ToString();
            LegenoRate = jsonData["Legeno"].ToString();
            
            int.TryParse(jsonData["Exp"].ToString(), out int exp);
            Exp = exp;
            
            Enum.TryParse(jsonData["Reward_Type"].ToString(), out RewardItemType);
            
            int.TryParse(jsonData["Reward_Id"].ToString(), out int rewardItemId);
            RewardItemId = rewardItemId;
            
            int.TryParse(jsonData["Reward_Value"].ToString(), out int rewardItemValue);
            RewardItemValue = rewardItemValue;

            ChartManager.PetSummonLevelCharts[GetID()] = this;
        }
    }
}