using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossNoticePopup : UI_Popup
{
    [SerializeField] private Button OkButton;
    [SerializeField] private Button CancelButton;

    public override bool isTop => true;

    private void Start()
    {
        OkButton.BindEvent(OnClickOk);
        CancelButton.BindEvent(ClosePopup);
    }

    private void OnClickOk()
    {
        Managers.Stage.IsAutoBoss.Value = true;
        Managers.Stage.StartBoss();
        ClosePopup();
    }
}