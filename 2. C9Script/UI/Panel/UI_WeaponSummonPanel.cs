using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponSummonPanel : UI_Panel
{
    [Serializable]
    public class WeaponTab
    {
        public WeaponSummonType WeaponSummonType;
        public Button TabButton;
        public GameObject SelectObj;
    }
    
    [SerializeField] private TMP_Text Summon1ValueText;
    [SerializeField] private TMP_Text Summon2ValueText;
    [SerializeField] private TMP_Text Summon3ValueText;
    
    [SerializeField] private Button Summon1Button;
    [SerializeField] private Button Summon2Button;
    [SerializeField] private Button Summon3Button;

    [SerializeField] private WeaponTab[] WeaponTabs;

    // Type, ProbabilityID, Count
    public Action<SummonType, string, int, double, Action> SummonEvent;

    private bool _isAllSummon;

    private string ProbabilityID =>
        ChartManager.ProbabilityIds[$"Weapon_{_selectWeaponTab.WeaponSummonType.ToString()}_Gotcha_{Managers.Game.UserData.SummonWeaponLv:00}"];

    private string AllProbabilityId =>
        ChartManager.ProbabilityIds[$"Weapon_All_Gotcha_{Managers.Game.UserData.SummonWeaponLv:00}"];

    private WeaponTab _selectWeaponTab;

    private WeaponTab SelectWeaponTab
    {
        get => _selectWeaponTab;
        set
        {
            if (_selectWeaponTab != null)
                _selectWeaponTab.SelectObj.SetActive(false);

            _isAllSummon = value.WeaponSummonType == WeaponSummonType.All;

            _selectWeaponTab = value;
            _selectWeaponTab.SelectObj.SetActive(true);
            SetUI();
        }
    }

    private void Start()
    {
        foreach (var tab in WeaponTabs)
        {
            tab.TabButton.BindEvent(() => SelectWeaponTab = tab);
        }
    }

    public override void Open()
    {
        base.Open();

        if (SelectWeaponTab == null)
            SelectWeaponTab = WeaponTabs[3];

        Summon1Button.ClearEvent();
        Summon1Button.BindEvent(OnClickSummon1);
        Summon1Button.GetComponent<UI_EventHandler>().ClickSoundType = UISoundType.ClickSummon;
        
        Summon2Button.ClearEvent();
        Summon2Button.BindEvent(OnClickSummon2);
        Summon2Button.GetComponent<UI_EventHandler>().ClickSoundType = UISoundType.ClickSummon;
        
        Summon3Button.ClearEvent();
        Summon3Button.BindEvent(OnClickSummon3);
        Summon3Button.GetComponent<UI_EventHandler>().ClickSoundType = UISoundType.ClickSummon;
        
        SetUI();
    }

    private void SetUI()
    {
        Summon1ValueText.text = _isAllSummon ?
            ChartManager.SystemCharts[SystemData.Summon_All_Weapon_1_Cost].Value.ToString("N0") :
            ChartManager.SystemCharts[SystemData.Summon_Weapon_1_Cost].Value.ToString("N0");
        
        Summon2ValueText.text = _isAllSummon ? 
            ChartManager.SystemCharts[SystemData.Summon_All_Weapon_2_Cost].Value.ToString("N0") :
            ChartManager.SystemCharts[SystemData.Summon_Weapon_2_Cost].Value.ToString("N0");
        
        Summon3ValueText.text = _isAllSummon ?
            ChartManager.SystemCharts[SystemData.Summon_All_Weapon_3_Cost].Value.ToString("N0") :
            ChartManager.SystemCharts[SystemData.Summon_Weapon_3_Cost].Value.ToString("N0");
    }
    
    private void OnClickSummon1()
    {
        string probabilityId;
        double cost;
        
        if (_isAllSummon)
        {
            probabilityId = AllProbabilityId;
            cost = ChartManager.SystemCharts[SystemData.Summon_All_Weapon_1_Cost].Value;
        }
        else
        {
            probabilityId = ProbabilityID;
            cost = ChartManager.SystemCharts[SystemData.Summon_Weapon_1_Cost].Value;
        }
        
        SummonEvent?.Invoke(SummonType.Weapon, probabilityId, 8, cost, OnClickSummon1);
    }

    private void OnClickSummon2()
    {
        string probabilityId;
        double cost;
        
        if (_isAllSummon)
        {
            probabilityId = AllProbabilityId;
            cost = ChartManager.SystemCharts[SystemData.Summon_All_Weapon_2_Cost].Value;
        }
        else
        {
            probabilityId = ProbabilityID;
            cost = ChartManager.SystemCharts[SystemData.Summon_Weapon_2_Cost].Value;
        }
        
        SummonEvent?.Invoke(SummonType.Weapon, probabilityId, 40, cost, OnClickSummon2);
    }

    private void OnClickSummon3()
    {
        string probabilityId;
        double cost;
        
        if (_isAllSummon)
        {
            probabilityId = AllProbabilityId;
            cost = ChartManager.SystemCharts[SystemData.Summon_All_Weapon_3_Cost].Value;
        }
        else
        {
            probabilityId = ProbabilityID;
            cost = ChartManager.SystemCharts[SystemData.Summon_Weapon_3_Cost].Value;
        }
        
        SummonEvent?.Invoke(SummonType.Weapon, probabilityId, 80, cost, OnClickSummon3);
    }
}