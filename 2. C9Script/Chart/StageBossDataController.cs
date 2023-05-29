using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public enum eSTAGEBOSS_DATA_STRUCT
{
    Id,
    BossId,
    BossHp,
    BossAttack,
    ClearLimitTime,
    ClearRewardIds,
    ClearRewardValues,
}

public class StageBossData
{
    public int Id;
    public int BossId;
    public double BossHp;
    public double BossAttack;
    public int ClearLimitTime;
    public int[] ClearRewardIds;
    public double[] ClearRewardValues;

    public StageBossData(int id, int bossId, double bossHp, double bossAttack, int clearLimitTime, int[] clearRewardIds, double[] clearRewardValues)
    {
        Id = id;
        BossId = bossId;
        BossHp = bossHp;
        BossAttack = bossAttack;
        ClearLimitTime = clearLimitTime;
        ClearRewardIds = clearRewardIds;
        ClearRewardValues = clearRewardValues;
    }
}

public class StageBossDataController : MonoBehaviour
{
    public Dictionary<int, StageBossData> StageBossTable { get; private set; }

    private readonly string PATH = "ExcelData/StageBossData";

    public bool DataInit = false;

    public StageBossDataController()
    {
        StageBossTable = new Dictionary<int, StageBossData>();

        string[,] datas = ChartManager.ExcelDataLoader.LoadTextData(PATH);

        StageBossData stageBossData = null;

        MainThreadDispatcher.StartCoroutine(Cor_Load());

        IEnumerator Cor_Load()
        {
            for (int i = 0; i < datas.GetLength(0); i++)
            {
                stageBossData = new StageBossData(
                    int.Parse(datas[i, (int)eSTAGEBOSS_DATA_STRUCT.Id]),
                    int.Parse(datas[i, (int)eSTAGEBOSS_DATA_STRUCT.BossId]),
                    double.Parse(datas[i, (int)eSTAGEBOSS_DATA_STRUCT.BossHp]),
                    double.Parse(datas[i, (int)eSTAGEBOSS_DATA_STRUCT.BossAttack]),
                    int.Parse(datas[i, (int)eSTAGEBOSS_DATA_STRUCT.ClearLimitTime]),
                    IntArrayConverter(datas[i, (int)eSTAGEBOSS_DATA_STRUCT.ClearRewardIds]),
                    DoubleArrayConverter(datas[i, (int)eSTAGEBOSS_DATA_STRUCT.ClearRewardValues])
                    );


                if (i % 100 == 0)
                    yield return null;

                StageBossTable.Add(stageBossData.Id, stageBossData);
            }

            DataInit = true;
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
