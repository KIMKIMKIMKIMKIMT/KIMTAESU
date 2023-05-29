using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AppsFlyerSDK;
using BackEnd;
using DG.Tweening;
using GameData;
using Google;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Scene
{
    public class UI_TitleScene : UI_Scene
    {
        [Serializable]
        public struct RankUserUI
        {
            public GameObject RootObj;
            public TMP_Text NicknameText;
            public TMP_Text GuildText;
        }

        [SerializeField] private RankUserUI[] rankUserUis;

        [Header("Guest, Google, Apple 순서대로")] [SerializeField]
        private Button[] LoginButtons;

        [SerializeField] private Connector[] Connectors;

        [SerializeField] private Button ChangeServerButton;
        [SerializeField] private Button Terms1Button;
        [SerializeField] private Button Terms2Button;
        [SerializeField] private Toggle TermsToggle;

        [SerializeField] private TMP_Text ProgressText;
        [SerializeField] private TMP_Text ServerText;
        [SerializeField] private TMP_Text VersionText;

        [SerializeField] private GameObject ServerObj;
        [SerializeField] private GameObject ChulguImageObj;
        [SerializeField] private GameObject TitleObj;
        [SerializeField] private GameObject GameStartObj;

        [SerializeField] private Animator EffectAnimator;

        [SerializeField] private UI_CutScenePanel UICutScenePanel;

        private bool _isStart;

        private Coroutine LoadServerCoroutine;
        private Coroutine LoadGameCoroutine;

        private void Awake()
        {
            MessageBroker.Default.Receive<AutoLoginMessage>().Subscribe(_ =>
            {
                if (Application.isEditor)
                    return;
                
                StartCoroutine(CoCheckAutoLogin());
            }).AddTo(gameObject);
        }

        private void Start()
        {
            MessageBroker.Default.Receive<ViewCutScene>().Subscribe(_ => { UICutScenePanel.Open(); });

            MessageBroker.Default.Receive<LoadServerChart>().Subscribe(_ =>
            {
                LoadServerCoroutine = StartCoroutine(CoLoadingServerText());
            });

            MessageBroker.Default.Receive<LoadGameData>().Subscribe(_ =>
            {
                if (LoadServerCoroutine != null)
                    StopCoroutine(LoadServerCoroutine);

                LoadGameCoroutine = StartCoroutine(CoLoadingText());
            });

            Terms1Button.BindEvent(() =>
            {
                // 이용약관
                Application.OpenURL("https://sites.google.com/view/dwgamespublic/%ED%99%88");
            });

            Terms2Button.BindEvent(() =>
            {
                // 개인정보 처리방침
                Application.OpenURL("https://sites.google.com/view/dwgames-private/%ED%99%88");
            });

            for (int i = 0; i < (int)LoginType.Max; i++)
            {
                if (LoginButtons.Length <= i || Connectors.Length <= i)
                    continue;

                int index = i;

                LoginButtons[i].BindEvent(() =>
                {
                    if (!TermsToggle.isOn)
                        return;

                    SetActiveAllLoginButtons(false);
                    ProgressText.text = "로그인 중";
                    Connectors[index].Login();
                });

                Connectors[i].OnSuccessLoginCallback += OnSuccessLogin;
                Connectors[i].OnFailLoginCallback += OnFailLogin;
            }

            MessageBroker.Default.Receive<LoadingStep>().Subscribe(step =>
            {
                ProgressText.text = step switch
                {
                    LoadingStep.None => "None",
                    LoadingStep.Login => "로그인 중",
                    _ => ProgressText.text
                };

                if (step == LoadingStep.None)
                    return;

                foreach (var button in LoginButtons)
                {
                    button.gameObject.SetActive(false);
                }
            });
            
            InitTitle();

            ServerObj.SetActive(false);
            VersionText.text = $"Ver.{Application.version}";
        }

        public void InitTitle()
        {
            LoginButtons[(int)LoginType.Guest].gameObject.SetActive(Application.isEditor);

            GameStartObj.SetActive(false);

            if (Application.platform == RuntimePlatform.Android)
            {
                LoginButtons[(int)LoginType.Google].gameObject.SetActive(true);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                LoginButtons[(int)LoginType.Google].gameObject.SetActive(true);
            }
        }

        IEnumerator CoCheckAutoLogin()
        {
            bool isAutoLogin = 1 == PlayerPrefs.GetInt("AutoLogin", 0);

            if (!isAutoLogin)
                yield break;

            while (!Backend.IsInitialized || !Connectors[(int)LoginType.Google].IsInit)
            {
                yield return null;
            }

            SetActiveAllLoginButtons(false);
            ProgressText.text = "로그인 확인 중";

            Connectors[(int)LoginType.Google].Login();
        }

        private void SetPush()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                {
                    Managers.Game.SettingData.Push.Subscribe(push =>
                    {
                        if (push)
                        {
                            Backend.Android.PutDeviceToken(Backend.Android.GetDeviceToken(), bro =>
                            {
                                if (!bro.IsSuccess())
                                    Managers.Backend.FailLog("Fail DeviceToken", bro);
                                else
                                    Debug.Log("Success Put Push");
                            });
                        }
                        else
                        {
                            Backend.Android.DeleteDeviceToken(bro =>
                            {
                                if (!bro.IsSuccess())
                                    Managers.Backend.FailLog("Fail DeviceToken", bro);
                                else
                                    Debug.Log("Success Delete Push");
                            });
                        }
                    });

                    var bro = Backend.Android.PutDeviceToken(Backend.Android.GetDeviceToken());

                    if (!bro.IsSuccess())
                        Managers.Backend.FailLog("Fail DeviceToken", bro);
                    break;
                }
            }
        }

        private void OnSuccessLogin()
        {
            Debug.Log("OnSuccessLogin");

            if (string.IsNullOrEmpty(Backend.UserNickName))
            {
                Debug.Log("SetNickname");
                var uiSetNicknamePopup = Managers.UI.ShowPopupUI<UI_SetNicknamePopup>();
                uiSetNicknamePopup.OnSuccessCallback += OnSuccessLogin;
                return;
            }

            Managers.Game.SetServerTime(true);
            
            Where where = new Where();
            //where.Equal("Server", Managers.Server.CurrentServer);
            Backend.GameData.GetMyData("User", where, OnBackendGetMyData);

            void OnBackendGetMyData(BackendReturnObject bro)
            {
                if (bro.IsSuccess())
                {
                    var jsonData = bro.GetReturnValuetoJSON()["rows"];

                    if (jsonData.Count > 0)
                    {
                        Managers.Server.CurrentServer = int.Parse(jsonData[0]["Server"]["N"].ToString());
                        SetTitleUI();
                    }
                    else
                    {
                        var popup = Managers.UI.ShowPopupUI<UI_SelectServerPopup>();
                        popup.OnSelectServerCallback = server =>
                        {
                            var yesNoPopup = Managers.UI.ShowPopupUI<UI_YesNoPopup>();
                            yesNoPopup.Init($"한번 선택한 서버는 바꿀 수 없습니다.\n{server}서버로 결정하시겠습니까?", () =>
                            {
                                Managers.UI.ClosePopupUI();
                                Managers.Server.CurrentServer = server;
                                GameDataManager.UserGameData.InsertGameData(isSuccess =>
                                {
                                    if (!isSuccess)
                                    {
                                        var noticePopup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
                                        noticePopup.Init("게임 정보 생성에 실패해 게임을 종료 합니다.", () =>
                                        {
#if UNITY_EDITOR
                                            EditorApplication.isPlaying = false;
#else
                                                Application.Quit();
#endif
                                        });
                                        return;
                                    }

                                    SetTitleUI();
                                });
                            });
                        };
                    }
                }
            }
        }

        private void OnFailLogin()
        {
            ProgressText.text = string.Empty;
            Managers.Message.ShowMessage(MessageType.FailLogin);
            SetActiveAllLoginButtons(true);
        }

        private void ResetLoginUI()
        {
            ProgressText.text = string.Empty;
            SetActiveAllLoginButtons(true);
        }

        private void OnClickChangeServer()
        {
            var uiSelectServerPopup = Managers.UI.ShowPopupUI<UI_SelectServerPopup>();
            uiSelectServerPopup.OnSelectServerCallback = OnSelectServer;
        }

        private void OnClickGameStart()
        {
            if (_isStart)
                return;

            Debug.Log("OnClickGameStart");

            GameStartObj.SetActive(false);

            _isStart = true;

            StartCoroutine(CoConnectChaplin());

            ServerObj.SetActive(false);

            Managers.Chart.InitChart();
        }

        private IEnumerator CoLoadingServerText()
        {
            while (true)
            {
                ProgressText.text =
                    $"서버 데이터 로딩중\n({Managers.Chart.LoadCompleteChartCount}/{Managers.Chart.LoadChartCount})";
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator CoLoadingText()
        {
            string[] progressTexts = new[]
            {
                "게임 데이터 로딩중.",
                "게임 데이터 로딩중..",
                "게임 데이터 로딩중..."
            };

            int index = 0;

            while (true)
            {
                if (Application.isEditor || Managers.Manager.ProjectType == ProjectType.Dev)
                    ProgressText.text = progressTexts[index++] + $" - {Managers.Chart.ChartVersion}";
                else
                    ProgressText.text = progressTexts[index++];
                if (index >= progressTexts.Length)
                    index = 0;
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void OnSelectServer(int server)
        {
            Managers.UI.ClosePopupUI();
            Managers.Server.CurrentServer = server;
            string serverString = server == 100 ? "개발" : server.ToString();
            ServerText.text = $"{serverString}서버";

            SetRankerModel();
        }

        private void SetActiveAllLoginButtons(bool isActive)
        {
            foreach (var button in LoginButtons)
                button.gameObject.SetActive(isActive);

#if !UNITY_EDITOR
            LoginButtons[(int)LoginType.Guest].gameObject.SetActive(false);
#endif

            Terms1Button.gameObject.SetActive(isActive);
            Terms2Button.gameObject.SetActive(isActive);
            TermsToggle.gameObject.SetActive(isActive);
        }

        private void SetRankerModel()
        {
            //TitleObj.SetActive(false);

            var rankList = Managers.Rank.GetRankList(Managers.Server.CurrentServer, RankType.Stage, 3);
            var inDates = rankList.Select(rankData => rankData.GamerInDate).ToList();
            var gameDatas = GameDataManager.EquipGameData.GetGameDatas(Managers.Server.CurrentServer, inDates.ToList());

            Dictionary<string, string> guildDic = new();
            int completeLoadInfoCount = 0;

            if (inDates.Count <= 0)
            {
                SetRankerUI();
                return;
            }

            foreach (var inDate in inDates)
            {
                Backend.Social.GetUserInfoByInDate(inDate, bro =>
                {
                    if (bro.IsSuccess())
                    {
                        var jsonData = bro.GetReturnValuetoJSON();

                        string userInDate = jsonData["row"]["inDate"].ToString();
                        string guildName = jsonData["row"]["guildName"] != null
                            ? jsonData["row"]["guildName"].ToString()
                            : string.Empty;

                        if (guildDic.ContainsKey(userInDate))
                            guildDic[userInDate] = guildName;
                        else
                            guildDic.Add(userInDate, guildName);
                    }
                    else
                    {
                        Managers.Backend.FailLog("Fail GetUserInfoByInDate", bro);
                    }

                    completeLoadInfoCount += 1;

                    if (completeLoadInfoCount < inDates.Count)
                        return;

                    SetRankerUI();
                });
            }

            void SetRankerUI()
            {
                bool isData = false;

                for (int i = 0; i < rankUserUis.Length; i++)
                {
                    if (gameDatas == null || gameDatas["Responses"].Count <= i)
                    {
                        rankUserUis[i].RootObj.SetActive(false);
                        continue;
                    }

                    isData = true;

                    var gameData = gameDatas["Responses"][i];

                    var costumeId = 0;

                    if (gameData.ContainsKey(EquipType.ShowCostume.ToString()))
                    {
                        if (!int.TryParse(gameData[EquipType.ShowCostume.ToString()].ToString(), out costumeId))
                        {
                            if (gameData.ContainsKey(EquipType.Costume.ToString()))
                            {
                                int.TryParse(gameData[EquipType.Costume.ToString()].ToString(), out costumeId);
                            }
                        }
                    }
                    else if (gameData.ContainsKey(EquipType.Costume.ToString()))
                    {
                        int.TryParse(gameData[EquipType.Costume.ToString()].ToString(), out costumeId);
                    }

                    var weaponId = 0;

                    if (gameData.ContainsKey(EquipType.Weapon.ToString()))
                    {
                        int.TryParse(gameData[EquipType.Weapon.ToString()].ToString(), out weaponId);
                    }

                    switch (i)
                    {
                        case 0:
                            Managers.Model.Ranker1Model.SetCostume(costumeId);
                            Managers.Model.Ranker1Model.SetWeapon(weaponId);
                            Managers.Model.Ranker1Model.SetAnimation("Openning1");
                            break;
                        case 1:
                            Managers.Model.Ranker2Model.SetCostume(costumeId);
                            Managers.Model.Ranker2Model.SetWeapon(weaponId);
                            Managers.Model.Ranker2Model.SetAnimation("Openning2");
                            break;
                        case 2:
                            Managers.Model.Ranker3Model.SetCostume(costumeId);
                            Managers.Model.Ranker3Model.SetWeapon(weaponId);
                            Managers.Model.Ranker3Model.SetAnimation("Openning3");
                            break;
                    }

                    rankUserUis[i].NicknameText.text = rankList[i].Nickname;

                    if (guildDic.TryGetValue(rankList[i].GamerInDate, out var guildName))
                        rankUserUis[i].GuildText.text = guildName;
                    else
                        rankUserUis[i].GuildText.text = string.Empty;

                    if (i == 0)
                        StartCoroutine(CoAppearRankerEffect());
                    else
                        rankUserUis[i].RootObj.SetActive(true);
                }

                if (!isData)
                    SetGameStart();
            }
        }

        private IEnumerator CoAppearRankerEffect()
        {
            EffectAnimator.gameObject.SetActive(true);
            yield return null;

            while (true)
            {
                if (EffectAnimator.IsEndCurrentAnimation())
                    break;

                yield return null;
            }

            EffectAnimator.gameObject.SetActive(false);
            rankUserUis[0].RootObj.SetActive(true);

            if (!_isStart)
                SetGameStart();
        }

        private void SetTitleUI()
        {
            Managers.Rank.CheckDev();

            ProgressText.text = string.Empty;
            SetPush();

            ChangeServerButton.BindEvent(OnClickChangeServer);

            Managers.Server.Nickname = Backend.UserNickName;

            ChulguImageObj.SetActive(false);
            Backend.Chat.SetFilterUse(true);

            ServerObj.SetActive(true);

            OnSelectServer(Managers.Server.CurrentServer);
        }

        private void SetGameStart()
        {
            GameStartObj.SetActive(true);
            GameStartObj.transform.DOScale(0.9f, 0.3f).SetLoops(-1, LoopType.Yoyo);
            GameStartObj.BindEvent(OnClickGameStart);
        }

        IEnumerator CoConnectChaplin()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    Managers.Server.IpAddress = ip.ToString();
                }
            }

            string marketType = "2";

            switch (Managers.Manager.StoreType)
            {
                case StoreType.GoogleStore:
                    marketType = "2";
                    break;
                case StoreType.OneStore:
                    marketType = "3";
                    break;
            }

            string url = "http://api.chaplingame.co.kr:10001/MemberC?" +
                         "uid=" + $"{Managers.Server.UserId}_Server{Managers.Server.CurrentServer}" +
                         "&user_email=" + Managers.Server.UserId +
                         "&game_code=CGR" +
                         $"&server_id={Managers.Server.CurrentServer}" +
                         "&game_usn=" + $"{Managers.Server.UserId}_Server{Managers.Server.CurrentServer}" +
                         "&country_code=kr" +
                         "&user_mdn=0" +
                         "&user_nick=" + Managers.Server.Nickname +
                         "&app_type=1" +
                         "&market=" + marketType +
                         "&ipt_ip_addr=" + Managers.Server.IpAddress;


            // UnityWebRequest에 내장되있는 GET 메소드를 사용한다.
            UnityWebRequest www = UnityWebRequest.Get(url);

            Debug.Log("CoConnect");

            yield return www.SendWebRequest(); // 응답이 올때까지 대기한다.

            if (www.error == null) // 에러가 나지 않으면 동작.
            {
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log("error");
            }
        }
    }
}