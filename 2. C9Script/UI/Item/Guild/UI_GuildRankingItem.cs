using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildRankingItem : UI_Base
{
    [SerializeField] private TMP_Text RankText;
    [SerializeField] private TMP_Text GuildNameText;
    [SerializeField] private TMP_Text ScoreText;

    [SerializeField] private Image RankImage;

    public void Init(int rank, string guildName, double guildScore)
    {
        RankText.text = rank.ToString();
        GuildNameText.text = guildName;
        ScoreText.text = guildScore.ToCurrencyString();
        
        RankImage.sprite = Managers.Resource.LoadRankIcon(rank);
    }
}