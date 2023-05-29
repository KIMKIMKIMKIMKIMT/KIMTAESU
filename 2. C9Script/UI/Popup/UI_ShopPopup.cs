using TMPro;
using UI;
using UniRx;
using UnityEngine;

public class UI_ShopPopup : UI_Popup
{
    [SerializeField] private Tab[] Tabs;
    [SerializeField] private TMP_Text MileageText;
    [SerializeField] private TMP_Text PvpTokenText;

    [SerializeField] private GameObject PackageTabNavigationObj;
    [SerializeField] private GameObject CashTabNavigationObj;

    private readonly CompositeDisposable _guideComposite = new();
    
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
            
            if (!Utils.IsAllClearGuideQuest())
                SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);
        }
    }

    private void Start()
    {
        foreach (var tab in Tabs)
            tab.Init(tabParam => CurrentTab = tabParam);

        CurrentTab = Tabs[0];

        Managers.Game.GoodsDatas[(int)Goods.Mileage]
            .Subscribe(value => { MileageText.text = value.ToCurrencyString(); });

        Managers.Game.GoodsDatas[(int)Goods.PvpToken].Subscribe(value =>
        {
            PvpTokenText.text = value.ToCurrencyString();
        });

        
        Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigation).AddTo(_guideComposite);
    }

    public override void Open()
    {
        base.Open();

        SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);
    }

    private void SetNavigation(int id)
    {
        if (Utils.IsAllClearGuideQuest())
        {
            _guideComposite.Clear();
            return;
        }
        
        if (Utils.IsCompleteGuideQuest())
        {
            PackageTabNavigationObj.SetActive(false);
            CashTabNavigationObj.SetActive(false);
            return;
        }
        
        PackageTabNavigationObj.SetActive(id == 29 && CurrentTab != Tabs[0]);
        CashTabNavigationObj.SetActive(id == 28 && CurrentTab != Tabs[1]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }
}