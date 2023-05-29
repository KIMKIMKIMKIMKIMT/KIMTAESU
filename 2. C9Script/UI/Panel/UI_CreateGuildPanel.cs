using System;
using System.Text.RegularExpressions;
using BackEnd;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreateGuildPanel : UI_Panel
{
    [SerializeField] private TMP_InputField GuildNameInputField;
    [SerializeField] private TMP_InputField GuildDescInputField;

    [SerializeField] private TMP_Text CostText;

    [SerializeField] private Transform GuildMarkItemRoot;

    [SerializeField] private Button CreateGuildButton;
    
    private UI_GuildMarkItem _selectGuildMarkItem;

    private void Start()
    {
        GuildNameInputField.onDeselect.AddListener(text =>
        {
            string guildName = Regex.Replace(text, @"[^가-힣]", "");
            GuildNameInputField.text = guildName;
        });

        CreateGuildButton.BindEvent(OnClickGuild);
        
        MakeGuildMarkItems();

        CostText.text = ChartManager.SystemCharts[SystemData.Guild_FoundCost].Value.ToCurrencyString();
    }

    private void OnDisable()
    {
        GuildNameInputField.text = string.Empty;
        GuildDescInputField.text = string.Empty;
    }

    public override void Open()
    {
        base.Open();

        GuildNameInputField.text = string.Empty;
        GuildDescInputField.text = string.Empty;

        if (_selectGuildMarkItem != null)
        {
            _selectGuildMarkItem.SetSelect(false);
            _selectGuildMarkItem = null;
        }
    }

    private void MakeGuildMarkItems()
    {
        GuildMarkItemRoot.DetachChildren();

        for (int i = 1; i <= 12; i++)
        {
            var uiGuildMarkItem = Managers.UI.MakeSubItem<UI_GuildMarkItem>(GuildMarkItemRoot);
            uiGuildMarkItem.Init(i, OnClickGuildMarkCallback);
            uiGuildMarkItem.SetSelect(false);
        }
    }

    private void OnClickGuildMarkCallback(UI_GuildMarkItem uiGuildMarkItem)
    {
        if (_selectGuildMarkItem != null)
            _selectGuildMarkItem.SetSelect(false);
        
        _selectGuildMarkItem = uiGuildMarkItem;
        _selectGuildMarkItem.SetSelect(true);
        
    }

    private void OnClickGuild()
    {
        string guildName = GuildNameInputField.text;
        string guildDesc = GuildDescInputField.text;

        if (Utils.GetNow() < Managers.Game.UserData.GuildJoinCoolTime)
        {
            var timeGap = Managers.Game.UserData.GuildJoinCoolTime - Utils.GetNow();

            Managers.Message.ShowMessage(timeGap.Hours < 1
                ? $"{timeGap}분 후 생성 가능합니다."
                : $"{timeGap.Hours}시간 후 생성 가능합니다.");
            return;
        }

        if (!Utils.IsEnoughItem(ItemType.Goods, (int)Goods.StarBalloon,
                ChartManager.SystemCharts[SystemData.Guild_FoundCost].Value))
        {
            Managers.Message.ShowMessage("재화가 부족합니다.");
            return;
        }
        
        if (string.IsNullOrEmpty(guildName))
        {
            Managers.Message.ShowMessage("길드 이름을 입력하세요");
            return;
        }

        if (!Regex.IsMatch(guildName, @"^[가-힣]"))
        {
            Managers.Message.ShowMessage("길드명은 한글 2~8자로만 가능합니다.");
            return;
        }

        if (guildName.Length < 1 || guildName.Length > 8)
        {
            Managers.Message.ShowMessage("길드 이름은 2~8자로 해야 합니다");
            return;
        }

        if (string.IsNullOrEmpty(guildDesc))
        {
            Managers.Message.ShowMessage("길드 설명을 입력하세요");
            return;
        }

        if (guildDesc.Length < 2 || guildDesc.Length > 60)
        {
            Managers.Message.ShowMessage("길드 설명은 2~60자로 해야 합니다");
            return;
        }

        if (_selectGuildMarkItem == null)
        {
            Managers.Message.ShowMessage("길드 마크를 선택하세요");
            return;
        }
        
        FadeScreen.Instance.OnLoadingScreen();

        var param = new Param();
        
        param.Add("Desc", guildDesc);
        param.Add("Grade", 1);
        param.Add("Mark", _selectGuildMarkItem.GuildMarkId);
        param.Add("MemberMaxCount", 30);
        param.Add("Server", Managers.Server.CurrentServer);

        Backend.Guild.CreateGuildV3(guildName, 10, param, bro =>
        {
            if (!bro.IsSuccess())
            {
                FadeScreen.Instance.OffLoadingScreen();
                var statusCode = bro.GetStatusCode();
                var errorCode = bro.GetErrorCode();
                var message = bro.GetMessage();

                if (statusCode.Equals("400"))
                {
                    if ((errorCode.Equals("BadParameterException") && message.Contains("bad goodsCount is too big")) ||
                        (errorCode.Equals("UndefinedParameterException") &&
                         message.Contains("undefined goodsCount must be more then")))
                    {
                        Managers.Message.ShowMessage("잘못된 길드 설정입니다");
                    }
                }
                else if (statusCode.Equals("403"))
                {
                    if (errorCode.Equals("ForbiddenException") && message.Contains("Forbidden createGuild"))
                    {
                        Managers.Message.ShowMessage($"{ChartManager.SystemCharts[SystemData.Guild_Found_Level].Value}레벨 부터 학교를 만들 수 있습니다.");
                    }
                }
                else if (statusCode.Equals("409") && errorCode.Equals("DuplicatedParameterException") &&
                         message.Contains("Duplicated guildName"))
                {
                    Managers.Message.ShowMessage("이미 사용중인 길드명 입니다");
                }
                else if (statusCode.Equals("412"))
                {
                    if (errorCode.Equals("PreconditionFailed") && message.Contains("guildName 사전 조건을 만족하지 않습니다"))
                    {
                        Managers.Message.ShowMessage("잘못된 길드명 입니다");
                    }
                    else if (errorCode.Equals("PreconditionFailed") && message.Contains("JoinedGamer 사전 조건을 만족하지 않습니다"))
                    {
                        Managers.Message.ShowMessage("이미 길드에 가입되어 있습니다");
                    }
                }
                else
                    Managers.Backend.FailLog(bro, "Fail CreateGuild");

                return;
            }
            
            Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.StarBalloon, ChartManager.SystemCharts[SystemData.Guild_FoundCost].Value);
            GameDataManager.GoodsGameData.SaveGameData();

            Managers.Guild.SetGuildRandomData();
            
            Managers.Guild.GetMyGuildData(() =>
            {
                Managers.Message.ShowMessage("학교 생성 완료!");

                var guildPopup = Managers.UI.FindPopup<UI_GuildPopup>();
                if (guildPopup != null)
                    Managers.   UI.ClosePopupUI(guildPopup);
                
                Managers.UI.ShowPopupUI<UI_GuildPopup>();
            }, false);
        });
    }
}