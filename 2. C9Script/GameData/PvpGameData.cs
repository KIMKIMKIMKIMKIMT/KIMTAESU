using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Newtonsoft.Json;

namespace GameData
{
    public class PvpGameData : BaseGameData
    {
        public override string TableName => "PvpInfo";
        protected override string InDate { get; set; }
        protected override Param MakeInitData()
        {
            Param param = new Param();

            Dictionary<int, float> defaultStatData = new();

            foreach (int statType in Enum.GetValues(typeof(StatType)))
            {
                if (defaultStatData.ContainsKey(statType))
                    defaultStatData[statType] = 0;
                else
                    defaultStatData.Add(statType, 0);
            }

            Dictionary<EquipType, int> defaultEquipData = new();

            foreach (EquipType equipType in Enum.GetValues(typeof(EquipType)))
            {
                if (defaultEquipData.ContainsKey(equipType))
                    defaultEquipData[equipType] = 0;
                else
                    defaultEquipData.Add(equipType, 0);
            }

            param.Add(PvpDataType.BaseStat.ToString(), JsonConvert.SerializeObject(defaultStatData));
            param.Add(PvpDataType.EquipItems.ToString(), JsonConvert.SerializeObject(defaultEquipData));
            param.Add(PvpDataType.PromoGrade.ToString(), 0);
            param.Add(PvpDataType.Lv.ToString(), 1);
            param.Add(PvpDataType.Passive_Godgod_Lv.ToString(), 0);

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();
            
            param.Add(PvpDataType.BaseStat.ToString(), JsonConvert.SerializeObject(Managers.Game.BaseStatDatas));
            param.Add(PvpDataType.EquipItems.ToString(), JsonConvert.SerializeObject(Managers.Game.EquipDatas));
            param.Add(PvpDataType.PromoGrade.ToString(), (int)Managers.Game.UserData.PromoGrade);
            param.Add(PvpDataType.Lv.ToString(), Managers.Game.UserData.Level);
            param.Add(PvpDataType.Passive_Godgod_Lv.ToString(), Managers.Game.SkillDatas[57].Level);

            return param;
        }

        public void GetGameData(string gamerInDate, Action<JsonData> callback)
        {
            Where where = new Where();
            
            where.Equal("owner_inDate", gamerInDate);
            where.Equal("Server", Managers.Server.CurrentServer);

            Backend.GameData.Get(TableName, where, bro =>
            {
                if (!bro.IsSuccess())
                {
                    Managers.Backend.FailLog("Fail PvpGameData Load", bro);
                    return;
                }
                
                callback?.Invoke(bro.GetFlattenJSON());
            });
        }

        protected override void SetGameData(JsonData jsonData)
        {
            
        }
    }
}