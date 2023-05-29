using System;
using BackEnd;
using BackEnd.Tcp;
using GameData;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_GuildMemberItem : UI_Base
{
    [SerializeField] private TMP_Text LvText;
    [SerializeField] private TMP_Text NicknameText;
    [SerializeField] private TMP_Text PowerText;
    [SerializeField] private TMP_Text PositionText;
    [SerializeField] private TMP_Text ContributionText;
    [SerializeField] private TMP_Text ConnectStateText;
    [SerializeField] private TMP_Text AttendanceText;
    [SerializeField] private TMP_Text RankContributionText;

    [SerializeField] private Image CostumeImage;
    [SerializeField] private Image PromoImage;

    [SerializeField] private GameObject StateObj;
    [SerializeField] private Button LeaveButton;

    private GuildMemberData _guildMemberData;

    private void Start()
    {
        LeaveButton.BindEvent(OnClickLeave);
    }

    public void Init(GuildMemberData guildMemberData)
    {
        _guildMemberData = guildMemberData;

        SetUI();
    }

    private void SetUI()
    {
        LvText.text = $"Lv.{_guildMemberData.Lv}";
        NicknameText.text = _guildMemberData.Nickname;
        PowerText.text = Utils.GetPower(_guildMemberData.StatDatas, _guildMemberData.PromoGrade).ToCurrencyString();
        PositionText.text = _guildMemberData.GetPositionString();
        ContributionText.text = _guildMemberData.GoodsDic[(int)GuildGoodsType.Gxp].ToCurrencyString();
        RankContributionText.text = _guildMemberData.GoodsDic[(int)GuildGoodsType.Rank].ToCurrencyString();

        AttendanceText.color = _guildMemberData.AttendanceTime > Utils.GetDay() ? Color.green : Color.red;
        AttendanceText.text = _guildMemberData.AttendanceTime > Utils.GetDay() ? "출석" : "미출석";

        CostumeImage.sprite =
            Managers.Resource.LoadItemIcon(ItemType.Costume, _guildMemberData.EquipData[EquipType.ShowCostume]);

        // if (_guildMemberData.PromoGrade <= 0)
        //     PromoImage.gameObject.SetActive(false);
        // else
        // {
            PromoImage.gameObject.SetActive(true);
            PromoImage.sprite = Managers.Resource.LoadPromoIcon(_guildMemberData.PromoGrade);
        //}
        
        LeaveButton.gameObject.SetActive(_guildMemberData.InDate == Backend.UserInDate);
        StateObj.SetActive(_guildMemberData.InDate != Backend.UserInDate);

        if (StateObj.activeSelf)
        {
            ConnectStateText.text = _guildMemberData.IsConnect() ? "접속중" : "미접속";
            ConnectStateText.color = _guildMemberData.IsConnect() ? Color.white : Color.red;
        }
    }

    private void OnClickLeave()
    {
        if (_guildMemberData.InDate != Backend.UserInDate)
            return;
        
        FadeScreen.Instance.OnLoadingScreen();

        string guildName = Managers.Guild.GuildData != null ? Managers.Guild.GuildData.GuildName : string.Empty;

        Backend.Guild.WithdrawGuildV3(bro =>
        {
            if (!bro.IsSuccess())
            {
                Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                if (statusCode.Contains("412") && errorCode.Contains("PreconditionFailed"))
                {
                    if (message.Contains("memberExist"))
                    {
                        Managers.Message.ShowMessage("학생이 남아있어 탈퇴할 수 없습니다.");
                    }
                    else if (message.Contains("subscribed guild"))
                    {
                        Managers.Message.ShowMessage("학교에 가입되어 있지 않습니다.");
                        FadeScreen.Instance.OnLoadingScreen();
                        var guildPopup = Managers.UI.FindPopup<UI_GuildPopup>();
                        if (guildPopup != null)
                            Managers.UI.ClosePopupUI(guildPopup);

                        Managers.Guild.GetMyGuildData(() => { Managers.UI.ShowPopupUI<UI_GuildPopup>(); }, false);

                        return;
                    }
                }
                else
                    Managers.Backend.FailLog("Fail WithDrawGuild", bro);
                
                FadeScreen.Instance.OffLoadingScreen();
                return;
            }

            {
                var guildPopup = Managers.UI.FindPopup<UI_GuildPopup>();
                if (guildPopup != null)
                    Managers.UI.ClosePopupUI(guildPopup);
                
                var param = new Param
                {
                    { "GuildName", guildName },
                    { "Type", "Leave" }
                };

                Backend.GameLog.InsertLog("Guild", param, 30);

                Managers.Guild.GetMyGuildData(() => { Managers.UI.ShowPopupUI<UI_GuildPopup>(); }, false);
                Managers.Message.ShowMessage("탈퇴 성공");

                Managers.Game.UserData.GuildJoinCoolTime = Utils.GetNow().AddDays(1);
                GameDataManager.UserGameData.SaveGameData();
                
                if (Backend.Chat.IsChatConnect(ChannelType.Guild))
                    Backend.Chat.LeaveChannel(ChannelType.Guild);
            }
        });
    }
}