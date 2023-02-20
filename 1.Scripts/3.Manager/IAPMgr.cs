using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Analytics;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class IAPMgr : DontDestroy<IAPMgr>, IStoreListener
{
    static IStoreController storeController = null;

    public bool isPurchaseUnderProcess = true;
    private string STORENAME
    {
        get
        {
            return GooglePlay.Name;
        }
    }

    private string[] _productlds;


    private void Start()
    {
        if (storeController == null)
        {
            _productlds = new string[] { "gem_100", "gem_500", "gem_1000", "gem_5000"};

            InitStore();
        }
    }

    void InitStore()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(_productlds[0], ProductType.Consumable, new IDs { { _productlds[0], STORENAME } });
        builder.AddProduct(_productlds[1], ProductType.Consumable, new IDs { { _productlds[1], STORENAME } });
        builder.AddProduct(_productlds[2], ProductType.Consumable, new IDs { { _productlds[2], STORENAME } });
        builder.AddProduct(_productlds[3], ProductType.Consumable, new IDs { { _productlds[3], STORENAME } });
        UnityPurchasing.Initialize(this, builder);
    }

    void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
    }

    void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnlnitializeFailed : " + error);
    }

    public void Purchase(int index)
    {
        if (storeController == null)
        {
            Debug.Log("구매 실패 : 결제 기능 초기화 실패");
        }
        else
        {
            PopupMgr.Instance.ShowOkCancelPopup("보석 구매", "제품 " + GetProductName(index) + "를 구매 하시겠습니까?", () =>
            {
#if UNITY_EDITOR
                GiveReward(_productlds[(int)index].ToString());
#elif UNITY_ANDROID
            storeController.InitiatePurchase(_productlds[(int)index]);
#endif
            });
        }
    }

    public string GetProductName(int index)
    {
        switch (index)
        {
            case 0:
                return "보석 100개";
            case 1:
                return "보석 500개";
            case 2:
                return "보석 1000개";
            case 3:
                return "보석 5000개";
            default:
                return "";

        }
    }
    PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs e)
    {
        bool validPurchase = true;
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

        try
        {
            var result = validator.Validate(e.purchasedProduct.receipt);
        }
        catch (IAPSecurityException)
        {
            validPurchase = false;
        }

        if (validPurchase)
        {
            GiveReward(e.purchasedProduct.definition.id);
        }

        return PurchaseProcessingResult.Complete;
    }

    void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        if (!p.Equals(PurchaseFailureReason.UserCancelled))
        {
            Debug.Log("구매 실패 : " + p);
        }
    }

    public void Pending()
    {
        isPurchaseUnderProcess = !isPurchaseUnderProcess;
    }

    public void GiveReward(string productId)
    {
        switch (productId)
        {
            case "gem_100":
                PlayerDataMgr.Instance.PlayerData.Gem += 100;
                PlayerDataMgr.Instance.SaveData();
                UIMgr.Instance.Refresh();
                break;
            case "gem_500":
                PlayerDataMgr.Instance.PlayerData.Gem += 500;
                PlayerDataMgr.Instance.SaveData();
                UIMgr.Instance.Refresh();
                break;
            case "gem_1000":
                PlayerDataMgr.Instance.PlayerData.Gem += 1000;
                PlayerDataMgr.Instance.SaveData();
                UIMgr.Instance.Refresh();
                break;
            case "gem_5000":
                PlayerDataMgr.Instance.PlayerData.Gem += 5000;
                PlayerDataMgr.Instance.SaveData();
                UIMgr.Instance.Refresh();
                break;
        }
    }
}