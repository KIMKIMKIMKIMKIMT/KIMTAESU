using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UniRx;
using UnityEngine.UI;

public class UI_WorldWoodAwakeningSettingPanel : UI_Panel
{
    #region Fields
    [SerializeField] private UI_WorldWoodAwakeningSettingStatItem _awakeningSettingStatItem;
    [SerializeField] private UI_WorldWoodAwakeningSettingGradeItem _awakeningSettingGradeItem;

    [SerializeField] private Transform _statItemGrid;
    [SerializeField] private Transform _gradeItemGrid;

    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _sideTouchCloseBtn;

    private List<UI_WorldWoodAwakeningSettingStatItem> _awakeningSettingStatItemList = new();
    private List<UI_WorldWoodAwakeningSettingGradeItem> _awakeningSettingGradeItemList = new();
    #endregion

    #region Unity Methods
    private void Start()
    {
        _closeBtn.BindEvent(Close);
        _sideTouchCloseBtn.BindEvent(Close);

        SetStatItem();
        SetGradeItem();

        MessageBroker.Default.Receive<WoodAwakeningSettingMessage>().Subscribe(message =>
        {
            switch (message.Type)
            {
                case WoodAwakeningSettingMessageType.Stat:
                    {
                        if (Managers.Game.WorldWoodAwakeningSettingData.StatId.Value == message.Value)
                            Managers.Game.WorldWoodAwakeningSettingData.StatId.Value = -1;
                        else
                            Managers.Game.WorldWoodAwakeningSettingData.StatId.Value = message.Value;

                    }
                    break;

                case WoodAwakeningSettingMessageType.Grade:
                    {
                        if (Managers.Game.WorldWoodAwakeningSettingData.Grade.Value == message.Value)
                            Managers.Game.WorldWoodAwakeningSettingData.Grade.Value = -1;
                        else
                            Managers.Game.WorldWoodAwakeningSettingData.Grade.Value = message.Value;
                    }
                    break;

            }
        });
    }

    private void OnDisable()
    {
        Managers.Game.WorldWoodAwakeningSettingData.Save();
    }
    #endregion

    #region Public Methods
    public override void Open()
    {
        base.Open();
    }
    #endregion

    #region Private Methods
    private void SetStatItem()
    {
        foreach (var data in ChartManager.WoodStatRateCharts.Values)
        {
            GameObject obj = Managers.Resource.Instantiate(_awakeningSettingStatItem.gameObject, _statItemGrid);
            if (obj == null)
                continue;

            var uiWoodAwakeningSettingStatItem = obj.GetComponent<UI_WorldWoodAwakeningSettingStatItem>();
            if (uiWoodAwakeningSettingStatItem == null)
                continue;

            uiWoodAwakeningSettingStatItem.gameObject.SetActive(true);
            uiWoodAwakeningSettingStatItem.Init(data.StatId);

            _awakeningSettingStatItemList.Add(uiWoodAwakeningSettingStatItem);
        }
    }

    private void SetGradeItem()
    {
        foreach (var data in ChartManager.WoodGradeRateCharts.Values)
        {
            GameObject obj = Managers.Resource.Instantiate(_awakeningSettingGradeItem.gameObject, _gradeItemGrid);
            if (obj == null)
                continue;

            var uiWoodAwakeningSettingGradeItem = obj.GetComponent<UI_WorldWoodAwakeningSettingGradeItem>();
            if (uiWoodAwakeningSettingGradeItem == null)
                continue;

            uiWoodAwakeningSettingGradeItem.gameObject.SetActive(true);
            uiWoodAwakeningSettingGradeItem.Init(data.Grade);

            _awakeningSettingGradeItemList.Add(uiWoodAwakeningSettingGradeItem);
        }
    }
    #endregion
}
