using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class XMasEventManager
{
    private SpriteRenderer _bgPlatformSprite;

    private SpriteRenderer BgPlatformSprite
    {
        get
        {
            if (_bgPlatformSprite != null)
                return _bgPlatformSprite;

            _bgPlatformSprite = GameObject.FindWithTag("XMasPlatform").GetComponent<SpriteRenderer>();
            return _bgPlatformSprite;
        }
    }

    private XMasPlayer _xMasPlayer;
    private readonly Queue<XMas_Poop> _xMasPoopPool = new();
    private readonly List<XMas_Poop> _xMasPoops = new();

    public readonly ReactiveProperty<int> Score = new(0);
    public readonly ReactiveProperty<bool> IsPlaying = new(false);

    private bool _isClose;

    public void Init()
    {
        Managers.Stage.State.Subscribe(state =>
        {
            BgPlatformSprite.enabled = state == StageState.XMasEvent;
        });
    }

    public void Start()
    {
        Managers.Game.MainPlayer.State.Value = CharacterState.None;

        FadeScreen.FadeOut(() =>
        {
            Managers.UI.CloseAllPopupUI();
            Managers.Monster.ClearSpawnMonster();

            Managers.Game.MainPlayer.transform.position = new Vector2(0, -3);
            Managers.Game.MainPlayer.gameObject.SetActive(false);
            
            if (_xMasPlayer == null)
                _xMasPlayer = Managers.Resource.Instantiate("Player/XMasChulgu").GetComponent<XMasPlayer>();
            _xMasPlayer.transform.position = new Vector2(0, -8);
            
            BgPlatformSprite.enabled = true;

            Score.Value = 0;
            Managers.Stage.State.Value = StageState.XMasEvent;
            IsPlaying.Value = true;
            _isClose = false;

            if (ChartManager.DungeonCharts.TryGetValue((int)DungeonType.XMasEvent, out var dungeonChart))
            {
                Managers.Stage.SetBg(dungeonChart.WorldId);
            }
            else
            {
                Debug.LogError("Fail Load DungeonChart : XMaxEvent(102)");
            }

            FadeScreen.FadeIn(() =>
            {
                _xMasPlayer.SetCameraViewPos();
                MainThreadDispatcher.StartCoroutine(CoPoopSpawner());
            }, 0.5f);

        }, 0.5f);
    }

    private IEnumerator CoPoopSpawner()
    {
        var gameCamera = Camera.main;

        if (gameCamera == null)
        {
            Debug.LogError("CoPoopSpawner - GameCamera is null");
            yield break;
        }

        var minPos = gameCamera.ViewportToWorldPoint(gameCamera.rect.min);
        var maxPos = gameCamera.ViewportToWorldPoint(gameCamera.rect.max);

        var delay = new WaitForSeconds(0.2f);
        
        while (true)
        {
            if (!IsPlaying.Value)
                yield break;
            
            if (Managers.Stage.State.Value != StageState.XMasEvent)
                yield break;
            
            var xMasPoop = _xMasPoopPool.Count > 0 ? _xMasPoopPool.Dequeue() : CreateXMasPoop();

            if (xMasPoop == null)
            {
                Debug.LogError("XMasPoop is null");
                yield return null;
                continue;
            }
            
            xMasPoop.gameObject.SetActive(true);
            xMasPoop.transform.position = new Vector2(Random.Range(minPos.x, maxPos.x), maxPos.y + 2);
            
            _xMasPoops.Add(xMasPoop);

            yield return delay;
        }
    }

    private static XMas_Poop CreateXMasPoop()
    {
        return Managers.Resource.Instantiate("ETC/XMas_Poop").GetComponent<XMas_Poop>();
    }

    public void ReturnXMasPoop(XMas_Poop xMasPoop)
    {
        xMasPoop.gameObject.SetActive(false);
        _xMasPoopPool.Enqueue(xMasPoop);
        _xMasPoops.Remove(xMasPoop);
    }

    public void ClearPoop()
    {
        _xMasPoops.ForEach(xMasPoop =>
        {
            xMasPoop.gameObject.SetActive(false);
            _xMasPoopPool.Enqueue(xMasPoop);
        });
        
        _xMasPoops.Clear();
    }

    public void End()
    {
        if (!IsPlaying.Value)
            return;

        IsPlaying.Value = false;

        CalculationScore();
        RewardPayment(out int defaultItemId, out double defaultItemValue, out int scoreItemId, out double scoreItemValue);
        var popup = Managers.UI.ShowPopupUI<UI_ClearXMasDungeonPopup>();
        if (popup != null)
            popup.Init(Score.Value, defaultItemId, defaultItemValue, scoreItemId, scoreItemValue);

        var param = new Param
        {
            { "EventType", "XMas" },
            { "HighestScore", Score.Value },
            { "ScoreItemValue", scoreItemValue }
        };

        Backend.GameLog.InsertLog("Event", param, bro =>
        {

        });
    }

    private void ResetPlayer()
    {
        Managers.Game.MainPlayer.transform.position = Vector3.zero;
        Managers.Game.MainPlayer.gameObject.SetActive(true);
        Managers.Game.MainPlayer.Initialize();
        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        Managers.Resource.Destroy(_xMasPlayer.gameObject);
    }

    private void RewardPayment(out int defaultItemId, out double defaultItemValue, out int scoreItemId, out double scoreItemValue)
    {
        defaultItemId = 0;
        defaultItemValue = 0;
        scoreItemId = 0;
        scoreItemValue = 0;
        
        if (!ChartManager.XMasEventDungeonCharts.TryGetValue(1, out var xMasEventDungeonChart))
        {
            Debug.LogError("Fail Load XMasEventDungeonChart : 1");
            return;
        }

        // Default Reward
        Managers.Game.IncreaseItem(ItemType.Goods, xMasEventDungeonChart.DefaultRewardId, xMasEventDungeonChart.DefaultRewardValue);
        
        // ClearReward
        int scoreRewardValue = Math.Min(Score.Value * xMasEventDungeonChart.ScoreRewardValue,
            xMasEventDungeonChart.MaxScoreRewardValue);
        Managers.Game.IncreaseItem(ItemType.Goods, xMasEventDungeonChart.ScoreRewardId, scoreRewardValue);

        defaultItemId = xMasEventDungeonChart.DefaultRewardId;
        defaultItemValue = xMasEventDungeonChart.DefaultRewardValue;

        scoreItemId = xMasEventDungeonChart.ScoreRewardId;
        scoreItemValue = scoreRewardValue;
        
        GameDataManager.GoodsGameData.SaveGameData();
    }

    private void CalculationScore()
    {
        // 점수 갱신
        if (Score.Value > Managers.Game.XMasEventData.XMasDungeonHighestScore)
            Managers.Game.XMasEventData.XMasDungeonHighestScore = Score.Value;

        Managers.Game.XMasEventData.XMasDungeonEntryCount -= 1;
        GameDataManager.XMasEventGameData.SaveGameData();
    }

    private void SetNormalStage()
    {
        Managers.Stage.State.Value = StageState.Normal;
        Managers.Monster.StartSpawn();
    }

    public void CloseXMasEvent()
    {
        if (_isClose)
            return;

        _isClose = true;
        
        FadeScreen.FadeOut(() =>
        {
            Managers.UI.CloseAllPopupUI();
            
            ClearPoop();
            ResetPlayer();
            SetNormalStage();

            Managers.UI.ShowPopupUI<UI_DungeonPopup>();
                
            FadeScreen.FadeIn(() =>
            {
                _isClose = false;
                Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
            }, 0.5f);

        }, 0.5f);
    }
}