using System;
using System.Collections.Generic;
using System.Linq;
using Chart;
using TMPro;
using UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_LabAwakeningPanel : UI_Panel
{
    [Serializable]
    public record PresetItem
    {
        public Button PresetButton;
        public GameObject OffObj;
    }
    
    [SerializeField] private TMP_Text LabTotalLevelText;
    [SerializeField] private TMP_Text LabLevelText;
    [SerializeField] private TMP_Text[] PatternSetText;
    [SerializeField] private TMP_Text ResetCostText;
    
    [SerializeField] private Button AwakeningGuideButton;
    [SerializeField] private Button ResetButton;
    [SerializeField] private Button LabAwakeningSettingButton;

    [SerializeField] private UI_Panel UILabAwakeningGuidePanel;
    [SerializeField] private UI_Panel UILabAwakeningSettingPanel;

    [SerializeField] private Transform UILabAwakeningItemRoot;

    [SerializeField] private PresetItem[] PresetItems;

    private int _currentPresetId;

    private readonly Dictionary<int, UI_LabAwakeningItem> _uiLabAwakeningItems = new();
    private readonly List<int> _openAwakeningId = new();

    private List<LabGradeRateChart> _gradeRates = new();
    private List<LabPatternChart> _patternRates = new();
    private List<LabStatRateChart> _statRates = new();
    private readonly Dictionary<(int, Grade), (float, float)> _statValueDic = new();

    private bool isCheck;

    private readonly CompositeDisposable _saveComposite = new();
    private readonly Dictionary<int, Dictionary<int, bool>> _changePresetIdDic = new();

    private int CurrentPresetId
    {
        get => _currentPresetId;
        set
        {
            Managers.Game.LabEquipPresetId = value;
            _currentPresetId = value;
            SetCurrentPresetUI();
            SetCost();
            Managers.Game.CalculateStat();
        }
    }

    private PresetItem _currentPresetItem;
    private PresetItem CurrentPresetItem
    {
        get => _currentPresetItem;
        set
        {
            if (_currentPresetItem != null)
                _currentPresetItem.OffObj.SetActive(true);

            _currentPresetItem = value;
            _currentPresetItem.OffObj.SetActive(false);
        }
    }

    private bool _isChangePreset;

    private void Start()
    {
        AwakeningGuideButton.BindEvent(() => UILabAwakeningGuidePanel.Open());
        LabAwakeningSettingButton.BindEvent(() => UILabAwakeningSettingPanel.Open());
        
        UILabAwakeningGuidePanel.Close();
        UILabAwakeningSettingPanel.Close();
        
        SetUI();

        CurrentPresetId = Managers.Game.LabEquipPresetId;

        for (int i = 0; i < 4; i++)
        {
            int presetId = i;
            if (PresetItems.Length <= i)
                continue;
            
            PresetItems[i].OffObj.SetActive(true);
            PresetItems[i].PresetButton.BindEvent(() =>
            {
                CurrentPresetItem = PresetItems[presetId];
                CurrentPresetId = presetId;
                _isChangePreset = true;
            });
        }

        CurrentPresetItem = PresetItems[CurrentPresetId];

        _gradeRates = ChartManager.LabGradeRateCharts.Values.ToList().OrderBy(data => data.Rate).ToList();
        _patternRates = ChartManager.LabPatternCharts.Values.ToList().OrderBy(data => data.Rate).ToList();
        _statRates = ChartManager.LabStatRateCharts.Values.ToList().OrderBy(data => data.Rate).ToList();

        foreach (var chartData in ChartManager.LabStatCharts.Values)
        {
            _statValueDic.TryAdd((chartData.StatId, chartData.Grade), ((float)chartData.MinValue, (float)chartData.MaxValue));
        }
        
        ResetButton.BindEvent(OnClickReset);
        ResetButton.BindEvent(OnClickReset, UIEvent.Pressed);
        ResetButton.BindEvent(ReservationSave, UIEvent.PointerUp);
        ResetButton.BindEvent(() => 
            Observable.TimerFrame(1).Subscribe(_ => Managers.Game.CalculateStat()), UIEvent.PointerUp);
    }

    private void OnDisable()
    {
        Save();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Managers.UI.FindPopup<UI_YesNoPopup>() != null)
                return;
            
            if (UILabAwakeningGuidePanel.gameObject.activeSelf)
            {
                UILabAwakeningGuidePanel.gameObject.SetActive(false);
                return;
            }

            if (UILabAwakeningSettingPanel.gameObject.activeSelf)
            {
                UILabAwakeningSettingPanel.gameObject.SetActive(false);
                return;
            }

            Managers.UI.ClosePopupUI();
        }
    }

    public override void Open()
    {
        base.Open();

        isCheck = false;
        SetUI();
        SetCurrentPresetUI();
    }

    private void SetUI()
    {
        LabTotalLevelText.text = $"Lv.{Managers.Game.LabResearchDatas.Values.ToList().Sum(data => data.Level)}";
        LabLevelText.text =
            $"{ChartManager.GetString(LabSkillType.Fire.ToString())}Lv.{Managers.Game.LabResearchDatas[LabSkillType.Fire].Level} " +
            $"{ChartManager.GetString(LabSkillType.Frozen.ToString())}Lv.{Managers.Game.LabResearchDatas[LabSkillType.Frozen].Level} " +
            $"{ChartManager.GetString(LabSkillType.Soy.ToString())}Lv.{Managers.Game.LabResearchDatas[LabSkillType.Soy].Level} " +
            $"{ChartManager.GetString(LabSkillType.Buzzword.ToString())}Lv.{Managers.Game.LabResearchDatas[LabSkillType.Buzzword].Level}";
        
        SetCost();
    }

    private void SetCost()
    {
        if (!Managers.Game.LabAwakeningDatas.TryGetValue(CurrentPresetId, out var labAwakeningDatas))
            return;

        var lockCount = labAwakeningDatas.Values.Count(data => data.StatId != 0 && data.IsLock);

        var cost = ChartManager.SystemCharts[SystemData.SkillAwakeningReset_Cost].Value +
                   ChartManager.SystemCharts[SystemData.SkillAwakeningLock_Cost].Value * lockCount;

        ResetCostText.text = cost.ToCurrencyString();
    }

    private void SetCurrentPresetUI()
    {
        if (!Managers.Game.LabAwakeningDatas.TryGetValue(CurrentPresetId, out var labAwakeningDatas))
            return;

        Dictionary<int, int> patternCountDictionary = new()
        {
            {1, labAwakeningDatas.Values.Where(data => data.StatId != 0).Count(data => data.PatternId == 1)},
            {2, labAwakeningDatas.Values.Where(data => data.StatId != 0).Count(data => data.PatternId == 2)},
            {3, labAwakeningDatas.Values.Where(data => data.StatId != 0).Count(data => data.PatternId == 3)},
            {4, labAwakeningDatas.Values.Where(data => data.StatId != 0).Count(data => data.PatternId == 4)}
        };

        Dictionary<int, int> applySetDictionary = new()
        {
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 }
        };

        var isNextPattern = false;
        var prevPatternId = 0;

        foreach (var chartData in ChartManager.LabSetCharts.Values)
        {
            if (!patternCountDictionary.TryGetValue(chartData.SetPattern, out var patternCount))
                continue;

            if (isNextPattern)
            {
                if (prevPatternId == chartData.SetPattern)
                    continue;
                
                isNextPattern = false;
            }

            if (patternCount < chartData.SetNumber)
            {
                isNextPattern = true;
                prevPatternId = chartData.SetPattern;
                continue;
            }

            applySetDictionary[chartData.SetPattern] = chartData.Id;
        }

        for (var patternId = 1; patternId <= 4; patternId++)
        {
            if (!patternCountDictionary.TryGetValue(patternId, out var patternCount))
                continue;

            if (!applySetDictionary.TryGetValue(patternId, out var setId))
                continue;

            // 적용중인 효과 없음
            if (setId == 0)
            {
                PatternSetText[patternId - 1].text = $"({patternCount}) : 적용중인 세트효과가 없습니다.";
            }
            else
            {
                if (!ChartManager.LabSetCharts.TryGetValue(setId, out var labSetChart))
                    continue;

                if (!ChartManager.StatCharts.TryGetValue(labSetChart.StatType, out var statChart))
                    continue;

                PatternSetText[patternId - 1].text = statChart.ValueType == ValueType.Percent
                    ? $"({patternCount}) : {ChartManager.GetString(statChart.Name)} {(labSetChart.StatValue * 100).ToCurrencyString()}%"
                    : $"({patternCount}) : {ChartManager.GetString(statChart.Name)} {labSetChart.StatValue}";
            }
        }
        
        SetLabAwakeningItems();
    }

    private void SetLabAwakeningItems()
    {
        if (!Managers.Game.LabAwakeningDatas.TryGetValue(Managers.Game.LabEquipPresetId, out var labAwakeningDatas))
            return;
        
        if (_uiLabAwakeningItems.Count <= 0)
            UILabAwakeningItemRoot.DestroyInChildren();

        var totalResearchLevel = Managers.Game.LabResearchDatas.Values.Sum(data => data.Level);

        _openAwakeningId.Clear();
        
        foreach (var chartData in ChartManager.LabAwakeningCharts.Values)
        {
            if (!labAwakeningDatas.TryGetValue(chartData.Id, out var labAwakeningData))
                continue;

            if (!_uiLabAwakeningItems.TryGetValue(chartData.Id, out var uiLabAwakeningItem))
            {
                uiLabAwakeningItem = Managers.UI.MakeSubItem<UI_LabAwakeningItem>(UILabAwakeningItemRoot);
                _uiLabAwakeningItems.Add(chartData.Id, uiLabAwakeningItem);
            }
            
            uiLabAwakeningItem.Init(labAwakeningData.Id, totalResearchLevel >= chartData.OpenLv, SetCost);
            
            if (totalResearchLevel >= chartData.OpenLv)
                _openAwakeningId.Add(chartData.Id);
        }
    }

    private void OnClickReset()
    {
        if (isCheck)
            return;
        
        bool isCheckFlag = false;
        int checkCount = 0;

        if (Managers.Game.LabAwakeningSettingData.PatternId.Value != -1)
            checkCount += 1;

        if (Managers.Game.LabAwakeningSettingData.StatId.Value != -1)
            checkCount += 1;

        if (Managers.Game.LabAwakeningSettingData.Grade.Value != -1)
            checkCount += 1;
        
        foreach (var id in _openAwakeningId)
        {
            if (Managers.Game.LabAwakeningDatas[CurrentPresetId][id].IsLock)
                continue;

            int flagCount = 0;
            
            var labAwakeningData = Managers.Game.LabAwakeningDatas[CurrentPresetId][id];

            if (checkCount == 0)
                continue;

            if (Managers.Game.LabAwakeningSettingData.PatternId.Value == labAwakeningData.PatternId)
                flagCount += 1;

            if (Managers.Game.LabAwakeningSettingData.Grade.Value == (int)labAwakeningData.Grade)
                flagCount += 1;

            if (Managers.Game.LabAwakeningSettingData.StatId.Value == labAwakeningData.StatId)
                flagCount += 1;

            if (flagCount >= checkCount)
                isCheckFlag = true;
        }
        
        if (isCheckFlag)
        {
            isCheck = true;
            var popup = Managers.UI.ShowPopupUI<UI_YesNoPopup>();
            popup.Init("설정된 옵션이 포함되어 있습니다.\n계속 진행 하시겠습니까?", () =>
            {
                Managers.UI.ClosePopupUI();
                isCheck = false;
                ResetAwakeningStat();
            }, () =>
            {
                Managers.UI.ClosePopupUI();
                isCheck = false;
            });
        }
        else
            ResetAwakeningStat();

        void ResetAwakeningStat()
        {
            bool isChangeFlag = false;

            List<int> changeAwakeningIds = new();

            int lockCount = _openAwakeningId.Count(awakeningId => Managers.Game.LabAwakeningDatas[CurrentPresetId][awakeningId].IsLock);
            double cost = ChartManager.SystemCharts[SystemData.SkillAwakeningReset_Cost].Value +
                          lockCount * ChartManager.SystemCharts[SystemData.SkillAwakeningLock_Cost].Value;

            if (!Utils.IsEnoughItem(ItemType.Goods, (int)Goods.StarBalloon, cost))
            {
                Managers.Message.ShowMessage("재화가 부족합니다.");
                return;
            }

            foreach (var id in _openAwakeningId)
            {
                if (Managers.Game.LabAwakeningDatas[CurrentPresetId][id].IsLock)
                    continue;

                int flagCount = 0;
            
                var labAwakeningData = Managers.Game.LabAwakeningDatas[CurrentPresetId][id];

                int pattern = GetRandomPattern();
                Grade grade = GetRandomGrade();
                int statId = GetRandomStat();
                double value = Random.Range(_statValueDic[(statId, grade)].Item1, _statValueDic[(statId, grade)].Item2);

                labAwakeningData.PatternId = pattern;
                labAwakeningData.Grade = grade;
                labAwakeningData.StatId = statId;
                labAwakeningData.StatValue = value;

                isChangeFlag = true;
                
                changeAwakeningIds.Add(labAwakeningData.Id);

                if (checkCount == 0)
                    continue;

                if (Managers.Game.LabAwakeningSettingData.PatternId.Value == pattern)
                    flagCount += 1;

                if (Managers.Game.LabAwakeningSettingData.Grade.Value == (int)grade)
                    flagCount += 1;

                if (Managers.Game.LabAwakeningSettingData.StatId.Value == statId)
                    flagCount += 1;

                if (flagCount >= checkCount)
                    isCheckFlag = true;
            }

            if (isChangeFlag)
            {
                if (!_changePresetIdDic.ContainsKey(CurrentPresetId))
                    _changePresetIdDic.Add(CurrentPresetId, new Dictionary<int, bool>());

                changeAwakeningIds.ForEach(awakeningId =>
                {
                    if (_changePresetIdDic[CurrentPresetId].ContainsKey(awakeningId))
                        _changePresetIdDic[CurrentPresetId][awakeningId] = true;
                    else
                        _changePresetIdDic[CurrentPresetId].Add(awakeningId, true);

                });

                Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.StarBalloon, cost);

                SetCurrentPresetUI();
            }
        }
    }

    private int GetRandomPattern()
    {
        double randNum = Random.value;
        double totalNum = 0;

        foreach (var chartData in _patternRates)
        {
            totalNum += chartData.Rate;

            if (randNum <= totalNum)
                return chartData.Id;
        }

        return 1;
    }

    private Grade GetRandomGrade()
    {
        double randNum = Random.value;
        double totalNum = 0;

        foreach (var chartData in _gradeRates)
        {
            totalNum += chartData.Rate;

            if (randNum <= totalNum)
                return chartData.Grade;
        }

        return Grade.Normal;
    }

    private int GetRandomStat()
    {
        double randNum = Random.value;
        double totalNum = 0;

        foreach (var chartData in _statRates)
        {
            totalNum += chartData.Rate;

            if (randNum <= totalNum)
                return chartData.StatId;
        }

        return 0;
    }

    private void ReservationSave()
    {
        _saveComposite.Clear();
        Observable.Timer(TimeSpan.FromSeconds(5f)).ObserveOnMainThread().Subscribe(_ =>
        {
            Save();
        }).AddTo(_saveComposite);
    }

    private void Save()
    {
        _saveComposite.Clear();
        
        List<(int, int)> savePresetIds = new();
        
        foreach (var presetId in _changePresetIdDic.Keys)
        {
            foreach (var awakeningId in _changePresetIdDic[presetId].Keys)
            {
                if (!_changePresetIdDic[presetId][awakeningId])
                    continue;
                
                savePresetIds.Add((presetId, awakeningId));
            }
        }

        savePresetIds.ForEach(data => _changePresetIdDic[data.Item1][data.Item2] = false);
        
        Debug.Log("Save AwakeningData");

        if (savePresetIds.Count > 0)
        {
            GameDataManager.LabGameData.SaveAwakeningGameData(savePresetIds, _isChangePreset);
            GameDataManager.GoodsGameData.SaveGameData();
        }
        else if (_isChangePreset)
            GameDataManager.LabGameData.SaveEquipPresetGameData();

        _isChangePreset = false;
    }
}