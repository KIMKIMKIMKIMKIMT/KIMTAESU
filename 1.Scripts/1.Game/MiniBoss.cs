using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : Enemy
{
    #region Fields
    [SerializeField] private BonusBox _bonusBoxPrefab;
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        
    }
    private void OnEnable()
    {
        SetEnemyData(GameDataMgr.Instance.EnemyDataController.EnemyDataTable[eENEMYDATA_LIST.MiniBoss1]);
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
        _rigid.AddForce(dir * (power / 3) * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }
    #endregion

    #region Protected Methods
    protected override void Die()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BossDie, true);
        }
        Effect effect = PoolMgr.Instance.GetEffect(4);
        effect.transform.position = transform.position;
        effect.transform.localScale = new Vector2(1, 1);

        BonusBox box = Instantiate(_bonusBoxPrefab);
        box.transform.position = transform.position;

        BattleMgr.Instance.StagaGold += 100;
        BattleMgr.Instance.KillCnt++;
        gameObject.SetActive(false);
    }

    protected override void SetEnemyData(EnemyData enemyData)
    {
        _enemyData = enemyData;
        _hp = enemyData.Hp;
        _atk = enemyData.Atk;
        _speed = enemyData.Speed;
    }
    #endregion
}
