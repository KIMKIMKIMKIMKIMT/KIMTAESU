using BackEnd;
using LitJson;

namespace GameData
{
    public class UnlimitedStatLevelGameData : BaseGameData
    {
        public override string TableName => "UnlimitedStatLevel";
        protected override string InDate { get; set; }
        protected override Param MakeInitData()
        {
            var param = new Param();

            foreach (var unlimitedStatUpgradeChart in ChartManager.UnlimitedPointUpgradeCharts.Values)
            {
                string statId = ((StatType)unlimitedStatUpgradeChart.StatId.GetDecrypted()).ToString();
                
                if (param.ContainsKey(statId))
                    continue;
                
                param.Add(statId, 0);
            }

            return param;
        }

        protected override Param MakeSaveData()
        {
            var param = new Param();

            foreach (var unlimitedStatLevelData in Managers.Game.UnlimitedStatLevelDatas)
            {
                string statId = ((StatType)unlimitedStatLevelData.Key).ToString();

                if (param.ContainsKey(statId))
                    continue;
                
                param.Add(statId, unlimitedStatLevelData.Value.Value.GetDecrypted());
            }

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            var param = new Param();
            
            foreach (var unlimitedStatUpgradeChart in ChartManager.UnlimitedPointUpgradeCharts.Values)
            {
                string statId = ((StatType)unlimitedStatUpgradeChart.StatId.GetDecrypted()).ToString();
                long statLv;

                if (jsonData.ContainsKey(statId))
                {
                    long.TryParse(jsonData[statId].ToString(), out statLv);
                }
                else
                {
                    param.Add(statId, 0);
                    statLv = 0;
                }

                Managers.Game.UnlimitedStatLevelDatas.TryAdd(unlimitedStatUpgradeChart.StatId, new(statLv));
            }
            
            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}