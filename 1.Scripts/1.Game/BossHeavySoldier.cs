using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHeavySoldier : Boss
{
    #region Fields
    [SerializeField] private Transform _weaponPos;
    private WaitForSeconds _wait = new WaitForSeconds(3f);
    private WaitForSeconds _patternWait = new WaitForSeconds(0.5f);
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        SetEnemyData(GameDataMgr.Instance.EnemyDataController.EnemyDataTable[eENEMYDATA_LIST.Boss1]);
        BattleMgr.Instance.SetBossHp(GameDataMgr.Instance.EnemyDataController.EnemyDataTable[eENEMYDATA_LIST.Boss1].Hp);
        _isDie = false;
        StopAllCoroutines();
        StartCoroutine(Attack());
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    #endregion

    #region Public Methods
    public void Pattern1()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BaseAttack1);
        }
        Vector3 dir = _player.transform.position - transform.position;
        EnemyBullet bullet1 = PoolMgr.Instance.GetEnemyBullet(2);
        bullet1.transform.position = _weaponPos.position;
        bullet1.SetBullet(dir, _enemyData.Atk, 4, false, eENEMY_TYPE.Boss1);
    }

    public void Pattern2(int value)
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BaseAttack1);
        }
        int curBullet = 1;
        int maxBullet = 15;
       
         for (int i = 0; i < maxBullet; i++)
         {
             Vector3 dir = new Vector3(Mathf.Cos(Mathf.PI * 3 * curBullet / maxBullet) * value, Mathf.Sin(Mathf.PI * 3 * value * curBullet / maxBullet) * value, 0);
             EnemyBullet bullet = PoolMgr.Instance.GetEnemyBullet(3);
             bullet.transform.position = _weaponPos.position;
             bullet.SetBullet(dir, _enemyData.Atk, 2, false, eENEMY_TYPE.Boss1);
             curBullet++;
         }
        
        
    }

    public void Pattern3()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BaseAttack1);
        }
        Vector3 dir = _player.transform.position - transform.position;
        EnemyBullet bullet1 = PoolMgr.Instance.GetEnemyBullet(1);
        bullet1.transform.position = _weaponPos.position;
        bullet1.SetBullet(dir, _enemyData.Atk, 2f, true, eENEMY_TYPE.Boss1);
    }
    public override void Hit(float dmg)
    {
        base.Hit(dmg);
    }
    #endregion

    #region Protected Methods
    protected override void Die()
    {
        base.Die();
    }
    protected override void SetEnemyData(EnemyData enemyData)
    {
        _enemyData = enemyData;
        _hp = enemyData.Hp;
        _atk = enemyData.Atk;
        _speed = enemyData.Speed;

    }
    #endregion

    #region Coroutines
    private IEnumerator Attack()
    {
        while (!_isDie)
        {
            yield return _wait;
            int ran = Random.Range(0, 3);

            if (ran == 0)
            {
                Pattern1();
            }
            else if (ran == 1)
            {
                for (int i = 1; i < 4; i++)
                {
                    Pattern2(i);
                    yield return _patternWait;
                }
                
            }
            else if (ran == 2)
            {
                Pattern3();
            }
        }
    }
    #endregion
}
