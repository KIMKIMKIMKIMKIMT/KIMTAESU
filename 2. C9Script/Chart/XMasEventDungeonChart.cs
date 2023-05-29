using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class XMasEventDungeonChart : IChart<int>
    {
        public ObscuredInt DungeonId;
        public ObscuredInt DefaultRewardId;
        public ObscuredInt DefaultRewardValue;
        public ObscuredInt ScoreRewardId;
        public ObscuredInt ScoreRewardValue;
        public ObscuredInt MaxScoreRewardValue;
        public ObscuredInt DailyEntryCount;

        public int GetID()
        {
            return DungeonId;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Dungeon_Id"].ToString(), out var dungeonId);
            DungeonId = dungeonId;

            int.TryParse(jsonData["Default_Reward_Id"].ToString(), out var defaultRewardId);
            DefaultRewardId = defaultRewardId;

            int.TryParse(jsonData["Default_Reward_Value"].ToString(), out var defaultRewardValue);
            DefaultRewardValue = defaultRewardValue;

            int.TryParse(jsonData["Score_Reward_Id"].ToString(), out var scoreRewardId);
            ScoreRewardId = scoreRewardId;

            int.TryParse(jsonData["Score_Reward_Value"].ToString(), out var scoreRewardValue);
            ScoreRewardValue = scoreRewardValue;

            int.TryParse(jsonData["Score_Reward_Max"].ToString(), out var maxScoreRewardValue);
            MaxScoreRewardValue = maxScoreRewardValue;

            int.TryParse(jsonData["Daily_Entry_Count"].ToString(), out var dailyEntryCount);
            DailyEntryCount = dailyEntryCount;

            ChartManager.XMasEventDungeonCharts.TryAdd(GetID(), this);
        }
    }
}