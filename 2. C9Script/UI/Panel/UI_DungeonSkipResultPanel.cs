using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_DungeonSkipResultPanel : UI_Panel
{
    [SerializeField] private TMP_Text StepSuccessText;
    [SerializeField] private TMP_Text RewardText;
    [SerializeField] private TMP_Text SkipCountText;

    [SerializeField] private Image RewardImage;

    [SerializeField] private Button CloseButton;

    public void Start()
    {
        CloseButton.BindEvent(Close);
    }

    public void Init(int clearStep, int goodsId, double goodsValue, int skipCount)
    {
        StepSuccessText.text = $"{clearStep}단계 스킵 성공!";
        RewardImage.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, goodsId);
        RewardText.text =
            $"획득 {ChartManager.GetString(ChartManager.GoodsCharts[goodsId].Name)} : {goodsValue.ToCurrencyString()}";
        SkipCountText.text = $"X{skipCount}";
        
        Open();
    }
}