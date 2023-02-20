using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class InvenUI : MonoBehaviour
{
    public enum eEQUIP_TYPE
    {
        Atk,
        Hp
    }

    #region Fields
    [SerializeField] private EquipmentPool _pool;
    [SerializeField] private ScrollPool _scrollPool;

    [SerializeField] private Transform _equipGrid;
    [SerializeField] private Transform _scrollGrid;

    [Header("Tank Profile")]
    [SerializeField] private GameObject[] _tanks;
    [SerializeField] private Text _atkTxt;
    [SerializeField] private Text _hpTxt;
    [SerializeField] private Text _atkLevTxt;
    [SerializeField] private Text _hpLevTxt;
    [SerializeField] private Text _weaponLvTxt;
    [SerializeField] private Text _equipLvTxt;
    [SerializeField] private Button _weaponBtn;
    [SerializeField] private Button _equipBtn;
    [SerializeField] private Image _atkEquipImg;
    [SerializeField] private Image _hpEquipImg;
    [SerializeField] private Image _atkWeaponPannelImg;
    [SerializeField] private Image _hpEquipPannelImg;
    [SerializeField] private Image _weaponLevelUpImg;
    [SerializeField] private Image _equipLevelUpImg;
    [SerializeField] private Sprite _defaultSpr;
    [SerializeField] private Sprite _defaultPannelSpr;
    private eATKWEAPON _weaponKey;
    private eHPEQUIP _hpEquipKey;

    private int _currentWeaponIndex;
    private int _currentEquipIndex;
    private int _currentWeaponLevel;
    private int _currentEquipLevel;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        TankData tank = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData();
        SetCurrentEquip(tank.WeaponType, tank.CurrentWeaponIndex, tank.CurrentWeaponLevel);
        SetCurrentEquip(tank.EquipType, tank.CurrentEquipIndex, tank.CurrentEquipLevel);

    }
    private void Start()
    {
        SetTank(PlayerDataMgr.Instance.PlayerData.CurTank);
        UIMgr.Instance.Refresh();
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
        TankData tankData = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData();
        SetCurrentEquip(tankData.WeaponType, tankData.CurrentWeaponIndex, tankData.CurrentWeaponLevel);
        SetCurrentEquip(tankData.EquipType, tankData.CurrentEquipIndex, tankData.CurrentEquipLevel);
        ShowEquip();
        PlayerDataMgr.Instance.SaveData();
        UIMgr.Instance.Refresh();
    }

    public void SetCurrentEquip(eATKWEAPON weapon, int index, int level)
    {
        if (weapon == eATKWEAPON.None)
        {
            _weaponKey = eATKWEAPON.None;
            _currentWeaponIndex = -1;
            _weaponBtn.interactable = false;
            _atkEquipImg.sprite = _defaultSpr;
            _atkWeaponPannelImg.sprite = _defaultPannelSpr;
            _weaponLvTxt.text = "";
            EquipmentLevelUpPosible(eTYPE.Weapon, level);
            PlayerDataMgr.Instance.SetTankToEquip(_weaponKey, _currentWeaponIndex);
            PlayerDataMgr.Instance.SaveData();
            return;
        }
        _weaponKey = weapon;
        _currentWeaponIndex = index;
        _weaponBtn.interactable = true;
        _atkEquipImg.sprite = SpriteMgr.Instance.GetAtkWeaponIcon(weapon);
        _atkWeaponPannelImg.sprite = SpriteMgr.Instance.GetEquipmentPannel(GameDataMgr.Instance.AtkWeaponData.AtkWeaponDic[_weaponKey].Grade);
        _weaponLvTxt.text = "Lv." + level;

        EquipmentLevelUpPosible(eTYPE.Weapon, level);
        PlayerDataMgr.Instance.SetTankToEquip(_weaponKey, _currentWeaponIndex);
        PlayerDataMgr.Instance.SaveData();
        UIMgr.Instance.Refresh();
    }

    public void SetCurrentEquip(eHPEQUIP equip, int index, int level)
    {
        if (equip == eHPEQUIP.None)
        {
            _hpEquipKey = eHPEQUIP.None;
            _currentEquipIndex = -1;
            _equipBtn.interactable = false;
            _hpEquipImg.sprite = _defaultSpr;
            _hpEquipPannelImg.sprite = _defaultPannelSpr;
            _equipLvTxt.text = "";
            EquipmentLevelUpPosible(eTYPE.Equip, level);
            PlayerDataMgr.Instance.SetTankToEquip(_hpEquipKey, _currentEquipIndex);
            PlayerDataMgr.Instance.SaveData();
            return;
        }
        _hpEquipKey = equip;
        _currentEquipIndex = index;
        _equipBtn.interactable = true;
        _hpEquipImg.sprite = SpriteMgr.Instance.GetHpEquipmentIcon(equip);
        _hpEquipPannelImg.sprite = SpriteMgr.Instance.GetEquipmentPannel(GameDataMgr.Instance.HpEquipData.HpEquipmentDic[_hpEquipKey].Grade);
        _equipLvTxt.text = "Lv." + level;
        EquipmentLevelUpPosible(eTYPE.Equip, level);
        PlayerDataMgr.Instance.SetTankToEquip(_hpEquipKey, _currentEquipIndex);
        PlayerDataMgr.Instance.SaveData();
        UIMgr.Instance.Refresh();
    }
    
    public void EquipmentLevelUpPosible(eTYPE type, int level)
    {
        switch (type)
        {
            case eTYPE.Weapon:
                if (_weaponKey != eATKWEAPON.None && PlayerDataMgr.Instance.PlayerData.WeaponScroll >= level)
                {
                    _weaponLevelUpImg.gameObject.SetActive(true);
                }
                else
                {
                    _weaponLevelUpImg.gameObject.SetActive(false);
                }
                break;
            case eTYPE.Equip:
                if (_hpEquipKey != eHPEQUIP.None && PlayerDataMgr.Instance.PlayerData.EquipScroll >= level)
                {
                    _equipLevelUpImg.gameObject.SetActive(true);
                }
                else
                {
                    _equipLevelUpImg.gameObject.SetActive(false);
                }
                break;
            
        }
    }

    public void ShowEquip()
    {   
        _pool.AllObjActiveOff();

        PlayerData data = PlayerDataMgr.Instance.PlayerData;
        
        for (int i = data.WeaponLevel.Length - 1; i >= 0; i--)
        {
            if (data.WeaponLevel[i] != null)
            {
                for (int j = 0; j < data.WeaponLevel[i].Count; j++)
                {
                    GetItem((eATKWEAPON)i, data.WeaponLevel[i][j], j);
                }
            }
        }

        for (int i = data.EquipLevel.Length - 1; i >= 0; i--)
        {
            if (data.EquipLevel[i] != null)
            {
                for (int j = 0; j < data.EquipLevel[i].Count; j++)
                {
                    GetItem((eHPEQUIP)i, data.EquipLevel[i][j], j);
                }
            }
        }

        _scrollPool.AllObjActiveOff();

        if (data.WeaponScroll > 0)
        {
            GetScroll(eSCROLL_TYPE.Weapon, data.WeaponScroll);
        }
        if (data.EquipScroll > 0)
        {
            GetScroll(eSCROLL_TYPE.Equip, data.EquipScroll);
        }
    }

    public void OnClickShowCurrentWeapon()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        if (_weaponKey == eATKWEAPON.None)
        {
            return;
        }
        UIMgr.Instance._ui_Popup.ShowEquipmentStatusPopup(_weaponKey, _currentWeaponIndex, true);
    }

    public void OnClickCurrentEquip()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        if (_hpEquipKey == eHPEQUIP.None)
        {
            return;
        }
        UIMgr.Instance._ui_Popup.ShowEquipmentStatusPopup(_hpEquipKey, _currentEquipIndex, true);
    }

    public void OnClickFusionPopup()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        UIMgr.Instance._ui_Popup.ShowFusionPopup();
    }

    public void GetItem(eATKWEAPON weapon, int level, int index)
    {
        Equipment obj1 = _pool.GetFromPool(_equipGrid, 0);
        obj1.SetData(weapon, level, index, GameDataMgr.Instance.AtkWeaponData.AtkWeaponDic[weapon].Grade);
    }
    public void GetItem(eHPEQUIP equip, int level, int index)
    {
        Equipment obj2 = _pool.GetFromPool(_equipGrid, 0);
        obj2.SetData(equip, level, index, GameDataMgr.Instance.HpEquipData.HpEquipmentDic[equip].Grade);
    }
    public void GetScroll(eSCROLL_TYPE type, int index)
    {
        UpgradeScroll scroll = _scrollPool.GetFromPool(_scrollGrid, 0);
        scroll.SetData(type, index);
    }
    public void Refresh()
    {
        TankData tankData = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData();
        _atkTxt.text = tankData.GetTankAtk().ToString();

        _hpTxt.text = tankData.GetTankHp().ToString();

        _atkLevTxt.text = "Lv." + tankData.AtkLevel.ToString();
        _hpLevTxt.text = "Lv." + tankData.HpLevel.ToString();

        if (_weaponKey == eATKWEAPON.None)
        {
            _weaponLvTxt.text = string.Empty;
            _weaponLevelUpImg.gameObject.SetActive(false);
        }
        else
        {
            int weaponlevel = PlayerDataMgr.Instance.PlayerData.GetEquipLevel(_weaponKey, _currentWeaponIndex);
            EquipmentLevelUpPosible(eTYPE.Weapon, weaponlevel);
            _weaponLvTxt.text = "Lv." + weaponlevel;
        }

        if (_hpEquipKey == eHPEQUIP.None)
        {
            _equipLvTxt.text = string.Empty;
            _equipLevelUpImg.gameObject.SetActive(false);
        }
        else
        {
            int equipLevel = PlayerDataMgr.Instance.PlayerData.GetEquipLevel(_hpEquipKey, _currentEquipIndex);
            EquipmentLevelUpPosible(eTYPE.Equip, equipLevel);
            _equipLvTxt.text = "Lv." + equipLevel;
        }
        ShowEquip();
    }
    #endregion
}
