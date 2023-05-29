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

public enum eTEAM_TYPE
{
    MyTeam,
    EnemyTeam,
}

public class GuildSportsManager
{
    #region Fields
    private List<(string, double)> _sortGuildMembers = new();
    private List<(string, double)> _sortEnemyGuildMembers = new();

    private List<Player> _teamPlayers = new();
    private List<Player> _enemyPlayers = new();

    public ReactiveProperty<double> MyGuildDps = new(0);
    public ReactiveProperty<double> EnemyGuildDps = new(0);
    public ReactiveProperty<float> PlayTime = new();

    private GuildSportsDungeonChart _gSportsChart => ChartManager.GuildSportsDungeonCharts[1];

    private Vector2 _mainPos = new Vector3(-10, -15.5f, 0);

    private Vector2[] _teamPos = new[]
    {
        new Vector2(10, -15.5f), new Vector2(-10,-20), new Vector2(10, -20),
        new Vector2(-10, -11), new Vector2(10, -11)
    };

    private Vector2[] _enemyPos = new[]
    {
        new Vector2(-10, 6), new Vector2(10, 6), new Vector2(-10, 1.5f), new Vector2(10, 1.5f),
         new Vector2(-10, 10.5f), new Vector2(10, 10.5f)
    };

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

    public bool IsEndFlag { get; private set; }
    #endregion

    #region Unity Methods
    #endregion

