using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

// 1. 소환된 철구가 주변에서 일정시간 회전하며 원 범위에 데미지를 준다
// 2. 회전 시간이 다 되면 일정시간 정지 했다가 주위로 발사되며 충돌한 몬스터에게 데미지를 준다
public class Skill_5 : BaseSkill
{
    [Header("회전 속도")]
    [SerializeField] private float RotationSpeed = 3;

    [Header("발사 후 커지는 최종 크기")]
    [SerializeField] private float IncreaseSize;

    [Header("발사 속도")] 
    [SerializeField] private float FireSpeed;

    [Header("회전 크기")] 
    [SerializeField] private float Radius;
    
    [SerializeField] private Transform[] SkillEffectObjs;
    [SerializeField] private CircleCollider2D[] SkillEffectColliders;

    [SerializeField] private CircleCollider2D CircleCollider2D;
    

    private float[] _startRadianAngles;
    private float[] _radianAngles;
    private Vector3[] _fireDirections;
    private float _areaDelay;
    
    
    private float _duration;
    private int _hitCount;

    private bool _isRotation;
    private bool _isFire;

    private readonly CompositeDisposable _compositeDisposable = new();

    private List<Monster> _areaTargetMonster = new();

    private void Awake()
    {
        Id = 5;
    }

    protected override void Init()
    {
        _startRadianAngles = new float[SkillEffectObjs.Length];
        _radianAngles = new float[SkillEffectObjs.Length];

        float radianAngle = (Mathf.Deg2Rad * 360) / SkillEffectObjs.Length;

        for (int i = 0; i < _startRadianAngles.Length; i++)
        {
            _startRadianAngles[i] = radianAngle * i;
        }
        
        _duration = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Area_Duration)].Value;
        _areaDelay = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Area_Delay)].Value;
        _hitCount = (int)ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Count)].Value;

        CircleCollider2D.radius = Radius * 4;
    }

    public override void StartSkill()
    {
        base.StartSkill();

        Transform tr = transform;
        
        tr.SetParent(Managers.Game.MainPlayer.transform);
        tr.localPosition = new Vector3(0, 1.5f, 0);

        _compositeDisposable.Clear();

        // 시작 위치 초기화
        for (int i = 0; i < _radianAngles.Length; i++)
        {
            _radianAngles[i] = _startRadianAngles[i]; 
            SkillEffectObjs[i].position = CalculateAnglePosition(i);
            SkillEffectObjs[i].localScale = Vector3.one;
            
            SkillEffectColliders[i].enabled = false;
            SkillEffectObjs[i].gameObject.SetActive(true);
        }
        
        _isRotation = true;
        StartCoroutine(CoRotationDamage());

        Observable.Timer(TimeSpan.FromSeconds(_duration)).Subscribe(_ =>
        {
            tr.SetParent(null);
            
            _isRotation = false;
            Observable.Timer(TimeSpan.FromSeconds(_areaDelay)).Subscribe(_ =>
            {
                Fire();
            }).AddTo(_compositeDisposable);
        }).AddTo(_compositeDisposable);
    }

    public override void StopSkill()
    {
        base.StopSkill();
        _compositeDisposable.Clear();
        _isFire = false;
        gameObject.SetActive(false);
    }

    private IEnumerator CoRotationDamage()
    {
        var damageDelay =
            new WaitForSeconds(_duration / _hitCount);

        while (true)
        {
            var areaTargetMonsters = _areaTargetMonster.ToList();
            areaTargetMonsters.ForEach(monster =>
            {
                if (monster == null)
                    return;
                
                double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                monster.Damage(damage, criticalMultiple);
            });
            
            if (!_isRotation)
                yield break;
            
            yield return damageDelay;
        }
    }

    private void Update()
    {
        if (_isRotation)
            UpdateRotation();
        
        if (_isFire)
            UpdateFire();
    }

    private void UpdateRotation()
    {
        for (int i = 0; i < _radianAngles.Length; i++)
        {
            _radianAngles[i] += Time.deltaTime * RotationSpeed;
            SkillEffectObjs[i].position = CalculateAnglePosition(i);
        }
    }

    private void UpdateFire()
    {
        for (int i = 0; i < SkillEffectObjs.Length; i++)
        {
            SkillEffectObjs[i].Translate(_fireDirections[i] * (FireSpeed * Time.deltaTime));
        }
    }

    private Vector2 CalculateAnglePosition(int index)
    {
        float x = Radius * Mathf.Cos(_radianAngles[index]);
        float y = Radius * Mathf.Sin(_radianAngles[index]);
        return new Vector2(x, y) + (Vector2)transform.position;
    }

    private void Fire()
    {
        _fireDirections = new Vector3[SkillEffectObjs.Length];
        
        for (int i = 0; i < SkillEffectObjs.Length; i++)
        {
            Vector3 direction = SkillEffectObjs[i].transform.position - transform.position;
            direction.Normalize();
            _fireDirections[i] = direction;
            SkillEffectObjs[i].DOScale(new Vector3(IncreaseSize, IncreaseSize, IncreaseSize), 
                ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Missile_Duration)].Value
            );
            
            SkillEffectColliders[i].enabled = true;
            SkillEffectColliders[i].OnTriggerEnter2DAsObservable().Subscribe(col =>
            {
                Monster monster = col.GetComponent<Monster>();
                if (monster == null)
                    return;
                
                double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                monster.Damage(damage, criticalMultiple);
            }).AddTo(_compositeDisposable);
        }

        _isFire = true;

        Observable.Timer(
                TimeSpan.FromSeconds(ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Missile_Duration)].Value))
            .Subscribe(
                _ =>
                {
                    StopSkill();
                }).AddTo(_compositeDisposable);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Radius + 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.tag.Equals("Monster"))
            return;
        
        _areaTargetMonster.Add(col.GetComponent<Monster>());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.tag.Equals("Monster"))
            return;

        var monster = other.GetComponent<Monster>();
        if (monster == null)
            return;

        if (_areaTargetMonster.Contains(monster))
            _areaTargetMonster.Remove(monster);
    }
}