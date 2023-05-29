using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;

public class PostData
{
    public string Title;
    public string Content;
    public DateTime ExpirationDateTime;
    public string InDate;
    public BackEnd.PostType PostType;
    public ItemType ItemType;
    public int ItemId;
    public double ItemValue;

    public PostData(JsonData postJson, BackEnd.PostType postType)
    {
        PostType = postType;
        
        Title = postJson["title"].ToString();
        Content = postJson["content"].ToString();
        ExpirationDateTime = DateTime.Parse(postJson["expirationDate"].ToString());
        InDate = postJson["inDate"].ToString();

        for (int i = 0; i < postJson["items"].Count; i++)
        {
            JsonData itemData = postJson["items"][i];

            ItemType = Enum.Parse<ItemType>(itemData["item"]["Item_Type"].ToString());
            ItemId = int.Parse(itemData["item"]["Item_Id"].ToString());
            ItemValue = double.Parse(itemData["itemCount"].ToString());
        }
    }

    public string GetExpirationTimeString()
    {
        TimeSpan gap = ExpirationDateTime - Utils.GetNow();
        
        if (gap.Days > 0)
            return $"{gap.Days}일 {gap.Hours}시";

        if (gap.Hours > 0)
            return $"{gap.Hours}시 {gap.Minutes}분";

        return $"{gap.Minutes}분";
    }
}

public class PostManager
{
    private readonly List<PostData> _postDatas = new();
    public List<PostData> PostDatas => _postDatas;

    public void Init()
    {
        GetPostList();
    }

    public void GetPostList(Action endCallback = null)
    {
        Backend.UPost.GetPostList(BackEnd.PostType.Admin, 100, bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog("Fail Admin Post", bro);
                return;
            }
            
            _postDatas.Clear();

            JsonData jsonData = bro.GetFlattenJSON();

            for (int i = 0; i < jsonData["postList"].Count; i++)
            {
                JsonData postJson = jsonData["postList"][i];
                PostData postData = new PostData(postJson, BackEnd.PostType.Admin);
                _postDatas.Add(postData);
            }

            Backend.UPost.GetPostList(BackEnd.PostType.Rank, 100, bro2 =>
            {
                if (!bro2.IsSuccess())
                {
                    Managers.Backend.FailLog("Fail Admin Post", bro2);
                    return;
                }

                JsonData jsonData2 = bro2.GetFlattenJSON();

                for (int i = 0; i < jsonData2["postList"].Count; i++)
                {
                    JsonData postJson = jsonData2["postList"][i];
                    PostData postData = new PostData(postJson, BackEnd.PostType.Rank);
                    _postDatas.Add(postData);
                }
                endCallback?.Invoke();
            });
        });
    }
}