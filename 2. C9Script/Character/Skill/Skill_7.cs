using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Skill_7 : BaseSkill
{
    private float _skillDuration;
    private int _hitCount;
    private float _hitDelay;
    private List<Monster> _targetMonsters = new();

    protected override void Init()
    {
        base.Init();

        Id = 7;

        _skillDuration = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Skill_Duration)].Value;
        _hitCount = (int)ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Count)].Value;
        _hitDelay = _skillDuration / _hitCount;
    }

    public override void StartSkill()
    {
        base.StartSkill();

        transform.SetParent(Managers.Game.MainPlayer.transform);
        transform.localPosition = new Vector3(-0.2f, 1.5f);

        _targetMonsters.Clear();

        Managers.Game.MainPlayer.SetSkillAnimation(Id);
        StartCoroutine(CoDamage());
        StartCoroutine(CoSkillDurationTimer());
    }

    private void Update()
    {
        if (Managers.Game.MainPlayer.TargetMonster != null && Managers.Game.MainPlayer.TargetMonster.IsDead)
            Managers.Game.MainPlayer.TargetMonster = null;
        
        if (Managers.Game.MainPlayer.TargetMonster == null)
        {
            Managers.Game.MainPlayer.FindTarget();
            if (Managers.Game.MainPlayer.TargetMonster == null)
                return;
        }

        if (Vector3.Distance(Managers.Game.MainPlayer.TargetMonster.transform.position,
                Managers.Game.MainPlayer.CenterPos) <= 0.1f)
            return;

        Vector3 direction = Managers.Game.MainPlayer.TargetMonster.transform.position -
                            Managers.Game.MainPlayer.CenterPos;
        direction.Normalize();

        Managers.Game.MainPlayer.transform.Translate(direction *
                                                     (Managers.Game.MainPlayer.MoveSpeed.Value * Time.deltaTime));
    }

    private IEnumerator CoDamage()
    {
        var hitDelay = new WaitForSeconds(_hitDelay);

        while (true)
        {
            var targetMonsters = _targetMonsters.ToList();

            targetMonsters.ForEach(
                monster =>
                {
                    if (monster == null)
                        return;
                    
                    double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                    double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                    monster.Damage(damage, criticalMultiple);
                });

            yield return hitDelay;
        }
    }

    private IEnumerator CoSkillDurationTimer()
    {
        yield return new WaitForSeconds(_skillDuration);
        StopSkill();
    }

    public override void StopSkill()
    {
        base.StopSkill();
        StopAllCoroutines();
        Managers.Game.MainPlayer.OnEndSkillAnimation(Id);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.tag.Equals("Monster"))
            return;

        var monster = col.GetComponent<Monster>();

        if (monster == null)
            return;

        _targetMonsters.Add(monster);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.tag.Equals("Monster"))
            return;

        var monster = other.GetComponent<Monster>();
        if (monster == null)
            return;

        if (_targetMonsters.Contains(monster))
            _targetMonsters.Remove(monster);
    }
}