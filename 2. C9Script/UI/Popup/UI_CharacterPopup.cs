using UI;
using UniRx;
using UnityEngine;

public class UI_CharacterPopup : UI_Popup
{
    [SerializeField] private Tab[] Tabs;
    [SerializeField] private GameObject CollectionTabNavigationObj;

    private Tab _currentTab;

    private Tab CurrentTab
    {
        get => _currentTab;
        set
        {
            if (_currentTab == value)
                return;

            if (value.Panel == null)
                return;

            _currentTab?.Close();
            _currentTab = value;
            _currentTab.Open();
        }
    }

    private void Start()
    {
        foreach (var tab in Tabs)
            tab.Init(tabParam => CurrentTab = tabParam);

        CurrentTab = Tabs[0];

        SetGuideEvent();
    }

    private void SetGuideEvent()
    {
        if (Utils.IsAllClearGuideQuest())
            return;

        var guideComposite = new CompositeDisposable();
        
        Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigation).AddTo(guideComposite);
        SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);

        void SetNavigation(int id)
        {
            if (Utils.IsAllClearGuideQuest())
            {
                guideComposite.Clear();
                return;
            }

            CollectionTabNavigationObj.SetActive(id == 17 && !Utils.IsCompleteGuideQuest());
        }

        Managers.Game.UserData.OnChangeGuideQuestProgressValue.Subscribe(SetNavigationValue).AddTo(guideComposite);
        SetNavigationValue(Managers.Game.UserData.ProgressGuideQuestValue);
            
        void SetNavigationValue(long value)
        {
            if (Utils.IsCompleteGuideQuest())
                CollectionTabNavigationObj.SetActive(false);
        }
    }

    public override void Open()
    {
        base.Open();
        if (CurrentTab != null)
            CurrentTab.Open();
    }
}