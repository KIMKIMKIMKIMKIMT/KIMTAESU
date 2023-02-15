using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    #region Fields
    [SerializeField] private BonusBox _bonusBoxPrefeb;

    protected bool _isDie;
    #endregion

    #region Public Methods
    public override void Hit(float dmg)
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.EnemyHit, true);
        }
        _hp -= dmg;
        BattleMgr.Instance.BossHpDecrease(_hp);
        DmgTxt txt = PoolMgr.Instance.GetDmgTxt(0);
        txt.SetText((int)dmg, transform.position);
        if (_hp <= 0)
        {
            Die();
            PoolMgr.Instance.EnemyBulletObjAllOff();
            BattleMgr.Instance.BossRaid(false);
        }
    }

    public override void KnockBack(Vector3 dir, float power)
    {

    }
    #endregion

    #region Protected Methods
    protected override void Die()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BossDie, true);
        }
        Effect effect = PoolMgr.Instance.GetEffect(6);
        effect.transform.position = transform.position;

        BonusBox box = Instantiate(_bonusBoxPrefeb);
        box.transform.position = transform.position;

        BattleMgr.Instance.StagaGold += 500;
        BattleMgr.Instance.KillCnt++;
        gameObject.SetActive(false);
        _isDie = true;
    }

    protected override void SetEnemyData(EnemyData enemyData)
    {
        
    }
    #endregion
}
