using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class GuildBuffChart : IChart<int>
    {
        public ObscuredInt GuildGradeId;
        public ObscuredInt EffectStatId1;
        public ObscuredDouble EffectStatValue1;
        public ObscuredInt EffectStatId2;
        public ObscuredDouble EffectStatValue2;
        public ObscuredInt EffectStatId3;
        public ObscuredDouble EffectStatValue3;
        public ObscuredInt EffectStatId4;
        public ObscuredDouble EffectStatValue4;
        
        
        public int GetID()
        {
            return GuildGradeId;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Guild_Grade_Id"].ToString(), out int guildGradeId);
            GuildGradeId = guildGradeId;

            int.TryParse(jsonData["Have_Effect_1"].ToString(), out int effectStatId1);
            EffectStatId1 = effectStatId1;

            double.TryParse(jsonData["Have_Effect_Value_1"].ToString(), out double effectStatValue1);
            EffectStatValue1 = effectStatValue1;
            
            int.TryParse(jsonData["Have_Effect_2"].ToString(), out int effectStatId2);
            EffectStatId2 = effectStatId2;

            double.TryParse(jsonData["Have_Effect_Value_2"].ToString(), out double effectStatValue2);
            EffectStatValue2 = effectStatValue2;
            
            int.TryParse(jsonData["Have_Effect_3"].ToString(), out int effectStatId3);
            EffectStatId3 = effectStatId3;

            double.TryParse(jsonData["Have_Effect_Value_3"].ToString(), out double effectStatValue3);
            EffectStatValue3 = effectStatValue3;
            
            int.TryParse(jsonData["Have_Effect_4"].ToString(), out int effectStatId4);
            EffectStatId4 = effectStatId4;

            double.TryParse(jsonData["Have_Effect_Value_4"].ToString(), out double effectStatValue4);
            EffectStatValue4 = effectStatValue4;

            ChartManager.GuildBuffCharts.TryAdd(GetID(), this);
        }
    }
}