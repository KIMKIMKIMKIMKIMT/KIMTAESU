using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineController : AttackSkill
{
    #region Fields
    private MinePool _pool;

    private int _mineCnt;
    private int _curCnt;
    private float _duration;
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        _pool = GetComponent<MinePool>();
        SetSkillData(GameDataMgr.Instance.AttackSKillDataController.AttackSkillTable[eATTACKSKILL_LIST.Mine]);
    }
    #endregion

    #region Public Methods
    
    public override void Upgrade()
    {
        if (Level < 5)
        {
            _currentCoolTime = _coolTime;
            Level++;
            switch (Level)
            {
                case 2:
                    _totalDmg *= 3f;
                    break;
                case 3:
                    _mineCnt = 2;
                    break;
                case 4:
                    _totalDmg *= 3f;
                    break;
                case 5:
                    _mineCnt = 3;
                    break;
            }
        }
    }

    protected override void Attack()
    {
        StopAllCoroutines();
        StartCoroutine(MineSpawn());
    }

    protected override void SetSkillData(AttackSkillData skillData)
    {
        Level = 1;
        _duration = 0.5f;
        _mineCnt = 1;
        _skillData = skillData;
        _coolTime = skillData.CoolTime * BuffSkillController.Buff_CoolTime;
        _originCoolTime = skillData.CoolTime;
        _totalDmg = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().GetTankAtk() * 3;
    }
    public override void SetCoolTime()
    {
        _coolTime = _originCoolTime * BuffSkillController.Buff_CoolTime;
    }
    #endregion

    #region Coroutines
    private IEnumerator MineSpawn()
    {
        WaitForSeconds _wait = new WaitForSeconds(_duration);
        _curCnt = 0;
        while (true)
        {
            float ranX = Random.Range(-2f, 2f);
            float ranY = Random.Range(-2f, 2f);
            Mine obj = _pool.GetFromPool(0);
            obj.transform.position = new Vector3(transform.position.x + ranX, transform.position.y + ranY, 0);
            obj.SetBullet(_totalDmg * BuffSkillController.Buff_Power, 5f);

            _curCnt++;

            if (_curCnt >= _mineCnt)
            {
                yield break;
            }
            yield return _wait;
        }
    }
    #endregion
}
