using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using Chart;
using Cinemachine;
using CodeStage.AntiCheat.ObscuredTypes;
using GameData;
using UniRx;
using UnityEngine;

public class GuildRaidManager
{
    public ObscuredInt EntryCount;
    public GuildRaidClearInfo GuildRaidClearInfo;

    public readonly ReactiveProperty<int> Step = new(1);
    public readonly ReactiveProperty<ObscuredInt> Wave = new(0);
    public readonly ReactiveProperty<int> KillCount = new(0);

    public readonly ReactiveProperty<double> GainGold = new(0);
    public readonly ReactiveProperty<double> GainGoldBar = new(0);
    public readonly ReactiveProperty<double> GainGuildPoint = new(0);

    public readonly ReactiveProperty<float> Wave3RemainTime = new(0);
    public float Wave3LimitTime;
    public readonly ReactiveProperty<float> PlayTime = new(0);

    private GuildRaidDungeonChart _guildRaidDungeonChart;

    private bool _isStart;
    public bool IsProgress;
    private bool _isEnd;

    private List<Player> _partyPlayers = new();

    private CinemachineVirtualCamera _gameCamera;

    private List<(string, double)> _sortGuildMembers = new();

    private Vector2[] PartyPlayerSpawnPositions = new[]
    {
        new Vector2(0, 3),
        new Vector2(-2, -2),
        new Vector2(2, -2)
    };

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

