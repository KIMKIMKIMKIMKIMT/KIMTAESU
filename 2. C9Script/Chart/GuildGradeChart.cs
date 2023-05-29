using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class GuildGradeChart : IChart<int>
    {
        public ObscuredInt GuildGradeId;
        public ObscuredString GuildGrade;
        public ObscuredInt GuildGradeUpGxp;
        public ObscuredInt GuildNextGrade;
        public ObscuredString Icon;
        
        public int GetID()
        {
            return GuildGradeId;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Guild_Grade_Id"].ToString(), out int guildGradeId);
            GuildGradeId = guildGradeId;

            GuildGrade = jsonData["Guild_Grade"].ToString();

            int.TryParse(jsonData["Grade_Up_Gxp"].ToString(), out int guildGradeUpGxp);
            GuildGradeUpGxp = guildGradeUpGxp;

            int.TryParse(jsonData["Grade_Up_To"].ToString(), out int guildNextGrade);
            GuildNextGrade = guildNextGrade;

            Icon = jsonData["Icon"].ToString();

            ChartManager.GuildGradeCharts.TryAdd(GetID(), this);
        }
    }
}