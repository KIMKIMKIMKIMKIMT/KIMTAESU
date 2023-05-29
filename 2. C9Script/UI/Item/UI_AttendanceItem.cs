using System.Collections.Generic;
using AppsFlyerSDK;
using Chart;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AttendanceItem : UI_Base
{
    [SerializeField] private TMP_Text DayText;
    [SerializeField] private TMP_Text RewardValueText;

    [SerializeField] private Image RewardImage;

    [SerializeField] private Button ReceiveButton;

    [SerializeField] private GameObject RedDotObj;
    [SerializeField] private GameObject ClearObj;

    private int _day;
    private AttendanceChart _attendanceChart;

    private void Start()
    {
        ReceiveButton.BindEvent(OnClickReceive);
    }

    public void Init(int day, int id)
    {
        _day = day;
        _attendanceChart = ChartManager.AttendanceCharts[id];

        SetUI();
    }

    private void SetUI()
    {
        DayText.text = $"D-{_day}";

        RewardImage.sprite = Managers.Resource.LoadItemIcon(_attendanceChart.RewardType, _attendanceChart.RewardId);
        RewardValueText.text = _attendanceChart.RewardValue.ToCurrencyString();

        // 이미 수령한 보상
        if (Managers.Game.UserData.AttendanceIndex > _attendanceChart.Id)
        {
            ClearObj.SetActive(true);
            RedDotObj.SetActive(false);
        }
        else if (Managers.Game.UserData.AttendanceIndex == _attendanceChart.Id)
        {
            ClearObj.SetActive(false);
            RedDotObj.SetActive(Utils.GetNow() >= Managers.Game.UserData.AttendanceDate);
        }
        // 아직 수령하지 못한 보상
        else
        {
            ClearObj.SetActive(false);
            RedDotObj.SetActive(false);
        }
    }

    private void OnClickReceive()
    {
        if (Utils.GetNow() < Managers.Game.UserData.AttendanceDate)
            return;

        if (Managers.Game.UserData.AttendanceIndex != _attendanceChart.Id)
            return;

        Managers.Game.UserData.AttendanceDate = Utils.GetDay(1);
        Managers.Game.UserData.AttendanceIndex++;
        Managers.Game.IncreaseItem(_attendanceChart.RewardType, _attendanceChart.RewardId,
            _attendanceChart.RewardValue);

        GameDataManager.UserGameData.SaveGameData();
        GameDataManager.SaveItemData(_attendanceChart.RewardType);
        InAppActivity.SendEvent("attendance");

        var gainItemDatas = new Dictionary<(ItemType, int), double>
        {
            [(_attendanceChart.RewardType, _attendanceChart.RewardId)] = _attendanceChart.RewardValue
        };

        Managers.UI.ShowGainItems(gainItemDatas);

        SetUI();
    }
}