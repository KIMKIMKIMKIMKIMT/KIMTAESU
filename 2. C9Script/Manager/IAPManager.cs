using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using AppsFlyerSDK;
using IAP;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance;

    private IIAP StoreIAP;
    private readonly Dictionary<string, int> _productAmountDic = new();

    public bool IsBuyProcess;

    private void Start()
    {
        Instance = this;
        Init();
    }

    public void Init()
    {
        switch (Managers.Manager.StoreType)
        {
            case StoreType.GoogleStore:
                StoreIAP = new IAP_GoogleStore();
                break;
            case StoreType.OneStore:
                StoreIAP = IAP_OneStore.Instance;
                break;
            default:
                return;
        }

        foreach (var shopChart in ChartManager.ShopCharts.Values)
            _productAmountDic[shopChart.ProductName] = shopChart.PriceValue;

        StoreIAP.ChaplinPaymentAction =
            (receipt, productId) => StartCoroutine(CoPaymentChaplin(receipt, productId));

        StoreIAP.Init();
    }

    public void BuyItem(int shopId, Action onCompleteCallback, Action onFailCallback)
    {
        if (IsBuyProcess)
            return;

        if (!ChartManager.ShopCharts.TryGetValue(shopId, out var shopChart))
        {
            Debug.Log($"Can't Find Shop : {shopId}");
            onFailCallback?.Invoke();
            return;
        }

        onFailCallback += () => IsBuyProcess = false;

        IsBuyProcess = true;
        StoreIAP.BuyItem(shopChart.ProductName, onCompleteCallback, onFailCallback);
    }

    IEnumerator CoPaymentChaplin(string receipt, string productId)
    {
        if (Application.isEditor)
            yield break;

        AppsFlyer.sendEvent("af_purchase", new Dictionary<string, string>()
        {
            ["C"] = "af_purchase",
            [AFInAppEvents.REVENUE] = _productAmountDic[productId].ToString()
        });

        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                Managers.Server.IpAddress = ip.ToString();
            }
        }

        string marketType = "2";

        switch (Managers.Manager.StoreType)
        {
            case StoreType.GoogleStore:
                marketType = "2";
                break;
            case StoreType.OneStore:
                marketType = "3";
                break;
        }

        string url = "http://api.chaplingame.co.kr:10001/PaymentC?" +
                     "uid=" + $"{Managers.Server.UserId}_Server{Managers.Server.CurrentServer}" +
                     "&user_email=" + Managers.Server.UserId +
                     "&game_code=CGR" +
                     $"&server_id={Managers.Server.CurrentServer}" +
                     "&game_usn=" + $"{Managers.Server.UserId}_Server{Managers.Server.CurrentServer}" +
                     "&country_code=kr" +
                     "&user_mdn=0" +
                     "&user_nick=" + BackEnd.Backend.UserNickName +
                     "&app_type=1" +
                     $"&market={marketType}" +
                     "&amount=" + _productAmountDic[productId] +
                     "&item_id=" + productId +
                     "&trn_no=" + receipt +
                     "&pay_result=1" +
                     "&ipt_ip_addr=" + Managers.Server.IpAddress;

        // UnityWebRequest에 내장되있는 GET 메소드를 사용한다.
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest(); // 응답이 올때까지 대기한다.

        if (www.error == null) // 에러가 나지 않으면 동작.
        {
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("error");
        }
    }

    private void OnDestroy()
    {
        if (StoreIAP != null)
            StoreIAP.Clear();
    }
}