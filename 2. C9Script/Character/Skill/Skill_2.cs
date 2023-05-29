
// 제자리에서 잠시 불을 모은 후 날라차기를 시전
// Player_Skill 에서 애니메이션 이벤트로 작동
public class Skill_2 : BaseSkill
{
    protected override void Init()
    {
        Id = 2;
    }

    public override void StartSkill()
    {
        base.StartSkill();
        Managers.Game.MainPlayer.SetSkillAnimation(Id);
    }
}