using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using UI.Panel;
using Chart;
using GameData;
using System;
using TMPro;
using BackEnd;

public class UI_WorldWoodPannel : UI_Panel
{
    #region Fields
    [Header("Effect")]
    [SerializeField] private UI_StatEffectItem _gradeStatEffectItem1;
    [SerializeField] private UI_StatEffectItem _gradeStatEffectItem2;
    [SerializeField] private UI_StatEffectItem _gradeStatEffectItem3;
    [SerializeField] private UI_StatEffectItem _gradeStatEffectItem4;
    [Header("Awakening")]
    [SerializeField] private UI_StatEffectItem _wakeUpStatEffectItem1;
    [SerializeField] private UI_StatEffectItem _wakeUpStatEffectItem2;
    [SerializeField] private UI_StatEffectItem _wakeUpStatEffectItem3;
    [SerializeField] private UI_StatEffectItem _wakeUpStatEffectItem4;
    [SerializeField] private UI_StatEffectItem _wakeUpStatEffectItem5;

    [SerializeField] private RectTransform[] _worldWoodObjs;

    [SerializeField] private UI_Panel _ui_WorldWoodAwakeningPanel;
    [SerializeField] private UI_Panel _ui_WorldWoodAwakeningSettingPanel;


    [SerializeField] private GameObject _plantPannelObj;
    [SerializeField] private GameObject _evolutionLockObj;
    [SerializeField] private GameObject _evolutionGoodsObj;
    [SerializeField] private GameObject[] _awakeningTxtObjs;
    [SerializeField] private RectTransform _chulguObj;

    [SerializeField] private Image _woodImg;
    [SerializeField] private Image _floorImg;

    [SerializeField] private Button _plantBtn;
    [SerializeField] private Button _evolutionBtn;
    [SerializeField] private Button _upgradebtn;
    [SerializeField] private Button _awakeningBtn;

    [SerializeField] private TMP_Text _reinForceMaterialTxt;
    [SerializeField] private TMP_Text _reinForceBtnlTxt;
    [SerializeField] private TMP_Text _currentWoodLevelTxt;
    [SerializeField] private TMP_Text _evolutionPriceTxt;
    [SerializeField] private TMP_Text _evolutionGradeTxt;
    [SerializeField] private TMP_Text _plantPriceTxt;
    [SerializeField] private TMP_Text _plantTxt;

    private WorldWoodChart _currentWoodChart;
    private WorldWoodData _currentWoodData;

    private Dictionary<int, UpgradeWoodLog> _woodLogDic = new Dictionary<int, UpgradeWoodLog>();
    private Dictionary<int, WorldWoodAwakneingData> _woodAwakeningDataDic = new Dictionary<int, WorldWoodAwakneingData>();

    private int _currentWoodIndex;
    private int _currentWoodAwakeningStatIndex;

    private bool _isSave;
    public bool IsPlant { get; private set; }

    #endregion

    #region Unity Methods
    private void Start()
    {
        _plantBtn.BindEvent(OnClickPlant);
        _upgradebtn.BindEvent(OnClickReinforce);
        _upgradebtn.BindEvent(OnClickReinforce,UIEvent.Pressed);
        _evolutionBtn.BindEvent(OnClickWakeUp);
        _awakeningBtn.BindEvent(() => _ui_WorldWoodAwakeningPanel.Open());

        _ui_WorldWoodAwakeningPanel.Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Managers.UI.FindPopup<UI_YesNoPopup>() != null)
                return;

            if (_ui_WorldWoodAwakeningSettingPanel.gameObject.activeInHierarchy)
            {
                _ui_WorldWoodAwakeningSettingPanel.gameObject.SetActive(false);
                return;
            }

