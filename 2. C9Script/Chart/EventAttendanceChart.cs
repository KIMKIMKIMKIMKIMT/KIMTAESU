using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class EventAttendanceChart : IChart<int>
    {
        public ObscuredInt Id;
        public ItemType RewardType;
        public ObscuredInt[] RewardIds;
        public ObscuredDouble[] RewardValues;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["EventAttendance_Id"].ToString(), out var id);
            Id = id;

            Enum.TryParse(jsonData["Reward_Type"].ToString(), out RewardType);

            var rewardIds = Array.ConvertAll(jsonData["Reward_Id"].ToString().Trim().Split(','), int.Parse);
            RewardIds = new ObscuredInt[rewardIds.Length];
            for (var i = 0; i < rewardIds.Length; i++)
                RewardIds[i] = rewardIds[i];

            var rewardValues = Array.ConvertAll(jsonData["Reward_Value"].ToString().Trim().Split(','), double.Parse);
            RewardValues = new ObscuredDouble[rewardValues.Length];
            for (var i = 0; i < rewardValues.Length; i++)
                RewardValues[i] = rewardValues[i];

            ChartManager.EventAttendanceCharts.TryAdd(GetID(), this);
        }
    }
}