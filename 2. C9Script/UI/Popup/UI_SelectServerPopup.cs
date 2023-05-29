using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectServerPopup : UI_Popup
{
    [SerializeField] private Transform ServerItemRootTr;

    [SerializeField] private Button CloseButton;

    private readonly List<UI_ServerItem> _uiServerItems = new();

    public Action<int> OnSelectServerCallback;

    private void Start()
    {
        MakeServerItems();
        //CloseButton.BindEvent(ClosePopup);
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //         Managers.UI.ClosePopupUI();
    // }

    private void MakeServerItems()
    {
        ServerItemRootTr.DestroyInChildren();
        _uiServerItems.Clear();

        if (Application.isEditor || Managers.Manager.ProjectType == ProjectType.Dev)
        {
            UI_ServerItem uiServerItem = Managers.UI.MakeSubItem<UI_ServerItem>(ServerItemRootTr);
            uiServerItem.Init(100, ServerState.Good, OnSelectServerCallback);

#if SERVER_TEST
            foreach (var serverData in Managers.Server.GetServerStates())
            {
                int server = serverData.Key;
                ServerState serverState = serverData.Value;

                uiServerItem = Managers.UI.MakeSubItem<UI_ServerItem>(ServerItemRootTr);
                uiServerItem.Init(server, serverState, OnSelectServerCallback);

                _uiServerItems.Add(uiServerItem);
            }
#endif
        }
        else
        {
            foreach (var serverData in Managers.Server.GetServerStates())
            {
                int server = serverData.Key;
                ServerState serverState = serverData.Value;

                if (serverState == ServerState.Close)
                    continue;

                UI_ServerItem uiServerItem = Managers.UI.MakeSubItem<UI_ServerItem>(ServerItemRootTr);
                uiServerItem.Init(server, serverState, OnSelectServerCallback);

                _uiServerItems.Add(uiServerItem);
            }
        }
    }
}