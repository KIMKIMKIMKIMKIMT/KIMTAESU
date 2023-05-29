using System;
using Chart;
using TMPro;
using UnityEngine;

public class UI_DetailStatItem : UI_Base
{
    [SerializeField] private TMP_Text StatText;
    [SerializeField] private TMP_Text StatValueText;

    private StatType? _statType;

    public void Init(StatType? statType)
    {
        _statType = statType;
        SetUI();
    }

    private void SetUI()
    {
        if (_statType.HasValue)
        {
            StatText.text = ChartManager.GetString(ChartManager.StatCharts[(int)_statType.Value].Name);
            
            var statValue = Managers.Game.BaseStatDatas[(int)_statType];

            switch (_statType)
            {
                case StatType.Attack:
                    statValue *= Managers.Game.BaseStatDatas[(int)StatType.AttackPer];
                    break;
                case StatType.Hp:
                    statValue *= Managers.Game.BaseStatDatas[(int)StatType.HpPer];
                    break;
                case StatType.Defence:
                    statValue *= Managers.Game.BaseStatDatas[(int)StatType.DefencePer];
                    break;
            }

            StatChart statChart = ChartManager.StatCharts[(int)_statType];

            string statValueString = 
                statChart.ValueType == ValueType.Percent ? 
                    $"{(Math.Round(statValue, 7) * 100d).ToCurrencyString()}%" :
                    $"{Math.Round(statValue, 7).ToCurrencyString()}";

            StatValueText.text = statValueString;
        }
        else
        {
            StatText.text = "-";
            StatValueText.text = "-";
        }
    }
}