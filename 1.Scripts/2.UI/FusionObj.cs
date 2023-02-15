using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FusionObj : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    #region Fields
    public eATKWEAPON WeaponKey { get; private set; }
    public eHPEQUIP EquipKey { get; private set; }
    public eTYPE Type { get; private set; }
    public int Index { get; private set; }
    public int Level { get; private set; }

    private FusionPopup _fusionPopup;
    private RectTransform _rectTransform;
    

    [SerializeField] private Image _pannel;
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _CheckObjImg;
    [SerializeField] private GameObject _fusionLockImg;
    [SerializeField] private GameObject _currnetEquipCheckImg;
    [SerializeField] private Text _levelTxt;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _fusionPopup = GetComponentInParent<FusionPopup>();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Type = eTYPE.None;
        WeaponKey = eATKWEAPON.None;
        EquipKey = eHPEQUIP.None;
        SetCheckObj();
    }
    #endregion

    #region Public Methods
    public void SetData(eATKWEAPON weapon, int level, int index, eEQUIPGRADE_TYPE grade)
    {
        WeaponKey = weapon;
        _icon.sprite = SpriteMgr.Instance.GetAtkWeaponIcon(WeaponKey);
        _pannel.sprite = SpriteMgr.Instance.GetEquipmentPannel(grade);
        Level = level;
        _levelTxt.text = "Lv." + Level;
        Index = index;
        Type = eTYPE.Weapon;
        if (_fusionPopup._fusionSlot.Obj != null && _fusionPopup._fusionSlot.Obj.WeaponKey != WeaponKey)
        {
            _fusionLockImg.SetActive(true);
        }
        else
        {
            _fusionLockImg.SetActive(false);
        }

        if (PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().WeaponType == WeaponKey && PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().CurrentWeaponIndex == Index)
        {
            _currnetEquipCheckImg.SetActive(true);
        }
        else
        {
            _currnetEquipCheckImg.SetActive(false);
        }
        SetCheckObj();
    }
    public void SetData(eHPEQUIP equip, int level, int index, eEQUIPGRADE_TYPE grade)
    {
        EquipKey = equip;
        _icon.sprite = SpriteMgr.Instance.GetHpEquipmentIcon(EquipKey);
        _pannel.sprite = SpriteMgr.Instance.GetEquipmentPannel(grade);
        Level = level;
        _levelTxt.text = "Lv." + Level;
        Index = index;
        Type = eTYPE.Equip;
        
        if (_fusionPopup._fusionSlot.Obj != null && _fusionPopup._fusionSlot.Obj.EquipKey != EquipKey)
        {
            _fusionLockImg.SetActive(true);
        }
        else
        {
            _fusionLockImg.SetActive(false);
        }

        if (PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().EquipType == EquipKey && PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().CurrentEquipIndex == Index)
        {
            _currnetEquipCheckImg.SetActive(true);
        }
        else
        {
            _currnetEquipCheckImg.SetActive(false);
        }
        SetCheckObj();
    }
    public void SetCheckObj()
    {
        switch (Type)
        {
            case eTYPE.Weapon:
                if (_fusionPopup._fusionSlot.Obj != null)
                {
                    bool isCheckObj = _fusionPopup._fusionSlot.Obj.WeaponKey == WeaponKey && _fusionPopup._fusionSlot.Obj.Index == Index;
                    _CheckObjImg.SetActive(isCheckObj);
                }
                else
                {
                    _CheckObjImg.SetActive(false);
                }
                break;
            case eTYPE.Equip:
                if (_fusionPopup._fusionSlot.Obj != null)
                {
                    bool isCheckObj = _fusionPopup._fusionSlot.Obj.EquipKey == EquipKey && _fusionPopup._fusionSlot.Obj.Index == Index;
                    _CheckObjImg.SetActive(isCheckObj);
                }
                else
                {
                    _CheckObjImg.SetActive(false);
                }
                break;
        }
    }
    public void SetCheckPartSlot0Obj()
    {
        switch (Type)
        {
            case eTYPE.Weapon:
                if (_fusionPopup._partSlots[0].Obj != null)
                {
                    bool isCheckObj = _fusionPopup._partSlots[0].Obj.WeaponKey == WeaponKey && _fusionPopup._partSlots[0].Obj.Index == Index;
                    _CheckObjImg.SetActive(isCheckObj);
                }
                else
                {
                    _CheckObjImg.SetActive(false);
                }
                break;
            case eTYPE.Equip:
                if (_fusionPopup._partSlots[0].Obj != null)
                {
                    bool isCheckObj = _fusionPopup._partSlots[0].Obj.EquipKey == EquipKey && _fusionPopup._partSlots[0].Obj.Index == Index;
                    _CheckObjImg.SetActive(isCheckObj);
                }
                else
                {
                    _CheckObjImg.SetActive(false);
                }
                break;
        }
    }
    public void SetCheckPartSlot1Obj()
    {
        switch (Type)
        {
            case eTYPE.Weapon:
                if (_fusionPopup._partSlots[1].Obj != null)
                {
                    bool isCheckObj = _fusionPopup._partSlots[1].Obj.WeaponKey == WeaponKey && _fusionPopup._partSlots[1].Obj.Index == Index;
                    _CheckObjImg.SetActive(isCheckObj);
                }
                else
                {
                    _CheckObjImg.SetActive(false);
                }
                break;
            case eTYPE.Equip:
                if (_fusionPopup._partSlots[1].Obj != null)
                {
                    bool isCheckObj = _fusionPopup._partSlots[1].Obj.EquipKey == EquipKey && _fusionPopup._partSlots[1].Obj.Index == Index;
                    _CheckObjImg.SetActive(isCheckObj);
                }
                else
                {
                    _CheckObjImg.SetActive(false);
                }
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_CheckObjImg.activeInHierarchy || _fusionLockImg.activeInHierarchy)
        {
            return;
        }
        _fusionPopup.SetSelectObj(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _fusionPopup.DragObj.OnDrag(eventData);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _fusionPopup.DragObj.OnPointerUp(eventData);
    }
    #endregion
}
