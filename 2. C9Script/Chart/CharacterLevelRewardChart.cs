using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    // Key : StatType, Level
    public class CharacterLevelRewardChart : IChart<int>
    {
        public ObscuredInt Level;
        public ObscuredInt StatId;
        public ObscuredFloat IncreaseValue;
        public ItemType[] LevelUpRewardItemTypes;
        public ObscuredInt[] LevelUpRewardItemIds;
        public ObscuredDouble[] LevelUpRewardItemValues;
        
        public int GetID()
        {
            return Level;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Level"].ToString(), out int level);
            Level = level;
            
            int.TryParse(jsonData["Stat_Id"].ToString(), out int statId);
            StatId = statId;
            
            float.TryParse(jsonData["Increase_Value"].ToString(), out float increaseValue);
            IncreaseValue = increaseValue;
            
            LevelUpRewardItemTypes = Array.ConvertAll(jsonData["LevelUp_Reward_Type"].ToString().Trim().Split(','),
                Enum.Parse<ItemType>);
            
            int[] levelUpRewardItemIds = Array.ConvertAll(jsonData["LevelUp_Reward_Id"].ToString().Trim().Split(','), int.Parse);
            LevelUpRewardItemIds = new ObscuredInt[levelUpRewardItemIds.Length];
            for (int i = 0; i < levelUpRewardItemIds.Length; i++)
                LevelUpRewardItemIds[i] = levelUpRewardItemIds[i];
            
            double[] levelUpRewardItemValues = Array.ConvertAll(jsonData["LevelUp_Reward_Value"].ToString().Trim().Split(','),
                double.Parse);
            LevelUpRewardItemValues = new ObscuredDouble[levelUpRewardItemValues.Length];
            for (int i = 0; i < levelUpRewardItemValues.Length; i++)
                LevelUpRewardItemValues[i] = levelUpRewardItemValues[i];
        }
    }
}