using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    //public class StageBossChart : IChart<int>
    //{
    //    public ObscuredInt Id;
    //    public ObscuredInt BossId;
    //    public ObscuredDouble BossHp;
    //    public ObscuredDouble BossAttack;
    //    public ObscuredInt StageClearLimitTime;
    //    public ObscuredInt[] ClearRewardIds;
    //    public ObscuredDouble[] ClearRewardValues;
        
    //    public int GetID()
    //    {
    //        return Id;
    //    }

    //    public void SetData(JsonData jsonData)
    //    {
    //        int.TryParse(jsonData["StageTable_Id"]["S"].ToString(), out int id);
    //        Id = id;
            
    //        int.TryParse(jsonData["Boss_Id"]["S"].ToString(), out int bossId);
    //        BossId = bossId;
            
    //        double.TryParse(jsonData["Boss_Hp"]["S"].ToString(), out double bossHp);
    //        BossHp = bossHp;
            
    //        double.TryParse(jsonData["Boss_Attack"]["S"].ToString(), out double bossAttack);
    //        BossAttack = bossAttack;
            
    //        int.TryParse(jsonData["Stage_Clear_Limit_Time"]["S"].ToString(), out int stageClearLimitTime);
    //        StageClearLimitTime = stageClearLimitTime;
            
    //        int[] clearRewardIds = Array.ConvertAll(jsonData["BossClear_Reward_Id"]["S"].ToString().Trim().Split(','), int.Parse);
    //        ClearRewardIds = new ObscuredInt[clearRewardIds.Length];
    //        for (int i = 0; i < clearRewardIds.Length; i++)
    //            ClearRewardIds[i] = clearRewardIds[i];
            
    //        double[] clearRewardValues = Array.ConvertAll(jsonData["BossClear_Reward_Value"]["S"].ToString().Trim().Split(','), double.Parse);
    //        ClearRewardValues = new ObscuredDouble[clearRewardValues.Length];
    //        for (int i = 0; i < clearRewardValues.Length; i++)
    //            ClearRewardValues[i] = clearRewardValues[i];

    //        ChartManager.StageBossCharts[GetID()] = this;
    //    }
    //}
}