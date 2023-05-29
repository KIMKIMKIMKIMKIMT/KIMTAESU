using System.Collections;
using UniRx.Triggers;
using UnityEngine;
using Random = UnityEngine.Random;

// 기모띠
public class Skill_12 : BaseSkill
{
    [Header("발사될 최소 거리")] 
    [SerializeField] private float FireMinDistance = 1f;
    [Header("발사될 최대 거리")] 
    [SerializeField] private float FireMaxDistance = 3f;

    [Header("발사 속도")] 
    [SerializeField] private float Speed = 3f;
    
    [Header("각 미사일 발사 주기 속도(초)")] 
    [SerializeField] private float FireDelay = 0.3f;

    [SerializeField] private Skill_12_Missile[] Missiles;

    private float _hitRange;

    protected override void Init()
    {
        base.Init();

        Id = 12;

        _hitRange = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Range)].Value;
    }
    
    public override void StartSkill()
    {
        base.StartSkill();
        transform.position = Managers.Game.MainPlayer.CenterPos;
        StartCoroutine(CoStart());
    }

    public override void StopSkill()
    {
        base.StopSkill();
        
        StopAllCoroutines();
        foreach (var missile in Missiles)
            missile.gameObject.SetActive(false);
    }

    private IEnumerator CoStart()
    {
        var fireDelay = new WaitForSeconds(FireDelay);
        
        foreach (var missile in Missiles)
        {
            missile.gameObject.SetActive(true);
            missile.transform.position = transform.position;
        }

        for (int i = 0; i < Missiles.Length; i++)
        {
            yield return fireDelay;
            
            float distance = Random.Range(FireMinDistance, FireMaxDistance);
            Vector3 direction =
                AngleToDirection(Random.Range(i * 90f, (i + 1) * 90f));
            var destination =  Missiles[i].transform.position + (direction * distance);
            
            Missiles[i].Fire(_damage, _bossDamage,  _criticalMultiple, _bossCriticalMultiple, Speed, _hitRange, destination);
        }

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            int offCount = 0;

            foreach (var missile in Missiles)
            {
                if (!missile.gameObject.activeSelf)
                    offCount += 1;
            }
            
            if (offCount >= Missiles.Length)
                StopSkill();
        }
    }

    private Vector3 AngleToDirection(float angle)
    {
        Vector3 direction = transform.up;

        return Quaternion.AngleAxis(angle, Vector3.forward) * direction;
        // var quaternion = Quaternion.Euler(0, angle, 0);
        // Vector3 newDirection = quaternion * direction;
        //
        // return newDirection;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, FireMinDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, FireMaxDistance);
    }
}