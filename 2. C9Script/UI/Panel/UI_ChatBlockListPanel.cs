
using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChatBlockListPanel : UI_Panel
{
    [SerializeField] private Transform UIChatBlockItemRoot;
    [SerializeField] private List<Button> CloseButtons; 

    private readonly List<UI_ChatBlockItem> _uiChatBlockItems = new();

    private void Start()
    {
        CloseButtons.ForEach(closeButton => closeButton.BindEvent(Close));
    }

    public override void Open()
    {
        base.Open();
        UpdateItems();
    }

    private void UpdateItems()
    {
        if (_uiChatBlockItems.Count <= 0)
            UIChatBlockItemRoot.DestroyInChildren();
        else
            _uiChatBlockItems.ForEach(uiChatBlockItem => uiChatBlockItem.gameObject.SetActive(false));
        
        JsonData jsonData = Backend.Chat.GetBlockUserList();

        List<UI_ChatBlockItem> uiChatBlockItems = new();

        for (int i = 0; i < jsonData.Count; i++)
        {
            UI_ChatBlockItem uiChatBlockItem;

            if (_uiChatBlockItems.Count > i)
                uiChatBlockItem = _uiChatBlockItems[i];
            else
            {
                uiChatBlockItem = Managers.UI.MakeSubItem<UI_ChatBlockItem>(UIChatBlockItemRoot);
                uiChatBlockItems.Add(uiChatBlockItem);
            }
            
            uiChatBlockItem.gameObject.SetActive(true);
            uiChatBlockItem.Init(jsonData[i].ToString(), Unblock);
        }
        
        uiChatBlockItems.ForEach(uiChatBlockItem => _uiChatBlockItems.Add(uiChatBlockItem));
    }

    private void Unblock(string nickname)
    {
        if (Backend.Chat.UnblockUser(nickname))
            Managers.Message.ShowMessage("차단 해제 성공");
        else
            Managers.Message.ShowMessage("차단 해제 실패");
        
        UpdateItems();
    }
}
