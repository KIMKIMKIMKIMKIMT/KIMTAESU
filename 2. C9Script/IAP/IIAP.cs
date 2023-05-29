using System;


public interface IIAP
{
    public void Init();
    public void BuyItem(string productId, Action onCompleteCallback, Action onFailCallback);
    public void Clear();
    public Action<string, string> ChaplinPaymentAction { get; set; }
}