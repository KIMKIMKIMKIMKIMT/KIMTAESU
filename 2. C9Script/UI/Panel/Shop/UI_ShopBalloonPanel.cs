using System;
using System.Collections;
using Chart;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public class UI_ShopBalloonPanel : UI_Panel
{
    [Serializable]
    struct FreeBalloonItem
    {
        public TMP_Text ItemText;
        public TMP_Text ItemValueText;
        public TMP_Text TimeText;
        public Button ReceiveButton;
        public GameObject ADIconObj;

        private ShopChart _shopChart;

        public void Init(ShopChart shopChart)
        {
            _shopChart = shopChart;
            
            ItemText.text = $"{shopChart.RewardItemValues[0]} 보석";
            ADIconObj.SetActive(shopChart.PriceType == ShopPriceType.AD);
            ReceiveButton.BindEvent(OnClickReceive);
            Refresh();
        }

        private void Refresh()
        {
            ItemValueText.text = $"({_shopChart.LimitValue - Managers.Game.ShopDatas[_shopChart.ShopId]}/{_shopChart.LimitValue})";
            UpdateTime();
        }

        private void OnClickReceive()
        {
            if (Managers.Game.UserData.AdSkip == 1)
            {
                Utils.BuyShopItem(_shopChart.ShopId, true, true, false, OnComplete);
                return;
            }
            else
            {
                if (_shopChart.LimitType != ShopLimitType.None && Managers.Game.ShopDatas[_shopChart.ShopId] >= _shopChart.LimitValue)
                    return;

                if (Managers.Game.FreeShopItemRemainTimes.TryGetValue(_shopChart.ShopId, out var receiveTime))
                {
                    var timeGap = receiveTime - Utils.GetNow();
                    if (timeGap.TotalSeconds > 0)
                        return;
                }

                Utils.BuyShopItem(_shopChart.ShopId, true, true, false, OnComplete);
            }
            
        }
        
        void OnComplete()
        {
            if (Managers.Game.FreeShopItemRemainTimes.ContainsKey(_shopChart.ShopId))
                Managers.Game.FreeShopItemRemainTimes[_shopChart.ShopId] = Utils.GetNow().AddMinutes(10);
            else
                Managers.Game.FreeShopItemRemainTimes.Add(_shopChart.ShopId, Utils.GetNow().AddMinutes(10));
            
            PlayerPrefs.SetString($"Shop_{_shopChart.ShopId}_ReceiveTime",
                Managers.Game.FreeShopItemRemainTimes[_shopChart.ShopId].ToString());

            Refresh();
        }

        public void UpdateTime()
        {
            if (_shopChart == null)
                return;

            if (Managers.Game.UserData.AdSkip == 1)
            {
                TimeText.text = "FREE";
                return;
            }
            
            if (Managers.Game.FreeShopItemRemainTimes.TryGetValue(_shopChart.ShopId, out var receiveTime))
            {
                var timeGap = receiveTime -  Utils.GetNow();
                if (timeGap.TotalSeconds > 0)
                {
                    TimeText.text = timeGap.Minutes > 0
                        ? $"{timeGap.Minutes}분 {timeGap.Seconds}초"
                        : $"{timeGap.Seconds}초";
                }
                else
                    TimeText.text = "FREE";
            }
            else
                TimeText.text = "FREE";
        }
    }
    [SerializeField] private FreeBalloonItem[] FreeBalloonItems;
    
    [Serializable]
    struct SpecialBalloonItem
    {
        public TMP_Text PriceText;
        public Image ProductImage;
        public Button BuyButton;
        public TMP_Text LimitText;
        public TMP_Text ResetText;

        public GameObject ResetObj;
        public GameObject SoldOutGameObject;

        private ShopChart _shopChart;

        public void Init(ShopChart shopChart)
        {
            _shopChart = shopChart;
            ProductImage.sprite = Managers.Resource.LoadShopIcon(shopChart.Icon);
            PriceText.text = $"{shopChart.PriceValue:N0}원";
            ResetText.text = Utils.GetShopLimitText(_shopChart.LimitType);
            ResetObj.SetActive(_shopChart.LimitType != ShopLimitType.None && _shopChart.LimitType != ShopLimitType.NonReset);
            BuyButton.BindEvent(OnClickBuy);
        }

        private void Refresh()
        {
            PriceText.text = $"{_shopChart.PriceValue:N0}원";
            LimitText.text = _shopChart.LimitType == ShopLimitType.None
                ? string.Empty
                : $"{_shopChart.LimitValue - Managers.Game.ShopDatas[_shopChart.ShopId]} / {_shopChart.LimitValue}";
            
            SoldOutGameObject.SetActive(_shopChart.LimitType != ShopLimitType.None && Managers.Game.ShopDatas[_shopChart.ShopId] >= _shopChart.LimitValue);
        }

        private void OnClickBuy()
        {
            Utils.BuyShopItem(_shopChart.ShopId, true, true, false, Refresh);
        }
    }

    [SerializeField] private Transform UIShopPackageItemRoot;
    [SerializeField] private Transform UIShopBalloonItemRoot;

    [SerializeField] private GameObject BuyFreeBalloonObj;

    private void Start()
    {
        int freeIndex = 0;

        UIShopPackageItemRoot.DestroyInChildren();
        UIShopBalloonItemRoot.DestroyInChildren();

        foreach (var shopChart in ChartManager.ShopCharts.Values)
        {
            if (shopChart.ShopType != ShopType.StarBalloon)
                continue;

            switch (shopChart.SubType)
            {
                case 0: // 무료 별풍선 상품
                {
                    if (FreeBalloonItems.Length <= freeIndex)
                        continue;
                
                    FreeBalloonItems[freeIndex++].Init(shopChart);
                }
                    break;
                case 1: // 특별 상품
                {
                    UI_ShopPackageItem uiItem = Managers.UI.MakeSubItem<UI_ShopPackageItem>(UIShopPackageItemRoot);
                    uiItem.Init(shopChart);
                }
                    break;
                case 2: // 일반 별풍선 상품
                {
                    UI_ShopBalloonItem uiItem = Managers.UI.MakeSubItem<UI_ShopBalloonItem>(UIShopBalloonItemRoot);
                    uiItem.Init(shopChart);
                }
                    break;
            }
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
            
            BuyFreeBalloonObj.SetActive(id == 28);
        }

        void SetNavigationValue(long value)
        {
            if (Utils.IsCompleteGuideQuest())
                BuyFreeBalloonObj.SetActive(false);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(CoFreeBalloonTimer());
    }

    private IEnumerator CoFreeBalloonTimer()
    {
        while (true)
        {
            for (int i = 0; i < FreeBalloonItems.Length; i++)
            {
                FreeBalloonItems[i].UpdateTime();
            }

            yield return new WaitForSeconds(1f);
        }
    }
}