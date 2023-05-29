using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using Chart;
using GameData;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WeaponInfoPanel : UI_Panel
{
    [SerializeField] private Button[] WeaponTypeTabButtons;
    [SerializeField] private Button EquipButton;
    [SerializeField] private Button SynthesisButton;
    [SerializeField] private Button AllSynthesisButton;
    [SerializeField] private Button ReinforceButton;

    [SerializeField] private GameObject[] WeaponTypeSelectTabObjs;

    [SerializeField] private Transform WeaponItemContentTr;

    [SerializeField] private Image SelectWeaponImage;
    [SerializeField] private Image SelectWeaponGradeImage;
    [SerializeField] private Image SelectWeaponSubGradeImage;
    [SerializeField] private GameObject SelectWeaponGodgodBg;
    [SerializeField] private Image EquipButtonImage;

    [SerializeField] private TMP_Text SelectWeaponText;
    [SerializeField] private TMP_Text EquipEffectValueText;
    [SerializeField] private TMP_Text HaveEffectValueText;
    [SerializeField] private TMP_Text MaterialText;
    [SerializeField] private TMP_Text SelectWeaponLvText;
    [SerializeField] private TMP_Text ReinforceButtonText;
    [SerializeField] private TMP_Text EquipButtonText;

    [SerializeField] private Sprite EquipSprite;
    [SerializeField] private Sprite EquippedSprite;

    [SerializeField] private UpgradeEffect UpgradeEffect;
    [SerializeField] private UI_SynthesysWeaponNoticePanel UISynthesysWeaponNoticePanel;

    [SerializeField] private UI_StatEffectItem EquipStatEffectItem;
    [SerializeField] private UI_StatEffectItem HaveStatEffectItem;

    private WeaponType _currentWeaponType = WeaponType.Mic;
    private readonly List<UI_WeaponItem> _uiWeaponItems = new();

    private WeaponChart _selectWeaponChart;
    private WeaponData _selectWeaponData;

    private bool IsSelectWeaponIsEquip => _selectWeaponChart.Id == Managers.Game.EquipDatas[EquipType.Weapon];

    private readonly List<CombineWeaponLog> _combineWeaponLogs = new();

    public WeaponType CurrentWeaponType
    {
        get => _currentWeaponType;
        private set
        {
            WeaponTypeSelectTabObjs[(int)CurrentWeaponType].SetActive(false);

            _currentWeaponType = value;

            WeaponTypeSelectTabObjs[(int)CurrentWeaponType].SetActive(true);

            MakeWeapons(CurrentWeaponType);
        }
    }

    private int _selectWeaponIndex;

    public int SelectWeaponIndex
    {
        get => _selectWeaponIndex;
        private set
        {
            _selectWeaponIndex = value;

            if (_selectWeaponItem != null)
                _selectWeaponItem.OnSelect();

            ChartManager.WeaponCharts.TryGetValue(_selectWeaponIndex, out _selectWeaponChart);
            Managers.Game.WeaponDatas.TryGetValue(_selectWeaponIndex, out _selectWeaponData);

            RefreshSelectWeaponInfo();
        }
    }

    private UI_WeaponItem _selectWeaponItem;
    private bool _isSave;
    
    private CompositeDisposable _compositeDisposable = new ();

    private readonly List<int> _changeDataWeaponIds = new();

    private ItemType _levelUpItemType = ItemType.None;
    private void Start()
    {
        SetButton();
        
        _uiWeaponItems.Clear();
        WeaponItemContentTr.DestroyInChildren();
            
        int equipWeaponId = Managers.Game.EquipDatas[(int)EquipType.Weapon];
        var weaponChart = ChartManager.WeaponCharts[equipWeaponId];
        CurrentWeaponType = weaponChart.Type;
        _selectWeaponItem = _uiWeaponItems.Find(uiWeaponItem => uiWeaponItem.ItemIndex == equipWeaponId);
        SelectWeaponIndex = equipWeaponId;
        
        UISynthesysWeaponNoticePanel.Init(AllSynthesis);
    }

    private void OnEnable()
    {
        Managers.Game.GoodsDatas[(int)Goods.Gold].Subscribe(_ =>
        {
            RefreshMaterial();
        }).AddTo(_compositeDisposable);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UISynthesysWeaponNoticePanel.gameObject.activeSelf)
            {
                UISynthesysWeaponNoticePanel.Close();
                return;
            }
            
            Managers.UI.ClosePopupUI();
        }
    }

    public override void Open()
    {
        base.Open();
        
        if (_selectWeaponItem)
            RefreshSelectWeaponInfo();
    }

    private void SetButton()
    {
        foreach (WeaponType weaponType in Enum.GetValues(typeof(WeaponType)))
        {
            var weaponTypeIndex = weaponType;

            if (WeaponTypeTabButtons.Length > (int)weaponTypeIndex)
                WeaponTypeTabButtons[(int)weaponTypeIndex].BindEvent(() => { CurrentWeaponType = weaponTypeIndex; });

            if (WeaponTypeSelectTabObjs.Length > (int)weaponTypeIndex)
                WeaponTypeSelectTabObjs[(int)weaponTypeIndex].SetActive(false);
        }

        EquipButton.BindEvent(OnClickEquip);
        SynthesisButton.BindEvent(OnClickSynthesys);
        SynthesisButton.BindEvent(OnClickSynthesys, UIEvent.Pressed);
        AllSynthesisButton.BindEvent(OnClickAllSynthesis);
        ReinforceButton.BindEvent(Reinforce);
        ReinforceButton.BindEvent(Reinforce, UIEvent.Pressed);
    }

    private void OnDisable()
    {
        _compositeDisposable.Clear();
        UISynthesysWeaponNoticePanel.Close();
        
        if (!_isSave)
            return;
        
        _isSave = false;
                
        GameDataManager.SaveItemData(_levelUpItemType);
        GameDataManager.WeaponGameData.SaveGameData();
        _changeDataWeaponIds.Clear();

        if (_combineWeaponLogs.Count > 0)
        {
            Param param = new Param();
            for (int i = 0; i < _combineWeaponLogs.Count; i++)
                param.Add($"Combine_{i}", _combineWeaponLogs[i]);
            _combineWeaponLogs.Clear();
            Utils.GetGoodsLog(ref param);
            Backend.GameLog.InsertLog("WeaponLog", param);
        }
                
        Managers.Game.CalculateStat();
    }

    private void MakeWeapons(WeaponType weaponType)
    {
        _uiWeaponItems.ForEach(uiWeaponItem => { uiWeaponItem.gameObject.SetActive(false); });

        int index = 0;
        // Data Loop 돌면서 생성하는 로직 처리
        foreach (var weaponData in ChartManager.WeaponCharts.Values.Where(weaponData => weaponData.Type == weaponType))
        {
            UI_WeaponItem uiWeaponItem;

            if (_uiWeaponItems.Count < index)
                uiWeaponItem = _uiWeaponItems[index];
            else
            {
                uiWeaponItem = Managers.UI.MakeSubItem<UI_WeaponItem>(WeaponItemContentTr);
                _uiWeaponItems.Add(uiWeaponItem);
            }

            uiWeaponItem.SetItem(weaponData.Id, selectWeaponItem =>
            {
                if (_selectWeaponItem != null)
                    _selectWeaponItem.OffSelect();

                _selectWeaponItem = selectWeaponItem;
                SelectWeaponIndex = selectWeaponItem.ItemIndex;
            });

            if (_selectWeaponChart != null)
            {
                if (_selectWeaponChart.Id == weaponData.Id)
                    uiWeaponItem.OnSelect();
            }

            index++;
        }
    }

    private void OnClickEquip()
    {
        if (SelectWeaponIndex <= 0)
            return;

        Managers.Game.EquipDatas[EquipType.Weapon] = SelectWeaponIndex;
        Managers.Game.MainPlayer.SetWeapon(SelectWeaponIndex);
        Managers.Model.PlayerModel.SetWeapon(SelectWeaponIndex);
        RefreshSelectWeaponInfo();
        Managers.Game.CalculateStat();
        GameDataManager.EquipGameData.SaveGameData(EquipType.Weapon);
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EquipItem, (int)QuestEquipItemType.Weapon, 1));
    }

    private void OnClickSynthesys()
    {
        if (SelectWeaponIndex <= 0)
            return;

        if (!Managers.Game.WeaponDatas.TryGetValue(SelectWeaponIndex, out var weaponData))
        {
            Debug.LogError($"{SelectWeaponIndex} WeaponData Null");
            return;
        }

        if (!ChartManager.WeaponCharts.TryGetValue(SelectWeaponIndex, out var weaponChart))
        {
            Debug.LogError($"{SelectWeaponIndex} WeaponChart Null");
            return;
        }

        if (weaponData.Quantity < weaponChart.CombineCount)
        {
            Debug.LogError("무기 합성 필요 갯수 부족");
            return;
        }

        if (!ChartManager.WeaponCharts.ContainsKey(weaponChart.CombineResultId))
            return;

        weaponData.Quantity -= weaponChart.CombineCount;
        Managers.Game.WeaponDatas[weaponChart.CombineResultId].Quantity++;
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.Combine, (int)QuestCombineItemType.Weapon, 1));
        
        _combineWeaponLogs.Add(new CombineWeaponLog(weaponChart.Id, weaponChart.CombineCount, weaponChart.CombineResultId, 1));
        
        if (!_changeDataWeaponIds.Contains(weaponChart.Id))
            _changeDataWeaponIds.Add(weaponChart.Id);
        
        if (!_changeDataWeaponIds.Contains(weaponChart.CombineResultId))
            _changeDataWeaponIds.Add(weaponChart.CombineResultId);
        
        _isSave = true;

        Managers.Game.CalculateStat();
    }

    private void OnClickAllSynthesis()
    {
        UISynthesysWeaponNoticePanel.Open();
    }

    private void AllSynthesis()
    {
        var cache = EventSystem.current;
        EventSystem.current.enabled = false;

        _isSave = true;

        var weaponCharts = ChartManager.WeaponCharts.OrderBy(chart => chart.Key)
            .Where(chart => chart.Value.Type == CurrentWeaponType).Select(chart => chart.Value);

        Dictionary<int, int> skillOpenCheckDic = new();

        foreach (var skillData in Managers.Game.SkillDatas.Values)
        {
            if (skillData.IsOpen)
                continue;

            if (!ChartManager.SkillCharts.TryGetValue(skillData.Id, out var skillChart))
            {
                Debug.LogError($"Can't Find Weapon Chart - {skillData.Id}");
                continue;
            }

            skillOpenCheckDic.TryAdd(skillChart.UnlockItemId, skillChart.UnlockItemValue);
        }

        foreach (var weaponChart in weaponCharts)
        {
            int weaponIndex = weaponChart.Id;

            if (!Managers.Game.WeaponDatas.ContainsKey(weaponIndex))
                continue;

            if (!Managers.Game.WeaponDatas[weaponIndex].IsAcquired)
            {
                continue;
            }

            if (Managers.Game.WeaponDatas[weaponIndex].Quantity < weaponChart.CombineCount)
            {
                continue;
            }

            if (!Managers.Game.WeaponDatas.ContainsKey(weaponChart.CombineResultId))
            {
                continue;
            }

            if (weaponChart.Type != CurrentWeaponType)
                continue;
            
            if (skillOpenCheckDic.TryGetValue(weaponChart.Id, out var unlockItemValue))
            {
                // 스킬을 해방할 수 있는 상태이면 넘김.
                if (Managers.Game.WeaponDatas[weaponChart.Id].Quantity >= unlockItemValue)
                    continue;
            }

            int combineCount =  Managers.Game.WeaponDatas[weaponIndex].Quantity / weaponChart.CombineCount;
            int quantity =  Managers.Game.WeaponDatas[weaponIndex].Quantity % weaponChart.CombineCount;

            Managers.Game.WeaponDatas[weaponIndex].Quantity = quantity;
            Managers.Game.WeaponDatas[weaponChart.CombineResultId].Quantity += combineCount;
            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.Combine, (int)QuestCombineItemType.Weapon, combineCount));
            
            _combineWeaponLogs.Add(new CombineWeaponLog(weaponChart.Id, combineCount * weaponChart.CombineCount, weaponChart.CombineResultId, combineCount));
            
            if (!_changeDataWeaponIds.Contains(weaponChart.Id))
                _changeDataWeaponIds.Add(weaponChart.Id);
        
            if (!_changeDataWeaponIds.Contains(weaponChart.CombineResultId))
                _changeDataWeaponIds.Add(weaponChart.CombineResultId);
        }
        
        Managers.Game.CalculateStat();

        cache.enabled = true;
    }

    private void Reinforce()
    {
        if (SelectWeaponIndex <= 0)
            return;

        if (!Managers.Game.WeaponDatas.TryGetValue(SelectWeaponIndex, out var weaponData))
        {
            Debug.LogError($"{SelectWeaponIndex} WeaponData Null");
            return;
        }

        if (!ChartManager.WeaponCharts.TryGetValue(SelectWeaponIndex, out var weaponChart))
        {
            Debug.LogError($"{SelectWeaponIndex} WeaponChart Null");
            return;
        }

        if (weaponData.Level >= weaponChart.MaxLevel)
        {
            Debug.Log("무기 최대 레벨");
            return;
        }

        var cost = Utils.CalculateItemValue(weaponChart.LevelUpItemValue, weaponChart.LevelUpItemIncreaseValue,
            weaponData.Level);

        if (!Utils.IsEnoughItem(weaponChart.LevelUpItemType, weaponChart.LevelUpItemId, cost))
        {
            Managers.Message.ShowMessage(MessageType.LackReinforceMaterial);
            return;
        }
        
        UpgradeEffect.Play();
        
        _isSave = true;
        _levelUpItemType = weaponChart.LevelUpItemType;
        
        if (!_changeDataWeaponIds.Contains(weaponChart.Id))
            _changeDataWeaponIds.Add(weaponChart.Id);

        Managers.Game.DecreaseItem(weaponChart.LevelUpItemType, weaponChart.LevelUpItemId,
            cost);
        weaponData.Level++;
        RefreshSelectWeaponInfo();
    }

    private void RefreshSelectWeaponInfo()
    {
        EquipButton.gameObject.SetActive(SelectWeaponIndex != 0);
        SynthesisButton.gameObject.SetActive(SelectWeaponIndex != 0);
        AllSynthesisButton.gameObject.SetActive(SelectWeaponIndex != 0);
        ReinforceButton.gameObject.SetActive(SelectWeaponIndex != 0);
        
        if (SelectWeaponIndex <= 0)
        {
            SelectWeaponGodgodBg.SetActive(false);
            SelectWeaponImage.gameObject.SetActive(false);
            SelectWeaponGradeImage.sprite = Managers.Resource.LoadItemGradeBg(Grade.Normal);
            SelectWeaponSubGradeImage.gameObject.SetActive(false);
            SelectWeaponText.text = string.Empty;

            EquipEffectValueText.text = string.Empty;
            HaveEffectValueText.text = string.Empty;
            SelectWeaponLvText.text = string.Empty;

            MaterialText.text = string.Empty;

            EquipStatEffectItem.gameObject.SetActive(false);
            HaveStatEffectItem.gameObject.SetActive(false);
        }
        else
        {
            
            EquipStatEffectItem.gameObject.SetActive(true);
            HaveStatEffectItem.gameObject.SetActive(true);
            
            EquipStatEffectItem.Init(_selectWeaponChart.EquipStatType, 0);
            HaveStatEffectItem.Init(_selectWeaponChart.HaveStatType, 0);

            SelectWeaponGodgodBg.SetActive(_selectWeaponChart.Grade == Grade.Godgod);
            SelectWeaponImage.gameObject.SetActive(true);
            SelectWeaponImage.sprite = Managers.Resource.LoadWeaponIcon(_selectWeaponChart.Icon);
            SelectWeaponGradeImage.sprite = Managers.Resource.LoadItemGradeBg(_selectWeaponChart.Grade);
            SelectWeaponSubGradeImage.gameObject.SetActive(true);
            SelectWeaponSubGradeImage.sprite = Managers.Resource.LoadSubGradeIcon(_selectWeaponChart.SubGrade);
            if (SelectWeaponSubGradeImage.sprite == null)
                SelectWeaponSubGradeImage.gameObject.SetActive(false);
            SelectWeaponText.text = ChartManager.GetString(_selectWeaponChart.Name);
            
            double equipEffectValue = _selectWeaponData.IsAcquired
                ? _selectWeaponChart.EquipStatValue + _selectWeaponChart.EquipStatUpgradeValue * Math.Max(_selectWeaponData.Level - 1, 0)
                : _selectWeaponChart.EquipStatValue;
            equipEffectValue = Math.Round(equipEffectValue, 7);
            string equipEffectText = ChartManager.StatCharts[_selectWeaponChart.EquipStatType].ValueType == ValueType.Percent ?
                $"{(equipEffectValue * 100).ToCurrencyString()}%" : 
                equipEffectValue.ToCurrencyString();
            EquipEffectValueText.text = equipEffectText;

            double haveEffectValue = _selectWeaponData.IsAcquired
                ? _selectWeaponChart.HaveStatValue + _selectWeaponChart.HaveStatUpgradeValue * Math.Max(_selectWeaponData.Level - 1, 0)
                : _selectWeaponChart.HaveStatValue;
            haveEffectValue = Math.Round(haveEffectValue, 7);
            string haveEffectText = ChartManager.StatCharts[_selectWeaponChart.EquipStatType].ValueType == ValueType.Percent
                ? $"{(haveEffectValue * 100).ToCurrencyString()}%"
                : haveEffectValue.ToCurrencyString();
            HaveEffectValueText.text = haveEffectText;
            
            var haveItemValue=  Utils.GetItemValue(_selectWeaponChart.LevelUpItemType, _selectWeaponChart.LevelUpItemId);
            var needItemValue = Utils.CalculateItemValue(_selectWeaponChart.LevelUpItemValue,
                _selectWeaponChart.LevelUpItemIncreaseValue, _selectWeaponData.Level);
            
            MaterialText.text = _selectWeaponData.IsAcquired && _selectWeaponData.Level < _selectWeaponChart.MaxLevel ? needItemValue.ToCurrencyString() : string.Empty;
            MaterialText.color = haveItemValue >= needItemValue ? Color.white : Color.red;

            SelectWeaponLvText.text = _selectWeaponData.IsAcquired ? $"(Lv.{_selectWeaponData.Level}/{_selectWeaponChart.MaxLevel})" : string.Empty;
            ReinforceButtonText.text = _selectWeaponData.Level >= _selectWeaponChart.MaxLevel ? "Max" : "강   화";
            ReinforceButtonText.color =
                _selectWeaponData.Level >= _selectWeaponChart.MaxLevel ? Color.red : Color.white;

            if (_selectWeaponData.Level >= _selectWeaponChart.MaxLevel)
            {
                ReinforceButtonText.text = "Max";
                ReinforceButtonText.color = Color.red;
            }
            else
            {
                ReinforceButtonText.text = "강   화";
                ReinforceButtonText.color = Color.white;
            }

            ReinforceButton.gameObject.SetActive(_selectWeaponData.IsAcquired);
            EquipButton.gameObject.SetActive(_selectWeaponData.IsAcquired);
            
            if (IsSelectWeaponIsEquip)
            {
                EquipButtonText.text = "장착중";
                EquipButtonImage.sprite = EquippedSprite;
            }
            else
            {
                EquipButtonText.text = "장   착";
                EquipButtonImage.sprite = EquipSprite;
            }
        }
    }

    private void RefreshMaterial()
    {
        if (SelectWeaponIndex <= 0)
            return;
        
        var haveItemValue=  Utils.GetItemValue(_selectWeaponChart.LevelUpItemType, _selectWeaponChart.LevelUpItemId);
        var needItemValue = Utils.CalculateItemValue(_selectWeaponChart.LevelUpItemValue,
            _selectWeaponChart.LevelUpItemIncreaseValue, _selectWeaponData.Level);
        
        MaterialText.text = _selectWeaponData.IsAcquired && _selectWeaponData.Level < _selectWeaponChart.MaxLevel ? needItemValue.ToCurrencyString() : string.Empty;
        MaterialText.color = haveItemValue >= needItemValue ? Color.white : Color.red;

        SelectWeaponLvText.text = _selectWeaponData.IsAcquired ? $"(Lv.{_selectWeaponData.Level}/{_selectWeaponChart.MaxLevel})" : string.Empty;
        ReinforceButtonText.text = _selectWeaponData.Level >= _selectWeaponChart.MaxLevel ? "Max" : "강   화";
        ReinforceButtonText.color =
            _selectWeaponData.Level >= _selectWeaponChart.MaxLevel ? Color.red : Color.white;
    }
}