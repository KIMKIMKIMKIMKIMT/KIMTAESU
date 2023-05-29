using System;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildPanel : UI_Panel
{
    enum GuildPanels
    {
        Member,
        Search,
        Management,
        Effect,
        Ranking,
        Raid,
        AllRaid,
        AllRaidRanking,
        GuildSports,
        GuildSportsRanking,
    }
    
    [SerializeField] private Image MarkImage;
    [SerializeField] private Image GradeImage;

    [SerializeField] private TMP_Text GuildNameText;
    [SerializeField] private TMP_Text GuildMasterValueText;
    [SerializeField] private TMP_Text GuildMemberCountText;
    [SerializeField] private TMP_Text GuildDescText;
    [SerializeField] private TMP_Text GxpText;

    [SerializeField] private Button GuildEffectButton;
    [SerializeField] private Button GuildMemberButton;
    [SerializeField] private Button SearchGuildButton;
    [SerializeField] private Button ManagementButton;
    [SerializeField] private Button AttendanceButton;
    [SerializeField] private Button RankingButton;
    [SerializeField] private Button GuildRaidButton;
    [SerializeField] private Button _guildAllRaidBtn;
    [SerializeField] private Button _guildAllRaidRankingBtn;
    [SerializeField] private Button _guildSportsBtn;
    [SerializeField] private Button _guildSportsRankingBtn;

    [SerializeField] private Slider GxpSlider;

    [SerializeField] private List<UI_Panel> UIPanels;

    [SerializeField] private GameObject ManagementRedDotObj;
    [SerializeField] private GameObject AttendanceRedDotObj;

    private GuildData GuildData => Managers.Guild.GuildData;

    private void Start()
    {
        GuildEffectButton.BindEvent(OnClickEffect);
        GuildMemberButton.BindEvent(OnClickGuildMember);
        SearchGuildButton.BindEvent(OnClickSearchGuild);
        ManagementButton.BindEvent(OnClickManagement);
        AttendanceButton.BindEvent(OnClickAttendance);
        RankingButton.BindEvent(OnClickRanking);
        GuildRaidButton.BindEvent(OnClickRaid);
        _guildAllRaidBtn.BindEvent(OnClickAllRaid);
        _guildAllRaidRankingBtn.BindEvent(OnClickAllRaidRanking);
        _guildSportsBtn.BindEvent(OnClickGuildSports);
        _guildSportsRankingBtn.BindEvent(OnClickGuildSportsRanking);

        MessageBroker.Default.Receive<GuildChangeDescMessage>().Subscribe(message =>
        {
            GuildDescText.text = message.Desc;
        });
        
        MessageBroker.Default.Receive<GuildChangeMarkMessage>().Subscribe(message =>
        {
            MarkImage.sprite = Managers.Resource.LoadGuildMarkIcon(message.MarkId);
        });

        ManagementRedDotObj.SetActive(Managers.Guild.GuildApplicantsDatas.Keys.Count > 0);
        
        MessageBroker.Default.Receive<GuildApplicantMessage>().Subscribe(_ =>
        {
            GuildMemberCountText.text = $"{GuildData.MemberCount} / {GuildData.GuildMetaData.MemberMaxCount}";
        });

        MessageBroker.Default.Receive<GuildReceivedGuildApplicantMessage>().Subscribe(_ =>
        {
            ManagementRedDotObj.SetActive(true);
        });

        MessageBroker.Default.Receive<GuildChangeGradeMessage>().Subscribe(message =>
        {
            GradeImage.sprite = Managers.Resource.LoadGuildGradeIcon(message.Grade);
            
            var guildGradeChart = ChartManager.GuildGradeCharts[message.Grade];
            GxpSlider.value = guildGradeChart.GuildNextGrade != 0 ? (float)message.Gxp / guildGradeChart.GuildGradeUpGxp : 1;
            GxpText.text = guildGradeChart.GuildNextGrade != 0 ? $"{message.Gxp}/{guildGradeChart.GuildGradeUpGxp}" : "-";
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (var uiPanel in UIPanels)
            {
                var managementPanel = uiPanel as UI_GuildManagementPanel;

                if (managementPanel != null && managementPanel.ClosePanel()) 
                {
                    return;
                }

                if (!uiPanel.gameObject.activeSelf) 
                    continue;
                
                uiPanel.Close();
                return;
            }
            
            Managers.UI.ClosePopupUI();
        }
    }

    public override void Open()
    {
        base.Open();
        
        CloseAllGuildPanels();
        
        SetUI();
    }

    private void SetUI()
    {
        MarkImage.sprite = Managers.Resource.LoadGuildMarkIcon(GuildData.GuildMetaData.Mark);
        GradeImage.sprite = Managers.Resource.LoadGuildGradeIcon(GuildData.GuildMetaData.Grade);

        GuildNameText.text = GuildData.GuildName;
        GuildMasterValueText.text = $"총장 : {GuildData.MasterNickname}";
        GuildMemberCountText.text = $"인원 : {GuildData.MemberCount} / {GuildData.GuildMetaData.MemberMaxCount}";
        GuildDescText.text = GuildData.GuildMetaData.Desc;

        var guildGradeChart = ChartManager.GuildGradeCharts[Managers.Guild.GuildData.GuildMetaData.Grade];
        
        GxpText.text = guildGradeChart.GuildNextGrade != 0
            ? $"{Managers.Guild.GuildData.GoodsData[1]}/{guildGradeChart.GuildGradeUpGxp}"
            : "-";
        GxpSlider.value = guildGradeChart.GuildNextGrade != 0
            ? (float)Managers.Guild.GuildData.GoodsData[1] / guildGradeChart.GuildGradeUpGxp
            : 1;
        
        AttendanceRedDotObj.SetActive(Managers.Game.UserData.GuildAttendanceTime <= Utils.GetNow());
    }

    private void OnClickEffect()
    {
        if (UIPanels[(int)GuildPanels.Effect].gameObject.activeSelf)
            UIPanels[(int)GuildPanels.Effect].Close();
        else
        {
            CloseAllGuildPanels();
            UIPanels[(int)GuildPanels.Effect].Open();
        }
    }

    private void OnClickGuildMember()
    {
        if (UIPanels[(int)GuildPanels.Member].gameObject.activeSelf)
            UIPanels[(int)GuildPanels.Member].Close();
        else
        {
            CloseAllGuildPanels();
            UIPanels[(int)GuildPanels.Member].Open();
        }
    }

    private void OnClickSearchGuild()
    {
        if (UIPanels[(int)GuildPanels.Search].gameObject.activeSelf)
            UIPanels[(int)GuildPanels.Search].Close();
        else
        {
            CloseAllGuildPanels();
            UIPanels[(int)GuildPanels.Search].Open();
        }
    }

    private void OnClickManagement()
    {
        if (Managers.Guild.GuildData == null)
        {
            Managers.Message.ShowMessage("잘못된 호출 입니다.");
            return;
        }

        if (!Managers.Guild.GuildMemberDatas.TryGetValue(Backend.UserInDate, out var myGuildMemberData))
        {
            Managers.Message.ShowMessage("내 학교 정보가 존재하지 않습니다.");
            return;
        }

        if (myGuildMemberData.Position.Equals("member"))
        {
            Managers.Message.ShowMessage("권한이 없습니다.");
            return;
        }
        
        if (UIPanels[(int)GuildPanels.Management].gameObject.activeSelf)
            UIPanels[(int)GuildPanels.Management].Close();
        else
        {
            CloseAllGuildPanels();
            UIPanels[(int)GuildPanels.Management].Open();
            ManagementRedDotObj.SetActive(false);
        }
    }

    private void CloseAllGuildPanels()
    {
        UIPanels.ForEach(uiPanel => uiPanel.Close());
    }

    private void OnClickAttendance()
    {
        if (Utils.GetNow() < Managers.Game.UserData.GuildAttendanceTime)
        {
            Managers.Message.ShowMessage("이미 출석 했습니다.");
            return;
        }
        
        FadeScreen.Instance.OnLoadingScreen();

        int gxp = (int)ChartManager.SystemCharts[SystemData.Guild_Attendance_Gxp].Value;

        Backend.Guild.ContributeGoodsV3(goodsType.goods1, gxp, bro =>
        {
            if (!bro.IsSuccess())
            {
                Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                if (statusCode.Contains("412"))
                {
                    if (errorCode.Contains("PreconditionFailed"))
                    {
                        if (message.Contains("type 사전 조건")) // amount가 음수인 경우
                            Managers.Message.ShowMessage("잘못된 호출 입니다.");
                        else if (message.Contains("subscribed guild 사전 조건") || message.Contains("notGuildMember"))
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
                }
                else if (statusCode.Contains("400") && errorCode.Contains("BadParameterException") &&
                         message.Contains("bad goodsType"))
                {
                    Managers.Message.ShowMessage("잘못된 호출 입니다.");
                }
                else
                    Managers.Backend.FailLog("Fail ContributeGoods", bro);
                
                FadeScreen.Instance.OffLoadingScreen();
                return;
            }
            
            Managers.Message.ShowMessage("출석 성공!");
            Managers.Game.UserData.GuildAttendanceTime = Utils.GetDay(1);
            GameDataManager.UserGameData.SaveGameData();

            var param = new Param
            {
                { "GuildName", Managers.Guild.GuildData != null ? Managers.Guild.GuildData.GuildName : string.Empty },
                { "Type", "Attendance" }
            };

            Backend.GameLog.InsertLog("Guild", param, 30);

            Managers.Guild.GetMyGuildData(() =>
            {
                FadeScreen.Instance.OffLoadingScreen();
                SetUI();
            }, false);
        });
    }

    private void OnClickRanking()
    {
                
        if (UIPanels[(int)GuildPanels.Ranking].gameObject.activeSelf)
            UIPanels[(int)GuildPanels.Ranking].Close();
        else
        {
            CloseAllGuildPanels();
            UIPanels[(int)GuildPanels.Ranking].Open();
            ManagementRedDotObj.SetActive(false);
        }
    }
    
    
    private void OnClickRaid()
    {
                
        if (UIPanels[(int)GuildPanels.Raid].gameObject.activeSelf)
            UIPanels[(int)GuildPanels.Raid].Close();
        else
        {
            CloseAllGuildPanels();
            UIPanels[(int)GuildPanels.Raid].Open();
            ManagementRedDotObj.SetActive(false);
        }
    }

    private void OnClickAllRaid()
    {
        if(UIPanels[(int)GuildPanels.AllRaid].gameObject.activeSelf)
            UIPanels[(int)GuildPanels.AllRaid].Close();
        else
        {
            CloseAllGuildPanels();
            UIPanels[(int)GuildPanels.AllRaid].Open();
            //ManagementRedDotObj.SetActive(false);
        }
    }

    private void OnClickAllRaidRanking()
    {
        if (UIPanels[(int)GuildPanels.AllRaidRanking].gameObject.activeSelf)
            UIPanels[(int)GuildPanels.AllRaidRanking].Close();
        else
        {
            CloseAllGuildPanels();
            UIPanels[(int)GuildPanels.AllRaidRanking].Open();
            //ManagementRedDotObj.SetActive(false);
        }
    }

    private void OnClickGuildSports()
    {
        if (UIPanels[(int)GuildPanels.GuildSports].gameObject.activeSelf)
            UIPanels[(int)GuildPanels.GuildSports].Close();
        else
        {
            CloseAllGuildPanels();
            UIPanels[(int)GuildPanels.GuildSports].Open();
            //ManagementRedDotObj.SetActive(false);
        }
    }

    private void OnClickGuildSportsRanking()
    {
        if (UIPanels[(int)GuildPanels.GuildSportsRanking].gameObject.activeSelf)
            UIPanels[(int)GuildPanels.GuildSportsRanking].Close();
        else
        {
            CloseAllGuildPanels();
            UIPanels[(int)GuildPanels.GuildSportsRanking].Open();
            //ManagementRedDotObj.SetActive(false);
        }
    }
}