using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public Action<float> OnAnimationEventAttack;
    public Action<float> OnAnimationEventAttack_2;
    public Action OnAnimationEventEndAttack;
    public Action OnAnimationEventAppear;
    public Action OnAnimationEventEndAppear;
    public Action OnAnimationEventAttackEffect_1;
    public Action<int> OnAnimationEventAttackEffect_2;
    public Dictionary<int, Action> OnSkillEffect = new();

    public void OnAnimationEvent_Attack(float percent = 1f)
    {
        if (percent == 0)
            percent = 1;
        OnAnimationEventAttack?.Invoke(percent);
    }

    public void OnAnimationEvent_AttackEffect_1()
    {
        OnAnimationEventAttackEffect_1?.Invoke();
    }

    public void OnAnimationEvent_Attack_2(float percent = 1f)
    {
        if (percent == 0)
            percent = 1;
        OnAnimationEventAttack_2?.Invoke(percent);
    }

    public void OnAnimationEvent_Attack_2_Effect(int count)
    {
        OnAnimationEventAttackEffect_2?.Invoke(count);
    }

    public void OnAnimationEvent_EndAttack()
    {
        OnAnimationEventEndAttack?.Invoke();
    }

    public void OnAnimationEvent_PlaySkillSound(int soundId)
    {
        
    }

    public void OnAnimationEvent_PlayMonsterAttackSound(string soundName)
    {
        Managers.Sound.PlayMonsterAttackSound(soundName);
    }

    public void OnAnimationEvent_AppearEffect()
    {
        OnAnimationEventAppear?.Invoke();
    }

    public void OnAnimationEvent_EndAppear()
    {
        OnAnimationEventEndAppear?.Invoke();
    }
}
