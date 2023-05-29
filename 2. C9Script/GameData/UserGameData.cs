using System;
using System.Collections.Generic;
using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using Newtonsoft.Json;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace GameData
{
    public class UserData
    {
        private ObscuredInt _level;

        public int Level
        {
            get => _level;
            set
            {
                bool isChange = _level != value;
                _level = value;
                
                if (isChange)
                    OnChangeLevel?.OnNext(_level);
            }
        }

        public DateTime LastConnectTime;
        public DateTime ResetTime;

        public int CurrentStage
        {
            get => Managers.Stage.StageId.Value;
            set => Managers.Stage.StageId.Value = value;
        }

        public ObscuredInt MaxReachStage = 1;
        public DateTime MaxReachStageTime;
        public DateTime AttendanceDate;

        private ObscuredInt _attendanceIndex;

        public int AttendanceIndex
        {
            get => _attendanceIndex;
            set => _attendanceIndex = value;
        }

        private ObscuredInt _promoGrade;

        public int PromoGrade
        {
            get => _promoGrade;
            set => _promoGrade = value;
        }

        public List<int> ReceivedSummonWeaponReward = new();

        private ObscuredInt _dungeon1ClearStep;

        public int Dungeon1ClearStep
        {
            get => _dungeon1ClearStep;
            set => _dungeon1ClearStep = value;
        }

        private ObscuredDouble _dungeon1HighestValue;

        public double Dungeon1HighestValue
        {
            get => _dungeon1HighestValue;
            set => _dungeon1HighestValue = value;
        }

        private ObscuredInt _dungeon2ClearStep;

        public int Dungeon2ClearStep
        {
            get => _dungeon2ClearStep;
            set => _dungeon2ClearStep = value;
        }

        private ObscuredDouble _dungeon2HighestValue;

        public double Dungeon2HighestValue
        {
            get => _dungeon2HighestValue;
            set => _dungeon2HighestValue = value;
        }

        private ObscuredInt _dungeon3ClearStep;

        public int Dungeon3ClearStep
        {
            get => _dungeon3ClearStep;
            set => _dungeon3ClearStep = value;
        }

        private ObscuredDouble _dungeon3HighestValue;

        public double Dungeon3HighestValue
        {
            get => _dungeon3HighestValue;
            set => _dungeon3HighestValue = value;
        }

        private ObscuredDouble _dpsDungeonHighestScore;

        public double DpsDungeonHighestScore
        {
            get => _dpsDungeonHighestScore;
            set => _dpsDungeonHighestScore = value;
        }

        public List<int> ReceivedDpsDungeonReward = new();

        private ObscuredDouble _dpsBossDungeonHighestScore;
        public double DpsBossDungeonHighestScore
        {
            get => _dpsBossDungeonHighestScore;
            set => _dpsBossDungeonHighestScore = value;
        }

        public List<int> ReceivedDpsBossDungeonReward = new();
        
        public int RetentionIndex;

        public int UseStatPoint
        {
            get => _useStatPoint;
            set
            {
                _useStatPoint = value;
                OnChangeUseStat?.OnNext(_useStatPoint);
            }
        }

        public int UseUnlimitedPoint
        {
            get => _useUnlimitedPoint;
            set
            {
                _useUnlimitedPoint = value;
                OnChangeUnlimitedPoint?.OnNext(_useUnlimitedPoint);
            }
        }

        private ObscuredByte _adSkip;
        public byte AdSkip
        {
            get => _adSkip;
            set => _adSkip = value;
        }

        public int SummonWeaponCount
        {
            get => _summonWeaponCount;
            set
            {
                _summonWeaponCount = value;
                SetSummonWeaponLv();
            }
        }

        public List<int> ReceivedSummonPetReward = new();

        public int SummonPetCount
        {
            get => _summonPetCount;
            set
            {
                _summonPetCount = value;
                SetSummonPetLv();
            }
        }

        public int ProgressGuideQuestId
        {
            get => _progressGuideQuestId;
            set
            {
                _progressGuideQuestId = value;
                OnChangeGuideQuestId?.OnNext(_progressGuideQuestId);
            }
        }

        public long ProgressGuideQuestValue
        {
            get => _progressGuideQuestValue;
            set
            {
                _progressGuideQuestValue = value;
                OnChangeGuideQuestProgressValue?.OnNext(_progressGuideQuestValue);
            }
        }

        public int EventAttendanceIndex
        {
            get => _eventAttendanceIndex;
            set => _eventAttendanceIndex = value;
        }

        private ObscuredInt _summonWeaponCount;
        private ObscuredInt _summonPetCount;
        private ObscuredInt _useStatPoint;
        private ObscuredInt _useUnlimitedPoint;
        private ObscuredInt _progressGuideQuestId;
        private ObscuredLong _progressGuideQuestValue;
        private ObscuredInt _eventAttendanceIndex;

        [JsonIgnore] public ObscuredInt SummonWeaponLv;
        [JsonIgnore] public ObscuredInt CurrentLevelSummonWeaponCount;
        [JsonIgnore] public ObscuredInt SummonPetLv;
        [JsonIgnore] public ObscuredInt CurrentLevelSummonPetCount;

        public DateTime ResetWeeklyDateTime;
        public DateTime ResetMonthlyDateTime;
        public DateTime GuildAttendanceTime;
        public DateTime GuildJoinCoolTime;
        public DateTime EventAttendanceTime;

        public void SetSummonWeaponLv()
        {
            int weaponSummonCount = SummonWeaponCount;

            SummonWeaponLv = 1;

            foreach (var chartData in ChartManager.WeaponSummonLevelCharts.Values)
            {
                if (weaponSummonCount < chartData.Exp)
                    break;

                if (chartData.Exp <= 0)
                    break;

                SummonWeaponLv++;
                weaponSummonCount -= chartData.Exp;
            }

            CurrentLevelSummonWeaponCount = weaponSummonCount;
        }

        public void SetSummonPetLv()
        {
            int petSummonCount = SummonPetCount;

            SummonPetLv = 1;

            foreach (var chartData in ChartManager.PetSummonLevelCharts.Values)
            {
                if (petSummonCount < chartData.Exp)
                    break;

                if (chartData.Exp <= 0)
                    break;

                SummonPetLv++;
                petSummonCount -= chartData.Exp;
            }

            CurrentLevelSummonPetCount = petSummonCount;
        }

        public int GetDungeonClearStep(int dungeonId)
        {
            switch (dungeonId)
            {
                case (int)DungeonType.Hwasengbang:
                    return Dungeon1ClearStep;
                case (int)DungeonType.MarinCamp:
                    return Dungeon2ClearStep;
                case (int)DungeonType.March:
                    return Dungeon3ClearStep;
                default:
                    return 0;
            }
        }

        public double GetDungeonHighestValue(int dungeonId)
        {
            switch (dungeonId)
            {
                case (int)DungeonType.Hwasengbang:
                    return Dungeon1HighestValue;
                case (int)DungeonType.MarinCamp:
                    return Dungeon2HighestValue;
                case (int)DungeonType.March:
                    return Dungeon3HighestValue;
                default:
                    return 0;
            }
        }

        public bool SetDungeonClearStep(int dungeonId, int clearStep)
        {
            switch (dungeonId)
            {
                case (int)DungeonType.Hwasengbang:
                {
                    if (clearStep <= Dungeon1ClearStep)
                        return false;

                    Dungeon1ClearStep = clearStep;
                    return true;
                }
                case (int)DungeonType.MarinCamp:
                {
                    if (clearStep <= Dungeon2ClearStep)
                        return false;

                    Dungeon2ClearStep = clearStep;
                    return true;
                }
                case (int)DungeonType.March:
                {
                    if (clearStep <= Dungeon3ClearStep)
                        return false;

                    Dungeon3ClearStep = clearStep;
                    return true;
                }
                default:
                    return false;
            }
        }

        public bool SetDungeonHighestValue(int dungeonId, double dungeonValue)
        {
            if (dungeonValue <= GetDungeonHighestValue(dungeonId))
                return false;

            switch (dungeonId)
            {
                case (int)DungeonType.Hwasengbang:
                    Dungeon1HighestValue = dungeonValue;
                    break;
                case (int)DungeonType.MarinCamp:
                    Dungeon2HighestValue = dungeonValue;
                    break;
                case (int)DungeonType.March:
                    Dungeon3HighestValue = dungeonValue;
                    break;
                default:
                    return false;
            }

            return true;
        }

        public bool IsAdSkip()
        {
            if (AdSkip == 0)
            {
                if (Managers.Game.ShopDatas.TryGetValue(6002, out int value))
                {
                    if (value != 0)
                    {
                        AdSkip = 1;
                        GameDataManager.UserGameData.SaveGameData();
                    }
                }
            }

            return AdSkip == 1;
        }

        [JsonIgnore] public readonly Subject<int> OnChangeLevel = new();
        [JsonIgnore] public readonly Subject<int> OnChangeUseStat = new();
        [JsonIgnore] public readonly Subject<int> OnChangeUnlimitedPoint = new();
        [JsonIgnore] public readonly Subject<int> OnChangeGuideQuestId = new();
        [JsonIgnore] public readonly Subject<long> OnChangeGuideQuestProgressValue = new();
    }

    public class UserGameData : BaseGameData
    {
        public override string TableName => "User";
        protected override string InDate { get; set; }

        protected override Param MakeInitData()
        {
            Param param = new Param()
            {
                { UserDataType.Level.ToString(), 1 },
                { UserDataType.LastConnectTime.ToString(), Utils.GetNow() },
                { UserDataType.ResetTime.ToString(), Utils.GetDay(1) },
                { UserDataType.CurrentStage.ToString(), 1 },
                { UserDataType.MaxReachStage.ToString(), 1 },
                { UserDataType.MaxReachStageTime.ToString(), Utils.GetNow() },
                { UserDataType.AttendanceTime.ToString(), new DateTime() },
                { UserDataType.AttendanceIndex.ToString(), 1 },
                { UserDataType.PromoGrade.ToString(), 0 },
                { UserDataType.ReceivedSummonWeaponReward.ToString(), new List<int>() },
                { UserDataType.SummonWeaponCount.ToString(), 0 },
                { UserDataType.ReceivedSummonPetReward.ToString(), new List<int>() },
                { UserDataType.SummonPetCount.ToString(), 0 },
                { UserDataType.Dungeon1ClearStep.ToString(), 0 },
                { UserDataType.Dungeon2ClearStep.ToString(), 0 },
                { UserDataType.Dungeon3ClearStep.ToString(), 0 },
                { UserDataType.Dungeon1HighestValue.ToString(), 0 },
                { UserDataType.Dungeon2HighestValue.ToString(), 0 },
                { UserDataType.Dungeon3HighestValue.ToString(), 0 },
                { UserDataType.DpsDungeonHighestScore.ToString(), 0 },
                { UserDataType.ReceivedDpsDungeonReward.ToString(), new List<int>() },
                { UserDataType.UseStatPoint.ToString(), 0 },
                { UserDataType.AdSkip.ToString(), 0 },
                { UserDataType.ProgressGuideQuestId.ToString(), 1 },
                { UserDataType.ProgressGuideQuestValue.ToString(), 0 },
                { UserDataType.RetentionIndex.ToString(), 0 },
                { UserDataType.UseUnlimitedPoint.ToString(), 0 },
                { UserDataType.ResetWeeklyDateTime.ToString(), Utils.GetNextWeeklyDate() },
                { UserDataType.ResetMonthlyDateTime.ToString(), Utils.GetNextMonthDate() },
                { UserDataType.GuildAttendanceTime.ToString(), new DateTime() },
                { UserDataType.GuildJoinCoolTime.ToString(), new DateTime() },
                { UserDataType.EventAttendanceIndex.ToString(), 1 },
                { UserDataType.EventAttendanceTime.ToString(), new DateTime() },
                { UserDataType.DpsBossDungeonHighestScore.ToString(), 0 },
                { UserDataType.ReceivedDpsBossDungeonReward.ToString(), new List<int>() }
            };

            Managers.Game.AdBuffDurationTimes.TryAdd(1, new FloatReactiveProperty(7200));
            Managers.Game.AdBuffDurationTimes.TryAdd(2, new FloatReactiveProperty(7200));

            PlayerPrefs.SetString("AdBuffTime", JsonConvert.SerializeObject(Managers.Game.AdBuffDurationTimes));

            return param;
        }

        protected override Param MakeSaveData()
        {
            Param param = new Param()
            {
                { UserDataType.Level.ToString(), Managers.Game.UserData.Level },
                { UserDataType.LastConnectTime.ToString(), Managers.Game.UserData.LastConnectTime },
                { UserDataType.ResetTime.ToString(), Managers.Game.UserData.ResetTime },
                { UserDataType.CurrentStage.ToString(), Managers.Game.UserData.CurrentStage },
                { UserDataType.MaxReachStage.ToString(), Managers.Game.UserData.MaxReachStage.GetDecrypted() },
                { UserDataType.MaxReachStageTime.ToString(), Managers.Game.UserData.MaxReachStageTime },
                { UserDataType.AttendanceTime.ToString(), Managers.Game.UserData.AttendanceDate },
                { UserDataType.AttendanceIndex.ToString(), Managers.Game.UserData.AttendanceIndex },
                { UserDataType.PromoGrade.ToString(), Managers.Game.UserData.PromoGrade },
                {
                    UserDataType.ReceivedSummonWeaponReward.ToString(),
                    Managers.Game.UserData.ReceivedSummonWeaponReward
                },
                { UserDataType.SummonWeaponCount.ToString(), Managers.Game.UserData.SummonWeaponCount },
                { UserDataType.ReceivedSummonPetReward.ToString(), Managers.Game.UserData.ReceivedSummonPetReward },
                { UserDataType.SummonPetCount.ToString(), Managers.Game.UserData.SummonPetCount },
                { UserDataType.Dungeon1ClearStep.ToString(), Managers.Game.UserData.Dungeon1ClearStep },
                { UserDataType.Dungeon2ClearStep.ToString(), Managers.Game.UserData.Dungeon2ClearStep },
                { UserDataType.Dungeon3ClearStep.ToString(), Managers.Game.UserData.Dungeon3ClearStep },
                { UserDataType.Dungeon1HighestValue.ToString(), Managers.Game.UserData.Dungeon1HighestValue },
                { UserDataType.Dungeon2HighestValue.ToString(), Managers.Game.UserData.Dungeon2HighestValue },
                { UserDataType.Dungeon3HighestValue.ToString(), Managers.Game.UserData.Dungeon3HighestValue },
                { UserDataType.DpsDungeonHighestScore.ToString(), Managers.Game.UserData.DpsDungeonHighestScore },
                { UserDataType.ReceivedDpsDungeonReward.ToString(), Managers.Game.UserData.ReceivedDpsDungeonReward },
                { UserDataType.UseStatPoint.ToString(), Managers.Game.UserData.UseStatPoint },
                { UserDataType.AdSkip.ToString(), Managers.Game.UserData.AdSkip },
                { UserDataType.ProgressGuideQuestId.ToString(), Managers.Game.UserData.ProgressGuideQuestId },
                { UserDataType.ProgressGuideQuestValue.ToString(), Managers.Game.UserData.ProgressGuideQuestValue },
                { UserDataType.RetentionIndex.ToString(), Managers.Game.UserData.RetentionIndex },
                { UserDataType.UseUnlimitedPoint.ToString(), Managers.Game.UserData.UseUnlimitedPoint },
                { UserDataType.ResetWeeklyDateTime.ToString(), Managers.Game.UserData.ResetWeeklyDateTime },
                { UserDataType.ResetMonthlyDateTime.ToString(), Managers.Game.UserData.ResetMonthlyDateTime },
                { UserDataType.GuildAttendanceTime.ToString(), Managers.Game.UserData.GuildAttendanceTime },
                { UserDataType.GuildJoinCoolTime.ToString(), Managers.Game.UserData.GuildJoinCoolTime },
                { UserDataType.EventAttendanceIndex.ToString(), Managers.Game.UserData.EventAttendanceIndex },
                { UserDataType.EventAttendanceTime.ToString(), Managers.Game.UserData.EventAttendanceTime },
                { UserDataType.DpsBossDungeonHighestScore.ToString(), Managers.Game.UserData.DpsBossDungeonHighestScore },
                { UserDataType.ReceivedDpsBossDungeonReward.ToString(), Managers.Game.UserData.ReceivedDpsBossDungeonReward }
                
            };

            return param;
        }

        protected override Param MakeSaveData(int dataType)
        {
            Param param = new Param();

            var userDataType = (UserDataType)dataType;

            switch (userDataType)
            {
                case UserDataType.Level:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.Level);
                    break;
                case UserDataType.LastConnectTime:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.LastConnectTime);
                    break;
                case UserDataType.ResetTime:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.ResetTime);
                    break;
                case UserDataType.MaxReachStage:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.MaxReachStage.GetDecrypted());
                    break;
                case UserDataType.MaxReachStageTime:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.MaxReachStageTime);
                    break;
                case UserDataType.CurrentStage:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.CurrentStage);
                    break;
                case UserDataType.AttendanceTime:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.AttendanceDate);
                    break;
                case UserDataType.AttendanceIndex:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.AttendanceIndex);
                    break;
                case UserDataType.PromoGrade:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.PromoGrade);
                    break;
                case UserDataType.ReceivedSummonWeaponReward:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.ReceivedSummonWeaponReward);
                    break;
                case UserDataType.SummonWeaponCount:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.SummonWeaponCount);
                    break;
                case UserDataType.ReceivedSummonPetReward:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.ReceivedSummonPetReward);
                    break;
                case UserDataType.SummonPetCount:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.SummonPetCount);
                    break;
                case UserDataType.Dungeon1ClearStep:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon1ClearStep);
                    break;
                case UserDataType.Dungeon2ClearStep:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon2ClearStep);
                    break;
                case UserDataType.Dungeon3ClearStep:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon3ClearStep);
                    break;
                case UserDataType.Dungeon1HighestValue:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon1HighestValue);
                    break;
                case UserDataType.Dungeon2HighestValue:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon2HighestValue);
                    break;
                case UserDataType.Dungeon3HighestValue:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon3HighestValue);
                    break;
                case UserDataType.DpsDungeonHighestScore:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.DpsDungeonHighestScore);
                    break;
                case UserDataType.ReceivedDpsDungeonReward:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.ReceivedDpsDungeonReward);
                    break;
                case UserDataType.UseStatPoint:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.UseStatPoint);
                    break;
                case UserDataType.AdSkip:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.AdSkip);
                    break;
                case UserDataType.ProgressGuideQuestId:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.ProgressGuideQuestId);
                    break;
                case UserDataType.ProgressGuideQuestValue:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.ProgressGuideQuestValue);
                    break;
                case UserDataType.RetentionIndex:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.RetentionIndex);
                    break;
                case UserDataType.UseUnlimitedPoint:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.UseUnlimitedPoint);
                    break;
                case UserDataType.ResetWeeklyDateTime:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.ResetWeeklyDateTime);
                    break;
                case UserDataType.ResetMonthlyDateTime:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.ResetMonthlyDateTime);
                    break;
                case UserDataType.GuildAttendanceTime:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.GuildAttendanceTime);
                    break;
                case UserDataType.GuildJoinCoolTime:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.GuildJoinCoolTime);
                    break;
                case UserDataType.EventAttendanceIndex:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.EventAttendanceIndex);
                    break;
                case UserDataType.EventAttendanceTime:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.EventAttendanceTime);
                    break;
                case UserDataType.DpsBossDungeonHighestScore:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.DpsBossDungeonHighestScore);
                    break;
                case UserDataType.ReceivedDpsBossDungeonReward:
                    param.Add(userDataType.ToString(), Managers.Game.UserData.ReceivedDpsBossDungeonReward);
                    break;
            }

            return param;
        }

        protected override Param MakeSaveData(List<int> dataTypes)
        {
            Param param = new Param();

            dataTypes.ForEach(dataType =>
            {
                var userDataType = (UserDataType)dataType;

                if (param.Contains(userDataType.ToString()))
                    return;

                switch (userDataType)
                {
                    case UserDataType.Level:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.Level);
                        break;
                    case UserDataType.LastConnectTime:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.LastConnectTime);
                        break;
                    case UserDataType.ResetTime:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.ResetTime);
                        break;
                    case UserDataType.MaxReachStage:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.MaxReachStage.GetDecrypted());
                        break;
                    case UserDataType.MaxReachStageTime:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.MaxReachStageTime);
                        break;
                    case UserDataType.CurrentStage:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.CurrentStage);
                        break;
                    case UserDataType.AttendanceTime:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.AttendanceDate);
                        break;
                    case UserDataType.AttendanceIndex:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.AttendanceIndex);
                        break;
                    case UserDataType.PromoGrade:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.PromoGrade);
                        break;
                    case UserDataType.ReceivedSummonWeaponReward:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.ReceivedSummonWeaponReward);
                        break;
                    case UserDataType.SummonWeaponCount:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.SummonWeaponCount);
                        break;
                    case UserDataType.ReceivedSummonPetReward:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.ReceivedSummonPetReward);
                        break;
                    case UserDataType.SummonPetCount:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.SummonPetCount);
                        break;
                    case UserDataType.Dungeon1ClearStep:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon1ClearStep);
                        break;
                    case UserDataType.Dungeon2ClearStep:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon2ClearStep);
                        break;
                    case UserDataType.Dungeon3ClearStep:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon3ClearStep);
                        break;
                    case UserDataType.Dungeon1HighestValue:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon1HighestValue);
                        break;
                    case UserDataType.Dungeon2HighestValue:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon2HighestValue);
                        break;
                    case UserDataType.Dungeon3HighestValue:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.Dungeon3HighestValue);
                        break;
                    case UserDataType.DpsDungeonHighestScore:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.DpsDungeonHighestScore);
                        break;
                    case UserDataType.ReceivedDpsDungeonReward:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.ReceivedDpsDungeonReward);
                        break;
                    case UserDataType.UseStatPoint:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.UseStatPoint);
                        break;
                    case UserDataType.AdSkip:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.AdSkip);
                        break;
                    case UserDataType.ProgressGuideQuestId:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.ProgressGuideQuestId);
                        break;
                    case UserDataType.ProgressGuideQuestValue:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.ProgressGuideQuestValue);
                        break;
                    case UserDataType.RetentionIndex:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.RetentionIndex);
                        break;
                    case UserDataType.UseUnlimitedPoint:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.UseUnlimitedPoint);
                        break;
                    case UserDataType.ResetWeeklyDateTime:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.ResetWeeklyDateTime);
                        break;
                    case UserDataType.ResetMonthlyDateTime:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.ResetMonthlyDateTime);
                        break;
                    case UserDataType.GuildAttendanceTime:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.GuildAttendanceTime);
                        break;
                    case UserDataType.GuildJoinCoolTime:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.GuildJoinCoolTime);
                        break;
                    case UserDataType.EventAttendanceIndex:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.EventAttendanceIndex);
                        break;
                    case UserDataType.EventAttendanceTime:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.EventAttendanceTime);
                        break;
                    case UserDataType.DpsBossDungeonHighestScore:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.DpsBossDungeonHighestScore);
                        break;
                    case UserDataType.ReceivedDpsBossDungeonReward:
                        param.Add(userDataType.ToString(), Managers.Game.UserData.ReceivedDpsBossDungeonReward);
                        break;
                }
            });


            return param;
        }

        protected override void SetGameData(JsonData jsonData)
        {
            Param param = new Param();

            // Level
            if (jsonData.ContainsKey(UserDataType.Level.ToString()))
                Managers.Game.UserData.Level = int.Parse(jsonData[UserDataType.Level.ToString()].ToString());
            else
            {
                Managers.Game.UserData.Level = 1;
                param.Add(UserDataType.Level.ToString(), Managers.Game.UserData.Level);
            }

            // LastConnectTime
            if (jsonData.ContainsKey(UserDataType.LastConnectTime.ToString()))
            {
                DateTime.TryParse(jsonData[UserDataType.LastConnectTime.ToString()].ToString(),
                    out Managers.Game.UserData.LastConnectTime);
            }
            else
            {
                Managers.Game.UserData.LastConnectTime = Utils.GetNow();
                param.Add(UserDataType.LastConnectTime.ToString(), Managers.Game.UserData.LastConnectTime);
            }

            // ResetTime
            if (jsonData.ContainsKey(UserDataType.ResetTime.ToString()))
            {
                DateTime.TryParse(jsonData[UserDataType.ResetTime.ToString()].ToString(),
                    out Managers.Game.UserData.ResetTime);
            }
            else
            {
                Managers.Game.UserData.ResetTime = Utils.GetDay();
                param.Add(UserDataType.ResetTime.ToString(), Managers.Game.UserData.ResetTime);
            }

            // CurrentStage
            if (jsonData.ContainsKey(UserDataType.CurrentStage.ToString()))
            {
                int.TryParse(jsonData[UserDataType.CurrentStage.ToString()].ToString(), out int currentStage);
                Managers.Game.UserData.CurrentStage = currentStage;
            }
            else
            {
                Managers.Game.UserData.CurrentStage = 1;
                param.Add(UserDataType.CurrentStage.ToString(), Managers.Game.UserData.CurrentStage);
            }

            // MaxReachStage
            if (jsonData.ContainsKey(UserDataType.MaxReachStage.ToString()))
            {
                int.TryParse(jsonData[UserDataType.MaxReachStage.ToString()].ToString(), out int maxReachStage);
                Managers.Game.UserData.MaxReachStage = maxReachStage;
            }
            else
            {
                Managers.Game.UserData.MaxReachStage = 1;
                param.Add(UserDataType.MaxReachStage.ToString(), (int)Managers.Game.UserData.MaxReachStage);
            }

            // MaxReachStageTime
            if (jsonData.ContainsKey(UserDataType.MaxReachStageTime.ToString()))
            {
                DateTime.TryParse(jsonData[UserDataType.MaxReachStageTime.ToString()].ToString(),
                    out Managers.Game.UserData.MaxReachStageTime);
            }
            else
            {
                Managers.Game.UserData.MaxReachStageTime = Utils.GetNow();
                param.Add(UserDataType.MaxReachStageTime.ToString(), Managers.Game.UserData.MaxReachStageTime);
            }

            // AttendanceTime
            if (jsonData.ContainsKey(UserDataType.AttendanceTime.ToString()))
            {
                DateTime.TryParse(jsonData[UserDataType.AttendanceTime.ToString()].ToString(),
                    out Managers.Game.UserData.AttendanceDate);
            }
            else
            {
                Managers.Game.UserData.AttendanceDate = new DateTime();
                param.Add(UserDataType.AttendanceTime.ToString(), Managers.Game.UserData.AttendanceDate);
            }

            // AttendanceIndex
            if (jsonData.ContainsKey(UserDataType.AttendanceIndex.ToString()))
            {
                int.TryParse(jsonData[UserDataType.AttendanceIndex.ToString()].ToString(),
                    out int attendanceIndex);
                Managers.Game.UserData.AttendanceIndex = attendanceIndex;
            }
            else
            {
                Managers.Game.UserData.AttendanceIndex = 1;
                param.Add(UserDataType.AttendanceIndex.ToString(), Managers.Game.UserData.AttendanceIndex);
            }

            // PromoGrade
            if (jsonData.ContainsKey(UserDataType.PromoGrade.ToString()))
            {
                int.TryParse(jsonData[UserDataType.PromoGrade.ToString()].ToString(),
                    out var promoGrade);
                Managers.Game.UserData.PromoGrade = promoGrade;
            }
            else
            {
                Managers.Game.UserData.PromoGrade = 0;
                param.Add(UserDataType.PromoGrade.ToString(), (int)Managers.Game.UserData.PromoGrade);
            }

            //ReceivedSummonWeaponReward
            if (jsonData.ContainsKey(UserDataType.ReceivedSummonWeaponReward.ToString()))
            {
                Managers.Game.UserData.ReceivedSummonWeaponReward =
                    JsonMapper.ToObject<List<int>>(
                        jsonData[UserDataType.ReceivedSummonWeaponReward.ToString()].ToJson());
            }
            else
            {
                param.Add(UserDataType.ReceivedSummonWeaponReward.ToString(),
                    Managers.Game.UserData.ReceivedSummonWeaponReward);
            }

            // SummonWeaponCount
            if (jsonData.ContainsKey(UserDataType.SummonWeaponCount.ToString()))
            {
                int.TryParse(jsonData[UserDataType.SummonWeaponCount.ToString()].ToString(),
                    out int summonWeaponCount);
                Managers.Game.UserData.SummonWeaponCount = summonWeaponCount;
            }
            else
            {
                Managers.Game.UserData.SummonWeaponCount = 0;
                param.Add(UserDataType.SummonWeaponCount.ToString(), Managers.Game.UserData.SummonWeaponCount);
            }

            // ReceivedSummonPetReward
            if (jsonData.ContainsKey(UserDataType.ReceivedSummonPetReward.ToString()))
            {
                Managers.Game.UserData.ReceivedSummonPetReward =
                    JsonMapper.ToObject<List<int>>(jsonData[UserDataType.ReceivedSummonPetReward.ToString()].ToJson());
            }
            else
            {
                param.Add(UserDataType.ReceivedSummonPetReward.ToString(),
                    Managers.Game.UserData.ReceivedSummonPetReward);
            }

            // SummonPetCount
            if (jsonData.ContainsKey(UserDataType.SummonPetCount.ToString()))
            {
                int.TryParse(jsonData[UserDataType.SummonPetCount.ToString()].ToString(),
                    out int summonPetCount);
                Managers.Game.UserData.SummonPetCount = summonPetCount;
            }
            else
            {
                Managers.Game.UserData.SummonPetCount = 0;
                param.Add(UserDataType.SummonPetCount.ToString(), Managers.Game.UserData.SummonPetCount);
            }

            // Dungeon1ClearStep
            if (jsonData.ContainsKey(UserDataType.Dungeon1ClearStep.ToString()))
            {
                int.TryParse(jsonData[UserDataType.Dungeon1ClearStep.ToString()].ToString(),
                    out int dungeon1ClearStep);
                Managers.Game.UserData.Dungeon1ClearStep = dungeon1ClearStep;
            }
            else
            {
                Managers.Game.UserData.Dungeon1ClearStep = 0;
                param.Add(UserDataType.Dungeon1ClearStep.ToString(), Managers.Game.UserData.Dungeon1ClearStep);
            }

            // Dungeon2ClearStep
            if (jsonData.ContainsKey(UserDataType.Dungeon2ClearStep.ToString()))
            {
                int.TryParse(jsonData[UserDataType.Dungeon2ClearStep.ToString()].ToString(),
                    out int dungeon2ClearStep);
                Managers.Game.UserData.Dungeon2ClearStep = dungeon2ClearStep;
            }
            else
            {
                Managers.Game.UserData.Dungeon2ClearStep = 0;
                param.Add(UserDataType.Dungeon2ClearStep.ToString(), Managers.Game.UserData.Dungeon2ClearStep);
            }

            // Dungeon3ClearStep
            if (jsonData.ContainsKey(UserDataType.Dungeon3ClearStep.ToString()))
            {
                int.TryParse(jsonData[UserDataType.Dungeon3ClearStep.ToString()].ToString(),
                    out int dungeon3ClearStep);
                Managers.Game.UserData.Dungeon3ClearStep = dungeon3ClearStep;
            }
            else
            {
                Managers.Game.UserData.Dungeon3ClearStep = 0;
                param.Add(UserDataType.Dungeon3ClearStep.ToString(), Managers.Game.UserData.Dungeon3ClearStep);
            }

            // Dungeon1HighestValue
            if (jsonData.ContainsKey(UserDataType.Dungeon1HighestValue.ToString()))
            {
                double.TryParse(jsonData[UserDataType.Dungeon1HighestValue.ToString()].ToString(),
                    out double dungeon1HighestValue);
                Managers.Game.UserData.Dungeon1HighestValue = dungeon1HighestValue;
            }
            else
            {
                Managers.Game.UserData.Dungeon1HighestValue = 0;
                param.Add(UserDataType.Dungeon1HighestValue.ToString(), Managers.Game.UserData.Dungeon1HighestValue);
            }

            // Dungeon2HighestValue
            if (jsonData.ContainsKey(UserDataType.Dungeon2HighestValue.ToString()))
            {
                double.TryParse(jsonData[UserDataType.Dungeon2HighestValue.ToString()].ToString(),
                    out double dungeon2HighestValue);
                Managers.Game.UserData.Dungeon2HighestValue = dungeon2HighestValue;
            }
            else
            {
                Managers.Game.UserData.Dungeon2HighestValue = 0;
                param.Add(UserDataType.Dungeon2HighestValue.ToString(), Managers.Game.UserData.Dungeon2HighestValue);
            }

            // Dungeon3HighestValue
            if (jsonData.ContainsKey(UserDataType.Dungeon3HighestValue.ToString()))
            {
                double.TryParse(jsonData[UserDataType.Dungeon3HighestValue.ToString()].ToString(),
                    out double dungeon3HighestValue);
                Managers.Game.UserData.Dungeon3HighestValue = dungeon3HighestValue;
            }
            else
            {
                Managers.Game.UserData.Dungeon3HighestValue = 0;
                param.Add(UserDataType.Dungeon3HighestValue.ToString(), Managers.Game.UserData.Dungeon3HighestValue);
            }

            // DpsDungeonHighestScore
            if (jsonData.ContainsKey(UserDataType.DpsDungeonHighestScore.ToString()))
            {
                double.TryParse(jsonData[UserDataType.DpsDungeonHighestScore.ToString()].ToString(),
                    out double dpsDungeonHighestScore);
                Managers.Game.UserData.DpsDungeonHighestScore = dpsDungeonHighestScore;
            }
            else
            {
                Managers.Game.UserData.DpsDungeonHighestScore = 0;
                param.Add(UserDataType.DpsDungeonHighestScore.ToString(),
                    Managers.Game.UserData.DpsDungeonHighestScore);
            }

            // ReceivedDpsDungeonReward
            if (jsonData.ContainsKey(UserDataType.ReceivedDpsDungeonReward.ToString()))
            {
                Managers.Game.UserData.ReceivedDpsDungeonReward =
                    JsonMapper.ToObject<List<int>>(jsonData[UserDataType.ReceivedDpsDungeonReward.ToString()].ToJson());
            }
            else
            {
                param.Add(UserDataType.ReceivedSummonPetReward.ToString(),
                    Managers.Game.UserData.ReceivedDpsDungeonReward);
            }

            // UseStatPoint
            if (jsonData.ContainsKey(UserDataType.UseStatPoint.ToString()))
            {
                int.TryParse(jsonData[UserDataType.UseStatPoint.ToString()].ToString(),
                    out var useStatPoint);
                Managers.Game.UserData.UseStatPoint = useStatPoint;
            }
            else
            {
                Managers.Game.UserData.UseStatPoint = 0;
                param.Add(UserDataType.UseStatPoint.ToString(), Managers.Game.UserData.UseStatPoint);
            }

            // AdSkip
            if (jsonData.ContainsKey(UserDataType.AdSkip.ToString()))
            {
                byte.TryParse(jsonData[UserDataType.AdSkip.ToString()].ToString(),
                    out byte adSkip);
                Managers.Game.UserData.AdSkip = adSkip;
            }
            else
            {
                Managers.Game.UserData.AdSkip = 0;
                param.Add(UserDataType.AdSkip.ToString(), Managers.Game.UserData.AdSkip);
            }

            // ProgressGuideQuestId
            if (jsonData.ContainsKey(UserDataType.ProgressGuideQuestId.ToString()))
            {
                int.TryParse(jsonData[UserDataType.ProgressGuideQuestId.ToString()].ToString(),
                    out var progressGuideQuestId);
                Managers.Game.UserData.ProgressGuideQuestId = progressGuideQuestId;
            }
            else
            {
                Managers.Game.UserData.ProgressGuideQuestId = 1;
                param.Add(UserDataType.ProgressGuideQuestId.ToString(), Managers.Game.UserData.ProgressGuideQuestId);
            }

            // ProgressGuideQuestValue
            if (jsonData.ContainsKey(UserDataType.ProgressGuideQuestValue.ToString()))
            {
                int.TryParse(jsonData[UserDataType.ProgressGuideQuestValue.ToString()].ToString(),
                    out var progressGuideQuestValue);
                Managers.Game.UserData.ProgressGuideQuestValue = progressGuideQuestValue;
            }
            else
            {
                Managers.Game.UserData.ProgressGuideQuestValue = 0;
                param.Add(UserDataType.ProgressGuideQuestValue.ToString(),
                    Managers.Game.UserData.ProgressGuideQuestValue);
            }

            // RetentionIndex
            if (jsonData.ContainsKey(UserDataType.RetentionIndex.ToString()))
            {
                int.TryParse(jsonData[UserDataType.RetentionIndex.ToString()].ToString(),
                    out Managers.Game.UserData.RetentionIndex);
            }
            else
            {
                Managers.Game.UserData.RetentionIndex = 0;
                param.Add(UserDataType.RetentionIndex.ToString(), Managers.Game.UserData.RetentionIndex);
            }

            // UseUnlimitedPoint
            if (jsonData.ContainsKey(UserDataType.UseUnlimitedPoint.ToString()))
            {
                int.TryParse(jsonData[UserDataType.UseUnlimitedPoint.ToString()].ToString(), out int useUnlimitedPoint);
                Managers.Game.UserData.UseUnlimitedPoint = useUnlimitedPoint;
            }
            else
            {
                Managers.Game.UserData.UseUnlimitedPoint = 0;
                param.Add(UserDataType.UseUnlimitedPoint.ToString(), Managers.Game.UserData.UseUnlimitedPoint);
            }

            // ResetWeeklyDateTime
            if (jsonData.ContainsKey(UserDataType.ResetWeeklyDateTime.ToString()))
            {
                DateTime.TryParse(jsonData[UserDataType.ResetWeeklyDateTime.ToString()].ToString(),
                    out var resetWeeklyDateTime);
                Managers.Game.UserData.ResetWeeklyDateTime = resetWeeklyDateTime;
            }
            else
            {
                Managers.Game.UserData.ResetWeeklyDateTime = Utils.GetNextWeeklyDate();
                param.Add(UserDataType.ResetWeeklyDateTime.ToString(), Managers.Game.UserData.ResetWeeklyDateTime);
            }

            // ResetMonthlyDateTime
            if (jsonData.ContainsKey(UserDataType.ResetMonthlyDateTime.ToString()))
            {
                DateTime.TryParse(jsonData[UserDataType.ResetMonthlyDateTime.ToString()].ToString(),
                    out var resetMonthlyDateTime);
                Managers.Game.UserData.ResetMonthlyDateTime = resetMonthlyDateTime;
            }
            else
            {
                Managers.Game.UserData.ResetMonthlyDateTime = Utils.GetNextMonthDate();
                param.Add(UserDataType.ResetMonthlyDateTime.ToString(), Managers.Game.UserData.ResetMonthlyDateTime);
            }

            // GuildAttendanceTime
            if (jsonData.ContainsKey(UserDataType.GuildAttendanceTime.ToString()))
            {
                DateTime.TryParse(jsonData[UserDataType.GuildAttendanceTime.ToString()].ToString(),
                    out var guildAttendanceTime);
                Managers.Game.UserData.GuildAttendanceTime = guildAttendanceTime;
            }
            else
            {
                Managers.Game.UserData.GuildAttendanceTime = new DateTime();
                param.Add(UserDataType.GuildAttendanceTime.ToString(), Managers.Game.UserData.GuildAttendanceTime);
            }
            
            // GuildJoinCoolTime
            if (jsonData.ContainsKey(UserDataType.GuildJoinCoolTime.ToString()))
            {
                DateTime.TryParse(jsonData[UserDataType.GuildJoinCoolTime.ToString()].ToString(),
                    out var guildJoinCoolTime);
                Managers.Game.UserData.GuildJoinCoolTime = guildJoinCoolTime;
            }
            else
            {
                Managers.Game.UserData.GuildJoinCoolTime = new DateTime();
                param.Add(UserDataType.GuildJoinCoolTime.ToString(), Managers.Game.UserData.GuildJoinCoolTime);
            }
            
            // SpecialAttendanceIndex
            if (jsonData.ContainsKey(UserDataType.EventAttendanceIndex.ToString()))
            {
                int.TryParse(jsonData[UserDataType.EventAttendanceIndex.ToString()].ToString(), out var specialAttendanceIndex);
                Managers.Game.UserData.EventAttendanceIndex = specialAttendanceIndex;
            }
            else
            {
                Managers.Game.UserData.EventAttendanceIndex = 1;
                param.Add(UserDataType.EventAttendanceIndex.ToString(), Managers.Game.UserData.EventAttendanceIndex);
            }
            
            // SpecialAttendanceTime
            if (jsonData.ContainsKey(UserDataType.EventAttendanceTime.ToString()))
            {
                DateTime.TryParse(jsonData[UserDataType.EventAttendanceTime.ToString()].ToString(),
                    out var specialAttendanceTime);
                Managers.Game.UserData.EventAttendanceTime = specialAttendanceTime;
            }
            else
            {
                Managers.Game.UserData.EventAttendanceTime = new DateTime();
                param.Add(UserDataType.EventAttendanceTime.ToString(), Managers.Game.UserData.EventAttendanceTime);
            }
            
            // DpsBossDungeonHighestScore
            if (jsonData.ContainsKey(UserDataType.DpsBossDungeonHighestScore.ToString()))
            {
                double.TryParse(jsonData[UserDataType.DpsBossDungeonHighestScore.ToString()].ToString(),
                    out var dpsBossDungeonHighestScore);
                Managers.Game.UserData.DpsBossDungeonHighestScore = dpsBossDungeonHighestScore;
            }
            else
            {
                Managers.Game.UserData.DpsBossDungeonHighestScore = 0;
                param.Add(UserDataType.DpsBossDungeonHighestScore.ToString(), Managers.Game.UserData.DpsBossDungeonHighestScore);
            }
            
            // ReceivedDpsBossDungeonReward
            if (jsonData.ContainsKey(UserDataType.ReceivedDpsBossDungeonReward.ToString()))
            {
                Managers.Game.UserData.ReceivedDpsBossDungeonReward =
                    JsonMapper.ToObject<List<int>>(jsonData[UserDataType.ReceivedDpsBossDungeonReward.ToString()].ToJson());
            }
            else
            {
                Managers.Game.UserData.ReceivedDpsBossDungeonReward = new();
                param.Add(UserDataType.ReceivedDpsBossDungeonReward.ToString(), Managers.Game.UserData.ReceivedDpsBossDungeonReward);
            }

            // 출석 초기화 플래그 체크
            if (ChartManager.SystemCharts.TryGetValue(SystemData.AttendanceResetFlag, out var systemChart))
            {
                if (jsonData.ContainsKey(UserDataType.AttendanceResetFlag.ToString()))
                {
                    if (int.TryParse(jsonData[UserDataType.AttendanceResetFlag.ToString()].ToString(),
                            out var attendanceResetFlag))
                    {
                        if (attendanceResetFlag < systemChart.Value)
                        {
                            // 리셋
                            Managers.Game.UserData.AttendanceDate = new DateTime();

                            if (param.ContainsKey(UserDataType.AttendanceTime.ToString()))
                                param.Remove(UserDataType.AttendanceTime.ToString());
                            param.Add(UserDataType.AttendanceTime.ToString(), Managers.Game.UserData.AttendanceDate);

                            Managers.Game.UserData.AttendanceIndex = 1;

                            if (param.ContainsKey(UserDataType.AttendanceIndex.ToString()))
                                param.Remove(UserDataType.AttendanceIndex.ToString());
                            param.Add(UserDataType.AttendanceIndex.ToString(), Managers.Game.UserData.AttendanceIndex);

                            if (param.ContainsKey(UserDataType.AttendanceResetFlag.ToString()))
                                param.Remove(UserDataType.AttendanceResetFlag.ToString());
                            param.Add(UserDataType.AttendanceResetFlag.ToString(), systemChart.Value.GetDecrypted());
                        }
                    }
                }
                else
                {
                    // 리셋
                    Managers.Game.UserData.AttendanceDate = new DateTime();

                    if (param.ContainsKey(UserDataType.AttendanceTime.ToString()))
                        param.Remove(UserDataType.AttendanceTime.ToString());
                    param.Add(UserDataType.AttendanceTime.ToString(), Managers.Game.UserData.AttendanceDate);

                    Managers.Game.UserData.AttendanceIndex = 1;

                    if (param.ContainsKey(UserDataType.AttendanceIndex.ToString()))
                        param.Remove(UserDataType.AttendanceIndex.ToString());
                    param.Add(UserDataType.AttendanceIndex.ToString(), Managers.Game.UserData.AttendanceIndex);

                    if (param.ContainsKey(UserDataType.AttendanceResetFlag.ToString()))
                        param.Remove(UserDataType.AttendanceResetFlag.ToString());
                    param.Add(UserDataType.AttendanceResetFlag.ToString(), systemChart.Value.GetDecrypted());
                }
            }
            
            // 이벤트 출석 초기화 플래그 체크
            if (ChartManager.SystemCharts.TryGetValue(SystemData.EventAttendanceResetFlag, out systemChart))
            {
                if (jsonData.ContainsKey(UserDataType.EventAttendanceResetFlag.ToString()))
                {
                    if (int.TryParse(jsonData[UserDataType.EventAttendanceResetFlag.ToString()].ToString(),
                            out var attendanceResetFlag))
                    {
                        if (attendanceResetFlag < systemChart.Value)
                        {
                            // 리셋
                            Managers.Game.UserData.EventAttendanceTime = new DateTime();

                            if (param.ContainsKey(UserDataType.EventAttendanceTime.ToString()))
                                param.Remove(UserDataType.EventAttendanceTime.ToString());
                            param.Add(UserDataType.EventAttendanceTime.ToString(), Managers.Game.UserData.EventAttendanceTime);

                            Managers.Game.UserData.EventAttendanceIndex = 1;

                            if (param.ContainsKey(UserDataType.EventAttendanceIndex.ToString()))
                                param.Remove(UserDataType.EventAttendanceIndex.ToString());
                            param.Add(UserDataType.EventAttendanceIndex.ToString(), Managers.Game.UserData.EventAttendanceIndex);

                            if (param.ContainsKey(UserDataType.EventAttendanceResetFlag.ToString()))
                                param.Remove(UserDataType.EventAttendanceResetFlag.ToString());
                            param.Add(UserDataType.EventAttendanceResetFlag.ToString(), systemChart.Value.GetDecrypted());
                        }
                    }
                }
                else
                {
                    // 리셋
                    Managers.Game.UserData.EventAttendanceTime = new DateTime();

                    if (param.ContainsKey(UserDataType.EventAttendanceTime.ToString()))
                        param.Remove(UserDataType.EventAttendanceTime.ToString());
                    param.Add(UserDataType.EventAttendanceTime.ToString(), Managers.Game.UserData.EventAttendanceTime);

                    Managers.Game.UserData.EventAttendanceIndex = 1;

                    if (param.ContainsKey(UserDataType.EventAttendanceIndex.ToString()))
                        param.Remove(UserDataType.EventAttendanceIndex.ToString());
                    param.Add(UserDataType.EventAttendanceIndex.ToString(), Managers.Game.UserData.EventAttendanceIndex);

                    if (param.ContainsKey(UserDataType.EventAttendanceResetFlag.ToString()))
                        param.Remove(UserDataType.EventAttendanceResetFlag.ToString());
                    param.Add(UserDataType.EventAttendanceResetFlag.ToString(), systemChart.Value.GetDecrypted());
                }
            }

            if (param.Count > 0)
                SaveGameData(param);
        }
    }
}