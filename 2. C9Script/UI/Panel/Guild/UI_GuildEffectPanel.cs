using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildEffectPanel : UI_Panel
{
    [SerializeField] private Image GradeImage;

    [SerializeField] private Button PrevButton;
    [SerializeField] private Button NextButton;
    [SerializeField] private Button CloseButton;
    [SerializeField] private GameObject BackgroundObj;

    [SerializeField] private TMP_Text GuildEffectTextItem;
    [SerializeField] private Transform GuildEffectTextRoot;

    private readonly List<TMP_Text> _guildEffectTextItems = new();

    private int _currentGrade;

    private void Start()
    {
        PrevButton.BindEvent(OnClickPrev);
        NextButton.BindEvent(OnClickNext);
        CloseButton.BindEvent(Close);
        BackgroundObj.BindEvent(Close);
        
        _currentGrade = Managers.Guild.GuildData.GuildMetaData.Grade;
        SetUI();
    }

    private void SetUI()
    {
        GradeImage.sprite = Managers.Resource.LoadGuildGradeIcon(_currentGrade);
        PrevButton.gameObject.SetActive(ChartManager.GuildBuffCharts.ContainsKey(_currentGrade - 1));
        NextButton.gameObject.SetActive(ChartManager.GuildBuffCharts.ContainsKey(_currentGrade + 1));
        SetGuildEffectTextItems();
    }

    private void OnClickPrev()
    {
        if (!ChartManager.GuildBuffCharts.ContainsKey(_currentGrade - 1))
            return;
            
        _currentGrade -= 1;
        SetUI();
    }

    private void OnClickNext()
    {
        if (!ChartManager.GuildBuffCharts.ContainsKey(_currentGrade + 1))
            return;
        
        _currentGrade += 1;
        SetUI();
    }

    private void SetGuildEffectTextItems()
    {
        if (_guildEffectTextItems.Count <= 0)
            GuildEffectTextRoot.DestroyInChildren();
        else
            _guildEffectTextItems.ForEach(item => item.gameObject.SetActive(false));

        var guildEffectTextItems = _guildEffectTextItems.ToList();

        if (!ChartManager.GuildBuffCharts.TryGetValue(_currentGrade, out var guildBuffChart))
        {
            Debug.LogError($"Fail Load GuildBuffChart : {_currentGrade}");
            return;
        }

        int index = 0;

        if (guildBuffChart.EffectStatId1 != 0)
            SetItem(guildBuffChart.EffectStatId1, guildBuffChart.EffectStatValue1);

        if (guildBuffChart.EffectStatId2 != 0)
            SetItem(guildBuffChart.EffectStatId2, guildBuffChart.EffectStatValue2);
        
        if (guildBuffChart.EffectStatId3 != 0)
            SetItem(guildBuffChart.EffectStatId3, guildBuffChart.EffectStatValue3);

        if (guildBuffChart.EffectStatId4 != 0)
            SetItem(guildBuffChart.EffectStatId4, guildBuffChart.EffectStatValue4);

        void SetItem(int statId, double statValue)
        {
            if (!ChartManager.StatCharts.TryGetValue(statId, out var statChart))
            {
                Debug.LogError($"Fail Load StatChart : {statId}");
                return;
            }

            TMP_Text textItem;

            if (guildEffectTextItems.Count > index)
                textItem = guildEffectTextItems[index++];
            else
            {
                textItem = Instantiate(GuildEffectTextItem, GuildEffectTextRoot);
                _guildEffectTextItems.Add(textItem);
            }
            
            textItem.gameObject.SetActive(true);
            textItem.text =
                statChart.ValueType == ValueType.Percent ? 
                    $"{ChartManager.GetString(statChart.Name)} : {(statValue * 100).ToCurrencyString()}%" :
                    $"{ChartManager.GetString(statChart.Name)} : {statValue.ToCurrencyString()}";
        }
    }
}