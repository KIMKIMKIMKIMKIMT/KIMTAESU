using UniRx;

public class SkillInfo
{
    private int _skillIndex;

    public int SkillIndex
    {
        get => _skillIndex;
        set
        {
            _compositeDisposable.Clear();
            _skillIndex = value;

            if (_skillIndex != 0 && Managers.Game.MainPlayer != null)
            {
                if (!Managers.Game.MainPlayer.SkillCoolTimes.ContainsKey(_skillIndex))
                    Managers.Game.MainPlayer.SkillCoolTimes.Add(_skillIndex, new());

                SkillCoolTime = ChartManager.SkillCharts[_skillIndex].CoolTime;
                Managers.Game.MainPlayer.SkillCoolTimes[_skillIndex].Subscribe(skillCoolTime =>
                {
                    RemainSkillCoolTime.Value = skillCoolTime;
                }).AddTo(_compositeDisposable);
            }
            else
            {
                SkillCoolTime = 0;
                RemainSkillCoolTime.Value = 0;
            }
        }
    }

    public float SkillCoolTime;
    public readonly ReactiveProperty<float> RemainSkillCoolTime;
    public readonly IReadOnlyReactiveProperty<bool> IsCoolTime;

    private readonly CompositeDisposable _compositeDisposable = new();

    public SkillInfo()
    {
        RemainSkillCoolTime = new ReactiveProperty<float>(0);
        IsCoolTime = RemainSkillCoolTime.Select(remainSkillCoolTime => remainSkillCoolTime > 0)
            .ToReactiveProperty();
        SkillIndex = 0;
    }
}