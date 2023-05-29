using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using Newtonsoft.Json;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;

public class UI_SkillPopup : UI_Popup
{
    [Serializable]
    record SkillTab
    {
        [FormerlySerializedAs("SkillType")] public SkillTabType skillTabType;
        public Button TabButton;
        public GameObject OnObj;
        public GameObject OffObj;

        public void Open()
        {
            OnObj.SetActive(true);
            OffObj.SetActive(false);
        }

        public void Close()
        {
            OnObj.SetActive(false);
            OffObj.SetActive(true);
        }
    }

    [SerializeField] private SkillTab[] SkillTabs;
    [SerializeField] private RectTransform SkillItemContentTr;
    [SerializeField] private TMP_Text SkillEnhancementStoneText;
    [SerializeField] private Button ResetButton;

    [SerializeField] private UI_EquipSkillPanel EquipSkillPanel;
    [SerializeField] private UI_Panel ResetSkillPanel;

    [SerializeField] private GameObject SkillScrollObj;
    [SerializeField] private UI_Panel UILabResearchPanel;

    [SerializeField] private GameObject SkillOpenNavigationObj;
    [SerializeField] private GameObject EquipNavigationObj;
    [SerializeField] private GameObject EquipSlotNavigationObj;
    [SerializeField] private GameObject PassiveNavigationObj;
    
    private readonly CompositeDisposable _compositeDisposable = new();

    private readonly List<UI_SkillItem> _uiSkillItems = new();
    private SkillTab _currentTab;

    [HideInInspector] public List<int> OpenSkillLog = new();

    public readonly Dictionary<int, UpgradeSkillLog> UpgradeSkillLogs = new();

    private CompositeDisposable _guideComposite = new();

