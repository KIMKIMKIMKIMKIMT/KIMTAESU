using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UI;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_StagePanel : UI_Panel
{
    #region Struct
    
    [Serializable]
    struct NormalStage
    {
        public TMP_Text StageText;
        public TMP_Text KillText;
        public Slider GageSlider;
        public Image GageSliderImage;
        public Button BossStartButton;
        public Toggle AutoBossToggle;
        public GameObject Obj;
        public Image BossNoticeImage;
        public GameObject GiveUpButton;
    }

    [Serializable]
    struct BossStage
    {
        public TMP_Text RemainTimeText;
        public GameObject Obj;
    }

    [Serializable]
    struct DungeonStage
    {
        public Image RewardImage;
        public TMP_Text RewardText;
        public TMP_Text RewardValueText;
        public TMP_Text RemainTimeText;
        public TMP_Text KillCountText;
        public Slider RemainTimeSlider;
        public GameObject Obj;

        public GameObject StageObj;
        public TMP_Text StageText;
        public Image StageImage;

        public Button ExitButton;
    }
    
    [Serializable]
    struct PromoStage
    {
        public Image PromoGradeImage;
        public TMP_Text PromoInfoText;
        public Slider BossHpBarSlider;
        public Slider RemainTimeSlider;
        public Button GiveUpButton;
        public GameObject Obj;
    }

    [Serializable]
    struct DpsStage
    {
        public TMP_Text DpsValueText;
        public TMP_Text RemainTimeText;
        public Slider RemainTimeSlider;
        public Button GiveUpButton;
        public GameObject Obj;
    }

    [Serializable]
    struct WorldCupDungeonStage
    {
        public TMP_Text GainValueText;
        public TMP_Text RemainTimeText;
        public Slider RemainTimeSlider;
        public Image AdBuffImage;
        public Button AdBuffButton;
        public GameObject Obj;
        
    }

    [Serializable]
    record RaidStage
    {
        public GameObject Wave1NoticeObj;
        public GameObject Wave2NoticeObj;
        public GameObject Wave3NoticeObj;

        public Sprite SkillStoneSprite;
        public Sprite GuildPointSprite;

        public Image Wave3RewardImage;

        public GameObject Wave3Obj;
        public Slider BossHpSlider;
        public Slider LimitTimeSlider;
        public TMP_Text BossNameText;

        public GameObject Obj;

        public Sequence NoticeSequence;
        [HideInInspector] public CompositeDisposable WaveNoticeComposite = new();

        public void SetSkillStoneIcon()
        {
            Wave3RewardImage.sprite = SkillStoneSprite;
        }

        public void SetGuildPointIcon()
        {
            Wave3RewardImage.sprite = GuildPointSprite;
        }
    }

    [Serializable]
    record GuildAllRaidStage
    {
        public GameObject Wave1NoticeObj;

        public Sprite GuildPointSprite;

        public GameObject Wave1Obj;
        public Slider BossHpSlider;
        public Slider LimitTimeSlider;
        public TMP_Text BossNameText;
        public TMP_Text TimeText;
        public TMP_Text StepText;

        public GameObject Obj;

        public Sequence NoticeSequence;
        [HideInInspector] public CompositeDisposable WaveNoticeComposite = new();
    }

    #endregion

    [SerializeField] private NormalStage UINormalStage;
    [SerializeField] private BossStage UIBossStage;
    [SerializeField] private DungeonStage UIDungeonStage;
    [SerializeField] private PromoStage UIPromoStage;
    [SerializeField] private DpsStage UIDpsStage;
    [SerializeField] private WorldCupDungeonStage UIWorldCupDungeonStage;
    [SerializeField] private RaidStage UIRaidStage;
    [SerializeField] private GuildAllRaidStage UIGuildAllRaidStage;
    

    [SerializeField] private GameObject BossNavigationObj;

    private readonly List<Tween> _tweens = new();

    private CompositeDisposable _promoCompositeDisposable = new();

    private void Start()
    {
        SetButtonEvent();
        SetPropertyEvent();
    }

    public void SetButtonEvent()
    {
        UINormalStage.BossStartButton.BindEvent(OnClickBossStart);
        UINormalStage.AutoBossToggle.OnValueChangedAsObservable().Subscribe(value =>
        {
            Managers.Stage.IsAutoBoss.Value = value;
            if (value)
                StartBoss();
        });
        
        UIDungeonStage.ExitButton.BindEvent(() => Managers.Dungeon.FailDungeon());
        UIPromoStage.GiveUpButton.BindEvent(() => Managers.Dungeon.FailPromo());
        UIDpsStage.GiveUpButton.BindEvent(()=>Managers.Dps.EndDpsDungeon(true));
        UINormalStage.GiveUpButton.BindEvent(() => Managers.Stage.FailStage());
        UIWorldCupDungeonStage.AdBuffButton.BindEvent(() =>
        {
            if (Managers.Game.WorldCupAdBuff.Value)
                return;

            Managers.Ad.Show(() =>
            {
                Managers.Game.WorldCupAdBuff.Value = true;
                Managers.Game.CalculateStat();
            });
        });
    }
    
    public void SetPropertyEvent()
    {
        Managers.Stage.State.Subscribe(state =>
        {
            if (BossNavigationObj.activeSelf && state == StageState.StageBoss)
                BossNavigationObj.SetActive(false);
            
            UINormalStage.Obj.SetActive(state is StageState.Normal or StageState.StageBoss);
            UIBossStage.Obj.SetActive(false);
            UIDungeonStage.Obj.SetActive(state == StageState.Dungeon);
            UIPromoStage.Obj.SetActive(state == StageState.Promo);
            UIDpsStage.Obj.SetActive(state == StageState.Dps);
            UIWorldCupDungeonStage.Obj.SetActive(state == StageState.WorldCupEvent);
            UIRaidStage.Obj.SetActive(state == StageState.Raid || state == StageState.GuildRaid);
            UIGuildAllRaidStage.Obj.SetActive(state == StageState.GuildAllRaid);
            
            if (state == StageState.StageBoss)
            {
                _tweens.ForEach(tween => tween.Kill());
                _tweens.Clear();

                Color color = UINormalStage.GageSliderImage.color;
                color.a = 1f;
                UINormalStage.GageSliderImage.color = color;
                UINormalStage.BossStartButton.transform.localScale = Vector3.one;

                UINormalStage.BossStartButton.interactable = false;

                UINormalStage.BossNoticeImage.gameObject.SetActive(true);

                DOTween.Sequence().Append(UINormalStage.BossNoticeImage.DOFade(1f, 0)).AppendInterval(1.5f)
                    .Append(UINormalStage.BossNoticeImage.DOFade(0, 0.5f));

            }
            else
                UINormalStage.BossNoticeImage.gameObject.SetActive(false);
            
            if (state == StageState.Raid)
                UIRaidStage.SetSkillStoneIcon();
            else if (state == StageState.GuildRaid)
                UIRaidStage.SetGuildPointIcon();
        });

        #region NormalStage

        Managers.Stage.State.Subscribe(state =>
        {
            switch (state)
            {
                case StageState.Normal:
                {
                    if (ChartManager.StageDataController.StageDataTable.TryGetValue(Managers.Stage.StageId.Value, out var stageChart))
                    {
                        UINormalStage.KillText.text = $"{Managers.Stage.KillCount.Value}/{stageChart.NeedBossChallengeKillCount}";
                        UINormalStage.GageSlider.value = (float)Managers.Stage.KillCount.Value / stageChart.NeedBossChallengeKillCount;
                    }
                    
                    UINormalStage.BossStartButton.gameObject.SetActive(true);
                    UINormalStage.GiveUpButton.SetActive(false);
                }
                    break;
                case StageState.StageBoss:
                    UINormalStage.BossStartButton.gameObject.SetActive(false);
                    UINormalStage.GiveUpButton.SetActive(true);
                    break;
            }
        });

        Managers.Stage.StageId.Subscribe(stage =>
        {
            UINormalStage.StageText.text = $"Stage-{stage.ToString()}";

            if (Managers.Stage.State.Value == StageState.Normal)
            {
                UINormalStage.KillText.text = Utils.IsCurrentStageIsMaxStage() ?
                    "Max Stage" :
                    $"{Managers.Stage.KillCount.Value}/{ChartManager.StageDataController.StageDataTable[stage].NeedBossChallengeKillCount}";
            }

            if (Managers.Stage.State.Value == StageState.StageBoss)
            {
                DOTween.Sequence().Append(UINormalStage.BossNoticeImage.DOFade(1f, 0)).AppendInterval(1.5f)
                    .Append(UINormalStage.BossNoticeImage.DOFade(0, 0.5f));
            }
        });
        
        Managers.Stage.KillCount.Where(_ => Managers.Stage.State.Value == StageState.Normal).Subscribe(killCount =>
        {
            if (Utils.IsCurrentStageIsMaxStage())
            {
                UINormalStage.KillText.text = "Max Stage";
                UINormalStage.GageSlider.value = 1f;
            }
            else
            {
                var needBossChallengeKillCount = ChartManager.StageDataController.StageDataTable[Managers.Stage.StageId.Value].NeedBossChallengeKillCount;
                UINormalStage.KillText.text = $"{killCount}/{needBossChallengeKillCount}";
                UINormalStage.GageSlider.value = (float)killCount / needBossChallengeKillCount;
            }

            if (killCount >= ChartManager.StageDataController.StageDataTable[Managers.Stage.StageId.Value].NeedBossChallengeKillCount &&
                !UINormalStage.BossStartButton.gameObject.activeSelf)
            {
                if (Utils.ExistCurrentStageBossData() && Utils.ExistNextStageData())
                {
                    UINormalStage.BossStartButton.interactable = true;
                    UINormalStage.BossStartButton.gameObject.SetActive(true);
                    
                    _tweens.Add(UINormalStage.GageSliderImage.DOFade(100f / 255f, 0.3f).SetLoops(-1, LoopType.Yoyo));
                    _tweens.Add(UINormalStage.BossStartButton.transform.DOScale(1.15f, 0.3f).SetLoops(-1, LoopType.Yoyo));
                }
            }
            else
            {
                if (_tweens.Count > 0)
                {
                    _tweens.ForEach(tween => tween.Kill());
                    UINormalStage.GageSliderImage.DOFade(1, 0);
                    UINormalStage.BossStartButton.transform.DOScale(1, 0);
                }
            }

            if (Utils.IsCurrentStageIsMaxStage() || (killCount < ChartManager.StageDataController.StageDataTable[Managers.Stage.StageId.Value].NeedBossChallengeKillCount &&
                UINormalStage.BossStartButton.gameObject.activeSelf))
                UINormalStage.BossStartButton.gameObject.SetActive(false);
        });
        
        Managers.Stage.IsAutoBoss.Subscribe(isAutoBoss =>
        {
            UINormalStage.AutoBossToggle.isOn = isAutoBoss;
        });

        #endregion
        
        #region BossStage
        
        Managers.Stage.StageRemainTime.Where(_ => Managers.Stage.State.Value == StageState.StageBoss).Subscribe(limitTime =>
        {
            UINormalStage.GageSlider.value = (float)(limitTime.TotalSeconds / Managers.Stage.StageLimitTime.TotalSeconds);
            UINormalStage.KillText.text = $"{(int)limitTime.TotalSeconds}초";
        });
        
        #endregion

        #region Dungeon
        
        Managers.Dungeon.DungeonId.Where(_ => Managers.Stage.State.Value == StageState.Dungeon).Subscribe(dungeonId =>
        {
            var rewardItemChart = ChartManager.GoodsCharts[ChartManager.DungeonCharts[dungeonId].RewardItemId];
            
            UIDungeonStage.RewardImage.sprite =
                Managers.Resource.LoadGoodsIcon(rewardItemChart.Icon);

            UIDungeonStage.RewardText.text = $"획득 {ChartManager.GetString(rewardItemChart.Name)}";
            UIDungeonStage.RewardText.color = Utils.ItemColor[rewardItemChart.Id];
            UIDungeonStage.StageText.color = Utils.DungeonColor[dungeonId];
        });

        CompositeDisposable compositeDisposable = new CompositeDisposable();

        Managers.Dungeon.StartDungeonEvent += dungeonStep =>
        {
            if (!ChartManager.DungeonCharts.TryGetValue(Managers.Dungeon.DungeonId.Value, out var dungeonChart))
                return;

            compositeDisposable.Clear();

            UIDungeonStage.StageText.text = $"{ChartManager.GetString(dungeonChart.Name)} {dungeonStep}단계";

            UIDungeonStage.StageText.alpha = 1f;
            UIDungeonStage.StageImage.color = new Color(1, 1, 1, 1);

            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
            {
                UIDungeonStage.StageText.DOFade(0, 2f);
                UIDungeonStage.StageImage.DOFade(0, 2f);
            }).AddTo(compositeDisposable);
        };
        
        UIDungeonStage.RewardValueText.text = "0";
        Managers.Dungeon.DungeonTotalReward.Where(_ => Managers.Stage.State.Value == StageState.Dungeon).Subscribe(rewardValue =>
        {
            UIDungeonStage.RewardValueText.text = rewardValue.ToCurrencyString();
        });

        Managers.Dungeon.DungeonRemainTime.Where(_ => Managers.Stage.State.Value == StageState.Dungeon).Subscribe(remainTime =>
        {
            if (Managers.Dungeon.DungeonLimitTime == null || Managers.Dungeon.DungeonLimitTime.TotalSeconds <= 0)
            {
                UIDungeonStage.RemainTimeSlider.value = 1;
                return;
            }

            UIDungeonStage.RemainTimeSlider.value =
                (float)(remainTime.TotalSeconds / Managers.Dungeon.DungeonLimitTime.TotalSeconds);

            UIDungeonStage.RemainTimeText.text = $"{((int)remainTime.TotalSeconds).ToString()}초";
        });

        Managers.Dungeon.DungeonStep.Subscribe(dungeonStep =>
        {
            var dungeonClearKillCount = 0;

            switch (Managers.Dungeon.DungeonId.Value)
            {
                case (int)DungeonType.Hwasengbang:
                {
                    if (ChartManager.HwasengbangDungeonCharts.TryGetValue(dungeonStep, out var hwasengbangDungeonChart))
                        dungeonClearKillCount = hwasengbangDungeonChart.StageClearMonsterKillCount;
                }
                    break;
                case (int)DungeonType.MarinCamp:
                {
                    if (ChartManager.MarinCampDungeonCharts.TryGetValue(dungeonStep, out var marinCampDungeonChart))
                        dungeonClearKillCount = marinCampDungeonChart.MonsterIds.Length;
                }
                    break;
                case (int)DungeonType.March:
                {
                    if (ChartManager.MarchDungeonCharts.TryGetValue(dungeonStep, out var marchDungeonChart))
                        dungeonClearKillCount = marchDungeonChart.StageClearKillCount;
                }
                    break;
            }
            
            UIDungeonStage.KillCountText.text = $"{dungeonStep}단계 : {Managers.Dungeon.KillCount.Value}/{dungeonClearKillCount}";
        });

        // Managers.Dungeon.ClearKillCount.Subscribe(clearKillCount =>
        // {
        //     UIDungeonStage.KillCountText.text =
        //         $"{Managers.Dungeon.DungeonStep.Value}단계 : 0/{Managers.Dungeon.ClearKillCount.Value}";
        // });

        Managers.Dungeon.KillCount.Subscribe(killCount =>
        {
            UIDungeonStage.KillCountText.text =
                $"{Managers.Dungeon.DungeonStep.Value}단계 : {killCount}/{Managers.Dungeon.ClearKillCount.Value}";
        });
        
        #endregion

        #region Promo
        
        Managers.Dungeon.EntryPromoId.Where(_ => Managers.Stage.State.Value == StageState.Promo).Subscribe(promoId =>
        {
            if (!ChartManager.PromoDungeonCharts.TryGetValue(promoId, out var promoDungeonChart))
                return;

            UIPromoStage.PromoGradeImage.sprite = Managers.Resource.LoadPromoIcon(promoId);
            UIPromoStage.PromoInfoText.text =
                $"{ChartManager.GetString(promoDungeonChart.Name)} - {ChartManager.GetString(ChartManager.MonsterCharts[promoDungeonChart.BossId].Name)}";

            _promoCompositeDisposable.Clear();
            
            Managers.Monster.PromoBoss.Hp.Subscribe(hp =>
            {
                float ratio = (float)(hp / Managers.Monster.PromoBoss.MaxHp);
                UIPromoStage.BossHpBarSlider.value = ratio;
            }).AddTo(_promoCompositeDisposable);
        });

        Managers.Dungeon.DungeonRemainTime.Where(_ => Managers.Stage.State.Value == StageState.Promo).Subscribe(
            remainTime =>
            {
                float ratio = (float)(remainTime.TotalSeconds / Managers.Dungeon.DungeonLimitTime.TotalSeconds);
                UIPromoStage.RemainTimeSlider.value = ratio;
            });
        
        #endregion
        
        #region Dps

        Managers.Dps.TotalDps.Subscribe(dpsValue =>
        {
            UIDpsStage.DpsValueText.text = dpsValue.ToCurrencyString();
        });

        Managers.Dps.RemainTime.Subscribe(remainTime =>
        {
            UIDpsStage.RemainTimeText.text = $"{(int)remainTime}초";
            UIDpsStage.RemainTimeSlider.value = Managers.Dps.LimitTime <= 0 ? 0 : remainTime / Managers.Dps.LimitTime;
        });

        #endregion

        #region WorldCup

        Managers.WorldCupEvent.RemainTime.Subscribe(time =>
        {
            UIWorldCupDungeonStage.RemainTimeText.text = $"{((int)time).ToString()}초";
            UIWorldCupDungeonStage.RemainTimeSlider.value = Managers.WorldCupEvent.LimitTime == 0 ? 0 : time / Managers.WorldCupEvent.LimitTime;
        });

        Managers.WorldCupEvent.TotalReward.Subscribe(rewardValue =>
        {
            UIWorldCupDungeonStage.GainValueText.text = rewardValue.ToCurrencyString();
        });

        Managers.Game.WorldCupAdBuff.Subscribe(enable =>
        {
            UIWorldCupDungeonStage.AdBuffImage.color = enable ? Color.white : new Color(0.2f, 0.2f, 0.2f);
        });

        Managers.Stage.State.Subscribe(state =>
        {
            UIWorldCupDungeonStage.AdBuffButton.gameObject.SetActive(state == StageState.WorldCupEvent);
        });

        #endregion
        
        #region Raid

        Managers.Raid.Wave.Subscribe(wave =>
        {
            CanvasGroup onObjCanvasGroup = null;
            
            UIRaidStage.Wave3Obj.SetActive(wave == 3);
            
            switch (wave)
            {
                case 1:
                    Managers.Sound.PlaySfxSound(SfxType.RaidWave);
                    UIRaidStage.Wave1NoticeObj.SetActive(true);
                    onObjCanvasGroup = UIRaidStage.Wave1NoticeObj.GetComponent<CanvasGroup>();
                    break;
                case 2:
                    Managers.Sound.PlaySfxSound(SfxType.RaidWave);
                    UIRaidStage.Wave2NoticeObj.SetActive(true);
                    onObjCanvasGroup = UIRaidStage.Wave2NoticeObj.GetComponent<CanvasGroup>();
                    break;
                case 3:
                    Managers.Sound.PlaySfxSound(SfxType.RaidWave);
                    UIRaidStage.Wave3NoticeObj.SetActive(true);
                    onObjCanvasGroup = UIRaidStage.Wave3NoticeObj.GetComponent<CanvasGroup>();
                    UIRaidStage.BossNameText.text = Managers.Raid.GetRaidBossName();
                    break;
                default:
                {
                    UIRaidStage.Wave1NoticeObj.SetActive(false);
                    UIRaidStage.Wave2NoticeObj.SetActive(false);
                    UIRaidStage.Wave3NoticeObj.SetActive(false);
                }
                    break;
            }

            if (onObjCanvasGroup == null) 
                return;
            
            UIRaidStage.WaveNoticeComposite.Clear();
            UIRaidStage.NoticeSequence?.Kill();

            UIRaidStage.NoticeSequence = DOTween.Sequence().Append(onObjCanvasGroup.DOFade(0, 0))
                .Append(onObjCanvasGroup.DOFade(1, 0.5f)).AppendInterval(1f)
                .Append(onObjCanvasGroup.DOFade(0, 0.5f));
        });

        Managers.Monster.OnSpawnRaidBoss.Subscribe(raidBossMonster =>
        {
            raidBossMonster.Hp.Subscribe(hp =>
            {
                UIRaidStage.BossHpSlider.value = Managers.Monster.RaidBossMonster.MaxHp == 0 ? 0 : (float)(hp / raidBossMonster.MaxHp);
            }).AddTo(raidBossMonster.gameObject);
        });

        
        

        Managers.Raid.Wave3RemainTime.Subscribe(time =>
        {
            UIRaidStage.LimitTimeSlider.value =
                Managers.Raid.Wave3LimitTime == 0 ? 0 : time / Managers.Raid.Wave3LimitTime;
        });
        
        Managers.GuildRaid.Wave.Subscribe(wave =>
        {
            CanvasGroup onObjCanvasGroup = null;
            
            UIRaidStage.Wave3Obj.SetActive(wave == 3);
            
            switch (wave)
            {
                case 1:
                    Managers.Sound.PlaySfxSound(SfxType.RaidWave);
                    UIRaidStage.Wave1NoticeObj.SetActive(true);
                    onObjCanvasGroup = UIRaidStage.Wave1NoticeObj.GetComponent<CanvasGroup>();
                    break;
                case 2:
                    Managers.Sound.PlaySfxSound(SfxType.RaidWave);
                    UIRaidStage.Wave2NoticeObj.SetActive(true);
                    onObjCanvasGroup = UIRaidStage.Wave2NoticeObj.GetComponent<CanvasGroup>();
                    break;
                case 3:
                    Managers.Sound.PlaySfxSound(SfxType.RaidWave);
                    UIRaidStage.Wave3NoticeObj.SetActive(true);
                    onObjCanvasGroup = UIRaidStage.Wave3NoticeObj.GetComponent<CanvasGroup>();
                    UIRaidStage.BossNameText.text = Managers.GuildRaid.GetRaidBossName();
                    break;
                default:
                {
                    UIRaidStage.Wave1NoticeObj.SetActive(false);
                    UIRaidStage.Wave2NoticeObj.SetActive(false);
                    UIRaidStage.Wave3NoticeObj.SetActive(false);
                }
                    break;
            }

            if (onObjCanvasGroup == null) 
                return;
            
            UIRaidStage.WaveNoticeComposite.Clear();
            UIRaidStage.NoticeSequence?.Kill();

            UIRaidStage.NoticeSequence = DOTween.Sequence().Append(onObjCanvasGroup.DOFade(0, 0))
                .Append(onObjCanvasGroup.DOFade(1, 0.5f)).AppendInterval(1f)
                .Append(onObjCanvasGroup.DOFade(0, 0.5f));
        });

        Managers.GuildRaid.Wave3RemainTime.Subscribe(time =>
        {
            UIRaidStage.LimitTimeSlider.value =
                Managers.GuildRaid.Wave3LimitTime == 0 ? 0 : time / Managers.GuildRaid.Wave3LimitTime;
        });

        #endregion

        #region GuildAllRaid
        Managers.Monster.OnSpawnGuildAllRaidBoss.Subscribe(guildAllRaidBossMonster =>
        {
            guildAllRaidBossMonster.Hp.Subscribe(hp =>
            {
                UIGuildAllRaidStage.BossHpSlider.value = Managers.Monster.AllRaidBossMonster.MaxHp == 0 ? 0 : (float)(hp / guildAllRaidBossMonster.MaxHp);
            }).AddTo(guildAllRaidBossMonster.gameObject);
        });

        Managers.AllRaid.RemainTime.Subscribe(time =>
        {
            UIGuildAllRaidStage.LimitTimeSlider.value =
                Managers.AllRaid.LimitTime == 0 ? 0 : time / Managers.AllRaid.LimitTime;
        });

        Managers.AllRaid.PlayTime.Subscribe(playTime =>
        {
            int min = (int)(playTime / 60);
            float sec = playTime % 60;
            UIGuildAllRaidStage.TimeText.text = min > 0 ? $"{min}분 {sec:N2}초" : $"{sec:N2}초";
        });

        Managers.AllRaid.Step.Subscribe(step => { UIGuildAllRaidStage.StepText.text = $"{step}단계"; });

        Managers.AllRaid.Wave.Subscribe(wave =>
        {
            CanvasGroup onObjCanvasGroup = null;

            UIGuildAllRaidStage.Wave1Obj.SetActive(wave == 1);

            switch (wave)
            {
                case 1:
                    Managers.Sound.PlaySfxSound(SfxType.RaidWave);
                    UIGuildAllRaidStage.Wave1NoticeObj.SetActive(true);
                    onObjCanvasGroup = UIGuildAllRaidStage.Wave1NoticeObj.GetComponent<CanvasGroup>();
                    UIGuildAllRaidStage.BossNameText.text = Managers.AllRaid.GetGuildAllRaidBossName();
                    break;
                default:
                    {
                        UIGuildAllRaidStage.Wave1NoticeObj.SetActive(false);
                    }
                    break;
            }

            if (onObjCanvasGroup == null)
                return;

            UIGuildAllRaidStage.WaveNoticeComposite.Clear();
            UIGuildAllRaidStage.NoticeSequence?.Kill();

            UIGuildAllRaidStage.NoticeSequence = DOTween.Sequence().Append(onObjCanvasGroup.DOFade(0, 0))
                .Append(onObjCanvasGroup.DOFade(1, 0.5f)).AppendInterval(1f)
                .Append(onObjCanvasGroup.DOFade(0, 0.5f));
        });
        #endregion

        #region GuideQuest

        Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigation).AddTo(gameObject);
        SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);

        void SetNavigation(int id)
        {
            BossNavigationObj.SetActive(id == 30);
        }

        #endregion
    }

    private void OnClickBossStart()
    {
        Managers.UI.ShowPopupUI<UI_BossNoticePopup>();
    }

    private void StartBoss()
    {
        if (Managers.Stage.StartBoss())
        {
        }
    }

}