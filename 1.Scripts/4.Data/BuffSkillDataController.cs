using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum eBUFFSKILL_LIST
{
    None = -1,
    Power,
    CoolTime,
    BulletSpeed,
    BulletScale,
    Hp,
    Speed
}
public class BuffSkillDataController
{
    public Dictionary<eBUFFSKILL_LIST,BuffSkillData> BuffSkillTable { get; private set; }

    private readonly string PATH = "ExcelData/BuffSkillData";

    public BuffSkillDataController()
    {
        BuffSkillTable = new Dictionary<eBUFFSKILL_LIST, BuffSkillData>();

        string[,] datas = ExcelDataLoader.Instance.LoadTextData(PATH);
        BuffSkillData skillData = null;

        for (int i = 0; i < datas.GetLength(0); i++)
        {
            skillData = new BuffSkillData(
                int.Parse(datas[i, (int)eBUFFSKILL_STRUCT.Index]),
                datas[i,(int)eBUFFSKILL_STRUCT.Name],
                (eBUFFSKILL_LIST)Enum.Parse(typeof(eBUFFSKILL_LIST), datas[i,(int)eBUFFSKILL_STRUCT.Key]),
                datas[i,(int)eBUFFSKILL_STRUCT.Des1],
                datas[i,(int)eBUFFSKILL_STRUCT.Des2],
                datas[i,(int)eBUFFSKILL_STRUCT.Des3],
                datas[i,(int)eBUFFSKILL_STRUCT.Des4],
                datas[i,(int)eBUFFSKILL_STRUCT.Des5]
                );

            BuffSkillTable.Add(skillData.Key, skillData);
        }
    }
}
public enum eBUFFSKILL_STRUCT
{
    Index,
    Name,
    Key,
    Des1,
    Des2,
    Des3,
    Des4,
    Des5
}
public class BuffSkillData
{
    public int Index;
    public string Name;
    public eBUFFSKILL_LIST Key;
    public string[] Descriptions;

    public BuffSkillData(int index, string name, eBUFFSKILL_LIST key, string lv1, string lv2, string lv3, string lv4, string lv5)
    {
        Index = index;
        Name = name;
        Key = key;
        Descriptions = new string[] { lv1, lv2, lv3, lv4, lv5 };
    }
}
