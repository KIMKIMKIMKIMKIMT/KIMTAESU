using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class HwasengbangDungeonChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt MonsterSpawnCount;
        public ObscuredInt StageClearMonsterKillCount;
        public ObscuredInt StageClearLimitTime;
        public ObscuredInt[] MonsterIds;
        public ObscuredDouble MonsterHp;
        public ObscuredFloat RespawnTime;
        public ObscuredInt SecondMonsterId;
        public ObscuredDouble SecondMonsterHp;
        public ObscuredDouble MonsterRewardValue;
        public ObscuredDouble StageClearRewardItemValue;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Hwasengbang_Dungeon_Id"].ToString(), out int id);
            Id = id;

            int.TryParse(jsonData["Monster_Spawn"].ToString(), out int monsterSpawnCount);
            MonsterSpawnCount = monsterSpawnCount;

            int.TryParse(jsonData["Stage_Clear_Monster_Killcount"].ToString(), out int stageClearMonsterKillCount);
            StageClearMonsterKillCount = stageClearMonsterKillCount;

            int.TryParse(jsonData["Stage_Clear_Limit_Time"].ToString(), out int stageClearLimitTime);
            StageClearLimitTime = stageClearLimitTime;

            int[] monsterIds = Array.ConvertAll(jsonData["Monster_Ids"].ToString().Trim().Split(','), int.Parse);
            MonsterIds = new ObscuredInt[monsterIds.Length];
            for (int i = 0; i < monsterIds.Length; i++)
                MonsterIds[i] = monsterIds[i];

            double.TryParse(jsonData["Monster_Hp"].ToString(), out double monsterHp);
            MonsterHp = monsterHp;
            
            float.TryParse(jsonData["RespawnTime"].ToString(), out float respawnTime);
            RespawnTime = respawnTime;

            int.TryParse(jsonData["Second_Monster_Id"].ToString(), out var secondMonsterId);
            SecondMonsterId = secondMonsterId;

            double.TryParse(jsonData["Second_Monster_Hp"].ToString(), out var secondMonsterHp);
            SecondMonsterHp = secondMonsterHp; 
            
            double.TryParse(jsonData["Monster_Reward_Value"].ToString(), out double monsterRewardValue);
            MonsterRewardValue = monsterRewardValue;
            
            double.TryParse(jsonData["Stage_Clear_Reward_Value"].ToString(), out double stageClearRewardItemValue);
            StageClearRewardItemValue = stageClearRewardItemValue;

            ChartManager.HwasengbangDungeonCharts[GetID()] = this;
        }
    }
}