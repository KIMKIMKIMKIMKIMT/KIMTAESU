using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class SkillPolicyChart : IChart<(int, SkillPolicyProperty)>
    {
        public ObscuredInt Id;
        public SkillPolicyProperty PolicyProperty;
        public ObscuredFloat Value;
        
        public (int, SkillPolicyProperty) GetID()
        {
            return (Id, PolicyProperty);
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Skill_Id"].ToString(), out int id);
            Id = id;
            
            Enum.TryParse(jsonData["Policy_Property"].ToString(), out PolicyProperty);
            
            float.TryParse(jsonData["Policy_Value"].ToString(), out float value);
            Value = value;

            ChartManager.SkillPolicyCharts[GetID()] = this;
        }
    }
}