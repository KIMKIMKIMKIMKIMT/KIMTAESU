using System.Collections;
using UnityEngine;

public class Skill_3 : BaseSkill
{
    private Animator _animator;
    private Camera _camera;
    private float _hitDelay;

    protected override void Init()
    {
        Id = 3;
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
        _hitDelay = _animator.GetCurrentAnimatorStateInfo(0).length / ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Count)].Value;
    }

    public override void StartSkill()
    {
        base.StartSkill();
        transform.localPosition = Vector3.zero;
        StartCoroutine(CoDamage());
    }

    private IEnumerator CoDamage()
    {
        var delay = new WaitForSeconds(_hitDelay);
        
        while (true)
        {
            Managers.Monster.FindMonstersInGameView().ForEach(monster =>
                {
                    double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                    double criticalMultiple =
                        monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                    monster.Damage(damage, criticalMultiple);
                }
            );

            yield return delay;
        }
    }

    private void Update()
    {
        if (_animator.IsEndCurrentAnimation())
            StopSkill();
    }
}