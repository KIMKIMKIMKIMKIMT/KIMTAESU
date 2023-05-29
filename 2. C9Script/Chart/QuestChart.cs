using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class QuestChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString Name;
        public QuestType Type;
        public QuestProgressType ProgressType;
        public ObscuredLong CompleteValue;
        public ItemType RewardType;
        public ObscuredInt RewardId;
        public ObscuredDouble RewardValue;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Quest_Id"].ToString(), out int id);
            Id = id;
            
            Name = jsonData["Quest_Name"].ToString();
            Enum.TryParse(jsonData["Quest_Type"].ToString(), out Type);
            Enum.TryParse(jsonData["Quest_Progress_Type"].ToString(), out ProgressType);
            
            long.TryParse(jsonData["Quest_Complete_Value"].ToString(), out long completeValue);
            CompleteValue = completeValue;
            
            Enum.TryParse(jsonData["Quest_Reward_Type"].ToString(), out RewardType);
            
            int.TryParse(jsonData["Quest_Reward_Id"].ToString(), out int rewardId);
            RewardId = rewardId;
            
            double.TryParse(jsonData["Quest_Reward_Value"].ToString(), out double rewardValue);
            RewardValue = rewardValue;

            ChartManager.QuestCharts[GetID()] = this;
        }
    }
}