using BackEnd;
using UniRx;
using UnityEngine;

public class BackendManager
{
    public void Init()
    {
        var bro = Backend.Initialize(true);

        Debug.Log(bro.IsSuccess() ? "Success Backend Initialize" : "Fail Backend Initialize");

        if (!SendQueue.IsInitialize)
            SendQueue.StartSendQueue();

        CheckVersion();

        SetErrorHandler();
    }

    private void CheckVersion()
    {
        if (Managers.Manager.ProjectType == ProjectType.Dev)
        {
            MessageBroker.Default.Publish(new AutoLoginMessage());
            return;
        }

        var bro = BackEnd.Backend.Utils.GetLatestVersion();
         var version = bro.GetReturnValuetoJSON()["version"].ToString();

        if (version == Application.version)
        {
            MessageBroker.Default.Publish(new AutoLoginMessage());
            return;
        }

        switch (Managers.Manager.StoreType)
        {
            case StoreType.GoogleStore:
            {
                var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
                noticePopup.Init("업데이트가 필요합니다.\n스토어에서 업데이트를 진행해주시기 바랍니다.", () =>
                {
#if !UNITY_EDITOR
                    Application.OpenURL("market://details?id=com.dwgames.chulguidlerpg");
                    Application.Quit();
#endif
                });
            }
                break;
            case StoreType.OneStore:
            {
                var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
                noticePopup.Init("업데이트가 필요합니다.\n스토어에서 업데이트를 진행해주시기 바랍니다.", () =>
                {
#if !UNITY_EDITOR
                    Application.Quit();
#endif
                });
            }
                break;
        }
    }

    private static void SetErrorHandler()
    {
        Backend.ErrorHandler.InitializePoll(true);

        Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = () =>
        {
            var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
            noticePopup.Init("다른 기기에서 로그인이 감지되었습니다.", () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
            });

            Time.timeScale = 0;
        };
    }

// 뒤끝 5.8.0 버전부터 특정 에러들에 대해 자동 3회 재시도 및 자동 토큰 갱신 추가
    public void FailLog(string failMessage, BackendReturnObject bro)
    {
        FailLog(bro);

        Debug.LogError(failMessage);
        Debug.LogError(
            $"StatusCode : {bro.GetStatusCode()} / ErrorCode : {bro.GetErrorCode()} / Message : {bro.GetMessage()}");
    }

    public void FailLog(BackendReturnObject bro, string failMessage = "")
    {
        FadeScreen.Instance.OffLoadingScreen();
        // 네트워크 불안정에 따른 호출/응답 실패
        if (bro.IsClientRequestFailError())
        {
            Managers.UI.CloseAllPopupUI();
            var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>(null, null, false);
            noticePopup.Init($"네트워크가 불안정 합니다. : {failMessage}", () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
            });

            //Time.timeScale = 0;
        }
        // 서버의 일시적 과부화, 정상적이지 않을때 발생
        else if (bro.IsServerError())
        {
            Managers.UI.CloseAllPopupUI();
            var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>(null, null, false);
            noticePopup.Init("서버 에러로 인해 게임을 종료합니다. 잠시 후 접속 해 주세요.", () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
            });

            Time.timeScale = 0;
        }
        // 베드 액세스 토큰 - 로그인을 통해 재발급 필요
        else if (bro.IsBadAccessTokenError())
        {
            Managers.UI.CloseAllPopupUI();
            var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>(null, null, false);
            noticePopup.Init("잘못된 계정 정보 입니다. 재설치후 접속 해주세요.", () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
            });

            Time.timeScale = 0;
        }
        // 서버 점검중
        else if (bro.IsMaintenanceError())
        {
            Managers.UI.CloseAllPopupUI();
            var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
            noticePopup.Init("서버 점검중 입니다.\n자세한 내용은 카페 공지사항을 확인 해 주세요.", () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
            });

            Time.timeScale = 0;
        }
        // 1초에 1번이상의 요청을 5분동안 지속하여 부하가 생겻을때
        else if (bro.IsMaintenanceError())
        {
            Managers.UI.CloseAllPopupUI();
            var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
            noticePopup.Init("서버 부하로 인해 게임을 종료합니다. 잠시 후 접속해주세요.", () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
            });

            Time.timeScale = 0;
        }
        else
        {
            if (bro.GetStatusCode().Equals("403"))
            {
                if (bro.GetErrorCode().Equals("ForbiddenException"))
                {
                    var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
                    noticePopup.Init("정지된 기기 입니다.\n자세한 사항은 고객센터에 문의하세요.", () =>
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
                    });
                }
                else
                {
                    var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
                    noticePopup.Init(bro.GetErrorCode().Replace('/', '\n'), () =>
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
                    });
                }
            }
            else if (bro.GetStatusCode().Equals("401"))
            {
                if (bro.GetErrorCode().Contains("BadUnauthorizedException"))
                {
                    // 서버와 클라이언트의 시간이 UTC+9(한국시간) 기준 10분 이상 차이가 나는 경우
                    if (bro.GetMessage().Contains("bad client_date"))
                    {
                        var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
                        noticePopup.Init("접속한 기기의 시간이 맞지 않아\n접속을 종료합니다.", () =>
                        {
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
                        });
                    }
                }
            }
        }
    }

    public void SetNotificationHandler()
    {
        Backend.Notification.OnAuthorize = (result, reason) =>
        {
            Debug.Log($"실시간 서버 연결 : {result}");
            Debug.Log($"실패시 이유 : {reason}");
        };

        Backend.Notification.OnDisConnect = reason => { Debug.Log($"실시간 서버 연결 해제 : {reason}"); };

        // 길드 가입 신청이 수락됐을때 호출
        Backend.Notification.OnApprovedGuildJoin = () =>
        {
            Observable.TimerFrame(1).ObserveOnMainThread().Subscribe(_ =>
            {
                if (!Managers.Game.IsPlaying)
                    return;

                var guildPopup = Managers.UI.FindPopup<UI_GuildPopup>();

                Managers.Guild.GetMyGuildData(() =>
                {
                    if (guildPopup != null)
                        Managers.UI.ClosePopupUI(guildPopup);

                    Managers.UI.ShowPopupUI<UI_GuildPopup>();
                }, false);
            });
        };

        // 길드에 가입신청이 들어왔을 때 호출
        Backend.Notification.OnReceivedGuildApplicant = () =>
        {
            Observable.TimerFrame(1).ObserveOnMainThread().Subscribe(_ =>
            {
                if (Managers.Guild.GuildData == null)
                    return;

                var myGuildMemberData = Managers.Guild.MyGuildMemberData;

                if (myGuildMemberData == null)
                    return;

                if (myGuildMemberData.Position.Equals("member"))
                    return;

                MessageBroker.Default.Publish(new GuildReceivedGuildApplicantMessage());
            });
        };


        Backend.Notification.Connect();
    }
}