using System;
using UnityEngine;

[RequireComponent(typeof(AnimationEventReceiver))]
public class Skill_15 : BaseSkill
{
    private AnimationEventReceiver _animationEventReceiver;
    private int _hitCount;

    private void Awake()
    {
        _animationEventReceiver = GetComponent<AnimationEventReceiver>();
        _animationEventReceiver.OnAnimationEventAttack += Damage;
        _animationEventReceiver.OnAnimationEventEndAttack += StopSkill;
    }

    protected override void Init()
    {
        base.Init();

        Id = 15;
        
        _hitCount = (int)ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Count)].Value;
    }

    public override void StartSkill()
    {
        base.StartSkill();

        transform.position = Vector2.zero;
    }

    private void Damage(float percent)
    {
        if (percent == 0)
            percent = 1;
        
        Managers.Monster.FindMonstersInGameView().ForEach(monster =>
        {
            if (monster == null)
                return;

            if (monster.IsDead)
                return;
            
            double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
            double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
            monster.Damage(damage * percent, criticalMultiple);
        });
    }
}