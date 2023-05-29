using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;
using Gaa;
using GameData;
using UniRx;

public class IAP_OneStore : MonoBehaviour, IIAP
{
    private static readonly string TAG = "IAP_OneStore";

    public static IAP_OneStore Instance;

    private string base64EncodedPublicKey = "input your public key";

    private LogScript logger;

    private List<ProductDetail> productDetails;

    private readonly Dictionary<string, PurchaseData> purchaseMap = new();
    private readonly Dictionary<string, string> signatureMap = new();

    private readonly List<string> _productIds = new();
    private readonly Dictionary<string, string> productTypes = new();

    private Action _onCompleteCallback;
    private Action _onFailCallback;

    Text pointView;
    Button connection;

    enum PurchaseButtonState
    {
        NONE,
        ACKNOWLEDGE,
        CONSUME,
        REACTIVE,
        CANCEL
    };

    void Awake()
    {
        Instance = this;

        if (Managers.Manager.StoreType != StoreType.OneStore)
            return;

        GaaIapResultListener.OnLoadingVisibility += OnLoadingVisibility;

        GaaIapResultListener.PurchaseClientStateResponse += PurchaseClientStateResponse;
        GaaIapResultListener.OnLoadingVisibility += _ =>
        {
            _onFailCallback?.Invoke();
        };
        GaaIapResultListener.OnPurchaseUpdatedResponse += OnPurchaseUpdatedResponse;
        GaaIapResultListener.OnQueryPurchasesResponse += OnQueryPurchasesResponse;
        GaaIapResultListener.OnProductDetailsResponse += OnProductDetailsResponse;

        GaaIapResultListener.OnConsumeSuccessResponse += OnConsumeSuccessResponse;
        GaaIapResultListener.OnAcknowledgeSuccessResponse += OnAcknowledgeSuccessResponse;
        GaaIapResultListener.OnManageRecurringResponse += OnManageRecurringResponse;

        GaaIapResultListener.SendLog += SendLog;

        foreach (var data in ChartManager.ShopCharts)
            _productIds.Add(data.Value.ProductName);
    }

    void OnDestroy()
    {
        if (Managers.Manager.StoreType != StoreType.OneStore)
            return;

        GaaIapResultListener.OnLoadingVisibility -= OnLoadingVisibility;

        GaaIapResultListener.PurchaseClientStateResponse -= PurchaseClientStateResponse;
        GaaIapResultListener.OnLoadingVisibility -= _ => _onFailCallback?.Invoke();
        GaaIapResultListener.OnPurchaseUpdatedResponse -= OnPurchaseUpdatedResponse;
        GaaIapResultListener.OnQueryPurchasesResponse -= OnQueryPurchasesResponse;
        GaaIapResultListener.OnProductDetailsResponse -= OnProductDetailsResponse;

        GaaIapResultListener.OnConsumeSuccessResponse -= OnConsumeSuccessResponse;
        GaaIapResultListener.OnAcknowledgeSuccessResponse -= OnAcknowledgeSuccessResponse;
        GaaIapResultListener.OnManageRecurringResponse -= OnManageRecurringResponse;

        GaaIapResultListener.SendLog -= SendLog;

        GaaIapCallManager.Destroy();
    }

    void Start()
    {
        if (Managers.Manager.StoreType != StoreType.OneStore)
            return;

        StartCoroutine(StartConnectService());
    }

    PurchaseData GetPurchaseData(string productId)
    {
        PurchaseData pData = null;
        foreach (KeyValuePair<string, PurchaseData> pair in purchaseMap)
        {
            if (productId.Equals(pair.Key))
            {
                pData = pair.Value;
                break;
            }
        }

        return pData;
    }

    public void SendLog(string tag, string message)
    {
        // if (logger != null)
        //     logger.Log(tag, message);
        // else
            Debug.Log("[" + tag + "]: " + message);
    }

    // ======================================================================================
    // Mange Point
    // ======================================================================================

    public void UsePoint(int used)
    {
        int point = PlayerPrefs.GetInt("Point");
        if (point >= used)
        {
            int result = point - used;
            PlayerPrefs.SetInt("Point", result);
        }
        else
        {
            SendLog(TAG, "UsePoint: There are not enough points to use.");
        }
    }

    public void AddPoint(int point)
    {
        SendLog(TAG, "AddPorint: " + point + " point");
        int savedPoint = PlayerPrefs.GetInt("Point");
        int result = savedPoint + point;
        PlayerPrefs.SetInt("Point", result);
    }


    // ======================================================================================
    // Request
    // ======================================================================================

    IEnumerator StartConnectService()
    {
        yield return new WaitForSeconds(1.0f);
        StartConnection();
    }

