using System;
using Chart;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class UI_ShopPackageItem : UI_Base
{
    [SerializeField] private TMP_Text PriceText;
    [SerializeField] private TMP_Text LimitText;
    [SerializeField] private TMP_Text ResetText;
    [SerializeField] private Image ProductImage;
    [SerializeField] private Button BuyButton;

    [SerializeField] private GameObject ResetObj;
    [SerializeField] private GameObject LimitObj;
    [SerializeField] private GameObject SoldOutObj;
    [SerializeField] private GameObject TwinkObj;

    private ShopChart _shopChart;

    private void Start()
    {
        BuyButton.BindEvent(OnClickBuy);
    }

    public void Init(ShopChart shopChart)
    {
        _shopChart = shopChart;
        SetUI();
    }

    private void SetUI()
    {
        ProductImage.sprite = Managers.Resource.LoadShopIcon(_shopChart.Icon);

        switch (_shopChart.PriceType)
        {
            case ShopPriceType.Free:
                PriceText.text = "FREE";
                break;
            case ShopPriceType.Cash:
                PriceText.text = $"{_shopChart.PriceValue:N0}원";
                break;
        }

        ResetText.text = Utils.GetShopLimitText(_shopChart.LimitType);

        ResetObj.SetActive(_shopChart.LimitType != ShopLimitType.None &&
                           _shopChart.LimitType != ShopLimitType.NonReset);
        LimitObj.SetActive(_shopChart.LimitType != ShopLimitType.None);
        LimitText.text =_shopChart.LimitType != ShopLimitType.None?
                $"({_shopChart.LimitValue - Managers.Game.ShopDatas[_shopChart.ShopId]}/{_shopChart.LimitValue})" : "(무제한)";
        bool isSoldOut = _shopChart.LimitType != ShopLimitType.None &&
                         Managers.Game.ShopDatas[_shopChart.ShopId] >= _shopChart.LimitValue;
        SoldOutObj.SetActive(isSoldOut);
        TwinkObj.SetActive(!isSoldOut && (_shopChart.ShopId == 6701 || _shopChart.ShopId == 6702 || _shopChart.ShopId == 6801 || _shopChart.ShopId == 6802));
    }

    private void OnClickBuy()
    {
        Utils.BuyShopItem(_shopChart.ShopId, true, true, false,() =>
        {
            if (_shopChart.ProductName.Equals("shop_package_10") || _shopChart.ShopId == 6002)
            {
                Managers.Game.UserData.AdSkip = 1;
                GameDataManager.UserGameData.SaveGameData();
                Managers.Game.CalculateStat();
            }

            SetUI();
        });
    }
}