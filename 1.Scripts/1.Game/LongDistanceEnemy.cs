using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongDistanceEnemy : GeneralEnemy
{
    #region Fields
    [SerializeField] private eENEMYDATA_LIST _enemyType;
    [SerializeField] private Transform _firePos;

    private float _coolTime;
    private float _curCoolTime;
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        
        _coolTime = 3f;
    }
    private void OnEnable()
    {
        SetEnemyData(GameDataMgr.Instance.EnemyDataController.EnemyDataTable[_enemyType]);
    }
    protected override void Update()
    {
        base.Update();
        _curCoolTime += Time.deltaTime;

        if (_curCoolTime > _coolTime)
        {
            Attack();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    #endregion

    #region Public Methods
    public void Attack()
    {
        _curCoolTime = 0;

        EnemyBullet obj = PoolMgr.Instance.GetEnemyBullet(0);
        obj.transform.position = _firePos.position;
        obj.SetBullet(_player.transform.position - transform.position, _atk, _speed, false, eENEMY_TYPE.LongDistanceEnemy1);
    }
    #endregion


    #region Protected Methods
    #endregion
}
