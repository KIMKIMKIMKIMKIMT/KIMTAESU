using TMPro;
using UI;
using UnityEngine;

public class UI_LabAwakeningRatePanel : UI_Panel
{
    [SerializeField] private TMP_Text NormalText;
    [SerializeField] private TMP_Text RareText;
    [SerializeField] private TMP_Text UniqueText;
    [SerializeField] private TMP_Text LegendText;
    [SerializeField] private TMP_Text LegenoText;
    [SerializeField] private TMP_Text NormalRateText;
    [SerializeField] private TMP_Text RareRateText;
    [SerializeField] private TMP_Text UniqueRateText;
    [SerializeField] private TMP_Text LegendRateText;
    [SerializeField] private TMP_Text LegenoRateText;

    [SerializeField] private TMP_Text RedPatternRateText;
    [SerializeField] private TMP_Text GreenPatternRateText;
    [SerializeField] private TMP_Text YellowPatternRateText;
    [SerializeField] private TMP_Text BluePatternRateText;

    [SerializeField] private Transform UIAwakeningStatValueItemRoot;

    [SerializeField] private UI_AwakeningStatValueItem UIAwakeningStatValueItem;

    private void Start()
    {
        SetUI();
    }

    private void SetUI()
    {
        NormalText.text = ChartManager.GetString(Grade.Normal.ToString());
        RareText.text = ChartManager.GetString(Grade.Rare.ToString());
        UniqueText.text = ChartManager.GetString(Grade.Unique.ToString());
        LegendText.text = ChartManager.GetString(Grade.Legend.ToString());
        LegenoText.text = ChartManager.GetString(Grade.Legeno.ToString());
        
        foreach (var chartData in ChartManager.LabGradeRateCharts.Values)
        {
            switch (chartData.Grade)
            {
                case Grade.Normal:
                    NormalRateText.text = (chartData.Rate * 100).ToCurrencyString()+"%";
                    break;
                case Grade.Rare:
                    RareRateText.text =(chartData.Rate * 100).ToCurrencyString()+"%";
                    break;
                case Grade.Unique:
                    UniqueRateText.text = (chartData.Rate * 100).ToCurrencyString()+"%";
                    break;
                case Grade.Legend:
                    LegendRateText.text = (chartData.Rate * 100).ToCurrencyString()+"%";
                    break;
                case Grade.Legeno:
                    LegenoRateText.text = (chartData.Rate * 100).ToCurrencyString()+"%";
                    break;
            }
        }

        foreach (var chartData in ChartManager.LabPatternCharts.Values)
        {
            switch (chartData.Id)
            {
                case 1:
                    RedPatternRateText.text = (chartData.Rate * 100).ToCurrencyString()+"%";
                    break;
                case 2:
                    GreenPatternRateText.text = (chartData.Rate * 100).ToCurrencyString()+"%";
                    break;
                case 3:
                    YellowPatternRateText.text = (chartData.Rate * 100).ToCurrencyString()+"%";
                    break;
                case 4:
                    BluePatternRateText.text = (chartData.Rate * 100).ToCurrencyString()+"%";
                    break;
            }
        }
        
        UIAwakeningStatValueItemRoot.DestroyInChildren();

        foreach (var chartData in ChartManager.LabStatCharts.Values)
        {
            var uiItem = Managers.Resource.Instantiate(UIAwakeningStatValueItem.gameObject, UIAwakeningStatValueItemRoot).GetComponent<UI_AwakeningStatValueItem>();
            uiItem.gameObject.SetActive(true);
            uiItem.Init(chartData.Grade, chartData.StatId, chartData.MinValue, chartData.MaxValue);
        }
    }
}