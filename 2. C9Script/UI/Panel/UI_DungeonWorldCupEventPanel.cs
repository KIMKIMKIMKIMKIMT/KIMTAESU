using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_DungeonWorldCupEventPanel : UI_Panel
{
    [SerializeField] private TMP_Text EntryText;
    [SerializeField] private TMP_Text HighestScoreValueText;
    [SerializeField] private TMP_Text TitleText;
    [SerializeField] private TMP_Text DescText;
    [SerializeField] private TMP_Text GuideText;
    [SerializeField] private TMP_Text BtnEntryTxt;
    [SerializeField] private TMP_Text MaxEntryTxt;

    [SerializeField] private Button EntryButton;

    private void Start()
    {
        EntryButton.BindEvent(OnClickEntry);

        if (ChartManager.DungeonCharts.TryGetValue((int)DungeonType.WorldCupEvent, out var worldCupEventDungeonChart))
        {
            TitleText.text = ChartManager.GetString(worldCupEventDungeonChart.Name);
            DescText.text = ChartManager.GetString(worldCupEventDungeonChart.Desc);
        }

        GuideText.text = ChartManager.GetString("Event_Dungeon_Desc_2");
        BtnEntryTxt.text = ChartManager.GetString("Dungeon_Start");
        MaxEntryTxt.text = ChartManager.GetString("Highest_Record");
    }

    public override void Open()
    {
        base.Open();

        EntryText.text = $"{ChartManager.GetString("Event_Dungeon_Remain_Time")} : {Managers.Game.ProgramerBeatData.ProgramerBeatEntryCount}";
        HighestScoreValueText.text = Managers.Game.ProgramerBeatData.ProgramerBeatHighestScore.ToCurrencyString();
    }

    private void OnClickEntry()
    {
        if (!ChartManager.WorldCupEventDungeonCharts.TryGetValue(1, out var worldCupEventDungeonChart))
        {
            Managers.Message.ShowMessage("던전 데이터가 존재하지 않습니다!");
            return;
        }

        if (Managers.Game.ProgramerBeatData.ProgramerBeatEntryCount <= 0)
        {
            Managers.Message.ShowMessage(ChartManager.GetString("Event_Dungeon_Desc_1"));
            return;
        }
        
        Managers.WorldCupEvent.StartDungeon();
    }
}