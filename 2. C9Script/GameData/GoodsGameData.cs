using System.Collections.Generic;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using UniRx;

namespace GameData
{
    public class GoodsGameData : BaseGameData
    {
        public override string TableName => "Goods";
        protected override string InDate { get; set; }

        private string GoodsKey(int id) => $"Goods_{id.ToString()}";

        private const string RaidTicketFlag = "RaidFlag";
        private const int RaidTicketFlagValue = 1;
        
        protected override Param MakeInitData()
        {
            Param param = new Param();
            
            foreach (var chartData in ChartManager.GoodsCharts)
            {
                int initValue = 0;

                switch (chartData.Key)
                {
                    case (int)Goods.Gold:
                        initValue = (int)ChartManager.SystemCharts[SystemData.Default_Gold].Value.GetDecrypted();
                        break;
                    case (int)Goods.StarBalloon:
                        initValue = (int)ChartManager.SystemCharts[SystemData.Default_Balloon].Value.GetDecrypted();
                        break;
                    case (int)Goods.Dungeon1Ticket:
                    case (int)Goods.Dungeon2Ticket:
                    case (int)Goods.Dungeon3Ticket:
                        initValue = (int)ChartManager.SystemCharts[SystemData.Dungeon_Ticket_Daily_Maxcount].Value.GetDecrypted();
                        break;
                    case (int)Goods.PvpTicket:
                        initValue = (int)ChartManager.SystemCharts[SystemData.Pvp_Ticket_Daily_Maxcount].Value.GetDecrypted(); 
                        break;
                    case (int)Goods.RaidTicket:
                        initValue = (int)ChartManager.SystemCharts[SystemData.Raid_Ticket_Daily_Count].Value.GetDecrypted();
                        break;
                    case (int)Goods.GuildRaidTicket:
                        initValue = (int)ChartManager.SystemCharts[SystemData.Guild_Raid_Ticket_Daily_Count].Value.GetDecrypted();
                        break;
                    case (int)Goods.GuildAllRaidTicket:
                        initValue = (int)ChartManager.SystemCharts[SystemData.All_Raid_Ticket_Daily_Count].Value.GetDecrypted();
                        break;
                    case (int)Goods.GuildSportsTicket:
                        initValue = (int)ChartManager.SystemCharts[SystemData.Guild_Sports_Ticket_Daily_Count].Value.GetDecrypted();
                        break;
                }
                
                param.Add(GoodsKey(chartData.Key), initValue);
            }

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            foreach (var chartData in ChartManager.GoodsCharts)
            {
                param.Add(GoodsKey(chartData.Key), Managers.Game.GoodsDatas[chartData.Key].Value.GetDecrypted());
            }

            return param;
        }

        protected override Param MakeSaveData(int id)
        {
            var param = new Param();
            
            if (Managers.Game.GoodsDatas.TryGetValue(id, out var goodsData))
                param.Add(GoodsKey(id), goodsData.Value.GetDecrypted());

            return param;
        }

