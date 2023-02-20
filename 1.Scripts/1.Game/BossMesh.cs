using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePATTERN
{
    Pattern1,
    Pattern2,
    Pattern3,

    Max
}
public class BossMesh : Boss
{
    #region Fields
    [SerializeField] private Transform _leftWeaponPos;
    [SerializeField] private Transform _rightWeaponPos;
    private WaitForSeconds _wait = new WaitForSeconds(3f);

    private ePATTERN _curPattern;
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        SetEnemyData(GameDataMgr.Instance.EnemyDataController.EnemyDataTable[eENEMYDATA_LIST.Boss2]);
        BattleMgr.Instance.SetBossHp(GameDataMgr.Instance.EnemyDataController.EnemyDataTable[eENEMYDATA_LIST.Boss2].Hp);
        StopAllCoroutines();
        StartCoroutine(Attack());
        
    }
    private void OnEnable()
    {
        _isDie = false;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    protected override void Update()
    {
        base.Update();

    }
    #endregion

    #region Public Methods
    public ePATTERN RandomPattern(int index)
    {
        _curPattern = (ePATTERN)index;
        return _curPattern;
    }
    
    public void Pattern1()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BaseAttack0);
        }
        Vector3 dir = _player.transform.position - transform.position;
        EnemyBullet bullet1 = PoolMgr.Instance.GetEnemyBullet(2);
        bullet1.transform.position = _leftWeaponPos.position;
        bullet1.SetBullet(dir, _enemyData.Atk, 4, false, eENEMY_TYPE.Boss1);
        EnemyBullet bullet2 = PoolMgr.Instance.GetEnemyBullet(2);
        bullet2.transform.position = _rightWeaponPos.position;
        bullet2.SetBullet(dir, _enemyData.Atk, 4, false, eENEMY_TYPE.Boss1);
    }

    public void Pattern2()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BaseAttack0);
        }
        int curBullet = 1;
        int maxBullet = 15;
        for (int i = 0; i < maxBullet; i++)
        {
            Vector3 dir = new Vector3(Mathf.Cos(Mathf.PI * 2 * curBullet / maxBullet), Mathf.Sin(Mathf.PI * 2 * curBullet / maxBullet), 0);
            EnemyBullet bullet = PoolMgr.Instance.GetEnemyBullet(3);
            bullet.transform.position = _leftWeaponPos.position;
            bullet.SetBullet(dir, _enemyData.Atk, 2, false, eENEMY_TYPE.Boss2);
            curBullet++;
        }
    }

    public void Pattern3()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BaseAttack0);
        }
        Vector3 dir = _player.transform.position - transform.position;
        EnemyBullet bullet1 = PoolMgr.Instance.GetEnemyBullet(1);
        bullet1.transform.position = _rightWeaponPos.position;
        bullet1.SetBullet(dir, _enemyData.Atk, 2f, true, eENEMY_TYPE.Boss2);
        EnemyBullet bullet2= PoolMgr.Instance.GetEnemyBullet(1);
        bullet2.transform.position = _leftWeaponPos.position;
        bullet2.SetBullet(dir, _enemyData.Atk, 2f, true, eENEMY_TYPE.Boss2);
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
        _atk = enemyData.Atk;
        _hp = enemyData.Hp;
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
                Pattern2();
            }
            else if(ran == 2)
            {
                Pattern3();
            }
        }
    }
    #endregion
}
