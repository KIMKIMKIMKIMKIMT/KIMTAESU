using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteMgr : DontDestroy<SpriteMgr>
{
    #region Fields
    public Sprite[] _atkSkillkImg;
    public Sprite[] _buffSkillkImg;
    private AssetIcon _assetIcon;
    private AtkWeaponIcon _atkWeaponIcon;
    private HpEquipmentIcon _hpEquipmentIcon;
    private EquipmentPannel _equipmentPannel;
    private ScrollIcon _scrollIcon;
    private ResultRewardIcon _resultRewardIcon;
    private ShopBoxSprite _shopBoxSprite;

    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        _assetIcon = GetComponentInChildren<AssetIcon>();
        _atkWeaponIcon = GetComponentInChildren<AtkWeaponIcon>();
        _hpEquipmentIcon = GetComponentInChildren<HpEquipmentIcon>();
        _equipmentPannel = GetComponentInChildren<EquipmentPannel>();
        _scrollIcon = GetComponentInChildren<ScrollIcon>();
        _resultRewardIcon = GetComponentInChildren<ResultRewardIcon>();
        _shopBoxSprite = GetComponentInChildren<ShopBoxSprite>();
    }
    #endregion

    #region Public Methods
    public Sprite GetAssetIcon(eASSET_TYPE asset)
    {
        return _assetIcon._icon[(int)asset];
    }
    public Sprite GetAtkWeaponIcon(eATKWEAPON type)
    {
        return _atkWeaponIcon.Icons[(int)type];
    }
    public Sprite GetHpEquipmentIcon(eHPEQUIP type)
    {
        return _hpEquipmentIcon._icons[(int)type];
    }
    public Sprite GetEquipmentPannel(eEQUIPGRADE_TYPE type)
    {
        return _equipmentPannel.Pannels[(int)type];
    }
    public Sprite GetScrollIcon(eSCROLL_TYPE type)
    {
        return _scrollIcon._icons[(int)type];
    }
    public Sprite GetResultRewardIcon(eRESULTREWARD_TYPE type)
    {
        return _resultRewardIcon._icon[(int)type];
    }
    public Sprite[] GetShopBoxSprite(eBOX_TYPE type)
    {
        switch (type)
        {
            case eBOX_TYPE.BronzeBox:
                return _shopBoxSprite.BronzeBox;
            case eBOX_TYPE.GoldBox:
                return _shopBoxSprite.GoldBox;
            default:
                return null;
        }
    }
    #endregion
}
