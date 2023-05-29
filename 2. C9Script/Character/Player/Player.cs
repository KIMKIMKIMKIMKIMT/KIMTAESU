using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using GameData;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.U2D.Animation;

public partial class Player : BaseCharacter
{
    #region Animator Parameter Hash

    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");

    #endregion
    [SerializeField] public int GuildSportsTeamIndex { get; private set; }

    [SerializeField] private Vector3 _centerOffset;
    [SerializeField] private TMP_Text NicknameText;

    [SerializeField] private float _attackRange = 3f;

    [SerializeField] private SpriteResolver[] CharacterSpriteResolvers;
    [SerializeField] private SpriteResolver WeaponSpriteResolver;

    [SerializeField] public Transform SkillTr;

    [SerializeField] private List<GameObject> FeverEffectObjs;

    [SerializeField] private Pet Pet;
    [SerializeField] private GameObject CostumeEffectObj;
    [SerializeField] private GameObject CostumeLegenoEffectObj;

    [SerializeField] private Transform SkillEffectRoot;
    public Transform GuildSportsTargetGroup;

    private readonly Color _feverModeColor = new(100f / 255f, 100f / 255f, 100f / 255f);
    public CinemachineVirtualCamera _gameCamera;
    
    private readonly Queue<Action> _nextSkillAnimations = new();
    private GameObject _petObj;
    private Vector2 _raidPortalDestination;
    private Vector3 _allRaidCircleDestination;
    private Vector3 _guildSportsDestination;
    private Vector2 _allRaidRunDestination = new Vector2(30, 0);

    private Action EndEvent;

    private List<SpriteRenderer> _skillEffectSpriteRenderers;

    public override Vector3 CenterPos => transform.position + _centerOffset;

    public Monster TargetMonster;
    public ReactiveProperty<bool> AutoSkillMode { get; } = new(false);
    public ReactiveProperty<bool> AutoFeverMode { get; } = new(false);

    public ReactiveProperty<float> FeverGage = new(0);
    public ReactiveProperty<bool> IsFeverMode = new(false);

    public Action OnAnimationAttackEvent;
    public FloatReactiveProperty MoveSpeed = new(0);
    public FloatReactiveProperty AttackSpeed = new(0);

    public bool IsPvpEnemyPlayer;
    public bool IsPartyPlayer;
    
    public bool IsMine => !IsPvpEnemyPlayer && !IsPartyPlayer;

    private Coroutine _skillCoolTimerCoroutine;
    public Coroutine _hpRecoveryCoroutine;
    
    public Dictionary<int, double> StatDatas;
    public int PartyPlayer_Passive_Level;

    private float _partyPlayerRecoveryTime;

    public CinemachineTargetGroup TargetGroupFollow;

    public int PassiveGodgodCount { get; private set; }
    public int PartyPlayerPassiveGodgodCount { get; private set; }
    public int IsPvpEnemyPlayerPassiveGodgodCount { get; private set; }

    private static bool IsSkillAnimation(int skillIndex)
    {
        return skillIndex == 2 ||
               skillIndex == 7 ||
               skillIndex == 4;
    }

    private bool IsSkillAnimationUsing()
    {
        return UsingSkillIds.Contains(4) ||
               UsingSkillIds.Contains(2) ||
               UsingSkillIds.Contains(7);
    }

