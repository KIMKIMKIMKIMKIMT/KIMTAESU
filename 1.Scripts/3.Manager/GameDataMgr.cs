using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataMgr : DontDestroy<GameDataMgr>
{
    #region Fields
    public AttackSKillDataController AttackSKillDataController { get; private set; }
    public BuffSkillDataController BuffSkillDataController { get; private set; }
    public EnemyDataController EnemyDataController { get; private set; }
    public QuestDataController QuestData { get; private set; }
    public AtkWeaponController AtkWeaponData { get; private set; }
    public HpEquipmentDataController HpEquipData { get; private set; }
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        AttackSKillDataController = new AttackSKillDataController();
        BuffSkillDataController = new BuffSkillDataController();
        EnemyDataController = new EnemyDataController();
        QuestData = new QuestDataController();
        AtkWeaponData = new AtkWeaponController();
        HpEquipData = new HpEquipmentDataController();
    }
    #endregion
}
