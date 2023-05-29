using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using GameData;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Param = BackEnd.Param;

public class UI_CostumeInfoPanel : UI_Panel
{
    [SerializeField] private TMP_Text CostumeNameText;
    [SerializeField] private TMP_Text EquipButtonText;
    [SerializeField] private TMP_Text ShowEquipButtonText;
    [SerializeField] private TMP_Text AwakeningText;
    [SerializeField] private TMP_Text[] AwakeningLvTexts;
    [SerializeField] private TMP_Text SetNameText;

    [SerializeField] private Button EquipButton;
    [SerializeField] private Button ShowEquipButton;
    [SerializeField] private Button AwakeningButton;
    [SerializeField] private Button UpButton;
    [SerializeField] private Button DownButton;
    [SerializeField] private Button _costumeCollectionBtn;

    [SerializeField] private UI_Panel _costumeCollectionPanel;
    [SerializeField] private UI_Panel _costumeCollectionQuesionPanel;

    [SerializeField] private Image EquipButtonImage;
    [SerializeField] private Image ShowEquipButtonImage;
    [SerializeField] private Image EquipCostumeGradeImage;
    [SerializeField] private Image EquipCostumeIconImage;
    [SerializeField] private Image ShowEquipCostumeGradeImage;
    [SerializeField] private Image ShowEquipCostumeIconImage;

    [SerializeField] private GameObject SetObj;
    [SerializeField] private Transform CostumeItemContentTr;

    [SerializeField] private UI_StatEffectItem EquipStatEffectItem1;
    [SerializeField] private UI_StatEffectItem EquipStatEffectItem2;
    [SerializeField] private UI_StatEffectItem HaveStatEffectItem1;
    [SerializeField] private UI_StatEffectItem HaveStatEffectItem2;
    [SerializeField] private UI_StatEffectItem SetStatEffectItem1;
    [SerializeField] private UI_StatEffectItem SetStatEffectItem2;
    [SerializeField] private UI_StatEffectItem SetStatEffectItem3;

    [SerializeField] private UI_StatEffectItem[] AwakingEffectItems;
    
    [SerializeField] private UI_SetItem WeaponSetItem;
    [SerializeField] private UI_SetItem PetSetItem;
    [SerializeField] private UI_SetItem CostumeSetItem;

    [SerializeField] private ScrollRect ScrollRect;

    [SerializeField] private Transform UICostumeAwakeningItemRoot;
    [SerializeField] private GameObject SetLockObj;
    [SerializeField] private GameObject SetInfoObj;
    [SerializeField] private GameObject SetUpdateObj;

    [SerializeField] private Sprite EquipSprite;
    [SerializeField] private Sprite EquippedSprite;

    private readonly List<UI_CostumeItem> _uiCostumeItems = new();

    private UI_CostumeItem _currentUICostumeItem;
    private UI_CostumeItem CurrentUICostumeItem
    {
        get => _currentUICostumeItem;
        set
        {
            if (_currentUICostumeItem == value)
                return;

            if (_currentUICostumeItem != null)
                _currentUICostumeItem.SetActiveSelect(false);

            _currentUICostumeItem = value;
            SetUI();
        }
    }

    private int CurrentIndex =>
        _uiCostumeItems.FindIndex(uiCostumeItem => uiCostumeItem.CostumeId == CurrentUICostumeItem.CostumeId);

    private bool IsCurrentCostumeHave => Managers.Game.CostumeDatas[CurrentUICostumeItem.CostumeId].IsAcquired;

    private bool IsCurrentCostumeEquip =>
        Managers.Game.EquipDatas[EquipType.Costume] == _currentUICostumeItem.CostumeId;

    private bool IsCurrentCostumeShowEquip =>
        Managers.Game.EquipDatas[EquipType.ShowCostume] == _currentUICostumeItem.CostumeId;

    private Coroutine _equipSaveTimerCoroutine;
    private readonly List<UI_CostumeAwakeningItem> _uiCostumeAwakeningItems = new();

    private bool _isSave;
    private bool _isEquipSave;
    private List<AwakenCostumeLog> _awakenCostumeLogs = new();

