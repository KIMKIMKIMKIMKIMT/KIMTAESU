using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public class ChatCouponChart : IChart<int>
{
    public ObscuredInt Id;
    public string ChatKey;
    public ItemType ItemType;
    public ObscuredInt ItemId;
    public ObscuredDouble ItemValue;


    public int GetID()
    {
        return Id;
    }

    

    public void SetData(JsonData jsonData)
    {
        int.TryParse(jsonData["ChatCouponId"].ToString(), out int id);
        Id = id;

        ChatKey = jsonData["Chat"].ToString();

        Enum.TryParse(jsonData["ItemType"].ToString(), out ItemType);

        int.TryParse(jsonData["RewardId"].ToString(), out int itemId);
        ItemId = itemId;

        double.TryParse(jsonData["RewardValue"].ToString(), out double itemValue);
        ItemValue = itemValue;

        ChartManager.ChatCouponCharts[GetID()] = this;
    }
}
