using System;
using System.Collections.Generic;
using System.Xml;
using GameData;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public class UI_RankingPopup : UI_Popup, LoopScrollPrefabSource, LoopScrollDataSource
{
    [Serializable]
    public class Tab
    {
        public RankType RankType;
        public Button TabButton;
        public GameObject SelectObj;
        public TMP_Text TabText;
    }

    [Serializable]
    public class RankerUI
    {
        public TMP_Text NicknameText;
        public TMP_Text GuildText;
        public GameObject ModelObj;
    }

    [Serializable]
    public class MyRankItem
    {
        public TMP_Text RankText;
        public TMP_Text NicknameText;
        public TMP_Text ScoreText;
        public Image RankImage;
        public TMP_Text StepText;
        public TMP_Text TimeText;

        public GameObject BaseInfoObj;
        public GameObject RaidInfoObj;
    }

    [SerializeField] private Tab[] Tabs;
    [SerializeField] private RankerUI[] RankerUis;
    [SerializeField] private MyRankItem MyRankItemUI;

    [SerializeField] private Button RankingRewardButton;
    [SerializeField] private Button CloseButton;
    
    [SerializeField] private LoopVerticalScrollRect ItemLoopScroll;

    [SerializeField] private GameObject TabRootObj;
    [SerializeField] private GameObject RankRootObj;

    private readonly Color NonSelectTabTextColor = new Color(87 / 255f, 87 / 255f, 87 / 255f);

    public override bool isTop => true;

    private readonly Stack<GameObject> _uiRankingItemsPool = new();
    private readonly Dictionary<Transform, UI_RankingItem> _uiRankingItemsCaching = new();

    private Tab _currentTab;

    public Tab CurrentTab
    {
        get => _currentTab;
        set
        {
            if (_currentTab == value)
                return;

            if (_currentTab != null)
            {
                _currentTab.TabText.color = NonSelectTabTextColor;
                _currentTab.SelectObj.SetActive(false);
            }

            _currentTab = value;

            _currentTab.TabText.color = Color.white;
            _currentTab.SelectObj.SetActive(true);

            SetUI();
        }
    }

    private void OnEnable()
    {
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.OpenMenu, (int)QuestOpenMenu.Ranking, 1));
    }

    private void Start()
    {
        for (int i = 0; i < Tabs.Length; i++)
        {
            int index = i;
            Tabs[i].TabButton.BindEvent(() => CurrentTab = Tabs[index]);
        }

        RankingRewardButton.BindEvent(OnClickRankingReward);
        CloseButton.BindEvent(ClosePopup);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI();            
        }
    }

    public override void Open()
    {
        base.Open();
        
        TabRootObj.SetActive(false);
        RankRootObj.SetActive(false);
        
        Managers.Rank.RefreshRankingListData(() =>
        {
            TabRootObj.SetActive(true);
            RankRootObj.SetActive(true);

            if (CurrentTab == null)
                CurrentTab = Tabs[0];
            else
                SetUI();
        });
    }

    private void SetUI()
    {
        Managers.Rank.GetRankerDic(CurrentTab.RankType, () =>
        {
            SetRankerModel();
            MakeRankingItems();
            SetMyRankingItem();
        });
    }

    private void SetRankerModel()
    {
        for (int i = 0; i < 3; i++)
        {
            PlayerModel rankerModel;

            switch (i)
            {
                case 0:
                    rankerModel = Managers.Model.Ranker1Model;
                    break;
                case 1:
                    rankerModel = Managers.Model.Ranker2Model;
                    break;
                case 2:
                    rankerModel = Managers.Model.Ranker3Model;
                    break;
                default:
                    continue;
            }

            if (!Managers.Rank.RankDatas.ContainsKey(CurrentTab.RankType))
            {
                RankerUis[i].ModelObj.SetActive(false);
                RankerUis[i].NicknameText.text = string.Empty;
                RankerUis[i].GuildText.text = string.Empty;
                continue;
            }
            
            if (Managers.Rank.RankDatas[CurrentTab.RankType].Count <= i || !Managers.Rank.RankerEquipDatas.ContainsKey((CurrentTab.RankType, i, EquipType.Costume)))
            {
                RankerUis[i].ModelObj.SetActive(false);
                RankerUis[i].NicknameText.text = string.Empty;
                RankerUis[i].GuildText.text = string.Empty;
            }
            else
            {
                RankerUis[i].ModelObj.SetActive(true);
                rankerModel.SetCostume(Managers.Rank.RankerEquipDatas[(CurrentTab.RankType, i, EquipType.Costume)]);
                rankerModel.SetWeapon(Managers.Rank.RankerEquipDatas[(CurrentTab.RankType, i, EquipType.Weapon)]);
                RankerUis[i].NicknameText.text = Managers.Rank.RankDatas[CurrentTab.RankType][i].Nickname;
                RankerUis[i].GuildText.text =
                    Managers.Rank.RankerGuildDic.TryGetValue(
                        Managers.Rank.RankDatas[CurrentTab.RankType][i].GamerInDate, out var guildName)
                        ? guildName
                        : string.Empty;
            }
        }
    }

    private void SetMyRankingItem()
    {
        if (CurrentTab.RankType == RankType.Raid)
        {
            MyRankItemUI.BaseInfoObj.SetActive(false);
            MyRankItemUI.RaidInfoObj.SetActive(true);
            MyRankItemUI.NicknameText.text = Managers.Rank.MyRankDatas.ContainsKey(CurrentTab.RankType)
                ? Managers.Rank.MyRankDatas[CurrentTab.RankType].Nickname
                : string.Empty;
            MyRankItemUI.RankText.text = Managers.Rank.MyRankDatas.ContainsKey(CurrentTab.RankType)
                ? Managers.Rank.MyRankDatas[CurrentTab.RankType].Rank.ToString()
                : string.Empty;
            MyRankItemUI.RankImage.sprite = Managers.Rank.MyRankDatas.ContainsKey(CurrentTab.RankType)
                ? Managers.Resource.LoadRankIcon(Managers.Rank.MyRankDatas[CurrentTab.RankType].Rank)
                : null;
            MyRankItemUI.StepText.text = Managers.Rank.MyRankDatas.ContainsKey(CurrentTab.RankType)
                ? $"{Managers.Rank.MyRankDatas[CurrentTab.RankType].Score}단계"
                : "0단계";

            int min = Managers.Rank.MyRankDatas.ContainsKey(CurrentTab.RankType)
                ? (int)(Managers.Rank.MyRankDatas[CurrentTab.RankType].Time / 60)
                : 0;
            float sec = Managers.Rank.MyRankDatas.ContainsKey(CurrentTab.RankType)
                ? Managers.Rank.MyRankDatas[CurrentTab.RankType].Time % 60
                : 0;

            MyRankItemUI.TimeText.text = $"{min:00}:{sec:N2}";
        }
        else
        {
            MyRankItemUI.BaseInfoObj.SetActive(true);
            MyRankItemUI.RaidInfoObj.SetActive(false);
            MyRankItemUI.NicknameText.text = Managers.Rank.MyRankDatas.ContainsKey(CurrentTab.RankType)
                ? Managers.Rank.MyRankDatas[CurrentTab.RankType].Nickname
                : string.Empty;
            MyRankItemUI.RankText.text = Managers.Rank.MyRankDatas.ContainsKey(CurrentTab.RankType)
                ? Managers.Rank.MyRankDatas[CurrentTab.RankType].Rank.ToString()
                : string.Empty;
            MyRankItemUI.ScoreText.text = Managers.Rank.MyRankDatas.ContainsKey(CurrentTab.RankType)
                ? Math.Truncate(Managers.Rank.MyRankDatas[CurrentTab.RankType].Score).ToCurrencyString()
                : string.Empty;
            MyRankItemUI.RankImage.sprite = Managers.Rank.MyRankDatas.ContainsKey(CurrentTab.RankType)
                ? Managers.Resource.LoadRankIcon(Managers.Rank.MyRankDatas[CurrentTab.RankType].Rank)
                : null;
        }
    }

    private void MakeRankingItems()
    {
        ItemLoopScroll.totalCount = Mathf.Min(Managers.Rank.RankDatas.ContainsKey(CurrentTab.RankType) ? Managers.Rank.RankDatas[CurrentTab.RankType].Count : 0, 100);
        ItemLoopScroll.prefabSource = this;
        ItemLoopScroll.dataSource = this;

        ItemLoopScroll.RefillCells();
    }

    private void OnClickRankingReward()
    {
        ClosePopup();
        Managers.UI.ShowPopupUI<UI_RankingRewardPopup>();
    }

    public GameObject GetObject(int index)
    {
        if (_uiRankingItemsPool.Count <= 0)
            return Managers.UI.MakeSubItem<UI_RankingItem>().gameObject;

        var uiRankingItem = _uiRankingItemsPool.Pop();
        uiRankingItem.SetActive(true);

        return uiRankingItem;
    }

    public void ReturnObject(Transform trans)
    {
        trans.gameObject.SetActive(false);
        trans.SetParent(transform, false);
        _uiRankingItemsPool.Push(trans.gameObject);
    }

    public void ProvideData(Transform tr, int idx)
    {
        if (!_uiRankingItemsCaching.TryGetValue(tr, out var uiRankingItem))
        {
            uiRankingItem = tr.GetComponent<UI_RankingItem>();
            _uiRankingItemsCaching.Add(tr, uiRankingItem);
        }

        if (uiRankingItem == null)
            return;

        uiRankingItem.Init(Managers.Rank.RankDatas[CurrentTab.RankType][idx]);
    }
}