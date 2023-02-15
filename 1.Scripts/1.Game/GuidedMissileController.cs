using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissileController : AttackSkill
{
    #region Fields
    
    private GuidedMissilePool _pool;
    private GuidedMissileCrossHairPool _crossHairPool;
    private GuidedMissileFollower _guidedMissileFollower; 
    private WaitForSeconds _delay = new WaitForSeconds(0.1f);

    private int _maxBullet;
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        _pool = GetComponent<GuidedMissilePool>();
        _crossHairPool = GetComponent<GuidedMissileCrossHairPool>();
        _guidedMissileFollower = BattleMgr.Instance.GuidedFollower;
        SetSkillData(GameDataMgr.Instance.AttackSKillDataController.AttackSkillTable[eATTACKSKILL_LIST.GuidedMissile]);
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
                    _maxBullet = 75;
                    _delay = new WaitForSeconds(0.08f);
                    break;
                case 3:
                    _totalDmg *= 1.5f;
                    break;
                case 4:
                    _maxBullet = 100;
                    _delay = new WaitForSeconds(0.06f);
                    break;
                case 5:
                    _totalDmg *= 2f;
                    break;
            }
        }
    }

    protected override void Attack()
    {
        StopAllCoroutines();
        StartCoroutine(GuidedMissile(_maxBullet));
    }

    protected override void SetSkillData(AttackSkillData skillData)
    {
        Level = 1;
        _skillData = skillData;
        _coolTime = skillData.CoolTime * BuffSkillController.Buff_CoolTime;
        _originCoolTime = skillData.CoolTime;
        _totalDmg = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().GetTankAtk() * 0.6f;
        _bulletSpeed = skillData.BulletSpeed;
        _maxBullet = 50;
        _guidedMissileFollower.gameObject.SetActive(true);
        _currentCoolTime = _coolTime;
    }
    public override void SetCoolTime()
    {
        _coolTime = _originCoolTime * BuffSkillController.Buff_CoolTime;
    }
    #endregion

    #region Courutiens
    private IEnumerator GuidedMissile(int maxbullet)
    {
        int maxBullet = maxbullet + 1;
        int curBullet = 1;
        int crosshairCnt = 0;
        while (true)
        {
            float ranX = Random.Range(-0.5f, 0.5f);
            float ranY = Random.Range(-0.5f, 0.5f);
            
            Vector3 dir = new Vector3(Mathf.Cos(Mathf.PI * 2 * curBullet / maxBullet) * 3, Mathf.Sin(Mathf.PI * 2 * curBullet / maxBullet) * 3, 0);
            GuidedMissileCrossHair crossHair = _crossHairPool.GetFromPool(0);
            crossHair.transform.position = new Vector3(transform.position.x + dir.x + ranX, transform.position.y + dir.y + ranY, 0);
            crosshairCnt++;

            GuidedMissile obj = _pool.GetFromPool(0);
            obj.transform.position = _guidedMissileFollower.transform.position;
            int ran = Random.Range(0, 2);
            obj.SetBullet(crossHair.transform.position, dir, _bulletSpeed * BuffSkillController.Buff_BulletSpeed, _totalDmg * BuffSkillController.Buff_Power, ran == 0 ? -1 : 1);
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.GuidedMissile, true);
            }
            
            

            curBullet++;
            if (curBullet >= maxBullet)
            {
                yield break;
            }
            yield return _delay;
        }
    }
    #endregion
}
