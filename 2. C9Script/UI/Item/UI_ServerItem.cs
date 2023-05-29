using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ServerItem : UI_Base
{
    [SerializeField] private TMP_Text ServerText;
    [SerializeField] private TMP_Text OpenDateText;

    [SerializeField] private Image ServerStateImage;

    [SerializeField] private Button ServerButton;

    [SerializeField] private GameObject ServerState1Obj;
    [SerializeField] private GameObject NewServerObj;
    
    [SerializeField] private GameObject ServerState2Obj;

    private int _server;
    private ServerState _serverState;
    private Action<int> _onSelectServerCallback;

    public void Init(int server, ServerState serverState, Action<int> onSelectServerCallback)
    {
        _server = server;
        _serverState = serverState;
        _onSelectServerCallback = onSelectServerCallback;

        SetUI();
    }

    private void SetUI()
    {
        string server = _server == 100 ? "개발" : _server.ToString();
        ServerText.text = $"{server}서버";

        ServerState1Obj.SetActive(true);
        ServerState2Obj.SetActive(false);

        switch (_server)
        {
            case 1:
                OpenDateText.text = $"22.11.16\nOPEN";
                break;
            case 2:
                OpenDateText.text = $"23.02.03\nOPEN";
                break;
            default:
                OpenDateText.text = string.Empty;
                break;
        }
        
        NewServerObj.SetActive(_server == 2);

        // switch (_serverState)
        // {
        //     case ServerState.Good:
        //         ServerStateImage.color = Color.green;
        //         ServerState1Obj.SetActive(true);
        //         ServerState2Obj.SetActive(false);
        //         break;
        //     case ServerState.Confusion:
        //         ServerStateImage.color = Color.red;
        //         ServerState1Obj.SetActive(false);
        //         ServerState2Obj.SetActive(true);
        //         break;
        // }

        ServerButton.BindEvent(() => { _onSelectServerCallback?.Invoke(_server); });
    }
}