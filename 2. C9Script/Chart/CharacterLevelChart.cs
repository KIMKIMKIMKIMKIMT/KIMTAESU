using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class CharacterLevelChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredDouble Exp;
        public ObscuredInt AbilityPoint;

        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Level_Id"].ToString(), out int id);
            Id = id;
            
            double.TryParse(jsonData["Exp"].ToString(), out double exp);
            Exp = exp;
            
            int.TryParse(jsonData["Ability_Point"].ToString(), out int abilityPoint);
            AbilityPoint = abilityPoint;
        }
    }
}