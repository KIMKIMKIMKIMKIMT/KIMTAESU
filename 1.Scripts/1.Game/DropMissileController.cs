using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DropMissileController : AttackSkill
{
    #region Fields
    private DropMissilePool _pool;
    private DropMissileCrossHairPool _crossHairPool;

    [SerializeField] private float _range;
    [SerializeField] private float _height;
    private float _duration;
    private int _bulletCnt;
    private int _curBulletCnt;
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        _pool = GetComponent<DropMissilePool>();
        _crossHairPool = GetComponent<DropMissileCrossHairPool>();
        SetSkillData(GameDataMgr.Instance.AttackSKillDataController.AttackSkillTable[eATTACKSKILL_LIST.DropMissile]);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    protected override void Update()
    {
        if (_currentCoolTime >= _coolTime)
        {
            Attack();
        }
        _currentCoolTime += Time.deltaTime;
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
                    _totalDmg *= 2f;
                    break;
                case 3:
                    _bulletCnt = 2;
                    break;
                case 4:
                    _totalDmg *= 2f;
                    break;
                case 5:
                    _bulletCnt = 3;
                    break;

            }
        }
    }

    protected override void Attack()
    {
        Collider2D[] nearObjs = Physics2D.OverlapCircleAll(transform.position, _range, LayerMask.GetMask("Enemy", "Enemy1"));
        if (nearObjs.Length > _bulletCnt)
        {
            List<Collider2D> targetList = new List<Collider2D>();
            _currentCoolTime = 0;

            List<Collider2D> nearObjColList = nearObjs.ToList<Collider2D>();

            for (int i = 0; i < _bulletCnt; i++)
            {
                Collider2D temp = nearObjColList[Random.Range(0, nearObjColList.Count)];
                nearObjColList.Remove(temp);
                targetList.Add(temp);
            }

            StartCoroutine(Cor_DropMissileAttack(targetList));
        }
        else if (nearObjs.Length > 0)
        {
            List<Collider2D> targetList = new List<Collider2D>();
            _currentCoolTime = 0;

            targetList = nearObjs.ToList<Collider2D>();

            StartCoroutine(Cor_DropMissileAttack(targetList));
        }
    }
    
    protected override void SetSkillData(AttackSkillData skillData)
    {
        _skillData = skillData;
        Level = 1;
        _bulletCnt = 1;
        _duration = 0.5f;
        _coolTime = _skillData.CoolTime * BuffSkillController.Buff_CoolTime;
        _originCoolTime = _skillData.CoolTime;
        _totalDmg = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().GetTankAtk() * 1.5f;
        _bulletSpeed = _skillData.BulletSpeed;
    }
    public override void SetCoolTime()
    {
        _coolTime = _originCoolTime * BuffSkillController.Buff_CoolTime;
    }
    #endregion

    #region Coroutines
    private IEnumerator Cor_DropMissileAttack(List<Collider2D> targets)
    {
        WaitForSeconds wait = new WaitForSeconds(_duration);

        int index = 0;
        for (int i = 0; i < _bulletCnt; i++)
        {
            DropMissile missile = _pool.GetFromPool(0);
            missile.transform.position = targets[index].transform.position + new Vector3(0, 10, 0);
            missile.SetBullet(targets[index].transform.position , _totalDmg * BuffSkillController.Buff_Power, _bulletSpeed * BuffSkillController.Buff_BulletSpeed);
            DropMissileCrossHair crosshair = _crossHairPool.GetFromPool(0);
            crosshair.SetCrossHair(transform.position, targets[index].transform.position);
            index++;
            if (index >= targets.Count)
            {
                index = 0;
            }
            yield return wait;
        }
    }
    #endregion
}
