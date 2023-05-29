using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_ClearRaidPopup : UI_Popup
{
    [SerializeField] private TMP_Text StepText;
    [SerializeField] private TMP_Text TimeText;
    [SerializeField] private TMP_Text GoldText;
    [SerializeField] private TMP_Text GoldBarText;
    [SerializeField] private TMP_Text SkillStoneText;

    [SerializeField] private Image Wave3RewardImage;
    [SerializeField] private Sprite SkillStoneSprite;
    [SerializeField] private Sprite GuildPointSprite;
    [SerializeField] private Sprite GuildAllRaidPointSprite;

    public void Init(int step, float time, double gold, double goldBar, double skillStone)
    {
        if (Managers.Stage.State.Value == StageState.Raid)
            Wave3RewardImage.sprite = SkillStoneSprite;
        else if (Managers.Stage.State.Value == StageState.GuildRaid)
            Wave3RewardImage.sprite = GuildPointSprite;
        else if (Managers.Stage.State.Value == StageState.GuildAllRaid)
            Wave3RewardImage.sprite = GuildAllRaidPointSprite;
        
        StepText.text = $"{step}단계 성공!";

        int min = (int)(time / 60);
        float sec = time % 60;

        TimeText.text = min > 0 ? $"{min}분 {sec:N2}초" : $"{sec:N2}초";

        GoldText.text = gold.ToCurrencyString();
        GoldBarText.text = goldBar.ToCurrencyString();
        SkillStoneText.text = skillStone.ToCurrencyString();
    }
}