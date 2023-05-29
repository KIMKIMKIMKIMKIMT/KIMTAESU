using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class PvpChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt WorldId;
        public ObscuredInt LimitTime;
        public ObscuredInt WinPoint;
        public ObscuredInt LosePoint;
        public ItemType RewardItemType;
        public ObscuredInt RewardItemId;
        public ObscuredInt RewardItemWinValue;
        public ObscuredInt RewardItemLoseValue;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Pvp_Dungeon_Id"].ToString(), out int id);
            Id = id;
            
            int.TryParse(jsonData["World_Id"].ToString(), out int worldId);
            WorldId = worldId;
            
            int.TryParse(jsonData["Stage_Clear_Limit_Time"].ToString(), out int limitTime);
            LimitTime = limitTime;
            
            int.TryParse(jsonData["Pvp_Win_Point"].ToString(), out int winPoint);
            WinPoint = winPoint;
            
            int.TryParse(jsonData["Pvp_Lose_Point"].ToString(), out int losePoint);
            LosePoint = losePoint;
            
            Enum.TryParse(jsonData["Reward_Item_Type"].ToString(), out RewardItemType);
            
            int.TryParse(jsonData["Reward_Item_Id"].ToString(), out int rewardItemId);
            RewardItemId = rewardItemId;
            
            int.TryParse(jsonData["Win_Item_Value"].ToString(), out int rewardItemWinValue);
            RewardItemWinValue = rewardItemWinValue;
            
            int.TryParse(jsonData["Lose_Item_Value"].ToString(), out int rewardItemLoseValue);
            RewardItemLoseValue = rewardItemLoseValue;

            ChartManager.PvpCharts[GetID()] = this;
        }
    }
}