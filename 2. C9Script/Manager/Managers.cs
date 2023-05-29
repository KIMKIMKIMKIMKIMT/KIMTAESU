using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AppsFlyerSDK;
using BackEnd;
using BackEnd.Tcp;
using DefaultNamespace;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class Managers : MonoBehaviour
{
    [SerializeField] private GameSystemData SystemData;
    [SerializeField] private bool IsCheat;


    public StoreType StoreType;
    public ProjectType ProjectType;
    public string EditorChartVersion;

    public static GameSystemData GameSystemData => Manager.SystemData;

    public static Managers Manager { get; private set; }

    public static BackendManager Backend { get; } = new();
    public static ChartManager Chart { get; } = new();
    public static SceneManager Scene { get; } = new();
    public static GameManager Game { get; } = new();
    public static UIManager UI { get; } = new();
    public static ResourceManager Resource { get; } = new();
    public static StageManager Stage { get; } = new();
    public static MonsterManager Monster { get; } = new();
    public static DamageTextManager DamageText { get; } = new();
    public static GameDataManager GameData { get; } = new();
    public static ChatManager Chat { get; } = new();
    public static SoundManager Sound { get; } = new();
    public static MessageManager Message { get; } = new();
    public static DungeonManager Dungeon { get; } = new();
    public static EffectManager Effect { get; } = new();
    public static RankManager Rank { get; } = new();
    public static ServerManager Server { get; } = new();
    public static PostManager Post { get; } = new();
    public static PvpManager Pvp { get; } = new();
    public static DpsDungeonManager Dps { get; } = new();
    public static ModelManager Model => ModelManager.Instance;
    public static IAPManager IAP => IAPManager.Instance;
    public static AdManager Ad => AdManager.Instance;
    public static WorldCupEventManager WorldCupEvent { get; } = new();
    public static RaidManager Raid { get; } = new();
    public static XMasEventManager XMasEvent { get; } = new();
    public static AntiCheatManager AntiCheat => AntiCheatManager.Instance;
    public static GuildManager Guild { get; } = new();
    public static GuildRaidManager GuildRaid { get; } = new();
    public static AllRaidManager AllRaid { get; } = new();
    public static CostumeCollectionManager CostumeSet { get; } = new();
    public static GuildSportsManager GuildSports { get; } = new();

    private void Awake()
    {
        Manager = this;
        DOTween.SetTweensCapacity(2000, 2000);
    }

    private void Start()
    {
        Screen.SetResolution(Screen.width, Screen.height, true, 60);
        DontDestroyOnLoad(gameObject);
        
        if (!Application.isEditor)
            Application.targetFrameRate = 60;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        InitManagers();

        Sound.PlayBgm(BgmType.Title);

        CultureInfo.CurrentCulture = new CultureInfo("ko-KR");
        //CultureInfo.CurrentCulture = new CultureInfo("en-US");
        AppsFlyer.sendEvent("game_start", new Dictionary<string, string>()
        {
            ["C"] = "game_start"
        });
    }

    private void InitManagers()
    {
        Backend.Init();
        Game.SettingData.Init();
        Scene.Init();
        Chat.Init();
        Message.Init();
        Sound.Init();
        Server.Init();
        

#if UNITY_EDITOR

        StoreType = StoreType.GoogleStore;
        ProjectType = ProjectType.Dev;

#endif

        if (!Application.genuine || !Application.genuineCheckAvailable)
        {
            var popup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
            popup.Init("변조된 앱 입니다.", () =>
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }

        StartCoroutine(CoCheckRoot());
    }

    private void Update()
    {
        if (BackEnd.Backend.IsInitialized)
            BackEnd.Backend.AsyncPoll();

        if (BackEnd.Backend.Chat.IsChatConnect(ChannelType.Public))
            BackEnd.Backend.Chat.Poll();

        if (SendQueue.IsInitialize)
            SendQueue.Poll();
        
        if (BackEnd.Backend.IsInitialized && Input.GetKeyDown(KeyCode.Escape) && UI.PopupCount <= 0)
        {
            var popup = UI.ShowPopupUI<UI_YesNoPopup>();
            popup.Init(Game.IsPlaying ? "저장 후 게임을 종료 하시겠습니까?" : "게임을 종료 하시겠습니까?", () =>
            {
                if (Game.IsPlaying)
                {
                    int gameDataCount = GameDataManager.TotalLoadGameDataCount;
                    int completeCount = 0;

                    FadeScreen.Instance.OnLoadingScreen();

                    GameData.SaveAllGameData(true, () =>
                    {
                        completeCount += 1;

                        if (completeCount < gameDataCount)
                            return;

                        QuitGame();
                    });
                }
                else
                {
                    QuitGame();
                }

                void QuitGame()
                {
#if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
            });
        }

#if UNITY_EDITOR

        #region Cheat

        if (!Application.isEditor && ProjectType != ProjectType.Dev)
            return;

        if (!Game.IsPlaying)
            return;

        if (!IsCheat)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            foreach (var goodsData in Game.GoodsDatas)
            {
                if (goodsData.Key == (int)Goods.Gold)
                    goodsData.Value.Value = Math.Min(double.MaxValue, goodsData.Value.Value + 100000000000000000000d);
                else
                    goodsData.Value.Value = Math.Min(double.MaxValue, goodsData.Value.Value + 100000);
            }

            Message.ShowMessage("치트 - 굿즈 증가");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            foreach (var weaponData in Game.WeaponDatas.Values)
                weaponData.Quantity += 1;

            Message.ShowMessage("치트 - 모든 무기 1개 증가");
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var petData in Game.PetDatas.Values)
                petData.Quantity += 1;

            Message.ShowMessage("치트 - 모든 펫 1개 증가");
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var costumeData in Game.CostumeDatas.Values)
                costumeData.Quantity += 1;

            Message.ShowMessage("치트 - 모든 코스튬 1개 증가");
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (var skillData in Game.SkillDatas.Values)
            {
                if (skillData.IsOpen)
                    continue;

                skillData.Level = 1;
            }

            Message.ShowMessage("치트 - 모든 스킬 오픈");
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            foreach (var chartData in ChartManager.StatGoldUpgradeCharts.Values)
            {
                if (!Game.StatLevelDatas.ContainsKey(chartData.StatId))
                    continue;

                Game.StatLevelDatas[chartData.StatId].Value = 0;
            }

            Message.ShowMessage("치트 - 골드 강화 스탯 초기화");
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Game.StatLevelDatas[(int)StatType.Attack].Value = Math.Max(
                1,
                Game.StatLevelDatas[(int)StatType.Attack].Value - 1000000);
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Game.StatLevelDatas[(int)StatType.Attack].Value = Math.Min(
                ChartManager.StatGoldUpgradeCharts[(int)StatType.Attack].MaxLevel,
                Game.StatLevelDatas[(int)StatType.Attack].Value + 1000000);
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Game.UserData.PromoGrade = Mathf.Max(1, Game.UserData.PromoGrade - 1);
            Message.ShowMessage("치트 - 승급전 등급 1단계 감소");
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Game.UserData.PromoGrade = Mathf.Min(Game.UserData.PromoGrade + 1,
                (int)ChartManager.SystemCharts[global::SystemData.MaxPromoLevel].Value);
            Message.ShowMessage("치트 - 승급전 등급 1단계 증가");
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Game.UserData.Level = Math.Max(1, Game.UserData.Level - 10);
            Message.ShowMessage("치트 - 레벨 10 감소");
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            int maxLevel = ChartManager.LevelCharts.Keys.Max();
            Game.UserData.Level = Math.Min(maxLevel, Game.UserData.Level + 10);
            Message.ShowMessage("치트 - 레벨 10 증가");
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            int maxStage = ChartManager.StageDataController.StageDataTable.Keys.Max();
            Game.UserData.MaxReachStage = Math.Min(maxStage, Game.UserData.MaxReachStage + 1000);
            Message.ShowMessage("치트 - 최대 스테이지 1000 증가");
            Game.CalculateStat();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Stage.State.Value == StageState.Normal)
            {
                if (ChartManager.StageDataController.StageDataTable.TryGetValue(Stage.StageId.Value, out var stageChart))
                {
                    Stage.KillCount.Value = stageChart.NeedBossChallengeKillCount;
                    Message.ShowMessage("치트 - 킬카운트 채우기");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
            Game.IsHotTime = !Game.IsHotTime;

        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (var data in Managers.Game.LabResearchDatas.Values)
            {
                data.CoolTime = new DateTime();
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (Stage.State.Value == StageState.Raid)
                Monster.AllKillMonster();
        }

        if (Input.GetKeyDown(KeyCode.M))
            Time.timeScale += 0.1f;

        if (Input.GetKeyDown(KeyCode.N))
            Time.timeScale -= 0.1f;

        #endregion

#endif
    }

    private IEnumerator CoCheckRoot()
    {
        while (true)
        {
            Debug.Log("Check Root");

            if (IsRooted())
            {
                var popup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
                popup.Init("루팅이 감지 되어 게임을 종료합니다.", () =>
                {
#if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                });
            }

            yield return new WaitForSeconds(60);
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!BackEnd.Backend.IsInitialized)
            return;

        if (!Game.IsPlaying)
            return;

        if (Utils.IsBuyingProcess || Ad.IsShowingAd)
            return;

        if (pauseStatus)
        {
            GameData.SaveAllGameData(true);
            Game.UserLog();
            Game.SendStageItemLog();
        }

        if (!pauseStatus)
        {
            Game.SetServerTime();
            IsRooted();
        }
    }

    private void OnApplicationQuit()
    {
        if (BackEnd.Backend.IsInitialized && Game.IsPlaying)
        {
            BackEnd.Backend.Notification.DisConnect();
            GameData.SaveAllGameData(true);
        }

        if (SendQueue.IsInitialize)
            SendQueue.StopSendQueue();

        if (BackEnd.Backend.Chat.IsChatConnect(ChannelType.Public))
            BackEnd.Backend.Chat.LeaveChannel(ChannelType.Public);
        
        if (BackEnd.Backend.Chat.IsChatConnect(ChannelType.Guild))
            BackEnd.Backend.Chat.LeaveChannel(ChannelType.Guild);
    }

    public static bool IsRooted()
    {
        bool isRoot = false;

        if (Application.platform == RuntimePlatform.Android)
        {
            if (isRootedPrivate("/system/bin/su"))
                isRoot = true;
            if (isRootedPrivate("/system/xbin/su"))
                isRoot = true;
            if (isRootedPrivate("/system/app/SuperUser.apk"))
                isRoot = true;
            if (isRootedPrivate("/data/data/com.noshufou.android.su"))
                isRoot = true;
            if (isRootedPrivate("/sbin/su"))
                isRoot = true;
        }

        return isRoot;
    }

    public static bool isRootedPrivate(string path)
    {
        bool boolTemp = File.Exists(path);

        return boolTemp;
    }
}