    #region Public Methods
    public void GuildSportsStart()
    {
        Managers.Game.MainPlayer.State.Value = CharacterState.None;

        FadeScreen.FadeOut(() =>
        {
            Managers.UI.CloseAllPopupUI();
            Managers.Monster.ClearSpawnMonster();

            Managers.Game.MainPlayer.IsDie = false;
            Managers.Game.MainPlayer.InitPassiveCount();

            SortRandomGuildMember();
            MakePartyPlayer();

            MyGuildDps.Value = 0;
            EnemyGuildDps.Value = 0;

            Managers.Stage.State.Value = StageState.GuildSports;
            GameCamera.m_Follow = Managers.Game.MainPlayer.GuildSportsTargetGroup;
            Managers.Game.MainPlayer.transform.position = _mainPos;
            Managers.Game.MainPlayer.transform.localScale = new Vector2(1.5f, 1.5f);
            GameCamera.m_Lens.OrthographicSize = 25f;

            PlayTime.Value = _gSportsChart.LimitTime;

            Managers.Stage.SetBg("Contents_BG_009");
            Managers.Stage.SetBGScale(new Vector3(5, 5, 1));

            IsEndFlag = false;

            FadeScreen.FadeIn(() =>
            {
                Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.GuildSportsTicket, 1);
                GameDataManager.GoodsGameData.SaveGameData();
                //IsPartyPlayerReady = true;
                //IsAllRaidSkillLock = true;
                Managers.Game.MainPlayer.State.Value = CharacterState.None;
                SetPlayerState(CharacterState.None);

                Managers.Monster.StartSpawn();

                Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
                {
                    Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                    SetPlayerState(CharacterState.Idle);
                    MainThreadDispatcher.StartCoroutine(CoLimitTimer());
                });

                
                //MainThreadDispatcher.StartCoroutine(Cor_CameraZoomIn());

            });
        });
    }

    public void EndGame(bool isWin, int score, double itemValue)
    {
        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(__ =>
        {
            FadeScreen.Instance.OnLoadingScreen();

            Backend.URank.Guild.ContributeGuildGoods(
                Managers.Rank.RankUUIDDictionary[(RankType.GuildSports, Managers.Server.CurrentServer)],
                goodsType.goods4, score,
                bro =>
                {
                    FadeScreen.Instance.OffLoadingScreen();

                    if (bro.IsSuccess())
                    {
                        // 레이드 경험치 기부 성공 후 길드내 굿즈 데이터 1회 갱신 
                        Managers.Guild.GetMyGuildGoodsData(() =>
                            {

                                UI_GuildSportsEndPopup popup = Managers.UI.ShowPopupUI<UI_GuildSportsEndPopup>();
                                if (popup != null)
                                    popup.Init(isWin ? ChartManager.GetString("Result_Win") : ChartManager.GetString("Result_Lose"),
                                        itemValue, score);

                                Managers.Guild.GuildMemberDatas[Backend.UserInDate].GoodsDic[(int)GuildGoodsType.GuildSports] +=
                                    score;

                                Param param = new Param()
                                {
                                {
                                    "GuildName",
                                    Managers.Guild.GuildData != null ? Managers.Guild.GuildData.GuildName : string.Empty
                                },
                                { "Type", "GuildSports" },
                                { "Result", isWin ? "Win" : "Lose" },
                                { "EnemyGuildName", Managers.Guild.EnemyGuild.GuildName },
                                { "Score", score }
                                };

                                Backend.GameLog.InsertLog("GuildSports", param);

                                Managers.Game.IncreaseItem(_gSportsChart.ItemType, _gSportsChart.ItemId, isWin ? _gSportsChart.WinItemValue : _gSportsChart.LoseItemValue);

                                GameDataManager.GoodsGameData.SaveGameData();

                                Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
                                    {
                                        FadeScreen.FadeOut(() =>
                                        {
                                            Managers.UI.CloseAllPopupUI();
                                            Managers.Monster.ClearSpawnMonster();
                                            Managers.Stage.State.Value = StageState.Normal;
                                            Managers.Monster.StartSpawn();
                                            Managers.Game.MainPlayer.transform.position = Vector3.zero;
                                            Managers.Game.MainPlayer.transform.localScale = Vector3.one;
                                            _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.BaseCameraSize;
                                            _gameCamera.m_Follow = Managers.Game.MainPlayer.transform;
                                            SetPlayerState(CharacterState.None);
                                            PoolManager.Instance.PlayerAllOffObj();
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
                                Managers.Game.MainPlayer.transform.localScale = Vector3.one;
                                _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.BaseCameraSize;
                                _gameCamera.m_Follow = Managers.Game.MainPlayer.transform;
                                SetPlayerState(CharacterState.None);
                                PoolManager.Instance.PlayerAllOffObj();
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
        });
    }
    
    #endregion

    #region Private Methods
    private void SortRandomGuildMember()
    {
        _sortGuildMembers.Clear();
        _sortEnemyGuildMembers.Clear();

        foreach (var guildMemberData in Managers.Guild.GuildMemberDatas.Values)
        {
            if (guildMemberData.InDate == BackEnd.Backend.UserInDate)
                continue;

            if (guildMemberData.InDate == null)
                continue;

            var power = Utils.GetPower(guildMemberData.StatDatas, guildMemberData.PromoGrade);
            _sortGuildMembers.Add((guildMemberData.InDate, power));
        }

        foreach (var enemyGuildMemberData in Managers.Guild.EnemyGuildMemberDatas.Values)
        {
            if (enemyGuildMemberData.InDate == null)
                continue;

            var enemyPower = Utils.GetPower(enemyGuildMemberData.EnemyStatDatas, enemyGuildMemberData.PromoGrade);
            _sortEnemyGuildMembers.Add((enemyGuildMemberData.InDate, enemyPower));
        }
        System.Random ran = new System.Random();
        _sortGuildMembers = _sortGuildMembers.OrderBy(_ => ran.Next()).ToList();
        _sortEnemyGuildMembers = _sortEnemyGuildMembers.OrderBy(_ => ran.Next()).ToList();

    }

    private void MakePartyPlayer()
    {
        _teamPlayers.Clear();
        _enemyPlayers.Clear();


        for (int i = 0; i < 5; i++)
        {
            if (_sortGuildMembers.Count <= i)
                continue;

            Player partyPlayer = PoolManager.Instance.GetPlayer(0);
            if (partyPlayer == null)
                continue;

            partyPlayer.IsPartyPlayer = true;
            partyPlayer.SetTeamIndex(0);
            partyPlayer.StatDatas = Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1].StatDatas;
            partyPlayer.PartyPlayer_Passive_Level = Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1].Passive_Godgod_Level;
            partyPlayer.InitializeAllRaid();
            partyPlayer.transform.position = Vector3.zero;
            partyPlayer.transform.localScale = new Vector2(1.5f, 1.5f);
            partyPlayer.State.Value = CharacterState.None;

            partyPlayer.SetWeapon(Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1]
                .EquipData[EquipType.Weapon]);
            partyPlayer.SetCostume(Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1]
                .EquipData[EquipType.ShowCostume]);
            partyPlayer.SetPet(Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1].EquipData[EquipType.Pet]);
            partyPlayer.SetNickname(Managers.Guild.GuildMemberDatas[_sortGuildMembers[i].Item1].Nickname);

            _teamPlayers.Add(partyPlayer);
        }

        Managers.Game.MainPlayer.SetTeamIndex(0);

        for (int i = 0; i < 6; i++)
        {
            if (_sortEnemyGuildMembers.Count <= i)
                continue;

            Player enemyPlayer = PoolManager.Instance.GetPlayer(0);
            if (enemyPlayer == null)
                continue;

            enemyPlayer.IsPartyPlayer = true;
            enemyPlayer.SetTeamIndex(1);
            enemyPlayer.StatDatas = Managers.Guild.EnemyGuildMemberDatas[_sortEnemyGuildMembers[i].Item1].EnemyStatDatas;
            enemyPlayer.PartyPlayer_Passive_Level = Managers.Guild.EnemyGuildMemberDatas[_sortEnemyGuildMembers[i].Item1].Passive_Godgod_Level;
            enemyPlayer.InitializeAllRaid();
            enemyPlayer.transform.position = Vector3.zero;
            enemyPlayer.transform.localScale = new Vector2(1.5f, 1.5f);
            enemyPlayer.State.Value = CharacterState.None;

            enemyPlayer.SetWeapon(Managers.Guild.EnemyGuildMemberDatas[_sortEnemyGuildMembers[i].Item1]
                .EquipData[EquipType.Weapon]);
            enemyPlayer.SetCostume(Managers.Guild.EnemyGuildMemberDatas[_sortEnemyGuildMembers[i].Item1]
                .EquipData[EquipType.ShowCostume]);
            enemyPlayer.SetPet(Managers.Guild.EnemyGuildMemberDatas[_sortEnemyGuildMembers[i].Item1].EquipData[EquipType.Pet]);
            enemyPlayer.SetNickname(Managers.Guild.EnemyGuildMemberDatas[_sortEnemyGuildMembers[i].Item1].Nickname);

            _enemyPlayers.Add(enemyPlayer);
        }

        ResetPartyPlayerPosition();
    }

    private void ResetPartyPlayerPosition()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i >= _teamPlayers.Count)
                continue;

            if (i >= _teamPos.Length)
                continue;

            _teamPlayers[i].transform.position = _teamPos[i];
            _teamPlayers[i].Direction.Value = _teamPlayers[i].transform.position.x > 0 ? CharacterDirection.Left : CharacterDirection.Right;
        }

        for (int i = 0; i < 6; i++)
        {
            if (i >= _enemyPlayers.Count)
                continue;

            if (i >= _enemyPos.Length)
                continue;

            _enemyPlayers[i].transform.position = _enemyPos[i];
            _enemyPlayers[i].Direction.Value = _enemyPlayers[i].transform.position.x > 0 ? CharacterDirection.Left : CharacterDirection.Right;
        }
    }

    private void SetPlayerState(CharacterState state)
    {
        _teamPlayers.ForEach(player =>
        {
            if (player == null)
                return;

            player.State.Value = state;
        });

        _enemyPlayers.ForEach(player =>
        {
            if (player == null)
                return;

            player.State.Value = state;
        });
    }

    private void EndTeamMove(eTEAM_TYPE type)
    {
        switch (type)
        {
            case eTEAM_TYPE.MyTeam:
                for (int i = 0; i < _teamPlayers.Count; i++)
                {
                    _teamPlayers[i].SetGuildSportsEndDirection(_teamPos[i]);
                }
                Managers.Game.MainPlayer.SetGuildSportsEndDirection(_mainPos);
                break;
            case eTEAM_TYPE.EnemyTeam:
                for (int i = 0; i < _enemyPlayers.Count; i++)
                {
                    _enemyPlayers[i].SetGuildSportsEndDirection(_enemyPos[i]);
                }
                break;
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator CoLimitTimer()
    {
        while (true)
        {
            yield return null;
            PlayTime.Value -= Time.deltaTime;

            if (PlayTime.Value <= 0)
                break;
        }

        IsEndFlag = true;
        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        SetPlayerState(CharacterState.None);

        if (MyGuildDps.Value >= EnemyGuildDps.Value)
        {
            Managers.Monster.GuildSportMonster[(int)eTEAM_TYPE.MyTeam].GetComponentInChildren<GuildSportsMonster>().GuildSportsSetAnim(eANIM_GUILDSPORTS_PARK.Idle_3);
            EndTeamMove(eTEAM_TYPE.MyTeam);
            yield return new WaitForSeconds(2f);
            Managers.Monster.GuildSportMonster[(int)eTEAM_TYPE.MyTeam].GetComponentInChildren<GuildSportsMonster>().GuildSportsSetAnim(eANIM_GUILDSPORTS_PARK.Victory);
            EndGame(true, _gSportsChart.WinPoint, _gSportsChart.WinItemValue);
        }
        else
        {
            Managers.Monster.GuildSportMonster[(int)eTEAM_TYPE.EnemyTeam].GetComponentInChildren<GuildSportsMonster>().GuildSportsSetAnim(eANIM_GUILDSPORTS_PARK.Idle_3);
            EndTeamMove(eTEAM_TYPE.EnemyTeam);
            yield return new WaitForSeconds(2f);
            Managers.Monster.GuildSportMonster[(int)eTEAM_TYPE.EnemyTeam].GetComponentInChildren<GuildSportsMonster>().GuildSportsSetAnim(eANIM_GUILDSPORTS_PARK.Victory);
            EndGame(false, _gSportsChart.LosePoint, _gSportsChart.LoseItemValue);
        }
    }
    #endregion
}
