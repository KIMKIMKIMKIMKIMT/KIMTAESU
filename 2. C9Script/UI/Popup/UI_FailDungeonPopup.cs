using TMPro;
using UI;
using UnityEngine;


public class UI_FailDungeonPopup : UI_Popup
{
    [SerializeField] private TMP_Text DescText;
    public override bool isTop => true;

    public override void Open()
    {
        base.Open();
        DescText.text = Managers.Stage.State.Value == StageState.Dps ? "더 강해져서 돌아오자" : "사용된 티켓은 반환됩니다";
    }
}