    private void Awake()
    {
        _gameCamera = FindObjectOfType<CinemachineVirtualCamera>();
        _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.BaseCameraSize;

        _skillEffectSpriteRenderers = SkillEffectRoot.GetComponentsInChildren<SpriteRenderer>(true).ToList();

        Managers.Game.SettingData.SkillEffect.Subscribe(value =>
        {
            if (_skillEffectSpriteRenderers == null)
                return;
            
            _skillEffectSpriteRenderers.ForEach(sr =>
            {
                if (sr == null)
                    return;

                sr.enabled = value;
            });
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        if (IsMine)
        {
            if (_gameCamera != null)
                _gameCamera.Follow = transform;
            _isPlayer = true;
            PassiveGodgodCount = 0;
        }

        if (IsPvpEnemyPlayer)
        {
            HpBar.Off();
            Managers.Stage.State.Subscribe(state =>
            {
                if (state != StageState.Pvp)
                    DestroyPlayer();
            }).AddTo(gameObject);
            PartyPlayerPassiveGodgodCount = 0;
        }
        
        NicknameText.gameObject.SetActive(IsPartyPlayer);
    }

    public override void InitializeAllRaid()
    {
        base.InitializeAllRaid();

        if (IsMine)
        {
            if (_gameCamera != null)
                _gameCamera.Follow = transform;
            _isPlayer = true;
            PassiveGodgodCount = 0;
        }
        else
        {
            _isPartPlayer = true;
            PartyPlayerPassiveGodgodCount = 0;
        }

        if (IsPvpEnemyPlayer)
        {
            HpBar.Off();
            Managers.Stage.State.Subscribe(state =>
            {
                if (state != StageState.Pvp)
                    DestroyPlayer();
            }).AddTo(gameObject);
            PartyPlayerPassiveGodgodCount = 0;
        }

        NicknameText.gameObject.SetActive(IsPartyPlayer);

    }
    // 데이터 셋팅
    protected override void SetData()
    {
        RefreshStat();
        Hp.Value = MaxHp;
    }

    protected override void SetPropertyEvent()
    {
        base.SetPropertyEvent();

        MoveSpeed.Subscribe(moveSpeed => { _animator.SetFloat(MoveSpeedHash, moveSpeed * 0.5f); });
        AttackSpeed.Subscribe(attackSpeed => { _animator.SetFloat(AttackSpeedHash, attackSpeed * 0.5f); });

        if (IsPvpEnemyPlayer)
            return;

        State.Where(state => state == CharacterState.None).Subscribe(_ =>
        {
            if (IsMine)
            {
                HpBar.gameObject.SetActive(false);
                ResetHp();
                AllStopSkill();

                if (Managers.Stage.State.Value != StageState.Normal &&
                    Managers.Stage.State.Value != StageState.StageBoss)
                    DisableFeverMode();
            }

            TargetMonster = null;
            Direction.Value = CharacterDirection.Right;
        });

        this.UpdateAsObservable().Where(_ => 
            (Managers.Stage.State.Value == StageState.Normal || Managers.Stage.State.Value == StageState.StageBoss) && 
            State.Value != CharacterState.RaidPortalMove && State.Value != CharacterState.None).Subscribe(_ =>
        {
            var pos = transform.position;
            var beforePos = pos;

            if (transform.position.x < Managers.GameSystemData.MinSpawnPosition.x)
                pos.x = Managers.GameSystemData.MinSpawnPosition.x;

            if (transform.position.x > Managers.GameSystemData.MaxSpawnPosition.x)
                pos.x = Managers.GameSystemData.MaxSpawnPosition.x;

            if (transform.position.y < Managers.GameSystemData.MinSpawnPosition.y)
                pos.y = Managers.GameSystemData.MinSpawnPosition.y;

            if (transform.position.y > Managers.GameSystemData.MaxSpawnPosition.y)
                pos.y = Managers.GameSystemData.MaxSpawnPosition.y;

            if (pos != beforePos)
                transform.position = pos;

        }).AddTo(_propertyComposite);

        //this.UpdateAsObservable().Where(_ => State.Value == CharacterState.Attack).Subscribe(_ => UpdateAttackDelay()).AddTo(_propertyComposite);

        this.UpdateAsObservable().Where(_ => State.Value == CharacterState.RaidPortalMove).Subscribe(_ => UpdateRaidPortalMove()).AddTo(_propertyComposite);

        this.UpdateAsObservable().Where(_ => State.Value == CharacterState.AllRaidRunMove).Subscribe(_ => UpdateAllRaidRunMove()).AddTo(_propertyComposite);

        this.UpdateAsObservable().Where(_ => State.Value == CharacterState.AllRaidCircleMove).Subscribe(_ => UpdateAllRaidCircleMove()).AddTo(_propertyComposite);

        this.UpdateAsObservable().Where(_ => Managers.Stage.State.Value == StageState.GuildAllRaid && IsPartyPlayer && Managers.AllRaid.IsPartyPlayerReady).Subscribe(_ => UpdateAllRaidPartyPlayerHpRecovery()).AddTo(_propertyComposite);

        this.UpdateAsObservable().Where(_ => State.Value == CharacterState.GuildSportsEndMove && Managers.Stage.State.Value == StageState.GuildSports).Subscribe(_ => UpdateGuildSportsMove()).AddTo(_propertyComposite);
        Managers.Stage.State.Where(_ => IsMine).Subscribe(state =>
        {
            if (state != StageState.Normal && state != StageState.StageBoss)
                DisableFeverMode();

            switch (state)
            {
                case StageState.Normal:
                case StageState.Promo:
                case StageState.Pvp:
                {
                    if (state == StageState.Normal)
                        ResetHp();

                    if (state == StageState.Pvp)
                    {
                        _gameCamera.Follow = null;
                        _gameCamera.transform.localPosition = new Vector3(0, 0, _gameCamera.transform.localPosition.z);
                    }
                    else
                    {
                        _gameCamera.Follow = transform;
                        _gameCamera.m_Lens.OrthographicSize = Managers.GameSystemData.BaseCameraSize;
                    }
                }
                    break;
            }
        }).AddTo(_propertyComposite);

        Managers.Stage.State.Where(_ => IsPartyPlayer && Managers.AllRaid.IsPartyPlayerReady).Subscribe(state =>
        {
            if (state == StageState.GuildAllRaid)
            {
                ResetHp();
                _partyPlayerRecoveryTime = 0;
            }
        });

        if (IsMine)
        {
            if (_skillCoolTimerCoroutine != null)
                StopCoroutine(_skillCoolTimerCoroutine);

            _skillCoolTimerCoroutine = StartCoroutine(CoSkillCoolTimer());

            if (_hpRecoveryCoroutine != null)
                StopCoroutine(_hpRecoveryCoroutine);

            _hpRecoveryCoroutine = StartCoroutine(CoHpRecovery());
        }

        this.UpdateAsObservable().Where(_ => IsMine).Subscribe(_ =>
        {
            if (!IsFeverMode.Value)
            {
                FeverGage.Value =
                    Mathf.Min(
                        FeverGage.Value +
                        (ChartManager.SystemCharts[SystemData.Fever_Per_Sec].Value *
                         Time.deltaTime), 100f);
            }
        }).AddTo(_propertyComposite);

        // 피버모드 사용 체크
        this.UpdateAsObservable().Where(_ =>
            (Managers.Stage.State.Value == StageState.Normal ||
             Managers.Stage.State.Value == StageState.StageBoss) && State.Value != CharacterState.None && IsMine).Subscribe(_ =>
        {
            if (FeverGage.Value >= 100f && AutoFeverMode.Value)
                EnableFeverMode();
        }).AddTo(_propertyComposite);

        // 자동스킬모드 일때 쿨타임 체크후 사용
        this.UpdateAsObservable().Where(_ => AutoSkillMode.Value &&
                                             State.Value != CharacterState.None &&
                                             !IsPvp && IsMine && !Managers.AllRaid.IsAllRaidSkillLock).Subscribe(_ =>
        {
            for (int i = 0; i < SkillInfos.Count; i++)
            {
                var skillInfo = SkillInfos[i];

                if (skillInfo.SkillIndex == 0)
                    continue;

                if (skillInfo.IsCoolTime.Value)
                    continue;

                StartAutoSkill(i);
            }
        }).AddTo(_propertyComposite);

        AutoSkillMode.Where(_ => !IsPvp && IsMine).Subscribe(autoSkill =>
        {
            if (autoSkill)
            {
                for (int i = 0; i < SkillInfos.Count; i++)
                {
                    int slotIndex = i;
                    StartAutoSkill(slotIndex);
                }
            }
            else
            {
                _nextSkillAnimations.Clear();
            }
        }).AddTo(_propertyComposite);

        for (int i = 0; i < Managers.Game.EquipSkillList.Count; i++)
        {
            int quickSlotIndex = i;

            for (int j = 0; j < Managers.Game.EquipSkillList[i].Length; j++)
            {
                int skillSlotIndex = j;

                Managers.Game.EquipSkillList[quickSlotIndex][skillSlotIndex].Where(_ => !IsPvp && IsMine).Subscribe(skillIndex =>
                {
                    if (Managers.Game.SkillQuickSlotIndex.Value != quickSlotIndex)
                        return;

                    SkillInfos[skillSlotIndex].SkillIndex = skillIndex;

                    if (skillIndex == 0)
                        return;

                    var coolTime =
                        (float)(ChartManager.SkillCharts[skillIndex].CoolTime -
                                ChartManager.SkillCharts[skillIndex].CoolTime *
                                Managers.Game.BaseStatDatas[(int)StatType.SkillCoolTimeReduce]);

                    if (SkillCoolTimes.ContainsKey(skillIndex))
                        SkillCoolTimes[skillIndex].Value = coolTime;
                    else
                        SkillCoolTimes.Add(skillIndex, new ReactiveProperty<float>(coolTime));
                    
                }).AddTo(_propertyComposite);
            }
        }

        bool isInit = true;
        Managers.Game.SkillQuickSlotIndex.Where(_ => !IsPvp && IsMine).Subscribe(quickSlotIndex =>
        {
            for (int skillSlotIndex = 0; skillSlotIndex < SkillInfos.Count; skillSlotIndex++)
            {
                SkillInfos[skillSlotIndex].SkillIndex =
                    Managers.Game.EquipSkillList[quickSlotIndex][skillSlotIndex].Value;
            }

            if (!isInit)
            {
                foreach (var skillInfo in SkillInfos)
                {
                    if (skillInfo.SkillIndex == 0)
                        continue;

                    if (skillInfo.IsCoolTime.Value)
                        continue;

                    var coolTime =
                        (float)(ChartManager.SkillCharts[skillInfo.SkillIndex].CoolTime -
                                ChartManager.SkillCharts[skillInfo.SkillIndex].CoolTime *
                                Managers.Game.BaseStatDatas[(int)StatType.SkillCoolTimeReduce]);

                    if (SkillCoolTimes.ContainsKey(skillInfo.SkillIndex))
                        SkillCoolTimes[skillInfo.SkillIndex].Value = coolTime;
                    else
                        SkillCoolTimes.Add(skillInfo.SkillIndex, new ReactiveProperty<float>(coolTime));
                }
            }

            isInit = false;
        }).AddTo(_propertyComposite);
    }

    IEnumerator CoSkillCoolTimer()
    {
        while (true)
        {
            foreach (var skillId in SkillCoolTimes.Keys)
            {
                if (SkillCoolTimes[skillId].Value <= 0f)
                    continue;

                SkillCoolTimes[skillId].Value -= Time.deltaTime;
            }

            yield return null;
        }
    }

    IEnumerator CoHpRecovery()
    {
        float time = 0;

        while (true)
        {
            time += Time.deltaTime;

            if (Hp.Value < MaxHp && time >= 1f)
            {
                if (Managers.Game.BaseStatDatas[(int)StatType.Hp_Recovery] > 0)
                    Hp.Value = Math.Min(MaxHp, Hp.Value + Managers.Game.BaseStatDatas[(int)StatType.Hp_Recovery]);

                time = 0;
            }

            yield return null;
        }
    }

    public void RefreshStat()
    {
        if (IsMine)
        {
            MaxHp = Math.Round(
                    Managers.Game.BaseStatDatas[(int)StatType.Hp] * Managers.Game.BaseStatDatas[(int)StatType.HpPer], 7);
        }
        else
        {
            if (StatDatas != null)
            {
                MaxHp = Math.Round(
                    StatDatas[(int)StatType.Hp] * StatDatas[(int)StatType.HpPer], 7);
            }
        }

        double moveSpeed = 0;
        double moveSpeedPer = 0;
        double attackSpeed = 0;
        double attackSpeedPer = 0;

        if (IsMine)
        {
            moveSpeed = Managers.Game.BaseStatDatas[(int)StatType.MoveSpeed];
            moveSpeedPer = Managers.Game.BaseStatDatas[(int)StatType.MoveSpeedPer];
            attackSpeed = Managers.Game.BaseStatDatas[(int)StatType.AttackSpeed];
            attackSpeedPer = Managers.Game.BaseStatDatas[(int)StatType.AttackSpeedPer];
        }
        else if (IsPvpEnemyPlayer)
        {
            moveSpeed = Managers.Pvp.EnemyBaseStatDatas[(int)StatType.MoveSpeed];
            moveSpeedPer = Managers.Pvp.EnemyBaseStatDatas[(int)StatType.MoveSpeedPer];
            attackSpeed = Managers.Pvp.EnemyBaseStatDatas[(int)StatType.AttackSpeed];
            attackSpeedPer = Managers.Pvp.EnemyBaseStatDatas[(int)StatType.AttackSpeedPer];
        }
        else if (IsPartyPlayer)
        {
            moveSpeed = StatDatas[(int)StatType.MoveSpeed];
            moveSpeedPer = StatDatas[(int)StatType.MoveSpeedPer];
            attackSpeed = StatDatas[(int)StatType.AttackSpeed];
            attackSpeedPer = StatDatas[(int)StatType.AttackSpeedPer];
        }

        MoveSpeed.Value = (float)Math.Round(moveSpeed * moveSpeedPer, 7);
        AttackSpeed.Value = (float)Math.Round(attackSpeed * attackSpeedPer, 7);
    }

    protected override void SetFlip(CharacterDirection direction)
    {
        base.SetFlip(direction);

        var scale = SkillTr.localScale;
        scale.x = direction == CharacterDirection.Left ? Math.Abs(scale.x) * -1 : Math.Abs(scale.x);
        SkillTr.localScale = scale;
    }

    public void SetRaidPortalMove(Vector2 portalDestination)
    {
        _raidPortalDestination = portalDestination;
        State.Value = CharacterState.RaidPortalMove;
    }
    public void SetAllRaidRunMove()
    {
        State.Value = CharacterState.AllRaidRunMove;
    }
    public void SetAllRaidCircleMove(Vector3 circleDestination)
    {
        _allRaidCircleDestination = circleDestination;
        State.Value = CharacterState.AllRaidCircleMove;
    }

    public void SetGuildSportsEndDirection(Vector2 dir)
    {
        _guildSportsDestination = dir;
        State.Value = CharacterState.GuildSportsEndMove;
    }

    protected override void UpdateIdle()
    {
        if (IsMine && IsDead)
            return;

        if (IsPvp)
        {
            State.Value = CharacterState.Attack;
            return;
        }

        if (TargetMonster == null)
        {
            FindTarget();
            if (TargetMonster == null)
                return;
        }
        
        if (IsDps)
        {
            State.Value = CharacterState.Attack;
            return;
        }

        if (TargetMonster.IsDead)
        {
            TargetMonster = null;
            return;
        }

        SetDirection();

        State.Value =
            TargetInAttackRange() ? CharacterState.Attack : CharacterState.Move;
    }

    private void SetDirection()
    {
        Vector3 direction = TargetMonster.transform.position - CenterPos;
        Direction.Value = direction.x < 0 ? CharacterDirection.Left : CharacterDirection.Right;
    }

    protected override void UpdateMove()
    {
        if (TargetMonster == null)
        {
            TargetMonster = Managers.Monster.FindTargetMonster(CenterPos);
            if (TargetMonster == null)
                State.Value = CharacterState.Idle;
            return;
        }

        if (TargetMonster.IsDead)
        {
            TargetMonster = null;
            return;
        }

        if (TargetInAttackRange())
        {
            State.Value = CharacterState.Attack;
            return;
        }

        var targetPosition = TargetMonster.CenterPos;

        Vector2 direction = targetPosition - CenterPos;
        direction.Normalize();

        SetDirection(targetPosition);

        transform.Translate(direction * (MoveSpeed.Value * Time.deltaTime));
    }

    private void UpdateRaidPortalMove()
    {
        var targetPosition = _raidPortalDestination;

        if (Vector2.Distance(transform.position, _raidPortalDestination) <= 0.8f)
        {
            State.Value = CharacterState.None;
            if (Managers.Stage.State.Value == StageState.Raid)
                Managers.Raid.StartNextWave();
            else if (Managers.Stage.State.Value == StageState.GuildRaid)
                Managers.GuildRaid.StartNextWave();
            return;
        }
        
        Vector2 direction = targetPosition - (Vector2)transform.position;
        direction.Normalize();

        SetDirection(targetPosition);

        transform.Translate(translation: direction * (MoveSpeed.Value * Time.deltaTime));
    }

    private void UpdateAllRaidRunMove()
    {
        transform.Translate(translation: Vector2.right * (10 * Time.deltaTime));
        SetDirection(_allRaidRunDestination);
    }

    private void UpdateAllRaidCircleMove()
    {
        Vector3 circlePos = _allRaidCircleDestination - transform.position;

        SetDirection(_allRaidCircleDestination);
        circlePos.Normalize();

        transform.Translate(circlePos * (15 * Time.deltaTime));

        if (Vector2.Distance(transform.position, _allRaidCircleDestination) <= 0.8f)
        {
            transform.position = _allRaidCircleDestination;
            State.Value = CharacterState.Idle;
            SetDirection(_allRaidRunDestination);
            
        }
    }

    private void UpdateAllRaidPartyPlayerHpRecovery()
    {
        _partyPlayerRecoveryTime += Time.deltaTime;

        if (Hp.Value < MaxHp && _partyPlayerRecoveryTime >= 1f)
        {
            if (StatDatas[(int)StatType.Hp_Recovery] > 0)
                Hp.Value = Math.Min(MaxHp, Hp.Value + StatDatas[(int)StatType.Hp_Recovery]);

            _partyPlayerRecoveryTime = 0;
        }
    }

    private void UpdateGuildSportsMove()
    {
        Vector2 direction = _guildSportsDestination - transform.position;

        SetDirection(_guildSportsDestination);
        direction.Normalize();

        transform.Translate(direction * (5 * Time.deltaTime));

        if (Vector2.Distance(transform.position, _guildSportsDestination) <= 1f)
        {
            transform.position = _guildSportsDestination;
            State.Value = CharacterState.None;
            SetDirection(Vector2.one);
        }
    }

    protected override void UpdateAnimation(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.None:
            {
                _animator.SetTrigger(IdleHash);
            }
                break;
            case CharacterState.Idle:
            {
                _animator.SetTrigger(IdleHash);
            }
                break;
            case CharacterState.Move:
            case CharacterState.RaidPortalMove:
            case CharacterState.AllRaidRunMove:
            case CharacterState.AllRaidCircleMove:
            case CharacterState.GuildSportsEndMove:
            {
                _animator.SetTrigger(MoveHash);
            }
                break;
            case CharacterState.Attack:
            {
                    //if (IsMine)
                    //{
                    //    if (PassiveGodgodCount >= 2)
                    //    {
                    //        _animator.SetTrigger(AttackHash2);
                    //    }
                    //    else
                    //    {
                    //        _animator.SetTrigger(AttackHash);
                    //    }
                    //}
                    //else if (IsPartyPlayer)
                    //{
                    //    if (PartyPlayerPassiveGodgodCount >= 2)
                    //    {
                    //        _animator.SetTrigger(AttackHash2);
                    //    }
                    //    else
                    //    {
                    //        _animator.SetTrigger(AttackHash);
                    //    }
                    //}
                    //else if (IsPvpEnemyPlayer)
                    //{
                    //    if (IsPvpEnemyPlayerPassiveGodgodCount >= 2)
                    //    {
                    //        _animator.SetTrigger(AttackHash2);
                    //    }
                    //    else
                    //    {
                    //        _animator.SetTrigger(AttackHash);
                    //    }
                    //}
                    _animator.SetTrigger(AttackHash);
                }
                break;
        }
    }

    protected override void Dead()
    {
        base.Dead();
        switch (Managers.Stage.State.Value)
        {
            case StageState.StageBoss:
            {
                Managers.Stage.FailStage();
            }
                break;
            case StageState.Promo:
            {
                Managers.Dungeon.FailPromo();
            }
                break;
            case StageState.Dungeon:
            {
                Managers.Dungeon.FailDungeon();
            }
                break;
            case StageState.Raid:
                Managers.Raid.Fail();
                break;
            case StageState.GuildRaid:
                Managers.GuildRaid.Fail();
                break;
            case StageState.GuildAllRaid:
                if (_isPlayer)
                {
                    Managers.AllRaid.Fail();
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
        }
    }


    private bool TargetInAttackRange()
    {
        if (TargetMonster == null)
            return false;

        if (Managers.Stage.State.Value == StageState.GuildAllRaid)
        {
            return Vector3.Distance(TargetMonster.CenterPos, CenterPos) <= _attackRange + 20;
        }

        if (Managers.Stage.State.Value == StageState.GuildSports)
        {
            return Vector3.Distance(TargetMonster.CenterPos, CenterPos) <= _attackRange + 4;
        }

        return Vector3.Distance(TargetMonster.CenterPos, CenterPos) <= _attackRange - 0.5f;
    }

    public void ResetHp()
    {
        Hp.Value = MaxHp;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(CenterPos, _attackRange);
    }

    public void ChangeWeapon(string labelName)
    {
        WeaponSpriteResolver.SetCategoryAndLabel("Weapon", labelName);
    }

    public string GetWeaponLabel()
    {
        return WeaponSpriteResolver.GetLabel();
    }

    public void SetSkillAnimation(int skillId)
    {
        State.Value = CharacterState.Skill;
        _animator.SetTrigger($"Skill{skillId}");
    }

    public void SetWeapon(int weaponId)
    {
        WeaponSpriteResolver.SetCategoryAndLabel(WeaponSpriteResolver.GetCategory(), weaponId.ToString());
    }

    public void SetCostume(int costumeId)
    {
        if (ChartManager.CostumeCharts.TryGetValue(costumeId, out var costumeChart))
        {
            CostumeEffectObj.SetActive(costumeChart.Grade == Grade.Legend);
            CostumeLegenoEffectObj.SetActive(costumeChart.Grade == Grade.Legeno);
        }
        else
        {
            CostumeEffectObj.SetActive(costumeId == 5006 || costumeId == 5007 || costumeId == 5008);
            CostumeLegenoEffectObj.SetActive(costumeId == 5009 || costumeId == 5010 || costumeId == 5011);
        }

        foreach (var spriteResolver in CharacterSpriteResolvers)
            spriteResolver.SetCategoryAndLabel(spriteResolver.GetCategory(), costumeId.ToString());
    }

    public void SetPet(int petId)
    {
        Pet.SetPet(petId);
    }

    public void EnableFeverMode()
    {
        if (Managers.Stage.State.Value != StageState.Normal &&
            Managers.Stage.State.Value != StageState.StageBoss &&
            State.Value == CharacterState.None)
            return;

        if (FeverGage.Value < 100f)
            return;

        _spriteRenderers.ForEach(sprite => sprite.color = _feverModeColor);
        FeverEffectObjs.ForEach(obj => obj.SetActive(true));

        IsFeverMode.Value = true;

        Managers.Game.CalculateStat();

        StartCoroutine(CoFeverCamEffect(true));

        FeverGage.Value = 0;

        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.UseFever));

        StartCoroutine(CoFeverTimer());
    }

