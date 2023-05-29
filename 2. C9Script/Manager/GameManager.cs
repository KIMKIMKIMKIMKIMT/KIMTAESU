using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BackEnd;
using Chart;
using CodeStage.AntiCheat.ObscuredTypes;
using GameData;
using GameData.Data;
using LitJson;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

public class GameManager
{
    private Player _mainPlayer;

    public Player MainPlayer
    {
        get
        {
            if (_mainPlayer != null)
                return _mainPlayer;

            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj == null)
            {
                Debug.LogError("Can't Find Player Tag GameObject");
                return null;
            }

            _mainPlayer = playerObj.GetComponent<Player>();
            if (_mainPlayer == null)
                Debug.LogError("Player Component is null");

            return _mainPlayer;
        }
    }

    public string countryType = "KR";

    public double NextLevelExp = 100;

    // 승급전 등급에 따른 공격력 배수
    public int AttackMultiValue;

    // 현재 강화중인 크리티컬 배수
    public StatType? CurrentReinforceStatType;

    // 치명타 증가 데미지
    public double IncreaseCriticalDamage;

    public readonly ReactiveProperty<int> SkillQuickSlotIndex = new(0);

    public readonly List<ReactiveProperty<int>[]> EquipSkillList = new()
    {
        new ReactiveProperty<int>[]
        {
            new(0),
            new(0),
            new(0),
            new(0),
            new(0)
        },
        new ReactiveProperty<int>[]
        {
            new(0),
            new(0),
            new(0),
            new(0),
            new(0)
        },
        new ReactiveProperty<int>[]
        {
            new(0),
            new(0),
            new(0),
            new(0),
            new(0)
        }
    };

    public DateTime ServerTime;

    public bool CalculateStatFlag;

    public readonly Dictionary<int, double> BaseStatDatas = new();
    public readonly Dictionary<int, double> AdBuffStatDatas = new();
    public Dictionary<int, FloatReactiveProperty> AdBuffDurationTimes = new();

    public readonly UserData UserData = new();
    public readonly NewYearEventData NewYearEventData = new();
    public readonly ProgramerBeatData ProgramerBeatData = new();
    public readonly XMasEventData XMasEventData = new();
    public ObscuredInt LabEquipPresetId;

    public readonly Dictionary<int, ReactiveProperty<ObscuredDouble>> GoodsDatas = new();
    public readonly Dictionary<EquipType, int> EquipDatas = new();
    public readonly Dictionary<int, WeaponData> WeaponDatas = new();
    public readonly Dictionary<int, SkillData> SkillDatas = new();
    public readonly Dictionary<int, CostumeData> CostumeDatas = new();
    public readonly Dictionary<int, bool> CostumeSetDatas = new();
    public readonly Dictionary<int, PetData> PetDatas = new();
    public readonly Dictionary<int, WorldWoodData> WoodsDatas = new();
    public readonly Dictionary<int, WorldWoodAwakneingData> WoodAwakeningDatas = new();
    public readonly Dictionary<int, QuestData> QuestDatas = new();
    public readonly Dictionary<int, ReactiveProperty<ObscuredLong>> StatLevelDatas = new();
    public readonly Dictionary<int, int> ShopDatas = new();
    public readonly Dictionary<int, (byte, byte, byte)> MissionDatas = new();
    public readonly Dictionary<int, CollectionData> CollectionDatas = new();
    public readonly Dictionary<int, ReactiveProperty<ObscuredLong>> UnlimitedStatLevelDatas = new();
    public readonly Dictionary<LabSkillType, LabResearchData> LabResearchDatas = new();
    public readonly Dictionary<int, Dictionary<int, LabAwakeningData>> LabAwakeningDatas = new();
    public readonly Dictionary<int, bool> ChatCouponDatas = new();
    
    public MissionProgressData MissionProgressData = new();

    public readonly Dictionary<int, DateTime> FreeShopItemRemainTimes = new();

    public readonly ReactiveProperty<double> GainGold = new(0);
    public readonly ReactiveProperty<double> GainStarBalloon = new(0);

    public SettingData SettingData = new();
    public LabAwakeningSettingData LabAwakeningSettingData = new();
    public WorldWoodAwakeningSettingData WorldWoodAwakeningSettingData = new();

    public Action OnRefreshStat;

    public bool IsPlaying;

    private readonly CompositeDisposable _compositeDisposable = new();

    public readonly Dictionary<string, string> StageLog = new();
    public readonly Dictionary<string, string> LvLog = new();

    public readonly ReactiveProperty<bool> WorldCupAdBuff = new(false);

    public Dictionary<int, ObscuredDouble> StageItemLog = new();

    private CompositeDisposable _serverTimerComposite = new();

    private bool _isHotTime;

    public bool IsHotTime
    {
        get => _isHotTime;
        set
        {
            _isHotTime = value;
            if (_isHotTime)
            {
                switch (Utils.GetNow().DayOfWeek)
                {
                    case DayOfWeek.Monday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Thursday:
                        HotTimeDropRate = 1.1;
                        break;
                    case DayOfWeek.Friday:
                    case DayOfWeek.Saturday:
                    case DayOfWeek.Sunday:
                        HotTimeDropRate = 1.2;
                        break;
                }
            }
            else
                HotTimeDropRate = 1;

            Debug.Log(value ? "핫타임 적용중" : "핫타임 해제");
            OnChangeIsHotTime?.Invoke(value);
            CalculateStat();
        }
    }

    public Action<bool> OnChangeIsHotTime;

    public double HotTimeDropRate = 1;

    private Camera _gameCamera;

    public Camera GameCamera
    {
        get
        {
            if (_gameCamera != null)
                return _gameCamera;

            _gameCamera = Camera.main;
            return _gameCamera;
        }
    }

    public bool IsWeaponJson;

    public Vector3 GetGameViewMinPos()
    {
        return GameCamera.ViewportToWorldPoint(GameCamera.rect.min);
    }

    public Vector3 GetGameViewMaxPos()
    {
        return GameCamera.ViewportToWorldPoint(GameCamera.rect.max);
    }


    public void Init()
    {
        SetServerTime(false, CheckOfflineReward);

        if (!UserData.IsAdSkip())
        {
            string buffTimeData = PlayerPrefs.GetString("AdBuffTime", string.Empty);

            if (!string.IsNullOrEmpty(buffTimeData))
                AdBuffDurationTimes =
                    JsonConvert.DeserializeObject<Dictionary<int, FloatReactiveProperty>>(buffTimeData);
        }
        
        if (Managers.Game.IsWeaponJson)
        {
            SendErrorLog();
            void SendErrorLog()
            {
                Backend.GameLog.InsertLog("WeaponDataErrorLog", new Param
                {
                    {"LastConnectTime", UserData.LastConnectTime}
                } ,bro =>
                {
                    if (!bro.IsSuccess())
                        Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(_ => SendErrorLog());
                });
            }
        }

        SetDefaultData();
        SetPropertyEvent();
        SetPeriodCallMethod();
        
        CheckEquipItem();

        CheckAttendance();
        CheckEventAttendance();
        DailyReset();
        CheckHotTime();
        CalculateStat();

        LabAwakeningSettingData.Init();
        
        Managers.Model.SetAllRankerModelAnimation("Idle");
        
        GameDataManager.PvpGameData.SaveGameData();

        MainPlayer.Initialize();

        SetEquip();

        Managers.Model.PlayerModel.SetWeapon(EquipDatas[EquipType.Weapon]);
 
        IsPlaying = true;

        SetGuideQuest();
    }

    public void SetEquip() 
    {
        if (MainPlayer == null)
            return;

        MainPlayer.SetWeapon(EquipDatas[EquipType.Weapon]);
        MainPlayer.SetCostume(EquipDatas[EquipType.ShowCostume]);
        MainPlayer.SetPet(EquipDatas[EquipType.Pet]);

        Managers.Model.PlayerModel.SetWeapon(EquipDatas[EquipType.Weapon]);
        Managers.Model.PlayerModel.SetCostume(EquipDatas[EquipType.ShowCostume]);
    }

    public void SetDefaultData()
    {
        foreach (int statType in ChartManager.StatCharts.Keys)
        {
            AdBuffStatDatas[statType] = 0;
            
            if (!StatLevelDatas.ContainsKey(statType))
                StatLevelDatas.Add(statType, new ReactiveProperty<ObscuredLong>(0));
        }

        NextLevelExp = ChartManager.LevelCharts.ContainsKey(UserData.Level)
            ? ChartManager.LevelCharts[UserData.Level].Exp
            : 0;

        foreach (var shopChart in ChartManager.ShopCharts.Values)
        {
            if (shopChart.ShopType != ShopType.StarBalloon)
                continue;

            if (shopChart.LimitType != ShopLimitType.Daily)
                continue;

            if (shopChart.PriceType != ShopPriceType.Free &&
                shopChart.PriceType != ShopPriceType.AD)
                continue;

            string key = $"Shop_{shopChart.ShopId}_ReceiveTime";

            if (!PlayerPrefs.HasKey(key))
                continue;

            var receiveTime = DateTime.Parse(PlayerPrefs.GetString(key));

            Managers.Game.FreeShopItemRemainTimes[shopChart.ShopId] = receiveTime;
        }
    }

    private void SetPropertyEvent()
    {
        GoodsDatas[(int)Goods.Exp].Subscribe(exp =>
        {
            // 레벨 업
            if (exp >= NextLevelExp)
            {
                if (!Utils.IsContainsNextLevelData())
                    return;

                if (!ChartManager.LevelCharts.TryGetValue(UserData.Level, out var levelChart))
                    return;

                double decreaseValue = 0;

                while (exp >= NextLevelExp)
                {
                    // 레벨업 보상 지급
                    for (int i = 0; i < levelChart.RewardItemIds.Length; i++)
                    {
                        int itemId = levelChart.RewardItemIds[i];
                        double itemValue = levelChart.RewardItemValues[i];

                        IncreaseItem(ItemType.Goods, itemId, itemValue);
                    }

                    // 경험치 감소 후 레벨업
                    UserData.Level += 1;
                    InAppActivity.SendLvEvent(UserData.Level);

                    // 다음 레벨업 경험치 셋팅
                    NextLevelExp = Utils.IsContainsNextLevelData() ? ChartManager.LevelCharts[UserData.Level].Exp : 0;

                    decreaseValue += levelChart.Exp;
                    exp -= levelChart.Exp;

                    LvLog[$"Lv-{UserData.Level}"] = Utils.GetNow().ToString();

                    if (!Utils.IsContainsNextLevelData())
                        break;

                    if (!ChartManager.LevelCharts.TryGetValue(UserData.Level, out levelChart))
                        break;
                }

                // 스탯 갱신
                CalculateStat();
                DecreaseItem(ItemType.Goods, (int)Goods.Exp, decreaseValue);
                
                GameDataManager.GoodsGameData.SaveGameData();
                GameDataManager.UserGameData.SaveGameData();
            }
        });
    }

    public void SetServerTime(bool isInit = false, Action endCallback = null)
    {
        var result = Backend.Utils.GetServerTime();
        Backend.Utils.GetServerTime(bb =>
        {
        });

        Backend.Utils.GetServerTime(bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog("Fai GetServerTime", bro);
                return;
            }

            string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
            ServerTime = DateTime.Parse(time).ToKstTime();
            Debug.Log($"서버 시간 : {ServerTime.ToString()}");

            if (isInit)
            {
                _serverTimerComposite.Clear();
                Observable.Timer(TimeSpan.FromSeconds(1)).RepeatUntilDestroy(Managers.Manager) //.ObserveOnMainThread()
                    .Subscribe(
                        _ =>
                        {
                            ServerTime = ServerTime.AddSeconds(1);
                            //Debug.Log($"ServerTime : {ServerTime.ToString("O")}");
                            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.PlayTime, 1));

                            if (IsPlaying)
                                UserData.LastConnectTime = ServerTime;
                        }).AddTo(_serverTimerComposite);
            }

            endCallback?.Invoke();
        });
    }

    public DateTime GetServerTime()
    {
        var bro = Backend.Utils.GetServerTime();

        if (!bro.IsSuccess())
        {
            Managers.Backend.FailLog("Fail GetServerTime", bro);
            return new DateTime();
        }

        return DateTime.Parse(bro.GetReturnValuetoJSON()["utcTime"].ToString());
    }

    private void SetPeriodCallMethod()
    {
        SetServerTime(false, () =>
        {
            if (ServerTime < UserData.ResetTime)
                return;

            DailyReset();
        });

        // 1초마다 광고 버프 체크, 상점 무료 보상 시간 체크, 핫타임 체크
        Observable.Timer(TimeSpan.FromSeconds(1)).Where(_ => !UserData.IsAdSkip()).RepeatUntilDestroy(Managers.Manager)
            .ObserveOnMainThread().Subscribe(
                _ =>
                {
                    List<int> removeBuffIds = new List<int>();
                    List<int> adBuffKeys = AdBuffDurationTimes.Keys.ToList();

                    foreach (var buffId in adBuffKeys)
                    {
                        if (buffId == 5)
                            continue;

                        AdBuffDurationTimes[buffId].Value--;

                        if (AdBuffDurationTimes[buffId].Value > 0)
                            continue;

                        // 버프 종료
                        int statId = ChartManager.AdBuffCharts[buffId].BuffStatType;
                        AdBuffStatDatas[statId] = 0;
                        removeBuffIds.Add(buffId);
                    }

                    if (removeBuffIds.Count > 0)
                    {
                        var adBuffPopup = Managers.UI.FindPopup<UI_AdBuffPopup>();
                        removeBuffIds.ForEach(buffId =>
                        {
                            AdBuffDurationTimes.Remove(buffId);

                            if (adBuffPopup != null)
                                adBuffPopup.DisableBuff(buffId);
                        });

                        // 5번 버프도 비활성화
                        if (AdBuffStatDatas.ContainsKey(ChartManager.AdBuffCharts[5].BuffStatType))
                        {
                            AdBuffStatDatas[ChartManager.AdBuffCharts[5].BuffStatType] = 0;
                            if (adBuffPopup != null)
                                adBuffPopup.DisableBuff(5);
                        }

                        CalculateStat();
                    }
                });

        // 10초마다 버프 시간 저장
        Observable.Timer(TimeSpan.FromSeconds(10)).Where(_ => !UserData.IsAdSkip()).RepeatUntilDestroy(Managers.Manager)
            .ObserveOnMainThread().Subscribe(
                _ => { PlayerPrefs.SetString("AdBuffTime", JsonConvert.SerializeObject(AdBuffDurationTimes)); });

        // // 30초마다 초기화 시간 체크
        // Observable.Timer(TimeSpan.FromSeconds(30)).RepeatUntilDestroy(Managers.Manager).ObserveOnMainThread().Subscribe(
        //     _ =>
        //     {
        //         
        //
        //         if (ServerTime < UserData.ResetTime)
        //             return;
        //
        //         // 시간 검증
        //         SetServerTime(false, () =>
        //         {
        //             if (ServerTime < UserData.ResetTime)
        //                 return;
        //
        //             DailyReset();
        //         });
        //     });

        MainThreadDispatcher.StartCoroutine(CoCheckDailyReset());

        IEnumerator CoCheckDailyReset()
        {
            var checkDelay = new WaitForSeconds(30f);
            
            while (true)
            {
                yield return checkDelay;
                
                UserLog();

                if (ServerTime < UserData.ResetTime)
                    continue;

                // 시간 검증
                SetServerTime(false, () =>
                {
                    if (ServerTime < UserData.ResetTime)
                        return;

                    DailyReset();
                });
            }
        }

        // 1분마다 핫타임 시간체크
        Observable.Timer(TimeSpan.FromMinutes(1)).RepeatUntilDestroy(Managers.Manager).ObserveOnMainThread().Subscribe(
            _ => { CheckHotTime(); });

        // 3분마다 서버 상태 체크, 유저 데이터 저장(마지막 접속시간)
        Observable.Timer(TimeSpan.FromMinutes(3)).RepeatUntilDestroy(Managers.Manager).ObserveOnMainThread().Subscribe(
            _ =>
            {
                Managers.Server.CheckServerStatus();
                GameDataManager.UserGameData.SaveGameData();
            });

        // 10분마다 데이터 저장
        Observable.Timer(TimeSpan.FromMinutes(10)).RepeatUntilDestroy(Managers.Manager).ObserveOnMainThread().Subscribe(
            _ =>
            {
                GameDataManager.GoodsGameData.SaveGameData();
                GameDataManager.PvpGameData.SaveGameData();
                GameDataManager.QuestGameData.SaveNonReceiveQuestData();
            });
        
        // 1시간마다 드랍 아이템 로그 등록
        Observable.Timer(TimeSpan.FromHours(1)).RepeatUntilDestroy(Managers.Manager).ObserveOnMainThread().Subscribe(
            _ =>
            {
                SendStageItemLog();
                SetServerTime();
            });
    }

    void SendStageItemLog(Param param, bool retryFlag = true)
    {
        if (param == null)
            return;
                    
        if (param.Count <= 0)
            return;
                    
        Backend.GameLog.InsertLog("StageItemLog", param, bro =>
        {
            if (!bro.IsSuccess())
            {
                if (retryFlag)
                    Observable.Timer(TimeSpan.FromSeconds(5f)).Subscribe(_ => { SendStageItemLog(param, false); });
            }
        });
    }

    public void IncreaseItem(ItemType itemType, int itemId, double value)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
            {
                if (!WeaponDatas.ContainsKey(itemId))
                    return;

                WeaponDatas[itemId].Quantity = Mathf.Min((int)value + WeaponDatas[itemId].Quantity, int.MaxValue);
            }
                break;
            case ItemType.Goods:
            {
                if (!GoodsDatas.ContainsKey(itemId))
                    return;

                switch (itemId)
                {
                    case (int)Goods.Gold:
                    {
                        if (value > 0)
                            GainGold.Value += value;
                    }
                        break;
                    case (int)Goods.StarBalloon:
                        if (value > 0)
                            GainStarBalloon.Value += value;
                        break;
                }

                value = Math.Round(value);

                GoodsDatas[itemId].Value += value;
            }
                break;
            case ItemType.Costume:
            {
                if (!CostumeDatas.ContainsKey(itemId))
                    return;

                CostumeDatas[itemId].Quantity += (int)value;
            }
                break;
            case ItemType.Pet:
            {
                if (!PetDatas.ContainsKey(itemId))
                    return;

                PetDatas[itemId].Quantity += (int)value;
            }
                break;
            case ItemType.Collection:
            {
                if (!CollectionDatas.ContainsKey(itemId))
                    return;

                CollectionDatas[itemId].Quantity += (int)value;
            }
                break;
        }
    }

    public void DecreaseItem(ItemType itemType, int itemId, double value)
    {
        IncreaseItem(itemType, itemId, -value);
    }

    public void CalculateStat(bool checkFlag = false)
    {
        if (checkFlag && !CalculateStatFlag)
            return;

        CalculateStatFlag = false;

        // 최종 계산에 곱할 값
        Dictionary<int, float> finalStatRates = new Dictionary<int, float>();

        // 계산전 스탯 초기화
        foreach (var statType in ChartManager.StatCharts.Keys)
        {
            if (BaseStatDatas.ContainsKey(statType))
                BaseStatDatas[statType] = 0;
            else
                BaseStatDatas.Add(statType, 0);

            if (!finalStatRates.ContainsKey(statType))
                finalStatRates.Add(statType, 0);
        }

        // 기본 스탯
        {
            foreach (var chartData in ChartManager.StatCharts.Values)
            {
                var statType = chartData.Id;

                if (BaseStatDatas.ContainsKey(statType))
                    BaseStatDatas[statType] += chartData.DefaultValue;

                if (MainPlayer != null && MainPlayer.IsFeverMode.Value)
                    BaseStatDatas[statType] += chartData.FeverValue;
            }
        }

        // 광고
        {
            int enableAdBuffCount = 0;

            foreach (var adBuffChart in ChartManager.AdBuffCharts.Values)
            {
                if (UserData.IsAdSkip())
                {
                    if (adBuffChart.BuffStatType != (int)StatType.None)
                        BaseStatDatas[adBuffChart.BuffStatType] += adBuffChart.BuffValue;
                }
                else
                {
                    if (!AdBuffDurationTimes.TryGetValue(adBuffChart.Id, out var adBuffDurationTime))
                        continue;

                    if (adBuffDurationTime.Value <= 0)
                        continue;

                    enableAdBuffCount += 1;

                    if (adBuffChart.BuffStatType != (int)StatType.None)
                        BaseStatDatas[adBuffChart.BuffStatType] += adBuffChart.BuffValue;
                }
            }

            if (!UserData.IsAdSkip() && enableAdBuffCount >= 4)
            {
                ChartManager.AdBuffCharts.TryGetValue(5, out var adBuffChart);
                BaseStatDatas[adBuffChart.BuffStatType] += adBuffChart.BuffValue;
            }
        }

        // 월드컵 이벤트 스탯
        {
            if (Managers.Stage.State.Value == StageState.WorldCupEvent)
            {
                if (ChartManager.WorldCupEventDungeonCharts.TryGetValue(1, out var worldCupEventDungeonChart))
                {
                    if (WorldCupAdBuff.Value)
                    {
                        BaseStatDatas[(int)StatType.MoveSpeedPer] += worldCupEventDungeonChart.AdStatValue;
                    }
        
                    //BaseStatDatas[(int)StatType.MoveSpeedPer] += worldCupEventDungeonChart.CostumeStatValue;
                }
            }
        }

        // 핫타임
        {
            if (_isHotTime)
            {
                switch (Utils.GetNow().DayOfWeek)
                {
                    case DayOfWeek.Monday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Thursday:
                        BaseStatDatas[(int)StatType.IncreaseGold] += 0.5;
                        BaseStatDatas[(int)StatType.IncreaseExp] += 0.5;
                        break;
                    case DayOfWeek.Friday:
                    case DayOfWeek.Saturday:
                    case DayOfWeek.Sunday:
                        BaseStatDatas[(int)StatType.IncreaseGold] += 1;
                        BaseStatDatas[(int)StatType.IncreaseExp] += 1;
                        break;
                }
            }
        }

        // 스탯 레벨에 따른 강화값
        {
            foreach (var statLvData in StatLevelDatas)
            {
                int statId = statLvData.Key;
                long statLv = statLvData.Value.Value;

                double statValue = 0;

                if (ChartManager.StatGoldUpgradeCharts.ContainsKey(statId))
                    statValue = Utils.CalculateGoldUpgradeStat(statId, statLv);
                else if (ChartManager.StatPointUpgradeCharts.ContainsKey(statId))
                    statValue = Utils.CalculateStatPointUpgradeStat(statId, statLv);

                BaseStatDatas[statId] += statValue;
            }
        }

        // 언리미티드 포인트에 따른 스탯 레벨 강화값
        {
            foreach (var unlimitedStatLvData in UnlimitedStatLevelDatas)
            {
                int statId = unlimitedStatLvData.Key;
                long statLv = unlimitedStatLvData.Value.Value;

                if (!ChartManager.UnlimitedPointUpgradeCharts.ContainsKey(statId))
                    continue;

                BaseStatDatas[statId] += Utils.CalculateUnlimitedPointUpgradeStat(statId, statLv);
            }
        }

        // 승급전 공격력 퍼센트
        {
            if (ChartManager.PromoDungeonCharts.TryGetValue(UserData.PromoGrade, out var promoDungeonChart))
                BaseStatDatas[promoDungeonChart.ClearRewardStat1Id] += promoDungeonChart.ClearRewardStat1Value;
        }

        // 장착 무기 (공격력)
        {
            int equipWeaponIndex = EquipDatas[EquipType.Weapon];
            var weaponChart = ChartManager.WeaponCharts[equipWeaponIndex];
            BaseStatDatas[weaponChart.EquipStatType] +=
                weaponChart.EquipStatValue +
                weaponChart.EquipStatUpgradeValue *
                Math.Max(WeaponDatas[equipWeaponIndex].Level - 1, 0);
        }

        // 장착 펫
        {
            int equipPetIndex = EquipDatas[EquipType.Pet];
            var petData = PetDatas[equipPetIndex];
            var petChart = ChartManager.PetCharts[equipPetIndex];

            if (petChart.EquipStatType1 != (int)StatType.None)
                BaseStatDatas[petChart.EquipStatType1] +=
                    petChart.EquipStatValue1 + petChart.EquipStatUpgradeValue1 * petData.Level;

            if (petChart.EquipStatType2 != (int)StatType.None)
                BaseStatDatas[petChart.EquipStatType2] +=
                    petChart.EquipStatValue2 + petChart.EquipStatUpgradeValue2 * petData.Level;

            if (petChart.EquipStatType3 != (int)StatType.None)
                BaseStatDatas[petChart.EquipStatType3] +=
                    petChart.EquipStatValue3 + petChart.EquipStatUpgradeValue3 * petData.Level;

            if (petChart.EquipStatType4 != (int)StatType.None)
                BaseStatDatas[petChart.EquipStatType4] +=
                    petChart.EquipStatValue4 + petChart.EquipStatUpgradeValue4 * petData.Level;
        }

        // 장착 코스튬
        {
            int equipCostumeIndex = EquipDatas[EquipType.Costume];
            var costumeChart = ChartManager.CostumeCharts[equipCostumeIndex];

            if (costumeChart.EquipStatType1 != (int)StatType.None)
                BaseStatDatas[costumeChart.EquipStatType1] += costumeChart.EquipStatValue1;

            if (costumeChart.EquipStatType2 != (int)StatType.None)
                BaseStatDatas[costumeChart.EquipStatType2] += costumeChart.EquipStatValue2;
        }

        // 적용 세트
        {
            foreach (var setChart in ChartManager.SetCharts.Values)
            {
                if (!WeaponDatas[setChart.WeaponId].IsAcquired)
                    continue;

                if (!PetDatas[setChart.PetId].IsAcquired)
                    continue;

                if (!CostumeDatas[setChart.CostumeId].IsAcquired)
                    continue;

                if (setChart.StatType1 != (int)StatType.None)
                    BaseStatDatas[setChart.StatType1] += setChart.StatValue1;

                if (setChart.StatType2 != (int)StatType.None)
                    BaseStatDatas[setChart.StatType2] += setChart.StatValue2;

                if (setChart.StatType3 != (int)StatType.None)
                    BaseStatDatas[setChart.StatType3] += setChart.StatValue3;
            }
        }

        // 보유 무기 스탯 계산기
        {
            foreach (var weaponData in WeaponDatas)
            {
                if (!weaponData.Value.IsAcquired)
                    continue;

                if (!ChartManager.WeaponCharts.TryGetValue(weaponData.Value.Id, out var weaponChart))
                {
                    Debug.LogError($"Can't Find WeaponChart!! : {weaponData.Value.Id}");
                    continue;
                }

                if (weaponChart.HaveStatType != (int)StatType.None)
                    BaseStatDatas[weaponChart.HaveStatType] += weaponChart.HaveStatValue +
                                                               weaponChart.HaveStatUpgradeValue *
                                                               Math.Max(weaponData.Value.Level - 1, 0);
            }
        }

        // 보유 펫
        {
            foreach (var petData in PetDatas.Values)
            {
                if (!petData.IsAcquired)
                    continue;

                if (!ChartManager.PetCharts.TryGetValue(petData.Id, out var petChart))
                {
                    Debug.LogError($"Can't Find PetChart!! : {petData.Id}");
                    continue;
                }

                int petLv = Math.Max(petData.Level - 1, 0);

                if (petChart.HaveStatType1 != (int)StatType.None)
                    BaseStatDatas[petChart.HaveStatType1] +=
                        petChart.HaveStatValue1 + petChart.HaveStatUpgradeValue1 * petLv;

                if (petChart.HaveStatType2 != (int)StatType.None)
                    BaseStatDatas[petChart.HaveStatType2] +=
                        petChart.HaveStatValue2 + petChart.HaveStatUpgradeValue2 * petLv;

                if (petChart.HaveStatType3 != (int)StatType.None)
                    BaseStatDatas[petChart.HaveStatType3] +=
                        petChart.HaveStatValue3 + petChart.HaveStatUpgradeValue3 * petLv;

                if (petChart.HaveStatType4 != (int)StatType.None)
                    BaseStatDatas[petChart.HaveStatType4] +=
                        petChart.HaveStatValue4 + petChart.HaveStatUpgradeValue4 * petLv;
            }
        }

        // 나무 스탯
        {
            if (WoodsDatas[0].Id != 0)
            {
                int woodlevel = Math.Max(WoodsDatas[0].Level - 1, 0);

                if (!ChartManager.WoodCharts.TryGetValue(WoodsDatas[0].Id, out var woodChart))
                {
                    Debug.LogError($"Can't Find WoodChart!! : {WoodsDatas[0].Id}");
                }

                if (woodChart.GradeStatType1 != (int)StatType.None)
                {
                    BaseStatDatas[woodChart.GradeStatType1] +=
                        woodChart.GradeStatValue1 + woodChart.GradeStatIncreaseValue1 * woodlevel;
                }
                if (woodChart.GradeStatType2 != (int)StatType.None)
                {
                    BaseStatDatas[woodChart.GradeStatType2] +=
                        woodChart.GradeStatValue2 + woodChart.GradeStatIncreaseValue2 * woodlevel;
                }
                if (woodChart.GradeStatType3 != (int)StatType.None)
                {
                    BaseStatDatas[woodChart.GradeStatType3] +=
                        woodChart.GradeStatValue3 + woodChart.GradeStatIncreaseValue3 * woodlevel;
                }
                if (woodChart.GradeStatType4 != (int)StatType.None)
                {
                    BaseStatDatas[woodChart.GradeStatType4] +=
                        woodChart.GradeStatValue4 + woodChart.GradeStatIncreaseValue4 * woodlevel;
                }
            }
        }

        // 보유 코스튬
        {
            foreach (var costumeData in CostumeDatas.Values)
            {
                if (!costumeData.IsAcquired)
                    continue;

                if (!ChartManager.CostumeCharts.TryGetValue(costumeData.Id, out var costumeChart))
                {
                    Debug.LogError($"Can't Find CostumeChart!! : {costumeData.Id}");
                    continue;
                }

                // 보유 효과
                if (costumeChart.HaveStatType1 != (int)StatType.None)
                    BaseStatDatas[costumeChart.HaveStatType1] += costumeChart.HaveStatValue1;
                if (costumeChart.HaveStatType2 != (int)StatType.None)
                    BaseStatDatas[costumeChart.HaveStatType2] += costumeChart.HaveStatValue2;

                // 각성 효과
                if (costumeChart.MaxAwakening == 0)
                    continue;

                for (int awakeningLv = 1; awakeningLv <= costumeData.Awakening; awakeningLv++)
                {
                    if (!ChartManager.CostumeAwakeningCharts.TryGetValue((costumeChart.Id, awakeningLv),
                            out var costumeAwakeningChart))
                    {
                        Debug.LogError($"코스튬 {costumeChart.Id} 각성 {awakeningLv} 데이터 없음!");
                        continue;
                    }

                    if (costumeAwakeningChart.StatType != (int)StatType.None)
                        BaseStatDatas[costumeAwakeningChart.StatType] += costumeAwakeningChart.StatValue;
                }
            }
        }

        

        // 보유 패시브 스킬 계산
        {
            foreach (var skillData in SkillDatas.Values)
            {
                if (!ChartManager.SkillCharts.TryGetValue(skillData.Id, out var skillChart))
                    continue;

                if (skillChart.TabType != SkillTabType.Passive)
                    continue;

                if (skillData.Level <= 0)
                    continue;

                if (skillData.Id == 57)
                    continue;

                int statType = (int)ChartManager
                    .SkillPolicyCharts[(skillData.Id, SkillPolicyProperty.Have_Effect)]
                    .Value;

                BaseStatDatas[statType] +=
                    skillChart.Value + (skillChart.IncreaseValue * Math.Max(skillData.Level - 1, 0));
            }
        }

        // 유물
        {
            foreach (var collectionData in CollectionDatas.Values)
            {
                if (collectionData.Lv <= 0)
                    continue;

                var collectionChart = ChartManager.CollectionCharts[collectionData.CollectionId];

                BaseStatDatas[collectionChart.StatType] += collectionChart.StatIncreaseValue * collectionData.Lv;
            }
        }
        
        // 길드 버프
        {
            if (Managers.Guild.GuildData != null)
            {
                if (ChartManager.GuildBuffCharts.TryGetValue(Managers.Guild.GuildData.GuildMetaData.Grade,
                        out var guildBuffChart))
                {
                    AddStat(guildBuffChart.EffectStatId1, guildBuffChart.EffectStatValue1);
                    AddStat(guildBuffChart.EffectStatId2, guildBuffChart.EffectStatValue2);
                    AddStat(guildBuffChart.EffectStatId3, guildBuffChart.EffectStatValue3);
                    AddStat(guildBuffChart.EffectStatId4, guildBuffChart.EffectStatValue4);
                }

                void AddStat(int statId, double statValue)
                {
                    if (statId == (int)StatType.None)
                        return;

                    BaseStatDatas[statId] += statValue;
                }
            }
        }
        
        // 연구소 문양 세트
        {
            if (Managers.Game.LabAwakeningDatas.TryGetValue(LabEquipPresetId, out var labAwakeningDatas))
            {
                Dictionary<int, int> patternCountDictionary = new()
                {
                    { 1, labAwakeningDatas.Values.Where(data => data.StatId != 0).Count(data => data.PatternId == 1) },
                    { 2, labAwakeningDatas.Values.Where(data => data.StatId != 0).Count(data => data.PatternId == 2) },
                    { 3, labAwakeningDatas.Values.Where(data => data.StatId != 0).Count(data => data.PatternId == 3) },
                    { 4, labAwakeningDatas.Values.Where(data => data.StatId != 0).Count(data => data.PatternId == 4) }
                };
                
                Dictionary<int, int> applySetDictionary = new()
                {
                    { 1, 0 },
                    { 2, 0 },
                    { 3, 0 },
                    { 4, 0 }
                };

                var isNextPattern = false;
                var prevPatternId = 0;

                foreach (var chartData in ChartManager.LabSetCharts.Values)
                {
                    if (!patternCountDictionary.TryGetValue(chartData.SetPattern, out var patternCount))
                        continue;

                    if (isNextPattern)
                    {
                        if (prevPatternId == chartData.SetPattern)
                            continue;
                
                        isNextPattern = false;
                    }

                    if (patternCount < chartData.SetNumber)
                    {
                        isNextPattern = true;
                        prevPatternId = chartData.SetPattern;
                        continue;
                    }

                    applySetDictionary[chartData.SetPattern] = chartData.Id;
                }
                
                for (var patternId = 1; patternId <= 4; patternId++)
                {
                    if (!patternCountDictionary.TryGetValue(patternId, out var patternCount))
                        continue;

                    if (!applySetDictionary.TryGetValue(patternId, out var setId))
                        continue;

                    if (!ChartManager.LabSetCharts.TryGetValue(setId, out var labSetChart))
                        continue;

                    if (labSetChart.StatType == 0)
                        continue;

                    BaseStatDatas[labSetChart.StatType] += labSetChart.StatValue;
                }
            }
        }
        
        // 연구소 각성 스텟
        {
            foreach (var labAwakeningData in LabAwakeningDatas[LabEquipPresetId].Values)
            {
                if (labAwakeningData.StatId == 0)
                    continue;
                
                BaseStatDatas[labAwakeningData.StatId] += labAwakeningData.StatValue;
            }
        }

        // CostumeSetEffect
        {
            foreach (var costumeSetData in Managers.Game.CostumeSetDatas)
            {
                if (!costumeSetData.Value)
                    continue;

                BaseStatDatas[ChartManager.CostumCollectionCharts[costumeSetData.Key].StatId]
                    += ChartManager.CostumCollectionCharts[costumeSetData.Key].StatValue;
            }

            BaseStatDatas[(int)StatType.FinalDamageIncrease] += Managers.CostumeSet.SetCount * 0.1f;
        }

        // WoodAwakeningStat
        {
            foreach (var woodAwakeningdata in WoodAwakeningDatas.Values)
            {
                if (woodAwakeningdata.StatId == 0)
                    continue;

                BaseStatDatas[woodAwakeningdata.StatId] += woodAwakeningdata.StatValue;
            }
        }
        
        // 성장 버프
        {
            foreach (var growthBuffChart in ChartManager.GrowthBuffCharts.Values)
            {
                if (Managers.Game.UserData.Level > growthBuffChart.ApplyMaxLv)
                    continue;

                for (var i = 0; i < growthBuffChart.BuffStatIds.Length; i++)
                    BaseStatDatas[growthBuffChart.BuffStatIds[i]] += growthBuffChart.BuffStatValues[i];
                
                break;
            }
        }

        // 치명타 추가 데미지 계산
        {
            IncreaseCriticalDamage = 0;

            for (int statType = (int)StatType.CriticalDamage2; statType <= (int)StatType.CriticalDamage9009; statType++)
            {
                if (BaseStatDatas[statType] <= 0)
                    break;

                IncreaseCriticalDamage += BaseStatDatas[statType];
            }
            
            for (int statType = (int)StatType.CriticalDamage10009; statType <= (int)StatType.CriticalDamage14009; statType++)
            {
                if (BaseStatDatas[statType] <= 0)
                    break;

                IncreaseCriticalDamage += BaseStatDatas[statType];
            }
            
            IncreaseCriticalDamage += BaseStatDatas[(int)StatType.CriticalDamage];
        }

        var baseStatDatasKeys = BaseStatDatas.Keys.ToList();

        foreach (var baseStatKey in baseStatDatasKeys)
        {
            var value = Math.Round(BaseStatDatas[baseStatKey], 7);
            BaseStatDatas[baseStatKey] = value;
        }

        OnRefreshStat?.Invoke();

        if (MainPlayer != null)
            MainPlayer.RefreshStat();
    }

    private void DailyReset()
    {
        if (Utils.GetNow() < UserData.ResetTime)
            return;

        var param = new Param();
        
        param.Add("Before Goods", GameDataManager.GoodsGameData.GetLog());

        UserData.RetentionIndex += 1;
        InAppActivity.SendRetentionEvent(UserData.RetentionIndex);

        UserData.ResetTime = Utils.GetDay(1);

        // 던전 티켓
        {
            IncreaseItem(ItemType.Goods, (int)Goods.Dungeon1Ticket,
                ChartManager.SystemCharts[SystemData.Dungeon_Ticket_Daily_Maxcount].Value);
            IncreaseItem(ItemType.Goods, (int)Goods.Dungeon2Ticket,
                ChartManager.SystemCharts[SystemData.Dungeon_Ticket_Daily_Maxcount].Value);
            IncreaseItem(ItemType.Goods, (int)Goods.Dungeon3Ticket,
                ChartManager.SystemCharts[SystemData.Dungeon_Ticket_Daily_Maxcount].Value);
        }

        // PVP 티켓
        {
            IncreaseItem(ItemType.Goods, (int)Goods.PvpTicket,
                ChartManager.SystemCharts[SystemData.Pvp_Ticket_Daily_Maxcount].Value);
        }

        // 레이드 티켓
        {
            IncreaseItem(ItemType.Goods, (int)Goods.RaidTicket,
                ChartManager.SystemCharts[SystemData.Raid_Ticket_Daily_Count].Value);
        }

        // GuildRaid Ticket
        {
            GoodsDatas[(int)Goods.GuildRaidTicket].Value += 1;
            GoodsDatas[(int)Goods.GuildAllRaidTicket].Value += 1;
            GoodsDatas[(int)Goods.GuildSportsTicket].Value += 1;
        }

        //GameDataManager.GoodsGameData.SaveGameData();

        var dayOfWeek = Utils.GetDay().DayOfWeek;

        bool isWeeklyReset = dayOfWeek == DayOfWeek.Monday || UserData.ResetWeeklyDateTime <= Utils.GetNow();
        bool isMonthlyReset = Utils.GetNow().Day == 1 || UserData.ResetMonthlyDateTime <= Utils.GetNow();

        if (isWeeklyReset)
            UserData.ResetWeeklyDateTime = Utils.GetNextWeeklyDate();

        if (isMonthlyReset)
            UserData.ResetMonthlyDateTime = Utils.GetNextMonthDate();

        //GameDataManager.UserGameData.SaveGameData();

        // 퀘스트
        foreach (var questData in QuestDatas.Values)
        {
            QuestChart questChart = ChartManager.QuestCharts[questData.Id];

            // 반복 퀘스트는 초기화 제외
            if (questChart.Type == QuestType.Repeat)
                continue;

            // 주간 퀘스트는 월요일에 초기화
            if (questChart.Type == QuestType.Weekly || questChart.Type == QuestType.WeeklyComplete)
            {
                if (!isWeeklyReset)
                    continue;
            }

            questData.ProgressValue = 0;
            questData.IsReceiveReward = false;
        }

        //GameDataManager.QuestGameData.SaveGameData();


        // 출석체크
        CheckAttendance();
        
        // 이벤트 출석체크
        CheckEventAttendance();

        // 상점 상품 리셋
        foreach (var shopChart in ChartManager.ShopCharts.Values)
        {
            if (shopChart.LimitType == ShopLimitType.None || shopChart.LimitType == ShopLimitType.NonReset)
                continue;

            if (shopChart.LimitType == ShopLimitType.Weekly)
            {
                if (!isWeeklyReset)
                    continue;
            }
            else if (shopChart.LimitType == ShopLimitType.Monthly)
            {
                if (!isMonthlyReset)
                    continue;
            }

            if (ShopDatas.ContainsKey(shopChart.ShopId))
                ShopDatas[shopChart.ShopId] = 0;
        }

        //GameDataManager.ShopGameData.SaveGameData();

        // 미션
        foreach (var missionChart in ChartManager.MissionCharts.Values)
        {
            if (!MissionDatas.ContainsKey(missionChart.MissionId))
                continue;

            if (missionChart.ResetType == ResetType.None)
                continue;

            switch (missionChart.ResetType)
            {
                case ResetType.Daily:
                    MissionDatas[missionChart.MissionId] = (0, 0, 0);
                    break;
                case ResetType.Weekly when isWeeklyReset:
                    MissionDatas[missionChart.MissionId] = (0, 0, 0);
                    break;
                case ResetType.Monthly when isMonthlyReset:
                    MissionDatas[missionChart.MissionId] = (0, 0, 0);
                    break;
            }
        }

        MissionProgressData.DailyKillCount = 0;
        MissionProgressData.DailyPlayTime = 0;

        //GameDataManager.MissionGameData.SaveGameData();

        // 새해 이벤트
        NewYearEventData.NewYearDungeonEntryCount = ChartManager.WorldCupEventDungeonCharts[1].DailyEntryCount;
        param.Add("NewYear Entry", NewYearEventData.NewYearDungeonEntryCount);
        //GameDataManager.NewYearEventGameData.SaveGameData();

        // ProgramerBeat Event
        ProgramerBeatData.ProgramerBeatEntryCount = ChartManager.WorldCupEventDungeonCharts[1].DailyEntryCount;
        param.Add("ProgramerBeat Entry", ProgramerBeatData.ProgramerBeatEntryCount);

        // 길드 레이드 입장횟수
        Managers.GuildRaid.EntryCount = 1;

        // 길드 30인 레이드 입장횟수
        Managers.AllRaid.EntryCount = 1;

        param.Add("GuildRaid Entry", Managers.GuildRaid.EntryCount.GetDecrypted());
        param.Add("GuildAllRaid Entry", Managers.AllRaid.EntryCount.GetDecrypted());
        
        // GameDataManager.GuildGameData.SaveGameData();
        //
        // Managers.GameData.SaveAllGameDataTransaction();

        GameDataManager.GameDatas.ForEach(gameData => gameData.SaveGameData(true));

        param.Add("After Goods", GameDataManager.GoodsGameData.GetLog());
        param.Add("After Mission", GameDataManager.MissionGameData.GetLog());
        param.Add("After Quest", GameDataManager.QuestGameData.GetLog());
        param.Add("After Shop", GameDataManager.ShopGameData.GetLog());
        
        Debug.Log($"DailyReset Log Size : {Encoding.UTF8.GetByteCount(JsonMapper.ToJson(param))}");

        Backend.GameLog.InsertLog("DailyReset", param);
    }

    public void CheckAttendance()
    {
        if (Utils.GetNow() < UserData.AttendanceDate)
            return;

        // 14일 보상을 모두 받았으면 초기화
        if (UserData.AttendanceIndex > ChartManager.AttendanceCharts.Keys.Max(id => id))
            UserData.AttendanceIndex = 1;

        if (Managers.UI.FindPopup<UI_AttendancePopup>() == null)
            Managers.UI.ShowPopupUI<UI_AttendancePopup>();
    }

    public void CheckEventAttendance()
    {
        if (Utils.GetNow() < UserData.EventAttendanceTime)
            return;

        if (!ChartManager.EventAttendanceCharts.ContainsKey(UserData.EventAttendanceIndex))
            return;

        if (Managers.UI.FindPopup<UI_EventAttendancePopup>() == null)
            Managers.UI.ShowPopupUI<UI_EventAttendancePopup>();
    }

    private void CheckOfflineReward()
    {
        TimeSpan offlineTime = Utils.GetNow() - UserData.LastConnectTime;

        if ((int)offlineTime.TotalMinutes >= 10)
            Managers.UI.ShowPopupUI<UI_OfflineRewardPopup>();
    }

    private void SetGuideQuest()
    {
        _compositeDisposable.Clear();

        if (!ChartManager.GuideQuestCharts.TryGetValue(UserData.ProgressGuideQuestId, out var guideQuestChart))
            return;

        MessageBroker.Default.Receive<QuestMessage>().Where(questMessage =>
            questMessage.ProgressType == guideQuestChart.QuestProgressType &&
            (guideQuestChart.QuestProgressId == 0 || guideQuestChart.QuestProgressId == questMessage.Id)
        ).Subscribe(questMessage =>
        {
            if (UserData.ProgressGuideQuestValue + questMessage.Value > guideQuestChart.QuestCompleteValue)
                UserData.ProgressGuideQuestValue = guideQuestChart.QuestCompleteValue;
            else
                UserData.ProgressGuideQuestValue += questMessage.Value;
        }).AddTo(_compositeDisposable);
        
        MessageBroker.Default.Receive<GuideQuestMessage>().Where(questMessage =>
            questMessage.ProgressType == guideQuestChart.QuestProgressType &&
            (guideQuestChart.QuestProgressId == 0 || guideQuestChart.QuestProgressId == questMessage.Id)
        ).Subscribe(questMessage =>
        {
            if (UserData.ProgressGuideQuestValue + questMessage.Value > guideQuestChart.QuestCompleteValue)
                UserData.ProgressGuideQuestValue = guideQuestChart.QuestCompleteValue;
            else
                UserData.ProgressGuideQuestValue += questMessage.Value;
        }).AddTo(_compositeDisposable);

        CheckGuideQuestChart(guideQuestChart);
    }

    public void SetNextGuideQuest()
    {
        UserData.ProgressGuideQuestValue = 0;
        UserData.ProgressGuideQuestId += 1;

        if (!ChartManager.GuideQuestCharts.TryGetValue(UserData.ProgressGuideQuestId, out var guideQuestChart))
            return;

        _compositeDisposable.Clear();

        MessageBroker.Default.Receive<QuestMessage>().Where(questMessage =>
            questMessage.ProgressType == guideQuestChart.QuestProgressType &&
            (guideQuestChart.QuestProgressId == 0 || guideQuestChart.QuestProgressId == questMessage.Id)
        ).Subscribe(questMessage =>
        {
            if (UserData.ProgressGuideQuestValue + questMessage.Value > guideQuestChart.QuestCompleteValue)
                UserData.ProgressGuideQuestValue = guideQuestChart.QuestCompleteValue;
            else
                UserData.ProgressGuideQuestValue += questMessage.Value;
        }).AddTo(_compositeDisposable);
        
        MessageBroker.Default.Receive<GuideQuestMessage>().Where(questMessage =>
            questMessage.ProgressType == guideQuestChart.QuestProgressType &&
            (guideQuestChart.QuestProgressId == 0 || guideQuestChart.QuestProgressId == questMessage.Id)
        ).Subscribe(questMessage =>
        {
            if (UserData.ProgressGuideQuestValue + questMessage.Value > guideQuestChart.QuestCompleteValue)
                UserData.ProgressGuideQuestValue = guideQuestChart.QuestCompleteValue;
            else
                UserData.ProgressGuideQuestValue += questMessage.Value;
        }).AddTo(_compositeDisposable);

        CheckGuideQuestChart(guideQuestChart);
    }

    public void UserLog()
    {
        Param param = new Param();

        if (LvLog.Count > 0)
        {
            foreach (var lvLog in LvLog)
                param.Add($"{lvLog.Key}", lvLog.Value);
            LvLog.Clear();
        }

        if (StageLog.Count > 0)
        {
            foreach (var stageLog in StageLog)
                param.Add($"{stageLog.Key}", stageLog.Value);
            StageLog.Clear();
        }

        if (param.Count > 0)
        {
            Utils.GetGoodsLog(ref param);
            Backend.GameLog.InsertLog("User", param);
        }
    }

    public void SendStageItemLog()
    {
        var param = new Param();

        var stageItemLogKeys = StageItemLog.Keys.ToList();
        
        foreach (var stageItemLogKey in stageItemLogKeys)
        {
            var stageItemLog = StageItemLog[stageItemLogKey].GetDecrypted();
            if (stageItemLog <= 0)
                return;

            param.Add($"Goods_{stageItemLogKey}", stageItemLog);
            StageItemLog[stageItemLogKey] = 0;
        }
        
        Debug.Log("스테이지 로그 발송");

        SendStageItemLog(param);
    }

    public void CheckGuideQuestChart(GuideQuestChart guideQuestChart)
    {
        switch (guideQuestChart.QuestProgressType)
        {
            case QuestProgressType.Summon:
            {
                MessageBroker.Default.Publish(new GuideQuestMessage(QuestProgressType.Summon, (int)SummonType.Weapon,
                    UserData.SummonWeaponCount));
                MessageBroker.Default.Publish(new GuideQuestMessage(QuestProgressType.Summon, (int)SummonType.Pet,
                    UserData.SummonPetCount));
            }
                break;
            case QuestProgressType.QuestComplete:
            {
                if (QuestDatas == null)
                    return;

                QuestDatas.Values.ToList().FindAll(questData => questData.IsReceiveReward).ForEach(questData =>
                    MessageBroker.Default.Publish(new GuideQuestMessage(QuestProgressType.QuestComplete, questData.Id, 1)));
            }
                break;
            case QuestProgressType.UpgradeStat:
            {
                if (guideQuestChart.QuestProgressId == 0 ||
                    guideQuestChart.QuestProgressId == (int)QuestUpgradeStatType.GoldReinforce)
                {
                    long sumLv = 0;

                    foreach (var statId in ChartManager.StatGoldUpgradeCharts.Keys)
                    {
                        if (!StatLevelDatas.ContainsKey(statId))
                            continue;

                        sumLv += StatLevelDatas[statId].Value;
                    }

                    MessageBroker.Default.Publish(new GuideQuestMessage(QuestProgressType.UpgradeStat,
                        (int)QuestUpgradeStatType.GoldReinforce, sumLv));
                }

                if (guideQuestChart.QuestProgressId == 0 ||
                    guideQuestChart.QuestProgressId == (int)QuestUpgradeStatType.StatPointReinforce)
                {
                    long sumLv = 0;
                    
                    foreach (var statId in ChartManager.StatPointUpgradeCharts.Keys)
                    {
                        if (!StatLevelDatas.ContainsKey(statId))
                            continue;

                        sumLv += StatLevelDatas[statId].Value;
                    }

                    MessageBroker.Default.Publish(new GuideQuestMessage(QuestProgressType.UpgradeStat,
                        (int)QuestUpgradeStatType.StatPointReinforce, sumLv));
                }

                if (guideQuestChart.QuestProgressId == 0 ||
                    guideQuestChart.QuestProgressId == (int)QuestUpgradeStatType.CollectionReinforce)
                {
                    long sumLv = 0;

                    foreach (var collectionChart in ChartManager.CollectionCharts.Values)
                    {
                        var collectionId = collectionChart.CollectionId;

                        if (!CollectionDatas.ContainsKey(collectionId))
                            continue;

                        sumLv += CollectionDatas[collectionId].Lv;
                    }

                    MessageBroker.Default.Publish(new GuideQuestMessage(QuestProgressType.UpgradeStat,
                        (int)QuestUpgradeStatType.CollectionReinforce, sumLv));
                }
            }
                break;
            case QuestProgressType.UseAuto:
            {
                if (guideQuestChart.QuestProgressId == 0 ||
                    guideQuestChart.QuestProgressId == (int)QuestUseAutoType.Fever)
                {
                    if (Managers.Game.MainPlayer == null)
                        return;

                    if (Managers.Game.MainPlayer.AutoFeverMode == null)
                        return;

                    if (Managers.Game.MainPlayer.AutoFeverMode.Value)
                        MessageBroker.Default.Publish(new GuideQuestMessage(QuestProgressType.UseAuto,
                            (int)QuestUseAutoType.Fever, 1));
                }

                if (guideQuestChart.QuestProgressId == 0 ||
                    guideQuestChart.QuestProgressId == (int)QuestUseAutoType.Skill)
                {
                    if (Managers.Game.MainPlayer.AutoSkillMode.Value)
                        MessageBroker.Default.Publish(new GuideQuestMessage(QuestProgressType.UseAuto,
                            (int)QuestUseAutoType.Skill, 1));
                }
            }
                break;
            case QuestProgressType.OpenSkill:
            {
                foreach (var skillData in SkillDatas.Values)
                {
                    if (!skillData.IsOpen)
                        continue;

                    if (guideQuestChart.QuestProgressId == 0 || guideQuestChart.QuestProgressId == skillData.Id)
                        MessageBroker.Default.Publish(new GuideQuestMessage(QuestProgressType.OpenSkill, skillData.Id, 1));
                }
            }
                break;
            case QuestProgressType.EquipSkill:
            {
                foreach (var equipSkillDatas in EquipSkillList)
                {
                    foreach (var equipSkill in equipSkillDatas)
                    {
                        if (equipSkill.Value != 0)
                            MessageBroker.Default.Publish(new GuideQuestMessage(QuestProgressType.EquipSkill,
                                equipSkill.Value, 1));
                    }
                }
            }
                break;
            case QuestProgressType.BuyShop:
            {
                foreach (var shopData in ShopDatas)
                {
                    if (shopData.Value > 0)
                    {
                        MessageBroker.Default.Publish(new GuideQuestMessage(QuestProgressType.BuyShop, shopData.Key,
                            shopData.Value));
                    }
                }
            }
                break;
        }
    }

    private void CheckHotTime()
    {
        bool IsHotTimeNow()
        {
            var nowTime = Utils.GetNow();

            var checkMinTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 20, 0, 0);
            var checkMaxTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 23, 0, 0);
            return nowTime >= checkMinTime && nowTime <= checkMaxTime;
        }

        if (IsHotTime)
        {
            if (!IsHotTimeNow())
            {
                IsHotTime = false;
            }
        }
        else
        {
            if (IsHotTimeNow())
            {
                IsHotTime = true;
            }
        }
    }

    private void CheckEquipItem()
    {
        bool isSaveFlag = false;
        
        // 무기
        if (WeaponDatas.TryGetValue(EquipDatas[EquipType.Weapon], out var equipWeaponData) && !equipWeaponData.IsAcquired)
        {
            var acquiredWeapons = Managers.Game.WeaponDatas.Values.ToList().FindAll(weaponData => weaponData.IsAcquired);

            int weaponId = 0;
            double beforeAttack = 0;

            foreach (var weaponData in acquiredWeapons)
            {
                var weaponChart = ChartManager.WeaponCharts[weaponData.Id];
                double attack = weaponChart.EquipStatValue + weaponChart.EquipStatUpgradeValue * (weaponData.Level - 1);

                if (attack > beforeAttack)
                {
                    weaponId = weaponData.Id;
                    beforeAttack = attack;
                }
            }
        
            Managers.Game.EquipDatas[EquipType.Weapon] = weaponId;

            isSaveFlag = true;
        }
        
        // 코스튬
        if (CostumeDatas.TryGetValue(EquipDatas[EquipType.Costume], out var equipCostumeData) &&
            !equipCostumeData.IsAcquired)
        {
            var acquiredCostumes = Managers.Game.CostumeDatas.Values.ToList().FindAll(costumeData => costumeData.IsAcquired);

            int costumeId = 0;
            double costumeBeforeStat = 0;


            foreach (var costumeData in acquiredCostumes)
            {
                var costumeChart = ChartManager.CostumeCharts[costumeData.Id];
                double equipStatValue = costumeChart.EquipStatValue1;

                if (equipStatValue > costumeBeforeStat)
                {
                    costumeId = costumeData.Id;
                    costumeBeforeStat = equipStatValue;
                }
            }

            Managers.Game.EquipDatas[EquipType.Costume] = costumeId;
            Managers.Game.EquipDatas[EquipType.ShowCostume] = costumeId;

            isSaveFlag = true;
        }

        // 펫
        if (PetDatas.TryGetValue(EquipDatas[EquipType.Pet], out var equipPetData) && !equipPetData.IsAcquired)
        {
            var acquiredPets = Managers.Game.PetDatas.Values.ToList().FindAll(petData => petData.IsAcquired);

            int petId = 0;
            double petBeforeStat = 0;


            foreach (var petData in acquiredPets)
            {
                var petChart = ChartManager.PetCharts[petData.Id];
                double equipStatValue = petChart.EquipStatValue1 + petChart.EquipStatUpgradeValue1 * (petData.Level - 1);

                if (equipStatValue > petBeforeStat)
                {
                    petId = petData.Id;
                    petBeforeStat = equipStatValue;
                }
            }
            
            Managers.Game.EquipDatas[EquipType.Pet] = petId;
            
            isSaveFlag = true;
        }
        
        if (isSaveFlag)
            GameDataManager.EquipGameData.SaveGameData();
    }
}