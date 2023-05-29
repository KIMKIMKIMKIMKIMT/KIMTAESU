using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using GameData;
using TMPro;
using UI;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestPopup : UI_Popup
{
    [Serializable]
    public record QuestTab
    {
        private Color NonSelectTextColor = new Color(87 / 255f, 87 / 255f, 87 / 255f);
        public QuestType Type;
        public GameObject SelectObj;
        public Button Button;
        public TMP_Text Text;

        public void OnSelect()
        {
            Text.color = Color.white;
            SelectObj.SetActive(true);
        }

        public void OffSelect()
        {
            Text.color = NonSelectTextColor;
            SelectObj.SetActive(false);
        }
    }

    [SerializeField] private QuestTab[] QuestTabs;

    [SerializeField] private Slider QuestCompleteSlider;

    [SerializeField] private Button CloseButton;
    [SerializeField] private Button AllCompleteButton;

    [SerializeField] private GameObject QuestObj;
    [SerializeField] private GameObject RepeatQuestScrollViewObj;

    [SerializeField] private RectTransform RepeatQuestItemRootTr;
    [SerializeField] private Transform QuestItemRootTr;
    [SerializeField] private Transform QuestCompleteRootTr;

    private readonly List<UI_QuestCompleteRewardItem> _uiQuestCompleteRewardItems = new();
    private readonly List<UI_QuestItem> _uiQuestItems = new();

    public override bool isTop => true;

    private const int MinX = -450;
    private const int MaxX = 350;
    private const int Gap = 800;

    private readonly CompositeDisposable _compositeDisposable = new();

    private QuestTab _currentTab;

    public Dictionary<int, long> CompleteQuests = new();
    public Dictionary<int, double> GainItems = new();

    private QuestTab CurrentTab
    {
        get => _currentTab;
        set
        {
            if (_currentTab != null)
                _currentTab.OffSelect();

            _currentTab = value;
            _currentTab.OnSelect();
            SetUI();
        }
    }

    private void OnEnable()
    {
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.OpenMenu, (int)QuestOpenMenu.Quest, 1));
    }

    private void Start()
    {
        foreach (var tab in QuestTabs)
            tab.Button.BindEvent(() => CurrentTab = tab);

        CloseButton.BindEvent(ClosePopup);
        AllCompleteButton.BindEvent(OnAllComplete);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();       
        }
    }

    public override void Open()
    {
        base.Open();

        if (CurrentTab == null)
            CurrentTab = QuestTabs[0];
        else
            SetUI();
    }

    private void OnDisable()
    {
        GameDataManager.QuestGameData.SaveGameData();

        if (CompleteQuests.Count > 0)
        {
            GameDataManager.GoodsGameData.SaveGameData();
            
            Param param = new();
            
            param.Add("CompleteQuests", CompleteQuests);
            param.Add("GainItems", GainItems);
            Utils.GetGoodsLog(ref param);

            Backend.GameLog.InsertLog("Quest", param);
            
            CompleteQuests.Clear();
            GainItems.Clear();
        }
    }

    private void SetUI()
    {
        _compositeDisposable.Clear();
        AllCompleteButton.interactable = false;

        if (_currentTab.Type == QuestType.Repeat)
        {
            QuestObj.SetActive(false);
            RepeatQuestScrollViewObj.gameObject.SetActive(true);
        }
        else
        {
            QuestObj.SetActive(true);
            RepeatQuestScrollViewObj.gameObject.SetActive(false);
            MakeQuestCompleteItems();
        }

        MakeQuestItems();
    }

    private void MakeQuestCompleteItems()
    {
        QuestType completeType;

        int questCompleteCount =
            Managers.Game.QuestDatas.Values.ToList().FindAll(questData =>
                questData.IsReceiveReward && ChartManager.QuestCharts[questData.Id].Type == CurrentTab.Type).Count;

        switch (CurrentTab.Type)
        {
            case QuestType.Daily:
                completeType = QuestType.DailyComplete;
                break;
            case QuestType.Weekly:
                completeType = QuestType.WeeklyComplete;
                break;
            default:
                return;
        }

        var questChartDatas = ChartManager.QuestCharts.Values.ToList()
            .FindAll(questChart => questChart.Type == completeType);

        long maxCount = questChartDatas
            .Max(questChart => questChart.CompleteValue);

        if (_uiQuestCompleteRewardItems.Count <= 0)
            QuestCompleteRootTr.DestroyInChildren();
        else
            _uiQuestCompleteRewardItems.ForEach(uiItem => uiItem.gameObject.SetActive(false));

        int index = 0;
        var uiQuestCompleteRewardItems = _uiQuestCompleteRewardItems.ToList();

        foreach (var questChart in questChartDatas)
        {
            float percent = questChart.CompleteValue / (float)maxCount;
            float posXOffset = Gap * percent;

            UI_QuestCompleteRewardItem uiQuestCompleteRewardItem;

            if (uiQuestCompleteRewardItems.Count > index)
                uiQuestCompleteRewardItem = uiQuestCompleteRewardItems[index++];
            else
            {
                uiQuestCompleteRewardItem = Managers.UI.MakeSubItem<UI_QuestCompleteRewardItem>(QuestCompleteRootTr);
                _uiQuestCompleteRewardItems.Add(uiQuestCompleteRewardItem);
            }

            uiQuestCompleteRewardItem.UIQuestPopup = this;
            uiQuestCompleteRewardItem.transform.localPosition = new Vector2(MinX + posXOffset, 0);
            uiQuestCompleteRewardItem.Init(questChart.Id);
            uiQuestCompleteRewardItem.gameObject.SetActive(true);
        }

        QuestCompleteSlider.value = questCompleteCount / (float)maxCount;
    }

    private void MakeQuestItems()
    {
        if (_uiQuestItems.Count <= 0)
        {
            QuestItemRootTr.DestroyInChildren();
            RepeatQuestItemRootTr.DestroyInChildren();
        }
        else
            _uiQuestItems.ForEach(uiQuestItem => uiQuestItem.gameObject.SetActive(false));

        var questChartDatas = ChartManager.QuestCharts.Values.ToList()
            .FindAll(questChart => questChart.Type == CurrentTab.Type);

        questChartDatas = questChartDatas
            .OrderBy(questChartData => Managers.Game.QuestDatas[questChartData.Id].IsReceiveReward)
            .ThenBy(questChartData => !Managers.Game.QuestDatas[questChartData.Id].IsComplete)
            .ThenBy(questChartData => questChartData.Id).ToList();

        int index = 0;

        var uiQuestItems = _uiQuestItems.ToList();

        var parent = CurrentTab.Type == QuestType.Repeat ? RepeatQuestItemRootTr : QuestItemRootTr;

        foreach (var questChart in questChartDatas)
        {
            UI_QuestItem uiQuestItem;

            if (uiQuestItems.Count > index)
            {
                uiQuestItem = uiQuestItems[index++];
                uiQuestItem.transform.SetParent(parent);
            }
            else
            {
                uiQuestItem = Managers.UI.MakeSubItem<UI_QuestItem>(parent);
                _uiQuestItems.Add(uiQuestItem);
            }

            uiQuestItem.UIQuestPopup = this;
            uiQuestItem.Init(questChart.Id, OnCompleteCallback, OnReceiveCallback);

            uiQuestItem.gameObject.SetActive(true);

            Managers.Game.QuestDatas[questChart.Id].OnChangeIsReceiveReward.Subscribe(_ =>
            {
                RefreshCompleteSlider();
            });
        }
    }

    private void RefreshCompleteSlider()
    {
        QuestType completeType;

        int questCompleteCount =
            Managers.Game.QuestDatas.Values.ToList().FindAll(questData =>
                questData.IsReceiveReward && ChartManager.QuestCharts[questData.Id].Type == CurrentTab.Type).Count;

        switch (CurrentTab.Type)
        {
            case QuestType.Daily:
                completeType = QuestType.DailyComplete;
                break;
            case QuestType.Weekly:
                completeType = QuestType.WeeklyComplete;
                break;
            default: return;
        }

        var questChartDatas = ChartManager.QuestCharts.Values.ToList()
            .FindAll(questChart => questChart.Type == completeType);

        long maxCount = questChartDatas
            .Max(questChart => questChart.CompleteValue);

        QuestCompleteSlider.value = questCompleteCount / (float)maxCount;
    }

    private void OnCompleteCallback(bool isReceive)
    {
        AllCompleteButton.interactable = isReceive;
    }

    private void OnReceiveCallback()
    {
        bool isReceive = false;

        _uiQuestItems.ForEach(uiQuestItem =>
        {
            if (uiQuestItem.QuestData.IsComplete && !uiQuestItem.QuestData.IsReceiveReward)
                isReceive = true;
        });

        AllCompleteButton.interactable = isReceive;
    }

    private void OnAllComplete()
    {
        _uiQuestItems.ForEach(uiQuestItem =>
        {
            if (uiQuestItem == null)
                return;

            if (!uiQuestItem.gameObject.activeSelf)
                return;

            uiQuestItem.AllComplete();
        });

        _uiQuestCompleteRewardItems.ForEach(uiQuestCompleteRewardItem =>
        {
            if (uiQuestCompleteRewardItem == null)
                return;

            if (!uiQuestCompleteRewardItem.gameObject.activeSelf)
                return;

            uiQuestCompleteRewardItem.Complete();
        });

        AllCompleteButton.interactable = false;
    }
}