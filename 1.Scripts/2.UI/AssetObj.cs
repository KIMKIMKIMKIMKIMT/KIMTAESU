using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetObj : MonoBehaviour
{
    #region Fields
    [SerializeField] private Image _assetIcon;
    [SerializeField] private Text _assetCnt;
    #endregion

    #region Public Methods
    public void SetAssetData(Sprite sprite, int cnt)
    {
        _assetIcon.sprite = sprite;
        _assetCnt.text = cnt.ToString("N0");
    }
    #endregion
}
