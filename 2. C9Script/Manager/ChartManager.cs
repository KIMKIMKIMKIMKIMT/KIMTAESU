using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BackEnd;
using Chart;
using LitJson;
using UniRx;
using UnityEngine;

public class ChartManager
{
    #region Chart Info

    private const string ChartVersionKey = "ChartVersionKey";
    private const string VersionChartUuid = "62097";
    public string ChartVersion = "1.0.3";

    #endregion

    #region ChartData
    public static ExcelDataLoader ExcelDataLoader { get; } = new();

    public static readonly Dictionary<string, StringChart> StringCharts = new();
    public static readonly Dictionary<int, WeaponChart> WeaponCharts = new();
    //public static readonly Dictionary<int, StageChart> StageCharts = new();
    //public static readonly Dictionary<int, StageBossChart> StageBossCharts = new();
    public static readonly Dictionary<int, MonsterChart> MonsterCharts = new();
    public static readonly Dictionary<int, SkillChart> SkillCharts = new();
    public static readonly Dictionary<(int, SkillPolicyProperty), SkillPolicyChart> SkillPolicyCharts = new();
    public static readonly Dictionary<int, DungeonChart> DungeonCharts = new();
    public static readonly Dictionary<SystemData, SystemChart> SystemCharts = new();
    public static readonly Dictionary<int, GoodsChart> GoodsCharts = new();
    public static readonly Dictionary<int, HwasengbangDungeonChart> HwasengbangDungeonCharts = new();
    public static readonly Dictionary<int, MarinCampDungeonChart> MarinCampDungeonCharts = new();
    public static readonly Dictionary<int, MarchDungeonChart> MarchDungeonCharts = new();
    public static readonly Dictionary<int, CostumeChart> CostumeCharts = new();
    public static readonly Dictionary<int, StatChart> StatCharts = new();
    public static readonly Dictionary<int, PetChart> PetCharts = new();
    public static readonly Dictionary<int, WorldWoodChart> WoodCharts = new();
    public static readonly Dictionary<int, SetChart> SetCharts = new();
    public static readonly Dictionary<int, QuestChart> QuestCharts = new();
    public static readonly Dictionary<int, AttendanceChart> AttendanceCharts = new();
    public static readonly Dictionary<int, AdBuffChart> AdBuffCharts = new();
    public static readonly Dictionary<int, WorldChart> WorldCharts = new();
    public static readonly Dictionary<int, PromoDungeonChart> PromoDungeonCharts = new();
    public static readonly Dictionary<int, PvpChart> PvpCharts = new();
    public static readonly Dictionary<int, WeaponSummonLevelChart> WeaponSummonLevelCharts = new();
    public static readonly Dictionary<int, PetSummonLevelChart> PetSummonLevelCharts = new();
    public static readonly Dictionary<int, DpsDungeonChart> DpsDungeonCharts = new();
    public static readonly Dictionary<int, LevelChart> LevelCharts = new();
    public static readonly Dictionary<int, StatGoldUpgradeChart> StatGoldUpgradeCharts = new();
    public static readonly Dictionary<(int, int), StatGoldUpgradeLevelChart> StatGoldUpgradeLevelCharts = new();
    public static readonly Dictionary<int, StatPointUpgradeChart> StatPointUpgradeCharts = new();
    public static readonly Dictionary<(int, int), StatPointUpgradeLevelChart> StatPointUpgradeLevelCharts = new();
    public static Dictionary<int, ShopChart> ShopCharts = new();
    public static readonly Dictionary<int, MissionChart> MissionCharts = new();
    public static readonly Dictionary<int, CollectionChart> CollectionCharts = new();
    public static readonly Dictionary<(int, int), CostumeAwakeningChart> CostumeAwakeningCharts = new();
    public static readonly Dictionary<int, GuideQuestChart> GuideQuestCharts = new();
    public static readonly Dictionary<int, WorldCupEventDungeonChart> WorldCupEventDungeonCharts = new();
    public static readonly Dictionary<int, RaidDungeonChart> RaidDungeonCharts = new();
    public static readonly Dictionary<int, XMasEventDungeonChart> XMasEventDungeonCharts = new();
    public static readonly Dictionary<int, UnlimitedPointUpgradeChart> UnlimitedPointUpgradeCharts = new();
    public static readonly Dictionary<int, UnlimitedPointUpgradeLevelChart> UnlimitedPointUpgradeLevelCharts = new();
    public static readonly Dictionary<int, GuildBuffChart> GuildBuffCharts = new();
    public static readonly Dictionary<int, GuildGradeChart> GuildGradeCharts = new();
    public static readonly Dictionary<int, LabGradeRateChart> LabGradeRateCharts = new();
    public static readonly Dictionary<int, LabPatternChart> LabPatternCharts = new();
    public static readonly Dictionary<int, LabSetChart> LabSetCharts = new();
    public static readonly Dictionary<LabSkillType, LabSkillChart> LabSkillCharts = new();
    public static readonly Dictionary<int, LabSkillLevelChart> LabSkillLevelCharts = new();
    public static readonly Dictionary<int, LabStatChart> LabStatCharts = new();
    public static readonly Dictionary<int, LabStatRateChart> LabStatRateCharts = new();
    public static readonly Dictionary<int, LabAwakeningChart> LabAwakeningCharts = new();
    public static readonly Dictionary<int, GuildRaidDungeonChart> GuildRaidDungeonCharts = new();
    public static readonly Dictionary<int, AllGuildRaidDungeonChart> AllGuildRaidDungeonCharts = new();
    public static readonly Dictionary<int, EventAttendanceChart> EventAttendanceCharts = new();
    public static readonly Dictionary<int, GrowthBuffChart> GrowthBuffCharts = new();
    public static readonly Dictionary<int, DpsBossDungeonChart> DpsBossDungeonCharts = new();
    public static readonly Dictionary<int, WoodStatRateChart> WoodStatRateCharts = new();
    public static readonly Dictionary<int, WoodStatDataChart> WoodStatDataCharts = new();
    public static readonly Dictionary<int, WoodGradeRateChart> WoodGradeRateCharts = new();
    public static readonly Dictionary<int, WoodAwakeningDataChart> WoodAwakeningDataCharts = new();
    public static readonly Dictionary<int, ChatCouponChart> ChatCouponCharts = new();
    public static readonly Dictionary<int, CostumCollectionChart> CostumCollectionCharts = new();
    public static readonly Dictionary<int, GuildSportsDungeonChart> GuildSportsDungeonCharts = new();

