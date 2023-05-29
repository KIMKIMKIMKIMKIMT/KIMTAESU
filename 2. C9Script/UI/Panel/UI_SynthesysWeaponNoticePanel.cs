using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_SynthesysWeaponNoticePanel : UI_Panel
{
    [SerializeField] private Button OkButton;
    [SerializeField] private Button[] CloseButtons;

    private Action _okCallback;

    private void Start()
    {
        OkButton.BindEvent(() =>
            {
                _okCallback?.Invoke(); 
                Close();
            }
        );
        foreach (var closeButton in CloseButtons)
            closeButton.BindEvent(Close);
    }

    public void Init(Action okCallback)
    {
        _okCallback = okCallback;
    }
}