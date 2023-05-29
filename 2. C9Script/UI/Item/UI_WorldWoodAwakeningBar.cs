using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using Chart;
using System;
using UniRx;

public class UI_WorldWoodAwakeningBar : UI_Base
{
    #region Fields
    private List<WoodStatRateChart> _woodStatRateList = new();
    private List<WoodGradeRateChart> _woodGradeRateList = new();
    private Dictionary<(int, Grade), (float, float)> _statValueDic = new();

    private readonly CompositeDisposable _saveComposite = new();

    private WorldWoodAwakneingData _awakeningData = new WorldWoodAwakneingData();
    [SerializeField] private UI_WorldWoodAwakeningPanel _ctrl;

    [SerializeField] private Button _awakeningBtn;

    [SerializeField] private GameObject _lockObj;
    [SerializeField] private Image _statImg;
    [SerializeField] private TMP_Text _gradeTxt;
    [SerializeField] private TMP_Text _effectTxt;
    [SerializeField] private TMP_Text _effectValueTxt;
    [SerializeField] private TMP_Text _btnPriceTxt;
    [SerializeField] private TMP_Text _btnChangeTxt;

    [SerializeField] private int _awakeningId;

    private bool _isFlag;
    private bool _isMaxGrade;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        _awakeningData = Managers.Game.WoodAwakeningDatas[_awakeningId];
        WoodGradeSet();
        SetAwakeBar();
        _ctrl.ResetButtonRefesh();
    }

    private void Start()
    {
        _woodStatRateList = ChartManager.WoodStatRateCharts.Values.ToList().OrderBy(data => data.Rate).ToList();
        _woodGradeRateList = ChartManager.WoodGradeRateCharts.Values.ToList().OrderBy(data => data.Rate).ToList();


        foreach (var data in ChartManager.WoodStatDataCharts.Values)
        {
            _statValueDic.TryAdd((data.StatId, data.Grade), ((float)data.MinValue, (float)data.MaxValue));
        }

        _awakeningBtn.BindEvent(OnClickAwakeningReset);
        _awakeningBtn.BindEvent(OnClickAwakeningReset,UIEvent.Pressed);
        _awakeningBtn.BindEvent(ReservationSave,UIEvent.PointerUp);
        _awakeningBtn.BindEvent(() =>
            Observable.TimerFrame(1).Subscribe(_ => Managers.Game.CalculateStat()), UIEvent.PointerUp);
    }
    #endregion


    #region Public Methods

    public void OnClickAwakeningReset()
    {
        if (_awakeningData.StatId == 0)
            FirstAwakening();
        else
            AwakeningReset();

    }
    #endregion

    #region Private Methods
    private void WoodGradeSet()
    {
        if (Managers.Game.WoodsDatas[0].Id > _awakeningId)
        {
            _lockObj.SetActive(Managers.Game.WoodAwakeningDatas[_awakeningId].StatId == 0);
            
        }
            
        
    }

    private void SetAwakeBar()
    {
        _lockObj.SetActive(Managers.Game.WoodAwakeningDatas[_awakeningId].StatId == 0);
        _btnPriceTxt.text = Managers.Game.WoodAwakeningDatas[_awakeningId].StatId == 0 ?
            ChartManager.WoodAwakeningDataCharts[_awakeningId].OpenCost.ToString("N0")
            : ChartManager.WoodAwakeningDataCharts[_awakeningId].AwakeningCost.ToString("N0");

        _btnChangeTxt.text = Managers.Game.WoodAwakeningDatas[_awakeningId].StatId == 0 ?
            "해금하기" : "조정하기";

        if (Managers.Game.WoodAwakeningDatas[_awakeningId].StatId == 0)
            return;

        _gradeTxt.text = $"[{ChartManager.GetString(_awakeningData.Grade.ToString())}]";
        _statImg.sprite = Managers.Resource.LoadStatIcon(ChartManager.StatCharts[_awakeningData.StatId].Icon);
        _effectTxt.text = ChartManager.GetString(ChartManager.StatCharts[_awakeningData.StatId].Name);
        _effectValueTxt.text = ChartManager.StatCharts[_awakeningData.StatId].ValueType == ValueType.Percent
            ? $"{Math.Round(_awakeningData.StatValue * 100, 7).ToCurrencyString()}%"
            : Math.Round(_awakeningData.StatValue, 7).ToCurrencyString();
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

        if (!Managers.Game.WoodAwakeningDatas.ContainsKey(_awakeningId))
        {
            Managers.Game.WoodAwakeningDatas.Add(_awakeningId, _awakeningData);
        }
        else
        {
            Managers.Game.WoodAwakeningDatas[_awakeningId] = _awakeningData;
        }

        GameDataManager.WoodGameData.SaveGameData();
        GameDataManager.GoodsGameData.SaveGameData();
        
    }

    private void AwakeningReset()
    {
        if (_isFlag || _isMaxGrade)
            return;

        if (!Utils.IsEnoughItem(ItemType.Goods, 2, ChartManager.WoodAwakeningDataCharts[_awakeningId].AwakeningCost))
        {
            Managers.Message.ShowMessage("재화가 부족합니다.");
            return;
        }

        bool isCheckFlag = false;
        bool isMaxFlag = false;

        int checkCount = 0;
        int flagCount = 0;

        if (Managers.Game.WorldWoodAwakeningSettingData.StatId.Value != -1)
            checkCount += 1;

        if (Managers.Game.WorldWoodAwakeningSettingData.Grade.Value != -1)
            checkCount += 1;

        if (checkCount != 0)
        {
            if (Managers.Game.WorldWoodAwakeningSettingData.StatId.Value == _awakeningData.StatId)
                flagCount += 1;

            if (Managers.Game.WorldWoodAwakeningSettingData.Grade.Value != -1)
            {
                if (Managers.Game.WorldWoodAwakeningSettingData.Grade.Value <= (int)_awakeningData.Grade)
                    flagCount += 1;
            }

                

            if (flagCount >= checkCount)
                isCheckFlag = true;
        }

        if (_awakeningData.Grade == Grade.Godgod)
            isMaxFlag = true;

        if (isCheckFlag)
        {
            _isFlag = true;
            var popup = Managers.UI.ShowPopupUI<UI_YesNoPopup>();
            popup.Init("설정 된 옵션 또는 더 높은 등급의\n각성 효과가 포함되어 있습니다.\n계속 진행 하시겠습니까?", () =>
            {
                Managers.UI.ClosePopupUI();
                _isFlag = false;
                Reset();
            }, () =>
            {
                Managers.UI.ClosePopupUI();
                _isFlag = false;
            });
        }
        else if (isMaxFlag)
        {
            _isMaxGrade = true;
            var popup = Managers.UI.ShowPopupUI<UI_YesNoPopup>();
            popup.Init("가장 높은 등급의 각성 효과가 포함되어 있습니다.\n계속 진행 하시겠습니까?", () =>
            {
                Managers.UI.ClosePopupUI();
                _isMaxGrade = false;
                Reset();
            }, () =>
            {
                Managers.UI.ClosePopupUI();
                _isMaxGrade = false;
            });
        }
        else
            Reset();


        void Reset()
        {
            _awakeningData.StatId = GetRandomStat();
            _awakeningData.Grade = GetRandomGrade();
            _awakeningData.StatValue = UnityEngine.Random.Range(_statValueDic[(_awakeningData.StatId, _awakeningData.Grade)].Item1, _statValueDic[(_awakeningData.StatId, _awakeningData.Grade)].Item2);

            _gradeTxt.text = $"[{ChartManager.GetString(_awakeningData.Grade.ToString())}]";
            _statImg.sprite = Managers.Resource.LoadStatIcon(ChartManager.StatCharts[_awakeningData.StatId].Icon);
            _effectTxt.text = ChartManager.GetString(ChartManager.StatCharts[_awakeningData.StatId].Name);
            _effectValueTxt.text = ChartManager.StatCharts[_awakeningData.StatId].ValueType == ValueType.Percent
                ? $"{Math.Round(_awakeningData.StatValue * 100, 7).ToCurrencyString()}%"
                : Math.Round(_awakeningData.StatValue, 7).ToCurrencyString();

            Managers.Game.DecreaseItem(ItemType.Goods, 2, ChartManager.WoodAwakeningDataCharts[_awakeningId].AwakeningCost);

            _ctrl.ResetButtonRefesh();
        }
        
    }

    private void FirstAwakening()
    {
        if (Managers.Game.WoodsDatas[0].Id <= _awakeningId)
        {
            Managers.Message.ShowMessage("세계수 진화 후 해금 가능합니다.");
            return;
        }
        if (!Utils.IsEnoughItem(ItemType.Goods, 2, ChartManager.WoodAwakeningDataCharts[_awakeningId].OpenCost))
        {
            Managers.Message.ShowMessage("재화가 부족합니다.");
            return;
        }

        _awakeningData.StatId = GetRandomStat();
        _awakeningData.Grade = GetRandomGrade();
        _awakeningData.StatValue = UnityEngine.Random.Range(_statValueDic[(_awakeningData.StatId, _awakeningData.Grade)].Item1, _statValueDic[(_awakeningData.StatId, _awakeningData.Grade)].Item2);

        _gradeTxt.text = $"[{ChartManager.GetString(_awakeningData.Grade.ToString())}]";
        _statImg.sprite = Managers.Resource.LoadStatIcon(ChartManager.StatCharts[_awakeningData.StatId].Icon);
        _effectTxt.text = ChartManager.GetString(ChartManager.StatCharts[_awakeningData.StatId].Name);
        _effectValueTxt.text = ChartManager.StatCharts[_awakeningData.StatId].ValueType == ValueType.Percent
            ? $"{Math.Round(_awakeningData.StatValue * 100, 7).ToCurrencyString()}%"
            : Math.Round(_awakeningData.StatValue, 7).ToCurrencyString();

        Managers.Game.DecreaseItem(ItemType.Goods, 2, ChartManager.WoodAwakeningDataCharts[_awakeningId].OpenCost);
        GameDataManager.WoodGameData.SaveGameData();
        GameDataManager.GoodsGameData.SaveGameData();

        SetAwakeBar();
        _ctrl.ResetButtonRefesh();
    }

    private int GetRandomStat()
    {
        double randNum = UnityEngine.Random.value;
        double totalNum = 0;

        foreach (var data in _woodStatRateList)
        {
            totalNum += data.Rate;

            if (randNum <= totalNum)
                return data.StatId;
        }

        return 0;
    }

    private Grade GetRandomGrade()
    {
        double randNum = UnityEngine.Random.value;
        double totalNum = 0;

        foreach (var data in _woodGradeRateList)
        {
            totalNum += data.Rate;

            if (randNum <= totalNum)
                return data.Grade;
        }

        return Grade.Normal;
    }
    #endregion

}
