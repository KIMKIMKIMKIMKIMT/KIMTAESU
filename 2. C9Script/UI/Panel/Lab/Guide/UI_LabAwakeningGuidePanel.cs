using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_LabAwakeningGuidePanel : UI_Panel
{
    [Serializable]
    public record Tab
    {
        [SerializeField] private Sprite OnButtonSprite;
        [SerializeField] private Sprite OffButtonSprite;

        [SerializeField] private Color OffButtonTextColor;

        [SerializeField] private Image ButtonImage;
        [SerializeField] private TMP_Text ButtonText;

        public Button TabButton;

        [SerializeField] private UI_Panel UIPanel;

        public void On()
        {
            ButtonImage.sprite = OnButtonSprite;
            ButtonText.color = Color.white;
            UIPanel.Open();
        }

        public void Off()
        {
            ButtonImage.sprite = OffButtonSprite;
            ButtonText.color = OffButtonTextColor;
            UIPanel.Close();
        }
    }

    [SerializeField] private Tab[] Tabs;
    [SerializeField] private Button CloseButton;

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

    private void Start()
    {
        foreach (var tab in Tabs)
        {
            tab.Off();
            tab.TabButton.BindEvent(() => CurrentTab = tab);
        }
        
        CloseButton.BindEvent(Close);

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