using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eSCROLL_TYPE
{
    Weapon,
    Equip,

    Max
}
public class UpgradeScroll : MonoBehaviour
{
    #region Fields
    [SerializeField] private Image _scrollImg;
    [SerializeField] private Text _cntTxt;
    #endregion

    #region Public Methods
    public void SetData(eSCROLL_TYPE type, int index)
    {
        _scrollImg.sprite = SpriteMgr.Instance.GetScrollIcon(type);
        _cntTxt.text = "x" + index;
    }
    #endregion
}
