using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class LabSetChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt StatType;
        public ObscuredInt SetPattern;
        public ObscuredInt SetNumber;
        public ObscuredDouble StatValue;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["SkillSet_Id"].ToString(), out int id);
            Id = id;

            int.TryParse(jsonData["Stat_Type"].ToString(), out int statType);
            StatType = statType;

            int.TryParse(jsonData["SkillSet_Pattern"].ToString(), out int setPattern);
            SetPattern = setPattern;

            int.TryParse(jsonData["SkillSet_Number"].ToString(), out int setNumber);
            SetNumber = setNumber;

            double.TryParse(jsonData["SkillSet_Stat_Value"].ToString(), out double statValue);
            StatValue = statValue;

            ChartManager.LabSetCharts.TryAdd(GetID(), this);
        }
    }
}