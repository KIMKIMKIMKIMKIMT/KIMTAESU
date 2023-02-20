using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    #region Fields
    [SerializeField] private Image[] _currentAttackSkillsImg;
    [SerializeField] private Image[] _currentBuffSkillsImg;
    [SerializeField] private Text[] _currentAttackSkillLevelTxt;
    [SerializeField] private Text[] _currentBuffSkillLevelTxt;

    [SerializeField] private Image _bgmOnToggle;
    [SerializeField] private Image _bgmOffToggle;
    [SerializeField] private Image _sfxOnToggle;
    [SerializeField] private Image _sfxOffToggle;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        InGameUIMgr.Instance._joyStick.SetActive(false);
        JoyStick.Instance.StopDrag();
        SetCurrentSkillData();
        SetBgmToggle(PlayerDataMgr.Instance.PlayerData.BgmToggle);
        SetSfxToggle(PlayerDataMgr.Instance.PlayerData.SfxToggle);
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        InGameUIMgr.Instance._joyStick.SetActive(true);
        Time.timeScale = 1;
    }
    #endregion

    #region Public Methods
    public void OnClickHome()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PopupMgr.Instance.ShowOkCancelPopup("홈으로 돌아가기", "홈으로 돌아가면 수익을 얻지 못하게 됩니다. 홈으로 돌아갈까요?", () =>
        {
            Time.timeScale = 1;
            LoadSceneMgr.Instance.LoadSceneAsync(eSCENESTATE.Main);
            OnClickQuit();
        });
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
            SoundMgr.Instance.PlayBGMPlayer(eAUDIOCLIP_BGM.Game);
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
        else
        {
            SoundMgr.Instance.StopEffectSound();
        }
    }
    public void OnClickQuit()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PopupMgr.Instance.RemovePopup();
    }

    public void SetCurrentSkillData()
    {
        for (int i = 0; i < BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList.Count; i++)
        {
            _currentAttackSkillsImg[i].sprite = SpriteMgr.Instance._atkSkillkImg[(int)BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList[i]];
        }
        for (int i = 0; i < BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList.Count; i++)
        {
            _currentAttackSkillLevelTxt[i].text = "Lv." + BattleMgr.Instance.Player.SkillController.CurrentAttackSkillTable[BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList[i]].Level;
        }
        for (int i = 0; i < BattleMgr.Instance.Player.SkillController.CurrentBuffSkillList.Count; i++)
        {
            _currentBuffSkillsImg[i].sprite = SpriteMgr.Instance._buffSkillkImg[(int)BattleMgr.Instance.Player.SkillController.CurrentBuffSkillList[i]];
        }
        for (int i = 0; i < BattleMgr.Instance.Player.SkillController.CurrentBuffSkillList.Count; i++)
        {
            _currentBuffSkillLevelTxt[i].text = "Lv." + BattleMgr.Instance.Player.SkillController.CurrentBuffSkillTable[BattleMgr.Instance.Player.SkillController.CurrentBuffSkillList[i]].Level;
        }
    }
    public void SetBgmToggle(bool toggle)
    {
        _bgmOnToggle.gameObject.SetActive(toggle);
        _bgmOffToggle.gameObject.SetActive(!toggle);
    }
    public void SetSfxToggle(bool toggle)
    {
        _sfxOnToggle.gameObject.SetActive(toggle);
        _sfxOffToggle.gameObject.SetActive(!toggle);
    }
    #endregion
}
