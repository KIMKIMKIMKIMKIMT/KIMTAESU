using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RankingItem : UI_Base
{
    [SerializeField] private TMP_Text RankText;
    [SerializeField] private TMP_Text NicknameText;
    [SerializeField] private TMP_Text ScoreText;

    [SerializeField] private TMP_Text StepText;
    [SerializeField] private TMP_Text TimeText;

    [SerializeField] private Image RankImage;

    [SerializeField] private GameObject BaseInfoObj;
    [SerializeField] private GameObject RaidInfoObj;
    
    private RankData _rankData;

    public void Init(RankData rankData)
    {
        _rankData = rankData;
        SetUI();
    }

    private void SetUI()
    {
        RankText.text = _rankData.Rank.ToString();
        NicknameText.text = _rankData.Nickname;
        ScoreText.text = Math.Truncate(_rankData.Score).ToCurrencyString();
        RankImage.sprite = Managers.Resource.LoadRankIcon(_rankData.Rank);

        switch (_rankData.RankType)
        {
            case RankType.Stage:
            case RankType.Pvp:
            {
                BaseInfoObj.SetActive(true);
                RaidInfoObj.SetActive(false);
            }
                break;
            case RankType.Raid:
            {
                BaseInfoObj.SetActive(false);
                RaidInfoObj.SetActive(true);

                int min = (int)(_rankData.Time / 60);
                float sec = _rankData.Time % 60;

                StepText.text = $"{_rankData.Score}단계";
                TimeText.text = min > 0 ? $"{min:00}:{sec:N2}" : $"00:{sec:N2}";
            }
                break;
        }
    }
}