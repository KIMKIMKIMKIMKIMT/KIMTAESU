using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildManagementApplicantsPanel : UI_Panel
{
    [SerializeField] private Transform UIGuildManagementApplicantItemRoot;

    [SerializeField] private Button CloseButton;

    private readonly List<UI_GuildManagementApplicantItem> _uiGuildManagementApplicantItems = new();

    private void Start()
    {
        CloseButton.BindEvent(Close);
        
        MessageBroker.Default.Receive<GuildApplicantMessage>().Subscribe(message =>
        {
            if (message.IsApprove)
            {
                Managers.Guild.ResetGuildMemberFlag();
                Managers.Guild.ResetGuildGoodsFlag();
            }

            if (Managers.Guild.GuildApplicantsDatas.ContainsKey(message.UIGuildManagementApplicantItem.InDate))
                Managers.Guild.GuildApplicantsDatas.Remove(message.UIGuildManagementApplicantItem.InDate);
            
            if (_uiGuildManagementApplicantItems.Contains(message.UIGuildManagementApplicantItem))
                _uiGuildManagementApplicantItems.Remove(message.UIGuildManagementApplicantItem);
            
            Destroy(message.UIGuildManagementApplicantItem.gameObject);
        });
    }

    public override void Open()
    {
        base.Open();

        if (_uiGuildManagementApplicantItems.Count <= 0)
            UIGuildManagementApplicantItemRoot.DestroyInChildren();
        else
            _uiGuildManagementApplicantItems.ForEach(uiItem => uiItem.gameObject.SetActive(false));
        
        Managers.Guild.GetGuildApplicants(SetGuildManagementApplicantItems);
        MessageBroker.Default.Publish(new OpenPanelMessage<UI_GuildManagementApplicantsPanel>());
    }

    private void SetGuildManagementApplicantItems()
    {
        var uiGuildManagementApplicantItems = _uiGuildManagementApplicantItems.ToList();

        int index = 0;

        foreach (var applicantMemberData in Managers.Guild.GuildApplicantsDatas.Values)
        {
            UI_GuildManagementApplicantItem uiGuildManagementApplicantItem;

            if (uiGuildManagementApplicantItems.Count > index)
                uiGuildManagementApplicantItem = uiGuildManagementApplicantItems[index++];
            else
            {
                uiGuildManagementApplicantItem = Managers.UI.MakeSubItem<UI_GuildManagementApplicantItem>(UIGuildManagementApplicantItemRoot);
                _uiGuildManagementApplicantItems.Add(uiGuildManagementApplicantItem);
            }
            
            uiGuildManagementApplicantItem.gameObject.SetActive(true);
            uiGuildManagementApplicantItem.Init(applicantMemberData);
        }
    }
}