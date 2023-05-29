using UnityEngine;


public class Player_AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private Player player;

    public void OnAnimationEvent_Attack()
    {
        player.OnAnimationEvent_Attack();
    }

    public void OnAnimationEvent_EndAttack()
    {
       player.OnAnimationEvent_EndAttack();
    }

    #region 스킬(2, 7)

    public void OnEndSkillAnimation(int skillId)
    {
        player.OnEndSkillAnimation(skillId);
    }

    public void OnSkill2_Move()
    {
        player.OnSkill2_Move();
    }

    public void OnSkill2Damage()
    {
        player.OnSkill2Damage();
    }

    public void PlaySkillSound(int skillId)
    {
        Managers.Sound.PlaySkillSound(2);
    }

    #endregion
}