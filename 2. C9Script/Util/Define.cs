public enum LoadingStep
{
    None,
    Login,
    Chart,
    PlayerData,
}

public enum CharacterState
{
    None,
    Idle,
    Move,
    Move2,
    Attack,
    Attack2,
    Attack_2,
    Skill,
    RaidPortalMove,
    AllRaidRunMove,
    AllRaidCircleMove,
    Appear,
    GuildSportsEndMove,
}

public enum CharacterDirection
{
    Left,
    Right,
}

public enum UIEvent
{
    Click,
    Pressed,
    PointerDown,
    PointerUp,
}

public enum CurrencyType
{
    Default,
    SI,
}

public enum Goods
{
    None,
    Gold = 1,
    StarBalloon,
    Mileage,
    Exp,
    GoldBar,
    Dungeon1Ticket,
    Dungeon2Ticket,
    Dungeon3Ticket,
    WorldBossTicket,
    PvpTicket,
    PvpToken,
    SkillEnhancementStone,
    StatPoint,
    ChangeNicknameTicket,
    RaidTicket,
    WorldCupToken = 101,
    Snow = 102,
    NewYearToken = 103,
    GaebalToken = 104,
    GuildRaidTicket = 105,
    GuildAllRaidTicket = 106,
    GuildSportsTicket = 107,
}

public enum DamageType
{
    Normal,
    Critical,
}

public enum StatType
{
    None,
    Attack,                 // 공격력 값
    AttackPer,              // 공격력 퍼센트
    AttackSpeed,            // 공격속도 값
    AttackSpeedPer,         // 공격속도 퍼센트
    MoveSpeed,              // 이동속도 값
    MoveSpeedPer,           // 이동속도 퍼센트
    SkillCoolTimeReduce,    // 스킬 쿨타임 감소
    SkillDamage,            // 스킬 추가 데미지
    Defence,                // 방어력
    DefencePer,
    Hp,                     // 체력
    HpPer,
    Hp_Recovery,            // 체력 회복량
    BossMonsterDamage,
    IncreaseGold,
    IncreaseExp,
    IncreaseFeverDuration,
    CriticalRate2,
    CriticalRate4,
    CriticalRate8,
    CriticalRate16,
    CriticalRate32,
    CriticalRate64,
    CriticalRate128,
    CriticalRate256,
    CriticalRate512,
    CriticalRate1024,
    CriticalRate2048,
    CriticalRate4096,
    CriticalRate5009,
    CriticalRate6009,
    CriticalRate7009,
    CriticalRate8009,
    CriticalRate9009,
    CriticalDamage2,
    CriticalDamage4,
    CriticalDamage8,
    CriticalDamage16,
    CriticalDamage32,
    CriticalDamage64,
    CriticalDamage128,
    CriticalDamage256,
    CriticalDamage512,
    CriticalDamage1024,
    CriticalDamage2048,
    CriticalDamage4096,
    CriticalDamage5009,
    CriticalDamage6009,
    CriticalDamage7009,
    CriticalDamage8009,
    CriticalDamage9009,
    CriticalDamage,
    FinalDamageIncrease,
    NormalMonsterDamage,
    CriticalRate10009,
    CriticalRate11009,
    CriticalRate12009,
    CriticalRate13009,
    CriticalRate14009,
    CriticalDamage10009,
    CriticalDamage11009,
    CriticalDamage12009,
    CriticalDamage13009,
    CriticalDamage14009,
    FieldGoldBuff,
    FieldExpBuff,
    FieldTicketBuff,
    NormalAttackDamage, //Wood
}

public enum WeaponType
{
    Mic,
    Fork,
    Bat,
    Gun,
}

public enum WeaponSummonType
{
    All,
    Mic,
    Fork,
    Bat,
    Gun,
}

public enum WeaponGrade
{
    C,
    B,
    A,
    S,
}

public enum Grade
{
    None = -1,
    Normal,
    Rare,
    Unique,
    Legend,
    Legeno,
    Godgod,
}

public enum PetType
{
    Vehicle,
    Secreatary,
    Manager,
    Food,
}

public enum MonsterType
{
    Normal,
    Boss,
    StageSpecial,
    AllRaidBoss,
}

public enum StageState
{
    Normal,
    StageBoss,
    Dungeon,
    Promo,
    Pvp,
    Dps,
    WorldCupEvent,
    Raid,
    XMasEvent,
    GuildRaid,
    GuildAllRaid,
    GuildSports,
}

public enum ItemType
{
    None,
    Goods,
    Weapon,
    Pet,
    Costume,
    Collection,
}

public enum SkillPolicyProperty
{
    Hit_Count,
    Hit_Delay,
    Hit_Range,
    Hit_Range_Width,
    Hit_Range_Height,
    Skill_Duration,
    Area_Duration,
    Area_Delay,
    Area_Size,
    Melee_Target,
    Missile_Duration,
    Bullet,
    Have_Effect,
    IncreaseValue_Per_Sec,
}