    public void Start()
    {
        MakeCostumeItems();

        EquipButton.BindEvent(OnClickEquip);
        ShowEquipButton.BindEvent(OnClickShowEquip);
        AwakeningButton.BindEvent(OnClickAwakening);
        UpButton.BindEvent(OnClickUp);
        DownButton.BindEvent(OnClickDown);
        _costumeCollectionBtn.BindEvent(() => _costumeCollectionPanel.Open());

        CurrentUICostumeItem = _uiCostumeItems[0];
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_costumeCollectionQuesionPanel.gameObject.activeInHierarchy)
            {
                _costumeCollectionQuesionPanel.Close();
                return;
            }

            if (_costumeCollectionPanel.gameObject.activeInHierarchy)
            {
                _costumeCollectionPanel.Close();
                return;
            }
            Managers.UI.ClosePopupUI();
        }
    }

    public override void Open()
    {
        base.Open();

        if (CurrentUICostumeItem != null)
        {
            UpdateCostumeItems();
            SetUI();
        }

        _costumeCollectionPanel.Close();
        _costumeCollectionQuesionPanel.Close();

    }

    private void OnDisable()
    {
        if (_isSave)
        {
            _isSave = false;
            GameDataManager.CostumeGameData.SaveGameData();
        }

        if (_isEquipSave)
        {
            _isEquipSave = false;
            GameDataManager.EquipGameData.SaveGameData();
        }

        if (_awakenCostumeLogs.Count > 0)
        {
            Param param = new Param();
            param.Add("AwakenCostume", _awakenCostumeLogs);
            Backend.GameLog.InsertLog("CostumeLog", param);
            _awakenCostumeLogs.Clear();
        }
        
        Managers.Model.ResetPlayerModel();
    }

    private void MakeCostumeItems()
    {
        _uiCostumeItems.Clear();
        CostumeItemContentTr.DestroyInChildren();

        var sortedList = ChartManager.CostumeCharts.Values.OrderBy(costumeChart => costumeChart.Sort);

        foreach (var chartData in sortedList)
        {
            var uiCostumeItem = Managers.UI.MakeSubItem<UI_CostumeItem>(CostumeItemContentTr);
            uiCostumeItem.Init(chartData.Id, item => CurrentUICostumeItem = item);
            uiCostumeItem.SetActiveSelect(false);
            _uiCostumeItems.Add(uiCostumeItem);
        }
    }

    private void UpdateCostumeItems()
    {
        _uiCostumeItems.ForEach(uiCostumeItem => uiCostumeItem.Refresh());
    }

    private void SetUI()
    {
        _currentUICostumeItem.SetActiveSelect(true);

        var costumeChart = ChartManager.CostumeCharts[CurrentUICostumeItem.CostumeId];
        var costumeData = Managers.Game.CostumeDatas[CurrentUICostumeItem.CostumeId];

        CostumeNameText.text = ChartManager.GetString(costumeChart.Name);
        
        EquipStatEffectItem1.Init(costumeChart.EquipStatType1, costumeChart.EquipStatValue1);
        EquipStatEffectItem2.Init(costumeChart.EquipStatType2, costumeChart.EquipStatValue2);
        HaveStatEffectItem1.Init(costumeChart.HaveStatType1, costumeChart.HaveStatValue1);
        HaveStatEffectItem2.Init(costumeChart.HaveStatType2, costumeChart.HaveStatValue2);

        EquipButton.gameObject.SetActive(IsCurrentCostumeHave);
        ShowEquipButton.gameObject.SetActive(IsCurrentCostumeHave);

        EquipButtonImage.sprite = IsCurrentCostumeEquip ? EquippedSprite : EquipSprite;
        EquipButtonText.text = IsCurrentCostumeEquip ? "장착중" : "장   착";
        
        ShowEquipButtonImage.sprite = IsCurrentCostumeShowEquip ? EquippedSprite : EquipSprite;
        ShowEquipButtonText.text = IsCurrentCostumeShowEquip ? "외형장착중" : "외형장착";

        SetObj.gameObject.SetActive(costumeChart.SetId != 0);

        var equipCostumeChart = ChartManager.CostumeCharts[Managers.Game.EquipDatas[EquipType.Costume]];
        EquipCostumeGradeImage.sprite = Managers.Resource.LoadItemGradeBg(equipCostumeChart.Grade);
        EquipCostumeIconImage.sprite = Managers.Resource.LoadCostumeIcon(equipCostumeChart.Icon);

        var showEquipCostumeChart = ChartManager.CostumeCharts[Managers.Game.EquipDatas[EquipType.ShowCostume]];
        ShowEquipCostumeGradeImage.sprite = Managers.Resource.LoadItemGradeBg(showEquipCostumeChart.Grade);
        ShowEquipCostumeIconImage.sprite = Managers.Resource.LoadCostumeIcon(showEquipCostumeChart.Icon);

        if (costumeChart.SetId != 0)
        {
            if (costumeChart.SetId == -1)
            {
                SetLockObj.SetActive(true);
                SetInfoObj.SetActive(false);
                SetUpdateObj.SetActive(true);
            }
            else
            {
                SetInfoObj.SetActive(true);
                SetUpdateObj.SetActive(false);
                
                var setChart = ChartManager.SetCharts[costumeChart.SetId];

                SetNameText.text = ChartManager.GetString(setChart.Name);

                SetStatEffectItem1.Init(setChart.StatType1, setChart.StatValue1);
                SetStatEffectItem2.Init(setChart.StatType2, setChart.StatValue2);
                SetStatEffectItem3.Init(setChart.StatType3, setChart.StatValue3);

                WeaponSetItem.Init(ItemType.Weapon, setChart.WeaponId);
                PetSetItem.Init(ItemType.Pet, setChart.PetId);
                CostumeSetItem.Init(ItemType.Costume, setChart.CostumeId);

                SetLockObj.SetActive(!WeaponSetItem.IsHave || !PetSetItem.IsHave || !CostumeSetItem.IsHave);
            }
        }

        if (costumeData.Awakening >= costumeChart.MaxAwakening || !costumeData.IsAcquired)
        {
            AwakeningText.text = string.Empty;
            AwakeningButton.gameObject.SetActive(false);
        }
        else
        {
            var costumeAwakeningChart = ChartManager.CostumeAwakeningCharts[(costumeChart.Id, costumeData.Awakening + 1)];
            AwakeningText.text =
                $"{Utils.GetItemValue(ItemType.Costume, costumeChart.Id)}/" +
                $"{costumeAwakeningChart.AwakeningItemValue}";
            AwakeningText.color =
                Utils.GetItemValue(ItemType.Costume, costumeChart.Id) >= costumeAwakeningChart.AwakeningItemValue
                    ? Color.white
                    : Color.red;
            AwakeningButton.gameObject.SetActive(true);
        }

        if (_uiCostumeAwakeningItems.Count <= 0)
            UICostumeAwakeningItemRoot.DestroyInChildren();
        else
            _uiCostumeAwakeningItems.ForEach(uiCostumeAwakeningItem => uiCostumeAwakeningItem.gameObject.SetActive(false));
        
        var uiCostumeAwakeningItems = _uiCostumeAwakeningItems.ToList();
        int index = 0;
        
        for (int awakening = 1; awakening <= costumeChart.MaxAwakening; awakening++)
        {
            if (!ChartManager.CostumeAwakeningCharts.TryGetValue((costumeChart.Id, awakening), out var costumeAwakeningChart))
                continue;

            UI_CostumeAwakeningItem uiCostumeAwakeningItem;

            if (uiCostumeAwakeningItems.Count > index)
                uiCostumeAwakeningItem = uiCostumeAwakeningItems[index++];
            else
            {
                uiCostumeAwakeningItem = Managers.UI.MakeSubItem<UI_CostumeAwakeningItem>(UICostumeAwakeningItemRoot);
                _uiCostumeAwakeningItems.Add(uiCostumeAwakeningItem);
            }

            bool isAwakening = Managers.Game.CostumeDatas[costumeChart.Id].Awakening >= awakening;
            
            uiCostumeAwakeningItem.gameObject.SetActive(true);
            uiCostumeAwakeningItem.Init(awakening, costumeAwakeningChart.StatType, costumeAwakeningChart.StatValue, isAwakening);
        }

        Managers.Model.PlayerModel.SetCostume(CurrentUICostumeItem.CostumeId);
    }

    private void OnClickEquip()
    {
        if (IsCurrentCostumeEquip)
            return;

        if (!Managers.Game.CostumeDatas[CurrentUICostumeItem.CostumeId].IsAcquired)
            return;

        Managers.Game.EquipDatas[EquipType.Costume] = CurrentUICostumeItem.CostumeId;
        Managers.Game.EquipDatas[EquipType.ShowCostume] = CurrentUICostumeItem.CostumeId;
        Managers.Game.MainPlayer.SetCostume(CurrentUICostumeItem.CostumeId);

        SetSaveEquipTimer();

        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EquipItem, (int)QuestEquipItemType.Costume, 1));

        Managers.Game.CalculateStat();

        SetUI();
    }

    private void OnClickShowEquip()
    {
        if (IsCurrentCostumeShowEquip)
            return;
        
        if (!Managers.Game.CostumeDatas[CurrentUICostumeItem.CostumeId].IsAcquired)
            return;
        
        Managers.Game.EquipDatas[EquipType.ShowCostume] = CurrentUICostumeItem.CostumeId;
        Managers.Game.MainPlayer.SetCostume(CurrentUICostumeItem.CostumeId);

        SetSaveEquipTimer();
        
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EquipItem, (int)QuestEquipItemType.Costume, 1));
        
        _isEquipSave = true;
        
        SetUI();
    }

    private void OnClickAwakening()
    {
        int costumeId = _currentUICostumeItem.CostumeId;

        if (!ChartManager.CostumeCharts.TryGetValue(_currentUICostumeItem.CostumeId, out var costumeChart))
            return;

        if (!Managers.Game.CostumeDatas.TryGetValue(_currentUICostumeItem.CostumeId, out var costumeData))
            return;

        int costumeAwakeningLv = costumeData.Awakening + 1;

        if (!ChartManager.CostumeAwakeningCharts.TryGetValue((costumeId, costumeAwakeningLv),
                out var costumeAwakeningChart))
            return;

        if (costumeData.Quantity < costumeAwakeningChart.AwakeningItemValue)
            return;

        Managers.Game.CostumeDatas[costumeId].Quantity -= costumeAwakeningChart.AwakeningItemValue;
        Managers.Game.CostumeDatas[costumeId].Awakening += 1;

        _awakenCostumeLogs.Add(new AwakenCostumeLog(costumeId.ToString(), Managers.Game.CostumeDatas[costumeId].Awakening.ToString(),
            costumeAwakeningChart.AwakeningItemValue.ToString() ));
        _isSave = true;
        
        Managers.Game.CalculateStat();

        SetUI();
    }

    private void OnClickUp()
    {
        int prevIndex = CurrentIndex - 1;

        if (prevIndex < 0)
            return;

        CurrentUICostumeItem = _uiCostumeItems[prevIndex];
        ScrollRect.verticalNormalizedPosition = _uiCostumeItems.Count > 1 ? 1 - CurrentUICostumeItem.transform.GetSiblingIndex() / ((float)_uiCostumeItems.Count - 1) : 1;
    }

    private void OnClickDown()
    {
        int nextIndex = CurrentIndex + 1;

        if (nextIndex >= _uiCostumeItems.Count)
            return;

        CurrentUICostumeItem = _uiCostumeItems[nextIndex];
        ScrollRect.verticalNormalizedPosition = _uiCostumeItems.Count > 1 ? 1 - CurrentUICostumeItem.transform.GetSiblingIndex() / ((float)_uiCostumeItems.Count - 1) : 1;
    }

    private void SetSaveEquipTimer()
    {
        if (_equipSaveTimerCoroutine != null)
            StopCoroutine(_equipSaveTimerCoroutine);

        _equipSaveTimerCoroutine = StartCoroutine(CoSaveEquipTimer());
    }

    private IEnumerator CoSaveEquipTimer()
    {
        yield return new WaitForSeconds(5);

        if (!_isEquipSave)
            yield break;

        _isEquipSave = false;
        GameDataManager.EquipGameData.SaveGameData();
        _equipSaveTimerCoroutine = null;
    }

}