using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
using System.Linq;
using System;

public class UI_GuildSportsRankingPanel : UI_Panel
{
    private enum PanelType
    {
        Ranking,
        Reward
    }

    [Serializable]
    public record GuildSportsRankerUI
    {
        public GameObject RootObj;
        public Image GradeImage;
        public Image MarkImage;
        public TMP_Text GuildNameText;

        public void SetUI(int rank)
        {
            if (!Managers.Rank.RankerGuildSportsData.TryGetValue(rank, out var guildData))
                return;

            RootObj.SetActive(true);
            GradeImage.sprite = Managers.Resource.LoadGuildGradeIcon(guildData.GuildMetaData.Grade);
            MarkImage.sprite = Managers.Resource.LoadGuildMarkIcon(guildData.GuildMetaData.Mark);
            GuildNameText.text = guildData.GuildName;
        }
    }

    [Serializable]
    public record MyGuildSportsRankingUI
    {
        public TMP_Text RankText;
        public TMP_Text GuildNameText;
        public TMP_Text ScoreText;
        public Image RankImage;

        public void SetUI(GuildSportsRankData guildSportsRankData)
        {
            if (guildSportsRankData.Rank == 0)
            {
                RankText.text = "-";
                RankImage.gameObject.SetActive(false);
                ScoreText.text = "-";
            }
            else
            {
                RankText.text = guildSportsRankData.Rank.ToString();
                RankImage.gameObject.SetActive(true);
                RankImage.sprite = Managers.Resource.LoadRankIcon(guildSportsRankData.Rank);
                ScoreText.text = guildSportsRankData.Score.ToCurrencyString();
            }

            GuildNameText.text = Managers.Guild.GuildData.GuildName;
        }
    }

    [Serializable]
    public record Tab
    {
        public Button TabButton;
        public GameObject PanelObj;

        public void On()
        {
            PanelObj.SetActive(true);
        }

        public void Off()
        {
            PanelObj.SetActive(false);
        }
    }

    #region Fields
    [SerializeField] private Button CloseButton;

    [SerializeField] private Tab[] Tabs;
    [SerializeField] private GuildSportsRankerUI[] RankerUis;
    [SerializeField] private MyGuildSportsRankingUI MyGuildRankUI;

    [SerializeField] private Transform UIGuildRankingItemRoot;
    [SerializeField] private Transform UIGuildRankingRewardItemRoot;

    private PanelType _panelType;

    private PanelType Type
    {
        get => _panelType;
        set
        {
            _panelType = value;

            if (_panelType == PanelType.Reward)
                Managers.Rank.GetGuildSportsRankList(SetGuildRankingItem);
        }
    }

    private List<UI_GuildRankingItem> _guildRankingItemList = new();
    #endregion

    #region Unity Methods
    private void Start()
    {
        CloseButton.BindEvent(Close);

        for (int i = 0; i < Tabs.Length; i++)
        {
            int index = i;

            Tabs[i].TabButton.BindEvent(() =>
            {
                if (index == 0)
                {
                    Tabs[0].On();
                    Tabs[1].Off();
                }
                else
                {
                    Tabs[1].On();
                    Tabs[0].Off();
                }
            });
        }

        SetGuildRankingRewardItem();
    }
    #endregion

    #region Public Methods
    public override void Open()
    {
        base.Open();

        if (Type == PanelType.Ranking)
            Managers.Rank.GetGuildSportsRankList(SetGuildRankingItem);
    }
    #endregion

    #region Private Methods
    private void SetGuildRankingItem()
    {
        if (_guildRankingItemList.Count <= 0)
            UIGuildRankingItemRoot.DestroyInChildren();
        else
            _guildRankingItemList.ForEach(uiItem => uiItem.gameObject.SetActive(false));

        foreach (var rankerUI in RankerUis)
            rankerUI.RootObj.SetActive(false);

        var uiGuildRankingItems = _guildRankingItemList.ToList();

        int index = 0;

        GuildSportsRankData myGuildRankData = new GuildSportsRankData();

        foreach (var guildRankData in Managers.Rank.GuildSportsRankDatas)
        {
            if (guildRankData.Rank >= 1 && guildRankData.Rank <= 3)
                RankerUis[guildRankData.Rank - 1].SetUI(guildRankData.Rank);

            UI_GuildRankingItem guildRankingItem;

            if (uiGuildRankingItems.Count > index)
                guildRankingItem = uiGuildRankingItems[index++];
            else
            {
                guildRankingItem = Managers.UI.MakeSubItem<UI_GuildRankingItem>(UIGuildRankingItemRoot);
                _guildRankingItemList.Add(guildRankingItem);
            }

            guildRankingItem.gameObject.SetActive(true);
            guildRankingItem.Init(guildRankData.Rank, guildRankData.GuildName, guildRankData.Score);

            if (guildRankData.GuildInDate == Managers.Guild.GuildData.InDate)
                myGuildRankData = guildRankData;
        }

        MyGuildRankUI.SetUI(myGuildRankData);
    }

    private void SetGuildRankingRewardItem()
    {
        UIGuildRankingRewardItemRoot.DestroyInChildren();

        foreach (var rankRewardData in Managers.Rank.GuildSportsRankRewardDatas)
        {
            var uiGuildRankingRewardItem =
                Managers.UI.MakeSubItem<UI_GuildRankingRewardItem>(UIGuildRankingRewardItemRoot);
            uiGuildRankingRewardItem.Init(rankRewardData);
        }
    }
    #endregion

}