            if (_ui_WorldWoodAwakeningPanel.gameObject.activeInHierarchy)
            {
                _ui_WorldWoodAwakeningPanel.gameObject.SetActive(false);
                return;
            }
            Managers.UI.ClosePopupUI();
        }
    }

    private void OnDisable()
    {
        if (!_isSave)
        {
            return;
        }

        _isSave = false;

        GameDataManager.GoodsGameData.SaveGameData();
        GameDataManager.WoodGameData.SaveGameData();

        Param param = new Param();

        if (_woodLogDic.Count > 0)
        {
            param.Add($"WoodUpgrade_{_currentWoodData.Id}", _woodLogDic[0]);
            _woodLogDic.Clear();
        }

        if (param.Count > 0)
        {
            Backend.GameLog.InsertLog("WoodLog", param);
        }
    }
    #endregion

    #region Public Methods
    public override void Open()
    {
        base.Open();

        _currentWoodData = Managers.Game.WoodsDatas[0];
        _ui_WorldWoodAwakeningPanel.gameObject.SetActive(false);

        if (_currentWoodData.Id != 0)
        {
            _currentWoodChart = ChartManager.WoodCharts[Managers.Game.WoodsDatas[0].Id];
            SetCurrentWoodObj();
            SetCurrentWoodInfo();
        }

        _plantPannelObj.SetActive(_currentWoodData.Id == 0);
        SetBG();
        AwakeingLock();


        if (_plantPannelObj.activeInHierarchy)
        {
            if (!Utils.IsEnoughItem(ItemType.Goods, 2, ChartManager.WoodCharts[_currentWoodData.Id + 1].GradeUpCost))
            {
                _plantTxt.color = Color.red;
            }
            else
            {
                _plantTxt.color = Color.white;
            }
        }


    }

    public void OnClickPlant()
    {
        if (!Utils.IsEnoughItem(ItemType.Goods, 2, ChartManager.WoodCharts[_currentWoodData.Id + 1].GradeUpCost))
        {
            return;
        }

        _plantPannelObj.SetActive(false);
        _woodLogDic.Clear();
        Managers.Game.DecreaseItem(ItemType.Goods, 2, ChartManager.WoodCharts[_currentWoodData.Id + 1].GradeUpCost);
        _currentWoodData.Id++;
        _currentWoodData.Level = 1;
        GameDataManager.GoodsGameData.SaveGameData();
        GameDataManager.WoodGameData.SaveGameData();
        _currentWoodChart = ChartManager.WoodCharts[_currentWoodData.Id];
        SetCurrentWoodObj();
        SetBG();
        Managers.Game.CalculateStat();
        SetCurrentWoodInfo();

        _isSave = true;
    }

    public void OnClickReinforce()
    {
        double price = Utils.CalculateItemValue(_currentWoodChart.LevelUpItemValue, _currentWoodChart.LevelUpItemIncreaseValue,
            _currentWoodData.Level);

        if (_currentWoodChart.MaxLevel <= _currentWoodData.Level)
        {
            return;
        }
        if (!Utils.IsEnoughItem(ItemType.Goods, _currentWoodChart.LevelUpItemId, price))
        {
            return;
        }

        if (_woodLogDic.ContainsKey(0))
        {
            _woodLogDic[0].UsingGoldbar += price;
            _woodLogDic[0].IncreaseLv += 1;
        }
        else
        {
            _woodLogDic.Add(0, new UpgradeWoodLog());
            _woodLogDic[0].UsingGoldbar = price;
            _woodLogDic[0].IncreaseLv = 1;
        }

        _isSave = true;
        _currentWoodData.Level++;

        Managers.Game.DecreaseItem(ItemType.Goods, _currentWoodChart.LevelUpItemId, price);
        SetCurrentWoodInfo();
        Managers.Game.CalculateStat();
    }

    public void OnClickWakeUp()
    {
        if (_currentWoodData.Id > 5)
        {
            return;
        }
        if (!Utils.IsEnoughItem(ItemType.Goods, 2, ChartManager.WoodCharts[_currentWoodData.Id + 1].GradeUpCost))
        {
            return;
        }
        if (_currentWoodData.Level < _currentWoodChart.MaxLevel)
        {
            return;
        }
        _isSave = true;

        _plantPannelObj.SetActive(false);
        _woodLogDic.Clear();
        Managers.Game.DecreaseItem(ItemType.Goods, 2, ChartManager.WoodCharts[_currentWoodData.Id + 1].GradeUpCost);
        _currentWoodData.Id++;
        _currentWoodData.Level = 1;
        GameDataManager.GoodsGameData.SaveGameData();
        GameDataManager.WoodGameData.SaveGameData();
        _currentWoodChart = ChartManager.WoodCharts[_currentWoodData.Id];
        SetCurrentWoodObj();
        SetBG();
        Managers.Game.CalculateStat();
        SetCurrentWoodInfo();
    }

    public void C9ActiveOff(bool isOn)
    {
        _chulguObj.gameObject.SetActive(isOn);
        _worldWoodObjs[_currentWoodData.Id - 1].gameObject.SetActive(isOn);
    }

    public void AwakeingLock()
    {
        for (int i = 0; i < _awakeningTxtObjs.Length; i++)
        {
            _awakeningTxtObjs[i].SetActive(Managers.Game.WoodAwakeningDatas[i + 1].StatId == 0);
        }
    }

    public void AddAwakeningDic(int awakeningId, WorldWoodAwakneingData data)
    {
        if (!_woodAwakeningDataDic.ContainsKey(awakeningId))
        {
            _woodAwakeningDataDic.Add(awakeningId, data);
        }
        else
        {
            _woodAwakeningDataDic[awakeningId] = data;
        }
    }
    #endregion

    #region Private Methods
    private void SetCurrentWoodObj()
    {
        if (_currentWoodData == null)
        {
            return;
        }

        _chulguObj.gameObject.SetActive(false);

        for (int i = 0; i < _worldWoodObjs.Length; i++)
        {
            _worldWoodObjs[i].gameObject.SetActive(false);
        }

        _worldWoodObjs[_currentWoodData.Id - 1].gameObject.SetActive(true);

        if (_currentWoodData.Id > 0 && _currentWoodData.Id <= 6)
        {
            _chulguObj.gameObject.SetActive(true);
        }
    }

    public void SetCurrentWoodInfo()
    {
        int currentWoodUpgradeLevel = Math.Max(_currentWoodData.Level - 1, 0);

        _gradeStatEffectItem1.Init(_currentWoodChart.GradeStatType1, _currentWoodChart.GradeStatValue1 + _currentWoodChart.GradeStatIncreaseValue1 * currentWoodUpgradeLevel);
        _gradeStatEffectItem2.Init(_currentWoodChart.GradeStatType2, _currentWoodChart.GradeStatValue2 + _currentWoodChart.GradeStatIncreaseValue2 * currentWoodUpgradeLevel);
        _gradeStatEffectItem3.Init(_currentWoodChart.GradeStatType3, _currentWoodChart.GradeStatValue3 + _currentWoodChart.GradeStatIncreaseValue3 * currentWoodUpgradeLevel);
        _gradeStatEffectItem4.Init(_currentWoodChart.GradeStatType4, _currentWoodChart.GradeStatValue4 + _currentWoodChart.GradeStatIncreaseValue4 * currentWoodUpgradeLevel);


        _wakeUpStatEffectItem1.Init(Managers.Game.WoodAwakeningDatas[1].StatId, Managers.Game.WoodAwakeningDatas[1].StatValue);
        _wakeUpStatEffectItem2.Init(Managers.Game.WoodAwakeningDatas[2].StatId, Managers.Game.WoodAwakeningDatas[2].StatValue);
        _wakeUpStatEffectItem3.Init(Managers.Game.WoodAwakeningDatas[3].StatId, Managers.Game.WoodAwakeningDatas[3].StatValue);
        _wakeUpStatEffectItem4.Init(Managers.Game.WoodAwakeningDatas[4].StatId, Managers.Game.WoodAwakeningDatas[4].StatValue);
        _wakeUpStatEffectItem5.Init(Managers.Game.WoodAwakeningDatas[5].StatId, Managers.Game.WoodAwakeningDatas[5].StatValue);

        double haveItemValue = Utils.GetItemValue(ItemType.Goods, _currentWoodChart.LevelUpItemId);
        double needItemValue = Utils.CalculateItemValue(_currentWoodChart.LevelUpItemValue, _currentWoodChart.LevelUpItemIncreaseValue, _currentWoodData.Level);

        string materialText = _currentWoodData.IsAcquired && _currentWoodData.Level < _currentWoodChart.MaxLevel
            ? $"({haveItemValue.ToCurrencyString()}/{needItemValue.ToCurrencyString()})"
            : string.Empty;

        _reinForceMaterialTxt.text = _currentWoodData.Level >= _currentWoodChart.MaxLevel ? string.Empty : materialText;
        _reinForceMaterialTxt.color = haveItemValue >= needItemValue ? Color.white : Color.red;
        _currentWoodLevelTxt.text = $"(Lv.{_currentWoodData.Level}/{_currentWoodChart.MaxLevel})";

        if (_currentWoodData.Level >= _currentWoodChart.MaxLevel)
        {
            _currentWoodData.Level = _currentWoodChart.MaxLevel;
            if (_currentWoodData.Id > 5)
            {
                _evolutionLockObj.SetActive(true);
                _evolutionGoodsObj.SetActive(false);
                _evolutionGradeTxt.text = string.Empty;
                _evolutionGradeTxt.color = Color.white;
                _reinForceBtnlTxt.text = "Max";
                _reinForceBtnlTxt.color = Color.red;
                _evolutionPriceTxt.text = "Max 등급 달성";
                _evolutionPriceTxt.color = Color.white;
            }
            else
            {
                _evolutionLockObj.SetActive(false);
                _evolutionGoodsObj.SetActive(true);
                _evolutionGradeTxt.color = Color.white;
                _evolutionGradeTxt.text = ChartManager.GetString(ChartManager.WoodCharts[_currentWoodData.Id + 1].Grade.ToString()) + " 진화";
                _evolutionPriceTxt.text = ChartManager.WoodCharts[_currentWoodData.Id + 1].GradeUpCost.ToString("N0");
                
                _evolutionPriceTxt.color = Utils.IsEnoughItem(ItemType.Goods,2, ChartManager.WoodCharts[_currentWoodData.Id + 1].GradeUpCost) ? Color.white : Color.red;
                _reinForceBtnlTxt.text = "Max";
                _reinForceBtnlTxt.color = Color.red;
            }
        }
        else
        {
            _evolutionLockObj.SetActive(true);
            _evolutionGoodsObj.SetActive(false);
            _evolutionPriceTxt.color = Color.white;
            _evolutionPriceTxt.text = _currentWoodData.Id > 5 ? "Max 등급 달성" : ChartManager.GetString(_currentWoodChart.Grade.ToString()) + " 최대레벨 달성 시";
            _evolutionGradeTxt.text = string.Empty;
            _reinForceBtnlTxt.text = "강  화";
            _reinForceBtnlTxt.color = Color.white;
        }
    }

    private void SetBG()
    {
        switch (_currentWoodData.Id)
        {
            case 0:
                break;
            case 1:
                _floorImg.rectTransform.localScale = new Vector3(1.1f, 1.1f, 1);
                _chulguObj.localScale = new Vector3(70, 70, 1);
                _chulguObj.anchoredPosition = new Vector3(130, -340, 0);
                break;
            case 2:
                _floorImg.rectTransform.localScale = new Vector3(1.1f, 1.1f, 1);
                _chulguObj.localScale = new Vector3(70, 70, 1);
                _chulguObj.anchoredPosition = new Vector3(130, -340, 0);
                break;
            case 3:
                _floorImg.rectTransform.localScale = new Vector3(1, 1, 1);
                _chulguObj.localScale = new Vector3(50, 50, 1);
                _chulguObj.anchoredPosition = new Vector3(130, -340, 0);
                break;
            case 4:
                _floorImg.rectTransform.localScale = new Vector3(0.9f, 0.9f, 1);
                _chulguObj.localScale = new Vector3(40, 40, 1);
                _chulguObj.anchoredPosition = new Vector3(130, -340, 0);
                break;
            case 5:
                _floorImg.rectTransform.localScale = new Vector3(0.8f, 0.8f, 1);
                _chulguObj.localScale = new Vector3(30, 30, 1);
                _chulguObj.anchoredPosition = new Vector3(130, -340, 0);
                break;
            case 6:
                _floorImg.rectTransform.localScale = new Vector3(0.6f, 0.6f, 1);
                _chulguObj.localScale = new Vector3(20, 20, 1);
                _chulguObj.anchoredPosition = new Vector3(120, -415, 0);
                break;
        }
    }
    #endregion
}
