using BackEnd;
using LitJson;

namespace GameData
{
    public class RankPvpGameData : BaseRankGameData
    {
        public override string TableName => "Ranking_PVP";
        protected override string InDate { get; set; }


        public override RankType RankType => RankType.Pvp;

        protected override Param MakeInitData()
        {
            Param param = new Param()
            {
                { "Score", 0 },
                {"MatchCount", 0}
            };

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param
            {
                { "Score", Managers.Pvp.PvpScore.GetDecrypted() },
                {"MatchCount", Managers.Pvp.MatchCount.GetDecrypted()}
            };

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();
            
            int.TryParse(jsonData["Score"].ToString(), out var pvpScore);
            Managers.Pvp.PvpScore = pvpScore;
            param.Add("Score", Managers.Pvp.PvpScore.GetDecrypted());

            if (jsonData.ContainsKey("MatchCount"))
            {
                int.TryParse(jsonData["MatchCount"].ToString(), out var matchCount);
                Managers.Pvp.MatchCount = matchCount;
            }
            else
            {
                Managers.Pvp.MatchCount = 0;
            }
            
            param.Add("MatchCount", Managers.Pvp.MatchCount.GetDecrypted());

            Backend.URank.User.UpdateUserScore(RankUUID, RankingServerTableName, InDate, param);

            Managers.Rank.UpdateMyRanking(RankType.Pvp, null);
        }

        public void AddScore(int value)
        {
            Param param = new Param();
            
            param.AddCalculation("Score", GameInfoOperator.addition, value);
            param.AddCalculation("MatchCount", GameInfoOperator.addition, 1);

            Backend.GameData.UpdateWithCalculationV2(ServerTableNameKey(Managers.Server.CurrentServer), InDate, Backend.UserInDate, param, bro =>
            {
                if (!bro.IsSuccess())
                {
                    Managers.Backend.FailLog("Fail Pvp AddScore", bro);
                    return;
                }
                
                GameDataManager.RankPvpGameData.LoadGameData();
            });
        }
    }
}