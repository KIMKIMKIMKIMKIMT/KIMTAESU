using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : MonoBehaviour
{
    #region Fields
    [SerializeField] private ChpaterScrollView _chapterScrollView;
    [SerializeField] private TankSelectionScrollView _tankSelectionScrollView;
    [SerializeField] private SettingPopup _settingPopup;
    [SerializeField] private DailyQuestPopup _dailyQuestPopup;
    [SerializeField] private EquipmentStatusPopup _equipmentStatusPopup;
    [SerializeField] private FusionPopup _fusionPopup;
    [SerializeField] private LevelUpPopup _levelUpPopup;

    [SerializeField] public RankPopup _rankPopup;
    [SerializeField] public BoxOpenPopup _boxOpenPopup;
    #endregion

    #region Public Methods
    public void ShowChapterSelection()
    {
        PopupMgr.Instance.AddPopup(_chapterScrollView.gameObject);
    }
    public void ShowTankSelection()
    {
        PopupMgr.Instance.AddPopup(_tankSelectionScrollView.gameObject);
    }
    public void ShowSettingPopup()
    {
        PopupMgr.Instance.AddPopup(_settingPopup.gameObject);
    }
    public void ShowDailyQuestPopup()
    {
        PopupMgr.Instance.AddPopup(_dailyQuestPopup.gameObject);
        _dailyQuestPopup.ShowQuestState();
    }
    public void ShowEquipmentStatusPopup(eATKWEAPON data, int index, bool current)
    {
        PopupMgr.Instance.AddPopup(_equipmentStatusPopup.gameObject);
        _equipmentStatusPopup.SetEquipmentStatus(data, index, current);
    }
    public void ShowEquipmentStatusPopup(eHPEQUIP data, int index, bool current)
    {
        PopupMgr.Instance.AddPopup(_equipmentStatusPopup.gameObject);
        _equipmentStatusPopup.SetEquipmentStatus(data, index, current);
    }
    public void ShowRankPopup()
    {
        PopupMgr.Instance.AddPopup(_rankPopup.gameObject);
    }
    public void ShowFusionPopup()
    {
        PopupMgr.Instance.AddPopup(_fusionPopup.gameObject);
    }
    public void ShowBoxOpenPopup(eBOX_TYPE type)
    {
        PopupMgr.Instance.AddPopup(_boxOpenPopup.gameObject);
        _boxOpenPopup.SetBoxSprite(type);
    }
    public void ShowLevelUpPopup()
    {
        PopupMgr.Instance.AddPopup(_levelUpPopup.gameObject);
    }
    #endregion
}
