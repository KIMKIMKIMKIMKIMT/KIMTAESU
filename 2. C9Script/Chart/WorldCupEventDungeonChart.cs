using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class WorldCupEventDungeonChart : IChart<int>
    {
        public ObscuredInt DungeonId;
        public ObscuredInt LimitTime;
        public ObscuredFloat Phase1Hp;
        public ObscuredFloat Phase1Value;
        public ObscuredFloat Phase2Hp;
        public ObscuredFloat Phase2Value;
        public ObscuredFloat Phase3Hp;
        public ObscuredFloat Phase3Value;
        public ObscuredInt MonsterHp;
        public ObscuredDouble HitRewardValue;
        public ObscuredDouble ClearRewardValue;
        public ObscuredFloat CostumeStatValue;
        public ObscuredFloat AdStatValue;
        public ObscuredInt DailyEntryCount;
        
        public int GetID()
        {
            return DungeonId;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Dungeon_Id"].ToString(), out int dungeonId);
            DungeonId = dungeonId;
            
            int.TryParse(jsonData["Stage_Limit_Time"].ToString(), out int limitTime);
            LimitTime = limitTime;
            
            float.TryParse(jsonData["MoveSpeed_Increase_HP_1"].ToString(), out float phase1Hp);
            Phase1Hp = phase1Hp;
            
            float.TryParse(jsonData["MoveSpeed_Increase_Value_1"].ToString(), out float phase1Value);
            Phase1Value = phase1Value;

            float.TryParse(jsonData["MoveSpeed_Increase_HP_2"].ToString(), out float phase2Hp);
            Phase2Hp = phase2Hp;

            float.TryParse(jsonData["MoveSpeed_Increase_Value_2"].ToString(), out float phase2Value);
            Phase2Value = phase2Value;
            
            float.TryParse(jsonData["MoveSpeed_Increase_HP_3"].ToString(), out float phase3Hp);
            Phase3Hp = phase3Hp;
            
            float.TryParse(jsonData["MoveSpeed_Increase_Value_3"].ToString(), out float phase3Value);
            Phase3Value = phase3Value;
            
            int.TryParse(jsonData["Monster_Hp"].ToString(), out int monsterHp);
            MonsterHp = monsterHp;
            
            double.TryParse(jsonData["Hit_Reward_Value"].ToString(), out double hitRewardValue);
            HitRewardValue = hitRewardValue;
            
            double.TryParse(jsonData["Clear_Reward_Value"].ToString(), out double clearRewardValue);
            ClearRewardValue = clearRewardValue;
            
            float.TryParse(jsonData["Costume_Stat_Value"].ToString(), out float costumeStatValue);
            CostumeStatValue = costumeStatValue;
            
            float.TryParse(jsonData["Ad_Stat_Value"].ToString(), out float adStatValue);
            AdStatValue = adStatValue;
            
            int.TryParse(jsonData["Daily_Entry_Count"].ToString(), out int dailyEntryCount);
            DailyEntryCount = dailyEntryCount;
            
            ChartManager.WorldCupEventDungeonCharts[GetID()] = this;
        }
    }
}