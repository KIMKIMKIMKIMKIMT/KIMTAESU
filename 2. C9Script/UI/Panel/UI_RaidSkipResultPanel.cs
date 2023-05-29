using TMPro;
using UI;
using UnityEngine;

public class UI_RaidSkipResultPanel : UI_Panel
{
    [SerializeField] private GameObject BackgroundObj;

    [SerializeField] private TMP_Text StepText;
    [SerializeField] private TMP_Text GoldText;
    [SerializeField] private TMP_Text GoldBarText;
    [SerializeField] private TMP_Text SkillStoneText;
    [SerializeField] private TMP_Text SkipCountText;

    private void Start()
    {
        BackgroundObj.BindEvent(Close);
    }

    public void Init(int clearStep, double gold, double goldBar, double skillStone, double skipCount)
    {
        StepText.text = $"{clearStep}단계 스킵 성공!";
        GoldText.text = $"골드 : {gold.ToCurrencyString()}";
        GoldBarText.text = $"골드바 : {goldBar.ToCurrencyString()}";
        SkillStoneText.text = $"스킬 강화석 : {skillStone.ToCurrencyString()}";
        SkipCountText.text = $"X{skipCount.ToCurrencyString()}";
    }
}