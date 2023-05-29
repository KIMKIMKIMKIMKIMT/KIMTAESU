using Chart;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopBalloonItem : UI_Base
{
    [SerializeField] private Image ProductImage;
    [SerializeField] private TMP_Text PriceText;
    [SerializeField] private TMP_Text LimitText;
    [SerializeField] private TMP_Text CashValueText;
    [SerializeField] private TMP_Text MileageValueText;
    [SerializeField] private Button BuyButton;
    [SerializeField] private GameObject SoldOutObj;
    
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
        PriceText.text = $"{_shopChart.PriceValue:N0}원";
        LimitText.text = _shopChart.LimitType == ShopLimitType.None
            ? "(무제한)"
            : $"({_shopChart.LimitValue - Managers.Game.ShopDatas[_shopChart.ShopId]}/{_shopChart.LimitValue})";

        for (int i = 0; i < _shopChart.RewardItemIds.Length; i++)
        {
            if (_shopChart.RewardItemIds[i] == (int)Goods.StarBalloon)
                CashValueText.text = _shopChart.RewardItemValues[i].ToString("N0");
            
            if (_shopChart.RewardItemIds[i] == (int)Goods.Mileage)
                MileageValueText.text = _shopChart.RewardItemValues[i].ToString("N0");
        }
        
        SoldOutObj.gameObject.SetActive(_shopChart.LimitType != ShopLimitType.None && Managers.Game.ShopDatas[_shopChart.ShopId] >= _shopChart.LimitValue);
    }

    private void OnClickBuy()
    {
        Utils.BuyShopItem(_shopChart.ShopId, true, true);
    }

    private void OnSuccessPurchase()
    {
        
    }
}