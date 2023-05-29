using System.Text;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using UniRx;
using UnityEngine.U2D.Animation;

namespace GameData
{
    public class StatLevelGameData : BaseGameData
    {
        public override string TableName => "StatLevel";
        protected override string InDate { get; set; }

        protected override Param MakeInitData()
        {
            Param param = new Param();

            param.Add(StatType.Attack.ToString(), 0);
            
            for (StatType statType = StatType.CriticalRate2; statType <= StatType.CriticalDamage9009; statType++)
            {
                param.Add(statType.ToString(), 0);
            }
            
            foreach (var statUpgradeGoldChart in ChartManager.StatGoldUpgradeCharts.Values)
            {
                if (param.ContainsKey(((StatType)statUpgradeGoldChart.StatId.GetDecrypted()).ToString()))
                    continue;

                param.Add(((StatType)statUpgradeGoldChart.StatId.GetDecrypted()).ToString(), 0);
            }

            foreach (var statUpgradePointChart in ChartManager.StatPointUpgradeCharts.Values)
            {
                if (param.ContainsKey(((StatType)statUpgradePointChart.StatId.GetDecrypted()).ToString()))
                    continue;

                param.Add(((StatType)statUpgradePointChart.StatId.GetDecrypted()).ToString(), 0);
            }

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            foreach (var statLevelData in Managers.Game.StatLevelDatas)
            {
                StatType statType = (StatType)statLevelData.Key;
                param.Add(statType.ToString(), statLevelData.Value.Value.GetDecrypted());
            }

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            if (jsonData.ContainsKey(StatType.Attack.ToString()))
            {
                int.TryParse(jsonData[StatType.Attack.ToString()].ToString(), out int attackLevel);

                if (Managers.Game.StatLevelDatas.ContainsKey((int)StatType.Attack))
                    Managers.Game.StatLevelDatas[(int)StatType.Attack].Value = attackLevel;
                else
                    Managers.Game.StatLevelDatas.Add((int)StatType.Attack, new ReactiveProperty<ObscuredLong>(attackLevel));
            }
            else
            {
                if (Managers.Game.StatLevelDatas.ContainsKey((int)StatType.Attack))
                    Managers.Game.StatLevelDatas[(int)StatType.Attack].Value = 0;
                else
                    Managers.Game.StatLevelDatas.Add((int)StatType.Attack, new ReactiveProperty<ObscuredLong>(0));

                param.Add(StatType.Attack.ToString(), 0);
            }

            for (StatType statType = StatType.CriticalRate2; statType <= StatType.CriticalDamage9009; statType++)
            {
                int statLevel = 0;
                if (jsonData.ContainsKey(statType.ToString()))
                    int.TryParse(jsonData[statType.ToString()].ToString(), out statLevel);
                else
                {
                    statLevel = 0;
                    param.Add(statType.ToString(), statLevel);
                }

                if (Managers.Game.StatLevelDatas.ContainsKey((int)statType))
                    Managers.Game.StatLevelDatas[(int)statType].Value = statLevel;
                else
                    Managers.Game.StatLevelDatas.Add((int)statType, new ReactiveProperty<ObscuredLong>(statLevel));
            }
            
            for (StatType statType = StatType.CriticalRate10009; statType <= StatType.CriticalDamage14009; statType++)
            {
                int statLevel = 0;
                if (jsonData.ContainsKey(statType.ToString()))
                    int.TryParse(jsonData[statType.ToString()].ToString(), out statLevel);
                else
                {
                    statLevel = 0;
                    param.Add(statType.ToString(), statLevel);
                }

                if (Managers.Game.StatLevelDatas.ContainsKey((int)statType))
                    Managers.Game.StatLevelDatas[(int)statType].Value = statLevel;
                else
                    Managers.Game.StatLevelDatas.Add((int)statType, new ReactiveProperty<ObscuredLong>(statLevel));
            }

            foreach (var statPointUpgradeChart in ChartManager.StatPointUpgradeCharts.Values)
            {
                int statLevel = 0;
                StatType statType = (StatType)statPointUpgradeChart.StatId.GetDecrypted();

                if (jsonData.ContainsKey(statType.ToString()))
                    int.TryParse(jsonData[statType.ToString()].ToString(), out statLevel);
                else
                {
                    statLevel = 0;
                    param.Add(statType.ToString(), statLevel);
                }
                
                if (Managers.Game.StatLevelDatas.ContainsKey((int)statType))
                    Managers.Game.StatLevelDatas[(int)statType].Value = statLevel;
                else
                    Managers.Game.StatLevelDatas.Add((int)statType, new ReactiveProperty<ObscuredLong>(statLevel));
            }

            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}