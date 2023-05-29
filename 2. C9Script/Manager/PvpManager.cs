using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using GameData;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class PvpManager
{
    public Dictionary<int, double> EnemyBaseStatDatas = new();
    public Dictionary<EquipType, int> EnemyEquipDatas = new();
    public int EnemyPromoGrade;
    public int EnemyPassiveGodgodLv;

    public RankData EnemyRankData;
    public Player EnemyPlayer;

    public ReactiveProperty<double> TotalMyDamage = new(0);
    public ReactiveProperty<double> TotalEnemyDamage = new(0);

    public TimeSpan PvpLimitTime;
    public readonly ReactiveProperty<TimeSpan> PvpRemainTime = new();

    public ObscuredInt PvpScore = 0;
    public ObscuredInt MatchCount;

    public readonly ReactiveProperty<bool> IsAutoMatch = new(false);

    public bool IsPvp;

    public Action OnChangePvpEnemy;

    public void SetEnemyData(RankData enemyRankData, Action callback)
    {
        EnemyRankData = enemyRankData;
        GameDataManager.PvpGameData.GetGameData(EnemyRankData.GamerInDate, jsonData =>
        {
            if (jsonData == null)
            {
                Debug.LogError("Fail SetEnemyData Null");
                return;
            }

            jsonData = jsonData["rows"][0];

            if (jsonData.ContainsKey(PvpDataType.BaseStat.ToString()))
                EnemyBaseStatDatas =
                    JsonConvert.DeserializeObject<Dictionary<int, double>>(jsonData[PvpDataType.BaseStat.ToString()]
                        .ToString());

            if (jsonData.ContainsKey(PvpDataType.EquipItems.ToString()))
                EnemyEquipDatas =
                    JsonConvert.DeserializeObject<Dictionary<EquipType, int>>(
                        jsonData[PvpDataType.EquipItems.ToString()].ToString());

            if (jsonData.ContainsKey(PvpDataType.PromoGrade.ToString()))
                EnemyPromoGrade = int.Parse(jsonData[PvpDataType.PromoGrade.ToString()].ToString());

            if (jsonData.ContainsKey(PvpDataType.Passive_Godgod_Lv.ToString()))
                EnemyPassiveGodgodLv = int.Parse(jsonData[PvpDataType.Passive_Godgod_Lv.ToString()].ToString());

            callback?.Invoke();
        });
    }

    public void StartMatch()
    {
        if (Managers.Pvp.IsPvp)
            return;

        Managers.Pvp.IsPvp = true;

        var chartData = ChartManager.DungeonCharts[(int)DungeonType.Pvp];

        if (Utils.IsWeeklyCalculateRankTime())
        {
            Managers.Message.ShowMessage("랭킹 정산중 입니다.");
            Managers.Pvp.IsPvp = false;
            return;
        }

        if (!Utils.IsEnoughItem(ItemType.Goods, chartData.EntryItemId, 1))
        {
            Managers.Message.ShowMessage("입장 재화가 부족합니다!!");
            Managers.Pvp.IsPvp = false;
            return;
        }

        FadeScreen.Instance.OnLoadingScreen();

        int startRank = Mathf.Max(Managers.Rank.MyRankDatas[RankType.Pvp].Rank - 5, 0);
        if (Managers.Rank.MyRankDatas[RankType.Pvp].Rank == 1)
            startRank = 2;

        // 내 위 5명 데이터 읽기
        Managers.Rank.GetRankList(RankType.Pvp, rankDatas =>
        {
            RankData enemyRankData;

            // 매치 상대를 못 찾음
            if (rankDatas.Count <= 0)
            {
                if (Managers.Manager.ProjectType == ProjectType.Dev)
                {
                    enemyRankData = Managers.Rank.MyRankDatas[RankType.Pvp];
                }
                else
                {
                    Managers.Message.ShowMessage("매칭 상대를 찾을 수 없습니다!!");
                    Managers.Pvp.IsPvp = false;
                    FadeScreen.Instance.OffLoadingScreen();
                    return;
                }
            }

            List<RankData> enemyRankDatas = new List<RankData>();

            // 1등이 나일때 2등과 매치
            if (rankDatas.Count > 0 && rankDatas[0].GamerInDate == Backend.UserInDate)
            {
                // 2등 상대가 없음 (1위, 1명만 존재)
                if (rankDatas.Count <= 1)
                {
                    if (Managers.Manager.ProjectType == ProjectType.Dev)
                    {
                        enemyRankData = Managers.Rank.MyRankDatas[RankType.Pvp];
                    }
                    else
                    {
                        Managers.Message.ShowMessage("매칭 상대를 찾을 수 없습니다!!");
                        Managers.Pvp.IsPvp = false;
                        FadeScreen.Instance.OffLoadingScreen();
                        return;
                    }
                }else
                    enemyRankData = rankDatas[Random.Range(1, rankDatas.Count)];
            }
            else
            {
                foreach (var rankData in rankDatas)
                {
                    if (rankData.GamerInDate == Backend.UserInDate)
                        break;

                    enemyRankDatas.Add(rankData);
                }

                if (enemyRankDatas.Count <= 0)
                {
                    if (Managers.Manager.ProjectType == ProjectType.Dev)
                    {
                        enemyRankData = Managers.Rank.MyRankDatas[RankType.Pvp];
                    }
                    else
                    {
                        Managers.Message.ShowMessage("매칭 상대를 찾을 수 없습니다!!");
                        Managers.Pvp.IsPvp = false;
                        FadeScreen.Instance.OffLoadingScreen();
                        return;
                    }
                }
                else
                    enemyRankData = enemyRankDatas[Random.Range(0, enemyRankDatas.Count)];
            }

            // 데이터가 없음 - 일어나면 안되는 상황
            if (enemyRankData == null)
            {
                if (Managers.Manager.ProjectType == ProjectType.Dev)
                {
                    enemyRankData = Managers.Rank.MyRankDatas[RankType.Pvp];
                }
                else
                {
                    Debug.LogError("매칭 상대 없음!!");
                    Managers.Pvp.IsPvp = false;
                    FadeScreen.Instance.OffLoadingScreen();
                    return;
                }
            }

            Managers.Pvp.SetEnemyData(enemyRankData, () =>
            {
                FadeScreen.Instance.OffLoadingScreen();
                
                Managers.Pvp.StartPvp();
            });
        }, 5, startRank, false);
    }

    private void StartPvp()
    {
        FadeScreen.FadeOut(() =>
        {
            Managers.UI.CloseAllPopupUI();
            Managers.Monster.ClearSpawnMonster();
            
            if (EnemyPlayer != null)
                EnemyPlayer.DestroyPlayer();

            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EnterDungeon, (int)DungeonType.Pvp, 1));

            Managers.Stage.State.Value = StageState.Pvp;
            Managers.Stage.SetBg(ChartManager.DungeonCharts[(int)DungeonType.Pvp].WorldId);

            TotalMyDamage.Value = 0;
            TotalEnemyDamage.Value = 0;

            Managers.Game.MainPlayer.InitPassiveCount();

            Managers.Game.MainPlayer.State.Value = CharacterState.None;
            Managers.Game.MainPlayer.IsPvpEnemyPlayer = false;
            Managers.Game.MainPlayer.Direction.Value = CharacterDirection.Right;
            Managers.Game.MainPlayer.transform.localPosition = new Vector2(-3.5f, -3f);
            Managers.Game.MainPlayer.SetScale( new Vector2(2.5f, 2.5f));

            if (MatchCount <= 0)
            {
                foreach (var statChart in ChartManager.StatCharts)
                {
                    if (EnemyBaseStatDatas.ContainsKey(statChart.Key))
                        EnemyBaseStatDatas[statChart.Key] = statChart.Value.DefaultValue;
                }

                EnemyEquipDatas[EquipType.Costume] = (int)ChartManager.SystemCharts[(SystemData.Default_Costume)].Value;
                EnemyEquipDatas[EquipType.Pet] = (int)ChartManager.SystemCharts[(SystemData.Default_Pet)].Value;
                EnemyEquipDatas[EquipType.Weapon] = (int)ChartManager.SystemCharts[(SystemData.Default_Weapon)].Value;
            }

            EnemyPlayer = Managers.Resource.Instantiate("Player/Chulgu").GetComponent<Player>();
            EnemyPlayer.IsPvpEnemyPlayer = true;
            EnemyPlayer.Initialize();
            EnemyPlayer.State.Value = CharacterState.None;
            EnemyPlayer.Direction.Value = CharacterDirection.Left;
            EnemyPlayer.transform.localPosition = new Vector2(3.5f, -3);
            EnemyPlayer.SetWeapon(EnemyEquipDatas[EquipType.Weapon]);
            EnemyPlayer.SetCostume(EnemyEquipDatas[EquipType.Costume]);
            EnemyPlayer.SetPet(EnemyEquipDatas[EquipType.Pet]);
            EnemyPlayer.SetScale( new Vector2(2.5f, 2.5f));

            var chartData = ChartManager.PvpCharts[2];

            PvpLimitTime = new TimeSpan(0, 0, 0, chartData.LimitTime);
            PvpRemainTime.Value = PvpLimitTime;

            OnChangePvpEnemy?.Invoke();

            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
            {
                FadeScreen.FadeIn(() =>
                {
                    Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                    EnemyPlayer.State.Value = CharacterState.Idle;
                    Managers.Game.MainPlayer.SetScale( new Vector2(2.5f, 2.5f));

                    MainThreadDispatcher.StartCoroutine(CoLimitTimer());
                }, 0.5f);
            });
        }, 0.5f);
    }

    private IEnumerator CoLimitTimer()
    {
        while (PvpRemainTime.Value.TotalSeconds > 0)
        {
            yield return null;
            PvpRemainTime.Value = PvpRemainTime.Value.Add(TimeSpan.FromSeconds(-Time.deltaTime));
        }

        EndPvp();
    }

    private void EndPvp()
    {
        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        EnemyPlayer.State.Value = CharacterState.None;
        
        if (Utils.IsWeeklyCalculateRankTime())
        {
            Managers.Message.ShowMessage("랭킹이 정산중이라 반영되지 않습니다.");
            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
            {
                FadeScreen.FadeOut(() =>
                {
                    Managers.Pvp.IsPvp = false;
                    Managers.UI.CloseAllPopupUI();
                    EnemyPlayer.DestroyPlayer();
                    Managers.Game.MainPlayer.transform.localPosition = Vector3.zero;
                    Managers.Game.MainPlayer.transform.localScale = Vector3.one;

                    Managers.Stage.State.Value = StageState.Normal;
                    Managers.Monster.StartSpawn();

                    Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
                    {
                        FadeScreen.FadeIn(() => { Managers.Game.MainPlayer.State.Value = CharacterState.Idle; });
                    });
                });
            });
            return;
        }

        var pvpChart = ChartManager.PvpCharts[1];
        List<(int, double)> acquireItems = new List<(int, double)>();

        Param param = new Param();

        double itemValue = 0;

        // 승리
        if (TotalMyDamage.Value >= TotalEnemyDamage.Value)
        {
            Managers.Game.IncreaseItem(pvpChart.RewardItemType, pvpChart.RewardItemId, pvpChart.RewardItemWinValue);

            if (pvpChart.RewardItemType == ItemType.Goods)
            {
                itemValue = pvpChart.RewardItemWinValue;
            }

            param.Add("Result", "Win");
            param.Add("EnemyNickname", EnemyRankData.Nickname);
            param.Add("Score", PvpScore.GetDecrypted() + pvpChart.WinPoint);
            param.Add("GainToken", pvpChart.RewardItemWinValue.GetDecrypted());

            GameDataManager.RankPvpGameData.AddScore(pvpChart.WinPoint);
        }
        // 패배
        else
        {
            Managers.Game.IncreaseItem(pvpChart.RewardItemType, pvpChart.RewardItemId, pvpChart.RewardItemLoseValue);

            if (pvpChart.RewardItemType == ItemType.Goods)
                itemValue = pvpChart.RewardItemLoseValue;

            param.Add("Result", "Lose");
            param.Add("EnemyNickname", EnemyRankData.Nickname);
            param.Add("Score", PvpScore.GetDecrypted() + pvpChart.LosePoint);
            param.Add("GainToken", pvpChart.RewardItemLoseValue.GetDecrypted());

            GameDataManager.RankPvpGameData.AddScore(pvpChart.LosePoint);
        }

        Managers.Game.DecreaseItem(ItemType.Goods, ChartManager.DungeonCharts[(int)DungeonType.Pvp].EntryItemId, 1);

        GameDataManager.PvpGameData.SaveGameData();
        GameDataManager.GoodsGameData.SaveGameData();

        Utils.GetGoodsLog(ref param);
        
        Backend.GameLog.InsertLog("Pvp", param);

        var uiPvpResultPopup = Managers.UI.ShowPopupUI<UI_PvpResultPopup>();
        uiPvpResultPopup.Init(itemValue);

        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
        {
            if (IsAutoMatch.Value && Managers.Game.GoodsDatas[(int)Goods.PvpTicket].Value > 0)
            {
                Managers.Pvp.IsPvp = false;
                StartMatch();
            }
            else
            {
                FadeScreen.FadeOut(() =>
                {
                    Managers.Pvp.IsPvp = false;
                    Managers.UI.CloseAllPopupUI();
                    EnemyPlayer.DestroyPlayer();
                    Managers.Game.MainPlayer.transform.localPosition = Vector3.zero;
                    Managers.Game.MainPlayer.transform.localScale = Vector3.one;

                    Managers.Stage.State.Value = StageState.Normal;
                    Managers.Monster.StartSpawn();

                    Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
                    {
                        FadeScreen.FadeIn(() => { Managers.Game.MainPlayer.State.Value = CharacterState.Idle; });
                    });
                });
            }
        });
    }
}