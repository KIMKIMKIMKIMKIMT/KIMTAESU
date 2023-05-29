using System;
using BackEnd;
using GameData;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UI_GuildManagementMemberItem : UI_Base
{
    [SerializeField] private TMP_Text LvText;
    [SerializeField] private TMP_Text NicknameText;
    [SerializeField] private TMP_Text PowerText;
    [SerializeField] private TMP_Text PositionText;
    [SerializeField] private TMP_Text ContributionText;
    [SerializeField] private TMP_Text ChangePositionButtonText;

    [SerializeField] private Image CostumeImage;
    [SerializeField] private Image PromoImage;

    [SerializeField] private Button ChangePositionButton;
    [SerializeField] private Button KickButton;

    private GuildMemberData _guildMemberData;
    public string InDate => _guildMemberData.InDate;

    private void Start()
    {
        ChangePositionButton.BindEvent(OnClickChangePosition);
        KickButton.BindEvent(OnClickKick);
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
        ContributionText.text = _guildMemberData.GoodsDic[1].ToCurrencyString();

        CostumeImage.sprite =
            Managers.Resource.LoadItemIcon(ItemType.Costume, _guildMemberData.EquipData[EquipType.ShowCostume]);

        if (_guildMemberData.PromoGrade <= 0)
            PromoImage.gameObject.SetActive(false);
        else
        {
            PromoImage.gameObject.SetActive(true);
            PromoImage.sprite = Managers.Resource.LoadPromoIcon(_guildMemberData.PromoGrade);
        }
        
        ChangePositionButtonText.text = _guildMemberData.Position.Contains("member") ? "교수 임명" : "교수 해임";
    }

    private void OnClickChangePosition()
    {
        if (!Managers.Guild.GuildMemberDatas.TryGetValue(Backend.UserInDate, out var myGuildMemberData))
        {
            Managers.Message.ShowMessage("학교 데이터를 찾을 수 없습니다.");
            return;
        }

        if (!myGuildMemberData.Position.Contains("master"))
        {
            Managers.Message.ShowMessage("권한이 없습니다.");
            return;
        }

        // 학생이면 교수로 임명
        if (_guildMemberData.Position.Contains("member"))
        {
            FadeScreen.Instance.OnLoadingScreen();
            
            Backend.Guild.NominateViceMasterV3(_guildMemberData.InDate, bro =>
            {
                FadeScreen.Instance.OffLoadingScreen();
                
                if (!bro.IsSuccess())
                {
                    string statusCode = bro.GetStatusCode();
                    string errorCode = bro.GetErrorCode();
                    string message = bro.GetMessage();

                    if (statusCode.Contains("404") && errorCode.Contains("NotFoundException") &&
                        message.Contains("guildMember"))
                    {
                        Managers.Message.ShowMessage("존재하지 않는 멤버 입니다.");
                    }
                    else
                        Managers.Backend.FailLog("Fail NominateViceMaster", bro);

                    return;
                }

                _guildMemberData.Position = "viceMaster";
                SetUI();
            });
        }
        // 교수면 학생으로 임명
        else if (_guildMemberData.Position.Contains("vice"))
        {
            FadeScreen.Instance.OnLoadingScreen();
            
            Backend.Guild.ReleaseViceMasterV3(_guildMemberData.InDate, bro =>
            {
                FadeScreen.Instance.OffLoadingScreen();
                
                if (!bro.IsSuccess())
                {
                    string statusCode = bro.GetStatusCode();
                    string errorCode = bro.GetErrorCode();
                    string message = bro.GetMessage();

                    if (statusCode.Contains("404") && errorCode.Contains("NotFoundException") &&
                        message.Contains("guildMember"))
                    {
                        Managers.Message.ShowMessage("존재하지 않는 멤버 입니다.");
                    }
                    else if (statusCode.Contains("403") && errorCode.Contains("ForbiddenException"))
                        Managers.Message.ShowMessage("권한이 없습니다.");
                    else
                        Managers.Backend.FailLog("Fail NominateViceMaster", bro);

                    return;
                }
                
                _guildMemberData.Position = "member";
                SetUI();
            });
        }
        else
        {
            Managers.Message.ShowMessage("바꿀 수 없는 학생 입니다.");
        }
    }

    private void OnClickKick()
    {
        if (!Managers.Guild.GuildMemberDatas.TryGetValue(Backend.UserInDate, out var myGuildMemberData))
        {
            Managers.Message.ShowMessage("데이터 오류");
            return;
        }

        if (myGuildMemberData.Position.Equals("member"))
        {
            Managers.Message.ShowMessage("권한이 없습니다.");
            return;
        }

        FadeScreen.Instance.OnLoadingScreen();
        
        Backend.Guild.ExpelMemberV3(_guildMemberData.InDate, bro =>
        {
            FadeScreen.Instance.OffLoadingScreen();

            if (!bro.IsSuccess())
            {
                Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);
                
                if (statusCode.Contains("404") && errorCode.Contains("NotFoundException") && message.Contains("requestedGamer"))
                    Managers.Message.ShowMessage("학교에 존재하지 않는 유저 입니다.");
                else if (statusCode.Contains("403") && errorCode.Contains("ForbiddenError") && message.Contains("Forbidden expelMaster"))
                    Managers.Message.ShowMessage("권한이 없습니다.");
                else if (statusCode.Contains("403") && errorCode.Contains("ForbiddenException") && message.Contains("Forbidden expelMaster"))
                    Managers.Message.ShowMessage("권한이 없습니다.");
                else if (statusCode.Contains("403") && errorCode.Contains("ForbiddenException") && message.Contains("Forbidden expelViceMaster"))
                    Managers.Message.ShowMessage("권한이 없습니다.");
                else
                    Managers.Backend.FailLog("Fail ExpelMember", bro);

                return;
            }

            var param = new Param()
            {
                { "GuildName", Managers.Guild.GuildData != null ? Managers.Guild.GuildData.GuildName : string.Empty },
                { "Type", "Kick" },
                { "Nickname", _guildMemberData.Nickname }
            };

            Backend.GameLog.InsertLog("Guild", param, 30);
            
            MessageBroker.Default.Publish(new GuildExpelMessage(this));
            Managers.Message.ShowMessage("추방 성공");
        });
    }
}