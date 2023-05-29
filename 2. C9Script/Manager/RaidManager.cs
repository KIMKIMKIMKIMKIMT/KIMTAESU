using System;
using System.Collections;
using BackEnd;
using Chart;
using Cinemachine;
using CodeStage.AntiCheat.ObscuredTypes;
using GameData;
using UniRx;
using UnityEngine;

public class RaidManager
{
    public RaidClearInfo RaidClearInfo;

    public readonly ReactiveProperty<int> Step = new(1);
    public readonly ReactiveProperty<ObscuredInt> Wave = new(0);
    public readonly ReactiveProperty<int> KillCount = new(0);

    public readonly ReactiveProperty<double> GainGold = new(0);
    public readonly ReactiveProperty<double> GainGoldBar = new(0);
    public readonly ReactiveProperty<double> GainSkillStone = new(0);

    public readonly ReactiveProperty<float> Wave3RemainTime = new(0);
    public float Wave3LimitTime;
    public readonly ReactiveProperty<float> PlayTime = new(0);

    private RaidDungeonChart _raidDungeonChart;

    private bool _isStart;
    public bool IsProgress;
    private bool _isEnd;

    private CinemachineVirtualCamera _gameCamera;

    private CinemachineVirtualCamera GameCamera
    {
        get
        {
            if (_gameCamera != null)
                return _gameCamera;

            _gameCamera = GameObject.FindWithTag("GameCamera").GetComponent<CinemachineVirtualCamera>();
            return _gameCamera;
        }
    }

    public void Init()
    {
        KillCount.Subscribe(ChangeKillCount);

        Managers.Stage.State.Where(state => state != StageState.Raid).Subscribe(_ =>
        {
            _isStart = false;
            _isEnd = false;
            IsProgress = false;
        });
    }

    private void ChangeKillCount(int killCount)
    {
        if (!IsProgress)
            return;

        switch (Wave.Value)
        {
            case 1:
            {
                GainGold.Value += _raidDungeonChart.Wave1MonsterGold *
                                  Managers.Game.BaseStatDatas[(int)StatType.IncreaseGold];

                if (killCount >= _raidDungeonChart.Wave1ClearCount)
                {
                    IsProgress = false;
                    Managers.Game.MainPlayer.SetRaidPortalMove(new Vector2(0, 10.5f));
                }
            }
                break;
            case 2:
            {
                GainGoldBar.Value += _raidDungeonChart.Wave2MonsterGoldBar;

                if (killCount >= _raidDungeonChart.Wave2ClearCount)
                {
                    IsProgress = false;
                    Managers.Game.MainPlayer.SetRaidPortalMove(new Vector2(0, 10.5f));
                }
            }
                break;
            case 3:
            {
                GainSkillStone.Value += _raidDungeonChart.Wave3SkillStone;

                if (killCount >= 1)
                {
                    Clear();
                }
            }
                break;
        }
    }

    public void StartNextWave()
    {
        switch (Wave.Value)
        {
            case 1:
            {
                Managers.Game.MainPlayer.State.Value = CharacterState.None;

                FadeScreen.FadeOut(() =>
                {
                    Managers.Game.MainPlayer.transform.position = Vector3.zero;
                    Wave.Value = 2;
                    KillCount.Value = 0;
                    FadeScreen.FadeIn(() =>
                    {
                        IsProgress = true;
                        Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                        Managers.Monster.StartSpawn();
                    });
                });
            }
                break;
            case 2:
            {
                Managers.Game.MainPlayer.State.Value = CharacterState.None;

                FadeScreen.FadeOut(() =>
                {
                    if (ChartManager.DungeonCharts.TryGetValue((int)DungeonType.Raid, out var dungeonChart))
                    {
                        if (ChartManager.WorldCharts.TryGetValue(dungeonChart.WorldId, out var worldChart))
                            Managers.Stage.SetBg($"{worldChart.BgName}_2");   
                    }
                    Managers.Game.MainPlayer.transform.position = Vector3.zero;
                    Wave.Value = 3;
                    KillCount.Value = 0;
                    FadeScreen.FadeIn(() =>
                    {
                        IsProgress = true;
                        Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                        Managers.Monster.StartSpawn();
                    });
                });
            }
                break;
            case 3:
            {
                Clear();
            }

                break;
        }
    }

