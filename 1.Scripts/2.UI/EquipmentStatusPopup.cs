using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentStatusPopup : MonoBehaviour
{
    #region Fields
    [SerializeField] private Image _icon;
    [SerializeField] private Image _gradeImg;
    [SerializeField] private GameObject _scrollWeaponIcon;
    [SerializeField] private GameObject _scrollEquipIcon;
    
    [SerializeField] private Text _levelTxt;
    [SerializeField] private Text _gradeTxt;
    [SerializeField] private Text _valueTxt;
    [SerializeField] private Text _nameTxt;
    [SerializeField] private Text _descriptionTxt;
    [SerializeField] private Text _currentScroll;
    [SerializeField] private Text _needScroll;
    [SerializeField] private Button _equipBtn;
    [SerializeField] private Button _equipClearBtn;
    [SerializeField] private Button _levelUpBtn;
    

    private eATKWEAPON _weaponKey;
    private eHPEQUIP _equipKey;
    private eTYPE _type;

    private int _index;
    private int _level;
    #endregion

    #region Unity Methods
    private void OnDisable()
    {
        _equipBtn.gameObject.SetActive(false);
        _equipClearBtn.gameObject.SetActive(false);
    }
    #endregion

    #region Public Methods
    public void SetEquipmentStatus(eATKWEAPON key, int index, bool current)
    {
        _weaponKey = key;
        AtkWeaponData data = GameDataMgr.Instance.AtkWeaponData.AtkWeaponDic[_weaponKey];
        _nameTxt.text = data.Name;
        _descriptionTxt.text = data.Des;
        _icon.sprite = SpriteMgr.Instance.GetAtkWeaponIcon(data.Key);
        _index = index;
        _level = PlayerDataMgr.Instance.GetEquipToLevel(_weaponKey, _index);
        _levelTxt.text = "Lv." + _level;
        _valueTxt.text = "공격력 - " + (data.Atk + ((_level -1) * 10)).ToString();
        _gradeTxt.text = "등급 - " + data.Grade.ToString();
        _currentScroll.text = PlayerDataMgr.Instance.PlayerData.WeaponScroll.ToString();
        _needScroll.text = _level.ToString();
        _gradeImg.color = SetGradeColor(data.Grade);
        _type = eTYPE.Weapon;
        _scrollWeaponIcon.SetActive(true);
        _scrollEquipIcon.SetActive(false);
        
        if (current)
        {
            _equipClearBtn.gameObject.SetActive(true);
            _equipBtn.gameObject.SetActive(false);
        }
        else
        {
            _equipBtn.gameObject.SetActive(true);
            _equipClearBtn.gameObject.SetActive(false);
        }
        
    }
    public void SetEquipmentStatus(eHPEQUIP key, int index, bool current)
    {
        _equipKey = key;
        HpEquipmentData data = GameDataMgr.Instance.HpEquipData.HpEquipmentDic[_equipKey];
        _valueTxt.text = "체력 - " + (data.Hp + ((_level - 1) * 10)).ToString();
        _nameTxt.text = data.Name;
        _descriptionTxt.text = data.Des;
        _icon.sprite = SpriteMgr.Instance.GetHpEquipmentIcon(data.Key);
        _index = index;
        _level = PlayerDataMgr.Instance.GetEquipToLevel(_equipKey, _index);
        _levelTxt.text = "Lv." + _level;
        _gradeTxt.text = "등급 - " + data.Grade.ToString();
        _currentScroll.text = PlayerDataMgr.Instance.PlayerData.EquipScroll.ToString();
        _needScroll.text = PlayerDataMgr.Instance.GetEquipToLevel(_equipKey, _index).ToString();
        _gradeImg.color = SetGradeColor(data.Grade);
        _type = eTYPE.Equip;
        _scrollEquipIcon.SetActive(true);
        _scrollWeaponIcon.SetActive(false);

        if (current)
        {
            _equipClearBtn.gameObject.SetActive(true);
            _equipBtn.gameObject.SetActive(false);
        }
        else
        {
            _equipBtn.gameObject.SetActive(true);
            _equipClearBtn.gameObject.SetActive(false);
        }
    }
    

    public void OnClickEquip()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        
        switch (_type)
        {
            case eTYPE.Weapon:
                PlayerDataMgr.Instance.SetTankToEquip(_weaponKey, _index);
                UIMgr.Instance.MainUI.InvenUI.SetCurrentEquip(_weaponKey, _index, PlayerDataMgr.Instance.GetEquipToLevel(_weaponKey, _index));
                UIMgr.Instance.MainUI.InvenUI.ShowEquip();
                break;

            case eTYPE.Equip:
                PlayerDataMgr.Instance.SetTankToEquip(_equipKey, _index);
                UIMgr.Instance.MainUI.InvenUI.SetCurrentEquip(_equipKey, _index, PlayerDataMgr.Instance.GetEquipToLevel(_equipKey, _index));
                UIMgr.Instance.MainUI.InvenUI.ShowEquip();
                break;
        }
        OnClickQuit();
    }
    public void OnClickEquipClear()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        switch (_type)
        {
            case eTYPE.Weapon:
                PlayerDataMgr.Instance.InitTankToWeapon();
                UIMgr.Instance.MainUI.InvenUI.SetCurrentEquip(eATKWEAPON.None, -1, 0);
                UIMgr.Instance.MainUI.InvenUI.ShowEquip();
                UIMgr.Instance.Refresh();
                OnClickQuit();
                break;

            case eTYPE.Equip:
                PlayerDataMgr.Instance.InitTankToEquip();
                UIMgr.Instance.MainUI.InvenUI.SetCurrentEquip(eHPEQUIP.None, -1, 0);
                UIMgr.Instance.MainUI.InvenUI.ShowEquip();
                UIMgr.Instance.Refresh();
                OnClickQuit();
                break;
        }
    }

    public void OnClickQuit()
    {
        PopupMgr.Instance.RemovePopup();
    }
    public void OnClickLevelUp()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        switch (_type)
        {
            case eTYPE.Weapon:
                if (PlayerDataMgr.Instance.PlayerData.WeaponScroll < _level)
                {
                    PopupMgr.Instance.ShowOkPopup("알림", "무기 스크롤이 부족합니다.");
                    return;
                }
                PlayerDataMgr.Instance.PlayerData.WeaponScroll -= _level;
                PlayerDataMgr.Instance.PlayerData.EquipLevelUp(_weaponKey, _index);
                SetEquipmentStatus(_weaponKey, _index, true);
                UIMgr.Instance.MainUI.InvenUI.Refresh();
                break;
            case eTYPE.Equip:
                if (PlayerDataMgr.Instance.PlayerData.EquipScroll < _level)
                {
                    PopupMgr.Instance.ShowOkPopup("알림", "방어구 스크롤이 부족합니다.");
                    return;
                }
                PlayerDataMgr.Instance.PlayerData.EquipScroll -= _level;
                PlayerDataMgr.Instance.PlayerData.EquipLevelUp(_equipKey, _index);
                SetEquipmentStatus(_equipKey, _index, true);
                UIMgr.Instance.MainUI.InvenUI.Refresh();
                break;
        }
    }
    public Color SetGradeColor(eEQUIPGRADE_TYPE type)
    {
        switch (type)
        {
            case eEQUIPGRADE_TYPE.Normal:
                return Color.white;
            case eEQUIPGRADE_TYPE.Elite:
                return Color.yellow;
            case eEQUIPGRADE_TYPE.Unique:
                return Color.red;
            default:
                return Color.white;
        }
    }
    #endregion
}
