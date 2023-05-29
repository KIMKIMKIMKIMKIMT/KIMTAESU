using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatEffectItem : UI_Base
{
    [SerializeField] private TMP_Text StatText;
    [SerializeField] private TMP_Text EffectValueText;

    [SerializeField] private Image StatImage;

    public void Init(int statId, double statValue)
    {
        if (statId == 0 || !ChartManager.StatCharts.TryGetValue(statId, out var statChart))
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);

        StatImage.sprite = Managers.Resource.LoadStatIcon(statChart.Icon);
        StatText.text = ChartManager.GetString(statChart.Name);
        EffectValueText.text = statChart.ValueType == ValueType.Percent
            ? $"{Math.Round(statValue * 100, 7).ToCurrencyString()}%"
            : Math.Round(statValue, 7).ToCurrencyString();
    }
}