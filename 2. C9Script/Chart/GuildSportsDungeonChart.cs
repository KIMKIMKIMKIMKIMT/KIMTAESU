using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chart;
using LitJson;
using CodeStage.AntiCheat.ObscuredTypes;
using System;

namespace Chart
{
    public class GuildSportsDungeonChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredInt WorldId;
        public ObscuredInt LimitTime;
        public ObscuredInt WinPoint;
        public ObscuredInt LosePoint;
        public ItemType ItemType;
        public ObscuredInt ItemId;
        public ObscuredDouble WinItemValue;
        public ObscuredDouble LoseItemValue;



        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Sports_Dungeon_Id"].ToString(), out int id);
            Id = id;

            int.TryParse(jsonData["World_Id"].ToString(), out int worldId);
            WorldId = worldId;

            int.TryParse(jsonData["Stage_Clear_Limit_Time"].ToString(), out int limitTime);
            LimitTime = limitTime;

            int.TryParse(jsonData["Win_Point"].ToString(), out int winPoint);
            WinPoint = winPoint;

            int.TryParse(jsonData["Lose_Point"].ToString(), out int losePoint);
            LosePoint = losePoint;

            Enum.TryParse(jsonData["Reward_Item_Type"].ToString(), out ItemType);

            int.TryParse(jsonData["Reward_Item_Id"].ToString(), out int itemId);
            ItemId = itemId;

            double.TryParse(jsonData["Win_Item_Value"].ToString(), out double winItemValue);
            WinItemValue = winItemValue;

            double.TryParse(jsonData["Lose_Item_Value"].ToString(), out double loseItemValue);
            LoseItemValue = loseItemValue;

            ChartManager.GuildSportsDungeonCharts[GetID()] = this;
        }
    }
}


