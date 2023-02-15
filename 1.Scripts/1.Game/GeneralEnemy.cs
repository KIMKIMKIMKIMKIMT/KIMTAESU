using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralEnemy : Enemy
{
    #region Fields
    #endregion

    #region Unity Methods
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
    public override void Hit(float dmg)
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.EnemyHit, true);
        }
        _hp -= dmg;
        DmgTxt txt = PoolMgr.Instance.GetDmgTxt(0);
        txt.SetText((int)dmg, transform.position);
        if (_hp <= 0)
        {
            Die();
        }
    }

    public override void KnockBack(Vector3 dir, float power)
    {
        _rigid.AddForce(dir * power * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }
    #endregion

    #region Protected Methods
    protected override void Die()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.EnemyDie, true);
        }
        Effect effect = PoolMgr.Instance.GetEffect(4);
        effect.transform.position = transform.position;
        BattleMgr.Instance.StagaGold += 10;
        BattleMgr.Instance.KillCnt++;
        int ran = Random.Range(0, 2);
        if (ran == 0)
        {
            if (_enemyData.Gem == 1)
            {
                int ran1 = Random.Range(0, 10);

                if (ran1 == 0)
                {
                    PoolMgr.Instance.GemSpawn(2, transform.position);
                }
                else
                {
                    PoolMgr.Instance.GemSpawn(_enemyData.Gem, transform.position);
                }
            }
            else
            {
                PoolMgr.Instance.GemSpawn(_enemyData.Gem, transform.position);
            }
            
        }
        gameObject.SetActive(false);
    }

    protected override void SetEnemyData(EnemyData enemyData)
    {
        _enemyData = enemyData;
        _atk = enemyData.Atk;
        _hp = enemyData.Hp;
        _speed = enemyData.Speed;
    }
    #endregion
}
