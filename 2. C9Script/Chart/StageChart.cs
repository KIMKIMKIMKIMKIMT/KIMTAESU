using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

//namespace Chart
//{
//    public class StageChart : IChart<int>
//    {
//        public ObscuredInt Id;
//        public ObscuredInt WorldIndex;
//        public ObscuredInt[] MonsterIds;
//        public ObscuredDouble[] MonsterHps;
//        public ObscuredInt[] SpawnMonsterCounts;
//        public ObscuredInt[] RespawnTimes;
//        public ObscuredInt NeedBossChallengeKillCount;
//        public ObscuredDouble[] GoldValues;
//        public ObscuredDouble[] ExpValues;
//        public ObscuredInt DropItemId;
//        public ObscuredInt DropItemValue;
//        public ObscuredDouble DropItemRate;
        
//        public int GetID()
//        {
//            return Id;
//        }

//        public void SetData(JsonData jsonData)
//        {
//            int.TryParse(jsonData["StageTable_Id"]["S"].ToString(), out int id);
//            Id = id;
            
//            int.TryParse(jsonData["World_Id"]["S"].ToString(), out int worldIndex);
//            WorldIndex = worldIndex;
            
//            int[] monsterIds = Array.ConvertAll(jsonData["Monster_Ids"]["S"].ToString().Trim().Split(','), int.Parse);
//            MonsterIds = new ObscuredInt[monsterIds.Length];
//            for (int i = 0; i < monsterIds.Length; i++)
//                MonsterIds[i] = monsterIds[i];
            
//            double[] monsterHps = Array.ConvertAll(jsonData["Monster_Hp"]["S"].ToString().Trim().Split(','), double.Parse);
//            MonsterHps = new ObscuredDouble[monsterHps.Length];
//            for (int i = 0; i < monsterHps.Length; i++)
//                MonsterHps[i] = monsterHps[i];
            
//            int[] spawnMonsterCounts = Array.ConvertAll(jsonData["Spawn_Monster_Count"]["S"].ToString().Trim().Split(','), int.Parse);
//            SpawnMonsterCounts = new ObscuredInt[spawnMonsterCounts.Length];
//            for (int i = 0; i < spawnMonsterCounts.Length; i++)
//                SpawnMonsterCounts[i] = spawnMonsterCounts[i];
            
//            int[] respawnTimes = Array.ConvertAll(jsonData["RespawnTime"]["S"].ToString().Trim().Split(','), int.Parse);
//            RespawnTimes = new ObscuredInt[respawnTimes.Length];
//            for (int i = 0; i < respawnTimes.Length; i++)
//                RespawnTimes[i] = respawnTimes[i];
            
//            int.TryParse(jsonData["Need_BossChallenge_KillCount"]["S"].ToString(), out int needBossChallengeKillCount);
//            NeedBossChallengeKillCount = needBossChallengeKillCount;
            
//            double[] goldValues = Array.ConvertAll(jsonData["Gold_Value"]["S"].ToString().Trim().Split(','), double.Parse);
//            GoldValues = new ObscuredDouble[goldValues.Length];
//            for (int i = 0; i < goldValues.Length; i++)
//                GoldValues[i] = goldValues[i];

//            double[] expValues = Array.ConvertAll(jsonData["Exp_Value"]["S"].ToString().Trim().Split(','), double.Parse);
//            ExpValues = new ObscuredDouble[expValues.Length];
//            for (int i = 0; i < expValues.Length; i++)
//                ExpValues[i] = expValues[i];
            
//            int.TryParse(jsonData["DropItem_Id"]["S"].ToString(), out int dropItemId);
//            DropItemId = dropItemId;
            
//            int.TryParse(jsonData["DropItem_Value"]["S"].ToString(), out int dropItemValue);
//            DropItemValue = dropItemValue;
            
//            double.TryParse(jsonData["DropItem_Rate"]["S"].ToString(), out double dropItemRate);
//            DropItemRate = dropItemRate;

//            ChartManager.StageCharts[GetID()] = this;
//        }
//    }
//}