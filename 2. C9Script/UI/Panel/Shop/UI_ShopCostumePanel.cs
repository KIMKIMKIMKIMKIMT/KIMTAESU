using UI;
using UniRx;
using UnityEngine;

public class UI_ShopCostumePanel : UI_Panel
{
    [SerializeField] private Transform UIShopCostumeItemRoot;
    [SerializeField] private GameObject BuyCostumeNavigationObj;
    
    private void Start()
    {
        UIShopCostumeItemRoot.DestroyInChildren();

        foreach (var chartData in ChartManager.ShopCharts.Values)
        {
            if (chartData.ShopType != ShopType.Product)
                continue;

            if (chartData.SubType != (int)ShopProductType.Costume)
                continue;

            UI_ShopCostumeItem uiItem = Managers.UI.MakeSubItem<UI_ShopCostumeItem>(UIShopCostumeItemRoot);
            uiItem.Init(chartData);
        }

        SetGuideEvent();
    }

    private void SetGuideEvent()
    {
        if (Utils.IsAllClearGuideQuest())
            return;

        var guideComposite = new CompositeDisposable();
        
        Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigation).AddTo(guideComposite);
        SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);

        Managers.Game.UserData.OnChangeGuideQuestProgressValue.Subscribe(SetNavigationValue).AddTo(guideComposite);
        SetNavigationValue(Managers.Game.UserData.ProgressGuideQuestValue);

        void SetNavigation(int id)
        {
            BuyCostumeNavigationObj.SetActive(id == 14);
        }

        void SetNavigationValue(long value)
        {
            if (Utils.IsAllClearGuideQuest())
            {
                BuyCostumeNavigationObj.SetActive(false);
                guideComposite.Clear();
                return;
            }
            
            if (!Utils.IsCompleteGuideQuest())
                return;
            
            BuyCostumeNavigationObj.SetActive(false);
        }
    }
}