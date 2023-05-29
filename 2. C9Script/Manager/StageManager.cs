using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UniRx;
using UnityEngine;


public class StageManager
{
    public readonly ReactiveProperty<ObscuredInt> StageId = new(1);
    public readonly ReactiveProperty<long> KillCount = new(0);

    public readonly ReactiveProperty<StageState> State = new(StageState.Normal);

    public TimeSpan StageLimitTime;
    public readonly ReactiveProperty<TimeSpan> StageRemainTime = new();

    public readonly ReactiveProperty<bool> IsAutoBoss = new(false);

    private Coroutine _limitTimerCoroutine;

    private SpriteRenderer BgImage;
    private Transform BGScale;

    private bool _isProgress;
    private bool _isClearStage;
    private bool _isFail;

    private readonly CompositeDisposable _compositeDisposable = new();

    public long NeedKillCount => ChartManager.StageDataController.StageDataTable[StageId.Value].NeedBossChallengeKillCount;

    public void SetBg(int worldId)
    {
        if (BgImage == null)
            return;
        
        BgImage.sprite = Managers.Resource.LoadBg(worldId);
    }

    public void SetBg(string bgName)
    {
        if (BgImage == null)
            return;
        
        BgImage.sprite = Managers.Resource.LoadBg(bgName);
    }

    public void SetBGScale(Vector3 scale)
    {
        BGScale.localScale = scale;
    }

    public void Init()
    {
        BgImage = GameObject.Find("BG_Stage")?.GetComponent<SpriteRenderer>();
        BGScale = GameObject.Find("BG_Stage")?.GetComponent<Transform>();
        SetPropertyEvent();
    }

    private void SetPropertyEvent()
    {
        StageId.Subscribe(stageId =>
        {
            SetBg(ChartManager.StageDataController.StageDataTable[stageId].WorldIndex);
            
            if (State.Value != StageState.StageBoss)
            {
                Managers.Monster.StartSpawn();
            }

            Managers.Game.UserData.CurrentStage = stageId;

            if (stageId > Managers.Game.UserData.MaxReachStage)
            {
                var gainItemDatas = new Dictionary<int, double>();

                if (ChartManager.StageBossDataController.StageBossTable.TryGetValue(stageId - 1, out var stageBossChart))
                {
                    for (int i = 0; i < stageBossChart.ClearRewardIds.Length; i++)
                    {
                        Managers.Game.IncreaseItem(ItemType.Goods, stageBossChart.ClearRewardIds[i], stageBossChart.ClearRewardValues[i]);

                        var gainItemKey = stageBossChart.ClearRewardIds[i];

                        if (gainItemDatas.ContainsKey(gainItemKey))
                            gainItemDatas[gainItemKey] += stageBossChart.ClearRewardValues[i];
                        else
                            gainItemDatas[gainItemKey] = stageBossChart.ClearRewardValues[i];
                    }
                }

                foreach (var gainItemData in gainItemDatas)
                {
                    MessageBroker.Default.Publish(new UI_GainDropItemsPanel.GainDropItemData(
                        gainItemData.Key, gainItemData.Value
                        ));
                }
                
                Managers.Game.UserData.MaxReachStage = stageId;
                Managers.Game.UserData.MaxReachStageTime = Utils.GetNow();
                GameDataManager.UserGameData.SaveGameData();
                GameDataManager.GoodsGameData.SaveGameData();
                InAppActivity.SendStageEvent(stageId);

                Managers.Rank.IsRefreshRankStageFlag = true;

                Managers.Game.StageLog[$"Stage-{stageId}"] = Managers.Game.UserData.MaxReachStageTime.ToString();
            }
            else
                GameDataManager.UserGameData.SetSaveTimer();
        });

        State.Subscribe(state =>
        {
            switch (state)
            {
                case StageState.Normal:
                case StageState.StageBoss:
                    Managers.Sound.PlayBgm(BgmType.Stage);
                    break;
                case StageState.Promo:
                    Managers.Sound.PlayBgm(BgmType.Promo);
                    break;
                case StageState.Dps:
                    Managers.Sound.PlayBgm(BgmType.Dps);
                    break;
                case StageState.Pvp:
                    Managers.Sound.PlayBgm(BgmType.Pvp);
                    break;
                case StageState.XMasEvent:
                    Managers.Sound.PlayBgm(BgmType.XMasEvent);
                    break;
                case StageState.GuildSports:
                    Managers.Sound.PlayBgm(BgmType.GuildSports);
                    break;
            }
            
            if (state != StageState.Pvp && Managers.Pvp.EnemyPlayer != null)
                Managers.Pvp.EnemyPlayer.DestroyPlayer();
                
        });

        State.Where(state => state == StageState.Normal).Subscribe(_ =>
        {
            SetBg(ChartManager.StageDataController.StageDataTable[StageId.Value].WorldIndex);
        });

        State.Where(state => state != StageState.StageBoss).Subscribe(_ => _isProgress = false);
        
        KillCount.Subscribe(killCount =>
        {
            if (killCount < ChartManager.StageDataController.StageDataTable[StageId.Value].NeedBossChallengeKillCount) 
                return;
            
            if (killCount > ChartManager.StageDataController.StageDataTable[StageId.Value].NeedBossChallengeKillCount)
                KillCount.Value = ChartManager.StageDataController.StageDataTable[StageId.Value].NeedBossChallengeKillCount;

            if (IsAutoBoss.Value)
            {
                StartBoss();
            }
        });
    }

