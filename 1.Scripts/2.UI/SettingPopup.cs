using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    #region Fields
    [SerializeField] private Image _bgmOnImg;
    [SerializeField] private Image _bgmOffImg;
    [SerializeField] private Image _sfxOnImg;
    [SerializeField] private Image _sfxOffImg;
    [SerializeField] private Image _bgmOnToggleImg;
    [SerializeField] private Image _bgmOffToggleImg;
    [SerializeField] private Image _sfxOnToggleImg;
    [SerializeField] private Image _sfxOffToggleImg;
    [SerializeField] private Image _bgmGreenBar;
    [SerializeField] private Image _sfxGreenBar;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        SetBgmToggle(PlayerDataMgr.Instance.PlayerData.BgmToggle);
        SetSfxToggle(PlayerDataMgr.Instance.PlayerData.SfxToggle);
    }
    #endregion

    #region Public Methods
    public void OnClickQuit()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PopupMgr.Instance.RemovePopup();
    }

    public void OnClickBgmToggle()
    {
        
        PlayerDataMgr.Instance.PlayerData.BgmToggle = !PlayerDataMgr.Instance.PlayerData.BgmToggle;
        SetBgmToggle(PlayerDataMgr.Instance.PlayerData.BgmToggle);
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }

        if (PlayerDataMgr.Instance.PlayerData.BgmToggle)
        {
            SoundMgr.Instance.PlayBGMPlayer(eAUDIOCLIP_BGM.Main);
        }
        else
        {
            SoundMgr.Instance.StopBGMPlayer();
        }
        
    }

    public void OnClickSfxToggle()
    {
        PlayerDataMgr.Instance.PlayerData.SfxToggle = !PlayerDataMgr.Instance.PlayerData.SfxToggle;
        SetSfxToggle(PlayerDataMgr.Instance.PlayerData.SfxToggle);
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
    }

    public void SetBgmToggle(bool toggle)
    {
        _bgmOnImg.gameObject.SetActive(toggle);
        _bgmOffImg.gameObject.SetActive(!toggle);
        _bgmOnToggleImg.gameObject.SetActive(toggle);
        _bgmOffToggleImg.gameObject.SetActive(!toggle);
        _bgmGreenBar.gameObject.SetActive(toggle);
    }

    public void SetSfxToggle(bool toggle)
    {
        _sfxOnImg.gameObject.SetActive(toggle);
        _sfxOffImg.gameObject.SetActive(!toggle);
        _sfxOnToggleImg.gameObject.SetActive(toggle);
        _sfxOffToggleImg.gameObject.SetActive(!toggle);
        _sfxGreenBar.gameObject.SetActive(toggle);
    }
    #endregion
}
