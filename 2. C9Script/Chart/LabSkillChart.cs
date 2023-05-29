using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class LabSkillChart : IChart<LabSkillType>
    {
        public LabSkillType LabSkillType;
        public ObscuredInt MaxLevel;
        public ObscuredString Icon;
        public ObscuredString Bg;
        
        public LabSkillType GetID()
        {
            return LabSkillType;
        }

        public void SetData(JsonData jsonData)
        {
            Enum.TryParse(jsonData["SkillLab_Type"].ToString(), out LabSkillType);

            int.TryParse(jsonData["Max_Level"].ToString(), out int maxLevel);
            MaxLevel = maxLevel;

            Icon = jsonData["Icon"].ToString();

            Bg = jsonData["Bg"].ToString();
            
            ChartManager.LabSkillCharts.TryAdd(GetID(), this);
        }
    }
}