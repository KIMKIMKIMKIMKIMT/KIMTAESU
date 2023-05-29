using LitJson;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Chart
{
    public class AllGuildRaidDungeonChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt MonsterId;
        public ObscuredInt LimitTime;
        public ObscuredDouble GoldValue;
        public ObscuredDouble GoldBarValue;
        public ObscuredDouble MonsterHp;
        public ObscuredDouble MonsterAtk;


        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Dungeon_Id"].ToString(), out int id);
            Id = id;

            int.TryParse(jsonData["Monster_Id"].ToString(), out int monsterId);
            MonsterId = monsterId;

            int.TryParse(jsonData["LimitTime"].ToString(), out int limitTime);
            LimitTime = limitTime;

            double.TryParse(jsonData["Gold_Value"].ToString(), out double goldValue);
            GoldValue = goldValue;

            double.TryParse(jsonData["Goldbar_Value"].ToString(), out double goldbarValue);
            GoldBarValue = goldbarValue;

            double.TryParse(jsonData["Monster_Hp"].ToString(), out double monsterHp);
            MonsterHp = monsterHp;

            double.TryParse(jsonData["Monster_Attack"].ToString(), out double monsterAtk);
            MonsterAtk = monsterAtk;

            ChartManager.AllGuildRaidDungeonCharts.TryAdd(GetID(), this);
        }
    }
}

