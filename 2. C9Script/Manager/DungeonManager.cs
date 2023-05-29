using System;
using System.Collections;
using BackEnd;
using Chart;
using Cinemachine;
using GameData;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class DungeonManager
{
    public readonly ReactiveProperty<int> DungeonId = new(0);
    public readonly ReactiveProperty<int> DungeonStep = new(0);

    public readonly ReactiveProperty<int> EntryPromoId = new(0);
    
    public readonly ReactiveProperty<int> ClearKillCount = new(0);
    public readonly ReactiveProperty<int> KillCount = new(0);
    public readonly ReactiveProperty<double> DungeonTotalReward = new(0);

    public TimeSpan DungeonLimitTime;
    public readonly ReactiveProperty<TimeSpan> DungeonRemainTime = new();

    public int HwasengbangDungeonWave;
    public int MarchDungeonWave;

    private Coroutine _limitTimerCoroutine;
    private CinemachineVirtualCamera _gameCamera;

    private bool _isProgress;
    
    public Action<int> StartDungeonEvent;

    public bool StartPromoBattle;

    private bool isClear;
    private bool isFailDungeon;
    private bool isFailPromo;

    public void Init()
    {
        _gameCamera = Object.FindObjectOfType<CinemachineVirtualCamera>();

        KillCount.Where(_ => Managers.Stage.State.Value == StageState.Dungeon).Subscribe(killCount =>
        {
            // Clear
            if (IsClearKillCount() && !isClear)
            {
                if (DungeonId.Value == (int)DungeonType.Hwasengbang)
                {
                    if (HwasengbangDungeonWave == 1)
                    {
                        if (ChartManager.HwasengbangDungeonCharts.TryGetValue(DungeonStep.Value,
                                out var hwasengbangDungeonChart))
                        {
                            if (hwasengbangDungeonChart.SecondMonsterId != 0)
                            {
                                HwasengbangDungeonWave = 2;
                                KillCount.Value = 0;
                                Managers.Monster.StartSpawn();
                            }
                            else
                            {
                                isClear = true;
                                ClearDungeon();
                            }
                        }
                        else
                        {
                            Debug.LogError($"Fail HwasengbangDungeonChart Id : {DungeonId.Value}");
                        }
                    }
                    else
                    {
                        isClear = true;
                        ClearDungeon();
                    }
                }
                else
                {
                    isClear = true;
                    ClearDungeon();
                }
            }
            else if (DungeonId.Value == (int)DungeonType.March)
            {
                // 몬스터 다 죽었으면 다음 웨이브 진행
                if (Managers.Monster.IsAllDeadMonster())
                {
                    MarchDungeonWave++;
                    Managers.Monster.StartSpawn();
                }
            }
        });
    }

    public void StartDungeon(int dungeonId, int dungeonStep)
    {
        Managers.Game.MainPlayer.State.Value = CharacterState.None;

        FadeScreen.FadeOut(() =>
        {
            _isProgress = true;
            Managers.Game.MainPlayer.transform.position = Vector3.zero;
            Managers.Game.MainPlayer.InitPassiveCount();

            Managers.UI.CloseAllPopupUI();
            
            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EnterDungeon, dungeonId, 1));

            Managers.Stage.State.Value = StageState.Dungeon;

            KillCount.Value = 0;
            DungeonTotalReward.Value = 0;

            isClear = false;
            isFailDungeon = false;

            DungeonId.Value = dungeonId;
            DungeonStep.Value = dungeonStep;

            if (DungeonId.Value == (int)DungeonType.Hwasengbang)
                HwasengbangDungeonWave = 1;
            
            if (DungeonId.Value == (int)DungeonType.March)
                MarchDungeonWave = 0;

            Managers.Stage.SetBg(ChartManager.DungeonCharts[dungeonId].WorldId);

            _limitTimerCoroutine = null;

            Observable.TimerFrame(1).Subscribe(_ =>
            {
                Managers.Monster.StartSpawn();
                StartDungeonEvent?.Invoke(DungeonStep.Value);
                int limitTime = 0;
                switch (DungeonId.Value)
                {
                    case (int)DungeonType.Hwasengbang:
                    {
                        limitTime = ChartManager.HwasengbangDungeonCharts[DungeonStep.Value].StageClearLimitTime;
                        _gameCamera.Follow = null;
                        _gameCamera.transform.position = new Vector3(0, 0, -10);
                        _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.MilitaryDungeonCameraSize;
                        ClearKillCount.Value = ChartManager.HwasengbangDungeonCharts[DungeonStep.Value].StageClearMonsterKillCount;
                        Managers.Sound.PlayBgm(BgmType.Hwasengbang);
                    }
                        break;
                    case (int)DungeonType.MarinCamp:
                    {
                        limitTime = ChartManager.MarinCampDungeonCharts[DungeonStep.Value].StageClearLimitTime;
                        _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.MilitaryDungeonCameraSize;
                        ClearKillCount.Value = ChartManager.MarinCampDungeonCharts[DungeonStep.Value].MonsterIds.Length;
                        Managers.Sound.PlayBgm(BgmType.Marinecamp);
                    }
                        break;
                    case (int)DungeonType.March:
                    {
                        limitTime = ChartManager.MarchDungeonCharts[DungeonStep.Value].StageClearLimitTime;
                        Managers.Game.MainPlayer.transform.position =
                            Managers.GameSystemData.MarchPlayerSpawnPosition;
                        _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.MarchCameraSize;
                        ClearKillCount.Value = ChartManager.MarchDungeonCharts[DungeonStep.Value].StageClearKillCount;
                        Managers.Sound.PlayBgm(BgmType.March);
                    }
                        break;
                }
                
                DungeonLimitTime = new TimeSpan(0, 0, limitTime);
                DungeonRemainTime.Value = DungeonLimitTime;
                
                FadeScreen.FadeIn(() =>
                {
                    Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                    _limitTimerCoroutine = Managers.Manager.StartCoroutine(CoLimitTimer(StageState.Dungeon));
                    
                });
            });
        }, 0.5f, 1f);
    }

    public bool IsClearKillCount()
    {
        switch (DungeonId.Value)
        {
            case 1:
            {
                if (HwasengbangDungeonWave == 1)
                    return KillCount.Value >= ChartManager.HwasengbangDungeonCharts[DungeonStep.Value]
                        .StageClearMonsterKillCount;
                else
                    return KillCount.Value >= 1;
            }
            case 2:
                return Managers.Monster.IsAllDeadMonster();
            case 3:
                return KillCount.Value >= ChartManager.MarchDungeonCharts[DungeonStep.Value].StageClearKillCount;
            default:
                return false;
        }
    }

    private IEnumerator CoLimitTimer(StageState checkState)
    {
        while (true)
        {
            if (Managers.Stage.State.Value != checkState)
                yield break;

            if (!_isProgress)
                yield break;

            yield return null;

            var timeSpan = DungeonRemainTime.Value.Add(TimeSpan.FromSeconds(-Time.deltaTime));
            DungeonRemainTime.Value = timeSpan;

            if (DungeonRemainTime.Value.TotalSeconds > 0)
                continue;

            switch (checkState)
            {
                case StageState.Dungeon:
                    FailDungeon();
                    break;
                case StageState.Promo:
                    FailPromo();
                    break;
            }

            yield break;
        }
    }

    private void ClearDungeon()
    {
        if (!_isProgress)
            return;
        
        _isProgress = false;
        
        if (_limitTimerCoroutine != null)
        {
            Managers.Manager.StopCoroutine(_limitTimerCoroutine);
            _limitTimerCoroutine = null;
        }
        
        Managers.Sound.PlayUISound(UISoundType.SuccessContents);

        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        Managers.Monster.SetAllMonsterStateNone();

        int clearRewardItemId = 0;
        double clearRewardItemValue = DungeonTotalReward.Value;

        var dungeonInfoChart = ChartManager.DungeonCharts[DungeonId.Value];

        clearRewardItemId = dungeonInfoChart.RewardItemId;

        Param param = new Param();

        switch (DungeonId.Value)
        {
            case (int)DungeonType.Hwasengbang:
            {
                var dungeonChart = ChartManager.HwasengbangDungeonCharts[DungeonStep.Value];
                param.Add("DungeonType", "Hwasengbang");
                
                switch (dungeonInfoChart.RewardItemId)
                {
                    case (int)Goods.Gold:
                        clearRewardItemValue += dungeonChart.StageClearRewardItemValue * Managers.Game.BaseStatDatas[(int)StatType.IncreaseGold];
                        break;
                    case (int)Goods.Exp:
                        clearRewardItemValue += dungeonChart.StageClearRewardItemValue * Managers.Game.BaseStatDatas[(int)StatType.IncreaseExp];
                        break;
                    default:
                        clearRewardItemValue += dungeonChart.StageClearRewardItemValue;
                        break;
                }
            }
                break;
            case (int)DungeonType.MarinCamp:
            {
                var dungeonChart = ChartManager.MarinCampDungeonCharts[DungeonStep.Value];
                param.Add("DungeonType", "MarineCamp");
                switch (dungeonInfoChart.RewardItemId)
                {
                    case (int)Goods.Gold:
                        clearRewardItemValue += dungeonChart.StageClearRewardItemValue * Managers.Game.BaseStatDatas[(int)StatType.IncreaseGold];
                        break;
                    case (int)Goods.Exp:
                        clearRewardItemValue += dungeonChart.StageClearRewardItemValue * Managers.Game.BaseStatDatas[(int)StatType.IncreaseExp];
                        break;
                    default:
                        clearRewardItemValue += dungeonChart.StageClearRewardItemValue;
                        break;
                }
            }
                break;
            case (int)DungeonType.March:
            {
                var dungeonChart = ChartManager.MarchDungeonCharts[DungeonStep.Value];
                param.Add("DungeonType", "March");
                switch (dungeonInfoChart.RewardItemId)
                {
                    case (int)Goods.Gold:
                        clearRewardItemValue += dungeonChart.StageClearRewardItemValue * Managers.Game.BaseStatDatas[(int)StatType.IncreaseGold];
                        break;
                    case (int)Goods.Exp:
                        clearRewardItemValue += dungeonChart.StageClearRewardItemValue * Managers.Game.BaseStatDatas[(int)StatType.IncreaseExp];
                        break;
                    default:
                        clearRewardItemValue += dungeonChart.StageClearRewardItemValue;
                        break;
                }
            }
                break;
        }

        // 던전 클리어 단계 갱신
        bool isSaveFlag = false;

        isSaveFlag |= Managers.Game.UserData.SetDungeonClearStep(DungeonId.Value, DungeonStep.Value);
        isSaveFlag |= Managers.Game.UserData.SetDungeonHighestValue(DungeonId.Value, clearRewardItemValue);
        
        param.Add("ClearStep", DungeonStep.Value);
        param.Add("GainItem", (ItemType.Goods, clearRewardItemId, clearRewardItemValue));
        Utils.GetGoodsLog(ref param);

        Backend.GameLog.InsertLog("Dungeon", param);

        if (isSaveFlag)
            GameDataManager.UserGameData.SaveGameData();

        Managers.Game.IncreaseItem(ItemType.Goods, clearRewardItemId, clearRewardItemValue);
        Managers.Game.DecreaseItem(ItemType.Goods, ChartManager.DungeonCharts[DungeonId.Value].EntryItemId, 1);

        GameDataManager.GoodsGameData.SaveGameData();

        var uiClearDungeonPopup = Managers.UI.ShowPopupUI<UI_ClearDungeonPopup>();

        if (uiClearDungeonPopup != null)
            uiClearDungeonPopup.Init(DungeonStep.Value, clearRewardItemId, clearRewardItemValue);

        Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
        {
            Managers.UI.CloseAllPopupUI();
            FadeScreen.GameViewFadeOut(() =>
            {
                Managers.Stage.State.Value = StageState.Normal;
                Managers.Monster.StartSpawn();
                
                if (_gameCamera != null)
                    _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.BaseCameraSize;
                
                FadeScreen.GameViewFadeIn(() =>
                {
                    Managers.Game.MainPlayer.ResetHp();
                    Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                }, 0.5f);
            }, 0);
        });
    }

    public void FailDungeon()
    {
        if (!_isProgress)
            return;

        _isProgress = false;
        
        if (Managers.Stage.State.Value != StageState.Dungeon)
            return;

        if (isFailDungeon)
            return;

        Managers.Sound.PlayUISound(UISoundType.GiveUp);
        isFailDungeon = true;

        if (_limitTimerCoroutine != null)
        {
            Managers.Manager.StopCoroutine(_limitTimerCoroutine);
            _limitTimerCoroutine = null;
        }

        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        Managers.Monster.SetAllMonsterStateNone();

        Managers.UI.ShowPopupUI<UI_FailDungeonPopup>();

        Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
        {
            Managers.UI.CloseAllPopupUI();
            FadeScreen.GameViewFadeOut(() =>
            {
                Managers.Game.MainPlayer.SetAllSkillCoolTime();
                Managers.Stage.State.Value = StageState.Normal;
                Managers.Monster.StartSpawn();
                _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.BaseCameraSize;
                FadeScreen.GameViewFadeIn(() =>
                {
                    Managers.Game.MainPlayer.ResetHp();
                    Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                }, 0.5f);
            }, 0);
        });
    }

    public void StartPromo(int promoId)
    {
        Managers.Game.MainPlayer.State.Value = CharacterState.None;

        FadeScreen.FadeOut(() =>
        {
            _isProgress = true;
            Managers.UI.CloseAllPopupUI();
            
            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EnterDungeon, (int)DungeonType.Promo, 1));

            Managers.Stage.State.Value = StageState.Promo;
            Managers.Game.MainPlayer.InitPassiveCount();

            Managers.Game.MainPlayer.transform.position = Managers.GameSystemData.PromoBossPlayerPosition;
            _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.PromoCameraSize;

            DungeonLimitTime = new TimeSpan(0, 0, (int)ChartManager.PromoDungeonCharts[promoId].ClearLimitTime);
            DungeonRemainTime.Value = DungeonLimitTime;

            isFailPromo = false;
            StartPromoBattle = false;

            Managers.Stage.SetBg(ChartManager.DungeonCharts[(int)DungeonType.Promo].WorldId);

            Managers.Monster.StartSpawn();

            EntryPromoId.Value = promoId;

            FadeScreen.FadeIn(() =>
            {
                MainThreadDispatcher.StartCoroutine(CoPromoCameraIn(Managers.GameSystemData.BaseCameraSize));
                MainThreadDispatcher.StartCoroutine(CoCheckPromoTimerStart());

                Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
            });
        }, 0.5f, 1f);
    }

    private IEnumerator CoPromoCameraIn(float minSize)
    {
        while (true)
        {
            _gameCamera.m_Lens.OrthographicSize -= Time.deltaTime * 4f;

            if (_gameCamera.m_Lens.OrthographicSize <= minSize)
                yield break;

            yield return null;
        }
    }

    private IEnumerator CoCheckPromoTimerStart()
    {
        var checkDelay = new WaitForSeconds(0.1f);
        while (true)
        {
            if (Managers.Stage.State.Value != StageState.Promo)
                yield break;

            if (Managers.Game.MainPlayer.State.Value == CharacterState.Attack)
            {
                _limitTimerCoroutine = Managers.Manager.StartCoroutine(CoLimitTimer(StageState.Promo));
                StartPromoBattle = true;
                yield break;
            }

            yield return checkDelay;
        }
    }

    public void ClearPromo()
    {
        if (!_isProgress)
            return;
        
        if (_limitTimerCoroutine != null)
        {
            Managers.Manager.StopCoroutine(_limitTimerCoroutine);
            _limitTimerCoroutine = null;
        }

        _isProgress = false;
        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        Managers.Game.UserData.PromoGrade += 1;
        Managers.Game.CalculateStat();
        GameDataManager.UserGameData.SaveGameData();
        Managers.UI.ShowPopupUI<UI_PromoSuccessPopup>();
        Managers.Sound.PlayUISound(UISoundType.SuccessContents);
    }

    public void FailPromo()
    {
        if (!_isProgress)
            return;
        
        if (Managers.Stage.State.Value != StageState.Promo)
            return;

        if (isFailPromo)
            return;

        Managers.Sound.PlayUISound(UISoundType.GiveUp);
        
        isFailPromo = true;

        if (_limitTimerCoroutine != null)
        {
            Managers.Manager.StopCoroutine(_limitTimerCoroutine);
            _limitTimerCoroutine = null;
        }

        _isProgress = false;
        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        Managers.Monster.PromoBoss.State.Value = CharacterState.None;
        Managers.UI.ShowPopupUI<UI_PromoFailPopup>();
    }

    public void EndPromo(bool isFail = false)
    {
        FadeScreen.GameViewFadeOut(() =>
        {
            if (isFail)
                Managers.Game.MainPlayer.SetAllSkillCoolTime();
            
            Managers.Stage.State.Value = StageState.Normal;
            Managers.Monster.StartSpawn();
            FadeScreen.GameViewFadeIn(() =>
            {
                Managers.Dungeon.EntryPromoId.Value = 0;
                Managers.Game.MainPlayer.ResetHp();
                Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
            }, 0.5f);
        });
    }
}