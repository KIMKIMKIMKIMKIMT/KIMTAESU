using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortDistanceEnemy : GeneralEnemy
{
    #region Fields
    [SerializeField] private eENEMYDATA_LIST _enemyType;
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
    }
    private void OnEnable()
    {
        SetEnemyData(GameDataMgr.Instance.EnemyDataController.EnemyDataTable[_enemyType]);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    #endregion

    #region Public Methods
    #endregion
}
