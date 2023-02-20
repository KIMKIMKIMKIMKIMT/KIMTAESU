using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMissileController : AttackSkill
{
    #region Fields
    private RangeMissilePool _pool;
    private WaitForSeconds _delay = new WaitForSeconds(0.25f);
    private int _bulletCnt;

    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        _pool = GetComponent<RangeMissilePool>();
        SetSkillData(GameDataMgr.Instance.AttackSKillDataController.AttackSkillTable[eATTACKSKILL_LIST.AroundBomb]);
    }

    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #region Public Methods
        
    public override void Upgrade()
    {
        if (Level < 5)
        {
            _currentCoolTime = _coolTime;
            Level++;
            switch (Level)
            {
                case 2:
                    _totalDmg *= 2f;
                    _bulletCnt = 12;
                    break;
                case 3:
                    _totalDmg *= 2f;
                    break;
                case 4:
                    _totalDmg *= 2f;
                    _bulletCnt = 15;
                    break;
                case 5:
                    _totalDmg *= 2f;
                    break;
            }
        }
    }

    protected override void Attack()
    {
        if (Level > 0)
        {
            StopAllCoroutines();
            StartCoroutine(RangeAttack(_bulletCnt));
        }
        
    }

    protected override void SetSkillData(AttackSkillData skillData)
    {
        Level = 1;
        _bulletCnt = 10;
        _skillData = skillData;
        _coolTime = skillData.CoolTime * BuffSkillController.Buff_CoolTime;
        _originCoolTime = skillData.CoolTime;
        _totalDmg = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().GetTankAtk() * 0.8f;
        _bulletSpeed = skillData.BulletSpeed;
    }
    public override void SetCoolTime()
    {
        _coolTime = _originCoolTime * BuffSkillController.Buff_CoolTime;
    }
    #endregion

    #region Coroutines
    private IEnumerator RangeAttack(int maxbullet)
    {
        int maxBullet = maxbullet + 1;
        int curBullet = 1;
        while (true)
        {
            yield return _delay;
            Vector3 dir = new Vector3(Mathf.Cos(Mathf.PI * 2 * curBullet / maxBullet) * 3, Mathf.Sin(Mathf.PI * 2 * curBullet / maxBullet) * 3, 0);
            RangeMissile obj = _pool.GetFromPool(0);
            obj.transform.position = new Vector3(transform.position.x + dir.x * 0.8f, transform.position.y + dir.y * 0.8f, 0);
            obj.SetBullet(_totalDmg * BuffSkillController.Buff_Power, _bulletSpeed * BuffSkillController.Buff_BulletSpeed);
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.RangeMissile, true);
            }
            curBullet++;
            if (curBullet >= maxBullet)
            {
                yield break;
            }
        }
    }
    #endregion

}
