using System;
using System.Collections.Generic;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_LabAwakeningSettingPanel : UI_Panel
{
    [SerializeField] private UI_LabAwakeningSettingPatternItem UILabAwakeningSettingPatternItem;
    [SerializeField] private UI_LabAwakeningSettingStatItem UILabAwakeningSettingStatItem;
    [SerializeField] private UI_LabAwakeningSettingGradeItem UILabAwakeningSettingGradeItem;

    [SerializeField] private Transform PatternItemRoot;
    [SerializeField] private Transform StatItemRoot;
    [SerializeField] private Transform GradeItemRoot;

    [SerializeField] private Button CloseButton;

    private readonly List<UI_LabAwakeningSettingPatternItem> _patternItems = new();
    private readonly List<UI_LabAwakeningSettingStatItem> _statItems = new();
    private readonly List<UI_LabAwakeningSettingGradeItem> _gradeItems = new();

    private void Start()
    {
        CloseButton.BindEvent(Close);

        SetPatternItems();
        SetStatItems();
        SetGradeItems();

        MessageBroker.Default.Receive<LabAwakeningSettingMessage>().Subscribe(message =>
        {
            switch (message.Type)
            {
                case LabAwakeningSettingMessageType.Grade:
                {
                    // _gradeItems.ForEach(item =>
                    // {
                    //     if ((int)item.Grade == message.Value)
                    //         return;
                    //
                    //     item.SetToggle(false);
                    // });

                    if (Managers.Game.LabAwakeningSettingData.Grade.Value == message.Value)
                        Managers.Game.LabAwakeningSettingData.Grade.Value = -1;
                    else
                        Managers.Game.LabAwakeningSettingData.Grade.Value = message.Value;
                }
                    break;
                case LabAwakeningSettingMessageType.Pattern:
                {
                    // _patternItems.ForEach(item =>
                    // {
                    //     if (item.PatternId == message.Value)
                    //         return;
                    //
                    //     item.SetToggle(false);
                    // });

                    if (Managers.Game.LabAwakeningSettingData.PatternId.Value == message.Value)
                        Managers.Game.LabAwakeningSettingData.PatternId.Value = -1;
                    else
                        Managers.Game.LabAwakeningSettingData.PatternId.Value = message.Value;
                }
                    break;
                case LabAwakeningSettingMessageType.Stat:
                {
                    // _statItems.ForEach(item =>
                    // {
                    //     if (item.StatId == message.Value)
                    //         return;
                    //
                    //     item.SetToggle(false);
                    // });

                    if (Managers.Game.LabAwakeningSettingData.StatId.Value == message.Value)
                        Managers.Game.LabAwakeningSettingData.StatId.Value = -1;
                    else
                        Managers.Game.LabAwakeningSettingData.StatId.Value = message.Value;
                }
                    break;
            }
        });
    }

    private void OnDisable()
    {
        Managers.Game.LabAwakeningSettingData.Save();
    }

    private void SetPatternItems()
    {
        PatternItemRoot.DestroyInChildren();

        foreach (var chartData in ChartManager.LabPatternCharts.Values)
        {
            var obj = Managers.Resource.Instantiate(UILabAwakeningSettingPatternItem.gameObject, PatternItemRoot);
            if (obj == null)
                continue;

            var uiLabAwakeningSettingPatternItem = obj.GetComponent<UI_LabAwakeningSettingPatternItem>();
            if (uiLabAwakeningSettingPatternItem == null)
                continue;

            uiLabAwakeningSettingPatternItem.gameObject.SetActive(true);
            uiLabAwakeningSettingPatternItem.Init(chartData.Id);

            _patternItems.Add(uiLabAwakeningSettingPatternItem);
        }
    }

    private void SetStatItems()
    {
        StatItemRoot.DestroyInChildren();

        foreach (var chartData in ChartManager.LabStatRateCharts.Values)
        {
            var obj = Managers.Resource.Instantiate(UILabAwakeningSettingStatItem.gameObject, StatItemRoot);
            if (obj == null)
                continue;

            var uiLabAwakeningSettingStatItem = obj.GetComponent<UI_LabAwakeningSettingStatItem>();
            if (uiLabAwakeningSettingStatItem == null)
                continue;

            uiLabAwakeningSettingStatItem.gameObject.SetActive(true);
            uiLabAwakeningSettingStatItem.Init(chartData.StatId);

            _statItems.Add(uiLabAwakeningSettingStatItem);
        }
    }

    private void SetGradeItems()
    {
        GradeItemRoot.DestroyInChildren();

        foreach (var chartData in ChartManager.LabGradeRateCharts.Values)
        {
            var obj = Managers.Resource.Instantiate(UILabAwakeningSettingGradeItem.gameObject, GradeItemRoot);
            if (obj == null)
                continue;

            var uiLabAwakeningSettingGradeItem = obj.GetComponent<UI_LabAwakeningSettingGradeItem>();
            if (uiLabAwakeningSettingGradeItem == null)
                continue;

            uiLabAwakeningSettingGradeItem.gameObject.SetActive(true);
            uiLabAwakeningSettingGradeItem.Init(chartData.Grade);

            _gradeItems.Add(uiLabAwakeningSettingGradeItem);
        }
    }
}