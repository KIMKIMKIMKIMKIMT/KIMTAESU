using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using TMPro;

public class UI_CostumCollectionPanel : UI_Panel
{
    #region Fields
    

    [SerializeField] private RectTransform _itemGrid;

    [SerializeField] private UI_Panel _quesionPanel;

    [SerializeField] private UI_CostumCollectItem _item;

    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _quesionBtn;

    [SerializeField] private TMP_Text _setEffectValue;

    public int SetCount;

    private List<UI_CostumCollectItem> _itemList = new List<UI_CostumCollectItem>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _closeBtn.BindEvent(Close);
        _quesionBtn.BindEvent(() => _quesionPanel.Open());
        _itemGrid.DestroyInChildren();

        foreach (var item in ChartManager.CostumCollectionCharts.Values)
        {
            var obj = Instantiate(_item, _itemGrid);

            var ui_CostumeCollectionItem = obj.GetComponent<UI_CostumCollectItem>();
            ui_CostumeCollectionItem.gameObject.SetActive(true);

            ui_CostumeCollectionItem.Init(item.SetName, item.Id, item.RequireCostumes, item.Icons, item.StatId, item.StatValue);
            _itemList.Add(ui_CostumeCollectionItem);
        }
    }
    #endregion

    #region Public Methods
    public override void Open()
    {
        base.Open();

        int index = 0;

        foreach (var item in ChartManager.CostumCollectionCharts.Values)
        {
            _itemList[index].Init(item.SetName, item.Id, item.RequireCostumes, item.Icons, item.StatId, item.StatValue);
            index++;
        }

        _setEffectValue.text = $"{Managers.CostumeSet.SetCount * 10}%";
    }
    #endregion

    #region Private Methods
    #endregion
}
