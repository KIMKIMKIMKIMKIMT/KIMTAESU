using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using TMPro;

public class UI_WorldWoodAwakeningPanel : UI_Panel
{
    #region Fields
    [SerializeField] private UI_AwakeningStatValueItem _item;
    [SerializeField] private UI_WorldWoodPannel _ui_WorldWoodPannel;
    [SerializeField] private UI_Panel _awakeningSettingPanel;

    [SerializeField] private RectTransform _valueGrid;

    [SerializeField] private TMP_Text[] _gradeRateTxt;
    [SerializeField] private TMP_Text[] _awakeningResetTxts;

    [SerializeField] private Button _settingBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _sidetouchCloseBtn;

    private int _chanceIndex;

    private List<UI_AwakeningStatValueItem> _itemList = new List<UI_AwakeningStatValueItem>();
    private Dictionary<int, UpgradeWoodAwakeningLog> _woodAwakeningLog = new Dictionary<int, UpgradeWoodAwakeningLog>();
    #endregion

    #region Unity Methods
    private void Start()
    {
        _closeBtn.BindEvent(Close);
        _sidetouchCloseBtn.BindEvent(Close);
        _settingBtn.BindEvent(() => _awakeningSettingPanel.Open());

        _chanceIndex = 0;

        foreach (var data in ChartManager.WoodGradeRateCharts.Values)
        {
            _gradeRateTxt[_chanceIndex].text = (data.Rate * 100).ToCurrencyString()+"%";
            _chanceIndex++;
        }

        foreach (var statData in ChartManager.WoodStatDataCharts.Values)
        {
            var item = Managers.Resource.Instantiate(_item.gameObject, _valueGrid).GetComponent<UI_AwakeningStatValueItem>();
            item.gameObject.SetActive(true);
            item.Init(statData.Grade, statData.StatId, statData.MinValue, statData.MaxValue);
            _itemList.Add(item);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_valueGrid);
    }

    private void OnDisable()
    {
        _ui_WorldWoodPannel.C9ActiveOff(true);
        _ui_WorldWoodPannel.SetCurrentWoodInfo();
        _ui_WorldWoodPannel.AwakeingLock();

        GameDataManager.WoodGameData.SaveGameData();
        GameDataManager.GoodsGameData.SaveGameData();
    }
    #endregion

    #region Public Methods
    public override void Open()
    {
        base.Open();

        _ui_WorldWoodPannel.C9ActiveOff(false);
    }

    public void ResetButtonRefesh()
    {
        for (int i = 0; i < _awakeningResetTxts.Length; i++)
        {
            if (Managers.Game.WoodAwakeningDatas[i + 1].StatId == 0)
            {
                if (Managers.Game.WoodsDatas[0].Id <= i + 1)
                {
                    _awakeningResetTxts[i].color = Color.red;
                    continue;
                }
                    

                if (!Utils.IsEnoughItem(ItemType.Goods, 2, ChartManager.WoodAwakeningDataCharts[i + 1].OpenCost))
                {
                    _awakeningResetTxts[i].color = Color.red;
                }
                else
                {
                    _awakeningResetTxts[i].color = Color.white;
                }
            }
            else
            {
                if (!Utils.IsEnoughItem(ItemType.Goods, 2, ChartManager.WoodAwakeningDataCharts[i + 1].AwakeningCost))
                {
                    _awakeningResetTxts[i].color = Color.red;
                }
                else
                {
                    _awakeningResetTxts[i].color = Color.white;
                }
            }
        }
    }
    #endregion

    #region Private Methods
    #endregion
}
