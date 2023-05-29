using System.Collections;
using UnityEngine;

// 간본앵
public class Skill_10 : BaseSkill
{
    [Header("항아리 순서대로 소환되는 시간")] [SerializeField]
    private float SpawnDelayTime = 0.15f;

    [Header("전부 소환후 잠시 대기되는 시간")] [SerializeField]
    private float BoomDelayTime = 0.3f;

    [SerializeField] private Skill_10_Object[] LeftObjs;
    [SerializeField] private Skill_10_Object[] RightObjs;
    [SerializeField] private GameObject BgObj;
    [SerializeField] private GameObject BgObj2;

    private Camera _gameCamera;
    private float _duration;
    private int _hitCount;
    private float _hitDelay;

    protected override void Init()
    {
        base.Init();

        Id = 10;

        _gameCamera = Camera.main;

        _duration = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Skill_Duration)].Value;
        _hitCount = (int)ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Count)].Value;

        _hitDelay = _hitCount == 0 ? 0 : _duration / _hitCount;
    }

    public override void StartSkill()
    {
        base.StartSkill();

        transform.localPosition = Vector3.zero;

        foreach (var obj in LeftObjs)
            obj.gameObject.SetActive(false);

        foreach (var obj in RightObjs)
            obj.gameObject.SetActive(false);

        BgObj.SetActive(false);
        BgObj2.SetActive(false);

        StartCoroutine(CoSpawn());
    }

    private IEnumerator CoSpawn()
    {
        int length = LeftObjs.Length;

        var spawnDelay = new WaitForSeconds(SpawnDelayTime);

        for (int i = 0; i < length; i++)
        {
            LeftObjs[i].gameObject.SetActive(true);
            RightObjs[i].gameObject.SetActive(true);

            yield return spawnDelay;
        }

        yield return new WaitForSeconds(BoomDelayTime);

        for (int i = 0; i < length; i++)
        {
            LeftObjs[i].Boom();
            RightObjs[i].Boom();
        }

        yield return new WaitForSeconds(0.05f);

        BgObj.SetActive(true); 
        BgObj2.SetActive(true);
        
        StartCoroutine(CoDamage());

        yield return new WaitForSeconds(_duration);

        BgObj.SetActive(false);
        BgObj2.SetActive(false);
        StopSkill();
    }

    public override void StopSkill()
    {
        base.StopSkill();
        int length = LeftObjs.Length;
        for (int i = 0; i < length; i++)
        {
            LeftObjs[i].gameObject.SetActive(false);
            RightObjs[i].gameObject.SetActive(false);
        }
        
        BgObj.SetActive(false);
        BgObj2.SetActive(false);
    }

    private IEnumerator CoDamage()
    {
        var hitDelay = new WaitForSeconds(_hitDelay);

        while (true)
        {
            Managers.Monster.FindMonstersInGameView().ForEach(monster =>
            {
                double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                monster.Damage(damage, criticalMultiple);
            });

            yield return hitDelay;
        }
    }
}