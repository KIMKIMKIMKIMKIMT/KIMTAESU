using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_OfflineRewardPopup : UI_Popup
{
    [SerializeField] private TMP_Text OfflineTimeText;
    [SerializeField] private TMP_Text ExpValueText;
    [SerializeField] private TMP_Text GoldValueText;

    [SerializeField] private Button ReceiveAdRewardButton;
    [SerializeField] private Button[] CloseButtons;

    public override bool isTop => true;

    private double _gainExp;
    private double _gainGold;

    private void Start()
    {
        ReceiveAdRewardButton.BindEvent(OnClickReceiveAdReward);
        foreach (var CloseButton in CloseButtons)
            CloseButton.BindEvent(OnClickClose);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI();
        }
    }

    public override void Open()
    {
        base.Open();

        TimeSpan offlineTime = Utils.GetNow() - Managers.Game.UserData.LastConnectTime;

        int offlineTimeMin = (int)Math.Min(ChartManager.SystemCharts[(SystemData.Offline_Limit_Time)].Value,
            offlineTime.TotalMinutes);

        OfflineTimeText.text =
            $"( {offlineTimeMin} / {(int)ChartManager.SystemCharts[SystemData.Offline_Limit_Time].Value} )분";
        
        var stageChart = ChartManager.StageDataController.StageDataTable[Managers.Game.UserData.CurrentStage];

        _gainExp = stageChart.ExpValues[0] * ChartManager.SystemCharts[SystemData.Offline_Reward_Min].Value *
                   offlineTimeMin;

        _gainGold = stageChart.GoldValues[0] * ChartManager.SystemCharts[SystemData.Offline_Reward_Min].Value *
                    offlineTimeMin;
        
        ExpValueText.text = _gainExp.ToCurrencyString();
        GoldValueText.text = _gainGold.ToCurrencyString();
        
        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.Exp, _gainExp);
        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.Gold, _gainGold);
        
        GameDataManager.UserGameData.SaveGameData();
        GameDataManager.GoodsGameData.SaveGameData();
    }

    private void OnClickReceiveAdReward()
    {
        Managers.Ad.Show(() =>
            {
                ClosePopup();
            var gainItemDatas = new Dictionary<(ItemType, int), double>();
            
            // 지급은 한번 제외 하고 2개 주고 표기는 3배로
            Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.Exp, _gainExp * 2);
            Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.Gold, _gainGold * 2);
            
            gainItemDatas[(ItemType.Goods, (int)Goods.Exp)] = _gainExp * 3;
            gainItemDatas[(ItemType.Goods, (int)Goods.Gold)] = _gainGold * 3;

            Managers.UI.ShowGainItems(gainItemDatas);
            
            GameDataManager.UserGameData.SaveGameData();
            GameDataManager.GoodsGameData.SaveGameData();
        });
    }

    private void OnClickClose()
    {
        ClosePopup();
    }
}