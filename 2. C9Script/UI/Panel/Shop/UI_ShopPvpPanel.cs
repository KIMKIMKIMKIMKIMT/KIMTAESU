using System;
using System.Collections.Generic;
using BackEnd;
using NSubstitute;
using UI;
using UnityEngine;

public class UI_ShopPvpPanel : UI_Panel
{
    [SerializeField] private Transform UIShopPvpItemRoot;
    
    public int usingPvpToken;
    public Dictionary<int, double> BuyGoods = new();

    public Action<int, int, double> BuyAction; 

    private void Start()
    {
        UIShopPvpItemRoot.DestroyInChildren();

        foreach (var shopChart in ChartManager.ShopCharts.Values)
        {
            if (shopChart.ShopType != ShopType.PvpToken)
                continue;

            var uiItem = Managers.UI.MakeSubItem<UI_ShopGoodsItem>(UIShopPvpItemRoot);
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
            
            param.Add("UsingPvpToken", usingPvpToken);
            param.Add("BuyGoods", BuyGoods);
            Utils.GetGoodsLog(ref param);

            Backend.GameLog.InsertLog("Shop", param);

            usingPvpToken = 0;
            BuyGoods.Clear();
        }
    }
    
    private void OnBuy(int pvpToken, int goodsId, double goodsValue)
    {
        usingPvpToken += pvpToken;
        if (BuyGoods.ContainsKey(goodsId))
            BuyGoods[goodsId] += goodsValue;
        else
            BuyGoods.Add(goodsId, goodsValue);
    }
}