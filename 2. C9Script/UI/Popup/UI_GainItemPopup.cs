using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_GainItemPopup : UI_Popup
{
    [SerializeField] private Transform UIGainItemRoot;
    [SerializeField] private Button CloseButton;

    private readonly List<UI_GainItem> _uiGainItems = new();

    public override bool isTop => true;

    private void Start()
    {
        CloseButton.BindEvent(ClosePopup);
    }
    
    public void SetItem(Dictionary<(ItemType, int), double> gainItemDatas)
    {
        StopAllCoroutines();
        
        transform.DOScaleY(0, 0f).onComplete += () =>
        {
            transform.DOScaleY(1, 0.5f);
        };
        
        if (_uiGainItems.Count <= 0)
            UIGainItemRoot.DestroyInChildren();
        
        _uiGainItems.ForEach(uiGainItem => uiGainItem.gameObject.SetActive(false));

        var uiGainItems = new List<UI_GainItem>();

        int index = 0;

        foreach (var gainItemData in gainItemDatas)
        {
            UI_GainItem uiGainItem;

            if (_uiGainItems.Count > index)
                uiGainItem = _uiGainItems[index++];
            else
            {
                uiGainItem = Managers.UI.MakeSubItem<UI_GainItem>(UIGainItemRoot);
                uiGainItems.Add(uiGainItem);
            }

            var itemType = gainItemData.Key.Item1;
            int itemId = gainItemData.Key.Item2;
            double itemValue = gainItemData.Value;

            uiGainItem.gameObject.SetActive(true);
            uiGainItem.Init(itemType, itemId, itemValue);
        }
        
        uiGainItems.ForEach(uiGainItem => _uiGainItems.Add(uiGainItem));

        StartCoroutine(CoCloseTimer());
    }

    private IEnumerator CoCloseTimer()
    {
        yield return new WaitForSeconds(1.5f);
        
        ClosePopup();
    }
}