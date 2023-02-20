using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : AttackSkill
{
    #region Fields
    
    private int _atkCnt;
    private int _curAtkCnt;
    private float _duration;

    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        SetSkillData(GameDataMgr.Instance.AttackSKillDataController.AttackSkillTable[eATTACKSKILL_LIST.MainWeapon]);
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
            Level++;
            switch (Level)
            {
                case 2:
                    _atkCnt = 2;
                    _totalDmg *= 1.5f;
                    break;
                case 3:
                    _atkCnt = 3;
                    _totalDmg *= 1.5f;
                    break;
                case 4:
                    _atkCnt = 4;
                    _totalDmg *= 1.5f;
                    break;
                case 5:
                    _atkCnt = 5;
                    _totalDmg *= 1.5f;
                    break;
            }
        }
    }
    #endregion

    #region Protected Methods
    protected override void Attack()
    {
        if (_player.Target != null)
        {
            StopAllCoroutines();
            StartCoroutine(Cor_BaseAttack());
        }
    }

    protected override void SetSkillData(AttackSkillData skillData)
    {
        Level = 1;
        _atkCnt = 1;
        _duration = 0.1f;
        _skillData = skillData;
        _totalDmg = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().GetTankAtk();
        _coolTime = skillData.CoolTime * BuffSkillController.Buff_CoolTime;
        _originCoolTime = skillData.CoolTime;
        _bulletSpeed = skillData.BulletSpeed;
        _bulletScale = skillData.BulletScale;
    }
    public override void SetCoolTime()
    {
        _coolTime = _originCoolTime * BuffSkillController.Buff_CoolTime;
    }
    #endregion

    #region Coroutines
    private IEnumerator Cor_BaseAttack()
    {
        WaitForSeconds _wait = new WaitForSeconds(_duration);
        _curAtkCnt = 0;
        while (true)
        {
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BaseAttack0, true);
            }
            _player.AttackTween();
            Base_Bullet bullet = PoolMgr.Instance.GetBaseBullet((int)eTANK.Tank_0);
            bullet.transform.position = _player.GetFirePos;
            Effect effect = PoolMgr.Instance.GetEffect((int)eEFFECT_LIST.BaseAttack);
            effect.transform.position = _player.GetFirePos;
            effect.transform.rotation = _player.Head.rotation;
            bullet.SetBullet(_player.HeadDir, 10 * BuffSkillController.Buff_BulletSpeed, _totalDmg * BuffSkillController.Buff_Power);
            _curAtkCnt++;

            if (_curAtkCnt >= _atkCnt)
            {
                yield break;
            }

            yield return _wait;
        }
    }
    #endregion
}
