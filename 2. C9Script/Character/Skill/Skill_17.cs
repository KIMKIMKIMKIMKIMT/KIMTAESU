using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class Skill_17 : BaseSkill
{
    #region Fields
    private AnimationEventReceiver _animationEventReceiver;
    private CinemachineImpulseListener _listener;
    private CinemachineImpulseSource _source;

    private int _hitCount;
    private int _curHitCount;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _animationEventReceiver = GetComponent<AnimationEventReceiver>();
        _listener = Managers.Game.MainPlayer._gameCamera.GetComponent<CinemachineImpulseListener>();
        _source = GetComponent<CinemachineImpulseSource>();

        _animationEventReceiver.OnAnimationEventAttack += StartCor_Attack;
        _animationEventReceiver.OnAnimationEventAttack_2 += StartCor_FinalAttack;

        _animationEventReceiver.OnAnimationEventEndAttack += StopSkill;
    }
    #endregion

    #region Protected Methods
    protected override void Init()
    {
        base.Init();
        
        Id = 17;

        _hitCount = (int)ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Count)].Value;
    }
    #endregion

    #region Public Methods
    public override void StartSkill()
    {
        base.StartSkill();

        _curHitCount = 0;
    }

    public override void StopSkill()
    {
        base.StopSkill();
        _listener.m_ReactionSettings.m_Duration = 0.1f;
        _listener.m_Gain = 5f;
        StopAllCoroutines();
    }
    #endregion

    #region Private Methods
    private void StartCor_Attack(float percent)
    {
        if (percent == 0)
            percent = 1;

        StopCoroutine(Cor_Attack(percent));
        StartCoroutine(Cor_Attack(percent));
    }

    private void StartCor_FinalAttack(float percent)
    {
        if (percent == 0)
            percent = 1;

        StopCoroutine(Cor_FianlAttack(percent));
        StartCoroutine(Cor_FianlAttack(percent));
    }
    #endregion

    #region Coroutines
    private IEnumerator Cor_Attack(float percent)
    {
        WaitForSeconds delay = new WaitForSeconds(0.08f);
        _source.GenerateImpulse();

        for (int i = 0; i < 2; i++)
        {
            Managers.Monster.FindMonstersInGameView().ForEach(monster =>
            {
                double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                monster.Damage(damage * percent, criticalMultiple);
            });

            _curHitCount++;
            yield return delay;
        }
    }

    private IEnumerator Cor_FianlAttack(float percent)
    {
        WaitForSeconds delay = new WaitForSeconds(0.05f);
        _listener.m_ReactionSettings.m_Duration = 2f;
        _listener.m_Gain = 50f;
        
        _source.GenerateImpulse();
        while (true)
        {
            Managers.Monster.FindMonstersInGameView().ForEach(monster =>
            {
                double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                monster.Damage(damage * percent, criticalMultiple);
            });

            _curHitCount++;

            if (_curHitCount >= _hitCount)
            {
                _listener.m_ReactionSettings.m_Duration = 0.1f;
                _listener.m_Gain = 5f;
                yield break;
            }

            yield return delay;
        }
    }
    #endregion
}
