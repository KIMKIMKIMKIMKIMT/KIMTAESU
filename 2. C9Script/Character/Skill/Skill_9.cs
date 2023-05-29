using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 간력장
// 중앙으로 몬스터를 끌어당긴다.
public class Skill_9 : BaseSkill
{
    [Header("끌어당기는 속도")]
    [SerializeField] private float PullSpeed;

    [SerializeField] private Animator Animator;
    
    private float _areaSize;
    private List<Monster> _monsters = new();

    private void Awake()
    {
        Id = 9;
    }

    protected override void Init()
    {
        base.Init();

        Id = 9;
        _areaSize = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Area_Size)].Value;
    }

    public override void StartSkill()
    {
        base.StartSkill();
        transform.position = Managers.Game.MainPlayer.CenterPos;

        StartCoroutine(CoDamage());
    }

    private void Update()
    {
        if (Animator.IsEndCurrentAnimation())
            StopSkill();

        if (Managers.Stage.State.Value == StageState.Raid && Managers.Raid.Wave.Value == 3)
            return;
        
        if (Managers.Stage.State.Value == StageState.GuildRaid && Managers.GuildRaid.Wave.Value == 3)
            return;

        if (Managers.Stage.State.Value == StageState.Dps || Managers.Stage.State.Value == StageState.Promo)
            return;
        
        _monsters = Managers.Monster.FindTargetMonsters(transform.position, _areaSize);

        _monsters.ForEach(monster =>
        {
            if (Vector3.Distance(monster.transform.position, transform.position) <= 0.2f)
                return;
                
            monster.transform.position =
                Vector3.Slerp(monster.transform.position, transform.position, PullSpeed * Time.deltaTime);
        });
    }

    private IEnumerator CoDamage()
    {
        float animationTime = Animator.GetCurrentAnimatorStateInfo(0).length;
        var hitDelay = new WaitForSeconds(animationTime / ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Count)].Value);

        while (true)
        {
            _monsters.ForEach(monster =>
            {
                if (monster == null)
                    return;

                if (monster.IsDead)
                    return;
                
                double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                monster.Damage(damage, criticalMultiple);
            });

            yield return hitDelay;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _areaSize);
    }
}