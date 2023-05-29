using TMPro;
using UI;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_LabAwakeningSettingPatternItem : UI_Panel
{
    [SerializeField] private Image PatternImage;

    [SerializeField] private TMP_Text PatternText;

    [SerializeField] private Toggle CheckToggle;

    [HideInInspector] public int PatternId;

    public void Init(int patternId)
    {
        if (!ChartManager.LabPatternCharts.TryGetValue(patternId, out var labPatternChart))
            return;

        PatternId = patternId;

        PatternImage.sprite = Managers.Resource.LoadLabIcon(labPatternChart.Icon);
        PatternText.text = ChartManager.GetString(labPatternChart.Name);

        CheckToggle.onValueChanged.RemoveAllListeners();
        CheckToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn || Managers.Game.LabAwakeningSettingData.PatternId.Value == patternId)
                MessageBroker.Default.Publish(new LabAwakeningSettingMessage(LabAwakeningSettingMessageType.Pattern, patternId));
        });

        Managers.Game.LabAwakeningSettingData.PatternId.Subscribe(patternIdValue =>
        {
            CheckToggle.isOn = patternIdValue == patternId;
        });
    }

    public void SetToggle(bool isOn)
    {
        CheckToggle.isOn = isOn;
    }
}