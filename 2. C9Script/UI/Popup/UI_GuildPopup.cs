using System;
using UI;
using UniRx;
using UnityEngine;

public class UI_GuildPopup : UI_Popup
{
    [SerializeField] private UI_NonGuildPanel UINonGuildPanel;
    [SerializeField] private UI_GuildPanel UIGuildPanel;

    public override void Open()
    {
        gameObject.SetActive(false);
        
        Managers.Guild.GetMyGuildData(() =>
        {
            gameObject.SetActive(true);
            
            base.Open();

            if (Managers.Guild.IsBelongGuild)
            {
                UINonGuildPanel.Close();
                UIGuildPanel.Open();
            }
            else
            {
                UIGuildPanel.Close();
                UINonGuildPanel.Open();
            }
        });
        
        MessageBroker.Default.Publish(new OpenPopupMessage<UI_GuildPopup>());
    }

    private void OnDisable()
    {
        MessageBroker.Default.Publish(new ClosePopupMessage<UI_GuildPopup>());
    }

    public void SetGuildUI()
    {
        UINonGuildPanel.Close();
        UIGuildPanel.Open();
    }
}