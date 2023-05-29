using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class UI_CollectionInfoPanel : UI_Panel
{
    [SerializeField] private Transform UICollectionItemRoot;

    private readonly List<UI_CollectionItem> _uiCollectionItems = new();

    private void Start()
    {
        UICollectionItemRoot.DestroyInChildren();

        foreach (var collectionChart in ChartManager.CollectionCharts.Values)
        {
            UI_CollectionItem uiItem = Managers.UI.MakeSubItem<UI_CollectionItem>(UICollectionItemRoot);
            
            uiItem.Init(collectionChart);
            _uiCollectionItems.Add(uiItem);
        }
    }

    private void OnDisable()
    {
        if (_uiCollectionItems.Find(uiCollectionItem => uiCollectionItem.IsSave) != null)
        {
            GameDataManager.CollectionGameData.SaveGameData();
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI();
        }
    }

    public override void Open()
    {
        base.Open();
        
        _uiCollectionItems.ForEach(uiCollectionItem => uiCollectionItem.Refresh());
    }
}