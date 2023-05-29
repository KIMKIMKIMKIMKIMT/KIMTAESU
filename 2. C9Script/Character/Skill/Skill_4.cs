using System.Collections;
using UnityEngine;

public class Skill_4 : BaseSkill
{
    [SerializeField] private GameObject AttackEffect;
    [SerializeField] private Animator AttackEffectAnimator;

    private float _attackRange;
    private float _skillDuration;

    private bool endFlag;

    protected override void Init()
    {
        Id = 4;

        _attackRange = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Range)].Value;
        _skillDuration = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Skill_Duration)].Value;
    }

    public override void StartSkill()
    {
        if (Managers.Game.MainPlayer.State.Value == CharacterState.Skill)
            Managers.Game.MainPlayer.State.Value = CharacterState.Idle;

        AttackEffect.SetActive(false);

        base.StartSkill();

        endFlag = false;

        Managers.Game.MainPlayer.OnAnimationAttackEvent -= OnAttackEffect;
        Managers.Game.MainPlayer.OnAnimationAttackEvent += OnAttackEffect;

        StopCoroutine(CoSkillDurationTimer());
        StartCoroutine(CoSkillDurationTimer());
    }

    public override void StopSkill()
    {
        base.StopSkill();
        Managers.Game.MainPlayer.OnAnimationAttackEvent -= OnAttackEffect;
        Managers.Game.MainPlayer.OnEndSkillAnimation(4);
    }

    private IEnumerator CoSkillDurationTimer()
    {
        yield return new WaitForSeconds(_skillDuration);
        endFlag = true;
    }

    private void Update()
    {
        if (!AttackEffect.activeSelf)
            return;

        if (AttackEffectAnimator.IsEndCurrentAnimation())
        {
            AttackEffect.SetActive(false);
            if (endFlag)
            {
                StopSkill();
            }
                
        }
    }

    private void OnAttackEffect()
    {
        transform.position = Managers.Game.MainPlayer.transform.position;

        Vector3 localScale = transform.localScale;
        float xScale = Mathf.Abs(localScale.x);

        if (Managers.Game.MainPlayer.Direction.Value == CharacterDirection.Left)
            xScale = -xScale;

        localScale.x = xScale;
        transform.localScale = localScale;
        
        AttackEffect.SetActive(true);

        Managers.Monster.FindTargetMonsters(Managers.Game.MainPlayer.CenterPos, Managers.Game.MainPlayer.Direction.Value, _attackRange)
            .ForEach(monster =>
        {
            double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
            double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
            monster.Damage(damage, criticalMultiple);
        });
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Managers.Game.MainPlayer.CenterPos, _attackRange);
    }
}