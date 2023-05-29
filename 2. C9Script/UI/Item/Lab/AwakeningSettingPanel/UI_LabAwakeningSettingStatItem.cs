using System;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_LabAwakeningSettingStatItem : UI_Panel
{
    [SerializeField] private TMP_Text StatText;

    [SerializeField] private Toggle CheckToggle;

    [HideInInspector] public int StatId;

    public void Init(int statId)
    {
        if (!ChartManager.StatCharts.TryGetValue(statId, out var statChart))
            return;

        StatId = statId;

        StatText.text = ChartManager.GetString(statChart.Name);

        CheckToggle.onValueChanged.RemoveAllListeners();
        CheckToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn || Managers.Game.LabAwakeningSettingData.StatId.Value == statId)
                MessageBroker.Default.Publish(new LabAwakeningSettingMessage(LabAwakeningSettingMessageType.Stat, statId));
        });
        
        Managers.Game.LabAwakeningSettingData.StatId.Subscribe(statIdValue =>
        {
            CheckToggle.isOn = statIdValue == statId;
        });
    }

    public void SetToggle(bool isOn)
    {
        CheckToggle.isOn = isOn;
    }
}