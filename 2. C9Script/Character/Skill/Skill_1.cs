using System.Collections;
using UnityEngine;

// 소소한 주작
public class Skill_1 : BaseSkill
{
    private int _hitCount;
    private float _hitDelay;
    
    protected override void Init()
    {
        base.Init();
        
        Id = 1;
        
        transform.SetParent(Managers.Game.MainPlayer.transform);
        transform.localPosition = new Vector2(-2, 4);
        
        _hitCount = (int)ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Count)].Value;
        _hitDelay = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Delay)].Value;
    }

    public override void StartSkill()
    {
        base.StartSkill();
        StartCoroutine(CoAttack());
    }

    private IEnumerator CoAttack()
    {
        var hitDelay = new WaitForSeconds(_hitDelay);

        int currentHitCount = 0;

        while (true)
        {
            if (currentHitCount >= _hitCount)
                break;
            
            yield return hitDelay;

            Monster monster = Managers.Monster.FindTargetMonster(transform.position);

            if (monster != null)
            {
                SetDirection(monster.transform);
                
                double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                
                monster.Damage(damage, criticalMultiple);
                
                if (Managers.Game.SettingData.SkillEffect.Value)
                    Managers.Effect.ShowEffect("Skill_1_HitEffect", monster.transform.position);
            }

            currentHitCount++;
        }

        StopSkill();
    }

    private void SetDirection(Transform targetTr)
    {
        Vector3 direction = targetTr.position - transform.position;
        direction.Normalize();

        Vector3 localScale = transform.localScale;
        float xScale = Mathf.Abs(localScale.x);

        if (direction.x < 0)
        {
            xScale = -xScale;
        }

        localScale.x = xScale;
        transform.localScale = localScale;
    }
}