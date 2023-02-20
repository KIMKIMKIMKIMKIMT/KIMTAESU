using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCANVAS_STATE
{
    Title,
    Main,

    Max
}
public class UIMgr : SingletonMonoBehaviour<UIMgr>
{
    #region Fields
    [SerializeField] private TitleUI _titleUI;
    [SerializeField] private MainUI _mainUI;
    public MainUI MainUI { get { return _mainUI; } }

    public UI_Top TopUI;
    public GetReward Reward;

    public UI_Popup _ui_Popup;
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (PlayerDataMgr.Instance.IsLogin)
        {
            SetCanvasState(eCANVAS_STATE.Main);
        }
    }
    #endregion
    #region Public Methods
    public void SetCanvasState(eCANVAS_STATE state)
    {
        switch (state)
        {
            case eCANVAS_STATE.Title:
                _titleUI.gameObject.SetActive(true);
                _mainUI.gameObject.SetActive(false);
                break;
            case eCANVAS_STATE.Main:
                _titleUI.gameObject.SetActive(false);
                _mainUI.gameObject.SetActive(true);
                break;
        }
    }
    public void Refresh()
    {
        TopUI.Refresh();
        _mainUI.Refresh();
    }
    #endregion
}
