using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_NoticePopup : UI_Popup
{
    [SerializeField] private TMP_Text ContentText;
    [SerializeField] private Button Button;

    private Action _callback;
    
    private void Start()
    {
        Button.BindEvent(() =>
        {
            _callback?.Invoke();
            ClosePopup();
        });
    }
    
    public void Init(string message, Action callback)
    {
        Debug.Log("Debug Call");
        ContentText.text = message;
        _callback = callback;
    }
}