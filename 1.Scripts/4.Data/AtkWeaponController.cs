using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum eATKWEAPON
{
    None = -1,
    Gun0_N,
    Gun1_N,
    Gun2_N,
    Gun3_N,
    Gun4_N,
    Gun0_E,
    Gun1_E,
    Gun2_E,
    Gun3_E,
    Gun4_E,
    Gun0_U,
    Gun1_U,
    Gun2_U,
    Gun3_U,
    Gun4_U,

    Max
}

public enum eEQUIPGRADE_TYPE
{
    Normal,
    Elite,
    Unique

}
public class AtkWeaponData
{
    public int Index;
    public eATKWEAPON Key;
    public string Name;
    public string Des;
    public int Atk;
    public eEQUIPGRADE_TYPE Grade;

    public AtkWeaponData(int index, eATKWEAPON key, string name, string des, int atk, eEQUIPGRADE_TYPE grade)
    {
        Index = index;
        Key = key;
        Name = name;
        Des = des;
        Atk = atk;
        Grade = grade;
    }
}

public class AtkWeaponController
{
    public enum eATKWEAPON_STRUCT
    {
        Index,
        Key,
        Name,
        Des,
        Atk,
        Grade
    }

    public Dictionary<eATKWEAPON, AtkWeaponData> AtkWeaponDic;

    private readonly string PATH = "ExcelData/AtkWeaponData";

    public AtkWeaponController()
    {
        AtkWeaponDic = new Dictionary<eATKWEAPON, AtkWeaponData>();

        string[,] datas = ExcelDataLoader.Instance.LoadTextData(PATH);

        AtkWeaponData atkWeaponData;
        for (int i = 0; i < datas.GetLength(0); i++)
        {
            atkWeaponData = new AtkWeaponData(
                int.Parse(datas[i, (int)eATKSKILL_STRUCT.Index]),
                (eATKWEAPON)Enum.Parse(typeof(eATKWEAPON), datas[i, (int)eATKWEAPON_STRUCT.Key]),
                datas[i, (int)eATKWEAPON_STRUCT.Name],
                datas[i, (int)eATKWEAPON_STRUCT.Des],
                int.Parse(datas[i, (int)eATKWEAPON_STRUCT.Atk]),
                (eEQUIPGRADE_TYPE)Enum.Parse(typeof(eEQUIPGRADE_TYPE), datas[i, (int)eATKWEAPON_STRUCT.Grade])
                );

            AtkWeaponDic.Add(atkWeaponData.Key, atkWeaponData);
        }
    }
    
}
