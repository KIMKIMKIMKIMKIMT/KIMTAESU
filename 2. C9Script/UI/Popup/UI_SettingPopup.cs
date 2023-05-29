using System;
using System.Collections;
using Firebase.Auth;
using Google;
using TMPro;
using UI;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_SettingPopup : UI_Popup
{
    [Serializable]
    public record OptionUI
    {
        public Button Button;
        public TMP_Text StateText;
        public GameObject OffObj;

        public void On()
        {
            if (StateText != null)
                StateText.text = "On";
            OffObj.SetActive(false);
        }

        public void Off()
        {
            if (StateText != null)
                StateText.text = "Off";
            OffObj.SetActive(true);
        }

        public void SetActive(bool isActive)
        {
            if (isActive)
                On();
            else
                Off();
        }
    }

    [SerializeField] private Button CloseButton;
    [SerializeField] private OptionUI Bgm;
    [SerializeField] private OptionUI Sfx;
    [SerializeField] private OptionUI DamageText;
    [SerializeField] private OptionUI Push;
    [SerializeField] private OptionUI SkillEffect;

    [SerializeField] private Button PowerSaveButton;
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button FAQButton;
    [SerializeField] private Button LogoutButton;
    [SerializeField] private Button CafeButton;

    private DateTime _lastSaveTime;

    public override bool isTop => true;

    private int _completeSaveCount;

    [Obsolete("Obsolete")]
    private void Start()
    {
        CloseButton.BindEvent(ClosePopup);

        Managers.Game.SettingData.Bgm.Subscribe(bgm => Bgm.SetActive(bgm));
        Bgm.Button.BindEvent(() => Managers.Game.SettingData.Bgm.Value = !Managers.Game.SettingData.Bgm.Value);

        Managers.Game.SettingData.Sfx.Subscribe(sfx => Sfx.SetActive(sfx));
        Sfx.Button.BindEvent(() => Managers.Game.SettingData.Sfx.Value = !Managers.Game.SettingData.Sfx.Value);

        Managers.Game.SettingData.DamageText.Subscribe(damageText => DamageText.SetActive(damageText));
        DamageText.Button.BindEvent(() =>
            Managers.Game.SettingData.DamageText.Value = !Managers.Game.SettingData.DamageText.Value);

        Managers.Game.SettingData.Push.Subscribe(push => Push.SetActive(push));
        Push.Button.BindEvent(() => Managers.Game.SettingData.Push.Value = !Managers.Game.SettingData.Push.Value);

        Managers.Game.SettingData.SkillEffect.Subscribe(skillEffect => SkillEffect.SetActive(skillEffect));
        SkillEffect.Button.BindEvent(() => { Managers.Game.SettingData.SkillEffect.Value = !Managers.Game.SettingData.SkillEffect.Value; });

        PowerSaveButton.BindEvent(() =>
        {
            // 절전모드
            Managers.UI.ShowPowerSave();
        });

        SaveButton.BindEvent(() =>
        {
            if (_lastSaveTime != null && (Utils.GetNow() - _lastSaveTime).TotalSeconds < 30)
            {
                Managers.Message.ShowMessage("저장은 30초마다 가능합니다.");
                return;
            }

            _lastSaveTime = Utils.GetNow();
            _completeSaveCount = 0;
            FadeScreen.Instance.OnLoadingScreen();
            StartCoroutine(CoSaveTimer());
            Managers.GameData.SaveAllGameData(true, () =>
            {
                _completeSaveCount += 1;

                if (_completeSaveCount >= GameDataManager.TotalLoadGameDataCount)
                {
                    StopAllCoroutines();
                    FadeScreen.Instance.OffLoadingScreen();
                    Managers.Message.ShowMessage("저장 완료");
                }
                // 세이브 완료
            });
        });

        FAQButton.BindEvent(() =>
        {
            // 문의 팝업
            string mailto = "dwgamecs@gmail.com";
            string subject = EscapeURL("버그 리포트 / 기타 문의사항");
            string body = EscapeURL
            (
                // "이 곳에 내용을 작성해주세요.\n\n\n\n" +
                // "________" +
                // "Device Model : " + SystemInfo.deviceModel + "\n\n" +
                // "Device OS : " + SystemInfo.operatingSystem + "\n\n" +
                // "________"
                "※ 아래 문의 양식을 바탕으로 적어주시면 빠른 문의처리가 가능합니다.\n" +
                "[문의 캐릭터 정보]\n" +
                "1. 캐릭터 명 : \n" +
                "2. 캐릭터 UID : \n\n" +
                "[문의 내용]]n" +
                "1. 문의 유형\n" +
                "    예시) 결제 관련 문의, 버그 관련 문의\n" +
                "2. 문의 내용\n" +
                "    예시 ) 뽑기가 진행되지 않습니다.\n\n" +
                "    (결제 관련 문의일 경우 적어주세요.)\n" +
                "1. 구매하신 상품 : \n" +
                "2. 구매하신 상품의  영수증번호를 포함한 스크린샷 첨부 : (이미지 첨부)\n" +
                "1) 구글플레이어 : 연동 메일내 GPA.0000-으로 시작하는 영수증 스크린샷\n" +
                "2) 원스토어 : 결제 내역내 TSTORE, KTXXXX_, LGXXX등....을 포함한 영수증 스크린샷\n"
            );

            Application.OpenURL("mailto:" + mailto + "?subject=" + subject + "&body=" + body);
        });

        LogoutButton.BindEvent(() =>
        {
            var popup = Managers.UI.ShowPopupUI<UI_YesNoPopup>();
            popup.Init("로그아웃 후 게임을 종료하시겠습니까?", () =>
            {
                FadeScreen.Instance.OnLoadingScreen();

                // int checkCount = GameDataManager.GameDatas.Count;
                // int completeCount = 0;
                // GameDataManager.GameDatas.ForEach(gameData => gameData.SaveGameData(true, () =>
                // {
                //     completeCount += 1;
                //     
                //     if ()
                // }));
                
                Managers.GameData.SaveAllGameDataTransaction(QuitGame);

                void QuitGame()
                {
                    if (PlayerPrefs.HasKey("AutoLogin"))
                        PlayerPrefs.DeleteKey("AutoLogin");
                    
                    BackEnd.Backend.BMember.Logout();

#if !UNITY_EDITOR
                    GoogleSignIn.DefaultInstance.SignOut();
                    GoogleSignIn.DefaultInstance.Disconnect();
#endif

                    Observable.TimerFrame(1).ObserveOnMainThread().Subscribe(_ =>
                    {
#if UNITY_EDITOR
                        EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                    });
                }
            });
        });

        CafeButton.BindEvent(() => { Application.OpenURL("https://cafe.naver.com/chulgurpg"); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI();
        }
    }

    [Obsolete("Obsolete")]
    private string EscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    private IEnumerator CoSaveTimer()
    {
        yield return new WaitForSeconds(10f);

        FadeScreen.Instance.OffLoadingScreen();
    }

    private void OnClickLogout()
    {
    }
}