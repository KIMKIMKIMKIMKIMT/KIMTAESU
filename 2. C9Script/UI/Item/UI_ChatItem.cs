using System;
using BackEnd;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChatItem : UI_Base
{
    [Serializable]
    public class ChatItem
    {
        public TMP_Text RankText;
        public TMP_Text NicknameText;
        public TMP_Text ChatText;
        public TMP_Text GuildText;
        public Image IconImage;
        public Button BlockButton;
        public GameObject Obj;
        public Image RankImage;
    }

    [SerializeField] private ChatItem MyChatItem;
    [SerializeField] private ChatItem OtherChatItem;

    [SerializeField] private GameObject BlockObj;
    [SerializeField] private Button CloseBlock;
    [SerializeField] private Button BlockButton;

    private ChatData _chatData;

    private Action<UI_ChatItem> _onDestroyChatItem;

    private const int _maxShowRank = 100;

    private void Start()
    {
        BlockObj.SetActive(false);
        
        OtherChatItem.BlockButton.BindEvent(() => BlockObj.SetActive(!BlockObj.activeSelf));
        CloseBlock.BindEvent(() => BlockObj.SetActive(false));
        BlockButton.BindEvent(OnClickBlock);

        Managers.Chat.ChatDatas.ObserveRemove().Subscribe(arg => OnRemoveChat(arg.Value));
        Managers.Chat.GuildChatDatas.ObserveRemove().Subscribe(arg => OnRemoveChat(arg.Value));

        void OnRemoveChat(ChatData chatData)
        {
            if (chatData != _chatData)
                return;
            
            _onDestroyChatItem?.Invoke(this);
        }

    }

    public void Init(ChatData chatData, Action<UI_ChatItem> onDestroyChatItem)
    {
        _chatData = chatData;
        _onDestroyChatItem = onDestroyChatItem;
        
        MyChatItem.Obj.SetActive(chatData.IsMy);
        OtherChatItem.Obj.SetActive(!chatData.IsMy);
        
        ChatItem chatItem = chatData.IsMy ? MyChatItem : OtherChatItem;

        chatItem.RankText.text = chatData.Rank <= _maxShowRank ? $"{chatData.Rank}위" : string.Empty;
        chatItem.RankText.color = Utils.GetRankColor(chatData.Rank);
        chatItem.NicknameText.text = chatData.Nickname;
        chatItem.GuildText.text = chatData.GuildName;
        
        chatItem.NicknameText.color = chatData.Nickname == "철구1009" ? Color.red : Color.white;

        chatItem.IconImage.sprite = Managers.Resource.LoadCostumeIcon(ChartManager.CostumeCharts[chatData.CostumeId].Icon);
        chatItem.ChatText.text = chatData.ChatMessage;
        
        if (chatData.Rank <= _maxShowRank)
        {
            chatItem.RankImage.gameObject.SetActive(true);
            chatItem.RankImage.sprite = Managers.Resource.LoadRankIcon(chatData.Rank);
        }
        else
            chatItem.RankImage.gameObject.SetActive(false);
    }

    private void OnClickBlock()
    {
        Backend.Chat.BlockUser(_chatData.Nickname, bro =>
        {
            if (!bro)
                return;
            
            Managers.Chat.RemoveChatData(_chatData.Nickname);
        });
    }
}