public enum SystemData
{
    Default_Gold,
    Default_Balloon,
    Dungeon_Ticket_Daily_Maxcount,
    Default_Weapon,
    Default_Costume,
    Default_Pet,
    Default_Wood,
    Fever_Duration,
    Fever_Per_Sec,
    SkillSlot_OpenLevel_1,
    SkillSlot_OpenLevel_2,
    SkillSlot_OpenLevel_3,
    SkillSlot_OpenLevel_4,
    SkillSlot_OpenLevel_5,
    MaxPromoLevel,
    Pvp_Ticket_Daily_Maxcount,
    Summon_All_Weapon_1_Cost,
    Summon_All_Weapon_2_Cost,
    Summon_All_Weapon_3_Cost,
    Summon_Weapon_1_Cost,
    Summon_Weapon_2_Cost,
    Summon_Weapon_3_Cost,
    Summon_Pet_1_Cost,
    Summon_Pet_2_Cost,
    Summon_Pet_3_Cost,
    DpsDungeon_LimitTime,
    LevelUpStatPoint,
    Summon_Collection_1_Cost,
    Summon_Collection_2_Cost,
    Summon_Collection_3_Cost,
    StatResetCost,
    Offline_Reward_Min,
    Offline_Limit_Time,
    MaxStage,
    SkillResetCost,
    Raid_Ticket_Daily_Count,
    AttendanceResetFlag,
    StatPoint1_MaxLevel,
    UnlimitedPoint_MaxLevel,
    UnlimitedPoint_Level_Per_Value,
    UnlimitedPoint_ResetCost,
    Guild_FoundCost,
    Guild_Attendance_Gxp,
    Guild_Apply_Level,
    Guild_Found_Level,
    SkillAwakeningReset_Cost,
    SkillAwakeningLock_Cost,
    EventAttendanceResetFlag,
    Guild_Raid_Ticket_Daily_Count,
    All_Raid_Ticket_Daily_Count,
    Guild_Sports_Ticket_Daily_Count,
}

public enum SkillTabType
{
    Active,
    Passive,
    Lab,
}

public enum SkillRangeType
{
    Area,
    Field,
}

public enum FindType
{
    Distance,
}

public enum MessageType
{
    FailLogin,
    LackReinforceMaterial,
    FailDungeonUIByStageState,
}

public enum RankType
{
    Stage,
    Pvp,
    Raid,
    Guild,
    GuildAllRaid,
    GuildSports
}

public enum UserDataType
{
    Level,                      // 레벨
    LastConnectTime,            // 마지막 연결 시간
    ResetTime,                  // 다음 일일 초기화 시간
    CurrentStage,               // 현재 스테이지
    MaxReachStage,              // 최대 스테이지
    MaxReachStageTime,          // 최대 스테이지에 도달한 시간
    AttendanceTime,             // 다음 출석체크 시간
    AttendanceIndex,            // 출석체크 인덱스
    PromoGrade,                 // 승급전 등급
    ReceivedSummonWeaponReward, // 수령한 무기 소환레벨 획득 보상 정보
    SummonWeaponCount,          // 무기 소환 횟수
    ReceivedSummonPetReward,    // 수령한 펫 소환레벨 획득 보상 정보
    SummonPetCount,             // 펫 소환 횟수
    Dungeon1ClearStep,          // 던전1(화생방) 클리어 단계
    Dungeon1HighestValue,       // 던전1(화생방) 최고 획득 재화
    Dungeon2ClearStep,          // 던전2(해병대캠프) 클리어 단계
    Dungeon2HighestValue,       // 던전2(해병대캠프) 최고 획득 재화
    Dungeon3ClearStep,          // 던전3(행군) 클리어 단계
    Dungeon3HighestValue,       // 던전3(행군) 최고 획득 재화
    DpsDungeonHighestScore,     // Dps던전 최고 점수
    ReceivedDpsDungeonReward,   // Dps던전 획득 보상 정보
    UseStatPoint,               // 사용 스탯 포인트
    AdSkip,                     // 광고 스킵
    ProgressGuideQuestId,       // 현재 진행중인 가이드 퀘스트 ID
    ProgressGuideQuestValue,    // 현재 진행중인 가이드 퀘스트 Value
    RetentionIndex,             // 게임 총 접속일
    AttendanceResetFlag,        // 출섹체크 리셋 플래그
    UseUnlimitedPoint,          // 1201레벨부터 사용되는 언리미티드 포인트
    ResetWeeklyDateTime,        // 주간 초기화 일자
    ResetMonthlyDateTime,       // 월간 초기화 일자
    GuildAttendanceTime,        // 길드 출석체크 시간
    GuildJoinCoolTime,          // 길드 생성, 가입 신청 쿨타임
    EventAttendanceIndex,     // 특별 출석 인덱스
    EventAttendanceTime,      // 다음 특별 출석 시간
    DpsBossDungeonHighestScore,
    ReceivedDpsBossDungeonReward,
    EventAttendanceResetFlag,
}

