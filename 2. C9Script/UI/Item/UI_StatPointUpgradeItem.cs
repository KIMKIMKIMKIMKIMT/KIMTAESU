using System;
using GameData;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatPointUpgradeItem : UI_Base
{
    [SerializeField] private TMP_Text MaxLvText;
    [SerializeField] private TMP_Text LvText;
    [SerializeField] private TMP_Text EffectText;
    [SerializeField] private TMP_Text PriceText;

    [SerializeField] private Image StatImage;

    [SerializeField] private Button ReinforceButton;

    [SerializeField] private UpgradeEffect UpgradeEffect;
    
    private int _statId;
    private int _multiple;

    public int Multiple
    {
        get => _multiple;
        set
        {
            _multiple = value;
            SetUI();
        }
    }

    [HideInInspector] public bool IsSave;

    private void Start()
    {
        ReinforceButton.SetScrollRect(transform.parent.parent.parent.GetComponent<ScrollRect>());
        ReinforceButton.BindEvent(OnClickReinforce);
        ReinforceButton.BindEvent(OnClickReinforce, UIEvent.Pressed);

        SetUI();
    }

    private void OnEnable()
    {
        IsSave = false;
    }

    public void Init(int statId, int multiple)
    {
        _statId = statId;
        _multiple = multiple;
        SetUI();
    }

    private void SetUI()
    {
        var statPointUpgradeChart = ChartManager.StatPointUpgradeCharts[_statId];
        StatImage.sprite = Managers.Resource.LoadStatIcon(ChartManager.StatCharts[_statId].Icon);
        MaxLvText.text = $"(Max Lv.{statPointUpgradeChart.MaxLevel})";

        Refresh();
    }

    public override void Refresh()
    {
        var statLv = Managers.Game.StatLevelDatas[_statId].Value;
        
        LvText.text = $"Lv.{statLv}";
        string effectValueText = ChartManager.StatCharts[_statId].ValueType == ValueType.Percent
            ? $"{(GetStatEffectValue() * 100).ToCurrencyString()}%"
            : GetStatEffectValue().ToCurrencyString();
        EffectText.text = $"{ChartManager.GetString(ChartManager.StatCharts[_statId].Name)} {effectValueText} 증가";
        PriceText.text = statLv >= ChartManager.StatPointUpgradeCharts[_statId].MaxLevel ? "Max" : Multiple.ToString();

        if (statLv >= ChartManager.StatPointUpgradeCharts[_statId].MaxLevel)
            PriceText.text = "Max";
        else if (statLv + Multiple > ChartManager.StatPointUpgradeCharts[_statId].MaxLevel)
            PriceText.text = "-";
        else
            PriceText.text = Multiple.ToString();
    }

    private void OnClickReinforce()
    {
        if (!IsHaveStatPoint())
            return;

        if (Managers.Game.StatLevelDatas[_statId].Value >= ChartManager.StatPointUpgradeCharts[_statId].MaxLevel)
            return;
        
        if (Managers.Game.StatLevelDatas[_statId].Value + Multiple > ChartManager.StatPointUpgradeCharts[_statId].MaxLevel)
            return;
        
        UpgradeEffect.Play();

        Managers.Game.UserData.UseStatPoint += Multiple;
        Managers.Game.StatLevelDatas[_statId].Value += Multiple;
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.UpgradeStat, (int)QuestUpgradeStatType.StatPointReinforce, Multiple));
        
        Managers.Game.CalculateStat();

        IsSave = true;

        Refresh();
    }

    private bool IsHaveStatPoint()
    {
        int statPoint = Math.Min(
            (int)ChartManager.SystemCharts[SystemData.LevelUpStatPoint].Value * Managers.Game.UserData.Level, 
            (int)ChartManager.SystemCharts[SystemData.LevelUpStatPoint].Value * (int)ChartManager.SystemCharts[SystemData.StatPoint1_MaxLevel].Value);
        return statPoint - Managers.Game.UserData.UseStatPoint >= Multiple;
    }

    private double GetStatEffectValue()
    {
        long statLv = Managers.Game.StatLevelDatas[_statId].Value;
        int prevLv = 0;

        float totalStatEffectValue = 0;

        foreach (var chartData in ChartManager.StatPointUpgradeLevelCharts.Values)
        {
            if (chartData.StatId != _statId)
                continue;

            long gap = statLv - prevLv;
            totalStatEffectValue += gap * chartData.IncreaseValue;

            if (statLv <= chartData.UpgradeLevel)
                break;

            prevLv = chartData.UpgradeLevel;
        }
        

        return totalStatEffectValue;
    }
}