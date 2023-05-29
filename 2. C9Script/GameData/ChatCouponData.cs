using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;
using BackEnd;

namespace GameData
{
    public class ChatCouponData
    {
        public int Id;

        private ObscuredBool _isChat;

        public bool IsChat
        {
            get => _isChat;
            set { _isChat = value; }
        }
    }

    public class ChatCouponGameData : BaseGameData
    {
        public override string TableName => "ChatCoupon";

        protected override string InDate { get; set; }

        private string CouponKey(int id) => $"Coupon_{id.ToString()}";

        protected override Param MakeInitData()
        {
            Param param = new Param();

            foreach (var chatCouponChart in ChartManager.ChatCouponCharts.Values)
            {
                ChatCouponData data = new ChatCouponData()
                {
                    Id = chatCouponChart.Id,
                    IsChat = false
                };

                param.Add(CouponKey(data.Id), data.IsChat);

                Managers.Game.ChatCouponDatas.Add(data.Id, data.IsChat);
            }

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param();

            //foreach (KeyValuePair<string, bool> pair in Managers.Game.ChatCouponDatas)
            //{
            //    param.Add(pair.Key, pair.Value);
            //}


            foreach (var chatCouponData in Managers.Game.ChatCouponDatas)
            {
                param.Add(CouponKey(chatCouponData.Key), chatCouponData.Value);
            }

            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            foreach (int chatCouponChart in ChartManager.ChatCouponCharts.Keys)
            {
                ChatCouponData chatCouponData = new();

                if (!jsonData.ContainsKey(CouponKey(chatCouponChart)))
                {
                    chatCouponData.Id = chatCouponChart;
                    chatCouponData.IsChat = false;

                    param.Add(CouponKey(chatCouponChart), chatCouponData.IsChat);
                }
                else
                {
                    chatCouponData.Id = chatCouponChart;
                    chatCouponData.IsChat = bool.Parse(jsonData[CouponKey(chatCouponChart)].ToString());
                }

                if (!Managers.Game.ChatCouponDatas.ContainsKey(chatCouponChart))
                    Managers.Game.ChatCouponDatas.Add(chatCouponChart, chatCouponData.IsChat);
                else
                    Managers.Game.ChatCouponDatas[chatCouponChart] = chatCouponData.IsChat;
            }

            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}