public enum PvpDataType
{
    BaseStat,
    EquipItems,
    PromoGrade,
    Lv,
    Passive_Godgod_Lv,
    MatchCount,
}

public enum LoginType
{
    Guest,
    Google,
    Apple,
    Max,
}

public enum ServerState
{
    Close = 0,
    Good,
    Confusion,
}

public enum QuestType
{
    Daily,
    Weekly,
    Repeat,
    DailyComplete,
    WeeklyComplete
}

public enum QuestProgressType
{
    PlayTime,
    UseSkill,
    KillMonster,
    Summon,
    KillBoss,
    Combine,
    QuestComplete,
    UpgradeStat,
    EquipItem,
    EnterDungeon,
    UseAuto,
    OpenSkill,
    EquipSkill,
    BuyShop,
    ShowAdBuff,
    OpenMenu,
    UseFever,
}

public enum PostType
{
    Ranking,
    Notice,
    Apologize,
    Update,
    Event,
}

public enum ValueType
{
    Value,
    Percent,
}

public enum DungeonType
{
    None,
    Hwasengbang = 1,
    MarinCamp = 2,
    March = 3,
    Pvp = 4,
    Dps = 5,
    Promo = 6,
    Raid = 7,
    WorldCupEvent = 101,
    XMasEvent = 102,
}

public enum DungeonTabType
{
    Military,
    Dps,
}

public enum SummonType
{
    Weapon = 1,
    Pet = 2,
    Collection = 3,
    Costume,
}

public enum StatReinforceType
{
    Gold,
    Point,
}

public enum DetectionType
{
    NonMove,        // 아예 안움직이는 상태
    Detection,      // 순찰하다가 탐지 범위 내에 들어올 경우 추적
    AlwaysTracking, // 항상 유저 추적
    NonDetection,   // 추적은 하지 않고 주변 순찰만 함
    RunAway,        // 한대 맞으면 정해진 4방항 꼭지점으로 이동
    RunAwayToRandom,// 스폰, 데미지 입을때 랜덤위치로 이동
}

public enum ShopType
{
    None,
    Product = 1,
    StarBalloon = 2,
    Mileage = 3,
    PvpToken = 4,
    DailyPlayTimeMission = 5,
    LvMission = 6,
    DailyKillMission = 7,
    Costume = 8, 
}

public enum ShopProductType
{
    Package = 1,
    Costume = 2,
}

public enum ShopPriceType
{
    None,
    Free,
    Cash,
    AD,
    Goods,
}

public enum ShopLimitType
{
    None,
    Daily,
    Weekly,
    Monthly,
    NonReset,
}

public enum MissionType
{
    DailyPlayTime = 1,
    Lv,
    DailyKillCount,
}

public enum ResetType
{
    None,
    Daily,
    Weekly,
    Monthly
}

public enum MissionDataType
{
    DailyPlayTime,
    DailyKillCount,
}

public enum MissionReceiveType
{
    Free,
    Cash,
    Ad,
}

public enum WorldType
{
    Stage = 1,
    Content = 2,
}

public enum StoreType
{
    GoogleStore,
    OneStore,
}

public enum QuestUpgradeStatType
{
    GoldReinforce = 1,
    StatPointReinforce = 2,
    CollectionReinforce = 3,
}

public enum QuestCombineItemType
{
    Weapon = 1,
    Pet = 2,
}

public enum QuestEquipItemType
{
    Weapon = 1,
    Pet = 2,
    Costume = 3,
}

public enum QuestUseAutoType
{
    Fever = 1,
    Skill = 2,
}

public enum QuestOpenMenu
{
    Mission = 1,
    Ranking = 2,
    Quest = 3,
    Attendance = 4,
    Post = 5,
}

public enum QuestBuyShopType
{
    Shop = 1,
    Costume = 2,
}

public enum ProjectType
{
    Dev,
    Live,
}

public enum BgmType
{
    Title,
    Stage,
    Hwasengbang,
    Marinecamp,
    March,
    Promo,
    Dps,
    Pvp,
    WorldCup,
    Raid,
    XMasEvent,
    GuildSports,
}

public enum UISoundType
{
    DefaultButton,
    BuyShopItem,
    GiveUp,
    SuccessContents,
    ClickSummon,
    SummonResultList,
}

public enum SfxType
{
    WorldCupWhistle,
    RaidWave,
    RaidPortal,
    BoomSnow,
}

public enum PetSortType
{
    Have,
    Grade,
    TypeAscending,
    TypeDescending,
    Max,
}

public enum GuildGoodsType
{
    Gxp = 1,
    Rank=2,
    AllRaidRank = 3,
    GuildSports = 4,
}

public enum LabSkillType
{
    Fire,
    Frozen,
    Soy,
    Buzzword,
    Passive,
}

public enum RaidType
{
    Solo,
    Guild,
    
}

public enum ChatType
{
    Public,
    Guild,
}