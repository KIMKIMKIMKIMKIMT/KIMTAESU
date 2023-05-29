using System;
using Chart;
using GameData;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_CollectionItem : UI_Base
{
    [SerializeField] private TMP_Text CollectionNameText;
    [SerializeField] private TMP_Text MaxLvText;
    [SerializeField] private TMP_Text EffectText;
    [SerializeField] private TMP_Text EffectValueText;
    [SerializeField] private TMP_Text LvText;
    [SerializeField] private TMP_Text QuantityText;

    [SerializeField] private Image IconImage;

    [SerializeField] private Button ReinforceButton;

    [SerializeField] private UpgradeEffect UpgradeEffect;

    private CollectionChart _collectionChart;
    private CollectionData _collectionData;

    private int LvUpPrice => 
        _collectionChart.UpgradeValue + _collectionChart.UpgradeValue * (_collectionData.Lv / _collectionChart.UpgradeIncreaseGap);

    public bool IsSave;

    private void Start()
    {
        ReinforceButton.SetScrollRect(transform.parent.parent.parent.GetComponent<ScrollRect>());
        ReinforceButton.BindEvent(OnClickReinforce);
        ReinforceButton.BindEvent(OnClickReinforce, UIEvent.Pressed);
    }

    private void OnEnable()
    {
        IsSave = false;
    }

    public void Init(CollectionChart collectionChart)
    {
        _collectionChart = collectionChart;
        _collectionData = Managers.Game.CollectionDatas[collectionChart.CollectionId];
        SetUI();
    }

    private void SetUI()
    {
        IconImage.sprite = Managers.Resource.LoadCollectionIcon(_collectionChart.Icon);
        CollectionNameText.text = ChartManager.GetString(_collectionChart.CollectionName);
        MaxLvText.text = $"(Max Lv.{_collectionChart.MaxLevel.ToString()})";

        Refresh();
    }

    public override void Refresh()
    {
        var statChart = ChartManager.StatCharts[_collectionChart.StatType];
        EffectText.text = ChartManager.GetString(statChart.Name);

        var effectValue = _collectionChart.StatIncreaseValue * _collectionData.Lv;
        effectValue = Math.Round(effectValue, 7);
        EffectValueText.text = statChart.ValueType == ValueType.Percent ? $"{(effectValue * 100).ToCurrencyString()}%" : effectValue.ToCurrencyString();
        LvText.text = $"Lv.{_collectionData.Lv}";
        ReinforceButton.gameObject.SetActive(_collectionData.Lv < _collectionChart.MaxLevel);
        QuantityText.text = $"( {_collectionData.Quantity} / {LvUpPrice} )";
        QuantityText.color = _collectionData.Quantity >= LvUpPrice ? Color.white : Color.red;
    }

    private void OnClickReinforce()
    {
        if (_collectionData.Quantity < LvUpPrice)
            return;

        if (_collectionData.Lv >= _collectionChart.MaxLevel)
            return;

        _collectionData.Quantity -= LvUpPrice;
        _collectionData.Lv++;
        
        UpgradeEffect.Play();
        IsSave = true;
        Refresh();
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.UpgradeStat, 3, 1));        
        Managers.Game.CalculateStat();
    }
}