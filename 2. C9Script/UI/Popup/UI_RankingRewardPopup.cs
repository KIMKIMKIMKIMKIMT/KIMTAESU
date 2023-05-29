using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_RankingRewardPopup : UI_Popup
{
    [SerializeField] private Button RankingButton;
    [SerializeField] private Button CloseButton;

    [SerializeField] private Transform UIRankingRewardItemRootTr;

    public override bool isTop => true;

    private void Start()
    {
        RankingButton.BindEvent(OnClickRanking);
        CloseButton.BindEvent(OnClickClose);

        MakeRankingRewardItems();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
            Managers.UI.ShowPopupUI<UI_RankingPopup>();
        }
    }

    private void MakeRankingRewardItems()
    {
        UIRankingRewardItemRootTr.DestroyInChildren();

        foreach (var rankRewardData in Managers.Rank.RankRewardDatas)
        {
            var uiRankingRewardItem = Managers.UI.MakeSubItem<UI_RankingRewardItem>(UIRankingRewardItemRootTr);
            uiRankingRewardItem.Init(rankRewardData);
        }
    }

    private void OnClickRanking()
    {
        ClosePopup();
        Managers.UI.ShowPopupUI<UI_RankingPopup>();
    }

    private void OnClickClose()
    {
        ClosePopup();
    }
}