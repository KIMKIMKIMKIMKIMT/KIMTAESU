using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum eENEMYDATA_LIST
{
    ShortDistanceEnemy1,
    ShortDistanceEnemy2,
    ShortDistanceEnemy3,
    LongDistanceEnemy1,
    LongDistanceEnemy2,
    MiniBoss1,
    Boss1,
    Boss2,
    Boss3,

    Max
}
public class EnemyDataController
{
    public Dictionary<eENEMYDATA_LIST, EnemyData> EnemyDataTable { get; private set; }

    private readonly string PATH = "ExcelData/EnemyData";

    public EnemyDataController()
    {
        EnemyDataTable = new Dictionary<eENEMYDATA_LIST, EnemyData>();
        string[,] datas = ExcelDataLoader.Instance.LoadTextData(PATH);

        for (int i = 0; i < datas.GetLength(0); i++)
        {
            EnemyData enemyData = new EnemyData(
            int.Parse(datas[i, (int)eENEMYDATA_STRUCT.Index]),
            datas[i, (int)eENEMYDATA_STRUCT.Name],
            (eENEMYDATA_LIST)Enum.Parse(typeof(eENEMYDATA_LIST), datas[i, (int)eENEMYDATA_STRUCT.Key]),
            float.Parse(datas[i, (int)eENEMYDATA_STRUCT.Atk]),
            float.Parse(datas[i, (int)eENEMYDATA_STRUCT.Hp]),
            float.Parse(datas[i, (int)eENEMYDATA_STRUCT.Speed]),
            int.Parse(datas[i, (int)eENEMYDATA_STRUCT.Gem])
            );

            EnemyDataTable.Add(enemyData.Key, enemyData);
        }
        
    }
}
public enum eENEMYDATA_STRUCT
{
    Index,
    Name,
    Key,
    Atk,
    Hp,
    Speed,
    Gem
}
public class EnemyData
{
    public int Index;
    public string Name;
    public eENEMYDATA_LIST Key;
    public float Atk;
    public float Hp;
    public float Speed;
    public int Gem;

    public EnemyData(int index, string name, eENEMYDATA_LIST key, float atk, float hp, float speed, int gem)
    {
        Index = index;
        Name = name;
        Key = key;
        Atk = atk;
        Hp = hp;
        Speed = speed;
        Gem = gem;
    }
}
