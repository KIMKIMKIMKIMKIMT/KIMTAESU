using Chart;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RankingRewardItem : UI_Base
{
    [SerializeField] private TMP_Text RankText;
    [SerializeField] private TMP_Text RewardText;

    [SerializeField] private Image RankImage;
    [SerializeField] private Image RewardImage;

    private RankRewardData _rankRewardData;

    public void Init(RankRewardData rankRewardData)
    {
        _rankRewardData = rankRewardData;
        SetUI();
    }

    private void SetUI()
    {
        RankText.text = _rankRewardData.GetRankString();
        RewardText.text = _rankRewardData.ItemValue.ToCurrencyString();

        RankImage.sprite = Managers.Resource.LoadRankIcon(_rankRewardData.StartRank);
        RewardImage.sprite =
            Managers.Resource.LoadItemIcon(_rankRewardData.ItemType, _rankRewardData.ItemId);
    }
    
}