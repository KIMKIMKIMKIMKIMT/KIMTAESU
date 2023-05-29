using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI;
using UniRx;
using UnityEngine.UI;

public class UI_WorldWoodAwakeningSettingGradeItem : UI_Panel
{
    #region Fields
    [SerializeField] private TMP_Text _gradeTxt;
    [SerializeField] private Toggle _checkToggle;

    [HideInInspector] public Grade Grade;
    #endregion

    #region Public Methods
    public void Init(Grade grade)
    {
        Grade = grade;

        _gradeTxt.text = ChartManager.GetString(grade.ToString());
        _checkToggle.onValueChanged.RemoveAllListeners();
        _checkToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn || Managers.Game.WorldWoodAwakeningSettingData.Grade.Value == (int)grade)
                MessageBroker.Default.Publish(new WoodAwakeningSettingMessage(WoodAwakeningSettingMessageType.Grade, (int)grade));
        });

        Managers.Game.WorldWoodAwakeningSettingData.Grade.Subscribe(gradeValue =>
        {
            _checkToggle.isOn = gradeValue == (int)grade;
        });
    }

    public void SetToggle(bool isOn)
    {
        _checkToggle.isOn = isOn;
    }
    #endregion
}
