using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_DetailStatPanel : UI_Panel
{
    [SerializeField] private Transform DetailStatItemRootTr;
    [SerializeField] private Button CloseButton;

    private readonly StatType[] _showStatTypes =
    {
        StatType.Attack,
        StatType.AttackSpeedPer,
        StatType.MoveSpeedPer,
        StatType.SkillCoolTimeReduce,
        StatType.SkillDamage,
        StatType.Defence,
        StatType.Hp,
        StatType.Hp_Recovery,
        StatType.NormalMonsterDamage,
        StatType.BossMonsterDamage,
        StatType.FinalDamageIncrease,
        StatType.IncreaseGold,
        StatType.IncreaseExp,
        StatType.IncreaseFeverDuration,
        StatType.NormalAttackDamage
    };

    private readonly List<UI_DetailStatItem> _uiDetailStatItems = new();

    public void Start()
    {
        CloseButton.BindEvent(Close);
    }

    public override void Open()
    {
        base.Open();

        if (_uiDetailStatItems.Count <= 0)
        {
            DetailStatItemRootTr.DestroyInChildren();

            int i;
            
            for (i = 0; i < _showStatTypes.Length; i++)
            {
                var uiDetailStatItem = Managers.UI.MakeSubItem<UI_DetailStatItem>(DetailStatItemRootTr);
                uiDetailStatItem.Init(_showStatTypes[i]);
                _uiDetailStatItems.Add(uiDetailStatItem);
            }

            Utils.GetReinforceCriticalRate(out var prevReinforceCriticalRateType, out var currentReinforceCriticalRateType);
            
            // 이전 치명타배율
            {
                var uiDetailStatItem = Managers.UI.MakeSubItem<UI_DetailStatItem>(DetailStatItemRootTr);
                uiDetailStatItem.Init(prevReinforceCriticalRateType);
                _uiDetailStatItems.Add(uiDetailStatItem);
            }
            
            // 현재 치명타배율
            {
                var uiDetailStatItem = Managers.UI.MakeSubItem<UI_DetailStatItem>(DetailStatItemRootTr);
                uiDetailStatItem.Init(currentReinforceCriticalRateType);
                _uiDetailStatItems.Add(uiDetailStatItem);
            }
            
        }
        else
        {
            _uiDetailStatItems.ForEach(uiDetailStatItem =>
                uiDetailStatItem.gameObject.SetActive(false));

            int i;
            
            for (i = 0; i < _showStatTypes.Length; i++)
            {
                var uiDetailStatItem = _uiDetailStatItems[i];
                uiDetailStatItem.Init(_showStatTypes[i]);
                uiDetailStatItem.gameObject.SetActive(true);
            }
            
            Utils.GetReinforceCriticalRate(out var prevReinforceCriticalRateType, out var currentReinforceCriticalRateType);

            // 이전 치명타배율
            {
                var uiDetailStatItem = _uiDetailStatItems[i++];
                uiDetailStatItem.Init(prevReinforceCriticalRateType);
                uiDetailStatItem.gameObject.SetActive(true);
            }
            
            // 현재 치명타배율
            {
                var uiDetailStatItem = _uiDetailStatItems[i];
                uiDetailStatItem.Init(currentReinforceCriticalRateType);
                uiDetailStatItem.gameObject.SetActive(true);
            }
        }
    }
}