using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_LabAwakeningSettingGradeItem : UI_Panel
{
    [SerializeField] private TMP_Text GradeText;

    [SerializeField] private Toggle CheckToggle;

    [HideInInspector] public Grade Grade;

    public void Init(Grade grade)
    {
        Grade = grade;

        GradeText.text = ChartManager.GetString(grade.ToString());
        CheckToggle.onValueChanged.RemoveAllListeners();
        CheckToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn || Managers.Game.LabAwakeningSettingData.Grade.Value == (int)grade)
                MessageBroker.Default.Publish(new LabAwakeningSettingMessage(LabAwakeningSettingMessageType.Grade, (int)grade));
        });

        Managers.Game.LabAwakeningSettingData.Grade.Subscribe(gradeValue =>
        {
            CheckToggle.isOn = gradeValue == (int)grade;
        });
    }

    public void SetToggle(bool isOn)
    {
        CheckToggle.isOn = isOn;
    }
}