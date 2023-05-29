using System;
using System.Collections.Generic;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DungeonPopup : UI_Popup
{
    [SerializeField] private TMP_Text EventTabText;

    [Serializable]
    public record Tab
    {
        public Button TabButton;
        public GameObject SelectObj;
        public UI_Panel UIPanel;
    }

    [SerializeField] private Tab[] Tabs;
    [SerializeField] private ScrollRect scrollRect;

    private Tab _currentTab;

    public Tab CurrentTab
    {
        get => _currentTab;
        set
        {
            if (_currentTab != null)
            {
                _currentTab.SelectObj.SetActive(false);
                _currentTab.UIPanel.Close();
            }

            _currentTab = value;
            _currentTab.SelectObj.SetActive(true);
            _currentTab.UIPanel.Open();
        }
    }

    private void Start()
    {
        foreach (var tab in Tabs)
            tab.TabButton.BindEvent(() => CurrentTab = tab);

        scrollRect.horizontalNormalizedPosition = 0f;

        EventTabText.text = ChartManager.GetString("Dungeon_Category_Event");
    }

    public override void Open()
    {
        base.Open();
        if (CurrentTab == null)
        {
            foreach (var tab in Tabs)
                tab.UIPanel.Close();
            
            CurrentTab = Tabs[0];
        }
        else
            CurrentTab.UIPanel.Open();
    }

    public void SetEventTab()
    {
        CurrentTab = Tabs[2];
    }
}