    public void StartConnection()
    {
        SendLog(TAG, "StartConnection()");
        if (GaaIapCallManager.IsServiceAvailable() == false)
        {
            OnLoadingVisibility(true);
            base64EncodedPublicKey =
                "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQChAesfjSc+X5WPupWCC4HIdLuo3JbYs2Q3DDNrHNSyNoyeptNySL7xfNTr+4Pk/xjaj3NV8P3uOiQl7ltwH67n8t/2LL6neDAas5sHnlrf74ZtRIwYbJ/fuVj3EnAywdStoFUY2IE6+EkaV4+MZ+9pIxpUCS+fsLWua90wMM9v2QIDAQAB";
            GaaIapCallManager.StartConnection(base64EncodedPublicKey);
        }
        else
        {
            SendLog(TAG, "StartConnection: Already connected to the payment module.");
        }
    }

    void BuyProduct(string productId, string type)
    {
        SendLog(TAG, "BuyProduct - productId: " + productId + ", type: " + type);
        Debug.Log("BuyProduct");

        PurchaseFlowParams param = new PurchaseFlowParams();
        param.productId = productId;
        param.productType = type;

        GaaIapCallManager.LaunchPurchaseFlow(param);
    }

    void QueryPurchases()
    {
        OnLoadingVisibility(true);
        purchaseMap.Clear();
        signatureMap.Clear();

        // Delete all items in the purchase history list UI.

        GaaIapCallManager.QueryPurchases(ProductType.INAPP);
        GaaIapCallManager.QueryPurchases(ProductType.AUTO);
    }

    void ConsumePurchase(string productId)
    {
        SendLog(TAG, "ConsumePurchase: productId: " + productId);
        PurchaseData purchaseData = GetPurchaseData(productId);
        if (purchaseData != null)
        {
            OnLoadingVisibility(true);
            GaaIapCallManager.Consume(purchaseData, /*developerPayload*/null);
        }
        else
        {
            SendLog(TAG, "ConsumePurchase: purchase data is null.");
        }
        IAPManager.Instance.IsBuyProcess = false;
    }

    void AcknowledgePurchase(string productId)
    {
        SendLog(TAG, "AcknowledgePurchase: productId: " + productId);
        PurchaseData purchaseData = GetPurchaseData(productId);
        if (purchaseData != null)
        {
            OnLoadingVisibility(true);
            GaaIapCallManager.Acknowledge(purchaseData, /*developerPayload*/null);
        }
        else
        {
            SendLog(TAG, "AcknowledgePurchase: purchase data is null.");
        }
    }

    void ManageRecurringProduct(string productId)
    {
        SendLog(TAG, "ManageRecurringProduct: productId: " + productId);
        PurchaseData purchaseData = GetPurchaseData(productId);
        if (purchaseData != null)
        {
            OnLoadingVisibility(true);
            string recurringAction = RecurringAction.REACTIVATE;
            if (purchaseData.recurringState == RecurringState.RECURRING)
            {
                recurringAction = RecurringAction.CANCEL;
            }

            GaaIapCallManager.ManageRecurringProduct(purchaseData, recurringAction);
        }
        else
        {
            SendLog(TAG, "ManageRecurringProduct: purchase data is null.");
        }
    }


    // ======================================================================================
    // Response
    // ======================================================================================

    void OnLoadingVisibility(bool visibility)
    {
        //loading.SetActive(visibility);
    }

    void PurchaseClientStateResponse(IapResult iapResult)
    {
        SendLog(TAG, "PurchaseClientStateResponse:\n\t\t-> " + iapResult.ToString());
        if (iapResult.IsSuccess())
        {
            Debug.Log("OneStore Connect");
            QueryPurchases();
            GaaIapCallManager.QueryProductDetails(_productIds.ToArray(), ProductType.ALL);
        }
        else
        {
            Debug.Log("OneStore Connect Fail");
            //text.text = "Disconnected";
            GaaIapResultListener.HandleError("PurchaseClientStateResponse", iapResult);
        }
    }

    void OnPurchaseUpdatedResponse(List<PurchaseData> purchases, List<string> signatures)
    {
        ParsePurchaseData("OnPurchaseUpdatedResponse", purchases, signatures);
    }

    void OnQueryPurchasesResponse(List<PurchaseData> purchases, List<string> signatures)
    {
        ParsePurchaseData("OnQueryPurchasesResponse", purchases, signatures);
    }

    private void ParsePurchaseData(string func, List<PurchaseData> purchases, List<string> signatures)
    {
        SendLog(TAG, func);
        for (int i = 0; i < purchases.Count; i++)
        {
            PurchaseData p = purchases[i];
            string s = signatures[i];

            purchaseMap.Add(p.productId, p);
            signatureMap.Add(p.productId, s);


            PurchaseButtonState state = PurchaseButtonState.NONE;
            {
                state = PurchaseButtonState.CONSUME;
            }

            string buttonText = state.ToString();
            string id = p.productId;

            SendLog(TAG, "PurchaseData[" + i + "]: " + p.productId);

            ConsumePurchase(p.productId);
        }
    }

