using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using Chart;
using DG.Tweening;
using GameData;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_PetInfoPanel : UI_Panel
{
    [SerializeField] private Transform ContentTr;

    [SerializeField] private TMP_Text ReinforceMaterialText;
    [SerializeField] private TMP_Text EquipButtonText;
    [SerializeField] private TMP_Text SelectPetLvText;
    [SerializeField] private TMP_Text ReinforceButtonText;
    [SerializeField] private TMP_Text PetSortText;

    [SerializeField] private Image ReinforceMaterialImage;
    [SerializeField] private Image PetImage;
    [SerializeField] private Image EquipButtonImage;

    [SerializeField] private Button EquipButton;
    [SerializeField] private Button SynthesisButton;
    [SerializeField] private Button AllSynthesisButton;
    [SerializeField] private Button ReinforceButton;
    [SerializeField] private Button UpButton;
    [SerializeField] private Button DownButton;
    [SerializeField] private Button PetSortButton;

    [SerializeField] private UI_StatEffectItem EquipStatEffectItem1;
    [SerializeField] private UI_StatEffectItem EquipStatEffectItem2;
    [SerializeField] private UI_StatEffectItem EquipStatEffectItem3;
    [SerializeField] private UI_StatEffectItem EquipStatEffectItem4;

    [SerializeField] private UI_StatEffectItem HaveStatEffectItem1;
    [SerializeField] private UI_StatEffectItem HaveStatEffectItem2;
    [SerializeField] private UI_StatEffectItem HaveStatEffectItem3;
    [SerializeField] private UI_StatEffectItem HaveStatEffectItem4;

    [SerializeField] private ScrollRect ScrollRect;

    [SerializeField] private Sprite EquipSprite;
    [SerializeField] private Sprite EquippedSprite;

    [SerializeField] private UpgradeEffect UpgradeEffect;

    private readonly List<UI_PetItem> _uiPetItems = new();
    private GameObject _modelObj;

    private PetChart _selectPetChart;
    private PetData _selectPetData;

    private bool IsCurrentPetEquip => _selectPetChart.Id == Managers.Game.EquipDatas[EquipType.Pet];

    private int CurrentIndex => _uiPetItems.FindIndex(item => item.PetId == CurrentUIPetItem.PetId);
    private int CurrentPetIndex;

    private UI_PetItem _currentUIPetItem;
    private UI_PetItem CurrentUIPetItem
    {
        get => _currentUIPetItem;
        set
        {
            if (_currentUIPetItem == value)
                return;

            if (_currentUIPetItem != null)
                _currentUIPetItem.SetActiveSelect(false);

            _currentUIPetItem = value;

            ChartManager.PetCharts.TryGetValue(_currentUIPetItem.PetId, out _selectPetChart);
            Managers.Game.PetDatas.TryGetValue(_currentUIPetItem.PetId, out _selectPetData);
            CurrentPetIndex = _currentUIPetItem.PetId;

            SetSelectPetInfo();
        }
    }

    private PetSortType _petSortType;

    private PetSortType PetSortType
    {
        get => _petSortType;
        set
        {
            _petSortType = value >= PetSortType.Max ? PetSortType.Have : value;

            switch (_petSortType)
            {
                case PetSortType.Have:
                    PetSortText.color = Color.white;
                    PetSortText.text = "보유";
                    break;
                case PetSortType.Grade:
                    PetSortText.color = Color.white;
                    PetSortText.text = "등급";
                    break;
                case PetSortType.TypeAscending:
                    PetSortText.color = Color.blue;
                    PetSortText.text = "타입";
                    break;
                case PetSortType.TypeDescending:
                    PetSortText.color = Color.red;
                    PetSortText.text = "타입";
                    break;
            }
        }
    }

    private List<(ItemType, int)> _saveItems = new();
    private List<int> _savePetIds = new();

    private bool _isSave;

    public List<CombinePetLog> CombinePetLogs = new();
    public Dictionary<int, ReinforcePetLog> UpgradeLog = new();

    private List<int> _changeDataPetIds = new();

    public void Start()
    {
        ReinforceButton.BindEvent(OnClickReinforce);
        ReinforceButton.BindEvent(OnClickReinforce, UIEvent.Pressed);
        EquipButton.BindEvent(OnClickEquip);
        UpButton.BindEvent(OnClickUp);
        DownButton.BindEvent(OnClickDown);
        SynthesisButton.BindEvent(OnClickSynthesis);
        AllSynthesisButton.BindEvent(OnClickAllSynthesis);
        PetSortButton.BindEvent(OnClickPetSort);

        CurrentUIPetItem = _uiPetItems[0];

        PetImage.transform.DOLocalMoveY(0, 0.5f).SetLoops(-1, LoopType.Yoyo);

        PetSortType = PetSortType.Have;
    }

    private void OnDisable()
    {
        if (!_isSave)
            return;

        _isSave = false;

        GameDataManager.GoodsGameData.SaveGameData();
        GameDataManager.PetGameData.SaveGameData();
        
        _changeDataPetIds.Clear();

        Param param = new Param();
        
        if (UpgradeLog.Count > 0)
        {
            foreach (var upgradeLog in UpgradeLog)
                param.Add($"PetUpgrade_{upgradeLog.Key}", upgradeLog.Value);
            UpgradeLog.Clear();
        }

        if (CombinePetLogs.Count > 0)
        {
            for (int i = 0; i < CombinePetLogs.Count; i++)
                param.Add($"Combine_{i}", CombinePetLogs[i]);
            CombinePetLogs.Clear();
        }

        if (param.Count > 0)
        {
            Utils.GetGoodsLog(ref param);
            Backend.GameLog.InsertLog("PetLog", param);
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.   UI.ClosePopupUI();
        }
    }

    private void MakePetItems()
    {
        if (_uiPetItems.Count <= 0)
            ContentTr.DestroyInChildren();
        else
            _uiPetItems.ForEach(uiPetItem => uiPetItem.gameObject.SetActive(false));

        var petCharts = GetSortPetList(PetSortType);

        var uiPetItems = new List<UI_PetItem>();

        int index = 0;

        foreach (var chartData in petCharts)
        {
            UI_PetItem uiPetItem;

            if (_uiPetItems.Count > index)
                uiPetItem = _uiPetItems[index++];
            else
            {
                uiPetItem = Managers.UI.MakeSubItem<UI_PetItem>(ContentTr);
                uiPetItems.Add(uiPetItem);
            }

            uiPetItem.gameObject.SetActive(true);
            uiPetItem.Init(chartData.Id, item => CurrentUIPetItem = item);

            uiPetItem.SetActiveSelect(CurrentPetIndex == chartData.Id);
        }
        
        uiPetItems.ForEach(uiPetItem => _uiPetItems.Add(uiPetItem));
    }

    private List<PetChart> GetSortPetList(PetSortType petSortType)
    {
        switch (petSortType)
        {
            case PetSortType.Have:
            {
                return ChartManager.PetCharts.Values
                    .OrderByDescending(petChart => Managers.Game.PetDatas[petChart.Id].IsAcquired)
                    .ThenByDescending(petChart => petChart.Grade)
                    .ThenByDescending(petChart => petChart.SubGrade)
                    .ThenByDescending(petChart => Managers.Game.PetDatas[petChart.Id].Level).ToList();
            }
            case PetSortType.Grade:
            {
                return ChartManager.PetCharts.Values
                    .OrderByDescending(petChart => petChart.Grade)
                    .ThenByDescending(petChart => petChart.SubGrade).ToList();
            }
            case PetSortType.TypeAscending:
            {
                return ChartManager.PetCharts.Values
                    .OrderBy(petChart => petChart.Id).ToList();
            }
            case PetSortType.TypeDescending:
            {
                return ChartManager.PetCharts.Values
                    .OrderByDescending(petChart => petChart.Id).ToList();
            }
            default:
                return ChartManager.PetCharts.Values.ToList();
        }
    }

    private void UpdatePetItems()
    {
        _uiPetItems.ForEach(uiPetItem => uiPetItem.Refresh());
    }

    public override void Open()
    {
        base.Open();
        MakePetItems();
        if (CurrentUIPetItem != null)
        {
            SetSelectPetInfo();
        }
    }

    private void SetSelectPetInfo()
    {
        CurrentUIPetItem.SetActiveSelect(true);

        var selectPetUpgradeLv = Math.Max(_selectPetData.Level - 1, 0);

        EquipStatEffectItem1.Init(_selectPetChart.EquipStatType1, _selectPetChart.EquipStatValue1 + _selectPetChart.EquipStatUpgradeValue1 * selectPetUpgradeLv);
        EquipStatEffectItem2.Init(_selectPetChart.EquipStatType2, _selectPetChart.EquipStatValue2 + _selectPetChart.EquipStatUpgradeValue2 * selectPetUpgradeLv);
        EquipStatEffectItem3.Init(_selectPetChart.EquipStatType3, _selectPetChart.EquipStatValue3 + _selectPetChart.EquipStatUpgradeValue3 * selectPetUpgradeLv);
        EquipStatEffectItem4.Init(_selectPetChart.EquipStatType4, _selectPetChart.EquipStatValue4 + _selectPetChart.EquipStatUpgradeValue4 * selectPetUpgradeLv);

        HaveStatEffectItem1.Init(_selectPetChart.HaveStatType1, _selectPetChart.HaveStatValue1 + _selectPetChart.HaveStatUpgradeValue1 * selectPetUpgradeLv);
        HaveStatEffectItem2.Init(_selectPetChart.HaveStatType2, _selectPetChart.HaveStatValue2 + _selectPetChart.HaveStatUpgradeValue2 * selectPetUpgradeLv);
        HaveStatEffectItem3.Init(_selectPetChart.HaveStatType3, _selectPetChart.HaveStatValue3 + _selectPetChart.HaveStatUpgradeValue3 * selectPetUpgradeLv);
        HaveStatEffectItem4.Init(_selectPetChart.HaveStatType4, _selectPetChart.HaveStatValue4 + _selectPetChart.HaveStatUpgradeValue4 * selectPetUpgradeLv);

        ReinforceMaterialImage.sprite =
            Managers.Resource.LoadItemIcon(_selectPetChart.LevelUpItemType, _selectPetChart.LevelUpItemId);

        var haveItemValue = Utils.GetItemValue(_selectPetChart.LevelUpItemType, _selectPetChart.LevelUpItemId);
        var needItemValue = Utils.CalculateItemValue(_selectPetChart.LevelUpItemValue, _selectPetChart.LevelUpItemIncreaseValue, _selectPetData.Level);

        var materialText = _selectPetData.IsAcquired && _selectPetData.Level < _selectPetChart.MaxLevel
            ? $"({haveItemValue.ToCurrencyString()}/{needItemValue.ToCurrencyString()})"
            : string.Empty;

        ReinforceMaterialText.text = _selectPetData.Level >= _selectPetChart.MaxLevel ? string.Empty : materialText;
        ReinforceMaterialText.color = haveItemValue >= needItemValue ? Color.white : Color.red;

        EquipButton.gameObject.SetActive(_selectPetData.IsAcquired);

        EquipButtonImage.sprite = IsCurrentPetEquip ? EquippedSprite : EquipSprite; 
        EquipButtonText.text = IsCurrentPetEquip ? "장착중" : "장   착";

        SelectPetLvText.text = _selectPetData.IsAcquired ? $"(Lv.{_selectPetData.Level}/{_selectPetChart.MaxLevel})" : string.Empty;

        if (_selectPetData.Level >= _selectPetChart.MaxLevel)
        {
            ReinforceButtonText.text = "Max";
            ReinforceButtonText.color = Color.red;
        }
        else
        {
            ReinforceButtonText.text = "강  화";
            ReinforceButtonText.color = Color.white;
        }

        AllSynthesisButton.gameObject.SetActive(_selectPetData.IsAcquired);
        SynthesisButton.gameObject.SetActive(_selectPetData.IsAcquired);
        ReinforceButton.gameObject.SetActive(_selectPetData.IsAcquired);

        PetImage.sprite = Managers.Resource.LoadPetIcon(_selectPetChart.Icon);
    }

    private void OnClickReinforce()
    {
        if (CurrentUIPetItem == null)
            return;

        if (_selectPetData.Level >= _selectPetChart.MaxLevel)
            return;

        var price = Utils.CalculateItemValue(_selectPetChart.LevelUpItemValue, _selectPetChart.LevelUpItemIncreaseValue,
            _selectPetData.Level);

        if (!Utils.IsEnoughItem(_selectPetChart.LevelUpItemType, _selectPetChart.LevelUpItemId, price))
            return;

        UpgradeEffect.Play();
        _selectPetData.Level += 1;
        Managers.Game.DecreaseItem(_selectPetChart.LevelUpItemType, _selectPetChart.LevelUpItemId, price);

        if (UpgradeLog.ContainsKey(_selectPetChart.Id))
        {
            UpgradeLog[_selectPetChart.Id].UsingGoldBar += price;
            UpgradeLog[_selectPetChart.Id].IncreaseLv += 1;
        }
        else
        {
            UpgradeLog.Add(_selectPetChart.Id, new ReinforcePetLog());
            UpgradeLog[_selectPetChart.Id].UsingGoldBar = price;
            UpgradeLog[_selectPetChart.Id].IncreaseLv = 1;
        }

        _isSave = true;

        Managers.Game.CalculateStat();
        
        if (!_changeDataPetIds.Contains(_selectPetChart.Id))
            _changeDataPetIds.Add(_selectPetChart.Id);
        
        SetSelectPetInfo();
    }

    private void OnClickEquip()
    {
        if (!Managers.Game.PetDatas[CurrentUIPetItem.PetId].IsAcquired)
            return;
        
        if (Managers.Game.EquipDatas[EquipType.Pet] == CurrentUIPetItem.PetId)
            return;

        Managers.Game.EquipDatas[EquipType.Pet] = CurrentUIPetItem.PetId;
        Managers.Game.MainPlayer.SetPet(CurrentUIPetItem.PetId);
        Managers.Game.CalculateStat();

        EquipButton.gameObject.SetActive(_selectPetData.IsAcquired);
        EquipButtonText.text = CurrentUIPetItem.PetId == Managers.Game.EquipDatas[EquipType.Pet] ? "장착중" : "장착";
        
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EquipItem, (int)QuestEquipItemType.Pet, 1));

        GameDataManager.EquipGameData.SaveGameData(EquipType.Pet);
        
        SetSelectPetInfo();
    }

    private void OnClickUp()
    {
        int prevIndex = CurrentIndex - 1;

        if (prevIndex < 0)
            return;

        CurrentUIPetItem = _uiPetItems[prevIndex];
        ScrollRect.verticalNormalizedPosition = _uiPetItems.Count > 1 ? 1 - CurrentUIPetItem.transform.GetSiblingIndex() / ((float)_uiPetItems.Count - 1) : 1;
    }

    private void OnClickDown()
    {
        int nextIndex = CurrentIndex + 1;

        if (nextIndex >= _uiPetItems.Count)
            return;

        CurrentUIPetItem = _uiPetItems[nextIndex];
        ScrollRect.verticalNormalizedPosition = _uiPetItems.Count > 1 ? 1 - CurrentUIPetItem.transform.GetSiblingIndex() / ((float)_uiPetItems.Count - 1) : 1;
    }

    private void OnClickSynthesis()
    {
        if (CurrentUIPetItem == null)
            return;

        if (!ChartManager.PetCharts.TryGetValue(CurrentUIPetItem.PetId, out var petChart))
            return;

        if (!Managers.Game.PetDatas.TryGetValue(CurrentUIPetItem.PetId, out var petData))
            return;

        if (petData.Quantity < petChart.CombineCount)
            return;

        if (!ChartManager.PetCharts.ContainsKey(petChart.CombineResult))
            return;

        petData.Quantity -= petChart.CombineCount;
        Managers.Game.PetDatas[petChart.CombineResult].Quantity += 1;
        
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.Combine, (int)QuestCombineItemType.Pet, 1));
        _isSave = true;
        
        CombinePetLogs.Add(new CombinePetLog(petChart.Id, petChart.CombineCount, petChart.CombineResult, 1));
        
        if (!_changeDataPetIds.Contains(petChart.Id))
            _changeDataPetIds.Add(petChart.Id);
        
        if (!_changeDataPetIds.Contains(petChart.CombineResult))
            _changeDataPetIds.Add(petChart.CombineResult);
        
        SetSelectPetInfo();
        UpdatePetItems();
    }

    private void OnClickAllSynthesis()
    {
        foreach (var petChart in ChartManager.PetCharts.Values)
        {
            if (!Managers.Game.PetDatas.TryGetValue(petChart.Id, out var petData))
                continue;

            if (petData.Quantity < petChart.CombineCount)
                continue;

            if (!ChartManager.PetCharts.ContainsKey(petChart.CombineResult))
                continue;

            int resultCount = petData.Quantity / petChart.CombineCount;
            int remainCount = petData.Quantity % petChart.CombineCount;

            petData.Quantity = remainCount;
            Managers.Game.PetDatas[petChart.CombineResult].Quantity += resultCount;
            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.Combine, (int)QuestCombineItemType.Pet, resultCount));
            
            CombinePetLogs.Add(new CombinePetLog(petChart.Id,resultCount * petChart.CombineCount, petChart.CombineResult, resultCount));
            
            if (!_changeDataPetIds.Contains(petChart.Id))
                _changeDataPetIds.Add(petChart.Id);
        
            if (!_changeDataPetIds.Contains(petChart.CombineResult))
                _changeDataPetIds.Add(petChart.CombineResult);
        }

        _isSave = true;
        
        SetSelectPetInfo();
        UpdatePetItems();
    }

    private void OnClickPetSort()
    {
        PetSortType += 1;
        MakePetItems();
        SetSelectPetInfo();
    }
}