    public void Start()
    {
        if (_isStart)
            return;

        if (Utils.IsWeeklyCalculateRankTime())
        {
            Managers.Message.ShowMessage("랭킹 정산중 입니다.");
            return;
        }
        
        if (!ChartManager.RaidDungeonCharts.TryGetValue(Step.Value, out _raidDungeonChart))
            return;

        _isStart = true;

        Managers.Game.MainPlayer.State.Value = CharacterState.None;

        FadeScreen.FadeOut(() =>
        {
            Managers.UI.CloseAllPopupUI();
            Managers.Monster.ClearSpawnMonster();

            GainGold.Value = 0;
            GainGoldBar.Value = 0;
            GainSkillStone.Value = 0;

            PlayTime.Value = 0;

            KillCount.Value = 0;

            Wave3LimitTime = _raidDungeonChart.Wave3LimitTime;
            Wave3RemainTime.Value = Wave3LimitTime;

            Wave.Value = 1;
            Managers.Stage.State.Value = StageState.Raid;

            Managers.Game.MainPlayer.transform.position = Vector3.zero;

            GameCamera.m_Lens.OrthographicSize = 14.5f;
            
            Managers.Sound.PlayBgm(BgmType.Raid);

            Managers.Game.MainPlayer.InitPassiveCount();

            if (ChartManager.DungeonCharts.TryGetValue((int)DungeonType.Raid, out var dungeonChart))
            {
                if (ChartManager.WorldCharts.TryGetValue(dungeonChart.WorldId, out var worldChart))
                    Managers.Stage.SetBg($"{worldChart.BgName}_1");   
            }

            FadeScreen.FadeIn(() =>
            {
                Managers.Monster.StartSpawn();
                Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                _isStart = false;
                IsProgress = true;
                MainThreadDispatcher.StartCoroutine(CoPlayTimer());
            });
        }, 0f);
    }

    private IEnumerator CoPlayTimer()
    {
        while (true)
        {
            if (_isEnd)
                yield break;

            yield return null;
            
            if (IsProgress)
                PlayTime.Value += Time.deltaTime;

            if (!(PlayTime.Value >= 900)) 
                continue;
            
            Fail();
            yield break;
        }
    }

    private void Clear()
    {
        if (_isEnd)
            return;

        _isEnd = true;

        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        IsProgress = false;

        if (Utils.IsWeeklyCalculateRankTime())
        {
            Managers.Message.ShowMessage("랭킹 정산중이라 반영되지 않습니다.");
            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
            {
                FadeScreen.FadeOut(() =>
                {
                    Managers.UI.CloseAllPopupUI();
                    Managers.Monster.ClearSpawnMonster();
                    Managers.Stage.State.Value = StageState.Normal;
                    Managers.Monster.StartSpawn();
                    Managers.Game.MainPlayer.transform.position = Vector3.zero;
                    _isEnd = false;

                    FadeScreen.FadeIn(() => { Managers.Game.MainPlayer.State.Value = CharacterState.Idle; });
                });
            });

            return;
        }

