using System;
using System.Collections.Generic;
using BackEnd;
using CodeStage.AntiCheat.Storage;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;

public class ServerManager
{
    private const string ServerChartUUID = "58251";
    
    private readonly Dictionary<int, ServerState> _serverStates = new();

    private const string LastConnectServerKey = "LastConnectServer";
    
    public int CurrentServer = 1;

    public string IdToken;
    public string UserId;
    public string IpAddress;
    public string Nickname;

    public void Init()
    {
        CurrentServer = Application.isEditor || Managers.Manager.ProjectType == ProjectType.Dev ? 100 : GetLastConnectServer();
        if (Application.isEditor)
            IdToken = "Editor";
    }

    public int GetLastConnectServer()
    {
        return PlayerPrefs.GetInt(LastConnectServerKey, 1);
    }

    public Dictionary<int, ServerState> GetServerStates()
    {
        if (_serverStates.Keys.Count > 0)
            return _serverStates;
        
        var bro = Backend.Chart.GetChartContents(ServerChartUUID);

        if (!bro.IsSuccess())
        {
            Managers.Backend.FailLog("Fail Load ServerData", bro);
            return new();
        }

        JsonData jsonData = bro.FlattenRows();

        for (int i = 0; i < jsonData.Count; i++)
        {
            int server = int.Parse(jsonData[i]["Server"].ToString());
            ServerState serverState = (ServerState)int.Parse(jsonData[i]["Server_State"].ToString());

            if (_serverStates.ContainsKey(server))
                _serverStates[server] = serverState;
            else
                _serverStates.Add(server, serverState);
        }

        return _serverStates;
    }

    public void CheckServerStatus()
    {
        if (Utils.IsDevServer() || Utils.IsWhiteList())
            return;
        
        Backend.Utils.GetServerStatus(ServerStatusResult);
    }

    public void ServerStatusResult(BackendReturnObject bro)
    {
        if (!bro.IsSuccess())
        {
            Managers.Backend.FailLog("Fail GetServeStatus", bro);
            return;
        }

        string serverStatus = bro.GetReturnValuetoJSON()["serverStatus"].ToString();

        switch (serverStatus)
        {
            // 점검중
            case "1":
            case "2":
            {
                if (serverStatus == "1") // 오프라인
                {
                    if (Managers.Game.IsPlaying)
                    {
                        Managers.GameData.SaveAllGameData();
                        if (!Managers.Game.UserData.IsAdSkip())
                        {
                            PlayerPrefs.SetString("AdBuffTime", JsonConvert.SerializeObject(Managers.Game.AdBuffDurationTimes));
                        }
                    }
                }
                var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>(null, null, false);
                noticePopup.Init("서버 점검중 입니다.\n자세한 내용은 카페 공지사항을 확인 해 주세요.", () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                });
            }
                break;
        }
    }
}