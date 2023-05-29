using System;
using System.Collections.Generic;
using GameData;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_AttendancePopup : UI_Popup
{
    [SerializeField] private List<UI_AttendanceItem> UIAttendanceItems;

    [SerializeField] private Button CloseButton;

    public override bool isTop => true;

    private void OnEnable()
    {
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.OpenMenu, (int)QuestOpenMenu.Attendance, 1));
    }

    private void Start()
    {
        CloseButton.BindEvent(ClosePopup);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI();
        }
    }
    
    public override void Open()
    {
        base.Open();
        
        Managers.Game.CheckAttendance();
        SetAttendanceItem();
    }

    public void SetAttendanceItem()
    {
        foreach (var chartData in ChartManager.AttendanceCharts)
        {
            UIAttendanceItems[chartData.Key - 1].Init(chartData.Key, chartData.Key);
        }
    }
}