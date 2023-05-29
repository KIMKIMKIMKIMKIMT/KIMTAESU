using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class WorldWoodChart : IChart<int>
    {
        #region Fields
        public ObscuredInt Id;
        public Grade Grade;
        public ObscuredInt MaxLevel;
        public ObscuredInt LevelUpItemId;
        public ObscuredInt GradeUpCost;
        public ObscuredDouble LevelUpItemValue;
        public ObscuredDouble LevelUpItemIncreaseValue;

        public ObscuredInt GradeStatType1;
        public ObscuredDouble GradeStatValue1;
        public ObscuredDouble GradeStatIncreaseValue1;
        public ObscuredInt GradeStatType2;
        public ObscuredDouble GradeStatValue2;
        public ObscuredDouble GradeStatIncreaseValue2;
        public ObscuredInt GradeStatType3;
        public ObscuredDouble GradeStatValue3;
        public ObscuredDouble GradeStatIncreaseValue3;
        public ObscuredInt GradeStatType4;
        public ObscuredDouble GradeStatValue4;
        public ObscuredDouble GradeStatIncreaseValue4;
        public ObscuredInt WakeUpStatType1;
        public ObscuredDouble WakeUpStatValue1;
        public ObscuredDouble WakeUpStatIncreaseValue1;
        public ObscuredInt WakeUpStatType2;
        public ObscuredDouble WakeUpStatValue2;
        public ObscuredDouble WakeUpStatIncreaseValue2;
        public ObscuredInt WakeUpStatType3;
        public ObscuredDouble WakeUpStatValue3;
        public ObscuredDouble WakeUpStatIncreaseValue3;
        public ObscuredInt WakeUpStatType4;
        public ObscuredDouble WakeUpStatValue4;
        public ObscuredDouble WakeUpStatIncreaseValue4;

        public ObscuredString WoodName;
        #endregion


        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Wood_Id"].ToString(), out int id);
            Id = id;

            Enum.TryParse(jsonData["Wood_Grade"].ToString(), out Grade);

            int.TryParse(jsonData["Wood_Grade_Cost"].ToString(), out int gradeUpCost);
            GradeUpCost = gradeUpCost;
            int.TryParse(jsonData["Max_Level"].ToString(), out int maxLevel);
            MaxLevel = maxLevel;

            int.TryParse(jsonData["Upgrade_Goods_Id"].ToString(), out int levelUpItemId);
            LevelUpItemId = levelUpItemId;

            double.TryParse(jsonData["Upgrade_Goods_Value"].ToString(), out double levelUpItemValue);
            LevelUpItemValue = levelUpItemValue;
            double.TryParse(jsonData["Upgrade_Goods_Increase_Value"].ToString(), out double levelUpItemIncreaseValue);
            LevelUpItemIncreaseValue = levelUpItemIncreaseValue;

            int.TryParse(jsonData["Grade_Stat_1"].ToString(), out int gradeStatType1);
            GradeStatType1 = gradeStatType1;

            double.TryParse(jsonData["Grade_Stat_1_Value"].ToString(), out double gradeStatValue1);
            GradeStatValue1 = gradeStatValue1;

            double.TryParse(jsonData["Grade_Stat_1_Increase_Value"].ToString(), out double gradeStatIncreaseValue1);
            GradeStatIncreaseValue1 = gradeStatIncreaseValue1;

            int.TryParse(jsonData["Grade_Stat_2"].ToString(), out int gradeStatType2);
            GradeStatType2 = gradeStatType2;

            double.TryParse(jsonData["Grade_Stat_2_Value"].ToString(), out double gradeStatValue2);
            GradeStatValue2 = gradeStatValue2;

            double.TryParse(jsonData["Grade_Stat_2_Increase_Value"].ToString(), out double gradeStatIncreaseValue2);
            GradeStatIncreaseValue2 = gradeStatIncreaseValue2;

            int.TryParse(jsonData["Grade_Stat_3"].ToString(), out int gradeStatType3);
            GradeStatType3 = gradeStatType3;

            double.TryParse(jsonData["Grade_Stat_3_Value"].ToString(), out double gradeStatValue3);
            GradeStatValue3 = gradeStatValue3;

            double.TryParse(jsonData["Grade_Stat_3_Increase_Value"].ToString(), out double gradeStatIncreaseValue3);
            GradeStatIncreaseValue3 = gradeStatIncreaseValue3;

            int.TryParse(jsonData["Grade_Stat_4"].ToString(), out int gradeStatType4);
            GradeStatType4 = gradeStatType4;

            double.TryParse(jsonData["Grade_Stat_4_Value"].ToString(), out double gradeStatValue4);
            GradeStatValue4 = gradeStatValue4;

            double.TryParse(jsonData["Grade_Stat_4_Increase_Value"].ToString(), out double gradeStatIncreaseValue4);
            GradeStatIncreaseValue4 = gradeStatIncreaseValue4;

            ChartManager.WoodCharts[GetID()] = this;
        }
    }
}


