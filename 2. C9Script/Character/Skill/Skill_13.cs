
// 그랜절

using System;
using System.Collections;
using UnityEngine;

public class Skill_13 : BaseSkill
    {
        enum AnimationType
        {
            Ready,
            Rotation,
        }

        [SerializeField] private Animator[] SkillAnimators;

        private int _hitCount;
        private float _skillDuration;
        private float _hitRange;
        private float _hitDelay;

        protected override void Init()
        {
            base.Init();

            Id = 13;

            _hitCount = (int)ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Count)].Value;
            _skillDuration = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Skill_Duration)].Value;
            _hitRange = ChartManager.SkillPolicyCharts[(Id, SkillPolicyProperty.Hit_Range)].Value;
            _hitDelay = _skillDuration / _hitCount;
            
            transform.SetParent(Managers.Game.MainPlayer.transform);
            transform.localPosition = Vector3.zero;
        }

        public override void StartSkill()
        {
            base.StartSkill();

            SkillAnimators[(int)AnimationType.Ready].gameObject.SetActive(true);
        }

        public override void StopSkill()
        {
            base.StopSkill();

            foreach (var animator in SkillAnimators)
            {
                animator.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (SkillAnimators[(int)AnimationType.Ready].gameObject.activeSelf)
            {
                if (SkillAnimators[(int)AnimationType.Ready].IsEndCurrentAnimation())
                {
                    SkillAnimators[(int)AnimationType.Ready].gameObject.SetActive(false);
                    SkillAnimators[(int)AnimationType.Rotation].gameObject.SetActive(true);

                    StartCoroutine(CoDamage());
                    StartCoroutine(CoSkillDurationTimer());
                }
            }
        }

        private IEnumerator CoDamage()
        {
            var hitDelay = new WaitForSeconds(_hitDelay);
            
            while (true)
            {
                Managers.Monster.FindTargetMonsters(Managers.Game.MainPlayer.CenterPos, _hitRange).ForEach(monster =>
                {
                    double damage = monster.Type == MonsterType.Boss ? _bossDamage : _damage;
                    double criticalMultiple = monster.Type == MonsterType.Boss ? _bossCriticalMultiple : _criticalMultiple;
                    monster.Damage(damage, criticalMultiple);
                });
                
                yield return hitDelay;
            }
        }

        private IEnumerator CoSkillDurationTimer()
        {
            yield return new WaitForSeconds(_skillDuration);
            
            StopSkill();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(Managers.Game.MainPlayer.CenterPos, _hitRange);
        }
    }
