using System;
using GameData;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_UnlimitedPointUpgradeItem : UI_Base
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
        var unlimitedPointUpgradeChart = ChartManager.UnlimitedPointUpgradeCharts[_statId];
        StatImage.sprite = Managers.Resource.LoadStatIcon(ChartManager.StatCharts[_statId].Icon);
        MaxLvText.text = $"(Max Lv.{unlimitedPointUpgradeChart.MaxLevel})";

        Refresh();
    }

    public override void Refresh()
    {
        var statLv = Managers.Game.UnlimitedStatLevelDatas[_statId].Value;
        
        LvText.text = $"Lv.{statLv}";
        string effectValueText = ChartManager.StatCharts[_statId].ValueType == ValueType.Percent
            ? $"{(GetStatEffectValue() * 100).ToCurrencyString()}%"
            : GetStatEffectValue().ToCurrencyString();
        EffectText.text = $"{ChartManager.GetString(ChartManager.StatCharts[_statId].Name)} {effectValueText} 증가";

        if (statLv >= ChartManager.UnlimitedPointUpgradeCharts[_statId].MaxLevel)
            PriceText.text = "Max";
        else if (statLv + Multiple > ChartManager.UnlimitedPointUpgradeCharts[_statId].MaxLevel)
            PriceText.text = "-";
        else
            PriceText.text = GetPrice(statLv).ToString();
    }

    public double GetPrice(long statLv)
    {
        double price = 0;
        long nextLv = statLv + Multiple;

        foreach (var unlimitedPointUpgradeLevelChart in ChartManager.UnlimitedPointUpgradeLevelCharts)
        {
            if (unlimitedPointUpgradeLevelChart.Value.StatId != _statId)
                continue;

            if (unlimitedPointUpgradeLevelChart.Value.UnlimitedPointIncreaseGap == 0)
            {
                price = 1;
                break;
            }

            for (; statLv < nextLv; statLv++) // 300(50), 400(20)
            {
                if (statLv > unlimitedPointUpgradeLevelChart.Value.UpgradeLevel)
                    break;
                
                price += statLv / unlimitedPointUpgradeLevelChart.Value.UnlimitedPointIncreaseGap + 1;
            }
        }

        return price;
    }

    private void OnClickReinforce()
    {
        long statLv = Managers.Game.UnlimitedStatLevelDatas[_statId].Value;
        long price = (long)GetPrice(statLv);
        
        if (!IsHaveStatPoint(price))
            return;

        if (Managers.Game.UnlimitedStatLevelDatas[_statId].Value >= ChartManager.UnlimitedPointUpgradeCharts[_statId].MaxLevel)
            return;
        
        if (Managers.Game.UnlimitedStatLevelDatas[_statId].Value + Multiple > ChartManager.UnlimitedPointUpgradeCharts[_statId].MaxLevel)
            return;
        
        UpgradeEffect.Play();

        Managers.Game.UserData.UseUnlimitedPoint += (int)price;
        Managers.Game.UnlimitedStatLevelDatas[_statId].Value += Multiple;
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.UpgradeStat, (int)QuestUpgradeStatType.StatPointReinforce, Multiple));
        
        Managers.Game.CalculateStat();

        IsSave = true;

        Refresh();
    }

    private bool IsHaveStatPoint(double price)
    {
        int unlimitedLv = Managers.Game.UserData.Level -
                          (int)ChartManager.SystemCharts[SystemData.StatPoint1_MaxLevel].Value;
        int maxLv = (int)ChartManager.SystemCharts[SystemData.UnlimitedPoint_MaxLevel].Value - (int)ChartManager.SystemCharts[SystemData.StatPoint1_MaxLevel].Value;
        
        int statPoint = Math.Min(
            (int)ChartManager.SystemCharts[SystemData.UnlimitedPoint_Level_Per_Value].Value * unlimitedLv, 
            (int)ChartManager.SystemCharts[SystemData.UnlimitedPoint_Level_Per_Value].Value * maxLv);
        return statPoint - Managers.Game.UserData.UseUnlimitedPoint >= price;
    }

    private double GetStatEffectValue()
    {
        long statLv = Managers.Game.UnlimitedStatLevelDatas[_statId].Value;
        int prevLv = 0;

        double totalStatEffectValue = 0;

        foreach (var chartData in ChartManager.UnlimitedPointUpgradeLevelCharts.Values)
        {
            if (chartData.StatId != _statId)
                continue;

            long gap = statLv - prevLv;
            totalStatEffectValue += gap * chartData.LevelIncreaseValue;

            if (statLv <= chartData.UpgradeLevel)
                break;

            prevLv = chartData.UpgradeLevel;
        }
        

        return totalStatEffectValue;
    }
}