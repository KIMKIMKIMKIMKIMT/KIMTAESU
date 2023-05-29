using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AwakeningStatValueItem : UI_Base
{
    [SerializeField] private TMP_Text GradeText;
    [SerializeField] private TMP_Text ValueText;
    [SerializeField] private TMP_Text StatText;

    [SerializeField] private Image StatImage;

    public void Init(Grade grade, int statId, double minValue, double maxValue)
    {
        GradeText.text = ChartManager.GetString(grade.ToString());

        StatText.text = ChartManager.StatCharts.TryGetValue(statId, out var statChart)
            ? ChartManager.GetString(statChart.Name)
            : string.Empty;

        StatImage.sprite = statChart != null ? Managers.Resource.LoadStatIcon(statChart.Icon) : null;

        if (statChart.ValueType == ValueType.Value)
            ValueText.text = $"{minValue.ToCurrencyString()}~{maxValue.ToCurrencyString()}%";
        else
            ValueText.text = $"{(minValue * 100).ToCurrencyString()}%~{(maxValue * 100).ToCurrencyString()}%";
    }
}