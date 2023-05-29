using System;
using System.Collections.Generic;
using System.Linq;
using Chart;
using GameData;
using UniRx;
using UnityEditor;
using UnityEngine;
using Util;

public abstract class BaseSkill : MonoBehaviour
{
    public int Id { get; set; }

    private bool _isInit;
    protected double _damage;
    protected double _bossDamage;
    protected double _criticalMultiple;
    protected double _bossCriticalMultiple;
    protected EffectSound _effectSound;

    private List<SpriteRenderer> _spriteRenderers;
    private List<Animator> _animators;

    private SkillChart _skillChart;
    

    protected virtual void Init()
    {
    }

    public virtual void StartSkill()
    {
        if (!_isInit)
        {
            _spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>(true).ToList();
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                _spriteRenderers.Add(sr);

            Managers.Game.SettingData.SkillEffect.Subscribe(value =>
            {
                if (_spriteRenderers == null)
                    return;
                
                _spriteRenderers.ForEach(spriteRenderer =>
                {
                    if (spriteRenderer == null)
                        return;
                    
                    spriteRenderer.enabled = Managers.Game.SettingData.SkillEffect.Value;
                });
            });

            ChartManager.SkillCharts.TryGetValue(Id, out _skillChart);
            
            Init();
            _isInit = true;
        }
        
        gameObject.SetActive(true);
        Managers.Game.MainPlayer.UsingSkillIds.Add(Id);
        _damage = Utils.CalculateSkillDamage(Id, out _criticalMultiple, MonsterType.Normal);
        _bossDamage = Utils.CalculateSkillDamage(Id, out _bossCriticalMultiple, MonsterType.Boss);
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.UseSkill, Id, 1));

        if (Id != 2)
            _effectSound = Managers.Sound.PlaySkillSound(Id);
    }

    public virtual void StopSkill()
    {
        if (Managers.Game.MainPlayer.UsingSkillIds.Contains(Id))
            Managers.Game.MainPlayer.UsingSkillIds.Remove(Id);
        
        if (_effectSound != null && _effectSound.Loop)
            _effectSound.Stop();

        gameObject.SetActive(false);
    }
}