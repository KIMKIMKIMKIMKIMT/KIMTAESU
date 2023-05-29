using System;
using System.Linq;
using BackEnd;
using Chart;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildManagementPanel : UI_Panel
{
    [Serializable]
    public struct Tab
    {
        public Button TabButton;
        public UI_Panel UIPanel;
    }

    [SerializeField] private Tab[] Tabs;

    [SerializeField] private TMP_Text MasterNicknameText;
    [SerializeField] private TMP_Text MemberCountValueText;
    [SerializeField] private TMP_Text GxpText;

    [SerializeField] private Slider GxpSlider;

    [SerializeField] private TMP_InputField DescInputField;

    [SerializeField] private Image MarkImage;
    [SerializeField] private Image GradeImage;

    [SerializeField] private Button ChangeDescButton;
    [SerializeField] private Button GradeUpButton;

    [SerializeField] private GameObject ApplicantsRedDotObj;

    private void Start()
    {
        foreach (var tab in Tabs)
        {
            tab.TabButton.BindEvent(tab.UIPanel.Open);
            tab.UIPanel.Close();
        }
        
        ChangeDescButton.BindEvent(OnChangeDesc);
        GradeUpButton.BindEvent(OnClickGradeUp);

        MessageBroker.Default.Receive<GuildChangeMarkMessage>().Subscribe(message =>
        {
            MarkImage.sprite = Managers.Resource.LoadGuildMarkIcon(message.MarkId);
        });
        
        MessageBroker.Default.Receive<GuildApplicantMessage>().Subscribe(_ =>
        {
            MemberCountValueText.text = $"{Managers.Guild.GuildData.MemberCount} / {Managers.Guild.GuildData.GuildMetaData.MemberMaxCount}";
        });

        MessageBroker.Default.Receive<GuildReceivedGuildApplicantMessage>().Subscribe(_ =>
        {
            ApplicantsRedDotObj.SetActive(true);
        });
        
        MessageBroker.Default.Receive<OpenPanelMessage<UI_GuildManagementApplicantsPanel>>().Subscribe(_ =>
        {
            ApplicantsRedDotObj.SetActive(false);
        });
    }

    public bool ClosePanel()
    {
        foreach (var tab in Tabs)
        {
            if (tab.UIPanel.gameObject.activeSelf)
            {
                tab.UIPanel.Close();
                return true;
            }
        }

        return false;
    }

    public override void Open()
    {
        base.Open();

        SetUI();
    }

    private void OnDisable()
    {
        foreach (var tab in Tabs)
            tab.UIPanel.Close();
    }

    private void SetUI()
    {
        GuildGradeChart guildGradeChart = ChartManager.GuildGradeCharts[Managers.Guild.GuildData.GuildMetaData.Grade];

        MasterNicknameText.text = Managers.Guild.GuildData.MasterNickname;
        MemberCountValueText.text =
            $"{Managers.Guild.GuildData.MemberCount} / {Managers.Guild.GuildData.GuildMetaData.MemberMaxCount}";

        GxpText.text = guildGradeChart.GuildNextGrade != 0
            ? $"{Managers.Guild.GuildData.GoodsData[1]}/{guildGradeChart.GuildGradeUpGxp}"
            : "-";
        GxpSlider.value = guildGradeChart.GuildNextGrade != 0
            ? (float)Managers.Guild.GuildData.GoodsData[1] / guildGradeChart.GuildGradeUpGxp
            : 1;

        DescInputField.text = Managers.Guild.GuildData.GuildMetaData.Desc;
        DescInputField.readOnly = !Managers.Guild.GuildMemberDatas[Backend.UserInDate].Position.Equals("master");

        MarkImage.sprite = Managers.Resource.LoadGuildMarkIcon(Managers.Guild.GuildData.GuildMetaData.Mark);
        GradeImage.sprite = Managers.Resource.LoadGuildGradeIcon(Managers.Guild.GuildData.GuildMetaData.Grade);
        
        ApplicantsRedDotObj.SetActive(Managers.Guild.GuildApplicantsDatas.Keys.Count > 0);
    }

    private void OnChangeDesc()
    {
        string desc = DescInputField.text;

        if (!Managers.Guild.GuildMemberDatas.TryGetValue(Backend.UserInDate, out var myGuildMemberData))
        {
            Managers.Message.ShowMessage("데이터 오류");
            return;
        }

        if (!myGuildMemberData.Position.Equals("master"))
        {
            Managers.Message.ShowMessage("권한이 없습니다.");
            return;
        }

        if (Managers.Guild.GuildData.GuildMetaData.ChangeDescCoolTime != null &&
            Utils.GetNow() < Managers.Guild.GuildData.GuildMetaData.ChangeDescCoolTime)
        {
            var timeGap = Managers.Guild.GuildData.GuildMetaData.ChangeDescCoolTime - Utils.GetNow();
            
            
            Managers.Message.ShowMessage(timeGap.Value.Hours > 0 ? 
                $"{timeGap.Value.Hours}시간 후 변경 가능합니다." :
                $"{timeGap.Value.Minutes}분 후 변경 가능합니다.");
            return;
        }

        if (string.IsNullOrEmpty(desc))
        {
            Managers.Message.ShowMessage("소개글을 입력하세요.");
            return;
        }

        if (desc.Length < 2 || desc.Length > 60)
        {
            Managers.Message.ShowMessage("소개글은 2~60자 내로 설정 가능합니다.");
            return;
        }

        if (desc == Managers.Guild.GuildData.GuildMetaData.Desc)
            return;

        var changeDescCoolTime = Utils.GetNow().AddMinutes(10);

        var param = new Param()
        {
            { "Desc", desc },
            { "ChangeDescCoolTime", changeDescCoolTime }
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

            Managers.Guild.GuildData.GuildMetaData.Desc = desc;
            Managers.Guild.GuildData.GuildMetaData.ChangeDescCoolTime = changeDescCoolTime;
            
            Managers.Message.ShowMessage("설정 완료");
            MessageBroker.Default.Publish(new GuildChangeDescMessage(desc));
        });
    }

    private void OnClickGradeUp()
    {
        if (!ChartManager.GuildGradeCharts.TryGetValue(Managers.Guild.GuildData.GuildMetaData.Grade,
                out var guildGradeChart))
        {
            Debug.LogError($"Fail Load GuildGradeChart : {Managers.Guild.GuildData.GuildMetaData.Grade}");
            Managers.Message.ShowMessage("데이터 오류");
            return;
        }

        if (guildGradeChart.GuildNextGrade == 0)
        {
            Managers.Message.ShowMessage("최대 등급 입니다.");
            return;
        }

        if (!Managers.Guild.GuildData.GoodsData.TryGetValue((int)GuildGoodsType.Gxp, out int goodsValue))
        {
            Managers.Message.ShowMessage("데이터 오류");
            return;
        }

        if (goodsValue < guildGradeChart.GuildGradeUpGxp)
        {
            Managers.Message.ShowMessage("학교 경험치가 부족합니다.");
            return;
        }

        if (!Managers.Guild.GuildMemberDatas.TryGetValue(Backend.UserInDate, out var myGuildMemberData))
        {
            Managers.Message.ShowMessage("데이터 오류");
            return;
        }

        if (!myGuildMemberData.Position.Equals("master"))
        {
            Managers.Message.ShowMessage("권한이 없습니다.");
            return;
        }

        FadeScreen.Instance.OnLoadingScreen();
        
        Backend.Guild.UseGoodsV3(goodsType.goods1, -guildGradeChart.GuildGradeUpGxp, OnBackendUseGoods);

        void OnBackendUseGoods(BackendReturnObject bro)
        {
            if (!bro.IsSuccess())
            {
                Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                if (statusCode.Contains("403") && errorCode.Contains("ForbiddenException") &&
                    message.Contains("Forbidden useGoods")) // 마스터 이외의 길드원이 사용 시도한 경우
                {
                    Managers.Message.ShowMessage("권한이 없습니다.");
                }
                else if (statusCode.Contains("412") && errorCode.Contains("PreconditionFailed"))
                {
                    if (message.Contains("type 사전 조건을 만족하지 않습니다.")) // amount값이 양수인 경우
                    {
                        Managers.Message.ShowMessage("데이터 오류 입니다.");
                    }
                    else if (message.Contains("inadequateAmount")) // 사용량이 보유량보다 큰 경우
                    {
                        Managers.Message.ShowMessage("학교 경험치가 부족합니다.");
                    }
                    else if (message.Contains("notGuildMember")) // 길드에 속해있지 않은 사람이 시도한 경우
                    {
                        Managers.Message.ShowMessage("잘못된 호출 입니다.");
                    }
                }
                else
                {
                    Managers.Backend.FailLog("Fail UseGoods", bro);
                }

                FadeScreen.Instance.OffLoadingScreen();
                return;
            }

            var param = new Param()
            {
                { "Grade", Managers.Guild.GuildData.GuildMetaData.Grade + 1 }
            };

            Backend.Guild.ModifyGuildV3(param, OnBackendModifyGuild);
        }
        
        void OnBackendModifyGuild(BackendReturnObject bro)
        {
            if (!bro.IsSuccess())
            {
                Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);
                    
                if ((statusCode.Contains("400") && errorCode.Contains("BadParameterException") && message.Contains("bad guildName")) 
                    || (statusCode.Contains("412") && errorCode.Contains("PreconditionFailed") && message.Contains("subscribed guild")))
                    Managers.Message.ShowMessage("잘못된 호출 입니다.");
                else
                {
                    Managers.Backend.FailLog("Fail ModifyGuild", bro);
                }
                    
                FadeScreen.Instance.OffLoadingScreen();
                return;
            }

            Managers.Guild.GuildData.GuildMetaData.Grade += 1;
            Managers.Guild.GetMyGuildGoodsData(OnGetMyGuildGoodsData);
        }

        void OnGetMyGuildGoodsData()
        {
            FadeScreen.Instance.OffLoadingScreen();
            Managers.Message.ShowMessage("학교 등급업 성공!");
            SetUI();
            MessageBroker.Default.Publish(new GuildChangeGradeMessage(
                Managers.Guild.GuildData.GuildMetaData.Grade, 
                Managers.Guild.GuildData.GoodsData[(int)GuildGoodsType.Gxp]));
        }
    }
}