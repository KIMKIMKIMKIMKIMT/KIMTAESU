using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject[] _tanks;
    [SerializeField] private Text _power;
    [SerializeField] private Text _hp;
    [SerializeField] private Text _powerLevTxt;
    [SerializeField] private Text _hpLevTxt;
    [SerializeField] private Text _powerUpPriceTxt;
    [SerializeField] private Text _hpUpPriceTxt;
    #endregion

    #region Unity Methods
    private void Start()
    {
        SetTank(PlayerDataMgr.Instance.PlayerData.CurTank);
    }
    #endregion

    #region Public Methods
    public void OnClickSelectTank()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        UIMgr.Instance._ui_Popup.ShowTankSelection();
    }
    public void SetTank(eTANK tank)
    {
        for (int i = 0; i < _tanks.Length; i++)
        {
            _tanks[i].SetActive(false);
        }
        _tanks[(int)tank].SetActive(true);
    }
    public void OnClickAtkUpgrade()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        if (PlayerDataMgr.Instance.PlayerData.TankInfo[PlayerDataMgr.Instance.PlayerData.CurTank].AtkLevel * 100 > PlayerDataMgr.Instance.PlayerData.Gold)
        {
            PopupMgr.Instance.ShowOkPopup("알림", "골드가 부족합니다.");
            return;
        }
        GameMgr.Instance.QuestAddCnt(eQUEST.Upgrade);
        PlayerDataMgr.Instance.PlayerData.Gold -= PlayerDataMgr.Instance.PlayerData.TankInfo[PlayerDataMgr.Instance.PlayerData.CurTank].AtkLevel * 100;
        PlayerDataMgr.Instance.PlayerData.TankInfo[PlayerDataMgr.Instance.PlayerData.CurTank].Atk += 10;
        PlayerDataMgr.Instance.PlayerData.TankInfo[PlayerDataMgr.Instance.PlayerData.CurTank].AtkLevel++;
        PlayerDataMgr.Instance.SaveData();
        UIMgr.Instance.Refresh();
    }
    public void OnClickHpUpgrade()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        if (PlayerDataMgr.Instance.PlayerData.TankInfo[PlayerDataMgr.Instance.PlayerData.CurTank].HpLevel * 100 > PlayerDataMgr.Instance.PlayerData.Gold)
        {
            PopupMgr.Instance.ShowOkPopup("알림", "골드가 부족합니다.");
            return;
        }
        GameMgr.Instance.QuestAddCnt(eQUEST.Upgrade);
        PlayerDataMgr.Instance.PlayerData.Gold -= PlayerDataMgr.Instance.PlayerData.TankInfo[PlayerDataMgr.Instance.PlayerData.CurTank].HpLevel * 100;
        PlayerDataMgr.Instance.PlayerData.TankInfo[PlayerDataMgr.Instance.PlayerData.CurTank].Hp += 10;
        PlayerDataMgr.Instance.PlayerData.TankInfo[PlayerDataMgr.Instance.PlayerData.CurTank].HpLevel++;
        PlayerDataMgr.Instance.SaveData();
        UIMgr.Instance.Refresh();

    }

    public void Refresh()
    {
        _power.text = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().GetTankAtk().ToString();
        _hp.text = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().GetTankHp().ToString();
        _powerLevTxt.text = "Lv." + PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().AtkLevel.ToString();
        _hpLevTxt.text = "Lv." + PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().HpLevel.ToString();
        _powerUpPriceTxt.text = (PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().AtkLevel * 100).ToString();
        _hpUpPriceTxt.text = (PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().HpLevel * 100).ToString();

    }
    #endregion
}
