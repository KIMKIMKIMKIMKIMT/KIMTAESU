using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PostItem : UI_Base
{
    [SerializeField] private TMP_Text SenderText;
    [SerializeField] private TMP_Text TitleText;
    [SerializeField] private TMP_Text ExpiredTimeText;
    [SerializeField] private TMP_Text RewardValueText;

    [SerializeField] private Image RewardImage;

    [SerializeField] private Button ShowDetailButton;

    private PostData _postData;
    public PostData PostData => _postData;
    private Action<UI_PostItem> _onDetailCallback;

    private void Start()
    {
        ShowDetailButton.BindEvent(() => _onDetailCallback?.Invoke(this));
    }

    public void Init(PostData postData, Action<UI_PostItem> onDetailCallback)
    {
        _postData = postData;
        _onDetailCallback = onDetailCallback;

        SetUI();
    }

    private void SetUI()
    {
        SenderText.text = $"From. 운영자";
        TitleText.text = _postData.Title;
        ExpiredTimeText.text = $"남은 기간 : {_postData.GetExpirationTimeString()}";

        RewardImage.sprite = Managers.Resource.LoadItemIcon(_postData.ItemType, _postData.ItemId);
        RewardValueText.text = _postData.ItemValue.ToCurrencyString();
    }
}