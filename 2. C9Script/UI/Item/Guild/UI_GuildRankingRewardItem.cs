using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildRankingRewardItem : UI_Base
{
    [SerializeField] private TMP_Text RankText;
    [SerializeField] private TMP_Text RewardValueText;

    [SerializeField] private Image RewardImage;
    [SerializeField] private Image RankImage;

    public void Init(RankRewardData rankRewardData)
    {
        RankText.text = rankRewardData.GetRankString();
        RewardImage.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, rankRewardData.ItemId);
        RewardValueText.text = rankRewardData.ItemValue.ToCurrencyString();
        RankImage.sprite = Managers.Resource.LoadRankIcon(rankRewardData.StartRank);
    }
}