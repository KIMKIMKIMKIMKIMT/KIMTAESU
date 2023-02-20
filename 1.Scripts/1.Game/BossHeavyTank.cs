using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHeavyTank : Boss
{
    #region Fields
    private WaitForSeconds _wait = new WaitForSeconds(3f);
    private WaitForSeconds _patternWait = new WaitForSeconds(1f);
    private WaitForSeconds _dashWait = new WaitForSeconds(3f);
    [SerializeField] private Transform _weaponPos;
    private Vector3 _playerPos;
    private Vector3 _dir;

    [SerializeField] private float _dashSpeed;
    private bool _isDash;
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        SetEnemyData(GameDataMgr.Instance.EnemyDataController.EnemyDataTable[eENEMYDATA_LIST.Boss3]);
        BattleMgr.Instance.SetBossHp(GameDataMgr.Instance.EnemyDataController.EnemyDataTable[eENEMYDATA_LIST.Boss3].Hp);
        _isDie = false;
        StopAllCoroutines();
        StartCoroutine(Attack());
    }

    protected override void Update()
    {
        if (!_isDash)
        {
            base.Update();
            SoundMgr.Instance.MoveSfxStop();
        }
        
        if (_isDash)
        {
            transform.position += _dir.normalized * _dashSpeed * Time.deltaTime;
            SoundMgr.Instance.MoveSfxPlay();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fence"))
        {
            _isDash = false;
        }
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
        EnemyBullet bullet1 = PoolMgr.Instance.GetEnemyBullet(4);
        bullet1.transform.position = _weaponPos.position;
        bullet1.SetBullet(dir, _enemyData.Atk, 5, false, eENEMY_TYPE.Boss3);
    }

    public void Pattern2()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BaseAttack1);
        }
        int curBullet = 1;
        int maxBullet = 15;
        for (int i = 0; i < maxBullet; i++)
        {
            Vector3 dir = new Vector3(Mathf.Cos(Mathf.PI * 2 * curBullet / maxBullet), Mathf.Sin(Mathf.PI * 2 * curBullet / maxBullet), 0);
            EnemyBullet bullet = PoolMgr.Instance.GetEnemyBullet(3);
            bullet.transform.position = _weaponPos.position;
            bullet.SetBullet(dir, _enemyData.Atk, 2, false, eENEMY_TYPE.Boss3);
            curBullet++;
        }
    }

    public void Pattern3()
    {
        _playerPos = _player.transform.position;
        _dir = _playerPos - transform.position;

        _isDash = true;
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
                for (int i = 0; i < 3; i++)
                {
                    Pattern1();
                    yield return _patternWait;
                }
            }
            else if (ran == 1)
            {
                Pattern2();
            }
            else if (ran == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    Pattern3();
                    yield return _dashWait;
                }
                
            }
            
        }
    }
    #endregion
}
