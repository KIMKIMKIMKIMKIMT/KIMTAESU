using System;
using System.Collections.Generic;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_ShopProductPanel : UI_Panel
{
    [Serializable]
    public record Tab
    {
        public Button TabButton;
        public GameObject SelectObj;
        public UI_Panel UIPanel;

        public void Open()
        {
            SelectObj.SetActive(true);
            UIPanel.Open();
        }

        public void Close()
        {
            SelectObj.SetActive(false);
            UIPanel.Close();
        }
    }

    [SerializeField] private List<Tab> Tabs;

    [SerializeField] private GameObject CostumeTabNavigationObj;
    [SerializeField] private GameObject PackageTabNavigationObj;

    private readonly CompositeDisposable _guideComposite = new();

    private Tab _currentTab;

    private Tab CurrentTab
    {
        get => _currentTab;
        set
        {
            _currentTab?.Close();
            _currentTab = value;
            _currentTab.Open();
            SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);
        }
    }

    public void Awake()
    {
        Tabs.ForEach(tab =>
        {
            tab.Close();
            tab.TabButton.BindEvent(() => CurrentTab = tab);
        });
        
        SetGuideEvent();
    }

    public override void Open()
    {
        base.Open();

        if (CurrentTab == null)
            CurrentTab = Tabs[0];
        else
            CurrentTab.Open();
    }

    private void SetGuideEvent()
    {
        if (Utils.IsAllClearGuideQuest())
            return;

        Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigation).AddTo(_guideComposite);
        Managers.Game.UserData.OnChangeGuideQuestProgressValue.Subscribe(value =>
        {
            if (!Utils.IsCompleteGuideQuest())
                return;
            
            CostumeTabNavigationObj.SetActive(false);
            PackageTabNavigationObj.SetActive(false);
        }).AddTo(_guideComposite);
    }

    private void SetNavigation(int id)
    {
        if (Utils.IsAllClearGuideQuest())
        {
            _guideComposite.Clear();
            CostumeTabNavigationObj.SetActive(false);
            PackageTabNavigationObj.SetActive(false);
            return;
        }
        
        CostumeTabNavigationObj.SetActive(id == 14 && CurrentTab != null && CurrentTab.UIPanel is not UI_ShopCostumePanel);
        PackageTabNavigationObj.SetActive(id == 29 && CurrentTab != null && CurrentTab.UIPanel is not UI_ShopPackagePanel);
    }
}