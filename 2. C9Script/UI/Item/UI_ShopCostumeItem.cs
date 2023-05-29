using Chart;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopCostumeItem : UI_Base
{
    [SerializeField] private TMP_Text CostumeNameText;
    [SerializeField] private TMP_Text LimitText;
    [SerializeField] private TMP_Text CashPriceText;
    [SerializeField] private TMP_Text GoodsPriceText;

    [SerializeField] private UI_StatEffectItem EquipEffectItem1;
    [SerializeField] private UI_StatEffectItem EquipEffectItem2;

    [SerializeField] private UI_StatEffectItem HaveEffectItem1;
    [SerializeField] private UI_StatEffectItem HaveEffectItem2;

    [SerializeField] private Image CostumeIconImage;

    [SerializeField] private Button ItemButton;
    [SerializeField] private Button BuyButton;

    [SerializeField] private GameObject BuyButtonGoodsObj;
    [SerializeField] private GameObject SoldOutObj;

    private ShopChart _shopChart;
    private CostumeChart _costumeChart;

    public void Init(ShopChart shopChart)
    {
        _shopChart = shopChart;
        _costumeChart = ChartManager.CostumeCharts[_shopChart.RewardItemIds[0]];

        BuyButton.BindEvent(OnClickBuy);
        ItemButton.BindEvent(OnClickItem);

        SetUI();
    }

    private void SetUI()
    {
        CostumeIconImage.sprite = Managers.Resource.LoadCostumeIcon(_costumeChart.Icon);

        CostumeNameText.text = ChartManager.GetString(_costumeChart.Name);

        EquipEffectItem1.Init(_costumeChart.EquipStatType1, _costumeChart.EquipStatValue1);
        EquipEffectItem2.Init(_costumeChart.EquipStatType2, _costumeChart.EquipStatValue2);

        HaveEffectItem1.Init(_costumeChart.HaveStatType1, _costumeChart.HaveStatValue1);
        HaveEffectItem2.Init(_costumeChart.HaveStatType2, _costumeChart.HaveStatValue2);

        if (_shopChart.LimitType == ShopLimitType.None)
            LimitText.gameObject.SetActive(false);
        else
        {
            LimitText.gameObject.SetActive(true);
            LimitText.text =
                $"{_shopChart.LimitValue - Managers.Game.ShopDatas[_shopChart.ShopId]}/{_shopChart.LimitValue}";
        }

        if (_shopChart.PriceType == ShopPriceType.Goods)
        {
            BuyButtonGoodsObj.SetActive(true);
            CashPriceText.text = string.Empty;
            GoodsPriceText.text = _shopChart.PriceValue.ToString("N0");
        }
        else if (_shopChart.PriceType == ShopPriceType.Cash)
        {
            BuyButtonGoodsObj.SetActive(false);
            CashPriceText.text = $"{_shopChart.PriceValue:N0}원";
        }
        else if (_shopChart.PriceType == ShopPriceType.Free)
        {
            BuyButtonGoodsObj.SetActive(false);
            CashPriceText.text = $"Free";
        }

        BuyButton.gameObject.SetActive(Managers.Game.ShopDatas[_shopChart.ShopId] < _shopChart.LimitValue);
        SoldOutObj.SetActive(_shopChart.LimitType != ShopLimitType.None &&
                             Managers.Game.ShopDatas[_shopChart.ShopId] >= _shopChart.LimitValue);
    }

    private void OnClickBuy()
    {
        if (Managers.Game.ShopDatas[_shopChart.ShopId] >= _shopChart.LimitValue)
            return;

        Utils.BuyShopItem(_shopChart.ShopId, true, true, false, SetUI);
    }

    private void OnClickItem()
    {
        Managers.Model.PlayerModel.SetCostume(_costumeChart.Id);
    }
}