using System;
using Chart;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_DungeonXMasEventPanel : UI_Panel
{
    [SerializeField] private TMP_Text TitleText;
    [SerializeField] private TMP_Text DescText;
    [SerializeField] private TMP_Text EntryCountText;
    [SerializeField] private TMP_Text HighestScoreText;

    [SerializeField] private Button EntryButton;

    private DungeonChart _dungeonChart;

    private void Start()
    {
        if (_dungeonChart == null) 
            return;
        
        TitleText.text = ChartManager.GetString(_dungeonChart.Name);
        DescText.text = ChartManager.GetString(_dungeonChart.Desc);
        EntryButton.BindEvent(OnClickEntry);
    }

    public override void Open()
    {
        base.Open();

        if (_dungeonChart == null)
        {
            if (!ChartManager.DungeonCharts.TryGetValue((int)DungeonType.XMasEvent, out _dungeonChart))
                Debug.LogError("Fail Load DungeonChart : XMasEvent(102)");
        }

        SetUI();
    }

    private void SetUI()
    {
        EntryCountText.text = $"금일 남은 입장 횟수 : {Managers.Game.XMasEventData.XMasDungeonEntryCount}";
        HighestScoreText.text = Managers.Game.XMasEventData.XMasDungeonHighestScore.ToString();
    }

    private void OnClickEntry()
    {
        if (_dungeonChart == null)
        {
            Debug.LogError("Fail Entry XMasEventDungeon - DungeonChart is null");
            return;
        }

        if (Managers.Game.XMasEventData.XMasDungeonEntryCount <= 0)
        {
            Managers.Message.ShowMessage("금일 입장횟수를 모두 사용하였습니다.");
            return;
        }
        
        Managers.XMasEvent.Start();
    }
}