using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using BackEnd;
using Chart;
using GameData;
using Cinemachine;
using UniRx.Triggers;
using CodeStage.AntiCheat.ObscuredTypes;
using System;


public class AllRaidManager
{
    #region Fields
    public ObscuredInt EntryCount;
    public GuildAllRaidClearInfo GuildAllRaidClearInfo;

    public readonly ReactiveProperty<float> PlayTime = new(0);
    public readonly ReactiveProperty<int> Step = new(1);
    public readonly ReactiveProperty<int> Wave = new(0);

    public readonly ReactiveProperty<double> GainGold = new(0);
    public readonly ReactiveProperty<double> GainGoldBar = new(0);
    public readonly ReactiveProperty<double> GainGuildPoint = new(0);

    public readonly ReactiveProperty<float> RemainTime = new(0);
    public float LimitTime;

    public bool IsAllRaidSkillLock;

    
    private CinemachineVirtualCamera _gameCamera;
    public CinemachineTargetGroup TargetGroup;

    private AllGuildRaidDungeonChart _allGuildRaidDungeonChart;

    private List<(string, double)> _sortGuildMembers = new();
    private List<Player> _partyPlayers = new();
    private List<Player> _allPlayers = new();

    private Vector2[] _partyPlayerSpawnPositions = new[]
    {
        new Vector2(0, 2), new Vector2(0, -2), new Vector2(0, 4), new Vector2(0, -4),new Vector2(0, 6), new Vector2(0, -6),
        new Vector2(-2, 0), new Vector2(-2, 2), new Vector2(-2, -2),new Vector2(-2, 4), new Vector2(-2, -4), new Vector2(-2, 6), new Vector2(-2, -6),
        new Vector2(-4, 0), new Vector2(-4, 2),new Vector2(-4, -2),new Vector2(-4, 4),new Vector2(-4, -4),new Vector2(-4, 6), new Vector2(-4, -6),
        new Vector2(-6, 0), new Vector2(-6, 2),new Vector2(-6, -2),new Vector2(-6, 4),new Vector2(-6, -4),new Vector2(-6, 6), new Vector2(-6, -6),
        new Vector2(-8, 0),new Vector2(-8, 2)
    };
    private Vector2 _pos = new Vector2(30, 0);

    private double _gainGold;
    private double _gainGoldbar;

    private bool _isEnd;

    private const float MAX_CAM_SIZE = 35f;

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
    public bool IsPartyPlayerReady { get; private set; }

    public List<Player> PartyPlayers { get { return _partyPlayers; } }
    public List<Player> AllPlayers { get { return _allPlayers; } }
    #endregion

    #region Private Methods
    private void SortStrongGuildMember()
    {
        _sortGuildMembers.Clear();

        foreach (var guildMemberData in Managers.Guild.GuildMemberDatas.Values)
        {
            if (guildMemberData.InDate == BackEnd.Backend.UserInDate)
                continue;

            if (guildMemberData.InDate == null)
                continue;

            var power = Utils.GetPower(guildMemberData.StatDatas, guildMemberData.PromoGrade);
            _sortGuildMembers.Add((guildMemberData.InDate, power));
        }
    }

    private void MakePartyPlayer()
    {
        _allPlayers.Clear();
        _partyPlayers.Clear();

        for (int i = 0; i < _sortGuildMembers.Count; i++)
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
            partyPlayer.InitializeAllRaid();
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
            _allPlayers.Add(partyPlayer);
        }

        _allPlayers.Add(Managers.Game.MainPlayer);

