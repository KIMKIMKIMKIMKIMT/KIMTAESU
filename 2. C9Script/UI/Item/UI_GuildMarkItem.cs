using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildMarkItem : UI_Base
{
    [SerializeField] private Image GuildMarkImage;
    
    [SerializeField] private Button MarkButton;

    [SerializeField] private GameObject SelectObj;

    private int _guildMarkId;

    public int GuildMarkId => _guildMarkId;
    private Action<UI_GuildMarkItem> _clickCallback;

    private void Start()
    {
        MarkButton.BindEvent(() =>
        {
            _clickCallback?.Invoke(this);
            SetSelect(true);
        });
    }

    public void Init(int guildMarkId, Action<UI_GuildMarkItem> clickCallback)
    {
        _guildMarkId = guildMarkId;
        _clickCallback = clickCallback;
        
        SetUI();
    }

    private void SetUI()
    {
        // 차트 적용전까지 임시 코드
        GuildMarkImage.sprite = Managers.Resource.LoadGuildMarkIcon(_guildMarkId);
    }

    public void SetSelect(bool isActive)
    {
        SelectObj.SetActive(isActive);
    }
}