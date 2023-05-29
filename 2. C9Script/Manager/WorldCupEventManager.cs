using System;
using System.Collections;
using BackEnd;
using Cinemachine;
using DG.Tweening;
using GameData;
using UniRx;
using UnityEngine;

public class WorldCupEventManager
{
    private bool _isStart;
    private bool _isEnd;

    private CinemachineVirtualCamera _gameCamera;

    public float LimitTime;
    public ReactiveProperty<float> RemainTime = new(0);

    public ReactiveProperty<double> TotalReward = new(0);

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

    public void StartDungeon()
    {
        if (_isStart)
            return;

        if (!ChartManager.WorldCupEventDungeonCharts.TryGetValue(1, out var worldCupEventDungeonChart))
            return;

        if (!ChartManager.DungeonCharts.TryGetValue((int)DungeonType.WorldCupEvent, out var dungeonChart))
            return;

        _isStart = true;

        Managers.Game.MainPlayer.State.Value = CharacterState.None;

        FadeScreen.FadeOut(() =>
        {
            _isStart = false;
            Managers.Stage.SetBg(dungeonChart.WorldId);

            Managers.UI.CloseAllPopupUI();

            Managers.Stage.State.Value = StageState.WorldCupEvent;
            Managers.Monster.StartSpawn();

            Managers.Game.MainPlayer.InitPassiveCount();

            TotalReward.Value = 0;
            LimitTime = worldCupEventDungeonChart.LimitTime;
            RemainTime.Value = LimitTime;

            GameCamera.Follow = null;

            Managers.Game.MainPlayer.transform.position = new Vector2(-5, 0);

            GameCamera.transform.position = new Vector3(0, 0, GameCamera.transform.position.z);
            GameCamera.m_Lens.OrthographicSize = 15;
            Managers.Sound.PlayBgm(BgmType.WorldCup);

            if (Managers.Game.UserData.IsAdSkip())
            {
                Managers.Game.WorldCupAdBuff.Value = true;
                Managers.Game.CalculateStat();
            }

            FadeScreen.FadeIn(() => { MainThreadDispatcher.StartCoroutine(CoCameraEffect()); });
        }, 0f);
    }

    private IEnumerator CoCameraEffect()
    {
        Vector3[] paths = {
            new(20, 0, 0),
            new(8, 6, 0),
            new(5, 0 ,0)
        };

        Managers.Monster.WorldCupMonster.transform.DOPath(paths, 1f, PathType.CatmullRom);
        
        yield return new WaitForSeconds(1f);

        while (GameCamera.m_Lens.OrthographicSize < 15)
        {
            GameCamera.m_Lens.OrthographicSize += 1.5f * Time.deltaTime;
            yield return null;
        }

        GameCamera.transform.DOMove(Managers.Game.MainPlayer.transform.position, 0.5f).onComplete = () =>
        {
            Managers.Sound.PlaySfxSound(SfxType.WorldCupWhistle);

            GameCamera.Follow = Managers.Game.MainPlayer.transform;

            Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
            Managers.Monster.WorldCupMonster.State.Value = CharacterState.Idle;

            MainThreadDispatcher.StartCoroutine(CoLimitTimer());
        };
    }

    private void EndDungeon()
    {
        if (!ChartManager.WorldCupEventDungeonCharts.TryGetValue(1, out var worldCupEventDungeonChart))
            return;

        if (!ChartManager.DungeonCharts.TryGetValue((int)DungeonType.WorldCupEvent, out var dungeonChart))
            return;

        if (_isEnd)
            return;

        _isEnd = true;

        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        Managers.Monster.WorldCupMonster.State.Value = CharacterState.None;

        TotalReward.Value += worldCupEventDungeonChart.ClearRewardValue;

        var clearDungeonPopup = Managers.UI.ShowPopupUI<UI_ClearDungeonPopup>();
        clearDungeonPopup.SetWorldCup(dungeonChart.RewardItemId, TotalReward.Value);

        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
        {
            FadeScreen.FadeOut(() =>
            {
                if (TotalReward.Value > Managers.Game.ProgramerBeatData.ProgramerBeatHighestScore)
                    Managers.Game.ProgramerBeatData.ProgramerBeatHighestScore = TotalReward.Value;

                Managers.Game.ProgramerBeatData.ProgramerBeatEntryCount -= 1;
                Managers.Game.IncreaseItem(ItemType.Goods, dungeonChart.RewardItemId, TotalReward.Value);

                GameDataManager.GoodsGameData.SaveGameData();
                GameDataManager.ProgramerBeatGameData.SaveGameData();
                
                var param = new Param();
        
                param.Add("EventType", "ProgramerBeat");
                param.Add("GainGaebalToken", TotalReward.Value);

                Backend.GameLog.InsertLog("Event", param);

                Managers.UI.CloseAllPopupUI();
                Managers.Stage.State.Value = StageState.Normal;
                Managers.Monster.StartSpawn();
                
                Managers.Game.WorldCupAdBuff.Value = false;
                Managers.Game.CalculateStat();

                _isEnd = false;

                Managers.Game.MainPlayer.SetCostume(Managers.Game.EquipDatas[(EquipType.ShowCostume)]);
                Managers.Game.MainPlayer.transform.position = Vector3.zero;
                
                FadeScreen.FadeIn(() => { Managers.Game.MainPlayer.State.Value = CharacterState.Idle; });
            }, 0.5f);
        });
    }

    private IEnumerator CoLimitTimer()
    {
        while (true)
        {
            yield return null;

            RemainTime.Value -= Time.deltaTime;

            if (RemainTime.Value <= 0)
            {
                EndDungeon();
                yield break;
            }
        }
    }
}