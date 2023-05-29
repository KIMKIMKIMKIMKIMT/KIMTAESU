using BackEnd;
using GameData;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildManagementApplicantItem : UI_Panel
{
    [SerializeField] private TMP_Text LvText;
    [SerializeField] private TMP_Text NicknameText;
    [SerializeField] private TMP_Text PowerText;

    [SerializeField] private Image CostumeImage;
    [SerializeField] private Image PromoImage;

    [SerializeField] private Button AcceptButton;
    [SerializeField] private Button RefuseButton;

    private GuildMemberData _guildMemberData;

    public string InDate => _guildMemberData.InDate;

    private void Start()
    {
        AcceptButton.BindEvent(OnClickAccept);
        RefuseButton.BindEvent(OnClickRefuse);
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

        CostumeImage.sprite =
            Managers.Resource.LoadItemIcon(ItemType.Costume, _guildMemberData.EquipData[EquipType.ShowCostume]);

        if (_guildMemberData.PromoGrade == 0)
            PromoImage.gameObject.SetActive(false);
        else
        {
            PromoImage.gameObject.SetActive(true);
            PromoImage.sprite = Managers.Resource.LoadPromoIcon(_guildMemberData.PromoGrade);
        }
    }

    private void OnClickAccept()
    {
        if (Managers.Guild.GuildData.MemberCount >= Managers.Guild.GuildData.GuildMetaData.MemberMaxCount)
        {
            Managers.Message.ShowMessage("학교 인원이 가득찼습니다.");
            return;
        }

        if (!Managers.Guild.GuildMemberDatas.TryGetValue(Backend.UserInDate, out var myGuildMemberData))
        {
            Managers.Message.ShowMessage("데이터 오류 입니다.");
            return;
        }

        if (!myGuildMemberData.Position.Contains("master") && myGuildMemberData.Position.Contains("vice"))
        {
            Managers.Message.ShowMessage("권한이 없습니다.");
            return;
        }

        // 다른 운영진이 수락해서 인원수가 지금 캐싱되있는 정보가 다를 수 있음
        // 수락 전 검증을 위해 데이터를 가져와야함
        Managers.Guild.GetMyGuildData(() =>
        {
            if (!Managers.Guild.GuildMemberDatas.TryGetValue(Backend.UserInDate, out var myGuildMemberData))
            {
                Managers.Message.ShowMessage("데이터 오류 입니다.");
                return;
            }

            if (!myGuildMemberData.Position.Contains("master") && myGuildMemberData.Position.Contains("vice"))
            {
                Managers.Message.ShowMessage("권한이 없습니다.");
                return;
            }

            if (Managers.Guild.GuildData.MemberCount >= Managers.Guild.GuildData.GuildMetaData.MemberMaxCount)
            {
                Managers.Message.ShowMessage("학교 인원이 가득찼습니다.");
                return;
            }

            FadeScreen.Instance.OnLoadingScreen();

            Backend.Guild.ApproveApplicantV3(_guildMemberData.InDate, bro =>
            {
                if (!bro.IsSuccess())
                {
                    Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                    if (statusCode.Contains("412") && errorCode.Contains("PreconditionFailed") &&
                        message.Contains("JoinedGamer"))
                        Managers.Message.ShowMessage("학교가 있는 유저 입니다.");
                    else if (statusCode.Contains("429") && errorCode.Contains("Too Many Request") &&
                             message.Contains("guild member count"))
                        Managers.Message.ShowMessage("학교 인원이 가득찼습니다.");
                    else
                        Managers.Backend.FailLog("Fail ApproveApplicant", bro);

                    FadeScreen.Instance.OffLoadingScreen();
                    return;
                }

                var param = new Param
                {
                    {
                        "GuildName",
                        Managers.Guild.GuildData != null ? Managers.Guild.GuildData.GuildName : string.Empty
                    },
                    { "Type", "Join" },
                    { "Nickname", _guildMemberData.Nickname }
                };

                Backend.GameLog.InsertLog("Guild", param, 30);

                FadeScreen.Instance.OffLoadingScreen();
                Managers.Guild.GuildData.MemberCount += 1;
                MessageBroker.Default.Publish(new GuildApplicantMessage(this, true));
            });
        }, false);
    }

    private void OnClickRefuse()
    {
        if (!Managers.Guild.GuildMemberDatas.TryGetValue(Backend.UserInDate, out var myGuildMemberData))
        {
            Managers.Message.ShowMessage("데이터 오류 입니다.");
            return;
        }

        if (!myGuildMemberData.Position.Contains("master") && myGuildMemberData.Position.Contains("vice"))
        {
            Managers.Message.ShowMessage("권한이 없습니다.");
            return;
        }

        FadeScreen.Instance.OnLoadingScreen();

        Backend.Guild.RejectApplicantV3(_guildMemberData.InDate, bro =>
        {
            FadeScreen.Instance.OffLoadingScreen();

            if (!bro.IsSuccess())
            {
                Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                if (statusCode.Contains("404") && errorCode.Contains("NotFoundException") &&
                    message.Contains("requestedGamer not found"))
                {
                    Managers.Message.ShowMessage("존재하지 않는 유저 입니다.");
                }
                else
                {
                    Managers.Backend.FailLog("Fail RejectApplicant", bro);
                    return;
                }
            }

            MessageBroker.Default.Publish(new GuildApplicantMessage(this, false));
        });
    }
}