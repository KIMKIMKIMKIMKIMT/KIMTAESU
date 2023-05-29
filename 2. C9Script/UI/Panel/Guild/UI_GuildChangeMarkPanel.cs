using System;
using BackEnd;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildChangeMarkPanel : UI_Panel
{
    [SerializeField] private Transform UIGuildMarkItemRoot;

    [SerializeField] private Button ChangeMarkButton;

    [SerializeField] private GameObject BackgroundObj;

    private UI_GuildMarkItem _selectUIGuildMarkItem;

    private UI_GuildMarkItem SelectUIGuildMarkItem
    {
        set
        {
            if (_selectUIGuildMarkItem != null)
                _selectUIGuildMarkItem.SetSelect(false);

            _selectUIGuildMarkItem = value;
            _selectUIGuildMarkItem.SetSelect(true);
        }
    }

    private void Start()
    {
        SetMarkItems();
        ChangeMarkButton.BindEvent(OnClickChangeMark);
        BackgroundObj.BindEvent(Close);
    }

    private void SetMarkItems()
    {
        UIGuildMarkItemRoot.DestroyInChildren();

        for (int i = 1; i <= 12; i++)
        {
            var uiGuildMarkItem = Managers.UI.MakeSubItem<UI_GuildMarkItem>(UIGuildMarkItemRoot);

            uiGuildMarkItem.SetSelect(false);
            uiGuildMarkItem.Init(i, OnClickGuildMark);
        }
    }

    private void OnClickGuildMark(UI_GuildMarkItem uiGuildMarkItem)
    {
        SelectUIGuildMarkItem = uiGuildMarkItem;
    }

    private void OnClickChangeMark()
    {
        if (!Managers.Guild.GuildMemberDatas.TryGetValue(Backend.UserInDate, out var myGuildMemberData))
        {
            Managers.Message.ShowMessage("잘못된 학교 데이터 입니다.");
            return;
        }

        if (!myGuildMemberData.Position.Equals("master"))
        {
            Managers.Message.ShowMessage("권한이 없습니다.");
            return;
        }

        if (Managers.Guild.GuildData.GuildMetaData.ChangeMarkCoolTime != null &&
            Utils.GetNow() < Managers.Guild.GuildData.GuildMetaData.ChangeMarkCoolTime)
        {
            var timeGap = Managers.Guild.GuildData.GuildMetaData.ChangeMarkCoolTime - Utils.GetNow();

            string messageText;

            if (timeGap.Value.Days > 0)
                messageText = $"{timeGap.Value.Days}일 후 교체 가능합니다.";
            else if (timeGap.Value.Hours > 0)
                messageText = $"{timeGap.Value.Hours}시간 후 교체 가능합니다.";
            else
                messageText = $"{timeGap.Value.Minutes}분 후 교체 가능합니다.";

            Managers.Message.ShowMessage(messageText);
            return;
        }

        if (_selectUIGuildMarkItem == null)
        {
            Managers.Message.ShowMessage("마크를 선택하세요.");
            return;
        }

        if (_selectUIGuildMarkItem.GuildMarkId == Managers.Guild.GuildData.GuildMetaData.Mark)
        {
            Managers.Message.ShowMessage("똑같은 마크 입니다.");
            return;
        }

        var changeMarkCoolTime = Utils.GetNow().AddDays(7);

        var param = new Param
        {
            { "Mark", _selectUIGuildMarkItem.GuildMarkId },
            { "ChangeMarkCoolTime", changeMarkCoolTime }
        };

        FadeScreen.Instance.OnLoadingScreen();

        Backend.Guild.ModifyGuildV3(param, bro =>
        {
            FadeScreen.Instance.OffLoadingScreen();

            if (!bro.IsSuccess())
            {
                Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                if (statusCode.Contains("400") && errorCode.Contains("BadParameterException") &&
                    message.Contains("bad guildName"))
                {
                    Managers.Message.ShowMessage("잘못된 호출 입니다.");
                }
                else if (statusCode.Contains("412") && errorCode.Contains("PreconditionFailed") &&
                         message.Contains("subscribed guild"))
                {
                    Managers.Message.ShowMessage("잘못된 호출 입니다.");
                }
                else if (statusCode.Contains("403") && errorCode.Contains("ForbiddenException"))
                    Managers.Message.ShowMessage("권한이 없습니다.");
                else
                {
                    Managers.Backend.FailLog("Fail ModifyGuild", bro);
                }

                return;
            }

            Managers.Guild.GuildData.GuildMetaData.Mark = _selectUIGuildMarkItem.GuildMarkId;
            Managers.Guild.GuildData.GuildMetaData.ChangeMarkCoolTime = changeMarkCoolTime;

            MessageBroker.Default.Publish(new GuildChangeMarkMessage(_selectUIGuildMarkItem.GuildMarkId));
            Managers.Message.ShowMessage("교체 완료");
            Close();
        });
    }
}