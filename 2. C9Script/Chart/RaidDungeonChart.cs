using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class RaidDungeonChart : IChart<int>
    {
        public ObscuredInt DungeonId;
        
        public ObscuredInt Wave1MonsterId;
        public ObscuredDouble Wave1MonsterHp;
        public ObscuredDouble Wave1MonsterAttack;
        public ObscuredDouble Wave1MonsterGold;
        public ObscuredInt[] Wave1Portals;
        public ObscuredInt Wave1RespawnTime;
        public ObscuredInt Wave1ClearCount;

        public ObscuredInt Wave2MonsterId;
        public ObscuredDouble Wave2MonsterHp;
        public ObscuredDouble Wave2MonsterGoldBar;
        public ObscuredInt[] Wave2Portals;
        public ObscuredInt[] Wave2SpawnCounts;
        public ObscuredInt Wave2RespawnTime;
        public ObscuredInt Wave2ClearCount;

        public ObscuredInt Wave3MonsterId;
        public ObscuredDouble Wave3MonsterHp;
        public ObscuredDouble Wave3MonsterAttack;
        public ObscuredDouble Wave3SkillStone;
        public ObscuredInt[] Wave3Portals;
        public ObscuredInt Wave3LimitTime;
        
        
        public int GetID()
        {
            return DungeonId;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Dungeon_Id"].ToString(), out int dungeonId);
            DungeonId = dungeonId;

            int.TryParse(jsonData["1_Wave_Monster_Id"].ToString(), out int wave1MonsterId);
            Wave1MonsterId = wave1MonsterId;

            double.TryParse(jsonData["1_Wave_Monster_Hp"].ToString(), out double wave1MonsterHp);
            Wave1MonsterHp = wave1MonsterHp;

            double.TryParse(jsonData["1_Wave_Monster_Attack"].ToString(), out double wave1MonsterAttack);
            Wave1MonsterAttack = wave1MonsterAttack;

            double.TryParse(jsonData["1_Wave_Monster_Gold"].ToString(), out double wave1MonsterGold);
            Wave1MonsterGold = wave1MonsterGold;

            var wave1Portals = Array.ConvertAll(jsonData["1_Wave_Portals"].ToString().Trim().Split(','), int.Parse);
            Wave1Portals = new ObscuredInt[wave1Portals.Length];
            for (int i = 0; i < wave1Portals.Length; i++)
                Wave1Portals[i] = wave1Portals[i];

            int.TryParse(jsonData["1_Wave_Respawn"].ToString(), out int wave1RespawnTime);
            Wave1RespawnTime = wave1RespawnTime;

            int.TryParse(jsonData["1_Wave_ClearCount"].ToString(), out int wave1ClearCount);
            Wave1ClearCount = wave1ClearCount;

            int.TryParse(jsonData["2_Wave_Monster_Id"].ToString(), out int wave2MonsterId);
            Wave2MonsterId = wave2MonsterId;

            double.TryParse(jsonData["2_Wave_Monster_Hp"].ToString(), out double wave2MonsterHp);
            Wave2MonsterHp = wave2MonsterHp;

            double.TryParse(jsonData["2_Wave_Monster_Goldbar"].ToString(), out double wave2MonsterGoldBar);
            Wave2MonsterGoldBar = wave2MonsterGoldBar;

            var wave2SpawnCounts = Array.ConvertAll(jsonData["2_Wave_SpawnCount"].ToString().Trim().Split(','), int.Parse);
            Wave2SpawnCounts = new ObscuredInt[wave2SpawnCounts.Length];
            for (int i = 0; i < wave2SpawnCounts.Length; i++)
                Wave2SpawnCounts[i] = wave2SpawnCounts[i];

            var wave2Portals = Array.ConvertAll(jsonData["2_Wave_Portals"].ToString().Trim().Split(','), int.Parse);
            Wave2Portals = new ObscuredInt[wave2Portals.Length];
            for (int i = 0; i < wave2Portals.Length; i++)
                Wave2Portals[i] = wave2Portals[i];

            int.TryParse(jsonData["2_Wave_Respawn"].ToString(), out int wave2RespawnTime);
            Wave2RespawnTime = wave2RespawnTime;

            int.TryParse(jsonData["2_Wave_ClearCount"].ToString(), out int wave2ClearCount);
            Wave2ClearCount = wave2ClearCount;

            int.TryParse(jsonData["3_Wave_Monster_Id"].ToString(), out int wave3MonsterId);
            Wave3MonsterId = wave3MonsterId;

            double.TryParse(jsonData["3_Wave_Monster_Hp"].ToString(), out double wave3MonsterHp);
            Wave3MonsterHp = wave3MonsterHp;

            double.TryParse(jsonData["3_Wave_Monster_Attack"].ToString(), out double wave3MonsterAttack);
            Wave3MonsterAttack = wave3MonsterAttack;

            double.TryParse(jsonData["3_Wave_Monster_SkillStone"].ToString(), out double wave3SkillStone);
            Wave3SkillStone = wave3SkillStone;

            var wave3Portals = Array.ConvertAll(jsonData["3_Wave_Portals"].ToString().Trim().Split(','), int.Parse);
            Wave3Portals = new ObscuredInt[wave3Portals.Length];
            for (int i = 0; i < wave3Portals.Length; i++)
                Wave3Portals[i] = wave3Portals[i];

            int.TryParse(jsonData["3_Wave_LimitTime"].ToString(), out int wave3LimitTime);
            Wave3LimitTime = wave3LimitTime;

            ChartManager.RaidDungeonCharts[GetID()] = this;
        }
    }
}