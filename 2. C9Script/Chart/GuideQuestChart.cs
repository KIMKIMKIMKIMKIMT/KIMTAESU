using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using UnityEngine;

namespace Chart
{
    public class GuideQuestChart : IChart<int>
    {
        public ObscuredInt GuideQuestId;
        public QuestProgressType QuestProgressType;
        public ObscuredInt QuestProgressId;
        public ObscuredInt QuestCompleteValue;
        public ItemType RewardItemType;
        public ObscuredInt RewardItemId;
        public ObscuredDouble RewardItemValue;
        public ObscuredString Desc;
        
        public int GetID()
        {
            return GuideQuestId;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["GuideQuest_Id"].ToString(), out int guideQuestId);
            GuideQuestId = guideQuestId;
            
            Enum.TryParse(jsonData["Quest_Progress_Type"].ToString(), out QuestProgressType);
            
            int.TryParse(jsonData["Quest_Progress_Id"].ToString(), out int questProgressId);
            QuestProgressId = questProgressId;
            
            int.TryParse(jsonData["Quest_Complete_Value"].ToString(), out int questCompleteValue);
            QuestCompleteValue = questCompleteValue;
            
            Enum.TryParse(jsonData["GuideQuest_Reward_Type"].ToString(), out RewardItemType);
            
            int.TryParse(jsonData["GuideQuest_Reward_Id"].ToString(), out int rewardItemId);
            RewardItemId = rewardItemId;

            double.TryParse(jsonData["GuideQuest_Reward_Value"].ToString(), out double rewardItemValue);
            RewardItemValue = rewardItemValue;
            
            Desc = jsonData["GuideQuest_Desc"].ToString();

            ChartManager.GuideQuestCharts[GetID()] = this;
        }
    }
}