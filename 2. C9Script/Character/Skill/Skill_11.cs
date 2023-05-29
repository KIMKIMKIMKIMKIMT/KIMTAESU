using System.Collections;
using DG.Tweening;
using UnityEngine;

// ㅇㅈ 도장 스킬
public class Skill_11 : BaseSkill
{
    [Header("스탬프 소환후 올라가는 시간(초)")] 
    [SerializeField] private float SpawnUpMoveTime = 1f;

    [Header("스탬프 소환후 올라갈 높이(현재 높이에서 얼마나 올라갈지)")]
    [SerializeField] private float UpHeight = 3f;
    
    [Header("스탬프 이동 속도")]
    [SerializeField] private float MoveSpeed = 1f;

    [Header("스탬프 발사 딜레이")]
    [SerializeField] private float Delay = 1.5f;

    [Header("스탬프 공격후 다음 공격 대기시간")]
    [SerializeField] private float AttackDelay = 1f;

    [Header("스탬프 찍는 시간(초)")] 
    [SerializeField] private float DownTime = 0.5f;

    [Header("스탬프 올라오는 시간(초)")] 
    [SerializeField] private float UpTime = 1f;

    [Header("ㅇㅈ 스탬프 시작위치")] 
    [SerializeField] private Vector3 Stamp1StartPos = new Vector3(-1, 2);
    
    [Header("ㅇㅈ 스탬프 시작 기운 각도")]
    [SerializeField] private Quaternion Stamp1StartAngle = new Quaternion(0, 0, 30f, 0);

    [Header("ㅇ ㅇㅈ 스탬프 시작위치")] 
    [SerializeField] private Vector3 Stamp2StartPos = new Vector3(1, 2);

    [Header("ㅇ ㅇㅈ 스탬프 시작 기운 각도")]
    [SerializeField] private Quaternion Stamp2StartAngle = new Quaternion(0, 0, -30f, 0);
    
    [SerializeField] private Skill_11_Stamp[] Stamps;

    private int _hitCount;

    protected override void Init()
    {
        base.Init();

        Id = 11;

        _hitCount = (int)ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Count)].Value;
    }

    public override void StartSkill()
    {
        base.StartSkill();

        transform.position = Managers.Game.MainPlayer.CenterPos;
        
        Stamps[0].transform.localPosition = Stamp1StartPos;
        Stamps[1].transform.localPosition = Stamp2StartPos;

        Stamps[0].transform.rotation = Stamp1StartAngle;
        Stamps[1].transform.rotation = Stamp2StartAngle;

        foreach (var stamp in Stamps)
        {
            stamp.gameObject.SetActive(true);
            var pos = stamp.transform.localPosition;
            stamp.transform.DOLocalMove(new Vector3(pos.x, pos.y + UpHeight, pos.z), SpawnUpMoveTime).onComplete +=
                () =>
                {
                    stamp.transform.DORotate(Vector3.zero, 0.5f).onComplete += () =>
                    {
                        if (gameObject.activeSelf)
                            StartCoroutine(CoStartStamp());
                    };
                };
        }
    }

    public override void StopSkill()
    {
        base.StopSkill();
        StopAllCoroutines();
        foreach (var stamp in Stamps)
            stamp.gameObject.SetActive(false);
    }

    private IEnumerator CoStartStamp()
    {
        foreach (var stamp in Stamps)
        {
            stamp.StartStamp(MoveSpeed, DownTime, UpTime, AttackDelay, _hitCount, _damage, _bossDamage, _criticalMultiple, _bossCriticalMultiple);
            yield return new WaitForSecondsRealtime(Delay);
        }

        StartCoroutine(CoCheckAttackCount());
    }

    private IEnumerator CoCheckAttackCount()
    {
        int checkAttackCount = _hitCount * Stamps.Length;
        
        while (true)
        {
            int totalAttackCount = 0;

            foreach (var stamp in Stamps)
            {
                totalAttackCount += stamp.AttackCount;
            }

            if (totalAttackCount >= checkAttackCount)
            {
                StopSkill();
                yield break;
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}