using System;
using BackEnd;
using Firebase.Analytics;
using UnityEngine;

public class Skill_12_Missile : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    private static readonly int BoomHash = Animator.StringToHash("Boom");

    private double _damage;
    private double _bossDamage;
    private double _criticalMultiple;
    private double _bossCriticalMultiple;
    private float _speed;
    private float _hitRange;
    private Vector3 _destination;

    private bool _isFire = false;

    public void Fire(double damage, double bossDamage, double criticalMultiple, double bossCriticalMultiple, float speed, float hitRange, Vector3 destination)
    {
        _damage = damage;
        _bossDamage = bossDamage;
        _criticalMultiple = criticalMultiple;
        _bossCriticalMultiple = bossCriticalMultiple;
        _speed = speed;
        _hitRange = hitRange;
        _destination = destination;
        
        _isFire = true;
    }

    private void OnDisable()
    {
        _isFire = false;
    }

    private void Update()
    {
        if (!_isFire)
            return;
        
        if (Vector3.Distance(transform.position, _destination) <= 0.5f)
        {
            Animator.SetTrigger(BoomHash);
            _isFire = false;
            return;
        }

        var direction = _destination - transform.position;
        direction.Normalize();
        transform.Translate(direction * (_speed * Time.deltaTime));
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