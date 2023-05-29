using System;
using Chart;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopGoodsItem : UI_Base
{
    [SerializeField] private TMP_Text ItemText;
    [SerializeField] private TMP_Text PriceText;
    [SerializeField] private Image ItemImage;
    [SerializeField] private Image PriceImage;
    [SerializeField] private Button BuyButton;
    [SerializeField] private TMP_Text LimitTypeText;
    [SerializeField] private TMP_Text LimitValueText;
    [SerializeField] private GameObject LimitObj;
    
    private ShopChart _shopChart;
    private bool _isSave;

    public Action<int, int, double> BuyAction;

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
        ItemImage.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, _shopChart.RewardItemIds[0]);
        ItemText.text =
            $"{ChartManager.GetGoodsName(_shopChart.RewardItemIds[0])} {_shopChart.RewardItemValues[0]}개";

        PriceImage.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, _shopChart.PriceId);
        PriceText.text = _shopChart.PriceValue.ToString();
        
        LimitObj.SetActive(_shopChart.LimitType != ShopLimitType.None && _shopChart.LimitType != ShopLimitType.NonReset);
        LimitTypeText.text = Utils.GetShopLimitText(_shopChart.LimitType);
        LimitValueText.text = _shopChart.LimitType != ShopLimitType.None ? $"({_shopChart.LimitValue - Managers.Game.ShopDatas[_shopChart.ShopId]}/{_shopChart.LimitValue})" : string.Empty;
    }

    private void OnClickBuy()
    {
        Utils.BuyShopItem(_shopChart.ShopId, false, false, true, () =>
        {
            BuyAction?.Invoke(_shopChart.PriceValue, _shopChart.RewardItemIds[0], _shopChart.RewardItemValues[0]);
            SetUI();
        });
    }
}