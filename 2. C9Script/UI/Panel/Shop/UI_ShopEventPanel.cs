using System.Collections.Generic;
using System.Linq;
using BackEnd;
using TMPro;
using UI;
using UnityEngine;
using UniRx;

public class UI_ShopEventPanel : UI_Panel
{
    [SerializeField] private TMP_Text GoodsValueText;

    [SerializeField] private Transform UIShopGoodsItemRoot;

    private readonly List<UI_ShopGoodsItem> _uiShopGoodsItems = new();

    private double _usingEventToken;
    private readonly Dictionary<int, double> _buyGoods = new();

    private readonly CompositeDisposable _compositeDisposable = new();

    private void OnEnable()
    {
        Managers.Game.GoodsDatas[(int)Goods.GaebalToken].Subscribe(token =>
        {
            GoodsValueText.text = token.ToCurrencyString();
        }).AddTo(_compositeDisposable);
    }

    private void OnDisable()
    {
        _compositeDisposable.Clear();
        
        if (_buyGoods.Count > 0)
        {
            GameDataManager.GoodsGameData.SaveGameData();
            GameDataManager.UserGameData.SaveGameData();
            
            Param param = new();
            
            param.Add("UsingNewYearEventToken", _usingEventToken);
            param.Add("BuyGoods", _buyGoods);
            Utils.GetGoodsLog(ref param);

            Backend.GameLog.InsertLog("Shop", param);

            _usingEventToken = 0;
            _buyGoods.Clear();
        }
    }

    public override void Open()
    {
        base.Open();
        MakeItems();
    }

    private void MakeItems()
    {
        if (_uiShopGoodsItems.Count <= 0)
            UIShopGoodsItemRoot.DestroyInChildren();
        else
            _uiShopGoodsItems.ForEach(uiShopGoodsItem => uiShopGoodsItem.gameObject.SetActive(false));

        var uiShopGoodsItems = _uiShopGoodsItems.ToList();

        int index = 0;

        foreach (var shopChart in ChartManager.ShopCharts.Values)
        {
            if (shopChart.ShopType != ShopType.Product)
                continue;

            if (shopChart.SubType != 3)
                continue;

            UI_ShopGoodsItem uiShopGoodsItem;

            if (uiShopGoodsItems.Count > index)
                uiShopGoodsItem = uiShopGoodsItems[index++];
            else
            {
                uiShopGoodsItem = Managers.UI.MakeSubItem<UI_ShopGoodsItem>(UIShopGoodsItemRoot);
                _uiShopGoodsItems.Add(uiShopGoodsItem);
            }
            
            uiShopGoodsItem.gameObject.SetActive(true);
            uiShopGoodsItem.Init(shopChart);
            uiShopGoodsItem.BuyAction = BuyAction;
        }
    }

    private void BuyAction(int price, int goodsId, double goodsValue)
    {
        _usingEventToken += price;
        if (_buyGoods.ContainsKey(goodsId))
            _buyGoods[goodsId] += goodsValue;
        else
            _buyGoods.Add(goodsId, goodsValue);
    }
}