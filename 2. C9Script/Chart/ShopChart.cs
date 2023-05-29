using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class ShopChart : IChart<int>
    {
        public ObscuredInt ShopId;
        public ShopType ShopType;
        public ObscuredInt SubType;
        public ShopPriceType PriceType;
        public ObscuredInt PriceId;
        public ObscuredInt PriceValue;
        public ShopLimitType LimitType;
        public ObscuredInt LimitValue;
        public ItemType[] RewardItemTypes;
        public ObscuredInt[] RewardItemIds;
        public ObscuredInt[] RewardItemValues;
        public ObscuredString Icon;
        public ObscuredString ProductName;
        public ObscuredInt Sort;

        public int GetID()
        {
            return ShopId;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Sale"].ToString(), out int sale);
            if (sale != 1)
                return;
            
            int.TryParse(jsonData["Shop_Id"].ToString(), out int shopId);
            ShopId = shopId;
            
            Enum.TryParse(jsonData["Category"].ToString(), out ShopType);
            
            int.TryParse(jsonData["Sub_Category"].ToString(), out int subType);
            SubType = subType;
            
            Enum.TryParse(jsonData["Price_Type"].ToString(), out PriceType);
            
            int.TryParse(jsonData["Price_Type_Id"].ToString(), out int priceId);
            PriceId = priceId;
            
            int.TryParse(jsonData["Price_Value"].ToString(), out int priceValue);
            PriceValue = priceValue;
            
            Enum.TryParse(jsonData["Purchase_Limit_Type"].ToString(), out LimitType);
            
            int.TryParse(jsonData["Purchase_Limit_Value"].ToString(), out int limitValue);
            LimitValue = limitValue;
            
            RewardItemTypes = Array.ConvertAll(jsonData["Reward_Item_Type"].ToString().Trim().Split(','), Enum.Parse<ItemType>);
            
            int[] rewardItemIds = Array.ConvertAll(jsonData["Reward_Item_Id"].ToString().Trim().Split(','), int.Parse);
            RewardItemIds = new ObscuredInt[rewardItemIds.Length];
            for (int i = 0; i < rewardItemIds.Length; i++)
                RewardItemIds[i] = rewardItemIds[i];
            
            int[] rewardItemValues = Array.ConvertAll(jsonData["Reward_Item_Value"].ToString().Trim().Split(','), int.Parse);
            RewardItemValues = new ObscuredInt[rewardItemValues.Length];
            for (int i = 0; i < rewardItemValues.Length; i++)
                RewardItemValues[i] = rewardItemValues[i];
            
            Icon = jsonData["Icon"].ToString();
            ProductName = jsonData["Product_Name"].ToString();
            
            int.TryParse(jsonData["Sort"].ToString(), out int sort);
            Sort = sort;

            ChartManager.ShopCharts[GetID()] = this;
        }
    }
}