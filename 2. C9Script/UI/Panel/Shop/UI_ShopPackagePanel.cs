using NSubstitute.Exceptions;
using UI;
using UniRx;
using UnityEngine;


public class UI_ShopPackagePanel : UI_Panel
{
    [SerializeField] private Transform UIPackageItemRoot;
    [SerializeField] private GameObject BuyFreePackageNavigationObj;

    private void Start()
    {
        UIPackageItemRoot.DestroyInChildren();

        foreach (var chartData in ChartManager.ShopCharts.Values)
        {
            if (chartData.ShopType != ShopType.Product)
                continue;

            if (chartData.SubType != (int)ShopProductType.Package)
                continue;

            UI_ShopPackageItem uiItem = Managers.UI.MakeSubItem<UI_ShopPackageItem>(UIPackageItemRoot);
            uiItem.Init(chartData);
        }
        
        SetGuideEvent();
    }

    private void SetGuideEvent()
    {
        if (Utils.IsAllClearGuideQuest())
            return;

        var guideComposite = new CompositeDisposable();
        
        Managers.Game.UserData.OnChangeGuideQuestId
            .Subscribe(SetNavigation)
            .AddTo(guideComposite);
        SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);

        Managers.Game.UserData.OnChangeGuideQuestProgressValue
            .Subscribe(SetNavigationValue)
            .AddTo(guideComposite);
        SetNavigationValue(Managers.Game.UserData.ProgressGuideQuestValue);
        
        void SetNavigation(int id)
        {
            if (Utils.IsAllClearGuideQuest())
            {
                guideComposite.Clear();
                return;
            }
            
            BuyFreePackageNavigationObj.SetActive(id == 29);
        }

        void SetNavigationValue(long value)
        {
            if (!Utils.IsCompleteGuideQuest())
                return;
            
            BuyFreePackageNavigationObj.SetActive(false);
        }
    }
}