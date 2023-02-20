using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackSkill : Skill
{
    #region Fields
    protected AttackSkillData _skillData;

    protected float _totalDmg;
    protected float _coolTime;
    protected float _originCoolTime;
    protected float _currentCoolTime;
    protected float _bulletSpeed;
    protected float _bulletScale;
    #endregion

    #region Unity Method
    protected override void Start()
    {
        base.Start();
    }

    protected virtual void Update()
    {
        if (_currentCoolTime >= _coolTime)
        {
            _currentCoolTime = 0;
            Attack();
        }
        _currentCoolTime += Time.deltaTime;
    }
    #endregion

    #region Protecteed Method
    protected abstract void SetSkillData(AttackSkillData skillData);
    public abstract void SetCoolTime();
    protected abstract void Attack();
    #endregion
}
