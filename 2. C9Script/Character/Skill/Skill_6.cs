
// 얼음 장판

using System;
using UnityEngine;

public class Skill_6 : BaseSkill
{
    [SerializeField] private Animator SkillAnimator;
    
    private float _hitRange;
    public bool _isCrash;

    protected override void Init()
    {
        base.Init();

        Id = 6;
        _hitRange = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Range)].Value;
        transform.SetParent(Managers.Game.MainPlayer.transform);
        transform.localPosition = Vector3.zero;
    }

    public override void StartSkill()
    {
        base.StartSkill();

        _isCrash = false;
    }

    private void Update()
    {
        if (SkillAnimator.IsEndCurrentAnimation())
            StopSkill();
    }

    private void OnDisable()
    {
        _isCrash = false;
    }

    public void OnAnimationEvent_Crash()
    {
        _isCrash = true;
    }
    
    public void OnAnimationEvent_Damage(SkillRangeType rangeType)
    {
        switch (rangeType)
        {
            case SkillRangeType.Area:
            {
                Managers.Monster.FindTargetMonsters(transform.position, _hitRange).ForEach(monster =>
                {
                    double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                    double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                    monster.Damage(damage, criticalMultiple);
                });
            }
                break;
            case SkillRangeType.Field:
            {
                Managers.Monster.FindMonstersInGameView().ForEach(monster =>
                {
                    double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                    double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                    monster.Damage(damage, criticalMultiple);
                });
            }
                break;
        }
    }
}