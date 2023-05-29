using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chart;
using GameData;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : BaseCharacter
{
    [SerializeField] private Collider2D MonsterCollider;
    [SerializeField] private Collider2D DetectionCollider;
    [SerializeField] private Collider2D TrackingCollider;

    [SerializeField] private Transform _attack_1_EffectPos;

    private Vector2[] _attack_2_EffectPos = { new Vector2(13, -1), new Vector2(10, 5.5f),
                                              new Vector2(50, 5.5f), new Vector2(43.5f, -2.5f),
                                              new Vector2(11, 2.5f), new Vector2(46.5f, 1.5f)};

    [SerializeField] private Transform CenterBoneTr;

    private AnimationEventReceiver _animationEventReceiver;

    private const float _minPatrolWaitingTime = 2f;
    private const float _maxPatrolWaitingTime = 4f;
    private const float _minPatrolDistance = 0.5f;
    private const float _maxPatrolDistance = 1.2f;

    private const float ArriveCheckDistance = 1.2f;

    private float _nextPatrolWaitingTime;

    [HideInInspector] public int Id;
    [HideInInspector] public MonsterType Type = MonsterType.Normal;

    private float _respawnTime;

    public ReactiveProperty<float> MoveSpeed { get; } = new(0);

    public override Vector3 CenterPos => transform.position + CenterOffset;

    private Vector3 CenterOffset;

    private float _patrolWaitingTime;
    private Vector3 _patrolDestination;

    protected MonsterChart _monsterChart;

    private bool _isTracking;
    private bool _isArriveTracking;

    private double _attack = 0;

    private double _goldValue;
    private double _expValue;

    private int _guildAllraidBossPattern = 1;

    public int DungeonRewardId;
    public double DungeonRewardValue;

    private List<Vector2> _runDestinations => Managers.GameSystemData.MonsterRunDestinations;
    // private readonly List<Vector2> _runDestinations = new()
    // {
    //     new(-12, 8),
    //     new(12, 8),
    //     new(-12, -12),
    //     new(12, -12)
    // };

    private int _runDestinationIndex = 0;

    private Vector2 _runRandomDestination;

    public override void Initialize()
    {
        base.Initialize();

        if (_animator != null)
        {
            _animationEventReceiver = _animator.gameObject.GetOrAddComponent<AnimationEventReceiver>();
            _animationEventReceiver.OnAnimationEventAttack = OnAnimationEvent_Attack;
            _animationEventReceiver.OnAnimationEventAttack_2 = OnAnimationEvent_Attack_2;
            _animationEventReceiver.OnAnimationEventAttackEffect_2 = AnimationEvent_Attack_2_Effect;

            _animationEventReceiver.OnAnimationEventEndAttack = OnAnimationEvent_EndAttack;
            _animationEventReceiver.OnAnimationEventAppear = OnAnimationEvent_Appear;
            _animationEventReceiver.OnAnimationEventEndAppear = OnAnimationEvent_EndAppear;
        }

        if (Type == MonsterType.StageSpecial)
        {
            gameObject.SetActive(false);
            Invoke(nameof(Respawn), _respawnTime);
        }
        else if (_monsterChart.DetectionType == (int)DetectionType.RunAwayToRandom)
        {
            _runRandomDestination = Managers.Monster.GetRandomSpawnPosition();
            State.Value = CharacterState.Move;
        }
        else
        {
            gameObject.SetActive(true);
            State.Value = CharacterState.Idle;
        }
    }

    protected override void SetData()
    {
        switch (Managers.Stage.State.Value)
        {
            case StageState.Normal:
                _nextPatrolWaitingTime = Random.Range(_minPatrolWaitingTime, _maxPatrolWaitingTime);
                break;
            case StageState.Dungeon:
            {
                if (Utils.IsHwasengbangDungeon())
                {
                    _respawnTime = ChartManager.HwasengbangDungeonCharts[Managers.Dungeon.DungeonStep.Value]
                        .RespawnTime;
                }
            }
                break;
        }

        _monsterChart = ChartManager.MonsterCharts[Id];
        Type = _monsterChart.Type;

        if (Managers.Stage.State.Value == StageState.GuildAllRaid)
            Type = MonsterType.AllRaidBoss;

        transform.localScale = new Vector3(_monsterChart.Scale, _monsterChart.Scale, _monsterChart.Scale);

        if (CenterBoneTr != null)
        {
            CenterOffset = Vector3.zero;
            CenterOffset.y = CenterBoneTr.position.y - transform.position.y;
        }

        MoveSpeed.Value = ChartManager.MonsterCharts[Id].MoveSpeed;
    }

    public void Init(int id, double hp, float respawnTime = -1, double attack = 0, double goldValue = 0,
        double expValue = 0)
    {
        Id = id;
        MaxHp = hp;
        Hp.Value = MaxHp;
        _respawnTime = respawnTime;
        _attack = attack;
        _goldValue = goldValue;
        _expValue = expValue;

        Initialize();
    }

    protected override void SetPropertyEvent()
    {
        base.SetPropertyEvent();

        Managers.Stage.State.Subscribe(state => { HpBar.gameObject.SetActive(state != StageState.Dungeon); })
            .AddTo(gameObject);

        this.UpdateAsObservable().Where(_ =>
                !IsDead && State.Value == CharacterState.Attack && Managers.Game.MainPlayer != null)
            .Subscribe(_ => SetDirection(Managers.Game.MainPlayer.transform.position));

        DetectionCollider.OnTriggerEnter2DAsObservable().Subscribe(colObject =>
        {
            if (!colObject.tag.Equals("Player"))
                return;

            _isTracking = true;
        });

        DetectionCollider.OnTriggerExit2DAsObservable().Subscribe(colObject =>
        {
            if (!colObject.tag.Equals("Player"))
                return;

            _isTracking = false;
        });

        TrackingCollider.OnTriggerEnter2DAsObservable().Subscribe(colObject =>
        {
            if (!colObject.tag.Equals("Player"))
                return;

            _isArriveTracking = true;
        });

        TrackingCollider.OnTriggerStay2DAsObservable().Subscribe(colObject =>
        {
            if (!colObject.tag.Equals("Player"))
                return;

            _isArriveTracking = true;
        });

        TrackingCollider.OnTriggerExit2DAsObservable().Subscribe(colObject =>
        {
            if (!colObject.tag.Equals("Player"))
                return;

            if (Managers.Stage.State.Value == StageState.GuildAllRaid)
                return;

            _isArriveTracking = false;
        });

        State.Where(_ => Id == 612).Subscribe(state => Debug.Log($"Boss State : {state.ToString()}"));
    }

    public override void Respawn()
    {
        base.Respawn();
        
        gameObject.SetActive(true);

        switch (Managers.Stage.State.Value)
        {
            case StageState.Dungeon:
            {
                if (Managers.Dungeon.DungeonId.Value == (int)DungeonType.Hwasengbang)
                    transform.position = Managers.Monster.GetRandomSpawnPositionInGameView();
            }
                break;
            default:
            {
                if (Type == MonsterType.StageSpecial)
                    transform.position = Vector3.zero;
                else
                    transform.position = Managers.Monster.GetRandomSpawnPosition();
            }
                break;
        }

        Hp.Value = MaxHp;
        State.Value = CharacterState.Idle;
    }

    protected override void UpdateIdle()
    {
        if (IsDead)
            return;

        if (Managers.Game.MainPlayer == null)
            return;

        switch (_monsterChart.DetectionType)
        {
            case (int)DetectionType.NonMove:
                return;
            case (int)DetectionType.NonDetection:
                {
                    if (IsPatrolTime())
                        StartPatrol();
                    else
                        _patrolWaitingTime += Time.deltaTime;
                }
                break;
            case (int)DetectionType.Detection:
                {
                    if (IsPlayerInTrackingRange())
                    {
                        if (Type == MonsterType.Boss)
                        {
                            if (Managers.Game.MainPlayer.IsDead)
                                return;

                            State.Value = CharacterState.Attack;
                            return;
                        }
                        else if (Type == MonsterType.AllRaidBoss)
                        {
                            if (_guildAllraidBossPattern == 0)
                            {
                                _guildAllraidBossPattern += 1;
                                State.Value = CharacterState.Attack_2;
                            }
                            else
                            {
                                _guildAllraidBossPattern += 1;

                                if (_guildAllraidBossPattern >= 4)
                                {
                                    _guildAllraidBossPattern = 0;
                                }

                                State.Value = CharacterState.Attack;
                            }

                        }

                        return;
                    }

                    if (IsPlayerInDetectionRange())
                    {
                        SetDirection(Managers.Game.MainPlayer.transform.position);
                        if (MoveSpeed.Value > 0)
                            State.Value = CharacterState.Move;
                    }
                    else
                    {
                        if (IsPatrolTime())
                            StartPatrol();
                        else
                            _patrolWaitingTime += Time.deltaTime;
                    }
                }
                break;
            case (int)DetectionType.AlwaysTracking: // 플레이어를 무조건 추적
                {
                    if (IsPlayerInTrackingRange())
                    {
                        if (Type == MonsterType.Boss)
                        {
                            if (Managers.Game.MainPlayer.IsDead)
                                return;

                            State.Value = CharacterState.Attack;
                            return;
                        }

                        return;
                    }

                    SetDirection(Managers.Game.MainPlayer.transform.position);

                    if (MoveSpeed.Value > 0)
                        State.Value = CharacterState.Move;
                }
                break;
        }
    }

    protected override void UpdateMove()
    {
        if (IsDead)
            return;

        if (Managers.Game.MainPlayer == null)
            return;

        if (_monsterChart.DetectionType == (int)DetectionType.NonDetection)
        {
            if (IsArrivePatrolDestination())
            {
                _nextPatrolWaitingTime = Random.Range(_minPatrolWaitingTime, _maxPatrolWaitingTime);
                State.Value = CharacterState.Idle;
            }
            else
            {
                MoveToPatrolDestination();
            }

            return;
        }

        if (_monsterChart.DetectionType == (int)DetectionType.RunAway)
        {
            if (IsArriveRunAwayDestination())
            {
                State.Value = CharacterState.Idle;
            }
            else
            {
                MoveToRunDestination();
            }

            return;
        }

        if (_monsterChart.DetectionType == (int)DetectionType.RunAwayToRandom)
        {
            if (IsArriveRunAwayRandomDestination())
            {
                State.Value = CharacterState.Idle;
            }
            else
            {
                MoveToRunRandomDestination();
            }

            return;
        }

        if (IsPlayerInTrackingRange())
        {
            switch (Type)
            {
                case MonsterType.Normal:
                    State.Value = CharacterState.Idle;
                    break;
                case MonsterType.Boss:
                    State.Value = CharacterState.Attack;
                    break;
                case MonsterType.AllRaidBoss:
                    if (_guildAllraidBossPattern == 1)
                    {
                        State.Value = CharacterState.Attack_2;
                        _guildAllraidBossPattern = 0;
                    }
                    else
                    {
                        State.Value = CharacterState.Attack;
                        _guildAllraidBossPattern = 1;
                    }
                    break;
            }

            return;
        }

        if (IsPlayerInDetectionRange() || _monsterChart.DetectionType == (int)DetectionType.AlwaysTracking)
        {
            SetDirection(Managers.Game.MainPlayer.transform.position);
            MoveToPlayer();
        }
        else
        {
            if (IsArrivePatrolDestination())
            {
                _nextPatrolWaitingTime = Random.Range(_minPatrolWaitingTime, _maxPatrolWaitingTime);
                State.Value = CharacterState.Idle;
            }
            else
            {
                MoveToPatrolDestination();
            }
        }
    }


    private bool IsPlayerInDetectionRange()
    {
        return _isTracking;
    }

    private bool IsPlayerInTrackingRange()
    {
        return _isArriveTracking;
    }

    private bool IsArrivePatrolDestination()
    {
        return Vector2.Distance(transform.position, _patrolDestination) <= ArriveCheckDistance;
    }

    private bool IsArriveRunAwayDestination()
    {
        return Vector2.Distance(transform.position, _runDestinations[_runDestinationIndex]) <= ArriveCheckDistance;
    }

    private bool IsArriveRunAwayRandomDestination()
    {
        return Vector2.Distance(transform.position, _runRandomDestination) <= ArriveCheckDistance;
    }

    private bool IsPatrolTime()
    {
        return _patrolWaitingTime >= _nextPatrolWaitingTime;
    }

    private void StartPatrol()
    {
        if (IsPlayerInTrackingRange())
        {
            if (Type == MonsterType.Boss)
            {
                if (Managers.Game.MainPlayer.IsDead)
                    return;

                State.Value = CharacterState.Attack;
                return;
            }
            else if (Type == MonsterType.AllRaidBoss)
            {
                if (_guildAllraidBossPattern == 1)
                {
                    State.Value = CharacterState.Attack_2;
                    _guildAllraidBossPattern = 0;
                }
                else
                {
                    State.Value = CharacterState.Attack;
                    _guildAllraidBossPattern = 1;
                }
            }
            
            return;
        }
        
        _patrolWaitingTime = 0;
        CharacterDirection patrolDirection
            = Random.Range(0, 2) == 0 ? CharacterDirection.Left : CharacterDirection.Right;
        float patrolDistance = Random.Range(_minPatrolDistance, _maxPatrolDistance);
        var position = transform.position;

        // 좌우 맵 크기 벗어낫는지 체크
        if (patrolDirection == CharacterDirection.Left)
        {
            patrolDistance *= -1;

            float destinationPosX = position.x + patrolDistance;

            if (Managers.Stage.State.Value == StageState.Dungeon &&
                Managers.Dungeon.DungeonId.Value == (int)DungeonType.Hwasengbang)
            {
                var gameViewMinPos = Managers.Game.GetGameViewMinPos();

                if (destinationPosX < gameViewMinPos.x)
                    patrolDistance = Math.Abs(patrolDistance);
            }
            else if (position.x + patrolDistance <= Managers.GameSystemData.MinSpawnPosition.x)
                patrolDistance = Math.Abs(patrolDistance);
        }
        else
        {
            float destinationPosX = position.x + patrolDistance;
            
            if (Managers.Stage.State.Value == StageState.Dungeon &&
                Managers.Dungeon.DungeonId.Value == (int)DungeonType.Hwasengbang)
            {
                var gameViewMaxPos = Managers.Game.GetGameViewMaxPos();

                if (destinationPosX > gameViewMaxPos.x)
                    patrolDistance *= -1;
            }
            else if (position.x + patrolDistance >= Managers.GameSystemData.MaxSpawnPosition.x)
                patrolDistance *= -1;
        }

        float patrolX = position.x + patrolDistance;
        _patrolDestination = new Vector3(patrolX, position.y, position.z);
        
        if (MoveSpeed.Value > 0)
            State.Value = CharacterState.Move;
    }

    private void MoveToPatrolDestination()
    {
        SetDirection(_patrolDestination);
        var direction = _patrolDestination - transform.position;
        direction.Normalize();
        transform.Translate(direction * (MoveSpeed.Value * Time.deltaTime));
    }

    private void MoveToRunDestination()
    {
        var destination = _runDestinations[_runDestinationIndex];
        SetDirection(destination);
        var direction = destination - (Vector2)transform.position;
        direction.Normalize();
        transform.Translate(direction * (MoveSpeed.Value * Time.deltaTime));
    }

    private void MoveToRunRandomDestination()
    {
        SetDirection(_runRandomDestination);
        var direction = _runRandomDestination - (Vector2)transform.position;
        direction.Normalize();
        transform.Translate(direction * (MoveSpeed.Value * Time.deltaTime));
    }

    private void MoveToPlayer()
    {
        SetDirection(Managers.Game.MainPlayer.transform.position);
        Vector3 direction = Managers.Game.MainPlayer.CenterPos - transform.position;
        direction.Normalize();
        transform.Translate(direction * (MoveSpeed.Value * Time.deltaTime));
    }

    public override void Damage(double damage, double criticalMultiple = 0, int teamIndex = -1)
    {
        base.Damage(damage, criticalMultiple, teamIndex);

        if (_monsterChart.DetectionType == (int)DetectionType.RunAway && State.Value == CharacterState.Idle)
        {
            var beforeRunDestinationIndex = _runDestinationIndex;
            _runDestinationIndex = Random.Range(0, _runDestinations.Count);

            if (_runDestinationIndex == beforeRunDestinationIndex)
            {
                var randNum = Random.Range(0, 2);

                if (randNum == 0)
                {
                    _runDestinationIndex += 1;
                    if (_runDestinationIndex >= _runDestinations.Count)
                        _runDestinationIndex = 0;
                }
                else
                {
                    _runDestinationIndex -= 1;
                    if (_runDestinationIndex < 0)
                        _runDestinationIndex = _runDestinations.Count - 1;
                }
            }

            State.Value = CharacterState.Move;
        }

        if (_monsterChart.DetectionType == (int)DetectionType.RunAwayToRandom && State.Value == CharacterState.Idle)
        {
            _runRandomDestination = Managers.Monster.GetRandomSpawnPosition();
            State.Value = CharacterState.Move;
        }

        if (Managers.Stage.State.Value == StageState.WorldCupEvent)
            Managers.WorldCupEvent.TotalReward.Value += DungeonRewardValue;
    }

    protected override void Dead()
    {
        base.Dead();

        switch (Managers.Stage.State.Value)
        {
            case StageState.Normal:
            {
                gameObject.SetActive(false);
                GiveStageItem();
                Managers.Stage.KillCount.Value =
                    Math.Min(Managers.Stage.KillCount.Value + 1, Managers.Stage.NeedKillCount);
                MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.KillMonster, Id, 1));
                Invoke(nameof(Respawn), _respawnTime);
            }
                break;
            case StageState.StageBoss:
            {
                gameObject.SetActive(false);
                Managers.Stage.ClearStage();
                MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.KillBoss, Id, 1));
            }
                break;
            case StageState.Dungeon:
            {
                switch (Managers.Dungeon.DungeonId.Value)
                {
                    case (int)DungeonType.Hwasengbang:
                    {
                        Invoke(nameof(Respawn), _respawnTime);
                    }
                        break;
                }

                gameObject.SetActive(false);

                if (Managers.Dungeon.KillCount.Value < Managers.Dungeon.ClearKillCount.Value)
                {
                    switch (DungeonRewardId)
                    {
                        case (int)Goods.Gold:
                            Managers.Dungeon.DungeonTotalReward.Value += DungeonRewardValue *
                                                                         Managers.Game.BaseStatDatas[
                                                                             (int)StatType.IncreaseGold];
                            break;
                        case (int)Goods.Exp:
                            Managers.Dungeon.DungeonTotalReward.Value += DungeonRewardValue *
                                                                         Managers.Game.BaseStatDatas[
                                                                             (int)StatType.IncreaseExp];
                            break;
                        default:
                            Managers.Dungeon.DungeonTotalReward.Value += DungeonRewardValue;
                            break;
                    }
                }

                if (Managers.Dungeon.DungeonId.Value == (int)DungeonType.Hwasengbang)
                {
                    if (Managers.Dungeon.HwasengbangDungeonWave == 2)
                    {
                        if (Id != ChartManager.HwasengbangDungeonCharts[Managers.Dungeon.DungeonStep.Value]
                                .SecondMonsterId)
                            return;
                    }
                }
                
                ++Managers.Dungeon.KillCount.Value;
            }
                break;
            case StageState.Promo:
                Managers.Dungeon.ClearPromo();
                break;
            case StageState.Raid:
                Managers.Raid.KillCount.Value += 1;
                gameObject.SetActive(false);
                break;
            case StageState.GuildRaid:
                Managers.GuildRaid.KillCount.Value += 1;
                gameObject.SetActive(false);
                break;
            case StageState.GuildAllRaid:
                gameObject.SetActive(false);
                Managers.AllRaid.Clear();
                break;
        }
    }

    private void GiveStageItem()
    {
        var stageChart = ChartManager.StageDataController.StageDataTable[Managers.Stage.StageId.Value];

        double goldValue = _goldValue * Managers.Game.BaseStatDatas[(int)StatType.IncreaseGold];
        double expValue = _expValue * Managers.Game.BaseStatDatas[(int)StatType.IncreaseExp];

        if (Managers.Game.BaseStatDatas[(int)StatType.FieldGoldBuff] >= 1)
            goldValue *= Managers.Game.BaseStatDatas[(int)StatType.FieldGoldBuff];
        
        if (Managers.Game.BaseStatDatas[(int)StatType.FieldExpBuff] >= 1)
            expValue *= Managers.Game.BaseStatDatas[(int)StatType.FieldExpBuff];
        
        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.Gold, goldValue);
        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.Exp, expValue);

        MessageBroker.Default.Publish(new UI_GainDropItemsPanel.GainDropItemData(
            (int)Goods.Gold, goldValue,
            (int)Goods.Exp, expValue));

        if (!Managers.Game.StageItemLog.ContainsKey((int)Goods.Gold))
            Managers.Game.StageItemLog.Add((int)Goods.Gold, 0);
            
        Managers.Game.StageItemLog[(int)Goods.Gold] += goldValue;

        if (Type == MonsterType.StageSpecial && stageChart.DropItemId != 0)
        {
            var randNum = Random.Range(0f, 1f);
            var dropItemRate = stageChart.DropItemRate * Managers.Game.HotTimeDropRate;

            if (Managers.Game.BaseStatDatas[(int)StatType.FieldTicketBuff] >= 1)
                dropItemRate *= Managers.Game.BaseStatDatas[(int)StatType.FieldTicketBuff];
            
            Debug.Log($"DropRate : {stageChart.DropItemRate:N7} X {Managers.Game.HotTimeDropRate:N7} = {dropItemRate:N7}");

            if (randNum <= dropItemRate)
            {
                Managers.Game.IncreaseItem(ItemType.Goods, stageChart.DropItemId, stageChart.DropItemValue);
                MessageBroker.Default.Publish(
                    new UI_GainDropItemsPanel.GainDropItemData(stageChart.DropItemId, stageChart.DropItemValue));
                
                if (!Managers.Game.StageItemLog.ContainsKey(stageChart.DropItemId))
                    Managers.Game.StageItemLog.Add(stageChart.DropItemId, 0);
            
                Managers.Game.StageItemLog[stageChart.DropItemId] += stageChart.DropItemValue;
            }
        }
    }

    private void OnAnimationEvent_Attack(float percent = 1f)
    {
        if (IsDead)
            return;

        switch (Type)
        {
            case MonsterType.Boss:
            {
                Managers.Game.MainPlayer.Damage(_attack * percent);

                    //if (Managers.Stage.State.Value == StageState.GuildAllRaid)
                    //{
                    //    for (int i = 0; i < Managers.AllRaid.PartyPlayers.Count; i++)
                    //    {
                    //        Managers.AllRaid.PartyPlayers[i].Damage(_attack * percent);
                    //        Debug.Log(_attack * percent);
                    //    }
                    //}
            }
                break;
            case MonsterType.AllRaidBoss:
                if (State.Value == CharacterState.Attack)
                {
                    Managers.Effect.ShowEffect("BossAttack_001_Effect", (Vector2)_attack_1_EffectPos.position);
                    
                    Managers.Game.MainPlayer.Damage(_attack * percent);
                }
                else if(State.Value == CharacterState.Attack_2)
                {
                    StopCoroutine(Cor_AllRaidBossAttack_2());
                    MainThreadDispatcher.StartCoroutine(Cor_AllRaidBossAttack_2());
                }
                break;
        }
    }

    private void OnAnimationEvent_EndAttack()
    {
        if (State.Value == CharacterState.None)
            return;
        
        State.Value = CharacterState.Idle;
    }

    private void OnAnimationEvent_Appear()
    {
        Managers.Effect.ShowEffect("Boss_999_Appear", new Vector2(30, 0));
    }
    private void OnAnimationEvent_EndAppear()
    {
        State.Value = CharacterState.Idle;
    }

    private void OnAnimationEvent_Attack_2(float percent = 1f)
    {
        StopCoroutine(Cor_AllRaidBossAttack_2());
        StartCoroutine(Cor_AllRaidBossAttack_2());
    }

    private IEnumerator Cor_AllRaidBossAttack_2(float percent = 1f)
    {
        int allPlayerCnt = Managers.AllRaid.AllPlayers.Count;
        int attackCnt = 0;

        WaitForSeconds delay = new WaitForSeconds(1.5f / allPlayerCnt);
        while (attackCnt < allPlayerCnt)
        {
            if(!Managers.AllRaid.AllPlayers[attackCnt].IsDie)
                Managers.AllRaid.AllPlayers[attackCnt].Damage(_attack * percent);

            

            attackCnt++;

            yield return delay;
        }
    }

    private void AnimationEvent_Attack_2_Effect(int effectCnt)
    {
        Managers.Effect.ShowEffect("BossAttack_002_Effect", _attack_2_EffectPos[effectCnt]);
    }
}