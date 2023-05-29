using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildManagementMemberPanel : UI_Panel
{
    [SerializeField] private Transform UIGuildManagementItemRoot;
    [SerializeField] private Button CloseButton;
    
    private readonly List<UI_GuildManagementMemberItem> _uiGuildManagementMemberItems = new();

    private void Start()
    {
        CloseButton.BindEvent(Close);

        MessageBroker.Default.Receive<GuildExpelMessage>().Subscribe(message =>
        {
            if (Managers.Guild.GuildMemberDatas.ContainsKey(message.UIGuildManagementMemberItem.InDate))
                Managers.Guild.GuildMemberDatas.Remove(message.UIGuildManagementMemberItem.InDate);

            message.UIGuildManagementMemberItem.gameObject.SetActive(false);
        });
    }

    public override void Open()
    {
        base.Open();
        
        if (_uiGuildManagementMemberItems.Count <= 0)
            UIGuildManagementItemRoot.DestroyInChildren();
        else
            _uiGuildManagementMemberItems.ForEach(uiGuildManagementMemberItem => uiGuildManagementMemberItem.gameObject.SetActive(false));

        Managers.Guild.GetMyGuildMember(() =>
        { 
            Managers.Guild.SetGuildGoodsFlag(true);
            Managers.Guild.GetMyGuildGoodsData(SetUI);
        });
    }

    private void SetUI()
    {
        var uiGuildManagementMemberItems = _uiGuildManagementMemberItems.ToList();
        
        int index = 0;

        foreach (var guildMemberData in Managers.Guild.GuildMemberDatas.Values)
        {
            UI_GuildManagementMemberItem uiGuildManagementMemberItem;

            if (uiGuildManagementMemberItems.Count > index)
                uiGuildManagementMemberItem = uiGuildManagementMemberItems[index++];
            else
            {
                uiGuildManagementMemberItem = Managers.UI.MakeSubItem<UI_GuildManagementMemberItem>(UIGuildManagementItemRoot);
                _uiGuildManagementMemberItems.Add(uiGuildManagementMemberItem);
            }
            
            uiGuildManagementMemberItem.Init(guildMemberData);
            uiGuildManagementMemberItem.gameObject.SetActive(true);
        }
    }
}