    private void DisableFeverMode()
    {
        if (!IsFeverMode.Value)
            return;

        _spriteRenderers.ForEach(sprite => sprite.color = Color.white);
        FeverEffectObjs.ForEach(obj => obj.SetActive(false));

        StartCoroutine(CoFeverCamEffect(false));

        IsFeverMode.Value = false;
        Managers.Game.CalculateStat();
    }

    private IEnumerator CoFeverTimer()
    {
        yield return new WaitForSeconds(ChartManager.SystemCharts[SystemData.Fever_Duration].Value *
                                        (float)Managers.Game.BaseStatDatas[(int)StatType.IncreaseFeverDuration]);

        DisableFeverMode();
    }

    private IEnumerator CoFeverCamEffect(bool isFever)
    {
        float increaseValue = (1.2f / 0.8f) * Time.deltaTime;

        if (isFever)
        {
            while (true)
            {
                if (!IsFeverMode.Value)
                    yield break;

                _gameCamera.m_Lens.OrthographicSize += increaseValue;

                if (_gameCamera.m_Lens.OrthographicSize >= Managers.GameSystemData.FeverCameraSize)
                    break;

                yield return null;
            }
        }
        else
        {
            while (true)
            {
                if (Managers.Stage.State.Value != StageState.Normal &&
                    Managers.Stage.State.Value != StageState.StageBoss)
                    yield break;

                _gameCamera.m_Lens.OrthographicSize -= increaseValue;

                if (_gameCamera.m_Lens.OrthographicSize <= 12f)
                    break;

                yield return null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.tag.Equals("Monster"))
            return;

        TargetMonster = col.GetComponent<Monster>();
    }

    protected override void HitEffect()
    {
        _spriteRenderers.ForEach(sprite =>
        {
            if (sprite == null)
                return;

            sprite.color = Color.red;
            Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
            {
                if (IsFeverMode.Value)
                    sprite.color = _feverModeColor;
                else
                    sprite.color = Color.white;
            }).AddTo(_hitColorComposite);
        });
    }

