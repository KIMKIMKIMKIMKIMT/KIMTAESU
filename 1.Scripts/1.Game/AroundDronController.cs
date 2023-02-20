using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AroundDronController : AttackSkill
{
    #region Fields
    [SerializeField] private AroundDron[] _dron;
    private AroundDron _lastDron;

    
    private float _duration;
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        SetSkillData(GameDataMgr.Instance.AttackSKillDataController.AttackSkillTable[eATTACKSKILL_LIST.Dron]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.Hit(_totalDmg * BuffSkillController.Buff_Power);
            enemy.KnockBack((enemy.transform.position - transform.position).normalized, 150);
        }
        if (collision.CompareTag("ItemBox"))
        {
            GameItemBox box = collision.GetComponent<GameItemBox>();
            box.Hit(_totalDmg * BuffSkillController.Buff_Power);
        }
    }

    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #region Private Methods
    private void CheckDuration()
    {
        if (_lastDron != null)
        {
            _lastDron.ActiveOffTweens();
        }
    }
    #endregion

    #region Public Methods
    public override void Upgrade()
    {
        if (Level < 5)
        {
            CancelInvoke();
            _currentCoolTime = _coolTime;
            Level++;
            switch (Level)
            {
                case 2:
                    _totalDmg *= 1.5f;
                    break;
                case 3:
                    _duration = 4.5f;
                    break;
                case 4:
                    _totalDmg *= 1.5f;
                    break;
                case 5:
                    _totalDmg *= 2f;
                    _duration = 5f;
                    break;
            }
        }
    }

    protected override void Attack()
    {
        if (Level > 0)
        {
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.AroundDron, true);
            }
            for (int i = 0; i < _dron.Length; i++)
            {
                _dron[i].gameObject.SetActive(false);
            }
            _dron[Level - 1].gameObject.SetActive(true);
            
            
            _lastDron = _dron[Level - 1];
            Invoke("CheckDuration", _duration);
        }
        
    }

    protected override void SetSkillData(AttackSkillData skillData)
    {
        Level = 1;
        _duration = 4f;
        _skillData = skillData;
        _coolTime = skillData.CoolTime * BuffSkillController.Buff_CoolTime;
        _originCoolTime = skillData.CoolTime;

        _totalDmg = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().GetTankAtk() * 0.5f;
        _currentCoolTime = _coolTime;
    }
    public override void SetCoolTime()
    {
        _coolTime = _originCoolTime * BuffSkillController.Buff_CoolTime;
    }
    #endregion

}
