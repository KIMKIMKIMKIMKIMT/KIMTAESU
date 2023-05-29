using System;
using System.Collections.Generic;
using BackEnd;
using BackEnd.Tcp;
using GameData;
using Newtonsoft.Json;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChatPopup : UI_Popup
{
    [Serializable]
    public record Tab
    {
        public ChannelType ChannelType;
        public GameObject CoverObj;
        public Button TabButton;

        public void On()
        {
            CoverObj.SetActive(false);
        }

        public void Off()
        {
            CoverObj.SetActive(true);
        }
    }

    [SerializeField] private TMP_InputField ChatInput;

    [SerializeField] private Button SendChatButton;
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button BlockButton;

    [SerializeField] private GameObject ConnectServerObj;
    [SerializeField] private GameObject ChatObj;

    [SerializeField] private Transform ChatContentTr;

    [SerializeField] private UI_ChatBlockListPanel UIChatBlockListPanel;

    [SerializeField] private Tab[] Tabs;

    [SerializeField] private GameObject GuildImageObj;

    private readonly List<UI_ChatItem> _uiChatItems = new();

    private ChatCouponChart _couponChart;

    private Dictionary<string, int> _chatDic = new Dictionary<string, int>();

    private Tab _currentTab;

    private Tab CurrentTab
    {
        get => _currentTab;
        set
        {
            if (value.ChannelType == ChannelType.Guild && Managers.Guild.GuildData == null)
            {
                Managers.Message.ShowMessage("길드 가입 후 이용 가능 합니다.");
                return;
            }

            if (_currentTab != null)
                _currentTab.Off();

            _currentTab = value;
            _currentTab.On();
            GuildImageObj.SetActive(_currentTab.ChannelType == ChannelType.Guild);

            if (!Backend.Chat.IsChatConnect(_currentTab.ChannelType))
            {
                ConnectServerObj.SetActive(true);
                ChatObj.SetActive(false);

                foreach (var uiChatItem in _uiChatItems)
                    uiChatItem.gameObject.SetActive(false);

                switch (_currentTab.ChannelType)
                {
                    case ChannelType.Public:
                        Managers.Chat.ConnectChat();
                        break;
                    case ChannelType.Guild:
                        Managers.Chat.ConnectGuildChat();
                        break;
                }
            }
            else
                SetChatItems();
        }
    }

    public override bool isTop => true;

    private const int MaxChatItemCount = 30;

    private void Start()
    {
        SetChatHandler();

        SendChatButton.BindEvent(OnClickSendChat);
        CloseButton.BindEvent(ClosePopup);
        BlockButton.BindEvent(OnClickBlock);

        foreach (var tab in Tabs)
        {
            tab.Off();
            tab.TabButton.BindEvent(() => CurrentTab = tab);
        }

        CurrentTab ??= Tabs[0];

        Backend.Chat.OnJoinGuildChannel += args =>
        {
            if (args.ErrInfo != ErrorInfo.Success && !args.Session.IsRemote)
            {
                if (args.ErrInfo.Category == ErrorCode.AuthenticationFailed &&
                    args.ErrInfo.Detail == ErrorCode.DisconnectFromRemote)
                {
                    // 가입하지 않은 길드의 채널접속을 시도한 경우
                    if (args.ErrInfo.Reason.Contains("Incorrect"))
                        Managers.Message.ShowMessage("가입하지 않은 길드 채널 입니다.");
                    else if (args.ErrInfo.Reason.Contains("fail to read guild") ||
                             args.ErrInfo.Reason.Contains("Invalid"))
                        Managers.Message.ShowMessage("존재하지 않은 길드 채널 입니다.");

                    CurrentTab = Tabs[0];
                }
            }
        };

        Managers.Chat.OnFailGetGuildGroupChannelList += () =>
        {
            Managers.Message.ShowMessage("길드에 가입되어 있지 않습니다.");
            CurrentTab = Tabs[0];
        };

        Backend.Chat.OnLeaveGuildChannel += args =>
        {
            if (args.ErrInfo == ErrorInfo.Success && Managers.Guild.GuildData == null && !args.Session.IsRemote)
                CurrentTab = Tabs[0];
        };

        foreach (var data in ChartManager.ChatCouponCharts.Values)
        {
            _chatDic.Add(data.ChatKey, data.Id);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIChatBlockListPanel.gameObject.activeSelf)
            {
                UIChatBlockListPanel.Close();
                return;
            }

            ClosePopup();
        }
    }

    public override void Open()
    {
        base.Open();

        if (CurrentTab != null)
            CurrentTab = CurrentTab;

        UIChatBlockListPanel.Close();
    }

    public void SetChatHandler()
    {
        Backend.Chat.OnJoinChannel += args =>
        {
            if (args.ErrInfo == ErrorInfo.Success)
            {
                if (!args.Session.IsRemote)
                {
                    ConnectServerObj.SetActive(false);
                    ChatObj.SetActive(true);
                }
            }
        };

        Backend.Chat.OnJoinGuildChannel += args =>
        {
            if (args.ErrInfo == ErrorInfo.Success)
            {
                if (!args.Session.IsRemote)
                {
                    ConnectServerObj.SetActive(false);
                    ChatObj.SetActive(true);
                }
            }
        };

        Managers.Chat.ChatDatas.ObserveAdd().Where(_ => CurrentTab.ChannelType == ChannelType.Public)
            .Subscribe(chatData => { MakeChatItem(chatData.Value); });

        Managers.Chat.GuildChatDatas.ObserveAdd().Where(_ => CurrentTab.ChannelType == ChannelType.Guild)
            .Subscribe(chatData => { MakeChatItem(chatData.Value); });
    }

    private void ClearChatItem()
    {
        foreach (var uiChatItem in _uiChatItems)
        {
            //Managers.Resource.Destroy(uiChatItem.gameObject);
            uiChatItem.gameObject.SetActive(false);
        }

        //_uiChatItems.Clear();
    }

    private void MakeChatItem(ChatData chatData)
    {
        UI_ChatItem uiChatItem;

        if (ChatContentTr.childCount > MaxChatItemCount && _uiChatItems.Count > MaxChatItemCount)
        {
            uiChatItem = _uiChatItems[0];
            _uiChatItems.RemoveAt(0);
            _uiChatItems.Add(uiChatItem);
            uiChatItem.transform.SetAsLastSibling();
        }
        else
        {
            uiChatItem = Managers.UI.MakeSubItem<UI_ChatItem>(ChatContentTr);
            _uiChatItems.Add(uiChatItem);
        }

        uiChatItem.gameObject.SetActive(true);
        uiChatItem.Init(chatData, OnDestroyChatItem);
    }

    private void SetChatItems()
    {
        ClearChatItem();

        switch (CurrentTab.ChannelType)
        {
            case ChannelType.Public:
                foreach (var chatData in Managers.Chat.ChatDatas)
                {
                    MakeChatItem(chatData);
                }

                break;
            case ChannelType.Guild:
                foreach (var chatData in Managers.Chat.GuildChatDatas)
                {
                    MakeChatItem(chatData);
                }

                break;
        }
    }

    private void OnClickSendChat()
    {
        if (!Backend.Chat.IsChatConnect(CurrentTab.ChannelType))
            return;

        string chatMessage = ChatInput.text;
        if (string.IsNullOrEmpty(chatMessage))
            return;

        if (_chatDic.ContainsKey(chatMessage))
        {
            if (!Managers.Game.ChatCouponDatas[_chatDic[chatMessage]])
            {
                Managers.Game.ChatCouponDatas[_chatDic[chatMessage]] = true;

                _couponChart = ChartManager.ChatCouponCharts[_chatDic[chatMessage]];

                Managers.Game.IncreaseItem(_couponChart.ItemType, _couponChart.ItemId, _couponChart.ItemValue);

                GameDataManager.SaveItemData(_couponChart.ItemType);
                GameDataManager.ChatCouponGameData.SaveGameData();

                var gainItemDatas = new Dictionary<(ItemType, int), double>
                {
                    [(_couponChart.ItemType, _couponChart.ItemId)] = _couponChart.ItemValue
                };

                Managers.UI.ShowGainItems(gainItemDatas);

                Param param = new Param()
                {
                    { "ChatKey", chatMessage },
                    { "ItemType", _couponChart.ItemType.ToString()},
                    { "ItemId", _couponChart.ItemId.ToString() },
                    { "ItemValue", _couponChart.ItemValue.ToString() }
                };

                Backend.GameLog.InsertLog("ChatLog", param);
            }
        }

        chatMessage = chatMessage.Replace("\n", string.Empty);

        ChatData chatData = new ChatData()
        {
            Rank = Managers.Rank.MyRankDatas.ContainsKey(RankType.Stage)
                ? Managers.Rank.MyRankDatas[RankType.Stage].Rank
                : 0,
            CostumeId = Managers.Game.EquipDatas[EquipType.ShowCostume],
            GuildName = Managers.Guild.GuildData != null ? Managers.Guild.GuildData.GuildName : string.Empty,
            Nickname = Backend.UserNickName,
            ChatMessage = chatMessage
        };

        Backend.Chat.ChatToChannel(CurrentTab.ChannelType, JsonConvert.SerializeObject(chatData));

        ChatInput.text = string.Empty;
    }

    private void OnDestroyChatItem(UI_ChatItem uiChatItem)
    {
        Destroy(uiChatItem.gameObject);

        if (_uiChatItems.Contains(uiChatItem))
            _uiChatItems.Remove(uiChatItem);
    }

    private void OnClickBlock()
    {
        UIChatBlockListPanel.Open();
    }
}