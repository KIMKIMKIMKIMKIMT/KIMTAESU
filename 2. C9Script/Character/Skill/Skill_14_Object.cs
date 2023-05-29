using System;
using UnityEngine;

public class Skill_14_Object : MonoBehaviour
{
    private double _damage;
    private double _bossDamage;
    private double _criticalMultiple;
    private double _bossCriticalMultiple;
    private float _hitRange;

    public void Init(double damage, double bossDamage, double criticalMultiple, double bossCriticalMultiple, float hitRange)
    {
        _damage = damage;
        _bossDamage = bossDamage;
        _criticalMultiple = criticalMultiple;
        _bossCriticalMultiple = bossCriticalMultiple;
        _hitRange = hitRange;
    }

    public void OnAnimationEvent_Damage()
    {
        Managers.Monster.FindTargetMonsters(transform.position, _hitRange).ForEach(monster =>
        {
            double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
            double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
            monster.Damage(damage, criticalMultiple);
        });
    }

    public void OnAnimationEvent_End()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _hitRange);
    }
}