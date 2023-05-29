using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class PromoDungeonChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt WorldId;
        public ObscuredString Name;
        public ObscuredFloat ClearLimitTime;
        public ObscuredInt BossId;
        public ObscuredDouble BossHp;
        public ObscuredInt ClearRewardStat1Id;
        public ObscuredDouble ClearRewardStat1Value;
        public ObscuredInt ClearRewardStat2Id;
        public ObscuredInt ClearRewardStat3Id;
        public ObscuredString Icon;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Promo_Id"].ToString(), out int id);
            Id = id;
            
            int.TryParse(jsonData["World_Id"].ToString(), out int worldId);
            WorldId = worldId;
            
            Name = jsonData["Promo_Name"].ToString();
            
            float.TryParse(jsonData["Promo_Clear_Limit_Time"].ToString(), out float clearLimitTime);
            ClearLimitTime = clearLimitTime;
            
            int.TryParse(jsonData["Boss_Id"].ToString(), out int bossId);
            BossId = bossId;
            
            double.TryParse(jsonData["Boss_Hp"].ToString(), out double bossHp);
            BossHp = bossHp;
            
            int.TryParse(jsonData["Clear_Reward_Stat_1"].ToString(), out int clearRewardStat1Id);
            ClearRewardStat1Id = clearRewardStat1Id;
            
            double.TryParse(jsonData["Clear_Reward_Stat_1_Value"].ToString(), out double clearRewardStat1Value);
            ClearRewardStat1Value = clearRewardStat1Value;
            
            int.TryParse(jsonData["Clear_Reward_Stat_2"].ToString(), out int clearRewardStat2Id);
            ClearRewardStat2Id = clearRewardStat2Id;
            
            int.TryParse(jsonData["Clear_Reward_Stat_3"].ToString(), out int clearRewardStat3Id);
            ClearRewardStat3Id = clearRewardStat3Id;
            
            Icon = jsonData["Icon"].ToString();

            ChartManager.PromoDungeonCharts[GetID()] = this;
        }
    }
}