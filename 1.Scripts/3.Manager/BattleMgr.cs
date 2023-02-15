using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMgr : SingletonMonoBehaviour<BattleMgr>
{
    #region Fields
    private PlayerController _player;
    [SerializeField] PlayerController[] _playerprefab;
    [SerializeField] private GuidedMissileFollower _guidedMissileFollower;

    

    private bool _bossRaid;
    private float _bossHp;
    private float _curBossHp;
    public bool IsBossRaid { get { return _bossRaid; } }

    public PlayerController Player { get { return _player; } }
    public GuidedMissileFollower GuidedFollower { get { return _guidedMissileFollower; } }
    public float PlayTime { get; private set; }

    public int Level;
    public int StagaGold;
    public int KillCnt;
    public int BossCnt;
    public float CurrentExp;
    public float Max_Exp;
    public bool IsClear;
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        _player = Instantiate(_playerprefab[(int)PlayerDataMgr.Instance.PlayerData.CurTank]);
        _player.transform.position = Vector3.zero;
    }
    private void Start()
    {
        Level = 1;
        InGameUIMgr.Instance._gameUI.SetLevel(Level);
        Max_Exp = 20;
        CurrentExp = 0;
        PlayTime = 0;
        _bossRaid = false;
        IsClear = false;
        KillCnt = 0;
        InitBuffSkill();
    }
    private void Update()
    {
        if (!_bossRaid)
        {
            PlayTime += Time.deltaTime;
        }
    }
    #endregion

    #region public Methods
    public void InitBuffSkill()
    {
        BuffSkillController.Buff_BulletScale = 1;
        BuffSkillController.Buff_BulletSpeed = 1;
        BuffSkillController.Buff_CoolTime = 1;
        BuffSkillController.Buff_Hp = 1;
        BuffSkillController.Buff_Power = 1;
        BuffSkillController.Buff_Speed = 1;

    }
    public void GameOver(bool clear)
    {
        SoundMgr.Instance.MoveSfxStop();
        IsClear = clear;
        if (PlayerDataMgr.Instance.PlayerData.TopRecord < PlayTime && GameMgr.Instance.Chapter == 4)
        {
            PlayerDataMgr.Instance.PlayerData.TopRecord = PlayTime;
            PlayerDataMgr.Instance.SetRankData();
        }
        InGameUIMgr.Instance._gameUI.GameOver(IsClear);
        GameMgr.Instance.QuestAddCnt(eQUEST.ChapterPlay);
        if (GameMgr.Instance.Chapter == 3)
        {
            GameMgr.Instance.QuestAddCnt(eQUEST.SurvivalPlay);
        }
    }
    public void IncreaseEXP(int exp)
    {
        CurrentExp += exp;
        InGameUIMgr.Instance._gameUI.SetFillAmount(CurrentExp / Max_Exp);
        if (CurrentExp >= Max_Exp)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        InGameUIMgr.Instance._gameUI.InitFillAmount();
        Level++;
        CurrentExp -= Max_Exp;
        Max_Exp *= 1.3f;
        InGameUIMgr.Instance._gameUI.SetSkillSelection(true);
        InGameUIMgr.Instance._gameUI.SetFillAmount(CurrentExp / Max_Exp);
        InGameUIMgr.Instance._gameUI.SetLevel(Level);
    }

    public void BossRaid(bool isbossraid)
    {
        _bossRaid = isbossraid;
        InGameUIMgr.Instance._gameUI.InitBossHpFillAmount(_bossRaid);
    }
    public void SetBossHp(float hp)
    {
        _bossHp = hp;
        _curBossHp = hp;
    }
    public void BossHpDecrease(float hp)
    {
        _curBossHp = hp;
        InGameUIMgr.Instance._gameUI.SetBossHpFillAmount(_curBossHp / _bossHp);
    }
    #endregion
}