        Managers.Stage.State.Where(state => state != StageState.GuildRaid).Subscribe(_ =>
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
                GainGold.Value += _guildRaidDungeonChart.Wave1MonsterGold *
                                  Managers.Game.BaseStatDatas[(int)StatType.IncreaseGold];

                if (killCount >= _guildRaidDungeonChart.Wave1ClearCount)
                {
                    IsProgress = false;
                    Managers.Game.MainPlayer.SetRaidPortalMove(new Vector2(0, 10.5f));
                }
            }
                break;
            case 2:
            {
                GainGoldBar.Value += _guildRaidDungeonChart.Wave2GoldBar;

                if (killCount >= _guildRaidDungeonChart.Wave2ClearCount)
                {
                    IsProgress = false;
                    Managers.Game.MainPlayer.SetRaidPortalMove(new Vector2(0, 10.5f));
                    SetPartyPlayerState(CharacterState.None);
                }
            }
                break;
            case 3:
            {
                // 단계 x (900 - 경과시간(초)) x 0.1
                GainGuildPoint.Value = Math.Round(Step.Value * (900 - PlayTime.Value) * 0.1);

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
                SetPartyPlayerState(CharacterState.None);

                FadeScreen.FadeOut(() =>
                {
                    Managers.Game.MainPlayer.transform.position = Vector3.zero;
                    ResetPartyPlayerPosition();
                    Wave.Value = 2;
                    KillCount.Value = 0;
                    FadeScreen.FadeIn(() =>
                    {
                        IsProgress = true;
                        Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                        SetPartyPlayerState(CharacterState.Idle);
                        Managers.Monster.StartSpawn();
                    });
                });
            }
                break;
            case 2:
            {
                Managers.Game.MainPlayer.State.Value = CharacterState.None;
                SetPartyPlayerState(CharacterState.None);

                FadeScreen.FadeOut(() =>
                {
                    if (ChartManager.DungeonCharts.TryGetValue((int)DungeonType.Raid, out var dungeonChart))
                    {
                        if (ChartManager.WorldCharts.TryGetValue(dungeonChart.WorldId, out var worldChart))
                            Managers.Stage.SetBg($"{worldChart.BgName}_2");
                    }

                    Managers.Game.MainPlayer.transform.position = Vector3.zero;
                    ResetPartyPlayerPosition();
                    Wave.Value = 3;
                    KillCount.Value = 0;
                    FadeScreen.FadeIn(() =>
                    {
                        IsProgress = true;
                        Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                        SetPartyPlayerState(CharacterState.Idle);
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

        if (!ChartManager.GuildRaidDungeonCharts.TryGetValue(Step.Value, out _guildRaidDungeonChart))
            return;

        _isStart = true;

        Managers.Game.MainPlayer.State.Value = CharacterState.None;

        FadeScreen.FadeOut(() =>
        {
            Managers.UI.CloseAllPopupUI();
            Managers.Monster.ClearSpawnMonster();

            GainGold.Value = 0;
            GainGoldBar.Value = 0;
            GainGuildPoint.Value = 0;

            PlayTime.Value = 0;

            KillCount.Value = 0;

            Wave3LimitTime = _guildRaidDungeonChart.Wave3LimitTime;
            Wave3RemainTime.Value = Wave3LimitTime;

            Wave.Value = 1;
            Managers.Stage.State.Value = StageState.GuildRaid;

            SortStrongGuildMember();
            MakePartyPlayer();

            Managers.Game.MainPlayer.transform.position = Vector3.zero;

            GameCamera.m_Lens.OrthographicSize = 14.5f;

            Managers.Sound.PlayBgm(BgmType.Raid);

            if (ChartManager.DungeonCharts.TryGetValue((int)DungeonType.Raid, out var dungeonChart))
            {
                if (ChartManager.WorldCharts.TryGetValue(dungeonChart.WorldId, out var worldChart))
                    Managers.Stage.SetBg($"{worldChart.BgName}_1");
            }

            FadeScreen.FadeIn(() =>
            {
                Managers.Monster.StartSpawn();
                Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                SetPartyPlayerState(CharacterState.Idle);
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
        SetPartyPlayerState(CharacterState.None);

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
                    PoolManager.Instance.PlayerAllOffObj();
                    _isEnd = false;

                    FadeScreen.FadeIn(() => { Managers.Game.MainPlayer.State.Value = CharacterState.Idle; });
                });
            });

            return;
        }

        FadeScreen.Instance.OnLoadingScreen();

        Backend.URank.Guild.ContributeGuildGoods(
            Managers.Rank.RankUUIDDictionary[(RankType.Guild, Managers.Server.CurrentServer)],
            goodsType.goods2, (int)GainGuildPoint.Value,
            bro =>
            {
                FadeScreen.Instance.OffLoadingScreen();

                if (bro.IsSuccess())
                {
                    // 레이드 경험치 기부 성공 후 길드내 굿즈 데이터 1회 갱신 
                    Managers.Guild.GetMyGuildGoodsData(() =>
                    {
                        var clearRaidPopup = Managers.UI.ShowPopupUI<UI_ClearRaidPopup>();
                        if (clearRaidPopup != null)
                            clearRaidPopup.Init(Step.Value, PlayTime.Value, GainGold.Value, GainGoldBar.Value,
                                GainGuildPoint.Value);

                        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.Gold, GainGold.Value);
                        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.GoldBar, GainGoldBar.Value);

                        Managers.Guild.GuildMemberDatas[Backend.UserInDate].GoodsDic[(int)GuildGoodsType.Rank] +=
                            (int)GainGuildPoint.Value;

                        var param = new Param()
                        {
                            {
                                "GuildName",
                                Managers.Guild.GuildData != null ? Managers.Guild.GuildData.GuildName : string.Empty
                            },
                            { "Type", "Raid" },
                            { "Step", Step.Value },
                            { "Gold", GainGold.Value },
                            { "GoldBar", GainGoldBar.Value },
                            { "Point", GainGuildPoint.Value }
                        };

                        Backend.GameLog.InsertLog("Guild", param);

                        Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.GuildRaidTicket, 1);

                        GameDataManager.GoodsGameData.SaveGameData();

                        if (GuildRaidClearInfo == null)
                        {
                            GuildRaidClearInfo = new GuildRaidClearInfo
                            {
                                ClearStep = Step.Value,
                                ClearTime = PlayTime.Value,
                                HighestGold = GainGold.Value,
                                HighestGoldBar = GainGoldBar.Value,
                                HighestGuildPoint = GainGuildPoint.Value
                            };
                        }
                        else if (Step.Value > GuildRaidClearInfo.ClearStep)
                        {
                            GuildRaidClearInfo.ClearStep = Step.Value;
                            GuildRaidClearInfo.ClearTime = PlayTime.Value;
                            GuildRaidClearInfo.HighestGold = GainGold.Value;
                            GuildRaidClearInfo.HighestGoldBar = GainGoldBar.Value;
                            GuildRaidClearInfo.HighestGuildPoint = GainGuildPoint.Value;
                        }
                        else if (Step.Value == GuildRaidClearInfo.ClearStep)
                        {
                            if (PlayTime.Value < GuildRaidClearInfo.ClearTime)
                            {
                                GuildRaidClearInfo.ClearTime = PlayTime.Value;
                                GuildRaidClearInfo.HighestGold = GainGold.Value;
                                GuildRaidClearInfo.HighestGoldBar = GainGoldBar.Value;
                                GuildRaidClearInfo.HighestGuildPoint = GainGuildPoint.Value;
                            }
                        }

                        GameDataManager.GuildGameData.SaveGameData();

                        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
                        {
                            FadeScreen.FadeOut(() =>
                            {
                                Managers.UI.CloseAllPopupUI();
                                Managers.Monster.ClearSpawnMonster();
                                Managers.Stage.State.Value = StageState.Normal;
                                Managers.Monster.StartSpawn();
                                Managers.Game.MainPlayer.transform.position = Vector3.zero;
                                PoolManager.Instance.PlayerAllOffObj();
                                _isEnd = false;

                                FadeScreen.FadeIn(() =>
                                {
                                    Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                                });
                            });
                        });
                    });

                    // Param param = new Param();
                    //
                    // param.Add("ClearStep", Step.Value);
                    // param.Add("ClearTime", PlayTime.Value);
                    // param.Add("GainGold", GainGold.Value);
                    // param.Add("GainGoldBar", GainGoldBar.Value);
                    // param.Add("GainSkillStone", GainSkillStone.Value);
                    // Utils.GetGoodsLog(ref param);
                    //
                    // Backend.GameLog.InsertLog("Raid", param);
                }
                else
                {
                    Utils.GetErrorReason(bro, out string statusCode, out string errorCode, out string message);

                    // 길드에 가입되자 않은 유저
                    if (statusCode.Equals("412") && errorCode.Contains("PreconditionFailed") &&
                        message.Contains("notGuildMember"))
                    {
                        Managers.Message.ShowMessage("가입한 길드 정보가 존재하지 않습니다.");
                    }
                    // 랭킹 정산 중
                    else if (statusCode.Equals("428") && errorCode.Contains("Precondition Required") &&
                             message.Contains("Precondition"))
                    {
                        Managers.Message.ShowMessage("랭킹 정산 중 입니다.");
                    }
                    else if (statusCode.Equals("404") && errorCode.Contains("NotFoundException") &&
                             message.Contains("guild rank"))
                    {
                        Managers.Message.ShowMessage("잘못된 호출 입니다.");
                    }
                    else if (statusCode.Equals("400"))
                    {
                        if ((errorCode.Contains("ValidationException") && message.Contains("rankUuid")) ||
                            errorCode.Contains("BadParameterException") && message.Contains("bad table") ||
                            (errorCode.Contains("PreconditionFailed") && message.Contains("value only")))
                        {
                            Managers.Message.ShowMessage("잘못된 호출 입니다.");
                        }
                        else
                            Managers.Backend.FailLog(bro, "Fail ContributeGuildGoods");
                    }
                    else
                        Managers.Backend.FailLog(bro, "Fail ContributeGuildGoods");

                    Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
                    {
                        FadeScreen.FadeOut(() =>
                        {
                            Managers.UI.CloseAllPopupUI();
                            Managers.Monster.ClearSpawnMonster();
                            Managers.Stage.State.Value = StageState.Normal;
                            Managers.Monster.StartSpawn();
                            Managers.Game.MainPlayer.transform.position = Vector3.zero;
                            PoolManager.Instance.PlayerAllOffObj();
                            _isEnd = false;

                            FadeScreen.FadeIn(() => { Managers.Game.MainPlayer.State.Value = CharacterState.Idle; });
                        });
                    });
                }
            });
    }

    public void Fail()
    {
        if (_isEnd)
            return;

        _isEnd = true;
        IsProgress = false;
        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        SetPartyPlayerState(CharacterState.None);
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
                PoolManager.Instance.PlayerAllOffObj();
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
        if (_guildRaidDungeonChart == null)
            return string.Empty;

        return !ChartManager.MonsterCharts.TryGetValue(_guildRaidDungeonChart.Wave3MonsterId, out var monsterChart)
            ? string.Empty
            : ChartManager.GetString(monsterChart.Name);
    }

    private void SortStrongGuildMember()
    {
        _sortGuildMembers.Clear();

        foreach (var guildMemberData in Managers.Guild.GuildMemberDatas.Values)
        {
            if (guildMemberData.InDate == BackEnd.Backend.UserInDate)
                continue;

            var power = Utils.GetPower(guildMemberData.StatDatas, guildMemberData.PromoGrade);
            _sortGuildMembers.Add((guildMemberData.InDate, power));
        }

        _sortGuildMembers = _sortGuildMembers.OrderByDescending(data => data.Item2).ToList();
    }

    private void MakePartyPlayer()
    {
        _partyPlayers.Clear();

        for (int i = 0; i < 3; i++)
        {
            if (_sortGuildMembers.Count <= i)
                continue;

            Player partyPlayer = PoolManager.Instance.GetPlayer(0);
            if (partyPlayer == null)
                continue;

            //var partyPlayer = obj.GetComponent<Player>();
            //if (partyPlayer == null)
            //    continue;

            partyPlayer.IsPartyPlayer = true;
            partyPlayer.StatDatas = Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1].StatDatas;
            partyPlayer.PartyPlayer_Passive_Level = Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1].Passive_Godgod_Level;
            partyPlayer.Initialize();
            partyPlayer.transform.position = Vector3.zero;
            partyPlayer.transform.localScale = Vector3.one;
            partyPlayer.State.Value = CharacterState.None;

            partyPlayer.SetWeapon(Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1]
                .EquipData[EquipType.Weapon]);
            partyPlayer.SetCostume(Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1]
                .EquipData[EquipType.ShowCostume]);
            partyPlayer.SetPet(Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1].EquipData[EquipType.Pet]);
            partyPlayer.SetNickname(Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1].Nickname);

            _partyPlayers.Add(partyPlayer);
        }

        ResetPartyPlayerPosition();
    }

    private void SetPartyPlayerState(CharacterState state)
    {
        _partyPlayers.ForEach(player =>
        {
            if (player == null)
                return;

            player.State.Value = state;
        });
    }

    private void ResetPartyPlayerPosition()
    {
        for (int i = 0; i < _partyPlayers.Count; i++)
        {
            if (i >= PartyPlayerSpawnPositions.Length)
                continue;

            if (_partyPlayers[i] == null)
                continue;

            _partyPlayers[i].transform.position = PartyPlayerSpawnPositions[i];
        }
    }

    private void DestroyPartyPlayers()
    {
        _partyPlayers.ForEach(player =>
        {
            if (player == null)
                return;

            Managers.Resource.Destroy(player.gameObject);
        });

        _partyPlayers.Clear();
    }
}