    private SkillTab CurrentTab
    {
        get => _currentTab;
        set
        {
            if (_currentTab != null)
            {
                _currentTab.Close();
                if (_currentTab.skillTabType == SkillTabType.Lab)
                    UILabResearchPanel.Close();
            }

            _currentTab = value;
            _currentTab.Open();
            if (_currentTab.skillTabType == SkillTabType.Lab)
            {
                ResetButton.gameObject.SetActive(false);
                SkillScrollObj.SetActive(false);
                UILabResearchPanel.Open();
            }
            else
            {
                ResetButton.gameObject.SetActive(true);
                SkillScrollObj.SetActive(true);
                MakeSkillItems();
            }
            
            SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);
        }
    }

    private void Start()
    {
        for (int i = 0; i < SkillTabs.Length; i++)
        {
            int index = i;
            SkillTabs[i].TabButton.BindEvent(() => CurrentTab = SkillTabs[index]);
            SkillTabs[i].Close();
        }

        CurrentTab = SkillTabs[0];

        EquipSkillPanel.Close();
        ResetSkillPanel.Close();
        UILabResearchPanel.Close();

        ResetButton.BindEvent(() => { ResetSkillPanel.Open(); });

        SetGuideEvent();
    }

    private void SetGuideEvent()
    {
        if (Utils.IsAllClearGuideQuest())
            return;
        
        Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigation).AddTo(_guideComposite);
        SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);
        Managers.Game.UserData.OnChangeGuideQuestProgressValue.Subscribe(SetNavigationValue).AddTo(_guideComposite);
    }
    
    void SetNavigation(int id)
    {
        if (Utils.IsAllClearGuideQuest())
        {
            _guideComposite.Clear();
            SkillOpenNavigationObj.SetActive(false);
            EquipNavigationObj.SetActive(false);
            EquipSlotNavigationObj.SetActive(false);
            PassiveNavigationObj.SetActive(false);
            return;
        }
        
        SkillOpenNavigationObj.SetActive((CurrentTab.skillTabType == SkillTabType.Active && id == 7) || (
            CurrentTab.skillTabType == SkillTabType.Passive && id == 11));
        EquipNavigationObj.SetActive(CurrentTab.skillTabType == SkillTabType.Active && id == 8);
        EquipSlotNavigationObj.SetActive(CurrentTab.skillTabType == SkillTabType.Active && id == 8);
        PassiveNavigationObj.SetActive(CurrentTab.skillTabType != SkillTabType.Passive && id == 11);
    }

    private void SetNavigationValue(long value)
    {
        if (!Utils.IsCompleteGuideQuest())
            return;
        
        SkillOpenNavigationObj.SetActive(false);
        EquipNavigationObj.SetActive(false);
        EquipSlotNavigationObj.SetActive(false);
        PassiveNavigationObj.SetActive(false);
    }

    private void OnEnable()
    {
        Managers.Game.GoodsDatas[(int)Goods.SkillEnhancementStone].Subscribe(quantity =>
        {
            SkillEnhancementStoneText.text = quantity.ToCurrencyString();
        }).AddTo(_compositeDisposable);
    }

    private void OnDisable()
    {
        _compositeDisposable.Clear();

        if (EquipSkillPanel.isChangeEquipSkill)
        {
            EquipSkillPanel.isChangeEquipSkill = false;
            GameDataManager.SkillGameData.SaveEquipSkillData();
        }

        if (_uiSkillItems.Find(uiSkillItem => uiSkillItem.IsSave) != null)
        {
            GameDataManager.GoodsGameData.SaveGameData();
            GameDataManager.SkillGameData.SaveGameData();
        }

        Param param = new();

        if (OpenSkillLog.Count > 0)
        {
            param.Add("OpenSkills", JsonConvert.SerializeObject(OpenSkillLog));
            OpenSkillLog.Clear();
        }

        if (UpgradeSkillLogs.Count > 0)
        {
            foreach (var upgradeSkillLog in UpgradeSkillLogs)
                param.Add($"UpgradeSkill_{upgradeSkillLog.Key}", upgradeSkillLog.Value);
            UpgradeSkillLogs.Clear();
        }

        if (param.Count > 0)
            Backend.GameLog.InsertLog("SkillLog", param);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UILabResearchPanel.gameObject.activeSelf)
                return;

            if (EquipSkillPanel.gameObject.activeSelf)
            {
                EquipSkillPanel.Close();
                return;
            }

            if (ResetSkillPanel.gameObject.activeSelf)
            {
                ResetSkillPanel.Close();
                return;
            }

            ClosePopup();
        }
    }

    public override void Open()
    {
        base.Open();

        if (CurrentTab != null)
        {
            CurrentTab = CurrentTab;
            MakeSkillItems();
        }
        
        EquipSkillPanel.Close();
        ResetSkillPanel.Close();
    }

    private void MakeSkillItems()
    {
        if (_uiSkillItems.Count <= 0)
            SkillItemContentTr.DestroyInChildren();

        _uiSkillItems.ForEach(uiSkillItem => uiSkillItem.gameObject.SetActive(false));

        int childIndex = 0;

        var uiSkillItems = _uiSkillItems.ToList();

        foreach (var skillChart in ChartManager.SkillCharts.Values)
        {
            if (skillChart.TabType != _currentTab.skillTabType)
                continue;

            UI_SkillItem uiSkillItem;

            if (uiSkillItems.Count > childIndex)
                uiSkillItem = uiSkillItems[childIndex++];
            else
            {
                uiSkillItem = Managers.UI.MakeSubItem<UI_SkillItem>(SkillItemContentTr);
                _uiSkillItems.Add(uiSkillItem);
            }

            uiSkillItem.UISkillPopup = this;
            uiSkillItem.gameObject.SetActive(true);
            uiSkillItem.Init(skillChart.Id, OnClickItemCallback);
        }

        float x = SkillItemContentTr.anchoredPosition.x;
        SkillItemContentTr.anchoredPosition = new Vector3(x, 0, 0);
    }

    private void OnClickItemCallback(int skillId)
    {
        EquipSkillPanel.SelectSkillId = skillId;
        EquipSkillPanel.Open();
    }
}