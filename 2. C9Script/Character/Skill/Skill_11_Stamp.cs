using System;
using System.Collections;
using DG.Tweening;
using UniRx;
using UnityEngine;


public class Skill_11_Stamp : MonoBehaviour
{
    enum BehaviourType
    {
        None,
        Tracking,
        Down,
        Waiting,
        Up,
    }

    [SerializeField] private SpriteRenderer SpriteRenderer;

    [SerializeField] private string HitEffectName;

    [SerializeField] private Sprite BaseSprite;
    [SerializeField] private Sprite DownSprite;
    [SerializeField] private Sprite HitSprite;


    private float _moveSpeed;
    private float _downTime;
    private float _upTime;
    private float _attackDelay;
    private float _attackDelayTime;
    private int _hitCount;
    private double _damage;
    private double _bossDamage;
    private double _criticalMultiple;
    private double _bossCriticalMultiple;

    private float _downSpeed = 30;
    private float _upSpeed = 15;

    private float _height = 3f;

    private Monster _targetMonster;

    private bool _isStart;

    [HideInInspector] public int AttackCount = 0;

    private Vector3 _targetPos;
    private Vector3 _upPos;

    private CompositeDisposable _compositeDisposable = new();

    private BehaviourType _behaviourType;

    private float _waitingTime = 0;

    public void StartStamp(float moveSpeed, float downTime, float upTime, float attackDelay, int hitCount,
        double damage, double bossDamage, double criticalMultiple, double bossCriticalMultiple)
    {
        _compositeDisposable.Clear();

        _moveSpeed = moveSpeed;
        _downTime = downTime;
        _upTime = upTime;
        _attackDelay = attackDelay;
        _hitCount = hitCount;
        _damage = damage;
        _bossDamage = bossDamage;
        _criticalMultiple = criticalMultiple;
        _bossCriticalMultiple = bossCriticalMultiple;
        AttackCount = 0;

        SpriteRenderer.sprite = BaseSprite;

        _isStart = true;

        _targetMonster = null;

        _behaviourType = BehaviourType.Tracking;
    }

    private void OnDisable()
    {
        _compositeDisposable.Clear();
        _isStart = false;
    }

    private void Update()
    {
        if (!_isStart)
            return;

        if (_behaviourType == BehaviourType.None)
        {
            _attackDelayTime += Time.deltaTime;

            if (_attackDelayTime >= _attackDelay)
            {
                _behaviourType = BehaviourType.Tracking;
                return;
            }
            return;
        }

        switch (_behaviourType)
        {
            case BehaviourType.Tracking:
                UpdateTracking();
                break;
            case BehaviourType.Down:
                UpdateDown();
                break;
            case BehaviourType.Waiting:
                UpdateWaiting();
                break;
            case BehaviourType.Up:
                UpdateUp();
                break;
        }
    }

    private void UpdateTracking()
    {
        if (_targetMonster == null)
        {
            _targetMonster = Managers.Monster.FindRandomMonsterInGameView();
            if (_targetMonster == null)
                return;
        }

        if (_targetMonster != null && _targetMonster.IsDead)
        {
            _targetMonster = null;
            return;
        }
        
        var targetPos = _targetMonster.transform.position;
        targetPos.y += _height;

        if (Vector3.Distance(targetPos, transform.position) <= 1f)
        {
            _targetPos = _targetMonster.transform.position;
            SpriteRenderer.sprite = DownSprite;
            _behaviourType = BehaviourType.Down;
            return;
        }

        Vector3 direction = targetPos - transform.position;
        direction.Normalize();

        transform.Translate(direction * (_moveSpeed * Time.deltaTime));
    }

    private void UpdateDown()
    {
        if (Vector3.Distance(_targetPos, transform.position) <= 1f)
        {
            _waitingTime = 0;
            _behaviourType = BehaviourType.Waiting;
            
            if (Managers.Game.SettingData.SkillEffect.Value)
                Managers.Effect.ShowEffect(HitEffectName, new Vector2(transform.position.x, transform.position.y - 1.2f));

            if (_targetMonster != null && !_targetMonster.IsDead)
            {
                double damage = _targetMonster.Type == MonsterType.Boss ? _bossDamage : _damage;
                double criticalMultiple = _targetMonster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                _targetMonster.Damage(damage, criticalMultiple);
            }

            return;
        }
        
        Vector3 direction = _targetPos - transform.position;
        direction.Normalize();

        transform.Translate(direction * (_downSpeed * Time.deltaTime));
    }

    private void UpdateWaiting()
    {
        _waitingTime += Time.deltaTime;

        if (_waitingTime >= 1f)
        {
            SpriteRenderer.sprite = BaseSprite;
            AttackCount++;
                
            if (AttackCount >= _hitCount)
            {
                gameObject.SetActive(false);
                return;
            }
            
            var position = transform.position;
            _upPos = new Vector3(position.x, position.y + _height, position.z);
            _behaviourType = BehaviourType.Up;
        }
    }

    private void UpdateUp()
    {
        if (Vector3.Distance(_upPos, transform.position) <= 0.5f)
        {
            _behaviourType = BehaviourType.None;
            //StartCoroutine(CoAttackDelay());
            return;
        }
        
        Vector3 direction = _upPos - transform.position;
        direction.Normalize();
        
        transform.Translate(direction * (_upSpeed * Time.deltaTime));
    }

    private IEnumerator CoAttackDelay()
    {
        yield return new WaitForSecondsRealtime(_attackDelay);

        _behaviourType = BehaviourType.Tracking;
    }
}