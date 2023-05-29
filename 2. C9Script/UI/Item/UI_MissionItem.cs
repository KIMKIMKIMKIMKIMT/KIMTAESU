using System;
using System.Collections.Generic;
using Chart;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MissionItem : UI_Base
{
    [Serializable]
    public record RewardItem
    {
        public Image RewardImage;
        public TMP_Text RewardText;
        public GameObject CheckObj;
        public GameObject LockObj;
        public GameObject Obj;
        public Button ReceiveButton;

        public void SetRewardInfo(ItemType itemType, int itemId, int itemValue)
        {
            RewardImage.sprite = Managers.Resource.LoadItemIcon(itemType, itemId);
            RewardText.text = itemValue.ToString();
        }
    }

    [SerializeField] private TMP_Text MissionValueText;

    [SerializeField] private RewardItem FreeRewardItem;
    [SerializeField] private RewardItem CashRewardItem;
    [SerializeField] private RewardItem AdRewardItem;

    private MissionChart _missionChart;
    private bool _isPurchase;

    public UI_MissionPopup UIMissionPopup;

    private void Start()
    {
        FreeRewardItem.ReceiveButton.BindEvent(() => OnClickFreeReceive());
        CashRewardItem.ReceiveButton.BindEvent(() => OnClickCashReceive());
        AdRewardItem.ReceiveButton.BindEvent(() => OnClickAdReceive());
    }

    public void Init(int missionId, bool isPurchase)
    {
        _missionChart = ChartManager.MissionCharts[missionId];
        _isPurchase = isPurchase;
        SetUI();
    }

    private void SetUI()
    {
        MissionValueText.text = _missionChart.MissionValue.ToString();

        SetFreeReward();
        SetCashReward();
        SetAdReward();
    }

    private void SetFreeReward()
    {
        FreeRewardItem.SetRewardInfo(_missionChart.RewardItemTypes[0], _missionChart.RewardItemIds[0],
            _missionChart.RewardItemValues[0]);

        FreeRewardItem.LockObj.SetActive(Managers.Game.MissionDatas[_missionChart.MissionId].Item1 == 0 &&
                                         !IsCanReceiveItem());
        FreeRewardItem.CheckObj.SetActive(Managers.Game.MissionDatas[_missionChart.MissionId].Item1 == 1);
    }

    private void SetCashReward()
    {
        CashRewardItem.SetRewardInfo(_missionChart.RewardItemTypes[1], _missionChart.RewardItemIds[1],
            _missionChart.RewardItemValues[1]);

        CashRewardItem.LockObj.SetActive(!_isPurchase ||
                                         (!IsCanReceiveItem() &&
                                          Managers.Game.MissionDatas[_missionChart.MissionId].Item1 == 0));
        CashRewardItem.CheckObj.SetActive(Managers.Game.MissionDatas[_missionChart.MissionId].Item2 == 1);
    }

    private void SetAdReward()
    {
        if (_missionChart.RewardItemTypes.Length < 3)
        {
            AdRewardItem.Obj.SetActive(false);
        }
        else
        {
            AdRewardItem.Obj.SetActive(true);
            AdRewardItem.SetRewardInfo(_missionChart.RewardItemTypes[2], _missionChart.RewardItemIds[2],
                _missionChart.RewardItemValues[2]);
            AdRewardItem.LockObj.SetActive(Managers.Game.MissionDatas[_missionChart.MissionId].Item3 == 0 &&
                                           !IsCanReceiveItem());
            AdRewardItem.CheckObj.SetActive(Managers.Game.MissionDatas[_missionChart.MissionId].Item3 == 1);
        }
    }

    private bool IsCanReceiveItem()
    {
        switch (_missionChart.MissionType)
        {
            case MissionType.DailyPlayTime:
                return Managers.Game.MissionProgressData.DailyPlayTime >= _missionChart.MissionValue;
            case MissionType.DailyKillCount:
                return Managers.Game.MissionProgressData.DailyKillCount >= _missionChart.MissionValue;
            case MissionType.Lv:
                return Managers.Game.UserData.Level >= _missionChart.MissionValue;
            default:
                return false;
        }
    }

    private void OnClickFreeReceive(bool isAllReceive = false)
    {
        if (Managers.Game.MissionDatas[_missionChart.MissionId].Item1 == 1)
            return;

        if (!IsCanReceiveItem())
            return;

        Managers.Game.IncreaseItem(_missionChart.RewardItemTypes[0], _missionChart.RewardItemIds[0],
            _missionChart.RewardItemValues[0]);
        var gameMissionData = Managers.Game.MissionDatas[_missionChart.MissionId];
        gameMissionData.Item1 = 1;
        Managers.Game.MissionDatas[_missionChart.MissionId] = gameMissionData;

        Managers.UI.ShowGainItems(new Dictionary<(ItemType, int), double>()
        {
            {(_missionChart.RewardItemTypes[0], _missionChart.RewardItemIds[0]), _missionChart.RewardItemValues[0]}
        });
        
        UIMissionPopup.ReceiveMissionIds.Add((_missionChart.MissionId, "Free"));
        UIMissionPopup.GainJewel += _missionChart.RewardItemValues[0];

        if (!isAllReceive)
            SetUI();
    }

    private void OnClickCashReceive(bool isAllReceive = false)
    {
        if (Managers.Game.MissionDatas[_missionChart.MissionId].Item2 == 1)
            return;

        if (!_isPurchase)
            return;

        if (!IsCanReceiveItem())
            return;

        Managers.Game.IncreaseItem(_missionChart.RewardItemTypes[1], _missionChart.RewardItemIds[1],
            _missionChart.RewardItemValues[1]);
        var gameMissionData = Managers.Game.MissionDatas[_missionChart.MissionId];
        gameMissionData.Item2 = 1;
        Managers.Game.MissionDatas[_missionChart.MissionId] = gameMissionData;

        Managers.UI.ShowGainItems(new Dictionary<(ItemType, int), double>()
        {
            {(_missionChart.RewardItemTypes[1], _missionChart.RewardItemIds[1]), _missionChart.RewardItemValues[1]}
        });
        
        UIMissionPopup.ReceiveMissionIds.Add((_missionChart.MissionId, "Cash"));
        UIMissionPopup.GainJewel += _missionChart.RewardItemValues[0];

        if (!isAllReceive)
            SetUI();
    }

    private void OnClickAdReceive(bool isAllReceive = false)
    {
        if (Managers.Game.MissionDatas[_missionChart.MissionId].Item3 == 1)
            return;

        if (!IsCanReceiveItem())
            return;

        if (_missionChart.RewardItemTypes.Length < 3)
            return;

        void GiveReward()
        {
            Managers.Game.IncreaseItem(_missionChart.RewardItemTypes[2], _missionChart.RewardItemIds[2],
                _missionChart.RewardItemValues[2]);
            var gameMissionData = Managers.Game.MissionDatas[_missionChart.MissionId];
            gameMissionData.Item3 = 1;
            Managers.Game.MissionDatas[_missionChart.MissionId] = gameMissionData;

            Managers.UI.ShowGainItems(new Dictionary<(ItemType, int), double>()
            {
                {(_missionChart.RewardItemTypes[2], _missionChart.RewardItemIds[2]), _missionChart.RewardItemValues[2]}
            });
            
            UIMissionPopup.ReceiveMissionIds.Add((_missionChart.MissionId, "Ad"));
            UIMissionPopup.GainJewel += _missionChart.RewardItemValues[0];

            if (!isAllReceive)
                SetUI();
        }

        if (Managers.Game.UserData.IsAdSkip())
        {
            GiveReward();
        }
        else
        {
            if (isAllReceive)
                return;

            Managers.Ad.Show(GiveReward);
        }
    }
}