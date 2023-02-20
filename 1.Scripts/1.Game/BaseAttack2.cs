using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack2 : AttackSkill
{
    #region Fields
    private WaitForSeconds _attackDelay = new WaitForSeconds(0.5f);

    private bool _isAttack;
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        _isAttack = false;
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
                    _attackDelay = new WaitForSeconds(0.4f);
                    break;
                case 3:
                    _attackDelay = new WaitForSeconds(0.3f);
                    break;
                case 4:
                    _attackDelay = new WaitForSeconds(0.2f);
                    break;
                case 5:
                    _attackDelay = new WaitForSeconds(0.1f);
                    break;
            }
        }
    }
    public override void SetCoolTime()
    {
        
    }
    #endregion

    #region Protected Methods
    protected override void Attack()
    {
        if (_player.Target == null)
        {
            _isAttack = false;
            StopAllCoroutines();
        }
        if (!_isAttack && _player.Target != null)
        {
            _isAttack = true;
            StopAllCoroutines();
            StartCoroutine(Cor_BaseAttack());
        }
    }

    protected override void SetSkillData(AttackSkillData skillData)
    {
        Level = 1;
        _skillData = skillData;
        _totalDmg = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().GetTankAtk();
        _coolTime = skillData.CoolTime;
        _bulletSpeed = skillData.BulletSpeed;
        _bulletScale = skillData.BulletScale;
    }
    #endregion

    #region Coroutines
    private IEnumerator Cor_BaseAttack()
    {
        while (true)
        {
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BaseAttack1, true);
            }
            _player.AttackTween();
            Base_Bullet bullet = PoolMgr.Instance.GetBaseBullet((int)eTANK.Tank_1);
            bullet.transform.position = _player.GetFirePos;
            Effect effect = PoolMgr.Instance.GetEffect((int)eEFFECT_LIST.BaseAttack);
            effect.transform.position = _player.GetFirePos;
            effect.transform.rotation = _player.Head.rotation;
            bullet.SetBullet(_player.HeadDir, 10 * BuffSkillController.Buff_BulletSpeed, _totalDmg * BuffSkillController.Buff_Power);

            yield return _attackDelay;
        }
    }
    #endregion
}
