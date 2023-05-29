using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatPointUpgradePanel : UI_Panel
{
    [SerializeField] private TMP_Text CurrentLvText;
    [SerializeField] private TMP_Text StatPointText;
    
    [SerializeField] private Transform UIStatPointUpgradeItemRoot;

    [SerializeField] private Button ResetButton;

    [SerializeField] private UI_ResetStatPointPanel uiResetStatPointPanel;
    
    public Action RefreshStat;

    private readonly List<UI_StatPointUpgradeItem> _uiStatPointUpgradeItems = new();

    public bool IsOnPanel => uiResetStatPointPanel.gameObject.activeSelf;

    public void CloseResetPanel()
    {
        uiResetStatPointPanel.Close();
    }

    private int HaveStatPoint => Math.Min(
        Managers.Game.UserData.Level * (int)ChartManager.SystemCharts[SystemData.LevelUpStatPoint].Value,
        (int)ChartManager.SystemCharts[SystemData.StatPoint1_MaxLevel].Value * (int)ChartManager.SystemCharts[SystemData.LevelUpStatPoint].Value);

    private int _multiple = 1;

    public int Multiple
    {
        get => _multiple;
        set
        {
            _multiple = value;
            _uiStatPointUpgradeItems.ForEach(uiStatPointUpgradeItem => uiStatPointUpgradeItem.Multiple = _multiple);
        }
    }

    private void Start()
    {
        RefreshStat += SetLvAndStatPoint;
        SetLvAndStatPoint();
        SetPointUpgradeItems();
        
        ResetButton.BindEvent(() => uiResetStatPointPanel.Open());
        uiResetStatPointPanel.OnRefresh = RefreshUpgradeItems;
        
        Managers.Game.UserData.OnChangeLevel.Subscribe(_ =>
        {
            SetLvAndStatPoint();
        });

        Managers.Game.UserData.OnChangeUseStat.Subscribe(_ =>
        {
            StatPointText.text = $"( " +
                                 $"{HaveStatPoint- Managers.Game.UserData.UseStatPoint}" +
                                 $" / {HaveStatPoint} )";
        });
    }

    private void OnDisable()
    {
        uiResetStatPointPanel.Close();

        if (_uiStatPointUpgradeItems.Find(uiStatPointUpgradeItem => uiStatPointUpgradeItem.IsSave))
        {
            GameDataManager.UserGameData.SaveGameData();
            GameDataManager.StatLevelGameData.SaveGameData();
        }
    }

    private void SetLvAndStatPoint()
    {
        CurrentLvText.text = $"Lv {Managers.Game.UserData.Level.ToString()}";
        StatPointText.text = $"({HaveStatPoint - Managers.Game.UserData.UseStatPoint} / {HaveStatPoint})";
    }

    private void SetPointUpgradeItems()
    {
        UIStatPointUpgradeItemRoot.DestroyInChildren();
        _uiStatPointUpgradeItems.Clear();

        var statIds = new List<int>();

        foreach (var chartData in ChartManager.StatPointUpgradeCharts.Values)
        {
            if (statIds.Contains(chartData.StatId))
                continue;
            
            statIds.Add(chartData.StatId);
        }

        foreach (var statId in statIds)
        {
            UI_StatPointUpgradeItem uiStatPointUpgradeItem
                = Managers.UI.MakeSubItem<UI_StatPointUpgradeItem>(UIStatPointUpgradeItemRoot);

            uiStatPointUpgradeItem.Init(statId, Multiple);
            _uiStatPointUpgradeItems.Add(uiStatPointUpgradeItem);
        }
    }

    private void RefreshUpgradeItems()
    {
        _uiStatPointUpgradeItems.ForEach(uiStatPointUpgradeItem => uiStatPointUpgradeItem.Refresh());
    }
}