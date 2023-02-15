using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankData
{
    #region Fields
    public int CurrentWeaponLevel;
    public int CurrentWeaponIndex;

    public int CurrentEquipLevel;
    public int CurrentEquipIndex;

    public int Atk;
    public int Hp;
    public int AtkLevel;
    public int HpLevel;

    public eATKWEAPON WeaponType;
    public eHPEQUIP EquipType;
    #endregion

    public TankData()
    {
        Atk = 10;
        Hp = 100;
        AtkLevel = 1;
        HpLevel = 1;


        WeaponType = eATKWEAPON.None;
        EquipType = eHPEQUIP.None;
    }

    public int GetTankAtk()
    {
        if (WeaponType == eATKWEAPON.None)
        {
            return Atk;
        }
        else
        {
            AtkWeaponData data = GameDataMgr.Instance.AtkWeaponData.AtkWeaponDic[WeaponType];

            return ((data.Atk + ((CurrentWeaponLevel -1) * 10)) + (Atk));
        }
    }

    public int GetTankHp()
    {
        if (EquipType == eHPEQUIP.None)
        {
            return Hp;
        }
        else
        {
            HpEquipmentData data = GameDataMgr.Instance.HpEquipData.HpEquipmentDic[EquipType];

            return ((data.Hp + ((CurrentEquipLevel -1) * 10)) + (Hp));
        }
    }
}

