using System;
using System.Collections.Generic;
using BackEnd;
using GameData;
using LitJson;
using TMPro;
using UI;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_CouponPopup : UI_Popup
{
    [SerializeField] private TMP_InputField CouponInput;
    [SerializeField] private TMP_Text FailReasonText;

    [SerializeField] private Button CloseButton;
    [SerializeField] private Button BackgroundButton;
    [SerializeField] private Button PopupButton;
    [SerializeField] private Button ReceiveButton;

    [SerializeField] private GameObject InputPanelObj;
    [SerializeField] private GameObject SuccessPanelObj;
    [SerializeField] private GameObject FailPanelObj;

    public override bool isTop => true;

    private void Start()
    {
        CloseButton.BindEvent(ClosePopup);
        BackgroundButton.BindEvent(ClosePopup);
        PopupButton.BindEvent(OnClickPopup);
        ReceiveButton.BindEvent(OnClickReceive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();       
        }
    }

    public override void Open()
    {
        base.Open();
        
        InputPanelObj.SetActive(true);
        SuccessPanelObj.SetActive(false);
        FailPanelObj.SetActive(false);
    }

    private void OnClickReceive()
    {
        string coupon = CouponInput.text;

        if (string.IsNullOrEmpty(coupon))
        {
            Managers.Message.ShowMessage("쿠폰 번호를 입력하세요");
            return;
        }
        
        CouponInput.text = String.Empty;
        Backend.Coupon.UseCoupon(coupon, bro =>
        {
            string failReason = string.Empty;
            
            if (!bro.IsSuccess())
            {
                if (bro.GetMessage().Contains("이미 사용되었거나, 틀린 번호입니다."))
                {
                    failReason = "(이미 사용되었거나 만료, 틀린 번호 입니다.)";
                }
                else if (bro.GetMessage().Contains("전부 사용 된 쿠폰입니다"))
                {
                    failReason = "(모두 사용된 쿠폰입니다.)";
                }
                else if (bro.GetMessage().Contains("이미 사용하신 쿠폰입니다."))
                {
                    failReason = "(이미 사용한 쿠폰입니다.)";
                }
                else if (bro.GetMessage().Contains("bad couponNumber"))
                {
                    failReason = "(잘못된 쿠폰 번호 입니다.)";
                }
                else
                {
                    Managers.Backend.FailLog("Fail Coupon", bro);
                }
                
                SetFailPanel(failReason);
                return;
            }

            JsonData jsonData = bro.GetFlattenJSON()["itemObject"];

            List<ItemType> itemTypes = new List<ItemType>();

            Dictionary<(ItemType, int), double> gainItems = new();

            List<(ItemType, int, double)> logDatas = new();

            for (int i = 0; i < jsonData.Count; i++)
            {
                ItemType itemType = Enum.Parse<ItemType>(jsonData[i]["item"]["Item_Type"].ToString());
                int itemId = int.Parse(jsonData[i]["item"]["Item_Id"].ToString());
                int itemValue = int.Parse(jsonData[i]["itemCount"].ToString());
                
                Managers.Game.IncreaseItem(itemType, itemId, itemValue);
                
                logDatas.Add((itemType, itemId, itemValue));
                
                if (!itemTypes.Contains(itemType))
                    itemTypes.Add(itemType);

                if (gainItems.ContainsKey((itemType, itemId)))
                    gainItems[(itemType, itemId)] += itemValue;
                else
                    gainItems[(itemType, itemId)] = itemValue;
            }

            Param param = new Param();
            
            param.Add("CouponName", coupon);
            param.Add("GainItem", logDatas);
            Utils.GetGoodsLog(ref param);
            
            Backend.GameLog.InsertLog("Coupon", param);

            itemTypes.ForEach(GameDataManager.SaveItemData);
            Managers.UI.ShowGainItems(gainItems);

            SetSuccessPanel();
        });
    }

    private void SetSuccessPanel()
    {
        InputPanelObj.SetActive(false);
        SuccessPanelObj.SetActive(true);
        FailPanelObj.SetActive(false);
    }

    private void SetFailPanel(string failReason)
    {
        InputPanelObj.SetActive(false);
        SuccessPanelObj.SetActive(false);
        FailPanelObj.SetActive(true);

        FailReasonText.text = failReason;
    }

    private void OnClickPopup()
    {
        if (SuccessPanelObj.activeSelf)
        {
            SuccessPanelObj.SetActive(false);
        }
        else if (FailPanelObj.activeSelf)
        {
            FailPanelObj.SetActive(false);
        }
        
        if (!InputPanelObj.activeSelf)
            InputPanelObj.SetActive(true);
    }
}