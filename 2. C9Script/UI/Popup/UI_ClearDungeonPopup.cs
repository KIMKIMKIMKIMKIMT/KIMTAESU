using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_ClearDungeonPopup : UI_Popup
{
    [SerializeField] private TMP_Text StepText;
    [SerializeField] private TMP_Text RewardValueText;

    [SerializeField] private Image RewardImage;

    public override bool isTop => true;

    public void Init(int clearStep, int goodsId, double goodsValue)
    {
        StepText.text = $"{clearStep}단계 도전 성공!";
        RewardImage.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, goodsId);
        RewardValueText.text =
            $"총 획득 {ChartManager.GetString(ChartManager.GoodsCharts[goodsId].Name)} : {goodsValue.ToCurrencyString()}";
    }

    public void SetWorldCup(int goodsId, double goodsValue)
    {
        StepText.text = string.Empty;
        RewardImage.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, goodsId);
        RewardValueText.text =
            $"총 획득 {ChartManager.GetString(ChartManager.GoodsCharts[goodsId].Name)} : {goodsValue.ToCurrencyString()}";
    }
}