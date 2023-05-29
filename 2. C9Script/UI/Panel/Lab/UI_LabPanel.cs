using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_LabPanel : UI_Panel
{
    [Serializable]
    public record Tab
    {
        public UI_Panel UIPanel;
        public GameObject OnObj;
        public GameObject OffObj;
        public Button TabButton;

        public void On()
        {
            OnObj.SetActive(true);
            OffObj.SetActive(false);
            UIPanel.Open();
        }

        public void Off()
        {
            OnObj.SetActive(false);
            OffObj.SetActive(true);
            UIPanel.Close();
        }
    }

    private Tab _currentTab;
    private Tab CurrentTab
    {
        get => _currentTab;
        set
        {
            if (_currentTab != null) 
                _currentTab.Off();

            _currentTab = value;
            _currentTab.On();
        }
    }

    [SerializeField] private Tab[] Tabs;

    private void Start()
    {
        foreach (var tab in Tabs)
        {
            tab.Off();
            tab.TabButton.BindEvent(() => CurrentTab = tab);
        }
        
        if (CurrentTab == null && Tabs.Length > 0)
            CurrentTab = Tabs[0];
    }

    public override void Open()
    {
        base.Open();
        
        if (CurrentTab != null)
            CurrentTab.On();
    }
}