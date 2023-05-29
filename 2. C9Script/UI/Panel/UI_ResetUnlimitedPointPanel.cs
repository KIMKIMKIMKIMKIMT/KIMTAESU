using System;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    public class UI_ResetUnlimitedPointPanel : UI_Panel
    {
        [SerializeField] private TMP_Text PriceText;

        [SerializeField] private Button ResetButton;
        [SerializeField] private Button CloseButton;

        public Action OnRefresh;

        private void Start()
        {
            ResetButton.BindEvent(OnClickReset);
            CloseButton.BindEvent(Close);
            PriceText.text = ((int)ChartManager.SystemCharts[SystemData.UnlimitedPoint_ResetCost].Value).ToString();
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
                ChartManager.SystemCharts[SystemData.UnlimitedPoint_ResetCost].Value)
                ? Color.red
                : Color.white;
        }

        private void OnClickReset()
        {
            if (!Utils.IsEnoughItem(ItemType.Goods, (int)Goods.StarBalloon,
                    ChartManager.SystemCharts[SystemData.UnlimitedPoint_ResetCost].Value))
            {
                Managers.Message.ShowMessage("재화가 부족합니다");
                return;
            }

            Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.StarBalloon,
                ChartManager.SystemCharts[SystemData.UnlimitedPoint_ResetCost].Value);

            foreach (var unlimitedPointUpgradeChart in ChartManager.UnlimitedPointUpgradeCharts.Values)
            {
                Managers.Game.UnlimitedStatLevelDatas[unlimitedPointUpgradeChart.StatId].Value = 0;
            }

            Managers.Game.UserData.UseUnlimitedPoint = 0;

            GameDataManager.UserGameData.SaveGameData();
            GameDataManager.GoodsGameData.SaveGameData();
            GameDataManager.UnlimitedStatLevelGameData.SaveGameData();

            Param param = new();

            param.Add("UseJewel", ChartManager.SystemCharts[SystemData.UnlimitedPoint_ResetCost].Value);
            Utils.GetGoodsLog(ref param);

            Backend.GameLog.InsertLog("ResetStat", param);

            Managers.Game.CalculateStat();

            OnRefresh?.Invoke();

            Close();
        }
    }
}