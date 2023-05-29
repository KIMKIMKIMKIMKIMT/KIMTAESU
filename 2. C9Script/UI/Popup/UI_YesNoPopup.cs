using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_YesNoPopup : UI_Popup
{
    [SerializeField] private TMP_Text MessageText;
    
    [SerializeField] private Button YesButton;
    [SerializeField] private Button NoButton;

    [SerializeField] private GameObject BackgroundObj;

    private Action _yesCallback;
    private Action _noCallback;

    public override bool isTop => true;

    private void Start()
    {
        BackgroundObj.BindEvent(_noCallback);
        YesButton.BindEvent(_yesCallback);
        NoButton.BindEvent(_noCallback);
    }

    private void OnDisable()
    {
        Managers.UI.DeletePopup<UI_YesNoPopup>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _noCallback?.Invoke();
            //Managers.UI.ClosePopupUI();
        }
    }

    public void Init(string message, Action yesCallback, Action noCallback = null)
    {
        MessageText.text = message;
        
        _yesCallback = yesCallback;
        _noCallback = noCallback ?? ClosePopup;
    }
    
    
}