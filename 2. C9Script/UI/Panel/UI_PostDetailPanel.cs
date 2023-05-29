using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_PostDetailPanel : UI_Panel
{
    [SerializeField] private TMP_Text SenderText;
    [SerializeField] private TMP_Text TitleText;
    [SerializeField] private TMP_Text ContentText;
    [SerializeField] private TMP_Text RewardValueText;

    [SerializeField] private Image RewardImage;

    [SerializeField] private Button ReceiveButton;
    [SerializeField] private Button CloseButton;

    private PostData _postData;
    private UI_PostItem _uiPostItem;
    private Action<UI_PostItem> _onReceiveCallback;

    private void Start()
    {
        ReceiveButton.BindEvent(OnClickReceive);
        CloseButton.BindEvent(Close);
    }

    public void Init(UI_PostItem uiPostItem, Action<UI_PostItem> onReceiveCallback)
    {
        _uiPostItem = uiPostItem;
        _postData = uiPostItem.PostData;
        _onReceiveCallback = onReceiveCallback;

        SetUI();
    }

    private void SetUI()
    {
        SenderText.text = "From. 운영자";
        TitleText.text = _postData.Title;
        ContentText.text = _postData.Content;

        RewardImage.sprite = Managers.Resource.LoadItemIcon(_postData.ItemType, _postData.ItemId);
        RewardValueText.text = _postData.ItemValue.ToCurrencyString();
    }

    private void OnClickReceive()
    {
        FadeScreen.Instance.OnLoadingScreen();
        
        Backend.UPost.ReceivePostItem(_postData.PostType, _postData.InDate, bro =>
        {
            if (!bro.IsSuccess())
            {
                FadeScreen.Instance.OffLoadingScreen();
                if (bro.GetMessage().Contains("post not found"))
                {
                    Managers.Message.ShowMessage("잘못된 우편 입니다.");
                    _onReceiveCallback?.Invoke(_uiPostItem);
                }
                else
                {
                    Managers.Backend.FailLog($"Fail ReceivePost {_postData.InDate}", bro);
                    return;
                }
            }
            
            JsonData jsonData = bro.GetFlattenJSON();

            var gainItem = new List<(ItemType, int, double)>();
            for (int i = 0; i < jsonData["postItems"].Count; i++)
            {
                ItemType itemType = Enum.Parse<ItemType>(jsonData["postItems"][i]["item"]["Item_Type"].ToString());
                int itemId = int.Parse(jsonData["postItems"][i]["item"]["Item_Id"].ToString());
                double itemValue = double.Parse(jsonData["postItems"][i]["itemCount"].ToString());
                
                gainItem.Add((itemType, itemId, itemValue));

                Managers.Game.IncreaseItem(itemType, itemId, itemValue);
                GameDataManager.SaveItemData(itemType);
            }
            
            var param = new Param();
            param.Add("GainPost", _postData.Title);

            for (int i = 0; i < gainItem.Count; i++)
                param.Add($"GainItem_{i}", gainItem[i]);

            Utils.GetGoodsLog(ref param);
            Backend.GameLog.InsertLog("Post", param);

            Managers.Post.PostDatas.Remove(_postData);
            FadeScreen.Instance.OffLoadingScreen();
            _onReceiveCallback?.Invoke(_uiPostItem);
        });
        
        // Backend.Social.Post.ReceiveAdminPostItemV2(_postData.InDate, bro =>
        // {
        //     if (!bro.IsSuccess())
        //     {
        //         if (bro.GetMessage().Contains("post not found"))
        //         {
        //             Managers.Message.ShowMessage("잘못된 우편 입니다.");
        //             _onReceiveCallback?.Invoke(_uiPostItem);
        //         }
        //         else
        //         {
        //             Managers.Backend.FailLog($"Fail ReceivePost {_postData.InDate}", bro);
        //             return;
        //         }
        //     }
        //
        //     JsonData jsonData = bro.GetFlattenJSON();
        //
        //     ItemType itemType = Enum.Parse<ItemType>(jsonData["item"]["Item_Type"].ToString());
        //     int itemId = int.Parse(jsonData["item"]["Item_Id"].ToString());
        //     double itemValue = double.Parse(jsonData["itemCount"].ToString());
        //
        //     var gainItem = new List<(ItemType, int, double)>();
        //     gainItem.Add((itemType, itemId, itemValue));
        //     var param = new Param();
        //     param.Add("GainPost", _postData.Title);
        //     
        //     for (int i = 0 ; i < gainItem.Count; i++)
        //         param.Add($"GainItem_{i}", gainItem[i]);
        //     
        //     Utils.GetGoodsLog(ref param);
        //     Backend.GameLog.InsertLog("Post", param);
        //
        //     Managers.Game.IncreaseItem(itemType, itemId, itemValue);
        //     GameDataManager.SaveItemData(itemType);
        //     Managers.Post.PostDatas.Remove(_postData);
        //
        //     Param parma = new Param();
        //
        //     _onReceiveCallback?.Invoke(_uiPostItem);
        // });
    }
}