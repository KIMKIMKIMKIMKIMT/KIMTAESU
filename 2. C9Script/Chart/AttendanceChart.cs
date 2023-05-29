using System;
using CodeStage.AntiCheat.ObscuredTypes;
using GoogleMobileAds.Api;
using LitJson;

namespace Chart
{
    public class AttendanceChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString Name;
        public ItemType RewardType;
        public ObscuredInt RewardId;
        public ObscuredDouble RewardValue;
        
        public int GetID()
        {
            return Id;  
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Attendance_Id"].ToString(), out int id);
            Id = id;
            
            Name = jsonData["Attendance_Name"].ToString();
            Enum.TryParse(jsonData["Attendance_Reward_Type"].ToString(), out RewardType);
            
            int.TryParse(jsonData["Attendance_Reward_Id"].ToString(), out int rewardId);
            RewardId = rewardId;

            double.TryParse(jsonData["Attendance_Reward_Value"].ToString(), out double rewardValue);
            RewardValue = rewardValue;

            ChartManager.AttendanceCharts[GetID()] = this;
        }
    }
}