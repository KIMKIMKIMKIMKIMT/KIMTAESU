using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackEnd;
using GameData;
using TMPro;
using UI;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_MissionPopup : UI_Popup
{
    [Serializable]
    public record Tab
    {
        public MissionType MissionType;
        public bool usingSubType;
        public Button TabButton;
    }

    [Serializable]
    public record SubTab
    {
        public int SubType;
        public Button SubTabButton;
    }

    [SerializeField] private Tab[] Tabs;

    [SerializeField] private TMP_Text GuideText;
    [SerializeField] private TMP_Text BuyButtonText;
    [SerializeField] private TMP_Text MissionValueText;
    [SerializeField] private TMP_Text[] SubTypeTexts;

    [SerializeField] private Button[] SubTypeButtons;
    [SerializeField] private Button AllReceiveButton;
    [SerializeField] private Button BuyButton;
    [SerializeField] private Button CloseButton;

    [SerializeField] private Button PrevSubTabButton;
    [SerializeField] private Button NextSubTabButton;

    [SerializeField] private Transform UIMissionItemRoot;

    [SerializeField] private GameObject AdRewardObj;
    [SerializeField] private GameObject SubTypeObj;

    public override bool isTop => true;

    private readonly List<UI_MissionItem> _uiMissionItems = new();

    private Tab _currentTab;

    private Tab CurrentTab
    {
        get => _currentTab;
        set
        {
            _currentTab = value;

            if (!_currentTab.usingSubType)
                _currentSubType = 0;
            else if (_currentSubType == 0)
                _currentSubType = 1;

            foreach (var shopChart in ChartManager.ShopCharts.Values)
            {
                if (shopChart.SubType != _currentSubType)
                    continue;
                
                if (shopChart.ShopType == ShopType.DailyPlayTimeMission &&
                    _currentTab.MissionType == MissionType.DailyPlayTime)
                {
                    _currentShopId = shopChart.ShopId;
                    break;
                }

                if (shopChart.ShopType == ShopType.LvMission &&
                    _currentTab.MissionType == MissionType.Lv)
                {
                    _currentShopId = shopChart.ShopId;
                    break;
                }

                if (shopChart.ShopType == ShopType.DailyKillMission &&
                    _currentTab.MissionType == MissionType.DailyKillCount)
                {
                    _currentShopId = shopChart.ShopId;
                    break;
                }
            }

            SetUI();
        }
    }

    private int _currentSubType;

    private int CurrentSubType
    {
        get => _currentSubType;
        set
        {
            _currentSubType = value;
            foreach (var shopChart in ChartManager.ShopCharts.Values)
            {
                if (shopChart.SubType != _currentSubType)
                    continue;
                
                if (shopChart.ShopType == ShopType.DailyPlayTimeMission &&
                    _currentTab.MissionType == MissionType.DailyPlayTime)
                {
                    _currentShopId = shopChart.ShopId;
                    break;
                }

                if (shopChart.ShopType == ShopType.LvMission &&
                    _currentTab.MissionType == MissionType.Lv)
                {
                    _currentShopId = shopChart.ShopId;
                    break;
                }

                if (shopChart.ShopType == ShopType.DailyKillMission &&
                    _currentTab.MissionType == MissionType.DailyKillCount)
                {
                    _currentShopId = shopChart.ShopId;
                    break;
                }
            }
            SetUI();
        }
    }
    private int _currentShopId;

    private int _startSubIndex = 1;

    private int StartSubIndex
    {
        get => _startSubIndex;
        set
        {
            _startSubIndex = value;
            SetSubTabEvent();
        }
    }

    public readonly List<(int, string)> ReceiveMissionIds = new();
    public double GainJewel;

    private void OnEnable()
    {
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.OpenMenu, (int)QuestOpenMenu.Mission, 1));
    }

    private void Start()
    {
        foreach (var tab in Tabs)
            tab.TabButton.BindEvent(() => CurrentTab = tab);

        SetSubTabEvent();
        
        AllReceiveButton.BindEvent(OnClickAllReceive);
        BuyButton.BindEvent(OnClickBuy);
        CloseButton.BindEvent(ClosePopup);
        PrevSubTabButton.BindEvent(OnClickPrevSubTab);
        NextSubTabButton.BindEvent(OnClickNextSubTab);
    }

    private void OnDisable()
    {
        if (ReceiveMissionIds.Count <= 0) 
            return;
        
        GameDataManager.MissionGameData.SaveGameData();
        GameDataManager.GoodsGameData.SaveGameData();
        GameDataManager.UserGameData.SaveGameData();
            
        Param param = new();
            
        param.Add("ReceiveIds", ReceiveMissionIds);
        param.Add("GainJewel", GainJewel);
        Utils.GetGoodsLog(ref param);

        Backend.GameLog.InsertLog("Mission", param);
            
        ReceiveMissionIds.Clear();
        GainJewel = 0;
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
            CurrentTab = Tabs[0];
        else
        {
            SetUI();
        }
    }

    private void SetSubTabEvent()
    {
        int subTypeIndex = StartSubIndex;
        for (var i = 0; i < SubTypeButtons.Length; i++, subTypeIndex++)
        {
            int cacheIndex = subTypeIndex;
            SubTypeTexts[i].text = subTypeIndex.ToString();
            SubTypeButtons[i].ClearEvent();
            SubTypeButtons[i].BindEvent(() => CurrentSubType = cacheIndex);
        }
        
        if (CurrentTab.usingSubType)
        {
            var missionCharts = ChartManager.MissionCharts.Values
                .Where(missionChart => missionChart.MissionType == MissionType.Lv).ToList();
            PrevSubTabButton.gameObject.SetActive(missionCharts.Find(missionChart => missionChart.SubLevel == StartSubIndex - 1) != null);
            NextSubTabButton.gameObject.SetActive(missionCharts.Find(missionChart => missionChart.SubLevel == StartSubIndex + 10) != null);
        }
        else
        {
            PrevSubTabButton.gameObject.SetActive(false);
            NextSubTabButton.gameObject.SetActive(false);
        }

    }

    private void SetUI()
    {
        ChartManager.ShopCharts.TryGetValue(_currentShopId, out var shopChart);
        SubTypeObj.SetActive(CurrentTab.usingSubType);
        if (CurrentTab.usingSubType)
        {
            var missionCharts = ChartManager.MissionCharts.Values
                .Where(missionChart => missionChart.MissionType == MissionType.Lv).ToList();
            PrevSubTabButton.gameObject.SetActive(missionCharts.Find(missionChart => missionChart.SubLevel == StartSubIndex - 1) != null);
            NextSubTabButton.gameObject.SetActive(missionCharts.Find(missionChart => missionChart.SubLevel == StartSubIndex + 10) != null);
        }
        else
        {
            PrevSubTabButton.gameObject.SetActive(false);
            NextSubTabButton.gameObject.SetActive(false);
        }

        GuideText.text =
            $"<color=#ffff00>{ChartManager.GetString(shopChart.ProductName)}</color> *미션 상품은 청약철회가 <color=#ff0000>불가능</color>한 상품입니다.";
        BuyButtonText.text = Utils.IsPurchasedProduct(shopChart.ShopId) ? "구매 완료" : $"{shopChart.PriceValue:N0}원";

        switch (CurrentTab.MissionType)
        {
            case MissionType.DailyPlayTime:
                MissionValueText.text = $"접속 시간 : {Managers.Game.MissionProgressData.DailyPlayTime / 60}분";
                break;
            case MissionType.DailyKillCount:
                MissionValueText.text = $"처치 수 : {Managers.Game.MissionProgressData.DailyKillCount}마리";
                break;
            case MissionType.Lv:
                MissionValueText.text = $"현재 레벨 : {Managers.Game.UserData.Level}";
                break;
        }
        
        RefreshMissionItems();
    }

    private void RefreshMissionItems()
    {
        if (_uiMissionItems.Count <= 0)
            UIMissionItemRoot.DestroyInChildren();
        else
            _uiMissionItems.ForEach(missionItem => missionItem.gameObject.SetActive(false));

        List<UI_MissionItem> uiMissionItems = new();

        bool isPurchase = Managers.Game.ShopDatas[_currentShopId] == 1;

        int index = 0;

        bool isAd = false;

        foreach (var missionChart in ChartManager.MissionCharts.Values)
        {
            if (missionChart.MissionType != CurrentTab.MissionType)
                continue;

            if (missionChart.SubLevel != _currentSubType)
                continue;

            UI_MissionItem uiItem;

            if (_uiMissionItems.Count > index)
                uiItem = _uiMissionItems[index++];
            else
            {
                uiItem = Managers.UI.MakeSubItem<UI_MissionItem>(UIMissionItemRoot);
                uiMissionItems.Add(uiItem);
            }

            uiItem.UIMissionPopup = this;
            uiItem.Init(missionChart.MissionId, isPurchase);
            uiItem.gameObject.SetActive(true);

            if (missionChart.RewardItemTypes.Length >= 3)
                isAd = true;
        }
        
        AdRewardObj.SetActive(isAd);

        uiMissionItems.ForEach(uiItem => _uiMissionItems.Add(uiItem));
    }

    private void OnClickAllReceive()
    {
        var missionDatas =
            Managers.Game.MissionDatas.ToList().FindAll(missionData =>
                ChartManager.MissionCharts[missionData.Key].MissionType == _currentTab.MissionType);

        if (_currentTab.usingSubType)
        {
            missionDatas = missionDatas.FindAll(missionData =>
                ChartManager.MissionCharts[missionData.Key].SubLevel == _currentSubType);
        }

        var gainItemDatas = new Dictionary<(ItemType, int), double>();
        
        bool isPurchase = Managers.Game.ShopDatas[_currentShopId] == 1;
        
        missionDatas.ForEach(missionData =>
        {
            if (!IsCompleteMission(missionData.Key))
                return;

            var rewardTypes = ChartManager.MissionCharts[missionData.Key].RewardItemTypes;
            var rewardIds = ChartManager.MissionCharts[missionData.Key].RewardItemIds;
            var rewardValues = ChartManager.MissionCharts[missionData.Key].RewardItemValues;

            var receiveData = Managers.Game.MissionDatas[missionData.Key];
                    
            if (missionData.Value.Item1 == 0 && rewardTypes.Length > 0)
            {
                Managers.Game.IncreaseItem(rewardTypes[0], rewardIds[0], rewardValues[0]);
                
                if (gainItemDatas.ContainsKey((rewardTypes[0], rewardIds[0])))
                    gainItemDatas[(rewardTypes[0], rewardIds[0])] += rewardValues[0];
                else
                    gainItemDatas[(rewardTypes[0], rewardIds[0])] = rewardValues[0];
                
                receiveData.Item1 = 1;

                ReceiveMissionIds.Add((missionData.Key, "Free"));
                GainJewel += rewardValues[0];
            }

            if (missionData.Value.Item2 == 0 && rewardTypes.Length > 1 && isPurchase)
            {
                Managers.Game.IncreaseItem(rewardTypes[1], rewardIds[1], rewardValues[1]);

                if (gainItemDatas.ContainsKey((rewardTypes[1], rewardIds[1])))
                    gainItemDatas[(rewardTypes[1], rewardIds[1])] += rewardValues[1];
                else
                    gainItemDatas[(rewardTypes[1], rewardIds[1])] = rewardValues[1];
                
                receiveData.Item2 = 1;
                
                ReceiveMissionIds.Add((missionData.Key, "Cash"));
                GainJewel += rewardValues[1];
            }

            if (missionData.Value.Item3 == 0 && rewardTypes.Length > 2 && Managers.Game.UserData.IsAdSkip())
            {
                Managers.Game.IncreaseItem(rewardTypes[2], rewardIds[2], rewardValues[2]);

                if (gainItemDatas.ContainsKey((rewardTypes[2], rewardIds[2])))
                    gainItemDatas[(rewardTypes[2], rewardIds[2])] += rewardValues[2];
                else
                    gainItemDatas[(rewardTypes[2], rewardIds[2])] = rewardValues[2];
                
                receiveData.Item3 = 1;
                
                ReceiveMissionIds.Add((missionData.Key, "Ad"));
                GainJewel += rewardValues[2];
            }

            Managers.Game.MissionDatas[missionData.Key] = receiveData;
        });

        Managers.UI.ShowGainItems(gainItemDatas);

        RefreshMissionItems();
    }

    private void OnClickBuy()
    {
        if (Utils.IsPurchasedProduct(_currentShopId))
            return;

        Utils.BuyShopItem(_currentShopId, true, true, false, SetUI);
    }

    private bool IsCompleteMission(int missionId)
    {
        if (!ChartManager.MissionCharts.TryGetValue(missionId, out var missionChart))
            return false;

        return missionChart.MissionType switch
        {
            MissionType.DailyKillCount => Managers.Game.MissionProgressData.DailyKillCount >= missionChart.MissionValue,
            MissionType.DailyPlayTime => Managers.Game.MissionProgressData.DailyPlayTime >= missionChart.MissionValue,
            MissionType.Lv => Managers.Game.UserData.Level >= missionChart.MissionValue,
            _ => false
        };
    }

    private void OnClickPrevSubTab()
    {
        if (StartSubIndex <= 0)
            return;
        
        StartSubIndex = 1;
    }

    private void OnClickNextSubTab()
    {
        if (StartSubIndex >= 10)
            return;

        StartSubIndex = 11;
    }
}