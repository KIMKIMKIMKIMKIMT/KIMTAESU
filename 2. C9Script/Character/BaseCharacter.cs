using System;
using System.Collections;
using System.Collections.Generic;
using SignInSample.Character;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    #region Animator Parameter Hash
    
    protected static readonly int IdleHash = Animator.StringToHash("Idle");
    protected static readonly int MoveHash = Animator.StringToHash("Move");
    protected static readonly int Move2Hash = Animator.StringToHash("Move2");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");
    protected static readonly int AttackHash2 = Animator.StringToHash("Attack2");
    protected static readonly int AttackHash_1 = Animator.StringToHash("Attack_1");
    protected static readonly int AttackHash_2 = Animator.StringToHash("Attack_2");
    protected static readonly int AppearHash = Animator.StringToHash("Appear");

    #endregion
    
    [SerializeField] protected Animator _animator;
    [SerializeField] protected List<SpriteRenderer> _spriteRenderers;
    [SerializeField] protected HpBar HpBar;
    [SerializeField] protected Transform ModelTr;

    protected bool _isPlayer;
    protected bool _isPartPlayer;
    
    public abstract Vector3 CenterPos { get; }

    public ReactiveProperty<CharacterState> State { get; } = new(CharacterState.None);
    public ReactiveProperty<CharacterDirection> Direction { get; } = new(CharacterDirection.Right);

    public double MaxHp { get; protected set; } = 0;

    public ReactiveProperty<double> Hp = new(0);

    public bool IsDead => (!IsPvp && !IsDps && !IsWorldCup && !IsGuildSports && Hp.Value <= 0) || !gameObject.activeSelf;
    public bool IsPvp => Managers.Stage.State.Value == StageState.Pvp;
    public bool IsDps => Managers.Stage.State.Value == StageState.Dps;
    public bool IsGuildSports => Managers.Stage.State.Value == StageState.GuildSports;

    public bool IsWorldCup => Managers.Stage.State.Value == StageState.WorldCupEvent;

    public bool IsDie;

    private Coroutine _hpBarTimerCoroutine;

    private void Awake()
    {
        if (HpBar != null)
        {
            HpBar.gameObject.SetActive(false);
            HpBar.Init(this);
        }
    }

    public virtual void Initialize()
    {
        SetData();
        SetPropertyEvent();
        HpBar.Off();

        IsDie = false;
    }

    public virtual void InitializeAllRaid()
    {
        SetData();
        SetPropertyEvent();
        HpBar.Off();

        IsDie = false;
    }

    protected abstract void SetData();

    protected virtual void SetPropertyEvent()
    {
        _propertyComposite.Clear();
        
        this.UpdateAsObservable().Where(_ =>State.Value == CharacterState.Idle)
            .Subscribe(_ => UpdateIdle()).AddTo(_propertyComposite);
        this.UpdateAsObservable().Where(_ =>  State.Value == CharacterState.Move)
            .Subscribe(_ => UpdateMove()).AddTo(_propertyComposite);

        State.Subscribe(UpdateAnimation);
        Direction.Subscribe(SetFlip);
    }

    public virtual void Respawn()
    {
    }

    protected virtual void UpdateAnimation(CharacterState state)
    {
        if (_animator == null)
            return;

        switch (state)
        {
            case CharacterState.None:
            case CharacterState.Idle:
                {
                    _animator.SetTrigger(IdleHash);
                }
                break;
            case CharacterState.Move:
                {
                    if (Managers.Stage.State.Value == StageState.WorldCupEvent)
                    {
                        if (Managers.Monster.WorldCupMonster.GetComponentInChildren<WorldCupMonster>().Phase > 2)
                            _animator.SetTrigger(Move2Hash);
                        else
                            _animator.SetTrigger(MoveHash);
                    }
                    else
                        _animator.SetTrigger(MoveHash);

                }
                break;
            //case CharacterState.Move2:
            //    {
            //        _animator.SetTrigger(Move2Hash);
            //    }
            //    break;
            case CharacterState.Attack:
                {
                    if (!_animator.ContainsHashKey(AttackHash))
                    {
                        State.Value = CharacterState.Idle;
                        return;
                    }

                    _animator.SetTrigger(AttackHash);
                }
                break;
            case CharacterState.Attack_2:
                if (!_animator.ContainsHashKey(AttackHash_2))
                {
                    State.Value = CharacterState.Idle;
                    return;
                }
                _animator.SetTrigger(AttackHash_2);
                break;
            case CharacterState.Appear:
                _animator.SetTrigger(AppearHash);
                break;
        }
    }

    protected abstract void UpdateIdle();
    protected abstract void UpdateMove();

    protected void SetDirection(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        Direction.Value = direction.x < 0 ? CharacterDirection.Left : CharacterDirection.Right;
    }

    // _animator가 붙어있는 오브젝트가 모델 오브젝트
    protected virtual void SetFlip(CharacterDirection direction)
    {
        if (ModelTr == null)
            return;
        
        var scale = ModelTr.localScale;
        scale.x = direction == CharacterDirection.Left ? Math.Abs(scale.x) * -1 : Math.Abs(scale.x);
        ModelTr.localScale = scale;
    }

    protected CompositeDisposable _propertyComposite = new();
    protected CompositeDisposable _hitColorComposite = new();

    public virtual void Damage(double damage, double criticalMultiple = 0, int teamIndex = -1)
    {
        if (!IsPvp && !IsDps && IsDead)
            return;

        if (State.Value == CharacterState.None)
            return;

        if (_isPlayer && !IsPvp && !_isPartPlayer)
        {
            damage /= (1 + (Managers.Game.BaseStatDatas[(int)StatType.Defence]
                * Managers.Game.BaseStatDatas[(int)StatType.DefencePer]) / 100);
        }

        Managers.DamageText.ShowDamageText(transform, damage, criticalMultiple);
        
        _hitColorComposite.Clear();

        HitEffect();

        if (Managers.Stage.State.Value != StageState.GuildAllRaid)
            Managers.Effect.ShowEffect("HitEffect", CenterPos);
        
        if (Managers.Stage.State.Value == StageState.Dps)
        {
            Managers.Dps.TotalDps.Value += damage;
            return;
        }
        if (Managers.Stage.State.Value == StageState.GuildSports)
        {
            if (teamIndex == 0)
            {
                Managers.GuildSports.MyGuildDps.Value += damage;
                return;
            }
            else if (teamIndex == 1)
            {
                Managers.GuildSports.EnemyGuildDps.Value += damage;
                return;
            }
            
        }

        if (Managers.Stage.State.Value == StageState.Pvp)
            return;

        if (!HpBar.gameObject.activeSelf && !Utils.IsHwasengbangDungeon() && !Utils.IsWorldCupDungeon())
        {
            if ((Managers.Stage.State.Value == StageState.Raid && Managers.Raid.Wave.Value == 3) || Managers.Stage.State.Value == StageState.Promo || Managers.Stage.State.Value == StageState.GuildAllRaid)
            {
                if (!tag.Equals("Monster"))
                    HpBar.On();
            }
            else
                HpBar.On();
        }
        
        if (_hpBarTimerCoroutine != null)
            StopCoroutine(_hpBarTimerCoroutine);
        
        _hpBarTimerCoroutine = StartCoroutine(CoHpBarTimer());

        if (!IsPvp)
            Hp.Value = Math.Max(Hp.Value - damage, 0.0);

        if (!IsPvp && !IsDps && !IsWorldCup && !IsGuildSports && IsDead)
        {
           Dead();
        }
    }
    

    private IEnumerator CoHpBarTimer()
    {
        yield return new WaitForSeconds(5f);
        
        HpBar.Off();
    }

    protected virtual void Dead()
    {
        if (Managers.Stage.State.Value == StageState.Dps || Managers.Stage.State.Value == StageState.GuildSports)
            return;
        
        if (_hpBarTimerCoroutine != null)
        {
            StopCoroutine(_hpBarTimerCoroutine);
            _hpBarTimerCoroutine = null;
        }
        
        HpBar.gameObject.SetActive(false);

        IsDie = true;
    }

    protected virtual void HitEffect()
    {
        _spriteRenderers.ForEach(sprite =>
        {
            if (sprite == null)
                return;

            if (Managers.Stage.State.Value == StageState.GuildAllRaid)
                return;

            sprite.color = Color.red;
            Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
            {
                if (sprite == null)
                    return;
                
                sprite.color = Color.white;
            }).AddTo(_hitColorComposite);
        });
    }
}