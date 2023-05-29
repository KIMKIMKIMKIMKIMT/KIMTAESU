using System.Collections.Generic;
using UI;
using UnityEngine;

// 굿즈 아이템 전용
public class UI_AcquirePopup : UI_Popup
{
    [SerializeField] private Transform UIAcquireItemRoot;
    
    private readonly List<UI_AcquireItem> _uiAcquireItems = new();
    private List<(int, double)> _acquireGoodsList;

    public void SetItems(List<(int, double)> acquireGoodsList)
    {
        if (acquireGoodsList.Count <= 0)
        {
            ClosePopup();
            return;
        }

        if (_uiAcquireItems.Count <= 0)
            UIAcquireItemRoot.DestroyInChildren();
        
        _uiAcquireItems.ForEach(item => item.gameObject.SetActive(false));

        var uiAcquireItems = new List<UI_AcquireItem>();

        for (int i = 0; i < acquireGoodsList.Count; i++)
        {
            UI_AcquireItem uiAcquireItem;

            if (_uiAcquireItems.Count > i)
                uiAcquireItem = _uiAcquireItems[i];
            else
            {
                uiAcquireItem = Managers.UI.MakeSubItem<UI_AcquireItem>(UIAcquireItemRoot);
                uiAcquireItems.Add(uiAcquireItem);
            }
            
            uiAcquireItem.gameObject.SetActive(true);
            uiAcquireItem.Init(acquireGoodsList[i].Item1, acquireGoodsList[i].Item2);
        }
        
        uiAcquireItems.ForEach(uiAcquireItem => _uiAcquireItems.Add(uiAcquireItem));
    }
}