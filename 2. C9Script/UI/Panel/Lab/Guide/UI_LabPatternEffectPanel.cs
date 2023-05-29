using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_LabPatternEffectPanel : UI_Panel
{
    [Serializable]
    public record UIPatternItem
    {
        public Image PatternImage;
        public TMP_Text PatternNameText;
        public Transform EffectValueRoot;
    }

    [SerializeField] private UIPatternItem[] UIPatternItems;

    [SerializeField] private GameObject EffectValuePrefab;

    private void Start()
    {
        SetUI();
    }

    private void SetUI()
    {
        // Key - PatternId
        // Value - SetNumber, StatType, StatValue
        Dictionary<int, List<(int, int, double)>> setDatas =new();

        foreach (var chartData in ChartManager.LabSetCharts.Values)
        {
            if (!setDatas.ContainsKey(chartData.SetPattern))
                setDatas.Add(chartData.SetPattern, new List<(int, int, double)>());
            
            setDatas[chartData.SetPattern].Add((chartData.SetNumber, chartData.StatType, chartData.StatValue));
        }

        for (var i = 0; i < UIPatternItems.Length; i++)
        {
            var patternId = i + 1;

            if (!ChartManager.LabPatternCharts.TryGetValue(patternId, out var labPatternChart))
                continue;

            UIPatternItems[i].PatternImage.sprite = Managers.Resource.LoadLabIcon(labPatternChart.Icon);
            UIPatternItems[i].PatternNameText.text = ChartManager.GetString(labPatternChart.Name);
            UIPatternItems[i].EffectValueRoot.DestroyInChildren();

            if (!setDatas.ContainsKey(patternId)) 
                continue;
            
            UIPatternItems[i].EffectValueRoot.DestroyInChildren();
            
            foreach (var (setNumber, statId, statValue) in setDatas[patternId])
            {
                if (!ChartManager.StatCharts.TryGetValue(statId, out var statChart))
                    continue;

                var effectValueText = Managers.Resource.Instantiate(EffectValuePrefab, UIPatternItems[i].EffectValueRoot).GetComponent<TMP_Text>();
                
                effectValueText.gameObject.SetActive(true);
                if (effectValueText == null)
                    continue;

                effectValueText.text = 
                    statChart.ValueType == ValueType.Percent ?
                        $"{setNumber} : {ChartManager.GetString(statChart.Name)} {(statValue * 100).ToCurrencyString()}%" : 
                        $"{setNumber} : {ChartManager.GetString(statChart.Name)} {statValue.ToCurrencyString()}";
                
                //if (Managers.Game.LabAwakeningDatas.Values.ToList().fin)
            }
        }
    }
}