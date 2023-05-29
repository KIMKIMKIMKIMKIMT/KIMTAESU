using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEditor;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("미사일 속도")] [SerializeField] float MaxSpeed = 0f;

    [Header("미사일 움직임 범위")] [SerializeField]
    float kProportionalConst = 0f;
    
    [Header("미사일 유지 시간")] 
    [SerializeField] private float Duration = 2f;

    [SerializeField] private Transform missileSpriteTransform = null;
    [SerializeField] private CircleCollider2D CircleCollider2D;

    private Monster _targetMonster;

    private Vector2 desiredVelocity; // our missiles desired velocity
    private Vector2 error;
    private Vector2 currentVelocity;
    private Vector2 sForce; // Steering force - force by which to "steer" the missile in the direction it wants to go.
    private Vector2 targetPosition;

    private double _damage;
    private double _bossDamage;
    private double _criticalMultiple;
    private double _bossCriticalMultiple;

    private Action<Missile> _endEvent;

    private List<SpriteRenderer> _spriteRenderers;

    private void Awake()
    {
        _spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>().ToList();
        
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            _spriteRenderers.Add(sr);

        Managers.Game.SettingData.SkillEffect.Subscribe(value =>
        {
            _spriteRenderers.ForEach(spriteRenderer => spriteRenderer.enabled = value);
        });
    }

    public void Shoot(Vector2 position, double damage, double bossDamage, double criticalMultiple, double bossCriticalMultiple, Action<Missile> endEvent)
    {
        transform.position = position;
        _damage = damage;
        _bossDamage = bossDamage;
        _criticalMultiple = criticalMultiple;
        _bossCriticalMultiple = bossCriticalMultiple;
        _endEvent = endEvent;

        gameObject.SetActive(true);

        ChangeTarget();
        StartCoroutine(CoDestroyTimer());
    }

    private IEnumerator CoDestroyTimer()
    {
        yield return new WaitForSeconds(Duration);
        gameObject.SetActive(false);
        _endEvent?.Invoke(this);
    }

    void Update()
    {
        if (_targetMonster == null || _targetMonster.IsDead)
        {
            ChangeTarget();
            if (_targetMonster == null)
                return;
        }

        var subValue = (Vector2)_targetMonster.transform.position - (Vector2)transform.position;

        // Normalize the subtracted value
        subValue.Normalize();

        // Multiply the normalized value by the missile's maxSpeed to get the desired Velocity.
        desiredVelocity = subValue * MaxSpeed;

        // Calculate missile error
        error = desiredVelocity - currentVelocity;

        // The force we apply to minimize our error is our error times our constant called kProportionalConst
        sForce = error * kProportionalConst;

        // Assign the current velocity by adding itself to steering force * deltaTime
        currentVelocity = currentVelocity + (sForce * Time.deltaTime) * (60f / ChulguIdleRpg.Application.Instance.GetFPS());

        // Move the missile according to all of the above calculation on the 2D plane
        transform.Translate(new Vector2(currentVelocity.x, currentVelocity.y));

        // Rotate the sprite to face the target - note: using a missile sprite nested in a parent gameobject so that the rotation works correctly. Otherwise it goes way out and the missile
        // position is messed up. So we rotate the nested sprite, but move the actual gameobject, which also houses the collider2D bounding box
        float targetAngle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(targetAngle - 90f, Vector3.forward);
        missileSpriteTransform.rotation = angleAxis;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        var monster = collision.GetComponent<Monster>();

        if (monster == null)
            return;
        
        double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
        double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
        monster.Damage(damage, criticalMultiple);
        
        if (Managers.Game.SettingData.SkillEffect.Value)
            Managers.Effect.ShowEffect("Skill_8_HitEffect", monster.transform.position);
        
        gameObject.SetActive(false);

        _endEvent?.Invoke(this);
    }

    void ChangeTarget()
    {
        var targetMonster = Managers.Monster.FindRandomMonsterInGameView();

        if (targetMonster == null)
            targetMonster = Managers.Monster.FindRandomMonster();

        if (targetMonster)
        {
            _targetMonster = targetMonster;
        }
    }

    public void SetActiveCollider(bool isActive)
    {
        CircleCollider2D.enabled = isActive;
    }
}