    void OnProductDetailsResponse(List<ProductDetail> products)
    {
        SendLog(TAG, "OnProductDetailsResponse()");
        productDetails = products;

        foreach (ProductDetail detail in productDetails)
        {
            SendLog(TAG, "ProductDetail: " + detail.title);

            string id = detail.productId;
            string type = detail.type;

            if (productTypes.ContainsKey(id))
                productTypes[id] = type;
            else
                productTypes.Add(id, type);
        }
    }

    void OnDetailItemClick(string productId, string productType)
    {
        BuyProduct(productId, productType);
    }

    void OnConsumeSuccessResponse(PurchaseData purchaseData)
    {
        SendLog(TAG, "OnConsumeSuccessResponse:\n\t\t-> productId: " + purchaseData.productId);

        if (purchaseMap.ContainsKey(purchaseData.productId))
            purchaseMap.Remove(purchaseData.productId);

        if (signatureMap.ContainsKey(purchaseData.productId))
            signatureMap.Remove(purchaseData.productId);

        // 구매 복구 아이템
        if (_onCompleteCallback == null)
        {
            int shopId = ChartManager.ShopCharts.Values.ToList()
                .Find(shopChart => shopChart.ProductName == purchaseData.productId).ShopId;

            if (ChartManager.ShopCharts.TryGetValue(shopId, out var shopChart))
            {
                Managers.Game.ShopDatas[shopId] += 1;

                var itemTypes = new List<ItemType>();
                var gainItemDatas = new Dictionary<(ItemType, int), double>();

                for (int i = 0; i < shopChart.RewardItemTypes.Length; i++)
                {
                    if (shopChart.RewardItemTypes[i] == ItemType.None)
                        continue;

                    Managers.Game.IncreaseItem(shopChart.RewardItemTypes[i], shopChart.RewardItemIds[i],
                        shopChart.RewardItemValues[i]);

                    if (!itemTypes.Contains(shopChart.RewardItemTypes[i]))
                        itemTypes.Add(shopChart.RewardItemTypes[i]);

                    var gainItemKey = (shopChart.RewardItemTypes[i], shopChart.RewardItemIds[i]);

                    if (gainItemDatas.ContainsKey(gainItemKey))
                        gainItemDatas[(shopChart.RewardItemTypes[i], shopChart.RewardItemIds[i])] +=
                            shopChart.RewardItemValues[i];
                    else
                        gainItemDatas[(shopChart.RewardItemTypes[i], shopChart.RewardItemIds[i])] =
                            shopChart.RewardItemValues[i];
                }

                Managers.UI.ShowGainItems(gainItemDatas);

                Managers.Sound.PlayUISound(UISoundType.BuyShopItem);

                itemTypes.ForEach(GameDataManager.SaveItemData);

                GameDataManager.ShopGameData.SaveGameData();
                Param param = new Param();

                param.Add("ProductName", shopChart.ProductName);
                param.Add("GainItem", gainItemDatas);
                Utils.GetGoodsLog(ref param);

                Backend.GameLog.InsertLog("Shop", param);

                MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.BuyShop, shopId, 1));
            }
        }
        else
        {
            Managers.Message.ShowMessage("상품 구매 성공!");
            ChaplinPaymentAction?.Invoke(purchaseData.orderId, purchaseData.productId);
            _onCompleteCallback?.Invoke();
        }
        
        IAPManager.Instance.IsBuyProcess = false;
    }

    void OnAcknowledgeSuccessResponse(PurchaseData purchaseData)
    {
        SendLog(TAG, "OnAcknowledgeSuccessResponse:\n\t\t-> productId: " + purchaseData.productId);
        QueryPurchases();
    }

    void OnManageRecurringResponse(PurchaseData purchaseData, string action)
    {
        SendLog(TAG, "OnManageRecurringResponse:\n\t\t-> productId: " + purchaseData.productId + ", action: " + action);
        QueryPurchases();
    }

    public void Init()
    {
    }

    public void BuyItem(string productId, Action onCompleteCallback, Action onFailCallback)
    {
        Debug.Log("BuyItem");
        
        _onCompleteCallback = onCompleteCallback;
        _onFailCallback = onFailCallback;

        if (!productTypes.TryGetValue(productId, out var productType))
        {
            Debug.Log("Can't Find OneStore Product");
            onFailCallback?.Invoke();
            return;
        }

        var param = new PurchaseFlowParams
        {
            productId = productId,
            productType = productType
        };

        GaaIapCallManager.LaunchPurchaseFlow(param);
    }

    public void Clear()
    {
    }

    public Action<string, string> ChaplinPaymentAction { get; set; }
}