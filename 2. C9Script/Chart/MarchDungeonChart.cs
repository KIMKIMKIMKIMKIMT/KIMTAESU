using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;

namespace Chart
{
    public class MarchDungeonChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt[] MonsterIds;
        public ObscuredDouble[] MonsterHps;
        
        public ObscuredInt[] FirstSpawnMonsterCounts;
        public ObscuredInt[] FirstSpawnAreas;

        public ObscuredInt[] SecondSpawnMonsterCounts;
        public ObscuredInt[] SecondSpawnAreas;

        public ObscuredInt[] ThirdSpawnMonsterCounts;
        public ObscuredInt[] ThirdSpawnAreas;

        public ObscuredInt StageClearKillCount;
        public ObscuredInt StageClearLimitTime;

        public ObscuredDouble[] MonsterRewardValues;
        public ObscuredDouble StageClearRewardItemValue;


        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["March_Dungeon_Id"].ToString(), out int id);
            Id = id;
            
            int[] monsterIds = Array.ConvertAll(jsonData["Monster_Ids"].ToString().Trim().Split(','), int.Parse);
            MonsterIds = new ObscuredInt[monsterIds.Length];
            for (int i = 0; i < monsterIds.Length; i++)
                MonsterIds[i] = monsterIds[i];
            
            double[] monsterHps = Array.ConvertAll(jsonData["Monster_Hps"].ToString().Trim().Split(','), double.Parse);
            MonsterHps = new ObscuredDouble[monsterHps.Length];
            for (int i = 0; i < monsterHps.Length; i++)
                MonsterHps[i] = monsterHps[i];
            
            int[] firstSpawnMonsterCounts = Array.ConvertAll(jsonData["First_Monster_Spawns"].ToString().Trim().Split(','), int.Parse);
            FirstSpawnMonsterCounts = new ObscuredInt[firstSpawnMonsterCounts.Length];
            for (int i = 0; i < firstSpawnMonsterCounts.Length; i++)
                FirstSpawnMonsterCounts[i] = firstSpawnMonsterCounts[i];
            
            int[] firstSpawnAreas = Array.ConvertAll(jsonData["First_Spawn_Areas"].ToString().Trim().Split(','), int.Parse);
            FirstSpawnAreas = new ObscuredInt[firstSpawnAreas.Length];
            for (int i = 0; i < firstSpawnAreas.Length; i++)
                FirstSpawnAreas[i] = firstSpawnAreas[i];

            int[] secondSpawnMonsterCounts = Array.ConvertAll(jsonData["Second_Monster_Spawns"].ToString().Trim().Split(','), int.Parse);
            SecondSpawnMonsterCounts = new ObscuredInt[secondSpawnMonsterCounts.Length];
            for (int i = 0; i < secondSpawnMonsterCounts.Length; i++)
                SecondSpawnMonsterCounts[i] = secondSpawnMonsterCounts[i];
            
            int[] secondSpawnAreas = Array.ConvertAll(jsonData["Second_Spawn_Areas"].ToString().Trim().Split(','), int.Parse);
            SecondSpawnAreas = new ObscuredInt[secondSpawnAreas.Length];
            for (int i = 0; i < secondSpawnAreas.Length; i++)
                SecondSpawnAreas[i] = secondSpawnAreas[i];
            
            int[] thirdSpawnMonsterCounts = Array.ConvertAll(jsonData["Third_Monster_Spawns"].ToString().Trim().Split(','), int.Parse);
            ThirdSpawnMonsterCounts = new ObscuredInt[thirdSpawnMonsterCounts.Length];
            for (int i = 0; i < thirdSpawnMonsterCounts.Length; i++)
                ThirdSpawnMonsterCounts[i] = thirdSpawnMonsterCounts[i];
            
            int[] thirdSpawnAreas = Array.ConvertAll(jsonData["Third_Spawn_Areas"].ToString().Trim().Split(','), int.Parse);
            ThirdSpawnAreas = new ObscuredInt[thirdSpawnAreas.Length];
            for (int i = 0; i < thirdSpawnAreas.Length; i++)
                ThirdSpawnAreas[i] = thirdSpawnAreas[i];

            int.TryParse(jsonData["Stage_Clear_Monster_Killcount"].ToString(), out int stageClearKillCount);
            StageClearKillCount = stageClearKillCount;
            
            int.TryParse(jsonData["Stage_Clear_Limit_Time"].ToString(), out int stageClearLimitTime);
            StageClearLimitTime = stageClearLimitTime;
            
            double[] monsterRewardValues = Array.ConvertAll(jsonData["Monster_Reward_Values"].ToString().Trim().Split(','), double.Parse);
            MonsterRewardValues = new ObscuredDouble[monsterRewardValues.Length];
            for (int i = 0; i < monsterRewardValues.Length; i++)
                MonsterRewardValues[i] = monsterRewardValues[i];
            
            double.TryParse(jsonData["Stage_Clear_Reward_Item_Value"].ToString(), out double stageClearRewardItemValue);
            StageClearRewardItemValue = stageClearRewardItemValue;

            ChartManager.MarchDungeonCharts[GetID()] = this;
        }
    }
}