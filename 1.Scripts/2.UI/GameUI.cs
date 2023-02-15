using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameUI : MonoBehaviour
{
    #region Fields
    [SerializeField] private SkillSelection _skillSelection;
    [SerializeField] private InGameResultReward _inGameResultReward;
    [SerializeField] private Pause _pause;
    [SerializeField] private BonusBoxPopup _bonusBoxPopup;
    [SerializeField] private BoomEffet _boomEffect;
    [SerializeField] private GameObject _warning;

    [SerializeField] private Image _expBar;
    [SerializeField] private Image _bossHpImg;
    [SerializeField] private Image _bossHpBar; 
    [SerializeField] private Text _playTimeTxt;
    [SerializeField] private Text _levelTxt;
    [SerializeField] private Text _killCnt;
    [SerializeField] private Text _getGold;

    private float _fillAmount;
    private float _bossHpFillAmount;
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (PlayerDataMgr.Instance.PlayerData.BgmToggle)
        {
            SoundMgr.Instance.PlayBGMPlayer(eAUDIOCLIP_BGM.Game);
        }
    }
    private void Update()
    {
        int minit = (int)BattleMgr.Instance.PlayTime / 60;
        int sec = (int)BattleMgr.Instance.PlayTime % 60;
        _playTimeTxt.text = string.Format("{0:00}:{1:00}", minit, sec);
        _killCnt.text = BattleMgr.Instance.KillCnt.ToString();
        _getGold.text = BattleMgr.Instance.StagaGold.ToString();

        _expBar.fillAmount = Mathf.Lerp(_expBar.fillAmount, _fillAmount, 0.7f);
        _bossHpBar.fillAmount = Mathf.Lerp(_bossHpBar.fillAmount, _bossHpFillAmount, 0.7f);

    }
    #endregion

    #region Public Methods
    public void GameOver(bool clear)
    {
        PopupMgr.Instance.AddPopup(_inGameResultReward.gameObject);
        _inGameResultReward.SetResult(clear);
    }
    public void OnClickPause()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PopupMgr.Instance.AddPopup(_pause.gameObject);
    }
    public void ShowBonusPopup()
    {
        PopupMgr.Instance.AddPopup(_bonusBoxPopup.gameObject);
    }
    public void SetFillAmount(float fillAmount)
    {
        _fillAmount = fillAmount;
    }

    public void InitFillAmount()
    {
        _fillAmount = 0;
        _expBar.fillAmount = 0;
    }

    public void InitBossHpFillAmount(bool bossRaid)
    {
        _bossHpImg.gameObject.SetActive(bossRaid);
        _bossHpFillAmount = 1;
        _bossHpBar.fillAmount = 1;
    }
    public void SetBossHpFillAmount(float fillAmount)
    {
        _bossHpFillAmount = fillAmount;
    }
    public void SetSkillSelection(bool isOn)
    {
        _skillSelection.gameObject.SetActive(isOn);
    }
    
    public void SetLevel(int level)
    {
        _levelTxt.text = level.ToString();
    }
    public void Warning()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.Warnning, true);
        }
        _warning.SetActive(true);
        Invoke("WarningActiveOff", 2);
    }
    public void WarningActiveOff()
    {
        _warning.SetActive(false);
    }
    public void ShowBoomEffect()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.ItemBoom);
        }
        _boomEffect.gameObject.SetActive(true);
    }
    #endregion
}
