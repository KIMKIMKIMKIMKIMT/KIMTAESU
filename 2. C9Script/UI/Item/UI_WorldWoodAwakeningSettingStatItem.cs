using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using TMPro;
using UniRx;
using UnityEngine.UI;

public class UI_WorldWoodAwakeningSettingStatItem : UI_Panel
{
    #region Fields
    [SerializeField] private TMP_Text _statTxt;
    [SerializeField] private Toggle _checkToggle;

    [HideInInspector] public int StatId;
    #endregion

    #region Public Methods
    public void Init(int statId)
    {
        if (!ChartManager.StatCharts.TryGetValue(statId, out var statChart))
            return;

        StatId = statId;

        _statTxt.text = ChartManager.GetString(statChart.Name);

        _checkToggle.onValueChanged.RemoveAllListeners();
        _checkToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn || Managers.Game.WorldWoodAwakeningSettingData.StatId.Value == statId)
                MessageBroker.Default.Publish(new WoodAwakeningSettingMessage(WoodAwakeningSettingMessageType.Stat, statId));
        });

        Managers.Game.WorldWoodAwakeningSettingData.StatId.Subscribe(statIdValue =>
        {
            _checkToggle.isOn = statIdValue == statId;
        });
    }

    public void SetToggle(bool isOn)
    {
        _checkToggle.isOn = isOn;
    }
    #endregion
}
