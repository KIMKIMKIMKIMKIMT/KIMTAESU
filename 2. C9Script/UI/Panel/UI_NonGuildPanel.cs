using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_NonGuildPanel : UI_Panel
{
    [Serializable]
    private record Tab
    {
        public UI_Panel UIPanel;
        public GameObject SelectObj;
        public Button TabButton;

        public void Open()
        {
            UIPanel.Open();
            SelectObj.SetActive(true);
        }

        public void Close()
        {
            UIPanel.Close();
            SelectObj.SetActive(false);
        }
    }

    [SerializeField] private Tab[] Tabs;

    private Tab _currentTab;

    private Tab CurrentTab
    {
        set
        {
            _currentTab?.Close();

            _currentTab = value;
            _currentTab.Open();
        }
    }

    private void Start()
    {
        foreach (var tab in Tabs)
        {
            tab.Close();
            tab.TabButton.BindEvent(() => CurrentTab = tab);
        }

        if (_currentTab == null && Tabs.Length > 0)
            CurrentTab = Tabs[0];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI();
        }
    }
}