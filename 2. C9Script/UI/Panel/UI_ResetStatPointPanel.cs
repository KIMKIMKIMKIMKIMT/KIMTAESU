using System;
using BackEnd;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResetStatPointPanel : UI_Panel
{
    [SerializeField] private TMP_Text PriceText;
    
    [SerializeField] private Button ResetButton;
    [SerializeField] private Button CloseButton;

    public Action OnRefresh;

    private void Start()
    {
        ResetButton.BindEvent(OnClickReset);
        CloseButton.BindEvent(Close);
        PriceText.text = ((int)ChartManager.SystemCharts[SystemData.StatResetCost].Value).ToString();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    public override void Open()
    {
        base.Open();
        PriceText.color = !Utils.IsEnoughItem(ItemType.Goods, (int)Goods.StarBalloon,
            ChartManager.SystemCharts[SystemData.StatResetCost].Value)
            ? Color.red
            : Color.white;
    }

    private void OnClickReset()
    {
        if (!Utils.IsEnoughItem(ItemType.Goods, (int)Goods.StarBalloon,
                ChartManager.SystemCharts[SystemData.StatResetCost].Value))
        {
            Managers.Message.ShowMessage("재화가 부족합니다");
            return;
        }
        
        Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.StarBalloon, ChartManager.SystemCharts[SystemData.StatResetCost].Value);

        foreach (var statPointUpgradeChart in ChartManager.StatPointUpgradeCharts.Values)
        {
            Managers.Game.StatLevelDatas[statPointUpgradeChart.StatId].Value = 0;
        }

        Managers.Game.UserData.UseStatPoint = 0;
        
        GameDataManager.UserGameData.SaveGameData();
        GameDataManager.GoodsGameData.SaveGameData();
        GameDataManager.StatLevelGameData.SaveGameData();

        Param param = new();
        
        param.Add("UseJewel", ChartManager.SystemCharts[SystemData.StatResetCost].Value);
        Utils.GetGoodsLog(ref param);

        Backend.GameLog.InsertLog("ResetStat", param);

        Managers.Game.CalculateStat();
        
        OnRefresh?.Invoke();
        
        Close();
    }
}