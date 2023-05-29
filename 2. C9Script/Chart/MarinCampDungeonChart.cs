using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class MarinCampDungeonChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt StageClearLimitTime;
        public ObscuredInt[] MonsterIds;
        public ObscuredDouble MonsterHp;
        public ObscuredDouble MonsterAttack;
        public ObscuredDouble MonsterRewardValue;
        public ObscuredDouble StageClearRewardItemValue;
        

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Marinecamp_Dungeon_Id"].ToString(), out int id);
            Id = id;
            
            int.TryParse(jsonData["Stage_Clear_Limit_Time"].ToString(), out int stageClearLimitTime);
            StageClearLimitTime = stageClearLimitTime;
            
            int[] monsterIds = Array.ConvertAll(jsonData["Monster_Ids"].ToString().Trim().Split(','), int.Parse);
            MonsterIds = new ObscuredInt[monsterIds.Length];
            for (int i = 0; i < monsterIds.Length; i++)
                MonsterIds[i] = monsterIds[i];
            
            double.TryParse(jsonData["Monster_Hp"].ToString().Replace(",", string.Empty), out double monsterHp);
            MonsterHp = monsterHp;

            double.TryParse(jsonData["Monster_Attack"].ToString().Replace(",", string.Empty), out double monsterAttack);
            MonsterAttack = monsterAttack;
            
            double.TryParse(jsonData["Monster_Reward_Value"].ToString(), out double monsterRewardValue);
            MonsterRewardValue = monsterRewardValue;
            
            double.TryParse(jsonData["Stage_Clear_Reward_Value"].ToString(), out double stageClearRewardItemValue);
            StageClearRewardItemValue = stageClearRewardItemValue;

            ChartManager.MarinCampDungeonCharts[GetID()] = this;
        }
    }
}