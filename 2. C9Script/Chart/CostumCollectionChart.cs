using LitJson;
using CodeStage.AntiCheat.ObscuredTypes;
using System;

namespace Chart
{
    public class CostumCollectionChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt SetName;
        public int[] RequireCostumes;
        public ObscuredInt StatId;
        public ObscuredDouble StatValue;
        public string[] Icons;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["CostumeCollection_Id"]["S"].ToString(), out int id);
            Id = id;

            int.TryParse(jsonData["Set_Name"]["S"].ToString(), out int setName);
            SetName = setName;

            int[] requireCostumes = Array.ConvertAll(jsonData["RequireCostume"]["S"].ToString().Trim().Split(','), int.Parse);
            RequireCostumes = new int[requireCostumes.Length];
            for (int i = 0; i < requireCostumes.Length; i++)
                RequireCostumes[i] = requireCostumes[i];

            int.TryParse(jsonData["Stat_Type"]["S"].ToString(), out int statId);
            StatId = statId;

            double.TryParse(jsonData["Stat_Type_Value"]["S"].ToString(), out double statValue);
            StatValue = statValue;

            string[] icons = (jsonData["Costume_Icon"]["S"].ToString().Trim().Split(','));
            Icons = new string[icons.Length];
            for (int i = 0; i < icons.Length; i++)
                Icons[i] = icons[i].Trim();

            ChartManager.CostumCollectionCharts[GetID()] = this;
        }
    }
}

