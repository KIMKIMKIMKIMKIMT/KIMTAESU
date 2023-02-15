using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarUI : MonoBehaviour
{
    #region Fields
    [SerializeField] private Sprite[] _chapterSprite;
    [SerializeField] private Image _chapterImg;
    #endregion

    #region Unity Methods
    #endregion

    #region Public Methods
    public void OnClickDailyQuestPopup()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        UIMgr.Instance._ui_Popup.ShowDailyQuestPopup();
    }

    public void OnClickSetting()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        UIMgr.Instance._ui_Popup.ShowSettingPopup();
    }
    public void OnClickChapterSelection()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        UIMgr.Instance._ui_Popup.ShowChapterSelection();
    }

    public void OnClickGameStart()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        GameMgr.Instance.GameStart();
        GameMgr.Instance.QuestAddCnt(eQUEST.ChapterPlay);
    }
    public void SetChapterImg(int index)
    {
        _chapterImg.sprite = _chapterSprite[index];
    }
    #endregion
}
