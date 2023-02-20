using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum eHPEQUIP
{
    None = -1,
    Shield0_N,
    Shield1_N,
    Shield2_N,
    Shield3_N,
    Shield4_N,
    Shield0_E,
    Shield1_E,
    Shield2_E,
    Shield3_E,
    Shield4_E,
    Shield0_U,
    Shield1_U,
    Shield2_U,
    Shield3_U,
    Shield4_U,

    Max
}
public class HpEquipmentData
{
    public int Index;
    public eHPEQUIP Key;
    public string Name;
    public string Des;
    public int Hp;
    public eEQUIPGRADE_TYPE Grade;

    public HpEquipmentData(int index, eHPEQUIP key, string name, string des, int hp, eEQUIPGRADE_TYPE grade)
    {
        Index = index;
        Key = key;
        Name = name;
        Des = des;
        Hp = hp;
        Grade = grade;
    }
}
public class HpEquipmentDataController
{
    public enum eHPEQUIP_STRUCT
    {
        Index,
        Key,
        Name,
        Des,
        Hp,
        Grade
    }

    public Dictionary<eHPEQUIP, HpEquipmentData> HpEquipmentDic;

    private readonly string PATH = "ExcelData/HpEquipmentData";

    public HpEquipmentDataController()
    {
        HpEquipmentDic = new Dictionary<eHPEQUIP, HpEquipmentData>();

        HpEquipmentData hpEquipmentData;
        string[,] datas = ExcelDataLoader.Instance.LoadTextData(PATH);
        for (int i = 0; i < datas.GetLength(0); i++)
        {
            hpEquipmentData = new HpEquipmentData(
            int.Parse(datas[i, (int)eHPEQUIP_STRUCT.Index]),
            (eHPEQUIP)Enum.Parse(typeof(eHPEQUIP), datas[i, (int)eHPEQUIP_STRUCT.Key]),
            datas[i, (int)eHPEQUIP_STRUCT.Name],
            datas[i, (int)eHPEQUIP_STRUCT.Des],
            int.Parse(datas[i, (int)eHPEQUIP_STRUCT.Hp]),
            (eEQUIPGRADE_TYPE)Enum.Parse(typeof(eEQUIPGRADE_TYPE), datas[i, (int)eHPEQUIP_STRUCT.Grade])
            );

            HpEquipmentDic.Add(hpEquipmentData.Key, hpEquipmentData);
        }
    }
}
