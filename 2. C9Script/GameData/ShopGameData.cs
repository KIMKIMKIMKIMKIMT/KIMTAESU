using BackEnd;
using LitJson;

namespace GameData
{
    public class ShopGameData : BaseGameData
    {
        public override string TableName => "Shop";
        protected override string InDate { get; set; }

        protected override Param MakeInitData()
        {
            Param param = new Param();
            
            foreach (var shopChart in ChartManager.ShopCharts.Values)
            {
                if (shopChart.LimitType == ShopLimitType.None)
                    continue;
                
                param.Add(shopChart.ProductName, 0);
            }

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();
            
            foreach (var shopData in Managers.Game.ShopDatas)
            {
                param.Add(ChartManager.ShopCharts[shopData.Key].ProductName, shopData.Value);
            }

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();
            
            foreach (var shopChart in ChartManager.ShopCharts.Values)
            {
                int buyCount = 0;
                
                if (jsonData.ContainsKey(shopChart.ProductName))
                {
                    int.TryParse(jsonData[shopChart.ProductName].ToString(), out buyCount);
                }
                else
                {
                    param.Add(shopChart.ProductName, 0);
                }

                if (Managers.Game.ShopDatas.ContainsKey(shopChart.ShopId))
                    Managers.Game.ShopDatas[shopChart.ShopId] = buyCount;
                else
                    Managers.Game.ShopDatas.Add(shopChart.ShopId, buyCount);
            }
            
            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}