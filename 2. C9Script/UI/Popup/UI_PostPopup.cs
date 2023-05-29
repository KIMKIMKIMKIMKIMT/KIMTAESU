using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using GameData;
using LitJson;
using Newtonsoft.Json;
using UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class UI_PostPopup : UI_Popup
{
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button BackButton;
    [SerializeField] private Button AllReceiveButton;

    [SerializeField] private Transform PostItemRootTr;

    [SerializeField] private UI_PostDetailPanel UIPostDetailPanel;

    public override bool isTop => true;

    private readonly List<UI_PostItem> _uiPostItems = new();

    private void OnEnable()
    {
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.OpenMenu, (int)QuestOpenMenu.Post, 1));
    }

    private void Start()
    {
        CloseButton.BindEvent(ClosePopup);
        AllReceiveButton.BindEvent(OnClickAllReceive);
        BackButton.BindEvent(OnClickBack);

        UIPostDetailPanel.gameObject.OnDisableAsObservable().Subscribe(_ =>
        {
            BackButton.gameObject.SetActive(false);
        });
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIPostDetailPanel.gameObject.activeSelf)
            {
                UIPostDetailPanel.Close();
                return;
            }
            
            ClosePopup();
        }
    }

    public override void Open()
    {
        base.Open();
        
        BackButton.gameObject.SetActive(false);
        UIPostDetailPanel.gameObject.SetActive(false);
        
        SetUI();
    }

    private void SetUI()
    {
        _uiPostItems.ForEach(uiPostItem =>
        {
            if (uiPostItem != null)
                Destroy(uiPostItem);
        });
        
        _uiPostItems.Clear();
        PostItemRootTr.DestroyInChildren();
        
        Managers.Post.GetPostList(MakePostItems);
    }

    private void MakePostItems()
    {
        foreach (var postData in Managers.Post.PostDatas)
        {
            var uiPostItem = Managers.UI.MakeSubItem<UI_PostItem>(PostItemRootTr);
            uiPostItem.Init(postData, OnDetailCallback);
            _uiPostItems.Add(uiPostItem);
        }

        if (_uiPostItems.Count > 0)
        {
            AllReceiveButton.image.sprite = Managers.Resource.LoadSpriteInPopupAtlas("Button_002");
        }
        else
        {
            AllReceiveButton.image.sprite = Managers.Resource.LoadSpriteInPopupAtlas("Button_001");
        }
    }

    private void OnDetailCallback(UI_PostItem uiPostItem)
    {
        UIPostDetailPanel.Init(uiPostItem, OnReceiveCallback);
        UIPostDetailPanel.Open();

        BackButton.gameObject.SetActive(true);
    }

    private void OnReceiveCallback(UI_PostItem uiPostItem)
    {
        Destroy(uiPostItem.gameObject);
        _uiPostItems.Remove(uiPostItem);
        OnClickBack();
    }

    private void OnClickAllReceive()
    {
        if (Managers.Post.PostDatas.Count <= 0)
            return;
        
        FadeScreen.Instance.OnLoadingScreen();

        List<ItemType> receiveItems = new List<ItemType>();
            
        Param param = new Param();
        List<(ItemType, int, double)> gainItems = new();
        
        Backend.UPost.ReceivePostItemAll(BackEnd.PostType.Admin, bro =>
        {
            if (!bro.IsSuccess())
            {
                if (bro.GetStatusCode() != "404" && bro.GetErrorCode() != "NotFoundException")
                {
                    Managers.Backend.FailLog("Fail Receive AdminPost", bro);
                    FadeScreen.Instance.OffLoadingScreen();
                    return;
                }
            }

            JsonData jsonData = bro.GetFlattenJSON();

            if (jsonData != null)
            {
                // 우편 갯수
                for (int i = 0; i < jsonData["postItems"].Count; i++)
                {
                    // 우편내 아이템 갯수
                    for (int j = 0; j < jsonData["postItems"][i].Count; j++)
                    {
                        ItemType itemType =
                            Enum.Parse<ItemType>(jsonData["postItems"][i][j]["item"]["Item_Type"].ToString());
                        int itemId = int.Parse(jsonData["postItems"][i][j]["item"]["Item_Id"].ToString());
                        double itemValue = double.Parse(jsonData["postItems"][i][j]["itemCount"].ToString());

                        Managers.Game.IncreaseItem(itemType, itemId, itemValue);
                        gainItems.Add((itemType, itemId, itemValue));

                        if (!receiveItems.Contains(itemType))
                            receiveItems.Add(itemType);
                    }
                }
                
                receiveItems.ForEach(GameDataManager.SaveItemData);
            }
            
            receiveItems.Clear();

            Backend.UPost.ReceivePostItemAll(BackEnd.PostType.Rank, bro2 =>
            {
                if (!bro2.IsSuccess())
                {
                    if (bro2.GetStatusCode() != "404" && bro2.GetErrorCode() != "NotFoundException")
                    {
                        Managers.Backend.FailLog("Fail Receive RankPost", bro2);
                        Utils.GetGoodsLog(ref param);
                        Backend.GameLog.InsertLog("Post", param);

                        receiveItems.ForEach(GameDataManager.SaveItemData);
                        FadeScreen.Instance.OffLoadingScreen();
                        return;
                    }
                }

                JsonData jsonData2 = bro2.GetFlattenJSON();

                if (jsonData2 != null)
                {
                    for (int i = 0; i < jsonData2["postItems"].Count; i++)
                    {
                        for (int j = 0; j < jsonData2["postItems"][i].Count; j++)
                        {
                            ItemType itemType =
                                Enum.Parse<ItemType>(jsonData2["postItems"][i][j]["item"]["Item_Type"].ToString());
                            int itemId = int.Parse(jsonData2["postItems"][i][j]["item"]["Item_Id"].ToString());
                            double itemValue = double.Parse(jsonData2["postItems"][i][j]["itemCount"].ToString());

                            Managers.Game.IncreaseItem(itemType, itemId, itemValue);
                            gainItems.Add((itemType, itemId, itemValue));

                            if (!receiveItems.Contains(itemType))
                                receiveItems.Add(itemType);
                        }
                    }

                    receiveItems.ForEach(GameDataManager.SaveItemData);
                }
                
                if (!param.Contains("GainPost"))
                    param.Add("GainPost", Managers.Post.PostDatas.SelectMany(postData => postData.Title).ToList());
                
                for (int i = 0; i < gainItems.Count; i++)
                {
                    if (param.Contains($"GainItem_{i}"))
                        continue;
                    param.Add($"GainItem_{i}", gainItems[i]);
                }

                Utils.GetGoodsLog(ref param);
                Backend.GameLog.InsertLog("Post", param);

                _uiPostItems.ForEach(Destroy);
                _uiPostItems.Clear();
                Managers.Post.PostDatas.Clear();
                
                FadeScreen.Instance.OffLoadingScreen();
                
                SetUI();
            });
        });


        // Backend.Social.Post.ReceiveAdminPostAllV2(bro =>
        // {
        //     if (!bro.IsSuccess())
        //     {
        //         Managers.Backend.FailLog("Fail ReceiveAdminPostAll", bro);
        //         return;
        //     }
        //
        //     JsonData jsonData = bro.GetFlattenJSON();
        //
        //     List<ItemType> receiveItems = new List<ItemType>();
        //
        //     Param param = new Param();
        //     List<(ItemType, int, double)> gainItems = new();
        //
        //     for (int i = 0; i < jsonData["items"].Count; i++)
        //     {
        //         ItemType itemType = Enum.Parse<ItemType>(jsonData["items"][i]["item"]["Item_Type"].ToString());
        //         int itemId = int.Parse(jsonData["items"][i]["item"]["Item_Id"].ToString());
        //         double itemValue = double.Parse(jsonData["items"][i]["itemCount"].ToString());
        //         
        //         Managers.Game.IncreaseItem(itemType, itemId, itemValue);
        //         gainItems.Add((itemType, itemId, itemValue));
        //
        //         if (!receiveItems.Contains(itemType))
        //             receiveItems.Add(itemType);
        //     }
        //     
        //     param.Add("GainPost", Managers.Post.PostDatas.SelectMany(postData => postData.Title).ToList());
        //     for (int i = 0 ; i < gainItems.Count; i++)
        //         param.Add($"GainItem_{i}", gainItems[i]);
        //     Utils.GetGoodsLog(ref param);
        //     Backend.GameLog.InsertLog("Post", param);
        //     
        //     receiveItems.ForEach(GameDataManager.SaveItemData);
        //     
        //     _uiPostItems.ForEach(Destroy);
        //     _uiPostItems.Clear();
        //     Managers.Post.PostDatas.Clear();
        //
        //     SetUI();
        // });
    }

    private void OnClickBack()
    {
        UIPostDetailPanel.Close();
    }
}