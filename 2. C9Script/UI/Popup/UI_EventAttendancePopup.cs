using System;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_EventAttendancePopup : UI_Popup
{
    [SerializeField] private GameObject[] CoverObjs;

    [SerializeField] private GameObject BackgroundObj;

    [SerializeField] private Button GetRewardButton;
    [SerializeField] private Button CloseButton;

    public override bool isTop => true;

    private void Start()
    {
        BackgroundObj.BindEvent(ClosePopup);
        CloseButton.BindEvent(ClosePopup);
        GetRewardButton.BindEvent(OnClickGetReward);
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
        
        SetUI();
    }

    private void SetUI()
    {
        for (var i = 0; i < CoverObjs.Length; i++)
            CoverObjs[i].SetActive(Managers.Game.UserData.EventAttendanceIndex > i + 1);
    }

    private void OnClickGetReward()
    {
        if (Managers.Game.UserData.EventAttendanceIndex > GetMaxEventAttendanceId() ||
            !ChartManager.EventAttendanceCharts.ContainsKey(Managers.Game.UserData.EventAttendanceIndex))
        {
            Managers.Message.ShowMessage("이미 모든 보상을 수령 했습니다.");
            return;
        }

        if (Managers.Game.UserData.EventAttendanceTime > Utils.GetNow())
        {
            Managers.Message.ShowMessage("금일 보상을 이미 수령 했습니다.");
            return;
        }

        if (!ChartManager.EventAttendanceCharts.TryGetValue(Managers.Game.UserData.EventAttendanceIndex,
                out var eventAttendanceChart))
        {
            Managers.Message.ShowMessage("데이터가 존재하지 않습니다.");
            return;
        }

        for (var i = 0; i < eventAttendanceChart.RewardIds.Length; i++)
            Managers.Game.IncreaseItem(ItemType.Goods, eventAttendanceChart.RewardIds[i], eventAttendanceChart.RewardValues[i]);

        Managers.Game.UserData.EventAttendanceIndex += 1;
        Managers.Game.UserData.EventAttendanceTime = Utils.GetDay(1);
        
        GameDataManager.GoodsGameData.SaveGameData();
        GameDataManager.UserGameData.SaveGameData();
        
        Managers.Message.ShowMessage("수령 완료");

        SetUI();
    }

    private int GetMaxEventAttendanceId()
    {
        return ChartManager.EventAttendanceCharts.Max(data => data.Key);
    }
}