    public void FindTarget()
    {
        TargetMonster = Managers.Monster.FindTargetMonster(CenterPos);
    }

    public void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    public void SetActiveModel(bool isActive)
    {
        
    }

    public void SetAllSkillCoolTime()
    {
        if (SkillInfos == null)
            return;
        
        foreach (var skillInfo in SkillInfos.Where(skillInfo => skillInfo.SkillIndex != 0))
        {
            if (SkillCoolTimes.ContainsKey(skillInfo.SkillIndex))
                SkillCoolTimes[skillInfo.SkillIndex].Value = skillInfo.SkillCoolTime;
            else
                SkillCoolTimes.Add(skillInfo.SkillIndex, new ReactiveProperty<float>(skillInfo.SkillCoolTime));
        }
    }

    public void SetNickname(string nickname)
    {
        NicknameText.text = nickname;
    }

    public void SetScale(Vector2 scale)
    {
        transform.localScale = scale;
    }

    public override void Damage(double damage, double criticalMultiple = 0, int teamIndex = -1)
    {
        if (IsPartyPlayer)
        {
            damage /= (1 + (StatDatas[(int)StatType.Defence]
                * StatDatas[(int)StatType.DefencePer]) / 100);

            base.Damage(damage, criticalMultiple);
        }
        else
        {
            base.Damage(damage, criticalMultiple);
        }
        

        
    }

    public void SetTeamIndex(int index)
    {
        GuildSportsTeamIndex = index;
    }

    public void InitPassiveCount()
    {
        PassiveGodgodCount = 0;
    }
}