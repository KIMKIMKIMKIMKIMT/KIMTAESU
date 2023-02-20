using System.Collections;
using System.Collections.Generic;
using System;

public enum eATTACKSKILL_LIST
{
    None = -1,
    MainWeapon,
    GuidedMissile,
    DropMissile,
    AroundBomb,
    Dron,
    Mine,

    Max
}

public class AttackSKillDataController
{
    public Dictionary<eATTACKSKILL_LIST, AttackSkillData> AttackSkillTable { get; private set; }

    private readonly string PATH = "ExcelData/AttackSkillData";
    public AttackSKillDataController()
    {
        AttackSkillTable = new Dictionary<eATTACKSKILL_LIST, AttackSkillData>();

        string[,] datas = ExcelDataLoader.Instance.LoadTextData(PATH);
        AttackSkillData skillData = null;
        for (int i = 0; i < datas.GetLength(0); i++)
        {
            skillData = new AttackSkillData(
                int.Parse(datas[i, (int)eATKSKILL_STRUCT.Index]),
                datas[i, (int)eATKSKILL_STRUCT.Name],
                (eATTACKSKILL_LIST)Enum.Parse(typeof(eATTACKSKILL_LIST), datas[i, (int)eATKSKILL_STRUCT.Key]),
                 datas[i, (int)eATKSKILL_STRUCT.Des1],
                 datas[i, (int)eATKSKILL_STRUCT.Des2],
                 datas[i, (int)eATKSKILL_STRUCT.Des3],
                 datas[i, (int)eATKSKILL_STRUCT.Des4],
                 datas[i, (int)eATKSKILL_STRUCT.Des5],
                 float.Parse(datas[i, (int)eATKSKILL_STRUCT.Damage]),
                 float.Parse(datas[i, (int)eATKSKILL_STRUCT.CoolTime]),
                 float.Parse(datas[i, (int)eATKSKILL_STRUCT.BulletSpeed]),
                 float.Parse(datas[i, (int)eATKSKILL_STRUCT.BulletScale])
                );

            AttackSkillTable.Add(skillData.Key, skillData);
        }
    }
}

public enum eATKSKILL_STRUCT
{
    Index,
    Name,
    Key,
    Des1,
    Des2,
    Des3,
    Des4,
    Des5,
    Damage,
    CoolTime,
    BulletSpeed,
    BulletScale
}

public class AttackSkillData
{
    public int Index;
    public string Name;
    public eATTACKSKILL_LIST Key;
    public string[] Descriptions;
    public float Damage;
    public float CoolTime;
    public float BulletSpeed;
    public float BulletScale;

    public AttackSkillData(int index, string name, eATTACKSKILL_LIST key, string lv1, string lv2, string lv3, string lv4, string lv5, float damage, float coolTime, float bulletspeed, float bulletscale)
    {
        Index = index;
        Name = name;
        Key = key;
        Descriptions = new string[] { lv1, lv2, lv3, lv4, lv5 };
        Damage = damage;
        CoolTime = coolTime;
        BulletSpeed = bulletspeed;
        BulletScale = bulletscale;
    }
}