    // Local Chart
    public static StageDataController StageDataController { get; private set; }
    public static StageBossDataController StageBossDataController { get; private set; }


    // 확률 차트 ID
    private static readonly Dictionary<string, string> _probabilityIds = new();
    public static Dictionary<string, string> ProbabilityIds => _probabilityIds;

    // 화이트 리스트
    public static readonly List<string> WhiteList = new();

    private readonly Queue<IEnumerator> _enumerators = new();

    private void LoadCachingChart()
    {
        MessageBroker.Default.Publish(new LoadGameData());
        Debug.Log("Start CachingLoad");

        // 일부 차트 데이터(스테이지 등)의 행 갯수가 너무 많아지면 순간적으로 멈추는 현상 발생
        // 일정 횟수마다 프레임 넘기면서 멈춤 없이 진행하기 위한 로직

        AddLoadProcess(LoadChartData<string, StringChart>("StringTable"));
        AddLoadProcess(LoadChartData<int, WeaponChart>("WeaponDataInfo"));
        //AddLoadProcess(LoadChartData<int, StageChart>("StageDataInfo"));
        //AddLoadProcess(LoadChartData<int, StageBossChart>("StageBossDataInfo"));
        AddLoadProcess(LoadChartData<int, MonsterChart>("MonsterDataInfo"));
        AddLoadProcess(LoadChartData<int, SkillChart>("SkillDataInfo"));
        AddLoadProcess(LoadChartData<(int, SkillPolicyProperty), SkillPolicyChart>("SkillPolicyDataInfo"));
        AddLoadProcess(LoadChartData<int, DungeonChart>("DungeonDataInfo"));
        AddLoadProcess(LoadChartData<SystemData, SystemChart>("SystemDataInfo"));
        AddLoadProcess(LoadChartData<int, GoodsChart>("GoodsDataInfo"));
        AddLoadProcess(LoadChartData<int, HwasengbangDungeonChart>("DHwasengbangDungeonTable"));
        AddLoadProcess(LoadChartData<int, MarinCampDungeonChart>("DMarinecampDungeonTable"));
        AddLoadProcess(LoadChartData<int, MarchDungeonChart>("DMarchDungeonTable"));
        AddLoadProcess(LoadChartData<int, CostumeChart>("CostumeDataInfo"));
        AddLoadProcess(LoadChartData<int, StatChart>("StatDataInfo"));
        AddLoadProcess(LoadChartData<int, PetChart>("PetDataInfo"));
        AddLoadProcess(LoadChartData<int, WorldWoodChart>("WoodDataInfo"));
        AddLoadProcess(LoadChartData<int, SetChart>("SetDataInfo"));
        AddLoadProcess(LoadChartData<int, QuestChart>("QuestDataInfo"));
        AddLoadProcess(LoadChartData<int, AttendanceChart>("AttendanceDataInfo"));
        AddLoadProcess(LoadChartData<int, AdBuffChart>("ADBuffDataInfo"));
        AddLoadProcess(LoadChartData<int, WorldChart>("WorldDataInfo"));
        AddLoadProcess(LoadChartData<int, PromoDungeonChart>("DPromoDungeonTable"));
        AddLoadProcess(LoadChartData<int, PvpChart>("DPvpDungeonTable"));
        AddLoadProcess(LoadChartData<int, WeaponSummonLevelChart>("WeaponSummonLevel"));
        AddLoadProcess(LoadChartData<int, PetSummonLevelChart>("PetSummonLevel"));
        AddLoadProcess(LoadChartData<int, DpsDungeonChart>("DDpsDungeonTable"));
        AddLoadProcess(LoadChartData<int, LevelChart>("LevelDataInfo"));
        AddLoadProcess(LoadChartData<int, StatGoldUpgradeChart>("StatGoldUpgradeInfo"));
        AddLoadProcess(LoadChartData<(int, int), StatGoldUpgradeLevelChart>("StatGoldUpgradeLevelInfo"));
        AddLoadProcess(LoadChartData<int, StatPointUpgradeChart>("StatPointUpgradeInfo"));
        AddLoadProcess(LoadChartData<(int, int), StatPointUpgradeLevelChart>("StatPointUpgradeLevelInfo"));
        AddLoadProcess(LoadChartData<int, ShopChart>("ShopDataInfo"));
        AddLoadProcess(LoadChartData<int, MissionChart>("MissionDataInfo"));
        AddLoadProcess(LoadChartData<int, CollectionChart>("CollectionDataInfo"));
        AddLoadProcess(LoadChartData<(int, int), CostumeAwakeningChart>("CostumeAwakeningTable"));
        AddLoadProcess(LoadChartData<int, GuideQuestChart>("GuideQuestDataInfo"));
        AddLoadProcess(LoadChartData<int, WorldCupEventDungeonChart>("DWorldcupEventDungeonTable"));
        AddLoadProcess(LoadChartData<int, RaidDungeonChart>("DRaidDungeonTable"));
        AddLoadProcess(LoadChartData<int, XMasEventDungeonChart>("DXMasDungeonTable"));
        AddLoadProcess(LoadChartData<int, UnlimitedPointUpgradeChart>("UnlimitedPointUpgradeInfo"));
        AddLoadProcess(LoadChartData<int, UnlimitedPointUpgradeLevelChart>("UnlimitedPointUpgradeLevelInfo"));
        AddLoadProcess(LoadChartData<int, GuildBuffChart>("GuildBuffDataInfo"));
        AddLoadProcess(LoadChartData<int, GuildGradeChart>("GuildGradeDataInfo"));
        AddLoadProcess(LoadChartData<int, LabGradeRateChart>("LabGradeRateTable"));
        AddLoadProcess(LoadChartData<int, LabPatternChart>("LabPatternTable"));
        AddLoadProcess(LoadChartData<int, LabSetChart>("LabSetTable"));
        AddLoadProcess(LoadChartData<LabSkillType, LabSkillChart>("LabSkillDataInfo"));
        AddLoadProcess(LoadChartData<int, LabSkillLevelChart>("LabSkillLevelDataInfo"));
        AddLoadProcess(LoadChartData<int, LabStatChart>("LabStatDataInfo"));
        AddLoadProcess(LoadChartData<int, LabStatRateChart>("LabStatRateTable"));
        AddLoadProcess(LoadChartData<int, LabAwakeningChart>("LabAwakeningTable"));
        AddLoadProcess(LoadChartData<int, GuildRaidDungeonChart>("GRaidDungeonTable"));
        AddLoadProcess(LoadChartData<int, AllGuildRaidDungeonChart>("30GRaidDungeonTable"));
        AddLoadProcess(LoadChartData<int, EventAttendanceChart>("EventAttendanceDataInfo"));
        AddLoadProcess(LoadChartData<int, GrowthBuffChart>("GrowthBuffDataInfo"));
        AddLoadProcess(LoadChartData<int, DpsBossDungeonChart>("DDpsBossDungeonTable"));
        AddLoadProcess(LoadChartData<int, WoodStatRateChart>("WoodStatRateTable"));
        AddLoadProcess(LoadChartData<int, WoodStatDataChart>("WoodStatDataInfo"));
        AddLoadProcess(LoadChartData<int, WoodGradeRateChart>("WoodGradeRateTable"));
        AddLoadProcess(LoadChartData<int, WoodAwakeningDataChart>("WoodAwakeningDataInfo"));
        AddLoadProcess(LoadChartData<int, ChatCouponChart>("ChatCouponTable"));
        AddLoadProcess(LoadChartData<int, CostumCollectionChart>("CostumeCollectionTable"));
        AddLoadProcess(LoadChartData<int, GuildSportsDungeonChart>("GSportsDungeonTable"));

        StartLoadProcess();

        // 확률 데이터 차트 정보
        Backend.Probability.GetProbabilityCardList(bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog("Fail GetProbabilityCardList", bro);
                return;
            }

            JsonData jsonData = bro.GetFlattenJSON();

            for (int i = 0; i < jsonData["rows"].Count; i++)
            {
                string probabilityName = jsonData["rows"][i]["probabilityName"].ToString();
                string id = jsonData["rows"][i]["selectedProbabilityFileId"].ToString();

                if (_probabilityIds.ContainsKey(probabilityName))
                    _probabilityIds[probabilityName] = id;
                else
                    _probabilityIds.Add(probabilityName, id);
            }
        });

        // 화이트 리스트 유저 닉네임 설정
        // 뒤끝의 함수로는 화이트 리스트에 등록된 유저를 체크할 수 없어서
        // 따로 화이트 리스트 차트를 만들어서 닉네임으로 사용
        Backend.Chart.GetChartContents("62395", bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog(bro);
                return;
            }

            JsonData json = bro.FlattenRows();

            for (int i = 0; i < json.Count; i++)
            {
                if (json[i].ContainsKey("Nickname") && !string.IsNullOrEmpty(json[i]["Nickname"].ToString()))
                    WhiteList.Add(json[i]["Nickname"].ToString());
            }
        });
    }

    private void AddLoadProcess(IEnumerator loadProcess)
    {
        _enumerators.Enqueue(loadProcess);
    }

    private void StartLoadProcess()
    {
        MainThreadDispatcher.StartCoroutine(_enumerators.Dequeue());
    }

    #endregion

    #region Chart Load Logic

    private bool _isChartLoadFail;
    private int _dataVersion;


    public int LoadChartCount;

    private int _loadCompleteChartCount;

    public int LoadCompleteChartCount
    {
        get => _loadCompleteChartCount;
        private set
        {
            _loadCompleteChartCount = value;

            if (_loadCompleteChartCount < LoadChartCount)
                return;

            if (_isChartLoadFail)
            {
                Debug.LogError("Fail Load Chart");
                return;
            }

            // 차트 로드 완료, 데이터 버전 저장
            PlayerPrefs.SetInt(ChartVersionKey, _dataVersion);
            LoadCachingChart();
        }
    }

    public void InitChart()
    {
        MessageBroker.Default.Publish(LoadingStep.Chart);
        Backend.Chart.GetChartContents(VersionChartUuid, bro =>
        {
            Debug.Log("GetContents");
            if (!bro.IsSuccess())
            {
                return;
            }

            var jsonData = bro.GetReturnValuetoJSON();
            var rowData = jsonData["rows"];

            if (rowData.Count <= 0)
            {
                Debug.LogError("Couldn't Load VersionData!");
                return;
            }

            for (int i = 0; i < rowData.Count; i++)
            {
                string projectType = rowData[i]["Project_Type"]["S"].ToString();

                if (projectType != Managers.Manager.ProjectType.ToString())
                    continue;

                int cacheDataVersion = PlayerPrefs.GetInt(ChartVersionKey, 0);
                int.TryParse(rowData[i]["Data_Version"]["S"].ToString(), out _dataVersion);
                ChartVersion = rowData[i]["Chart_Version"]["S"].ToString();

                if (Application.isEditor && !string.IsNullOrEmpty(Managers.Manager.EditorChartVersion))
                    ChartVersion = Managers.Manager.EditorChartVersion;

                // 캐싱된 데이터가 없거나 데이터 버전이 올라갔으면 서버 차트 불러오기
                if (cacheDataVersion < _dataVersion)
                {
                    LoadServerChartList();
                    return;
                }

                // 캐싱 데이터 로드
                LoadCachingChart();
            }
        });
    }

    private void LoadServerChartList()
    {
        Debug.Log("Start ServerChartList");

        Backend.Chart.GetChartList(bro =>
        {
            if (!bro.IsSuccess())
            {
                return;
            }

            MessageBroker.Default.Publish(new LoadServerChart());

            var json = bro.FlattenRows();

            for (int i = 0; i < json.Count; i++)
            {
                string chartFileId = json[i]["selectedChartFileId"].ToString();

                if (chartFileId == VersionChartUuid)
                    continue;

                string chartName = json[i]["chartName"].ToString();

                if (chartName.Contains("ServerDataInfo"))
                    continue;

                if (!chartName.Contains(ChartVersion))
                    continue;

                // chartName _로 버전 구분 [0] : 차트이름, [1] 버전

                Debug.Log($"ServerLoad / {chartName} / {chartFileId}");
                // AddAction(() => NewLoadServerChartData(chartFileId, chartName.Split('_')[0]));
                LoadServerChartData(chartFileId, chartName.Split('_')[0]);
                LoadChartCount++;
            }
            
            //loadActions.Dequeue().Invoke();
        });
    }

    private void LoadServerChartData(string chartFileId, string chartName)
    {
        //Backend.Chart.GetOneChartAndSave(chartFileId, chartName, bro =>
         SendQueue.Enqueue(Backend.Chart.GetOneChartAndSave, chartFileId , chartName, bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog($"Fail Save Chart : {chartName}", bro);
                return;
            }

            Debug.Log($"Success Save Chart : {chartName}");
            LoadCompleteChartCount++;
        });
    }

    private static JsonData GetLocalChartData(string chartName)
    {
        var chartJson = JsonMapper.ToObject(Backend.Chart.GetLocalChartData(chartName));

        if (!chartName.Contains("Stage") && !chartName.Contains("CostumeCollection"))
            chartJson = BackendReturnObject.Flatten(chartJson);

        if (chartJson.ContainsKey("rows"))
            return chartJson["rows"];
        
        Debug.LogError($"{chartName} Non Rows!!");
        return null;

    }

    private IEnumerator LoadChartData<T, K>(string chartName)
        where K : IChart<T>, new()
    {
        Debug.Log($"Start Co Chart Load : {chartName}");

        var chartData = GetLocalChartData(chartName);

        if (chartData == null)
            yield break;

        for (int i = 0; i < chartData.Count; i++)
        {
            var data = new K();
            data.SetData(chartData[i]);

            if (i % 100 == 0)
                yield return null;
        }

        Debug.Log($"End Co Chart Load : {chartName}");

        if (_enumerators.Count > 0)
            MainThreadDispatcher.StartCoroutine(_enumerators.Dequeue());
        else
        {
            ShopCharts = ShopCharts.OrderBy(shopChart => shopChart.Value.Sort)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            StageDataController = new StageDataController();
            StageBossDataController = new StageBossDataController();

            Managers.GameData.InitGameData();
        }
    }

    //public void StageDataLoad()
    //{
    //    MainThreadDispatcher.StartCoroutine(Cor_StageDataLoadWait());
    //}

    public static string GetString(string stringKey)
    {
        return StringCharts.TryGetValue(stringKey, out var stringChart) ? stringChart.Value : stringKey;
    }

    public static string GetGoodsName(int goodsId)
    {
        if (!ChartManager.GoodsCharts.TryGetValue(goodsId, out var goodsChart))
            return string.Empty;

        return GetString(goodsChart.Name);
    }

    #endregion
}