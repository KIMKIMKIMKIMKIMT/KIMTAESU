using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChatBlockItem : UI_Base
{
    [SerializeField] private TMP_Text NicknameText;
    [SerializeField] private Button UnblockButton;
    
    private string _nickname;
    private Action<string> _unblockAction;

    private void Start()
    {
        UnblockButton.BindEvent(OnClickUnblock);
    }

    public void Init(string nickname, Action<string> unblockAction)
    {
        _nickname = nickname;
        _unblockAction = unblockAction;

        SetUI();
    }

    private void SetUI()
    {
        NicknameText.text = _nickname;
    }

    private void OnClickUnblock()
    {
        _unblockAction?.Invoke(_nickname);
    }
}