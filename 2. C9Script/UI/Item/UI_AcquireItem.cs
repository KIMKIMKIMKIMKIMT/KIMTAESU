using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AcquireItem : UI_Base
{
    [SerializeField] private TMP_Text GoodsValueText;
    [SerializeField] private Image GoodsImage;
    public void Init(int goodsId, double goodsValue)
    {
        GoodsImage.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, goodsId);
        GoodsValueText.text = goodsValue.ToCurrencyString();
    }
}