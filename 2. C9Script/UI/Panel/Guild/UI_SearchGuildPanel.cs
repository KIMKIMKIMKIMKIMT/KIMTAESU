using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using TMPro;
using UI;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UI_SearchGuildPanel : UI_Panel
{
    [SerializeField] private Transform GuildInfoItemRoot;
    [SerializeField] private TMP_InputField GuildNameInputField;

    [SerializeField] private Button RefreshButton;
    [SerializeField] private Button SearchButton;

    private float _refreshTime;
    private readonly List<UI_GuildInfoItem> _uiGuildInfoItems = new();

    private void Start()
    {
        if (_uiGuildInfoItems.Count <= 0)
            GuildInfoItemRoot.DetachChildren();
        else
            _uiGuildInfoItems.ForEach(uiGuildInfoItem => uiGuildInfoItem.gameObject.SetActive(false));
        
        Managers.Guild.GetGuildList(SetGuildInfoItems);

        RefreshButton.BindEvent(OnClickRefresh);
        SearchButton.BindEvent(OnClickSearch);
    }

    private void SetGuildInfoItems()
    {
        var uiGuildInfoItems = _uiGuildInfoItems.ToList();

        int index = 0;

        foreach (var guildInfo in Managers.Guild.GuildList)
        {
            UI_GuildInfoItem uiGuildInfoItem;

            if (uiGuildInfoItems.Count > index)
                uiGuildInfoItem = uiGuildInfoItems[index++];
            else
            {
                uiGuildInfoItem = Managers.UI.MakeSubItem<UI_GuildInfoItem>(GuildInfoItemRoot);
                _uiGuildInfoItems.Add(uiGuildInfoItem);
            }

            uiGuildInfoItem.gameObject.SetActive(true);
            uiGuildInfoItem.Init(guildInfo);
        }
    }

    private void SetGuildInfoItems(GuildData guildData)
    {
        if (_uiGuildInfoItems.Count <= 0)
            GuildInfoItemRoot.DetachChildren();
        else
            _uiGuildInfoItems.ForEach(uiGuildInfoItem => uiGuildInfoItem.gameObject.SetActive(false));

        var uiGuildInfoItems = _uiGuildInfoItems.ToList();

        UI_GuildInfoItem uiGuildInfoItem;

        if (uiGuildInfoItems.Count > 0)
            uiGuildInfoItem = uiGuildInfoItems[0];
        else
        {
            uiGuildInfoItem = Managers.UI.MakeSubItem<UI_GuildInfoItem>(GuildInfoItemRoot);
            _uiGuildInfoItems.Add(uiGuildInfoItem);
        }

        uiGuildInfoItem.gameObject.SetActive(true);
        uiGuildInfoItem.Init(guildData);
    }

    private void OnClickRefresh()
    {
        if (_refreshTime > 0)
        {
            Managers.Message.ShowMessage($"{(int)_refreshTime}초 후 갱신 가능합니다.");
            return;
        }

        _refreshTime = 10f;
        MainThreadDispatcher.StartCoroutine(CoRefreshTimer());
        Managers.Guild.GetGuildList(SetGuildInfoItems);
    }

    private IEnumerator CoRefreshTimer()
    {
        var delay = new WaitForSeconds(0.5f);

        while (_refreshTime > 0)
        {
            yield return delay;
            _refreshTime = math.max(0, _refreshTime - 0.5f);
        }
    }

    private void OnClickSearch()
    {
        string guildName = GuildNameInputField.text;

        if (string.IsNullOrEmpty(guildName))
        {
            Managers.Message.ShowMessage("검색 할 학교명을 입력하세요");
            return;
        }

        FadeScreen.Instance.OnLoadingScreen();

        Backend.Guild.GetGuildIndateByGuildNameV3(guildName, OnBackendGetGuildInDateByGuildNameV3);
    }

    private void OnBackendGetGuildInDateByGuildNameV3(BackendReturnObject bro)
    {
        if (!bro.IsSuccess())
        {
            string statusCode = bro.GetStatusCode();
            string errorCode = bro.GetErrorCode();
            string message = bro.GetMessage();

            if (statusCode.Contains("404") && errorCode.Contains("NotFoundException"))
            {
                // 존재하지 않는 길드명
                if (message.Contains("guild not found"))
                {
                    Managers.Message.ShowMessage("존재하지 않는 학교명 입니다.");
                }
                else if (message.Contains("Resource not found"))
                {
                    Managers.Message.ShowMessage("검색 할 학교명을 입력하세요.");
                }
            }
            else
                Managers.Backend.FailLog("Fail GetGuildInDateByGuildName", bro);

            FadeScreen.Instance.OffLoadingScreen();
            return;
        }

        string guildInDate = bro.GetReturnValuetoJSON()["guildInDate"]["S"].ToString();

        Backend.Guild.GetGuildInfoV3(guildInDate, OnBackendGetGuildInfoV3);
    }

    private void OnBackendGetGuildInfoV3(BackendReturnObject bro)
    {
        if (!bro.IsSuccess())
        {
            string statusCode = bro.GetStatusCode();
            string errorCode = bro.GetErrorCode();
            string message = bro.GetMessage();

            // guildInDate가 존재하지 않음
            if (statusCode.Contains("404") && errorCode.Contains("NotFoundException") &&
                message.Contains("guild not found"))
            {
                Managers.Message.ShowMessage("학교 정보가 존재 하지 않습니다.");
            }
            // old guild를 조회한 경우
            else if (statusCode.Contains("412") && errorCode.Contains("PreconditionFailed") &&
                     message.Contains("version is different"))
            {
                Managers.Message.ShowMessage("잘못된 학교 정보 입니다.");
            }
            else
            {
                Managers.Backend.FailLog("Fail GetGuildInfo", bro);
            }

            FadeScreen.Instance.OffLoadingScreen();
            return;
        }

        var guildData = Managers.Guild.ParseGuildData(bro);

        if (guildData.GuildMetaData.Server != Managers.Server.CurrentServer)
        {
            Managers.Message.ShowMessage("같은 서버의 학교만 검색 가능합니다.");
            FadeScreen.Instance.OffLoadingScreen();
            return;
        }
        SetGuildInfoItems(guildData);
        
        FadeScreen.Instance.OffLoadingScreen();
    }
}