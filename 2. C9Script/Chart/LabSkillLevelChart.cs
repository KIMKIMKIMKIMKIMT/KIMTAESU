using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class LabSkillLevelChart : IChart<int>
    {
        public ObscuredInt LabLevel;
        public ObscuredInt LabCost;
        public ObscuredInt LabTime;
        public ObscuredInt FinishCost;
        public ObscuredInt NextLabCoolTime;
        public ObscuredDouble IncreaseDamagePercent;

        public int GetID()
        {
            return LabLevel;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Lab_Level"].ToString(), out int labLevel);
            LabLevel = labLevel;

            int.TryParse(jsonData["Lab_Cost"].ToString(), out int labCost);
            LabCost = labCost;

            int.TryParse(jsonData["Lab_Time"].ToString(), out int labTime);
            LabTime = labTime;

            int.TryParse(jsonData["Finish_Cost"].ToString(), out int finishCost);
            FinishCost = finishCost;

            int.TryParse(jsonData["NextLab_Cooltime"].ToString(), out int nextLabCoolTime);
            NextLabCoolTime = nextLabCoolTime;

            double.TryParse(jsonData["Increase_Damage"].ToString(), out double increaseDamagePercent);
            IncreaseDamagePercent = increaseDamagePercent;
            
            ChartManager.LabSkillLevelCharts.TryAdd(GetID(), this);
        }
    }
}