    public bool StartBoss(bool isSkip = false)
    {
        if (_isProgress)
            return false;
        
        if (!Utils.ExistNextStageData() || !Utils.ExistCurrentStageBossData())
            return false;

        if (!isSkip && KillCount.Value < ChartManager.StageDataController.StageDataTable[StageId.Value].NeedBossChallengeKillCount)
            return false;

        if (State.Value == StageState.StageBoss && !isSkip)
            return false;

        if (Utils.IsCurrentStageIsMaxStage())
        {
            Managers.Message.ShowMessage("최대 스테이지 입니다.");
            return false;
        }

        _compositeDisposable.Clear();
        _isProgress = true;
        _isFail = false;

        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        
        FadeScreen.GameViewFadeOut(() =>
        {
            State.Value = StageState.StageBoss;
            _isClearStage = false;

            Managers.Game.MainPlayer.transform.position = Managers.GameSystemData.StageBossPlayerPosition;

            StageRemainTime.Value = new TimeSpan();
            StageRemainTime.Value +=
                TimeSpan.FromSeconds(ChartManager.StageBossDataController.StageBossTable[StageId.Value].ClearLimitTime);
            StageLimitTime = StageRemainTime.Value;

            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(
                _ =>
                {
                    Managers.Monster.StartSpawn();
                    Managers.Monster.SetAllMonsterStateNone();
                    
                    FadeScreen.GameViewFadeIn(() =>
                    {
                        _isProgress = false;
                        _limitTimerCoroutine = Managers.Manager.StartCoroutine(CoLimitTimer());
                        Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                        Managers.Monster.SetAllMonsterState(CharacterState.Idle);
                    });
                }).AddTo(_compositeDisposable).AddTo(FadeScreen.Instance.CompositeDisposable);
        }, 0f);

        return true;
    }

    private IEnumerator CoLimitTimer()
    {
        while (true)
        {
            if (State.Value != StageState.StageBoss &&
                State.Value != StageState.Dungeon)
                yield break;
            
            if (_isClearStage)
                yield break;

            yield return null;

            var timeSpan = StageRemainTime.Value.Add(TimeSpan.FromSeconds(-Time.deltaTime));
            StageRemainTime.Value = timeSpan;

            if (StageRemainTime.Value.TotalSeconds > 0f) 
                continue;
            
            switch (State.Value)
            {
                case StageState.StageBoss:
                    FailStage();
                    break;
            }
            yield break;
        }
    }

    public void ClearStage()
    {
        if (_isClearStage)
            return;

        if (_limitTimerCoroutine != null)
            Managers.Manager.StopCoroutine(_limitTimerCoroutine);
        
        Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
        {
            if (State.Value != StageState.StageBoss)
                return;
            
            Managers.Game.MainPlayer.State.Value = CharacterState.None;
            Managers.Monster.SetAllMonsterStateNone();

            // 마지막 스테이지라면
            if (Utils.IsCurrentStageIsMaxStage())
            {
                FadeScreen.GameViewFadeOut(() =>
                {
                    State.Value = StageState.Normal;
                    KillCount.Value = 0;
                    StageId.Value++;
                    Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
                    {
                        FadeScreen.GameViewFadeIn(() => { Managers.Game.MainPlayer.State.Value = CharacterState.Idle; });
                    });
                }, 0.5f);
            }
            
            if (StageRemainTime.Value.TotalSeconds >= StageLimitTime.TotalSeconds * 0.5 && IsAutoBoss.Value)
            {
                _isProgress = false;
                StageId.Value += 1;
                StartBoss(true);
                return;
            }
        
            FadeScreen.GameViewFadeOut(() =>
            {
                State.Value = StageState.Normal;
                KillCount.Value = 0;
                StageId.Value++;
                Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
                {
                    FadeScreen.GameViewFadeIn(() => { Managers.Game.MainPlayer.State.Value = CharacterState.Idle; });
                }).AddTo(FadeScreen.Instance.CompositeDisposable);
            }, 0.5f);
        }).AddTo(_compositeDisposable).AddTo(FadeScreen.Instance.CompositeDisposable);
    }

    public void FailStage()
    {
        if (_isFail)
            return;

        _isFail = true;
        
        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        Managers.Monster.SetAllMonsterStateNone();
        
        IsAutoBoss.Value = false;

        FadeScreen.GameViewFadeOut(() =>
        {
            State.Value = StageState.Normal;
            KillCount.Value = 0;
            Managers.Monster.StartSpawn();
            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
            {
                FadeScreen.GameViewFadeIn(() =>
                {
                    _isFail = false;
                    Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                });
            });
        });
    }

    public void CheckBoss()
    {
        if (IsAutoBoss.Value &&
            KillCount.Value >= ChartManager.StageDataController.StageDataTable[StageId.Value].NeedBossChallengeKillCount)
            StartBoss();
    }
}