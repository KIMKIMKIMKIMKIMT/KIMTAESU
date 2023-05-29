using System;
using System.Collections.Generic;
using BackEnd;
using UI;
using UnityEngine;


public class UI_ShopMileagePanel : UI_Panel
{
    [SerializeField] private Transform UIShopMileageItemRoot;

    public int usingMileage;
    public Dictionary<int, double> BuyGoods = new();

    public Action<int, int, double> BuyAction; 

    private void Start()
    {
        UIShopMileageItemRoot.DestroyInChildren();

        foreach (var shopChart in ChartManager.ShopCharts.Values)
        {
            if (shopChart.ShopType != ShopType.Mileage)
                continue;

            var uiItem = Managers.UI.MakeSubItem<UI_ShopGoodsItem>(UIShopMileageItemRoot);
            uiItem.Init(shopChart);
            uiItem.BuyAction = OnBuy;
        }
    }

    private void OnDisable()
    {
        if (BuyGoods.Count > 0)
        {
            GameDataManager.GoodsGameData.SaveGameData();
            GameDataManager.UserGameData.SaveGameData();
            
            Param param = new();
            
            param.Add("UsingMileage", usingMileage);
            param.Add("BuyGoods", BuyGoods);
            Utils.GetGoodsLog(ref param);

            Backend.GameLog.InsertLog("Shop", param);

            usingMileage = 0;
            BuyGoods.Clear();
        }
    }

    private void OnBuy(int mileage, int goodsId, double goodsValue)
    {
        usingMileage += mileage;
        if (BuyGoods.ContainsKey(goodsId))
            BuyGoods[goodsId] += goodsValue;
        else
            BuyGoods.Add(goodsId, goodsValue);
    }
}