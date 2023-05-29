using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing;
using BackEnd;
using LitJson;
using UniRx;

namespace IAP
{
    public class IAP_GoogleStore : IIAP, IStoreListener
    {
        private IStoreController _controller;
        private Action _onCompleteCallback;
        private Action _onFailCallback;
        private string _productId;

        public Action<string, string> ChaplinPaymentAction { get; set; }

        public void Init()
        {
            InitUnityAPI();
        }

        private async void InitUnityAPI()
        {
            var options = new InitializationOptions().SetEnvironmentName("production");

            await UnityServices.InitializeAsync(options);

            InitStore();
        }

        private void InitStore()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var shopChart in ChartManager.ShopCharts.Values)
            {
                if (shopChart.PriceType != ShopPriceType.Cash)
                    continue;

                builder.AddProduct(shopChart.ProductName, ProductType.Consumable, new IDs
                {
                    { shopChart.ProductName, GooglePlay.Name }
                });
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyItem(string productId, Action onCompleteCallback, Action onFailCallback)
        {
            _productId = productId;
            _onCompleteCallback = onCompleteCallback;
            _onFailCallback = onFailCallback;
            _controller.InitiatePurchase(productId);
        }

        public void Clear()
        {
            
        }

        // Unity IAP 초기화 실패
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            InitStore();
            IAPManager.Instance.IsBuyProcess = false;
        }

        // 구매 준비 완료
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller = controller;
        }

        // 구글 플레이 스토어 구매 성공
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
#if UNITY_EDITOR
            _onCompleteCallback?.Invoke();
            IAPManager.Instance.IsBuyProcess = false;
            return PurchaseProcessingResult.Complete;
#else
            var bro = Backend.Receipt.IsValidateGooglePurchase(purchaseEvent.purchasedProduct.receipt, "GOOGLE", false);
            if (bro.IsSuccess())
            {
                _onCompleteCallback?.Invoke();
                _onCompleteCallback = null;

                JsonData receiptData = JsonMapper.ToObject(purchaseEvent.purchasedProduct.receipt);
                receiptData = JsonMapper.ToObject(receiptData["Payload"].ToString());
                receiptData = JsonMapper.ToObject(receiptData["json"].ToString());
                var orderId = receiptData["orderId"].ToString();

                ChaplinPaymentAction?.Invoke(orderId, _productId);

                Managers.Message.ShowMessage("상품 구매 성공!");
                IAPManager.Instance.IsBuyProcess = false;

                return PurchaseProcessingResult.Complete;
            }
            else
            {
                _onFailCallback?.Invoke();
                IAPManager.Instance.IsBuyProcess = false;
                return PurchaseProcessingResult.Complete;
            }
#endif
        }

        // 구매 실패
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            if (!failureReason.Equals(PurchaseFailureReason.UserCancelled))
                Debug.Log($"구매 실패 : {failureReason}");

            _onFailCallback?.Invoke();
            IAPManager.Instance.IsBuyProcess = false;
        }
    }
}