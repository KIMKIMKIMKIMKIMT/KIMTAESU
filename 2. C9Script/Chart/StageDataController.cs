using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public enum eSTAGE_DATA_STRUCT
{
    Id,
    WorldIndex,
    MonsterIds,
    MonsterHps,
    SpawnMonsterCounts,
    RespawnTimes,
    NeedBossChallengeKillCount,
    GoldValues,
    ExpValues,
    DropItemId,
    DropItemValue,
    DropItemRate
}

public class StageData
{
    public int Id;
    public int WorldIndex;
    public int[] MonsterIds;
    public double[] MonsterHps;
    public int[] SpawnMonsterCounts;
    public int[] RespawnTimes;
    public int NeedBossChallengeKillCount;
    public double[] GoldValues;
    public double[] ExpValues;
    public int DropItemId;
    public int DropItemValue;
    public double DropItemRate;

    public StageData(int id, int worldIndex, int[] monsterIds, double[] monsterHps, int[] spawnMonsterCounts, int[] respawnTimes, int needBossChallengeKillCount, double[] goldValues, double[] expValues, int dropItemId, int dropItemValue, double dropItemRate)
    {
        Id = id;
        WorldIndex = worldIndex;
        MonsterIds = monsterIds;
        MonsterHps = monsterHps;
        SpawnMonsterCounts = spawnMonsterCounts;
        RespawnTimes = respawnTimes;
        NeedBossChallengeKillCount = needBossChallengeKillCount;
        GoldValues = goldValues;
        ExpValues = expValues;
        DropItemId = dropItemId;
        DropItemValue = dropItemValue;
        DropItemRate = dropItemRate;
    }
}

public class StageDataController : MonoBehaviour
{
    public Dictionary<int, StageData> StageDataTable { get; private set; }

    private readonly string PATH = "ExcelData/StageData";

    public bool DataInit = false;

    public StageDataController()
    {
        StageDataTable = new Dictionary<int, StageData>();

        string[,] datas = ChartManager.ExcelDataLoader.LoadTextData(PATH);

        StageData stageData = null;

        MainThreadDispatcher.StartCoroutine(Cor_Load(datas));

        IEnumerator Cor_Load(string[,] data)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                stageData = new StageData(
                    int.Parse(datas[i, (int)eSTAGE_DATA_STRUCT.Id]),
                    int.Parse(datas[i, (int)eSTAGE_DATA_STRUCT.WorldIndex]),
                    IntArrayConverter(datas[i, (int)eSTAGE_DATA_STRUCT.MonsterIds]),
                    DoubleArrayConverter(datas[i, (int)eSTAGE_DATA_STRUCT.MonsterHps]),
                    IntArrayConverter(datas[i, (int)eSTAGE_DATA_STRUCT.SpawnMonsterCounts]),
                    IntArrayConverter(datas[i, (int)eSTAGE_DATA_STRUCT.RespawnTimes]),
                    int.Parse(datas[i, (int)eSTAGE_DATA_STRUCT.NeedBossChallengeKillCount]),
                    DoubleArrayConverter(datas[i, (int)eSTAGE_DATA_STRUCT.GoldValues]),
                    DoubleArrayConverter(datas[i, (int)eSTAGE_DATA_STRUCT.ExpValues]),
                    int.Parse(datas[i, (int)eSTAGE_DATA_STRUCT.DropItemId]),
                    int.Parse(datas[i, (int)eSTAGE_DATA_STRUCT.DropItemValue]),
                    double.Parse(datas[i, (int)eSTAGE_DATA_STRUCT.DropItemRate])
                    );
                    

                if (i % 100 == 0)
                    yield return null;

                StageDataTable.Add(stageData.Id, stageData);
            }

            DataInit = true;
            yield return null;
        }
    }

    public int[] IntArrayConverter(string datas)
    {
        int[] intData = Array.ConvertAll(datas.ToString().Trim().Split(','), int.Parse);
        return intData;
    }

    public double[] DoubleArrayConverter(string datas)
    {
        double[] doubleData = Array.ConvertAll(datas.ToString().Trim().Split(','), double.Parse);
        return doubleData;
    }
}