        var clearRaidPopup = Managers.UI.ShowPopupUI<UI_ClearRaidPopup>();
        if (clearRaidPopup != null)
            clearRaidPopup.Init(Step.Value, PlayTime.Value, GainGold.Value, GainGoldBar.Value, GainSkillStone.Value);

        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.Gold, GainGold.Value);
        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.GoldBar, GainGoldBar.Value);
        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.SkillEnhancementStone, GainSkillStone.Value);
        
        if (ChartManager.DungeonCharts.TryGetValue((int)DungeonType.Raid, out var dungeonChart))
            Managers.Game.DecreaseItem(ItemType.Goods, dungeonChart.EntryItemId, 1);

        GameDataManager.GoodsGameData.SaveGameData();

        var isChangeClearInfo = false;

        if (RaidClearInfo == null)
        {
            RaidClearInfo = new RaidClearInfo
            {
                ClearStep = Step.Value,
                ClearTime = PlayTime.Value,
                HighestGold = GainGold.Value,
                HighestGoldBar = GainGoldBar.Value,
                HighestSkillStone = GainSkillStone.Value
            };

            isChangeClearInfo = true;
        }
        else if (Step.Value > RaidClearInfo.ClearStep)
        {
            RaidClearInfo.ClearStep = Step.Value;
            RaidClearInfo.ClearTime = PlayTime.Value;
            RaidClearInfo.HighestGold = GainGold.Value;
            RaidClearInfo.HighestGoldBar = GainGoldBar.Value;
            RaidClearInfo.HighestSkillStone = GainSkillStone.Value;

            isChangeClearInfo = true;
        }
        else if (Step.Value == RaidClearInfo.ClearStep)
        {
            if (PlayTime.Value < RaidClearInfo.ClearTime)
            {
                RaidClearInfo.ClearTime = PlayTime.Value;
                RaidClearInfo.HighestGold = GainGold.Value;
                RaidClearInfo.HighestGoldBar = GainGoldBar.Value;
                RaidClearInfo.HighestSkillStone = GainSkillStone.Value;
                isChangeClearInfo = true;
            }
        }

        if (isChangeClearInfo)
        {
            GameDataManager.RankRaidGameData.SaveGameData();
            Managers.Rank.IsRefreshRankRaidFlag = true;
        }

        var param = new Param
        {
            { "ClearStep", Step.Value },
            { "ClearTime", PlayTime.Value },
            { "GainGold", GainGold.Value },
            { "GainGoldBar", GainGoldBar.Value },
            { "GainSkillStone", GainSkillStone.Value }
        };

        Utils.GetGoodsLog(ref param);

        Backend.GameLog.InsertLog("Raid", param);

        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
        {
            FadeScreen.FadeOut(() =>
            {
                Managers.UI.CloseAllPopupUI();
                Managers.Monster.ClearSpawnMonster();
                Managers.Stage.State.Value = StageState.Normal;
                Managers.Monster.StartSpawn();
                Managers.Game.MainPlayer.transform.position = Vector3.zero;
                _isEnd = false;

                FadeScreen.FadeIn(() => { Managers.Game.MainPlayer.State.Value = CharacterState.Idle; });
            });
        });
    }

    public void Fail()
    {
        if (_isEnd)
            return;

        _isEnd = true;
        IsProgress = false;
        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        Managers.Monster.SetAllMonsterStateNone();

        Managers.UI.ShowPopupUI<UI_FailDungeonPopup>();

        Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(_ =>
        {
            FadeScreen.FadeOut(() =>
            {
                Managers.Game.MainPlayer.SetAllSkillCoolTime();
                Wave.Value = 0;
                Managers.UI.CloseAllPopupUI();
                Managers.Stage.State.Value = StageState.Normal;
                Managers.Game.MainPlayer.transform.position = Vector3.zero;
                Managers.Monster.StartSpawn();
                _isEnd = false;

                FadeScreen.FadeIn(() => { Managers.Game.MainPlayer.State.Value = CharacterState.Idle; });
            });
        });
    }

    public void StartWave3Timer()
    {
        MainThreadDispatcher.StartCoroutine(CoWave3Timer());
    }

    private IEnumerator CoWave3Timer()
    {
        while (true)
        {
            yield return null;
            Wave3RemainTime.Value -= Time.deltaTime;

            if (!IsProgress)
                yield break;

            if (Wave3RemainTime.Value > 0)
                continue;

            Fail();
            yield break;
        }
    }

    public string GetRaidBossName()
    {
        if (_raidDungeonChart == null)
            return string.Empty;

        return !ChartManager.MonsterCharts.TryGetValue(_raidDungeonChart.Wave3MonsterId, out var monsterChart)
            ? string.Empty
            : ChartManager.GetString(monsterChart.Name);
    }
}