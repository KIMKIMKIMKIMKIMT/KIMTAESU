using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

#region Struct

[Serializable]
public struct UIItem
{
    public Image ItemImage;
    public TMP_Text ItemValueText;
}

#endregion

public class UI_ClearXMasDungeonPopup : UI_Popup
{
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private UIItem DefaultRewardItem;
    [SerializeField] private UIItem ScoreRewardItem;
    [SerializeField] private GameObject CloseObj;

    private void Start()
    {
        CloseObj.BindEvent(() =>
        {
            Managers.XMasEvent.CloseXMasEvent();
        });
    }

    public void Init(int score, int defaultItemId, double defaultItemValue, int scoreItemId, double scoreItemValue)
    {
        SetScore(score);
        SetDefaultItem(defaultItemId, defaultItemValue);
        SetScoreItem(scoreItemId, scoreItemValue);
    }

    private void SetScore(int score)
    {
        ScoreText.text = $"피한 눈 갯수 : {score}";    
    }
    
    private void SetDefaultItem(int defaultItemId, double defaultItemValue)
    {
        DefaultRewardItem.ItemImage.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, defaultItemId);
        DefaultRewardItem.ItemValueText.text = defaultItemValue.ToCurrencyString();
    }

    private void SetScoreItem(int scoreItemId, double scoreItemValue)
    {
        ScoreRewardItem.ItemImage.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, scoreItemId);
        ScoreRewardItem.ItemValueText.text = scoreItemValue.ToCurrencyString();
    }
}