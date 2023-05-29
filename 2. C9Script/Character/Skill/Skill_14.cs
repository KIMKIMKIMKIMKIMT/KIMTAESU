using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Skill_14 : BaseSkill
{
    [Header("다음 오브젝트 소환되는 주기")]
    [SerializeField] private float SpawnDelayTime = 0.02f;
    
    [SerializeField] private Skill_14_Object[] Skill14Objects;

    private Camera _gameCamera;
    private float _hitRange;
    private int _hitCount;
    private float _hitDelay;
    
    public float HitRangeX = 13;
    public float HitRangeY = 23;

    private List<Monster> _targetMonster = new();

    protected override void Init()
    {
        base.Init();

        Id = 14;
        _gameCamera = Camera.main;

        _hitRange = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Range)].Value;
        _hitCount = (int)ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Count)].Value;
        _hitDelay = 3.11f / _hitCount;
    }

    public override void StartSkill()
    {
        base.StartSkill();

        transform.position = Vector2.zero;
        
        foreach (var skill14Object in Skill14Objects)
            skill14Object.gameObject.SetActive(false);

        StartCoroutine(CoSpawn());
    }

    private IEnumerator CoSpawn()
    {
        var spawnDelay = new WaitForSeconds(SpawnDelayTime);
        ShuffleObjects();
        
        StartCoroutine(CoDamage());

        foreach (var skill14Object in Skill14Objects)
        {
            skill14Object.gameObject.SetActive(true);
            skill14Object.Init(_damage, _bossDamage, _criticalMultiple, _bossCriticalMultiple, _hitRange);
            yield return spawnDelay;
        }
        
        StartCoroutine(CoCheckSkill());
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.tag.Equals("Monster"))
            return;

        var monster = col.GetComponent<Monster>();
        if (monster == null)
            return;

        if (!_targetMonster.Contains(monster))
            _targetMonster.Add(monster);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.tag.Equals("Monster"))
            return;

        var monster = other.GetComponent<Monster>();
        if (monster == null)
            return;

        if (_targetMonster.Contains(monster))
            _targetMonster.Remove(monster);
    }

    private IEnumerator CoDamage()
    {
        var damageDelay = new WaitForSeconds(_hitDelay);

        while (true)
        {
            Managers.Monster.FindMonstersInGameView().ForEach(monster =>
            {
                double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                monster.Damage(damage, criticalMultiple);
            });
            
            yield return damageDelay;
        }
    }

    private IEnumerator CoCheckSkill()
    {
        while (true)
        {
            bool isOn = false;
            
            foreach (var skill14Object in Skill14Objects)
            {
                if (!skill14Object.gameObject.activeSelf)
                    continue;

                isOn = true;
            }

            if (!isOn)
                break;

            yield return null;
        }
        
        StopSkill();
    }

    private void ShuffleObjects()
    {
        var objects = Skill14Objects;

        for (int i = 0; i < objects.Length; ++i)
        {
            int random1 = Random.Range(0, objects.Length);
            int random2 = Random.Range(0, objects.Length);

            (objects[random1], objects[random2]) = (objects[random2], objects[random1]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(HitRangeX, HitRangeY, 0));
    }
}