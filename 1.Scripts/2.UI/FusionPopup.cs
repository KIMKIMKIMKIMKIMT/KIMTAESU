using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FusionPopup : MonoBehaviour
{
    #region Fields
    private FusionObjPool _pool;
    private FusionDragObjPool _dragObjPool;
    [SerializeField] private Transform _nextFusionSlot;


    [SerializeField] private Transform _selectPanel;
    

    [SerializeField] public FusionSlot _fusionSlot;
    [SerializeField] public PartSlot[] _partSlots;
    public FusionObj SelectObj { get; private set; }
    public FusionDragObj DragObj { get; private set; }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _pool = GetComponentInChildren<FusionObjPool>();
        _dragObjPool = GetComponentInChildren<FusionDragObjPool>();
    }

    private void OnEnable()
    {
        ShowEquip();
    }
    private void OnDisable()
    {
        UIMgr.Instance.MainUI.InvenUI.Refresh();
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

    public void ShowEquip()
    {
        _pool.AllObjActiveOff();
        _dragObjPool.AllObjActiveOff();

        
        if (_fusionSlot.Obj != null)
        {
            SetFusionObj(_fusionSlot.Obj);

            for (int i = 0; i < _partSlots.Length; i++)
            {
                if (_partSlots[i].Obj != null)
                {
                    _partSlots[i].SetObj(_partSlots[i].Obj);
                    _partSlots[i].Obj.transform.SetParent(_selectPanel);
                    _partSlots[i].Obj.transform.position = _partSlots[i].transform.position;
                }
            }
        }
        else
        {
            for (int i = 0;  i < _partSlots.Length; i++)
            {

            }
        }


        PlayerData data = PlayerDataMgr.Instance.PlayerData;

        //깊은복사
        for (int i = data.WeaponLevel.Length - 1; i >= 0; i--)
        {
            for (int j = 0; j < data.WeaponLevel[i].Count; j++)
            {
                GetItem((eATKWEAPON)i, (data.WeaponLevel[i][j]), j);
            }
        }

        for (int i = data.EquipLevel.Length - 1; i >= 0; i--)
        {
            for (int j = 0; j < data.EquipLevel[i].Count; j++)
            {
                GetItem((eHPEQUIP)i, (data.EquipLevel[i][j]), j);
            }
        }

    }

    public void ShowFusionEquip()
    {
        _pool.AllObjActiveOff();

        PlayerData data = PlayerDataMgr.Instance.PlayerData;

        switch (_fusionSlot.Obj.Type)
        {
            case eTYPE.Weapon:
                for (int i = 0; i < data.WeaponLevel[(int)_fusionSlot.Obj.WeaponKey].Count; i++)
                {
                    GetItem(_fusionSlot.Obj.WeaponKey, (data.WeaponLevel[(int)_fusionSlot.Obj.WeaponKey][i]), i);
                }

                for (int i = data.WeaponLevel.Length - 1; i >= 0; i--)
                {
                    if ((eATKWEAPON)i != _fusionSlot.Obj.WeaponKey)
                    {
                        for (int j = 0; j < data.WeaponLevel[i].Count; j++)
                        {
                            GetItem((eATKWEAPON)i, (data.WeaponLevel[i][j]), j);
                        }
                    }
                }
                for (int i = data.EquipLevel.Length - 1; i >= 0; i--)
                {
                    for (int j = 0; j < data.EquipLevel[i].Count; j++)
                    {
                        GetItem((eHPEQUIP)i, (data.EquipLevel[i][j]), j);
                    }
                }
                break;
            case eTYPE.Equip:
                for (int i = 0; i < data.EquipLevel[(int)_fusionSlot.Obj.EquipKey].Count; i++)
                {
                    GetItem(_fusionSlot.Obj.EquipKey, (data.EquipLevel[(int)_fusionSlot.Obj.EquipKey][i]), i);
                }
                for (int i = data.WeaponLevel.Length - 1; i >= 0; i--)
                {
                    for (int j = 0; j < data.WeaponLevel[i].Count; j++)
                    {
                        GetItem((eATKWEAPON)i, (data.WeaponLevel[i][j]), j);
                    }
                }
                for (int i = data.EquipLevel.Length - 1; i >= 0; i--)
                {
                    if ((eHPEQUIP)i != _fusionSlot.Obj.EquipKey)
                    {
                        for (int j = 0; j < data.EquipLevel[i].Count; j++)
                        {
                            GetItem((eHPEQUIP)i, (data.EquipLevel[i][j]), j);
                        }
                    }
                    
                }
                break;
        }
        

        

        
    }

public void GetItem(eATKWEAPON weapon, int level, int index)
    {
        FusionObj obj = _pool.GetFromPool(_pool.transform, 0);
        obj.SetData(weapon, level, index, GameDataMgr.Instance.AtkWeaponData.AtkWeaponDic[weapon].Grade);
        obj.SetCheckObj();
    }
    public void GetItem(eHPEQUIP equip, int level, int index)
    {
        FusionObj obj = _pool.GetFromPool(_pool.transform, 0);
        obj.SetData(equip, level, index, GameDataMgr.Instance.HpEquipData.HpEquipmentDic[equip].Grade);
        obj.SetCheckObj();
    }

    public void SetSelectObj(FusionObj obj, PointerEventData data)
    {
        SelectObj = obj;
        if (SelectObj == null)
        {
            return;
        }
        switch (obj.Type)
        {
            case eTYPE.Weapon:
                FusionDragObj weaponObj = _dragObjPool.GetFromPool(_dragObjPool.transform);
                weaponObj.transform.position = data.position;
                weaponObj.SetData(obj.WeaponKey, obj.Level, obj.Index, GameDataMgr.Instance.AtkWeaponData.AtkWeaponDic[obj.WeaponKey].Grade);
                DragObj = weaponObj;
                break;
            case eTYPE.Equip:
                FusionDragObj equipObj = _dragObjPool.GetFromPool(_dragObjPool.transform);
                equipObj.transform.position = data.position;
                equipObj.SetData(obj.EquipKey, obj.Level, obj.Index, GameDataMgr.Instance.HpEquipData.HpEquipmentDic[obj.EquipKey].Grade);
                DragObj = equipObj;
                break;
        }
    }

    public void ReturnTransform(FusionObj obj)
    {
        obj.transform.SetParent(_pool.transform);
    }
    public void SetFusionObj(FusionDragObj obj)
    {
        switch (obj.Type)
        {
            case eTYPE.Weapon:
                FusionPartClear();
                if ((eATKWEAPON)(int)obj.WeaponKey + 5 >= eATKWEAPON.Max)
                {
                    return;
                }
                
                FusionDragObj fusionWeaponObj = _dragObjPool.GetFromPool(_selectPanel);
                fusionWeaponObj.transform.position = _fusionSlot.transform.position;
                fusionWeaponObj.SetData(obj.WeaponKey, obj.Level, obj.Index, GameDataMgr.Instance.AtkWeaponData.AtkWeaponDic[obj.WeaponKey].Grade);
                _fusionSlot.SetObj(fusionWeaponObj);

                FusionDragObj nextWeaponObj = _dragObjPool.GetFromPool(_dragObjPool.transform, 0);
                nextWeaponObj.transform.position = _nextFusionSlot.position;
                nextWeaponObj.SetData((eATKWEAPON)((int)obj.WeaponKey) + 5, obj.Level, obj.Index, GameDataMgr.Instance.AtkWeaponData.AtkWeaponDic[(eATKWEAPON)((int)obj.WeaponKey) + 5].Grade);

                _partSlots[0].ColliderSet(true);
                
                SelectObj.SetCheckObj();

                ShowFusionEquip();
                break;
            case eTYPE.Equip:
                FusionPartClear();
                if ((eHPEQUIP)(int)obj.EquipKey + 5 >= eHPEQUIP.Max)
                {
                    return;
                }
                FusionDragObj fusionEquipObj = _dragObjPool.GetFromPool(_selectPanel);
                fusionEquipObj.transform.position = _fusionSlot.transform.position;
                fusionEquipObj.SetData(obj.EquipKey, obj.Level, obj.Index, GameDataMgr.Instance.HpEquipData.HpEquipmentDic[obj.EquipKey].Grade);
                _fusionSlot.SetObj(fusionEquipObj);

                FusionDragObj nextEquipObj = _dragObjPool.GetFromPool(_dragObjPool.transform, 0);
                nextEquipObj.transform.position = _nextFusionSlot.position;
                nextEquipObj.SetData((eHPEQUIP)((int)obj.EquipKey) + 5, obj.Level, obj.Index, GameDataMgr.Instance.HpEquipData.HpEquipmentDic[(eHPEQUIP)((int)obj.EquipKey) + 5].Grade);

                _partSlots[0].ColliderSet(true);

                SelectObj.SetCheckObj();

                ShowFusionEquip();
                break;
        }
    }
    public void SetPartObj(FusionDragObj obj)
    {
        if (_partSlots[0].Collider == DragObj.LastHitCollider)
        {
            switch (obj.Type)
            {
                case eTYPE.Weapon:
                    FusionDragObj partWeaponObj = _dragObjPool.GetFromPool(_selectPanel);
                    partWeaponObj.transform.position = _partSlots[0].transform.position;
                    partWeaponObj.SetData(obj.WeaponKey, obj.Level, obj.Index, GameDataMgr.Instance.AtkWeaponData.AtkWeaponDic[obj.WeaponKey].Grade);
                    _partSlots[0].SetObj(partWeaponObj);
                    SelectObj.SetCheckPartSlot0Obj();
                    _partSlots[1].ColliderSet(true);
                    break;
                case eTYPE.Equip:
                    FusionDragObj partEquipObj = _dragObjPool.GetFromPool(_selectPanel);
                    partEquipObj.transform.position = _partSlots[0].transform.position;
                    partEquipObj.SetData(obj.EquipKey, obj.Level, obj.Index, GameDataMgr.Instance.HpEquipData.HpEquipmentDic[obj.EquipKey].Grade);
                    _partSlots[0].SetObj(partEquipObj);
                    SelectObj.SetCheckPartSlot0Obj();
                    _partSlots[1].ColliderSet(true);
                    break;
            }
            
        }
        else if (_partSlots[1].Collider == DragObj.LastHitCollider)
        {
            switch (obj.Type)
            {
                case eTYPE.Weapon:
                    FusionDragObj partWeaponObj = _dragObjPool.GetFromPool(_selectPanel);
                    partWeaponObj.transform.position = _partSlots[1].transform.position;
                    partWeaponObj.SetData(obj.WeaponKey, obj.Level, obj.Index, GameDataMgr.Instance.AtkWeaponData.AtkWeaponDic[obj.WeaponKey].Grade);
                    _partSlots[1].SetObj(partWeaponObj);
                    SelectObj.SetCheckPartSlot1Obj();
                    break;
                case eTYPE.Equip:
                    FusionDragObj partEquipObj = _dragObjPool.GetFromPool(_selectPanel);
                    partEquipObj.transform.position = _partSlots[1].transform.position;
                    partEquipObj.SetData(obj.EquipKey, obj.Level, obj.Index, GameDataMgr.Instance.HpEquipData.HpEquipmentDic[obj.EquipKey].Grade);
                    _partSlots[1].SetObj(partEquipObj);
                    SelectObj.SetCheckPartSlot1Obj();
                    break;
            }
        }
    }

    public void ClearFusionSlot()
    {
        _dragObjPool.AllObjActiveOff();
    }
    public void OnClickFusion()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        switch (_fusionSlot.Obj.Type)
        {
            case eTYPE.Weapon:
                if (_fusionSlot.Obj != null && _partSlots[0].Obj != null && _partSlots[1].Obj != null)
                {
                    if (_fusionSlot.Obj.WeaponKey == _partSlots[0].Obj.WeaponKey && _fusionSlot.Obj.WeaponKey == _partSlots[1].Obj.WeaponKey)
                    {
                        PopupMgr.Instance.ShowOkPopup("합성 완료", "합성에 성공했습니다.", () =>
                        {
                            eATKWEAPON newWeapon = (eATKWEAPON)((int)_fusionSlot.Obj.WeaponKey + 5);
                            PlayerDataMgr.Instance.PlayerData.AddEquip(newWeapon, _fusionSlot.Obj.Level);
                            PlayerDataMgr.Instance.SetFusionEquip(_fusionSlot.Obj.WeaponKey, _fusionSlot.Obj.Index, newWeapon, PlayerDataMgr.Instance.PlayerData.WeaponLevel[(int)newWeapon].Count - 1);

                            PlayerDataMgr.Instance.PlayerData.RemoveEquip(_fusionSlot.Obj.WeaponKey, _fusionSlot.Obj.Index);
                            PlayerDataMgr.Instance.PlayerData.RemoveEquip(_partSlots[0].Obj.WeaponKey, _partSlots[0].Obj.Index -1);
                            PlayerDataMgr.Instance.PlayerData.RemoveEquip(_partSlots[1].Obj.WeaponKey, _partSlots[1].Obj.Index -2);

                            FusionPartClear();
                            ShowEquip();
                        });

                    }
                    else
                    {
                        PopupMgr.Instance.ShowOkPopup("합성 실패", "합성에 필요 한 조건이 충족되지 않았습니다.", () => 
                        {
                            FusionPartClear();
                            ShowEquip(); 
                        });
                    }
                }
                else
                {
                    PopupMgr.Instance.ShowOkPopup("합성 실패", "합성에 필요 한 조건이 충족되지 않았습니다.", () => 
                    {
                        FusionPartClear();
                        ShowEquip(); 
                    });
                }
                break;
            case eTYPE.Equip:
                if (_fusionSlot.Obj != null && _partSlots[0].Obj != null && _partSlots[1].Obj != null)
                {
                    if (_fusionSlot.Obj.EquipKey == _partSlots[0].Obj.EquipKey && _fusionSlot.Obj.EquipKey == _partSlots[1].Obj.EquipKey)
                    {
                        PopupMgr.Instance.ShowOkPopup("합성 완료", "합성에 성공했습니다.", () =>
                        {
                            PlayerDataMgr.Instance.PlayerData.AddEquip((eHPEQUIP)((int)_fusionSlot.Obj.EquipKey + 5), _fusionSlot.Obj.Level);

                            PlayerDataMgr.Instance.PlayerData.RemoveEquip(_fusionSlot.Obj.EquipKey, _fusionSlot.Obj.Index);
                            PlayerDataMgr.Instance.PlayerData.RemoveEquip(_partSlots[0].Obj.EquipKey, _partSlots[0].Obj.Index -1);
                            PlayerDataMgr.Instance.PlayerData.RemoveEquip(_partSlots[1].Obj.EquipKey, _partSlots[1].Obj.Index -2);

                            FusionPartClear();
                            ShowEquip();
                        });

                    }
                    else
                    {
                        PopupMgr.Instance.ShowOkPopup("합성 실패", "합성에 필요 한 조건이 충족되지 않았습니다.", () => 
                        {
                            FusionPartClear();
                            ShowEquip(); 
                        });
                    }
                }
                else
                {
                    PopupMgr.Instance.ShowOkPopup("합성 실패", "합성에 필요 한 조건이 충족되지 않았습니다.", () => 
                    {
                        FusionPartClear();
                        ShowEquip(); 
                    });
                }
                break;
            default:
                PopupMgr.Instance.ShowOkPopup("합성 실패", "합성에 필요 한 조건이 충족되지 않았습니다.", () =>
                {
                    FusionPartClear();
                    ShowEquip();
                });
                break;
        }
    }
    public void FusionPartClear()
    {
        _fusionSlot.ClearObj();
        for (int i = 0; i < _partSlots.Length; i++)
        {
            _partSlots[i].ClearObj();
            _partSlots[i].ColliderSet(false);
        }
    }
    #endregion
}
