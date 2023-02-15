using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetEquipment : MonoBehaviour
{
    #region Fields
    private eTYPE _type;
    private eATKWEAPON _weaponKey;
    private eHPEQUIP _equipKey;

    [SerializeField] private Image _pannel;
    [SerializeField] private Image _icon;
    #endregion

    #region Public Methods
    public void SetData(eATKWEAPON weapon, eEQUIPGRADE_TYPE grade)
    {
        _type = eTYPE.Weapon;
        _weaponKey = weapon;
        _icon.sprite = SpriteMgr.Instance.GetAtkWeaponIcon(_weaponKey);
        _pannel.sprite = SpriteMgr.Instance.GetEquipmentPannel(grade);
    }
    public void SetData(eHPEQUIP equip, eEQUIPGRADE_TYPE grade)
    {
        _type = eTYPE.Equip;
        _equipKey = equip;
        _icon.sprite = SpriteMgr.Instance.GetHpEquipmentIcon(_equipKey);
        _pannel.sprite = SpriteMgr.Instance.GetEquipmentPannel(grade);
    }
    #endregion
}
