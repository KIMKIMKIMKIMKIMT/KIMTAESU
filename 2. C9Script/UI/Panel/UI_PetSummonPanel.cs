using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;


public class UI_PetSummonPanel : UI_Panel
{
    [SerializeField] private TMP_Text Summon1ValueText;
    [SerializeField] private TMP_Text Summon2ValueText;
    [SerializeField] private TMP_Text Summon3ValueText;

    [SerializeField] private Button Summon1Button;
    [SerializeField] private Button Summon2Button;
    [SerializeField] private Button Summon3Button;

    private string ProbabilityID =>
        ChartManager.ProbabilityIds[$"Pet_Gotcha_{Managers.Game.UserData.SummonPetLv:00}"];

    // Type, ProbabilityID, Count
    public Action<SummonType, string, int, double, Action> SummonEvent;

    public override void Open()
    {
        base.Open();

        Summon1Button.ClearEvent();
        Summon1Button.BindEvent(OnClickSummon1);

        Summon2Button.ClearEvent();
        Summon2Button.BindEvent(OnClickSummon2);

        Summon3Button.ClearEvent();
        Summon3Button.BindEvent(OnClickSummon3);

        SetUI();
    }

    private void SetUI()
    {
        Summon1ValueText.text = ChartManager.SystemCharts[SystemData.Summon_Pet_1_Cost].Value.ToString("N0");
        Summon2ValueText.text = ChartManager.SystemCharts[SystemData.Summon_Pet_2_Cost].Value.ToString("N0");
        Summon3ValueText.text = ChartManager.SystemCharts[SystemData.Summon_Pet_3_Cost].Value.ToString("N0");
    }

    private void OnClickSummon1()
    {
        double cost = ChartManager.SystemCharts[SystemData.Summon_Pet_1_Cost].Value;
        SummonEvent?.Invoke(SummonType.Pet, ProbabilityID, 8, cost, OnClickSummon1);
    }

    private void OnClickSummon2()
    {
        double cost = ChartManager.SystemCharts[SystemData.Summon_Pet_2_Cost].Value;
        SummonEvent?.Invoke(SummonType.Pet, ProbabilityID, 40, cost, OnClickSummon2);
    }

    private void OnClickSummon3()
    {
        double cost = ChartManager.SystemCharts[SystemData.Summon_Pet_3_Cost].Value;
        SummonEvent?.Invoke(SummonType.Pet, ProbabilityID, 80, cost, OnClickSummon3);
    }
}