using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum eTYPE
{
    None = -1,
    Weapon,
    Equip,

    Max
}

public class Equipment : MonoBehaviour
{
    #region Fields
    private eATKWEAPON _weaponKey;
    private eHPEQUIP _equipKey;
    private eTYPE _type;
    [SerializeField] private GameObject _equipCheckObj;
    [SerializeField] private Image _pannel;
    [SerializeField] private Image _icon;
    [SerializeField] private Text _levelTxt;
    
    private int _index;
    #endregion
    
    #region Unity Methods
    #endregion

    #region Public Methods
    public void SetData(eATKWEAPON weapon, int level, int index, eEQUIPGRADE_TYPE grade)
    {
        _weaponKey = weapon;
        _type = eTYPE.Weapon;
        _icon.sprite = SpriteMgr.Instance.GetAtkWeaponIcon(_weaponKey);
        _pannel.sprite = SpriteMgr.Instance.GetEquipmentPannel(grade);
        _levelTxt.text = "Lv." + level;
        _index = index;

        bool isEquip = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().WeaponType == _weaponKey && PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().CurrentWeaponIndex == _index;

        _equipCheckObj.SetActive(isEquip);
        
    }
    public void SetData(eHPEQUIP equip, int level, int index, eEQUIPGRADE_TYPE grade)
    {
        _equipKey = equip;
        _type = eTYPE.Equip;
        _icon.sprite = SpriteMgr.Instance.GetHpEquipmentIcon(_equipKey);
        _pannel.sprite = SpriteMgr.Instance.GetEquipmentPannel(grade);
        _levelTxt.text = "Lv." + level;
        _index = index;

        bool isEquip = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().EquipType == _equipKey && PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().CurrentEquipIndex == _index;

        _equipCheckObj.SetActive(isEquip);
    }

    public void OnClickStatus()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        switch (_type)
        {
            case eTYPE.Weapon:
                UIMgr.Instance._ui_Popup.ShowEquipmentStatusPopup(_weaponKey, _index, false);
                break;
            case eTYPE.Equip:
                UIMgr.Instance._ui_Popup.ShowEquipmentStatusPopup(_equipKey, _index, false);
                break;
        }
    }
    #endregion
}
