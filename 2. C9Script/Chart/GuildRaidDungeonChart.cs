using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class GuildRaidDungeonChart: IChart<int>
    {
        public ObscuredInt Id;
        
        public ObscuredInt Wave1MonsterId;
        public ObscuredDouble Wave1MonsterHp;
        public ObscuredDouble Wave1MonsterAttack;
        public ObscuredDouble Wave1MonsterGold;
        public ObscuredInt[] Wave1Portals;
        public ObscuredInt Wave1Respawn;
        public ObscuredInt Wave1ClearCount;

        public ObscuredInt Wave2MonsterId;
        public ObscuredDouble Wave2MonsterHp;
        public ObscuredDouble Wave2GoldBar;
        public ObscuredInt[] Wave2Portals;
        public ObscuredInt[] Wave2SpawnCounts;
        public ObscuredInt Wave2Respawn;
        public ObscuredInt Wave2ClearCount;

        public ObscuredInt Wave3MonsterId;
        public ObscuredDouble Wave3MonsterHp;
        public ObscuredDouble Wave3MonsterAttack;
        public ObscuredDouble Wave3GuildPoint;
        public ObscuredInt Wave3Portal;
        public ObscuredInt Wave3LimitTime;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Dungeon_Id"].ToString(), out var id);
            Id = id;

            int.TryParse(jsonData["1_Wave_Monster_Id"].ToString(), out var wave1MonsterId);
            Wave1MonsterId = wave1MonsterId;

            double.TryParse(jsonData["1_Wave_Monster_Hp"].ToString(), out var wave1MonsterHp);
            Wave1MonsterHp = wave1MonsterHp;

            double.TryParse(jsonData["1_Wave_Monster_Attack"].ToString(), out var wave1MonsterAttack);
            Wave1MonsterAttack = wave1MonsterAttack;

            double.TryParse(jsonData["1_Wave_Monster_Gold"].ToString(), out var wave1MonsterGold);
            Wave1MonsterGold = wave1MonsterGold;

            var wave1Portals = Array.ConvertAll(jsonData["1_Wave_Portals"].ToString().Trim().Split(','), int.Parse);
            Wave1Portals = new ObscuredInt[wave1Portals.Length];
            for (var i = 0; i < wave1Portals.Length; i++)
                Wave1Portals[i] = wave1Portals[i];

            int.TryParse(jsonData["1_Wave_Respawn"].ToString(), out var wave1Respawn);
            Wave1Respawn = wave1Respawn;

            int.TryParse(jsonData["1_Wave_ClearCount"].ToString(), out var wave1ClearCount);
            Wave1ClearCount = wave1ClearCount;

            int.TryParse(jsonData["2_Wave_Monster_Id"].ToString(), out var wave2MonsterId);
            Wave2MonsterId = wave2MonsterId;

            double.TryParse(jsonData["2_Wave_Monster_Hp"].ToString(), out var wave2MonsterHp);
            Wave2MonsterHp = wave2MonsterHp;

            double.TryParse(jsonData["2_Wave_Monster_Goldbar"].ToString(), out var wave2GoldBar);
            Wave2GoldBar = wave2GoldBar;

            var wave2Portals = Array.ConvertAll(jsonData["2_Wave_Portals"].ToString().Trim().Split(','), int.Parse);
            Wave2Portals = new ObscuredInt[wave2Portals.Length];
            for (var i = 0; i < wave2Portals.Length; i++)
                Wave2Portals[i] = wave2Portals[i];

            var wave2SpawnCounts = Array.ConvertAll(jsonData["2_Wave_SpawnCount"].ToString().Trim().Split(','), int.Parse);
            Wave2SpawnCounts = new ObscuredInt[wave2SpawnCounts.Length];
            for (var i = 0; i < wave2SpawnCounts.Length; i++)
                Wave2SpawnCounts[i] = wave2SpawnCounts[i];

            int.TryParse(jsonData["2_Wave_Respawn"].ToString(), out var wave2Respawn);
            Wave2Respawn = wave2Respawn;

            int.TryParse(jsonData["2_Wave_ClearCount"].ToString(), out var wave2ClearCount);
            Wave2ClearCount = wave2ClearCount;

            int.TryParse(jsonData["3_Wave_Monster_Id"].ToString(), out var wave3MonsterId);
            Wave3MonsterId = wave3MonsterId;

            double.TryParse(jsonData["3_Wave_Monster_Hp"].ToString(), out var wave3MonsterHp);
            Wave3MonsterHp = wave3MonsterHp;

            double.TryParse(jsonData["3_Wave_Monster_Attack"].ToString(), out var wave3MonsterAttack);
            Wave3MonsterAttack = wave3MonsterAttack;

            double.TryParse(jsonData["3_Wave_Monster_SkillStone"].ToString(), out var wave3SkillStone);
            Wave3GuildPoint = wave3SkillStone;

            int.TryParse(jsonData["3_Wave_Portals"].ToString(), out var wave3Portal);
            Wave3Portal = wave3Portal;

            int.TryParse(jsonData["3_Wave_LimitTime"].ToString(), out var wave3LimitTime);
            Wave3LimitTime = wave3LimitTime;

            ChartManager.GuildRaidDungeonCharts.TryAdd(GetID(), this);
        }
    }
}