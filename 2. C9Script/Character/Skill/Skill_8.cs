using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Skill_8 : BaseSkill
{
    [Header("한프레임에 발사될 미사일 갯수")]
    [SerializeField] private int FireMissileCountInFrame = 3;
    
    [Header("다음 미사일 발사 주기")]
    [SerializeField] private float FireDelay = 0.15f;

    [SerializeField] private GameObject MissilePrefab;

    private List<Missile> _missiles = new();
    
    private int _missileCount;
    private int _endCount = 0;

    protected override void Init()
    {
        base.Init();

        Id = 8;
        _missileCount = (int)ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Bullet)].Value;
    }

    public override void StartSkill()
    {
        base.StartSkill();

        StartCoroutine(CoFireMissile());
    }

    public override void StopSkill()
    {
        base.StopSkill();
        
        StopAllCoroutines();
        _missiles.ForEach(missile => missile.gameObject.SetActive(false));
    }

    private IEnumerator CoFireMissile()
    {
        int fireCount = 0;
        _endCount = 0;
        List<Missile> missiles = new List<Missile>();
        
        _missiles.ForEach(missile => missile.gameObject.SetActive(false));

        int index = 0;
        
        while (true)
        {
            for (int i = 0; i < FireMissileCountInFrame; i++, fireCount++)
            {
                if (fireCount >= _missileCount)
                    break;

                Missile missile;
                
                if (_missiles.Count > index)
                    missile = _missiles[index++];
                else
                {
                    missile = Managers.Resource.Instantiate(MissilePrefab)?.GetComponent<Missile>();
                    missiles.Add(missile);
                }

                if (missile == null)
                    continue;

                missile.SetActiveCollider(false);
                missile.Shoot(Managers.Game.MainPlayer.CenterPos, _damage, _bossDamage, _criticalMultiple, _bossCriticalMultiple, _ => CheckEnd());
            }

            if (fireCount >= _missileCount)
                break;

            yield return new WaitForSeconds(FireDelay);
        }
        
        missiles.ForEach(missile => _missiles.Add(missile));
        
        yield return new WaitForSeconds(FireDelay);
        
        _missiles.ForEach(missile => missile.SetActiveCollider(true));
    }

    public void CheckEnd()
    {
        _endCount++;
        
        if (_endCount >= _missiles.Count)
            StopSkill();
    }
}