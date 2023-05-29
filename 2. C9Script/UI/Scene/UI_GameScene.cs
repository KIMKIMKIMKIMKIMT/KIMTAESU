using System;
using BackEnd;
using DG.Tweening;
using TMPro;
using UI;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class UI_GameScene : UI_Scene
{
    enum TopButton
    {
        Package,
        Mission,
    }

    enum BottomButton
    {
        Character,
        Skill,
        Summon,
        Chulwadae,
        Dungeon,
        Shop,
        Guild,
    }

    enum SideMenus
    {
        Ranking,
        Quest,
        Attendance,
        Post,
        Coupon,
        Setting,
    }

    [Serializable]
    public record BottomMenu
    {
        public GameObject InfoObj;
        public GameObject CloseObj;
        public Button MenuButton;
    }

    [Serializable]
    public record GuideQuestUIBundle
    {
        public TMP_Text GuideQuestText;
        public TMP_Text GuideQuestProgressText;

        public Image RewardItemImage;
        public TMP_Text RewardItemValueText;

        public Button ClearButton;

        public GameObject UIObj;
    }

    [Serializable]
    public record PowerUIBundle
    {
        public Image PromoGradeImage;
        public TMP_Text PowerText;
        public GameObject UIObj;
    }

    [Serializable]
    public record RaidUIBundle
    {
        public TMP_Text StepText;
        public TMP_Text TimeText;
        public TMP_Text GoldText;
        public TMP_Text GoldBarText;
        public TMP_Text SkillStoneText;
    }

    [SerializeField] private TMP_Text GoldText;
    [SerializeField] private TMP_Text StarBalloonText;
    [SerializeField] private TMP_Text LevelText;
    [SerializeField] private TMP_Text GainGoldText;
    [SerializeField] private TMP_Text GainStarBalloonText;
    [SerializeField] private TMP_Text PvpMyNicknameText;
    [SerializeField] private TMP_Text PvpMyTotalDamageText;
    [SerializeField] private TMP_Text PvpEnemyNicknameText;
    [SerializeField] private TMP_Text PvpEnemyTotalDamageText;
    [SerializeField] private TMP_Text PvpRemainTimeText;
    [SerializeField] private TMP_Text PvpTicketText;
    [SerializeField] private TMP_Text XMasScoreValueText;

    [Header("GuildSports")]
    [SerializeField] private TMP_Text MyGuildName;
    [SerializeField] private TMP_Text EnemyGuildName;
    
    [SerializeField] private TMP_Text MyGuildDpsValue;
    [SerializeField] private TMP_Text EnemyGuildDpsValue;
    [SerializeField] private TMP_Text GuildSportsRemainTimeTxt;

    [SerializeField] private Slider ExpSlider;

    [SerializeField] private Button SideMenuButton;
    [SerializeField] private Button CloseSideMenuButton;
    [SerializeField] private Button ChatButton;
    [SerializeField] private Button AdBuffButton;
    [SerializeField] private Button MoveStageButton;
    [SerializeField] private Button PvpAutoMatchCancelButton;
    [SerializeField] private Button RaidGiveUpButton;
    [SerializeField] private Button GuildAllRaidGiveUpButton;
    [SerializeField] private Button GrowthBuffButton;

    [SerializeField] private Button[] SideMenuButtons;

    [SerializeField] private Button[] BottomButtons;
    [SerializeField] private BottomMenu[] BottomMenus;

    [SerializeField] private Button[] TopButtons;

    [SerializeField] private Image PvpMyPromoGradeImage;
    [SerializeField] private Image PvpEnemyPromoGradeImage;

    [SerializeField] private GameObject SideMenuObj;
    [SerializeField] private GameObject GoodsPanelObj;
    [SerializeField] private GameObject BottomPanelObj;
    [SerializeField] private GameObject InGamePanelObj;
    [SerializeField] private GameObject TopPanelObj;
    [SerializeField] private GameObject TopPanelNormalObj;
    [SerializeField] private GameObject TopPanelPvpObj;
    [SerializeField] private GameObject TopPanelRaidObj;
    [SerializeField] private GameObject TopPanelXMasObj;
    [SerializeField] private CanvasGroup LvUpEffectCanvas;
    [SerializeField] private GameObject AutoMatchObj;
    [SerializeField] private GameObject TopPanelGuildSportsObj;

    [SerializeField] private GuideQuestUIBundle GuideQuestUI;
    [SerializeField] private PowerUIBundle PowerUI;
    [SerializeField] private RaidUIBundle RaidUI;
    [SerializeField] private Button EventDungeonButton;
    [SerializeField] private Button HotTimeButton;

    [SerializeField] private GameObject FeverNavigationObj;
    [SerializeField] private GameObject AutoFeverNavigationObj;
    [SerializeField] private GameObject BottomMenuNavigationObj;
    [SerializeField] private GameObject MissionNavigationObj;
    [SerializeField] private GameObject SideMenuNavigationObj;
    [SerializeField] private GameObject CompleteGuideQuestNavigationObj;

    [SerializeField] private Vector2[] BottomMenuNavigationPositions;

    [SerializeField] private Image GuideQuestBgImage;
    [SerializeField] private Sprite GuideQuestBgSprite;
    [SerializeField] private Sprite CompleteGuideQuestBgSprite;

    private Tween _gainGoldTween;
    private Tween _gainStarBalloonTween;

    private float PowerSaveModeTime;
    
    private CompositeDisposable _guideComposite = new();

    private void Start()
    {
        SetButtonEvent();
        SetPropertyEvent();

        SideMenuObj.SetActive(false);
    }

    private void SetButtonEvent()
    {
        SideMenuButton.BindEvent(OnClickSideMenu);
        CloseSideMenuButton.BindEvent(OnClickCloseSideMenu);
        ChatButton.BindEvent(OnClickChatButton);

        #region Receive OpenPopupMessage

        MessageBroker.Default.Receive<OpenPopupMessage<UI_CharacterPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Character].CloseObj.SetActive(true);
            BottomMenus[(int)BottomButton.Character].InfoObj.SetActive(false);
        });

        MessageBroker.Default.Receive<OpenPopupMessage<UI_SkillPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Skill].CloseObj.SetActive(true);
            BottomMenus[(int)BottomButton.Skill].InfoObj.SetActive(false);
        });

        MessageBroker.Default.Receive<OpenPopupMessage<UI_SummonPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Summon].CloseObj.SetActive(true);
            BottomMenus[(int)BottomButton.Summon].InfoObj.SetActive(false);
        });

        MessageBroker.Default.Receive<OpenPopupMessage<UI_ChulwadaePopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Chulwadae].CloseObj.SetActive(true);
            BottomMenus[(int)BottomButton.Chulwadae].InfoObj.SetActive(false);
        });

        MessageBroker.Default.Receive<OpenPopupMessage<UI_DungeonPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Dungeon].CloseObj.SetActive(true);
            BottomMenus[(int)BottomButton.Dungeon].InfoObj.SetActive(false);
        });

        MessageBroker.Default.Receive<OpenPopupMessage<UI_ShopPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Shop].CloseObj.SetActive(true);
            BottomMenus[(int)BottomButton.Shop].InfoObj.SetActive(false);
        });

        MessageBroker.Default.Receive<OpenPopupMessage<UI_GuildPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Guild].CloseObj.SetActive(true);
            BottomMenus[(int)BottomButton.Guild].InfoObj.SetActive(false);
        });

        #endregion

        #region Receive ClosePopupMessage

        MessageBroker.Default.Receive<ClosePopupMessage<UI_CharacterPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Character].CloseObj.SetActive(false);
            BottomMenus[(int)BottomButton.Character].InfoObj.SetActive(true);
        });

        MessageBroker.Default.Receive<ClosePopupMessage<UI_SkillPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Skill].CloseObj.SetActive(false);
            BottomMenus[(int)BottomButton.Skill].InfoObj.SetActive(true);
        });

        MessageBroker.Default.Receive<ClosePopupMessage<UI_SummonPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Summon].CloseObj.SetActive(false);
            BottomMenus[(int)BottomButton.Summon].InfoObj.SetActive(true);
        });

        MessageBroker.Default.Receive<ClosePopupMessage<UI_ChulwadaePopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Chulwadae].CloseObj.SetActive(false);
            BottomMenus[(int)BottomButton.Chulwadae].InfoObj.SetActive(true);
        });

        MessageBroker.Default.Receive<ClosePopupMessage<UI_DungeonPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Dungeon].CloseObj.SetActive(false);
            BottomMenus[(int)BottomButton.Dungeon].InfoObj.SetActive(true);
        });

        MessageBroker.Default.Receive<ClosePopupMessage<UI_ShopPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Shop].CloseObj.SetActive(false);
            BottomMenus[(int)BottomButton.Shop].InfoObj.SetActive(true);
        });

        MessageBroker.Default.Receive<ClosePopupMessage<UI_GuildPopup>>().Subscribe(_ =>
        {
            BottomMenus[(int)BottomButton.Guild].CloseObj.SetActive(false);
            BottomMenus[(int)BottomButton.Guild].InfoObj.SetActive(true);
        });

        #endregion

        BottomMenus[(int)BottomButton.Character].MenuButton.BindEvent(OnClickMenu<UI_CharacterPopup>);
        BottomMenus[(int)BottomButton.Skill].MenuButton.BindEvent(OnClickMenu<UI_SkillPopup>);
        BottomMenus[(int)BottomButton.Summon].MenuButton.BindEvent(OnClickMenu<UI_SummonPopup>);
        BottomMenus[(int)BottomButton.Chulwadae].MenuButton.BindEvent(OnClickMenu<UI_ChulwadaePopup>);
        BottomMenus[(int)BottomButton.Dungeon].MenuButton.BindEvent(OnClickMenu<UI_DungeonPopup>);
        BottomMenus[(int)BottomButton.Shop].MenuButton.BindEvent(OnClickMenu<UI_ShopPopup>);
        BottomMenus[(int)BottomButton.Guild].MenuButton.BindEvent(OnClickMenu<UI_GuildPopup>);

        SideMenuButtons[(int)SideMenus.Ranking].BindEvent(OnClickMenu<UI_RankingPopup>);
        SideMenuButtons[(int)SideMenus.Quest].BindEvent(OnClickMenu<UI_QuestPopup>);
        SideMenuButtons[(int)SideMenus.Attendance].BindEvent(OnClickMenu<UI_AttendancePopup>);
        SideMenuButtons[(int)SideMenus.Post].BindEvent(OnClickMenu<UI_PostPopup>);
        SideMenuButtons[(int)SideMenus.Coupon].BindEvent(OnClickMenu<UI_CouponPopup>);
        SideMenuButtons[(int)SideMenus.Setting].BindEvent(OnClickMenu<UI_SettingPopup>);

        TopButtons[(int)TopButton.Package].BindEvent(OnClickMenu<UI_EventAttendancePopup>);
        TopButtons[(int)TopButton.Mission].BindEvent(OnClickMenu<UI_MissionPopup>);

        AdBuffButton.BindEvent(OnClickMenu<UI_AdBuffPopup>);
        MoveStageButton.BindEvent(OnClickMenu<UI_StagePopup>);

        GuideQuestUI.ClearButton.BindEvent(OnClickClearGuideQuest);

        EventDungeonButton.BindEvent(() =>
        {
            var dungeonPopup = Managers.UI.ShowPopupUI<UI_DungeonPopup>();
            dungeonPopup.SetEventTab();
        });

        HotTimeButton.BindEvent(() => { Managers.UI.ShowPopupUI<UI_HotTimePopup>(); });

        HotTimeButton.image.color =
            Managers.Game.IsHotTime ? Color.white : new Color(100 / 255f, 100 / 255f, 100 / 255f);
        Managers.Game.OnChangeIsHotTime += isHotTime =>
        {
            HotTimeButton.image.color = isHotTime ? Color.white : new Color(100 / 255f, 100 / 255f, 100 / 255f);
        };

        PvpAutoMatchCancelButton.BindEvent(() => Managers.Pvp.IsAutoMatch.Value = false);

        RaidGiveUpButton.BindEvent(() =>
        {
            if (Managers.Stage.State.Value == StageState.Raid)
                Managers.Raid.Fail();
            else if (Managers.Stage.State.Value == StageState.GuildRaid)
                Managers.GuildRaid.Fail();
        });

        GuildAllRaidGiveUpButton.BindEvent(() => Managers.AllRaid.Fail());
        
        GrowthBuffButton.BindEvent(() => Managers.UI.ShowPopupUI<UI_GrowthBuffPopup>());
    }

    private void OnClickMenu<T>() where T : UI_Popup
    {
        if (Managers.Stage.State.Value != StageState.Normal && Managers.Stage.State.Value != StageState.StageBoss)
        {
            Managers.Message.ShowMessage(MessageType.FailDungeonUIByStageState);
            return;
        }

        OnClickCloseSideMenu();

        var popup = Managers.UI.FindPopup<T>();

        if (popup == null || !popup.gameObject.activeSelf)
        {
            Managers.UI.ClosePopupUI();
            Managers.UI.ShowPopupUI<T>();
        }
        else
        {
            Managers.UI.ClosePopupUI();
        }
    }

    private void SetPropertyEvent()
    {
        Managers.Game.GoodsDatas[(int)Goods.Gold].Subscribe(gold => { GoldText.text = gold.ToCurrencyString(); });

        Managers.Game.GoodsDatas[(int)Goods.StarBalloon].Subscribe(starBalloon =>
        {
            StarBalloonText.text = starBalloon.ToCurrencyString();
        });

        Managers.Game.GoodsDatas[(int)Goods.Exp].Subscribe(_ =>
        {
            float ratio = Managers.Game.NextLevelExp <= 0
                ? 1
                : (float)(Managers.Game.GoodsDatas[(int)Goods.Exp].Value / Managers.Game.NextLevelExp);
            ExpSlider.value = ratio;
        });

        LevelText.text = $"Lv.{Managers.Game.UserData.Level}";
        Managers.Game.UserData.OnChangeLevel.Subscribe(level =>
        {
            LevelText.text = $"Lv.{level}";

            if (Managers.Game.IsPlaying)
            {
                var sequence = DOTween.Sequence().Append(LvUpEffectCanvas.transform.DOScale(0f, 0f))
                    .Append(LvUpEffectCanvas.transform.DOScale(1.2f, 0.2f))
                    .Append(LvUpEffectCanvas.transform.DOScale(1f, 0.05f))
                    .AppendInterval(0.5f).Append(LvUpEffectCanvas.DOFade(0f, 0.5f));

                sequence.onComplete = () =>
                {
                    DOTween.Sequence().Append(LvUpEffectCanvas.transform.DOScale(0, 0))
                        .Append(LvUpEffectCanvas.DOFade(1f, 0));
                };
            }
        }).AddTo(gameObject);

        Managers.Game.GainGold.Subscribe(gold =>
        {
            if (gold == 0)
            {
                GainGoldText.text = string.Empty;
                var pos = GainGoldText.transform.localPosition;
                pos.y = -50;
                GainGoldText.DOFade(1, 0);
                GainGoldText.transform.localPosition = pos;
            }
            else
            {
                GainGoldText.text = $"+{gold.ToCurrencyString()}";

                if (_gainGoldTween == null)
                {
                    _gainGoldTween = DOTween.Sequence().PrependInterval(0.5f)
                        .Append(GainGoldText.transform.DOLocalMoveY(0, 1f)).Join(GainGoldText.DOFade(0.5f, 1f));
                    _gainGoldTween.onComplete += () =>
                    {
                        _gainGoldTween = null;
                        Managers.Game.GainGold.Value = 0;
                    };
                }
            }
        });

        Managers.Game.GainStarBalloon.Subscribe(starBalloon =>
        {
            if (starBalloon == 0)
            {
                GainStarBalloonText.text = string.Empty;
                if (GainStarBalloonText.transform != null)
                {
                    var pos = GainStarBalloonText.transform.localPosition;
                    pos.y = -50;
                    GainStarBalloonText.DOFade(1, 0);
                    GainStarBalloonText.transform.localPosition = pos;
                }
            }
            else
            {
                GainStarBalloonText.text = $"+{starBalloon.ToCurrencyString()}";

                if (_gainStarBalloonTween == null)
                {
                    _gainStarBalloonTween = DOTween.Sequence().PrependInterval(0.5f)
                        .Append(GainStarBalloonText.transform.DOLocalMoveY(0, 1f))
                        .Join(GainStarBalloonText.DOFade(0.5f, 1.2f));
                    _gainStarBalloonTween.onComplete += () =>
                    {
                        _gainStarBalloonTween = null;
                        Managers.Game.GainStarBalloon.Value = 0;
                    };
                }
            }
        });

        Managers.XMasEvent.Score.Subscribe(score => { XMasScoreValueText.text = score.ToCurrencyString(); });

        Managers.Stage.State.Subscribe(state =>
        {
            TopPanelRaidObj.SetActive(state == StageState.Raid || state == StageState.GuildRaid);
            TopPanelXMasObj.SetActive(state == StageState.XMasEvent);

            if (state == StageState.XMasEvent)
            {
                GoodsPanelObj.SetActive(false);
                BottomPanelObj.SetActive(true);
                InGamePanelObj.SetActive(false);
                TopPanelNormalObj.SetActive(false);
                TopPanelPvpObj.SetActive(false);
                TopPanelRaidObj.SetActive(false);
                TopPanelGuildSportsObj.SetActive(false);
            }
            else if (state == StageState.Raid || state == StageState.GuildRaid)
            {
                GoodsPanelObj.SetActive(true);
                BottomPanelObj.SetActive(true);
                InGamePanelObj.SetActive(true);
                TopPanelNormalObj.SetActive(false);
                TopPanelPvpObj.SetActive(false);
                TopPanelGuildSportsObj.SetActive(false);
                //EventDungeonButton.gameObject.SetActive(Managers.Stage.State.Value == StageState.Normal || Managers.Stage.State.Value == StageState.StageBoss);
            }
            else if (state == StageState.Pvp)
            {
                GoodsPanelObj.SetActive(false);
                BottomPanelObj.SetActive(false);
                InGamePanelObj.SetActive(false);
                TopPanelNormalObj.SetActive(false);
                TopPanelPvpObj.SetActive(true);
                TopPanelGuildSportsObj.SetActive(false);

                PvpMyNicknameText.text = Backend.UserNickName;
                PvpEnemyNicknameText.text = Managers.Pvp.EnemyRankData.Nickname;

                PvpMyTotalDamageText.text = "0";
                PvpEnemyTotalDamageText.text = "0";

                if (Managers.Game.UserData.PromoGrade == 0)
                {
                    PvpMyPromoGradeImage.gameObject.SetActive(false);
                }
                else
                {
                    PvpMyPromoGradeImage.gameObject.SetActive(true);
                    PvpMyPromoGradeImage.sprite = Managers.Resource.LoadPromoIcon(Managers.Game.UserData.PromoGrade);
                }

                if (Managers.Pvp.EnemyPromoGrade == 0)
                    PvpEnemyPromoGradeImage.gameObject.SetActive(false);
                else
                {
                    PvpEnemyPromoGradeImage.gameObject.SetActive(true);
                    PvpEnemyPromoGradeImage.sprite = Managers.Resource.LoadPromoIcon(Managers.Pvp.EnemyPromoGrade);
                }
            }
            else if (state == StageState.GuildSports)
            {
                GoodsPanelObj.SetActive(false);
                BottomPanelObj.SetActive(false);
                InGamePanelObj.SetActive(false);
                TopPanelNormalObj.SetActive(false);
                TopPanelPvpObj.SetActive(false);
                TopPanelGuildSportsObj.SetActive(true);

                MyGuildName.text = Managers.Guild.GuildData.GuildName;
                EnemyGuildName.text = Managers.Guild.EnemyGuild.GuildName;

                MyGuildDpsValue.text = "0";
                EnemyGuildDpsValue.text = "0";
            }
            else if (state == StageState.Dps)
            {
                GoodsPanelObj.SetActive(true);
                BottomPanelObj.SetActive(true);
                InGamePanelObj.SetActive(true);
                TopPanelNormalObj.SetActive(false);
                TopPanelPvpObj.SetActive(false);
                TopPanelGuildSportsObj.SetActive(false);
            }
            else
            {
                GoodsPanelObj.SetActive(true);
                BottomPanelObj.SetActive(true);
                InGamePanelObj.SetActive(true);
                TopPanelNormalObj.SetActive(true);
                TopPanelPvpObj.SetActive(false);
                TopPanelGuildSportsObj.SetActive(false);
                TopPanelObj.SetActive(Managers.Stage.State.Value != StageState.Dungeon && !Utils.IsWorldCupDungeon() && !Utils.IsGuildAllRaidDungeon());
                //EventDungeonButton.gameObject.SetActive(Managers.Stage.State.Value == StageState.Normal || Managers.Stage.State.Value == StageState.StageBoss);
            }
        });

        Managers.Pvp.OnChangePvpEnemy += () =>
        {
            PvpMyNicknameText.text = Backend.UserNickName;
            PvpEnemyNicknameText.text = Managers.Pvp.EnemyRankData.Nickname;

            PvpMyTotalDamageText.text = "0";
            PvpEnemyTotalDamageText.text = "0";

            if (Managers.Game.UserData.PromoGrade == 0)
            {
                PvpMyPromoGradeImage.gameObject.SetActive(false);
            }
            else
            {
                PvpMyPromoGradeImage.gameObject.SetActive(true);
                PvpMyPromoGradeImage.sprite = Managers.Resource.LoadPromoIcon(Managers.Game.UserData.PromoGrade);
            }

            if (Managers.Pvp.EnemyPromoGrade == 0)
                PvpEnemyPromoGradeImage.gameObject.SetActive(false);
            else
            {
                PvpEnemyPromoGradeImage.gameObject.SetActive(true);
                PvpEnemyPromoGradeImage.sprite = Managers.Resource.LoadPromoIcon(Managers.Pvp.EnemyPromoGrade);
            }
        };

        Managers.Pvp.PvpRemainTime.Where(_ => Managers.Stage.State.Value == StageState.Pvp).Subscribe(remainTime =>
        {
            PvpRemainTimeText.text = ((int)remainTime.TotalSeconds).ToString();
        });

        Managers.Pvp.TotalMyDamage.Where(_ => Managers.Stage.State.Value == StageState.Pvp).Subscribe(damage =>
        {
            PvpMyTotalDamageText.text = damage.ToCurrencyString();
        }).AddTo(gameObject);

        Managers.Pvp.TotalEnemyDamage.Where(_ => Managers.Stage.State.Value == StageState.Pvp).Subscribe(damage =>
        {
            PvpEnemyTotalDamageText.text = damage.ToCurrencyString();
        });

        Managers.GuildSports.MyGuildDps.Where(_ => Managers.Stage.State.Value == StageState.GuildSports).Subscribe(damage =>
        {
            MyGuildDpsValue.text = damage.ToCurrencyString();
        });

        Managers.GuildSports.EnemyGuildDps.Where(_ => Managers.Stage.State.Value == StageState.GuildSports).Subscribe(damage =>
        {
            EnemyGuildDpsValue.text = damage.ToCurrencyString();
        });

        Managers.GuildSports.PlayTime.Where(_ => Managers.Stage.State.Value == StageState.GuildSports).Subscribe(remainTime =>
        {
            GuildSportsRemainTimeTxt.text = ((int)remainTime).ToString();
        });

        SetGuideQuestUI(Managers.Game.UserData.ProgressGuideQuestId);
        SetGuideQuestProgress(Managers.Game.UserData.ProgressGuideQuestValue);

        Managers.Game.OnRefreshStat += SetPowerUI;

        Managers.Game.SettingData.PowerSave.Subscribe(powerSave =>
        {
            if (powerSave)
                PowerSaveModeTime = 60;
        });

        Managers.Pvp.IsAutoMatch.Subscribe(isAutoMatch => { AutoMatchObj.SetActive(isAutoMatch); });

        Managers.Game.GoodsDatas[(int)Goods.PvpTicket].Subscribe(pvpTicket =>
        {
            PvpTicketText.text = $"({pvpTicket}/1)";
        });

        Managers.Raid.Step.Subscribe(step => { RaidUI.StepText.text = $"{step}단계"; });

        Managers.Raid.PlayTime.Subscribe(playTime =>
        {
            int min = (int)(playTime / 60);
            float sec = playTime % 60;
            RaidUI.TimeText.text = min > 0 ? $"{min}분 {sec:N2}초" : $"{sec:N2}초";
        });

        Managers.Raid.GainGold.Subscribe(gold => { RaidUI.GoldText.text = gold.ToCurrencyString(); });

        Managers.Raid.GainGoldBar.Subscribe(goldBar => { RaidUI.GoldBarText.text = goldBar.ToCurrencyString(); });

        Managers.Raid.GainSkillStone.Subscribe(skillStone =>
        {
            RaidUI.SkillStoneText.text = skillStone.ToCurrencyString();
        });

        Managers.GuildRaid.Step.Subscribe(step => { RaidUI.StepText.text = $"{step}단계"; });

        Managers.GuildRaid.PlayTime.Subscribe(playTime =>
        {
            int min = (int)(playTime / 60);
            float sec = playTime % 60;
            RaidUI.TimeText.text = min > 0 ? $"{min}분 {sec:N2}초" : $"{sec:N2}초";
        });

        Managers.GuildRaid.GainGold.Subscribe(gold => { RaidUI.GoldText.text = gold.ToCurrencyString(); });

        Managers.GuildRaid.GainGoldBar.Subscribe(goldBar => { RaidUI.GoldBarText.text = goldBar.ToCurrencyString(); });

        Managers.GuildRaid.GainGuildPoint.Subscribe(guildPoint =>
        {
            RaidUI.SkillStoneText.text = guildPoint.ToCurrencyString();
        });

        SetGuideEvent();
    }

    private void SetGuideEvent()
    {
        if (Utils.IsAllClearGuideQuest())
            return;
        
        Managers.Game.UserData.OnChangeGuideQuestId
            .Subscribe(SetGuideQuestUI)
            .AddTo(_guideComposite);

        Managers.Game.UserData.OnChangeGuideQuestProgressValue
            .Subscribe(SetGuideQuestProgress)
            .AddTo(_guideComposite);

        Managers.Game.UserData.OnChangeGuideQuestId
            .Subscribe(SetNavigationId)
            .AddTo(_guideComposite);
        SetNavigationId(Managers.Game.UserData.ProgressGuideQuestId);

        Managers.Game.UserData.OnChangeGuideQuestProgressValue
            .Subscribe(OnGuideProgressValue)
            .AddTo(_guideComposite);
        OnGuideProgressValue(Managers.Game.UserData.ProgressGuideQuestValue);

        void OnGuideProgressValue(long progressValue)
        {
            if (!ChartManager.GuideQuestCharts.TryGetValue(Managers.Game.UserData.ProgressGuideQuestId,
                    out var guideQuestChart))
            {
                GuideQuestBgImage.sprite = GuideQuestBgSprite;
                CompleteGuideQuestNavigationObj.SetActive(false);
                return;
            }
            
            CompleteGuideQuestNavigationObj.SetActive(progressValue >= guideQuestChart.QuestCompleteValue);
            
            GuideQuestBgImage.sprite = progressValue >= guideQuestChart.QuestCompleteValue
                ? CompleteGuideQuestBgSprite
                : GuideQuestBgSprite;
            
            if (!Utils.IsCompleteGuideQuest())
                return;
            
            FeverNavigationObj.SetActive(false);
            AutoFeverNavigationObj.SetActive(false);
            MissionNavigationObj.SetActive(false);
            SideMenuNavigationObj.SetActive(false);
            BottomMenuNavigationObj.SetActive(false);
        }
    }

    void SetNavigationId(int id)
    {
        if (Utils.IsAllClearGuideQuest())
        {
            _guideComposite.Clear();
            return;
        }

        if (Utils.IsCompleteGuideQuest())
            return;

        FeverNavigationObj.SetActive(id == 3);
        AutoFeverNavigationObj.SetActive(id == 4);

        MissionNavigationObj.SetActive(id == 23);
        SideMenuNavigationObj.SetActive((id == 24 ||
                                         id == 25 ||
                                         id == 26 ||
                                         id == 27) && !SideMenuObj.activeSelf);

        BottomMenuNavigationObj.SetActive(
            id == 2 ||
            id == 5 ||
            id == 6 ||
            id == 7 ||
            id == 8 ||
            id == 11 ||
            id == 12 ||
            id == 13 ||
            id == 14 ||
            id == 15 ||
            id == 16 ||
            id == 17 ||
            id == 18 ||
            id == 19 ||
            id == 20 ||
            id == 21 ||
            id == 22 ||
            id == 28 ||
            id == 29
        );

        if (BottomMenuNavigationObj.activeSelf)
        {
            bool isFlip = false;

            switch (id)
            {
                // 캐릭터
                case 2:
                case 6:
                case 13:
                case 15:
                case 17:
                {
                    var pos = BottomMenuNavigationObj.transform.localPosition;
                    pos.x = BottomMenuNavigationPositions[(int)BottomButton.Character].x;
                    BottomMenuNavigationObj.transform.localPosition = pos;
                }
                    break;
                // 스킬
                case 7:
                case 11:
                {
                    var pos = BottomMenuNavigationObj.transform.localPosition;
                    pos.x = BottomMenuNavigationPositions[(int)BottomButton.Skill].x;
                    BottomMenuNavigationObj.transform.localPosition = pos;
                }
                    break;
                // 소환
                case 5:
                case 12:
                case 16:
                {
                    var pos = BottomMenuNavigationObj.transform.localPosition;
                    pos.x = BottomMenuNavigationPositions[(int)BottomButton.Summon].x;
                    BottomMenuNavigationObj.transform.localPosition = pos;
                }
                    break;
                // 철와대
                case 18:
                case 19:
                {
                    var pos = BottomMenuNavigationObj.transform.localPosition;
                    pos.x = BottomMenuNavigationPositions[(int)BottomButton.Chulwadae].x;
                    BottomMenuNavigationObj.transform.localPosition = pos;
                }
                    break;
                // 재입대
                case 20:
                case 21:
                case 22:
                {
                    var pos = BottomMenuNavigationObj.transform.localPosition;
                    pos.x = BottomMenuNavigationPositions[(int)BottomButton.Dungeon].x;
                    BottomMenuNavigationObj.transform.localPosition = pos;
                }
                    break;
                // 상점
                case 14:
                case 28:
                case 29:
                {
                    var pos = BottomMenuNavigationObj.transform.localPosition;
                    pos.x = BottomMenuNavigationPositions[(int)BottomButton.Shop].x;
                    BottomMenuNavigationObj.transform.localPosition = pos;
                }
                    isFlip = true;
                    break;
            }

            var scale = BottomMenuNavigationObj.transform.GetChild(0).localScale;
            scale.x = isFlip ? Math.Abs(scale.x) * -1 : Math.Abs(scale.x);
            BottomMenuNavigationObj.transform.GetChild(0).localScale = scale;
        }
    }

    private void Update()
    {
        if (!Managers.Game.SettingData.PowerSave.Value)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            PowerSaveModeTime = 60;
        }

        if (PowerSaveModeTime <= 0)
            return;

        PowerSaveModeTime -= Time.deltaTime;

        if (PowerSaveModeTime <= 0)
        {
        }
    }

    private void SetGuideQuestUI(int guideQuestId)
    {
        if (!ChartManager.GuideQuestCharts.TryGetValue(guideQuestId, out var guideQuestChart))
        {
            GuideQuestUI.UIObj.SetActive(false);
            PowerUI.UIObj.SetActive(true);
            SetPowerUI();
            return;
        }

        PowerUI.UIObj.SetActive(false);
        GuideQuestUI.UIObj.SetActive(true);
        GuideQuestUI.GuideQuestText.text = ChartManager.GetString(guideQuestChart.Desc);
        GuideQuestUI.GuideQuestProgressText.text = $"(0/{guideQuestChart.QuestCompleteValue})";
        GuideQuestUI.RewardItemImage.sprite =
            Managers.Resource.LoadItemIcon(guideQuestChart.RewardItemType, guideQuestChart.RewardItemId);
        GuideQuestUI.RewardItemValueText.text = guideQuestChart.RewardItemValue.ToCurrencyString();
    }

    private void SetGuideQuestProgress(long guideQuestProgressValue)
    {
        if (!ChartManager.GuideQuestCharts.TryGetValue(Managers.Game.UserData.ProgressGuideQuestId, out var guideQuestChart))
            return;

        GuideQuestUI.GuideQuestProgressText.text =
            $"({guideQuestProgressValue}/{guideQuestChart.QuestCompleteValue})";
    }

    private void SetPowerUI()
    {
        if (Managers.Game.UserData.PromoGrade == 0 ||
            !ChartManager.PromoDungeonCharts.ContainsKey(Managers.Game.UserData.PromoGrade))
            PowerUI.PromoGradeImage.gameObject.SetActive(false);
        else
        {
            PowerUI.PromoGradeImage.gameObject.SetActive(true);
            PowerUI.PromoGradeImage.sprite = Managers.Resource.LoadPromoIcon(Managers.Game.UserData.PromoGrade);
        }

        PowerUI.PowerText.text = Utils.GetPower().ToCurrencyString();
    }

    private void OnClickSideMenu()
    {
        SideMenuObj.SetActive(true);
        SideMenuButton.gameObject.SetActive(false);
        SetNavigationId(Managers.Game.UserData.ProgressGuideQuestId);
    }

    private void OnClickCloseSideMenu()
    {
        SideMenuObj.SetActive(false);
        SideMenuButton.gameObject.SetActive(true);
        SetNavigationId(Managers.Game.UserData.ProgressGuideQuestId);
    }

    private void OnClickChatButton()
    {
        if (Managers.UI.FindPopup<UI_ChatPopup>() != null)
            return;

        Managers.UI.ShowPopupUI<UI_ChatPopup>();
    }

    private void OnClickClearGuideQuest()
    {
        if (!ChartManager.GuideQuestCharts.TryGetValue(Managers.Game.UserData.ProgressGuideQuestId,
                out var guideQuestChart))
            return;

        if (Managers.Game.UserData.ProgressGuideQuestValue < guideQuestChart.QuestCompleteValue)
            return;

        InAppActivity.SendEvent($"guide_quest_{guideQuestChart.GuideQuestId}");
        Managers.Game.IncreaseItem(guideQuestChart.RewardItemType, guideQuestChart.RewardItemId,
            guideQuestChart.RewardItemValue);
        Managers.Game.SetNextGuideQuest();

        GameDataManager.UserGameData.SaveGameData();
        GameDataManager.GoodsGameData.SaveGameData();
    }
}