using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class UI_GuildMemberPanel : UI_Panel
{
    [SerializeField] private Transform UIGuildMemberItemRoot;

    private readonly List<UI_GuildMemberItem> _uiGuildMemberItems = new();

    public override void Open()
    {
        base.Open();

        Managers.Guild.GetMyGuildMember(() =>
        {
            Managers.Guild.GetMyGuildGoodsData(SetUI);
        });
    }

    private void SetUI()
    {
        if (_uiGuildMemberItems.Count <= 0)
            UIGuildMemberItemRoot.DestroyInChildren();
        else
            _uiGuildMemberItems.ForEach(uiGuildMemberItem => uiGuildMemberItem.gameObject.SetActive(false));

        var uiGuildMemberItems = _uiGuildMemberItems.ToList();
        
        int index = 0;

        foreach (var guildMemberData in Managers.Guild.GuildMemberDatas.Values)
        {
            UI_GuildMemberItem uiGuildMemberItem;

            if (uiGuildMemberItems.Count > index)
                uiGuildMemberItem = uiGuildMemberItems[index++];
            else
            {
                uiGuildMemberItem = Managers.UI.MakeSubItem<UI_GuildMemberItem>(UIGuildMemberItemRoot);
                _uiGuildMemberItems.Add(uiGuildMemberItem);
            }
            
            uiGuildMemberItem.Init(guildMemberData);
            uiGuildMemberItem.gameObject.SetActive(true);
        }
    }
}