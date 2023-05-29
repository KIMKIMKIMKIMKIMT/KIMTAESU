using System;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildInfoItem : UI_Base
{
    [SerializeField] private TMP_Text GuildNameText;
    [SerializeField] private TMP_Text GuildMemberCountValueText;
    [SerializeField] private TMP_Text GuildMasterNicknameText;
    [SerializeField] private TMP_Text GuildDescText;

    [SerializeField] private Image GuildMarkImage;
    [SerializeField] private Image GuildGradeImage;
    
    [SerializeField] private Button JoinButton;
    
    private GuildData _guildData;

    private void Start()
    {
        JoinButton.BindEvent(OnClickJoin);
    }

    public void Init(GuildData guildData)
    {
        _guildData = guildData;
        SetUI();
    }

    private void SetUI()
    {
        GuildNameText.text = _guildData.GuildName;
        GuildMemberCountValueText.text = $"{_guildData.MemberCount} / {_guildData.GuildMetaData.MemberMaxCount}";
        GuildMasterNicknameText.text = _guildData.MasterNickname;
        GuildDescText.text = _guildData.GuildMetaData.Desc;

        GuildMarkImage.sprite = Managers.Resource.LoadGuildMarkIcon(_guildData.GuildMetaData.Mark);
        GuildGradeImage.sprite = Managers.Resource.LoadGuildGradeIcon(_guildData.GuildMetaData.Grade);
    }

    private void OnClickJoin()
    {
        if (Utils.GetNow() < Managers.Game.UserData.GuildJoinCoolTime)
        {
            var timeGap = Managers.Game.UserData.GuildJoinCoolTime - Utils.GetNow();

            Managers.Message.ShowMessage(timeGap.Hours < 1
                ? $"{timeGap}분 후 가입 신청이 가능합니다."
                : $"{timeGap.Hours}시간 후 가입 신청이 가능합니다.");
            return;
        }
        
        FadeScreen.Instance.OnLoadingScreen();

        Backend.Guild.ApplyGuildV3(_guildData.InDate, bro =>
        {
            FadeScreen.Instance.OffLoadingScreen();

            if (!bro.IsSuccess())
            {
                string statusCode = bro.GetStatusCode();
                string errorCode = bro.GetErrorCode();
                string message = bro.GetMessage();

                // 콘솔 설정 조건에 맞지 않는 유저가 길드 가입 요청 시도한 경우
                if (statusCode.Equals("403") && errorCode.Contains("ForbiddenException") &&
                    message.Contains("Forbidden applyGuild"))
                {
                    Managers.Message.ShowMessage($"{ChartManager.SystemCharts[SystemData.Guild_Apply_Level].Value}레벨 부터 학교에 가입할 수 있습니다.");
                }
                else if (statusCode.Equals("409") && errorCode.Contains("DuplicatedParameterException") &&
                         message.Contains("Duplicated alreadyRequestGamer"))
                {
                    Managers.Message.ShowMessage("이미 가입 신청한 학교 입니다.");
                }
                else if (statusCode.Equals("412") && errorCode.Contains("PreconditionFailed") &&
                         message.Contains("JoinedGamer"))
                {
                    Managers.Message.ShowMessage("이미 가입한 학교가 존재 합니다.");
                }
                // 길드원이 이미 100명 이상인 경우
                else if (statusCode.Equals("429") && errorCode.Contains("Too Many Request") &&
                         message.Contains("guild member count"))
                {
                    Managers.Message.ShowMessage("인원이 모두 찬 학교 입니다");   
                }
                else
                {
                    Managers.Backend.FailLog(bro, "Fail ApplyGuild");
                }

                return;
            }
            
            Managers.Message.ShowMessage("가입 신청 성공");
        });
    }

}