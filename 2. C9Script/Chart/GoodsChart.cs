using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class GoodsChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString Name;
        public ObscuredString Icon;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Goods_Id"].ToString(), out int id);
            Id = id;
            
            Name = jsonData["Goods_Name"].ToString();
            
            Icon = jsonData["Goods_Icon"].ToString();

            ChartManager.GoodsCharts[GetID()] = this;
        }
    }
}