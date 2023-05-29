using BackEnd;
using LitJson;

namespace GameData
{
    public class AccountGameData : BaseGameData
    {
        public override string TableName => "Account";
        protected override string InDate { get; set; }
        protected override Param MakeInitData()
        {
            var param = new Param()
            {
                { "IdToken", Managers.Server.IdToken }
            };

            return param;
        }

        protected override Param MakeSaveData()
        {
            var param = new Param()
            {
                { "IdToken", Managers.Server.IdToken }
            };

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            
        }
    }
}