using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    public class UI_UnlimitedPointUpgradePanel : UI_Panel
    {
        [SerializeField] private TMP_Text CurrentLvText;
        [SerializeField] private TMP_Text UnlimitedPointText;

        [SerializeField] private Transform UIUnlimitedPointUpgradeItemRoot;

        [SerializeField] private Button ResetButton;

        [SerializeField] private UI_ResetUnlimitedPointPanel uiResetUnlimitedPointPanel;

        public Action RefreshStat;

        private readonly List<UI_UnlimitedPointUpgradeItem> _uiUnlimitedPointUpgradeItems = new();

        public bool IsOnPanel => uiResetUnlimitedPointPanel.gameObject.activeSelf;

        public void CloseResetPanel()
        {
            uiResetUnlimitedPointPanel.Close();
        }

        private int HaveUnlimitedPoint()
        {
            if (Managers.Game.UserData.Level <= ChartManager.SystemCharts[SystemData.StatPoint1_MaxLevel].Value)
                return 0;

            int unlimitedLv = Managers.Game.UserData.Level -
                              (int)ChartManager.SystemCharts[SystemData.StatPoint1_MaxLevel].Value;
            return Math.Min(unlimitedLv * (int)ChartManager.SystemCharts[SystemData.UnlimitedPoint_Level_Per_Value].Value,
                (int)ChartManager.SystemCharts[SystemData.UnlimitedPoint_MaxLevel].Value *
                (int)ChartManager.SystemCharts[SystemData.UnlimitedPoint_Level_Per_Value].Value);
        }

        private int _multiple = 1;

        public int Multiple
        {
            get => _multiple;
            set
            {
                _multiple = value;
                _uiUnlimitedPointUpgradeItems.ForEach(uiUnlimitedPointUpgradeItem => uiUnlimitedPointUpgradeItem.Multiple = _multiple);
            }
        }

        private void Start()
        {
            RefreshStat += SetLvAndUnlimitedPoint;
            SetLvAndUnlimitedPoint();
            SetPointUpgradeItems();

            ResetButton.BindEvent(() => uiResetUnlimitedPointPanel.Open());
            uiResetUnlimitedPointPanel.OnRefresh = RefreshUpgradeItems;

            Managers.Game.UserData.OnChangeLevel.Subscribe(_ => { SetLvAndUnlimitedPoint(); });

            Managers.Game.UserData.OnChangeUnlimitedPoint.Subscribe(_ =>
            {
                UnlimitedPointText.text = $"( " +
                                     $"{HaveUnlimitedPoint() - Managers.Game.UserData.UseUnlimitedPoint}" +
                                     $" / {HaveUnlimitedPoint()} )";
            });
        }

        private void OnDisable()
        {
            uiResetUnlimitedPointPanel.Close();

            if (_uiUnlimitedPointUpgradeItems.Find(uiStatPointUpgradeItem => uiStatPointUpgradeItem.IsSave))
            {
                GameDataManager.UserGameData.SaveGameData();
                GameDataManager.UnlimitedStatLevelGameData.SaveGameData();
            }
        }

        private void SetLvAndUnlimitedPoint()
        {
            CurrentLvText.text = $"Lv {Managers.Game.UserData.Level.ToString()}";
            UnlimitedPointText.text = $"({HaveUnlimitedPoint() - Managers.Game.UserData.UseUnlimitedPoint} / {HaveUnlimitedPoint()})";
        }

        private void SetPointUpgradeItems()
        {
            UIUnlimitedPointUpgradeItemRoot.DestroyInChildren();
            _uiUnlimitedPointUpgradeItems.Clear();

            var statIds = new List<int>();

            foreach (var chartData in ChartManager.UnlimitedPointUpgradeCharts.Values)
            {
                if (statIds.Contains(chartData.StatId))
                    continue;

                statIds.Add(chartData.StatId);
            }

            foreach (var statId in statIds)
            {
                var uiStatPointUpgradeItem = Managers.UI.MakeSubItem<UI_UnlimitedPointUpgradeItem>(UIUnlimitedPointUpgradeItemRoot);
                uiStatPointUpgradeItem.Init(statId, Multiple);
                
                _uiUnlimitedPointUpgradeItems.Add(uiStatPointUpgradeItem);
            }
        }

        private void RefreshUpgradeItems()
        {
            _uiUnlimitedPointUpgradeItems.ForEach(uiUnlimitedPointUpgradeItem => uiUnlimitedPointUpgradeItem.Refresh());
        }
    }
}