        ResetPartyPlayerPosition();
    }

    private void ResetPartyPlayerPosition()
    {
        for (int i = 0; i < _partyPlayers.Count; i++)
        {
            if (i >= _partyPlayerSpawnPositions.Length)
                continue;

            if (_partyPlayers[i] == null)
                continue;

            _partyPlayers[i].transform.position = _partyPlayerSpawnPositions[i];
        }
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

    private void MoveToAllPlayer()
    {
        for (int i = 0; i < _allPlayers.Count; i++)
        {
            _allPlayers[i].SetAllRaidRunMove();
        }
    }

    private void RunToCircle()
    {
        for (int i = 0; i < _allPlayers.Count; i++)
        {
            Vector3 circleDir = new Vector3(_pos.x + Mathf.Cos(Mathf.PI * 2 * i / _allPlayers.Count) * 30,
                _pos.y + Mathf.Sin(Mathf.PI * 2 * i / _allPlayers.Count) * 30, 0);

            _allPlayers[i].SetAllRaidCircleMove(circleDir);
        }
    }

    //단계 x( 200 - 경과시간(초)) x 0.1
    private void ClearRewardSet()
    {
        GainGold.Value += _allGuildRaidDungeonChart.GoldValue *
           Managers.Game.BaseStatDatas[(int)StatType.IncreaseGold];

        GainGoldBar.Value += _allGuildRaidDungeonChart.GoldBarValue;

        GainGuildPoint.Value = Math.Round(Step.Value * (200 - PlayTime.Value) * 0.1);
    }

    private void Init()
    {

    }

    

    public void Fail()
    {
        _isEnd = true;
        IsPartyPlayerReady = false;
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
                _gameCamera.m_Follow = Managers.Game.MainPlayer.transform;
                Managers.Monster.StartSpawn();
                PoolManager.Instance.PlayerAllOffObj();
                _isEnd = false;
                Managers.Stage.SetBGScale(new Vector3(1.5f, 1.5f, 1));

                FadeScreen.FadeIn(() =>
                {
                    Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                    Managers.Game.MainPlayer.IsDie = false;
                });
            });
        });
    }

    private void DestroyPartyPlayers()
    {
        _partyPlayers.ForEach(player =>
        {
            if (player == null)
                return;

            player.StopAllCoroutines();
            Managers.Resource.Destroy(player.gameObject);
        });

        _partyPlayers.Clear();
        _allPlayers.Clear();
    }
    #endregion

    #region Public Methods
    public void Clear()
    {
        if (_isEnd)
            return;

        _isEnd = true;

        ClearRewardSet();
        IsPartyPlayerReady = false;
        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        SetPartyPlayerState(CharacterState.None);

        FadeScreen.Instance.OnLoadingScreen();

        Backend.URank.Guild.ContributeGuildGoods(
            Managers.Rank.RankUUIDDictionary[(RankType.GuildAllRaid, Managers.Server.CurrentServer)],
            goodsType.goods3, (int)GainGuildPoint.Value,
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

                        Managers.Guild.GuildMemberDatas[Backend.UserInDate].GoodsDic[(int)GuildGoodsType.AllRaidRank] +=
                            (int)GainGuildPoint.Value;

                        Param param = new Param()
                        {
                        {
                            "GuildName",
                            Managers.Guild.GuildData != null ? Managers.Guild.GuildData.GuildName : string.Empty
                        },
                        { "Type", "AllRaid" },
                        { "Step", Step.Value },
                        { "Gold", GainGold.Value },
                        { "GoldBar", GainGoldBar.Value },
                        { "Point", GainGuildPoint.Value }
                        };

                        Backend.GameLog.InsertLog("GuildAllRaid", param);

                        Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.GuildAllRaidTicket, 1);

                        GameDataManager.GoodsGameData.SaveGameData();

                        if (GuildAllRaidClearInfo == null)
                        {
                            GuildAllRaidClearInfo = new GuildAllRaidClearInfo
                            {
                                ClearStep = Step.Value,
                                ClearTime = PlayTime.Value,
                                HighestGold = GainGold.Value,
                                HighestGoldBar = GainGoldBar.Value,
                                HighestGuildPoint = GainGuildPoint.Value
                            };
                        }
                        else if (Step.Value > GuildAllRaidClearInfo.ClearStep)
                        {
                            GuildAllRaidClearInfo.ClearStep = Step.Value;
                            GuildAllRaidClearInfo.ClearTime = PlayTime.Value;
                            GuildAllRaidClearInfo.HighestGold = GainGold.Value;
                            GuildAllRaidClearInfo.HighestGoldBar = GainGoldBar.Value;
                            GuildAllRaidClearInfo.HighestGuildPoint = GainGuildPoint.Value;
                        }
                        else if (Step.Value == GuildAllRaidClearInfo.ClearStep)
                        {
                            if (PlayTime.Value < GuildAllRaidClearInfo.ClearTime)
                            {
                                GuildAllRaidClearInfo.ClearTime = PlayTime.Value;
                                GuildAllRaidClearInfo.HighestGold = GainGold.Value;
                                GuildAllRaidClearInfo.HighestGoldBar = GainGoldBar.Value;
                                GuildAllRaidClearInfo.HighestGuildPoint = GainGuildPoint.Value;
                            }
                        }

                        GameDataManager.GuildAllRaidGameData.SaveGameData();

                        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
                        {
                            FadeScreen.FadeOut(() =>
                            {
                                Managers.UI.CloseAllPopupUI();
                                Managers.Monster.ClearSpawnMonster();
                                Managers.Stage.State.Value = StageState.Normal;
                                Managers.Monster.StartSpawn();
                                Managers.Game.MainPlayer.transform.position = Vector3.zero;
                                _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.BaseCameraSize;
                                _gameCamera.m_Follow = Managers.Game.MainPlayer.transform;
                                PoolManager.Instance.PlayerAllOffObj();
                                _isEnd = false;
                                Managers.Stage.SetBGScale(new Vector3(1.5f, 1.5f, 1));

                                FadeScreen.FadeIn(() =>
                                {
                                    Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                                    Managers.Game.MainPlayer.IsDie = false;
                                });
                            });
                        });
                    });
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
                            _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.BaseCameraSize;
                            _gameCamera.m_Follow = Managers.Game.MainPlayer.transform;
                            PoolManager.Instance.PlayerAllOffObj();
                            _isEnd = false;
                            IsPartyPlayerReady = false;
                            Managers.Stage.SetBGScale(new Vector3(1.5f, 1.5f, 1));

                            FadeScreen.FadeIn(() =>
                            {
                                Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                                Managers.Game.MainPlayer.IsDie = false;
                            });
                        });
                    });
                }
            });
    }

    public void AllRaidStart()
    {
        if (!ChartManager.AllGuildRaidDungeonCharts.TryGetValue(Step.Value, out _allGuildRaidDungeonChart))
            return;

        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        SetPartyPlayerState(CharacterState.None);
        FadeScreen.FadeOut(() =>
        {
            Managers.UI.CloseAllPopupUI();
            Managers.Monster.ClearSpawnMonster();

            GainGold.Value = 0;
            GainGoldBar.Value = 0;
            GainGuildPoint.Value = 0;

            LimitTime = _allGuildRaidDungeonChart.LimitTime;
            RemainTime.Value = LimitTime;

            PlayTime.Value = 0;

            IsPartyPlayerReady = false;

            Managers.Game.MainPlayer.IsDie = false;
            Managers.Game.MainPlayer.InitPassiveCount();

            GameCamera.Follow = Managers.Game.MainPlayer.TargetGroupFollow.transform;

            SortStrongGuildMember();
            MakePartyPlayer();

            Managers.Stage.State.Value = StageState.GuildAllRaid;
            Managers.Game.MainPlayer.transform.position = Vector3.zero;

            _isEnd = false;


            if (ChartManager.DungeonCharts.TryGetValue((int)DungeonType.Raid, out var dungeonChart))
            {
                if (ChartManager.WorldCharts.TryGetValue(dungeonChart.WorldId, out var worldChart))
                    Managers.Stage.SetBg("BG_World_999");

                Managers.Stage.SetBGScale(new Vector3(8, 8, 1));
            }

            FadeScreen.FadeIn(() =>
            {
                Managers.Sound.PlayBgm(BgmType.Title);
                IsPartyPlayerReady = true;
                IsAllRaidSkillLock = true;
                Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                SetPartyPlayerState(CharacterState.Idle);

                MainThreadDispatcher.StartCoroutine(Cor_CameraZoomIn());

            });
        });
    }

    public string GetGuildAllRaidBossName()
    {
        if (_allGuildRaidDungeonChart == null)
            return string.Empty;

        return !ChartManager.MonsterCharts.TryGetValue(_allGuildRaidDungeonChart.MonsterId, out var monsterChart)
            ? string.Empty
            : ChartManager.GetString(monsterChart.Name);
    }
    #endregion

    #region Coutines
    private IEnumerator Cor_CameraZoomIn()
    {
        yield return new WaitForSeconds(1f);

        if (_isEnd)
            yield break;

        MoveToAllPlayer();

        if (_isEnd)
            yield break;

        while (!_isEnd)
        {
            if (Managers.Game.MainPlayer.transform.position.x > 20f)
            {
                RunToCircle();
                break;
            }
            yield return null;
        }

        if (_isEnd)
            yield break;

        yield return new WaitForSeconds(4f);

        if (_isEnd)
            yield break;

        Wave.Value = 1;

        if (_isEnd)
            yield break;

        Managers.Monster.StartSpawn();

        if (_isEnd)
            yield break;

        MainThreadDispatcher.StartCoroutine(Cor_PlayTimer());
        IsAllRaidSkillLock = false;
    }

    private IEnumerator Cor_PlayTimer()
    {
        while (!_isEnd)
        {
            yield return null;
            RemainTime.Value -= Time.deltaTime;
            PlayTime.Value += Time.deltaTime;

            if (_isEnd)
                yield break;

            if (RemainTime.Value > 0)
                continue;

            Fail();
            yield break;
        }
    }
    #endregion
}