        protected override Param MakeSaveData(List<int> ids)
        {
            var param = new Param();
            
            ids.ForEach(id =>
            {
                var goodsKey = GoodsKey(id);

                if (param.Contains(goodsKey))
                    return;

                if (!Managers.Game.GoodsDatas.TryGetValue(id, out var goodsData))
                    return;
                
                param.Add(goodsKey, goodsData.Value.GetDecrypted());
            });

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            // 기존 데이터 초기화
            for (int i = 16; i <= 23; i++)
            {
                if (jsonData.ContainsKey(GoodsKey(i)))
                {
                    if (!param.ContainsKey(GoodsKey(i)))
                        param.Add(GoodsKey(i), string.Empty);
                }
            }

            foreach (var chartData in ChartManager.GoodsCharts)
            {
                double value = 0;
                
                if (!jsonData.ContainsKey(GoodsKey(chartData.Key)) || string.IsNullOrEmpty(jsonData[(GoodsKey(chartData.Key))].ToString()))
                {
                    switch (chartData.Key)
                    {
                        case (int)Goods.Gold:
                            value = (int)ChartManager.SystemCharts[SystemData.Default_Gold].Value.GetDecrypted();
                            break;
                        case (int)Goods.StarBalloon:
                            value = (int)ChartManager.SystemCharts[SystemData.Default_Balloon].Value.GetDecrypted();
                            break;
                        case (int)Goods.Dungeon1Ticket:
                        case (int)Goods.Dungeon2Ticket:
                        case (int)Goods.Dungeon3Ticket:
                            value = (int)ChartManager.SystemCharts[SystemData.Dungeon_Ticket_Daily_Maxcount].Value.GetDecrypted();
                            break;
                        case (int)Goods.PvpTicket:
                            value = (int)ChartManager.SystemCharts[SystemData.Pvp_Ticket_Daily_Maxcount].Value.GetDecrypted(); 
                            break;
                        case (int)Goods.RaidTicket:
                            value = (int)ChartManager.SystemCharts[SystemData.Raid_Ticket_Daily_Count].Value.GetDecrypted();
                            break;
                        case (int)Goods.GuildRaidTicket:
                            value = (int)ChartManager.SystemCharts[SystemData.Guild_Raid_Ticket_Daily_Count].Value.GetDecrypted();
                            break;
                        case (int)Goods.GuildAllRaidTicket:
                            value = (int)ChartManager.SystemCharts[SystemData.All_Raid_Ticket_Daily_Count].Value.GetDecrypted();
                            break;
                        case (int)Goods.GuildSportsTicket:
                            value = (int)ChartManager.SystemCharts[SystemData.Guild_Sports_Ticket_Daily_Count].Value.GetDecrypted();
                            break;
                    }
                    
                    param.Add(GoodsKey(chartData.Key), value);
                }
                else
                {
                    double.TryParse(jsonData[GoodsKey(chartData.Key)].ToString(), out value);
                }

                if (!Managers.Game.GoodsDatas.ContainsKey(chartData.Key))
                    Managers.Game.GoodsDatas.Add(chartData.Key, new ReactiveProperty<ObscuredDouble>(value));
                else
                    Managers.Game.GoodsDatas[chartData.Key].Value = value;
            }

            // if (!jsonData.ContainsKey(RaidTicketFlag))
            // {
            //     Managers.Game.GoodsDatas[(int)Goods.RaidTicket].Value =
            //         (int)ChartManager.SystemCharts[SystemData.Raid_Ticket_Daily_Count].Value.GetDecrypted();
            //     
            //     if (param.ContainsKey(GoodsKey((int)Goods.RaidTicket)))
            //         param.Remove(GoodsKey((int)Goods.RaidTicket));
            //     param.Add(GoodsKey((int)Goods.RaidTicket), Managers.Game.GoodsDatas[(int)Goods.RaidTicket].Value.GetDecrypted());
            //     
            //     if (!param.ContainsKey(RaidTicketFlag))
            //         param.Add(RaidTicketFlag, RaidTicketFlagValue);
            // }
            // else
            // {
            //     if (int.TryParse(jsonData[RaidTicketFlag].ToString(), out int raidFlag))
            //     {
            //         if (raidFlag < RaidTicketFlagValue)
            //         {
            //             Managers.Game.GoodsDatas[(int)Goods.RaidTicket].Value =
            //                 (int)ChartManager.SystemCharts[SystemData.Raid_Ticket_Daily_Count].Value.GetDecrypted();
            //
            //             if (param.ContainsKey(GoodsKey((int)Goods.RaidTicket)))
            //                 param.Remove(GoodsKey((int)Goods.RaidTicket));
            //             param.Add(GoodsKey((int)Goods.RaidTicket),
            //                 Managers.Game.GoodsDatas[(int)Goods.RaidTicket].Value.GetDecrypted());
            //
            //             if (!param.ContainsKey(RaidTicketFlag))
            //                 param.Add(RaidTicketFlag, RaidTicketFlagValue);
            //         }
            //     }
            // }

            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}