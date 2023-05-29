using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildRankingPanel : UI_Panel
{
    enum PanelType
    {
        Ranking,
        Reward
    }

    [Serializable]
    public record RankerUI
    {
        public GameObject RootObj;
        public Image GradeImage;
        public Image MarkImage;
        public TMP_Text GuildNameText;

        public void SetUI(int rank)
        {
            if (!Managers.Rank.RankerGuildData.TryGetValue(rank, out var guildData))
                return;

            RootObj.SetActive(true);
            GradeImage.sprite = Managers.Resource.LoadGuildGradeIcon(guildData.GuildMetaData.Grade);
            MarkImage.sprite = Managers.Resource.LoadGuildMarkIcon(guildData.GuildMetaData.Mark);
            GuildNameText.text = guildData.GuildName;
        }
    }

    [Serializable]
    public record MyGuildRankingUI
    {
        public TMP_Text RankText;
        public TMP_Text GuildNameText;
        public TMP_Text ScoreText;
        public Image RankImage;

        public void SetUI(GuildRankData guildRankData)
        {
            if (guildRankData.Rank == 0)
            {
                RankText.text = "-";
                RankImage.gameObject.SetActive(false);
                ScoreText.text = "-";
            }
            else
            {
                RankText.text = guildRankData.Rank.ToString();
                RankImage.gameObject.SetActive(true);
                RankImage.sprite = Managers.Resource.LoadRankIcon(guildRankData.Rank);
                ScoreText.text = guildRankData.Score.ToCurrencyString();
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

    [SerializeField] private Button CloseButton;

    [SerializeField] private Tab[] Tabs;
    [SerializeField] private RankerUI[] RankerUis;
    [SerializeField] private MyGuildRankingUI MyGuildRankUI;

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
                Managers.Rank.GetGuildRankList(SetGuildRankingItem);
        }
    }

    private readonly List<UI_GuildRankingItem> _uiGuildRankingItems = new();

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

    public override void Open()
    {
        base.Open();

        if (Type == PanelType.Ranking)
            Managers.Rank.GetGuildRankList(SetGuildRankingItem);
    }

    private void SetGuildRankingItem()
    {
        if (_uiGuildRankingItems.Count <= 0)
            UIGuildRankingItemRoot.DestroyInChildren();
        else
            _uiGuildRankingItems.ForEach(uiItem => uiItem.gameObject.SetActive(false));

        foreach (var rankerUI in RankerUis)
            rankerUI.RootObj.SetActive(false);

        var uiGuildRankingItems = _uiGuildRankingItems.ToList();

        int index = 0;

        GuildRankData myGuildRankData = new GuildRankData();

        foreach (var guildRankData in Managers.Rank.GuildRankDatas)
        {
            if (guildRankData.Rank >= 1 && guildRankData.Rank <= 3)
                RankerUis[guildRankData.Rank - 1].SetUI(guildRankData.Rank);

            UI_GuildRankingItem uiGuildRankingItem;

            if (uiGuildRankingItems.Count > index)
                uiGuildRankingItem = uiGuildRankingItems[index++];
            else
            {
                uiGuildRankingItem = Managers.UI.MakeSubItem<UI_GuildRankingItem>(UIGuildRankingItemRoot);
                _uiGuildRankingItems.Add(uiGuildRankingItem);
            }

            uiGuildRankingItem.gameObject.SetActive(true);
            uiGuildRankingItem.Init(guildRankData.Rank, guildRankData.GuildName, guildRankData.Score);

            if (guildRankData.GuildInDate == Managers.Guild.GuildData.InDate)
                myGuildRankData = guildRankData;
        }
        
        MyGuildRankUI.SetUI(myGuildRankData);
    }

    private void SetGuildRankingRewardItem()
    {
        UIGuildRankingRewardItemRoot.DestroyInChildren();

        //var rankRewardDatas = Managers.Rank.GuildRankRewardDatas.OrderBy(data => data.StartRank).ToList();

        foreach (var rankRewardData in Managers.Rank.GuildRankRewardDatas)
        {
            var uiGuildRankingRewardItem =
                Managers.UI.MakeSubItem<UI_GuildRankingRewardItem>(UIGuildRankingRewardItemRoot);
            uiGuildRankingRewardItem.Init(rankRewardData);
        }
    }
}