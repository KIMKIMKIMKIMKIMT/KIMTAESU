using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.ObscuredTypes;
using Firebase.Analytics;
using GameData;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils
{
    public static bool IsBuyingProcess;

    const string Zero = "0";

    public static Vector2 MinPos = new(-12f, -15f);
    public static Vector2 MaxPos = new(12f, 10f);

    private static readonly string[] CurrencyUnits_KR =
    {
        "", "만", "억", "조", "경", "해", "자", "양", "구", "간", "정", "재", "극", "항하사", "아승기", "나유타", "불가사의", "무량대수",
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R",
        "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI",
        "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY",
        "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO",
        "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA", "CB", "CC", "CD", "CE",
        "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU",
        "CV", "CW", "CX",
    };

    public static readonly Dictionary<int, Color> DungeonColor = new()
    {
        { 1, new Color(170 / 255f, 99 / 255f, 184 / 255f) },
        { 2, new Color(193 / 255f, 74 / 255f, 48 / 255f) },
        { 3, new Color(110 / 255f, 176 / 255f, 104 / 255f) },
    };

    public static readonly Dictionary<int, Color> ItemColor = new()
    {
        { 1, new Color(255 / 255f, 228 / 255f, 42 / 255f) },
        { 2, new Color(0.27f, 0.56f, 0.98f) },
        { 3, new Color(0.52f, 0.52f, 0.46f) },
        { 4, new Color(156 / 255f, 214 / 255f, 138 / 255f) },
        { 5, new Color(231 / 255f, 151 / 255f, 25 / 255f) }
    };

    public static readonly Dictionary<int, Color> CriticalColor = new()
    {
        { 0, new Color(1, 1, 1, 1) },
        { 1, new Color(1, 1, 1, 1) },
        { 2, new Color(0.3f, 0.29f, 0.29f) },
        { 4, new Color(0.44f, 0.35f, 0.28f) },
        { 8, new Color(0.59f, 0.59f, 0.59f) },
        { 16, new Color(0.72f, 0.72f, 0.72f) },
        { 32, new Color(0.76f, 0.71f, 0.58f) },
        { 64, new Color(0.76f, 0.71f, 0.58f) },
        { 128, new Color(0.65f, 0.8f, 0.84f) },
        { 256, new Color(0.88f, 0.86f, 0.94f) },
        { 512, new Color(157 / 255f, 102 / 255f, 102 / 255f) },
        { 1024, new Color(0.6f, 0.39f, 0.4f) },
        { 2048, new Color(0.9f, 0.87f, 0.69f) },
        { 4096, new Color(0.64f, 0.72f, 0.88f) },
        { 5009, new Color(0.21f, 0.47f, 0.85f) },
        { 6009, new Color(0.62f, 0.73f, 0.87f) },
        { 7009, new Color(0.98f, 0.49f, 0.39f) },
        { 8009, new Color(0.63f, 0.41f, 0.18f) },
        { 9009, new Color(0.16f, 0.14f, 0.19f) },
        { 10009, new Color(0.72f, 0.29f, 0.2f) },
        { 11009, new Color(0.92f, 0f, 0.62f) },
        { 12009, new Color(0.95f, 0.87f, 0.84f) },
        { 13009, new Color(0.96f, 0.56f, 0.07f) },
        { 14009, new Color(0.62f, 0.68f, 0.56f) },
    };

    static readonly string[] SI = new string[] { };

    public static string ToCurrencyString(this double number, CurrencyType currencyType = CurrencyType.Default)
    {
        {
            if (number == 0d)
                return Zero;

            number = Math.Round(number, 7);

            var n = number % 1;

            if (number < 1d && number > -1d)
            {
                string numString = number.ToString();
                string numFormatString = number.ToString("N4");

                return numString.Length < numFormatString.Length ? numString : numFormatString;
            }

            if (number < 10d && number > -10d)
            {
                string numString = number.ToString();
                string numFormatString = number.ToString("N3");

                return numString.Length < numFormatString.Length ? numString : numFormatString;
            }

            if (number < 100d && number > -100d)
            {
                string numString = number.ToString();
                string numFormatString = number.ToString("N2");

                return numString.Length < numFormatString.Length ? numString : numFormatString;
            }

            if (number < 1000d && number > -1000d)
            {
                string numString = number.ToString();
                string numFormatString = number.ToString("N1");

                return numString.Length < numFormatString.Length ? numString : numFormatString;
            }

            if (-10000d < number && number < 10000d)
                return number.ToString("N0");

            if (-100000d < number && number < 100000d)
                return number.ToString("N0");

            if (double.IsInfinity(number))
                return "Infinity";

            string showNumber;
            string[] partsSplit = number.ToString("E").Split('+');
            if (partsSplit.Length < 2)
            {
                Debug.LogWarning($"Failed - ToCurrencyString({number})");
                return Zero;
            }

            if (false == int.TryParse(partsSplit[1], out int exponent))
            {
                Debug.LogWarning(string.Format("Failed - ToCurrencyString({0}) : partsSplit[1] = {1}",
                    number, partsSplit[1]));
                return Zero;
            }

            int quotient = exponent / 4;
            int remainder = exponent % 4;
            string unitString = currencyType == CurrencyType.Default ? CurrencyUnits_KR[quotient] : SI[quotient];
            if (exponent < 4)
            {
                showNumber = number.ToString("F2");
                return $"{showNumber}{unitString}";
            }
            else
            {
                var temp = double.Parse(partsSplit[0].Replace("E", "")) * Math.Pow(10, remainder);

                string format = "F3";

                switch (remainder)
                {
                    case 0:
                        format = "F3";
                        break;
                    case 1:
                        format = "F2";
                        break;
                    case 2:
                        format = "F1";
                        break;
                    case 3:
                        format = "F0";
                        break;
                }

                showNumber = temp.ToString(format);
            }


            // 0 - 3, 1 - 2, 2 - 1, 3 - 

            return $"{showNumber}{unitString}"; // significant,
        }
    }

    public static string ToCurrencyString(this ObscuredDouble number, CurrencyType currencyType = CurrencyType.Default)
    {
        {
            if (number == 0d)
                return Zero;

            number = Math.Round(number, 7);

            var n = number % 1;

            if (number < 1d && number > -1d)
                return number.ToString("N4");

            if (number < 10d && number > -10d)
            {
                return number.ToString(n == 0 ? "N0" : "N3");
            }

            if (number < 100d && number > -100d)
            {
                return number.ToString(n == 0 ? "N0" : "N2");
            }

            if (number < 1000d && number > -1000d)
            {
                return number.ToString(n == 0 ? "N0" : "N1");
            }

            if (-10000d < number && number < 10000d)
                return number.ToString("N0");

            if (-100000d < number && number < 100000d)
                return number.ToString("N0");

            if (double.IsInfinity(number))
                return "Infinity";

            string showNumber;
            string[] partsSplit = number.ToString("E").Split('+');
            if (partsSplit.Length < 2)
            {
                Debug.LogWarning($"Failed - ToCurrencyString({number})");
                return Zero;
            }

            if (false == int.TryParse(partsSplit[1], out int exponent))
            {
                Debug.LogWarning(string.Format("Failed - ToCurrencyString({0}) : partsSplit[1] = {1}",
                    number, partsSplit[1]));
                return Zero;
            }

            int quotient = exponent / 4;
            int remainder = exponent % 4;
            string unitString = currencyType == CurrencyType.Default ? CurrencyUnits_KR[quotient] : SI[quotient];
            if (exponent < 4)
            {
                showNumber = number.ToString("F2");
                return $"{showNumber}{unitString}";
            }
            else
            {
                var temp = double.Parse(partsSplit[0].Replace("E", "")) * Math.Pow(10, remainder);

                // string text = temp.ToString("F3");
                // text = text.Substring(0, Math.Min(5, text.Length));
                // if (text.Contains("."))
                //     text = text.Replace(".", CurrencyUnits_KR[quotient]);
                // else
                //     text = text + CurrencyUnits_KR[quotient];
                // return text;
                string format = "F3";

                switch (remainder)
                {
                    case 0:
                        format = "F3";
                        break;
                    case 1:
                        format = "F2";
                        break;
                    case 2:
                        format = "F1";
                        break;
                    case 3:
                        format = "F0";
                        break;
                }

                showNumber = temp.ToString(format);
            }


            // 0 - 3, 1 - 2, 2 - 1, 3 - 

            return $"{showNumber}{unitString}"; // significant,
        }
    }

    public static string ToCurrencyString(this int number, CurrencyType currencyType = CurrencyType.Default)
    {
        {
            if (number == 0d)
                return Zero;

            var n = number % 1;

            if (number < 1d && number > -1d)
                return number.ToString("N4");

            if (number < 10d && number > -10d)
            {
                return number.ToString(n == 0 ? "N0" : "N3");
            }

            if (number < 100d && number > -100d)
            {
                return number.ToString(n == 0 ? "N0" : "N2");
            }

            if (number < 1000d && number > -1000d)
            {
                return number.ToString(n == 0 ? "N0" : "N1");
            }

            if (-10000d < number && number < 10000d)
                return number.ToString("N0");

            if (double.IsInfinity(number))
                return "Infinity";

            string showNumber;
            string[] partsSplit = number.ToString("E").Split('+');
            if (partsSplit.Length < 2)
            {
                Debug.LogWarning($"Failed - ToCurrencyString({number})");
                return Zero;
            }

            if (false == int.TryParse(partsSplit[1], out int exponent))
            {
                Debug.LogWarning(string.Format("Failed - ToCurrencyString({0}) : partsSplit[1] = {1}",
                    number, partsSplit[1]));
                return Zero;
            }

            int quotient = exponent / 4;
            int remainder = exponent % 4;
            if (exponent < 4)
            {
                showNumber = number.ToString("F2");
            }
            else
            {
                var temp = int.Parse(partsSplit[0].Replace("E", ""));

                string format = "F3";

                switch (remainder)
                {
                    case 0:
                        format = "F3";
                        break;
                    case 1:
                        format = "F2";
                        break;
                    case 2:
                        format = "F1";
                        break;
                    case 3:
                        format = "F0";
                        break;
                }

                showNumber = temp.ToString(format);
            }

            // 0 - 3, 1 - 2, 2 - 1, 3 - 0

            string unitString = currencyType == CurrencyType.Default ? CurrencyUnits_KR[quotient] : SI[quotient];

            return $"{showNumber}{unitString}"; // significant,
        }
    }

    public static string ToCurrencyString(this float number, CurrencyType currencyType = CurrencyType.Default)
    {
        {
            if (number == 0d)
                return Zero;

            //number = Math.Round(number, 7);

            var n = number % 1;

            if (number < 1d && number > -1d)
                return number.ToString("N4");

            if (number < 10d && number > -10d)
            {
                return number.ToString(n == 0 ? "N0" : "N3");
            }

            if (number < 100d && number > -100d)
            {
                return number.ToString(n == 0 ? "N0" : "N2");
            }

            if (number < 1000d && number > -1000d)
            {
                return number.ToString(n == 0 ? "N0" : "N1");
            }

            if (-10000d < number && number < 10000d)
                return number.ToString("N0");

            if (-100000d < number && number < 100000d)
                return number.ToString("N0");

            if (double.IsInfinity(number))
                return "Infinity";

            string showNumber;
            string[] partsSplit = number.ToString("E").Split('+');
            if (partsSplit.Length < 2)
            {
                Debug.LogWarning($"Failed - ToCurrencyString({number})");
                return Zero;
            }

            if (false == int.TryParse(partsSplit[1], out int exponent))
            {
                Debug.LogWarning($"Failed - ToCurrencyString({number}) : partsSplit[1] = {partsSplit[1]}");
                return Zero;
            }

            int quotient = exponent / 4;
            int remainder = exponent % 4;
            string unitString = currencyType == CurrencyType.Default ? CurrencyUnits_KR[quotient] : SI[quotient];
            if (exponent < 4)
            {
                showNumber = number.ToString("F2");
                return $"{showNumber}{unitString}";
            }
            else
            {
                var temp = double.Parse(partsSplit[0].Replace("E", "")) * Math.Pow(10, remainder);

                // string text = temp.ToString("F3");
                // text = text.Substring(0, Math.Min(5, text.Length));
                // if (text.Contains("."))
                //     text = text.Replace(".", CurrencyUnits_KR[quotient]);
                // else
                //     text = text + CurrencyUnits_KR[quotient];
                // return text;
                string format = "F3";

                switch (remainder)
                {
                    case 0:
                        format = "F3";
                        break;
                    case 1:
                        format = "F2";
                        break;
                    case 2:
                        format = "F1";
                        break;
                    case 3:
                        format = "F0";
                        break;
                }

                showNumber = temp.ToString(format);
            }


            // 0 - 3, 1 - 2, 2 - 1, 3 - 

            return $"{showNumber}{unitString}"; // significant,
        }
    }

    public static string ToCurrencyString(this ObscuredFloat number, CurrencyType currencyType = CurrencyType.Default)
    {
        {
            if (number == 0d)
                return Zero;

            //number = Math.Round(number, 7);

            var n = number % 1;

            if (number < 1d && number > -1d)
                return number.ToString("N4");

            if (number < 10d && number > -10d)
            {
                return number.ToString(n == 0 ? "N0" : "N3");
            }

            if (number < 100d && number > -100d)
            {
                return number.ToString(n == 0 ? "N0" : "N2");
            }

            if (number < 1000d && number > -1000d)
            {
                return number.ToString(n == 0 ? "N0" : "N1");
            }

            if (-10000d < number && number < 10000d)
                return number.ToString("N0");

            if (-100000d < number && number < 100000d)
                return number.ToString("N0");

            if (double.IsInfinity(number))
                return "Infinity";

            string showNumber;
            string[] partsSplit = number.ToString("E").Split('+');
            if (partsSplit.Length < 2)
            {
                Debug.LogWarning($"Failed - ToCurrencyString({number})");
                return Zero;
            }

            if (false == int.TryParse(partsSplit[1], out int exponent))
            {
                Debug.LogWarning(string.Format("Failed - ToCurrencyString({0}) : partsSplit[1] = {1}",
                    number, partsSplit[1]));
                return Zero;
            }

            int quotient = exponent / 4;
            int remainder = exponent % 4;
            string unitString = currencyType == CurrencyType.Default ? CurrencyUnits_KR[quotient] : SI[quotient];
            if (exponent < 4)
            {
                showNumber = number.ToString("F2");
                return $"{showNumber}{unitString}";
            }
            else
            {
                var temp = double.Parse(partsSplit[0].Replace("E", "")) * Math.Pow(10, remainder);

                // string text = temp.ToString("F3");
                // text = text.Substring(0, Math.Min(5, text.Length));
                // if (text.Contains("."))
                //     text = text.Replace(".", CurrencyUnits_KR[quotient]);
                // else
                //     text = text + CurrencyUnits_KR[quotient];
                // return text;
                string format = "F3";

                switch (remainder)
                {
                    case 0:
                        format = "F3";
                        break;
                    case 1:
                        format = "F2";
                        break;
                    case 2:
                        format = "F1";
                        break;
                    case 3:
                        format = "F0";
                        break;
                }

                showNumber = temp.ToString(format);
            }


            // 0 - 3, 1 - 2, 2 - 1, 3 - 

            return $"{showNumber}{unitString}"; // significant,
        }
    }

    public static string ToCurrencyString(this long number, CurrencyType currencyType = CurrencyType.Default)
    {
        {
            if (-1d < number && number < 1d)
            {
                return Zero;
            }

            if (double.IsInfinity(number))
            {
                return "Infinity";
            }

            //string significant = (number < 0) ? "-" : string.Empty;
            string showNumber;
            string[] partsSplit = number.ToString("E").Split('+');
            if (partsSplit.Length < 2)
            {
                Debug.LogWarning($"Failed - ToCurrencyString({number})");
                return Zero;
            }

            if (false == int.TryParse(partsSplit[1], out int exponent))
            {
                Debug.LogWarning(string.Format("Failed - ToCurrencyString({0}) : partsSplit[1] = {1}",
                    number, partsSplit[1]));
                return Zero;
            }

            int quotient = exponent / 4;
            int remainder = exponent % 4;
            if (exponent < 4)
            {
                showNumber = number.ToString();
            }
            else
            {
                var temp = long.Parse(partsSplit[0].Replace("E", ""));

                string format = "F3";

                switch (remainder)
                {
                    case 0:
                        format = "F3";
                        break;
                    case 1:
                        format = "F2";
                        break;
                    case 2:
                        format = "F1";
                        break;
                    case 3:
                        format = "F0";
                        break;
                }

                showNumber = temp.ToString(format);
            }

            string unitString = currencyType == CurrencyType.Default ? CurrencyUnits_KR[quotient] : SI[quotient];

            return $"{showNumber}{unitString}"; // significant,
        }
    }

    public static double GetWeaponAttack(int weaponIndex)
    {
        var weaponChart = ChartManager.WeaponCharts[weaponIndex];
        var weaponData = Managers.Game.WeaponDatas[weaponIndex];

        if (!weaponData.IsAcquired)
            return 0;

        var attack = weaponChart.EquipStatValue;
        var increaseAttack = attack * (weaponChart.EquipStatUpgradeValue * (Mathf.Max(weaponData.Level - 1, 0)));

        return attack + increaseAttack;
    }

    public static double GetWeaponHaveEffectValue(int weaponIndex)
    {
        var weaponChart = ChartManager.WeaponCharts[weaponIndex];
        var weaponData = Managers.Game.WeaponDatas[weaponIndex];

        var effectValue = weaponChart.HaveStatValue;
        var increaseEffectValue = effectValue * (weaponData.Level - 1);

        return effectValue + increaseEffectValue;
    }

    public static double GetWeaponHaveEffectValueByPercent(int weaponIndex)
    {
        return GetWeaponHaveEffectValue(weaponIndex) * 100;
    }

    public static long GetWeaponReinforcePrice(int weaponIndex)
    {
        var weaponChart = ChartManager.WeaponCharts[weaponIndex];
        var weaponData = Managers.Game.WeaponDatas[weaponIndex];

        var price = weaponChart.LevelUpItemId;
        var increasePrice = price * (weaponChart.LevelUpItemValue * (weaponData.Level - 1));

        return price + (int)increasePrice;
    }

    public static bool ExistCurrentStageBossData()
    {
        return ChartManager.StageBossDataController.StageBossTable.ContainsKey(Managers.Stage.StageId.Value);
    }

    public static bool ExistNextStageData()
    {
        int nextStageId = Managers.Stage.StageId.Value + 1;
        return ChartManager.StageDataController.StageDataTable.ContainsKey(nextStageId);
    }

    public static bool IsHwasengbangDungeon()
    {
        return Managers.Stage.State.Value == StageState.Dungeon && Managers.Dungeon.DungeonId.Value == 1 && Managers.Dungeon.HwasengbangDungeonWave == 1;
    }

    public static bool IsWorldCupDungeon()
    {
        return Managers.Stage.State.Value == StageState.WorldCupEvent;
    }

    public static bool IsGuildAllRaidDungeon()
    {
        return Managers.Stage.State.Value == StageState.GuildAllRaid;
    }

    public static bool IsEnoughItem(ItemType itemType, int id, double value)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
            {
                if (!Managers.Game.WeaponDatas.TryGetValue(id, out var weaponData))
                    return false;

                return weaponData.Quantity >= value;
            }
            case ItemType.Goods:
            {
                if (!Managers.Game.GoodsDatas.TryGetValue(id, out var goodsData))
                    return false;

                return goodsData.Value >= value;
            }
        }

        return false;
    }

    public static bool IsEnoughItem(ItemType itemType, int id, double value, float increaseValue, int level)
    {
        return IsEnoughItem(itemType, id, CalculateItemValue(value, increaseValue, level));
    }

    public static double CalculateItemValue(double value, double increaseValue, int level)
    {
        level -= 1;

        if (level < 0)
            level = 0;

        return Math.Round(value + level * increaseValue, 7);
    }

    public static double GetStatGoldUpgradePrice(int statId, int multiple = 1)
    {
        double totalValue = 0;

        var statLv = Managers.Game.StatLevelDatas[statId].Value;
        var nextLv = statLv + multiple;
        long prevStatUpgradeLv = 0;

        foreach (var chartData in ChartManager.StatGoldUpgradeLevelCharts.Values)
        {
            if (chartData.StatId != statId)
                continue;

            bool isStop = false;

            for (var i = statLv; i < nextLv; i++, statLv++)
            {
                if (statLv > chartData.UpgradeLevel)
                {
                    isStop = true;
                    break;
                }

                totalValue += chartData.UpgradeItemValue +
                              chartData.UpgradeItemIncreaseValue * (statLv - prevStatUpgradeLv - 1);
            }

            prevStatUpgradeLv = chartData.UpgradeLevel;

            if (isStop)
                continue;

            if (statLv >= nextLv)
                break;
        }

        return totalValue;
    }

    public static string GetStatName(StatType statType)
    {
        return !ChartManager.StatCharts.TryGetValue((int)statType, out var statChart)
            ? string.Empty
            : ChartManager.GetString(statChart.Name);
    }

    public static string GetIncreaseStatDesc(StatType statType)
    {
        switch (statType)
        {
            case StatType.None:
                return string.Empty;
            case StatType.SkillCoolTimeReduce:
                return $"{GetStatName(statType)} 감소";
            default:
                return $"{GetStatName(statType)} 증가";
        }
    }

    public static double CalculateAttackDamage(out double criticalMultiple,
        MonsterType monsterType = MonsterType.Normal, bool isBaseAttack = true)
    {
        if (IsHwasengbangDungeon() || IsWorldCupDungeon())
        {
            criticalMultiple = 1;
            return 1;
        }

        double damage = Managers.Game.BaseStatDatas[(int)StatType.Attack];
        damage *= Managers.Game.BaseStatDatas[(int)StatType.AttackPer];
        if (isBaseAttack)
        {
            damage *= Managers.Game.BaseStatDatas[(int)StatType.NormalAttackDamage]; // Wood
        }

        StatType? criticalRateStatType = null;
        criticalMultiple = 0;

        for (StatType statType = StatType.CriticalRate2; statType <= StatType.CriticalRate9009; statType++)
        {
            if (Managers.Game.BaseStatDatas[(int)statType] <= 0)
                break;

            double criticalRate = Managers.Game.BaseStatDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }

        for (StatType statType = StatType.CriticalRate10009; statType <= StatType.CriticalRate14009; statType++)
        {
            if (Managers.Game.BaseStatDatas[(int)statType] <= 0)
                break;

            double criticalRate = Managers.Game.BaseStatDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }

        // 크리티컬 적용
        if (criticalRateStatType.HasValue)
        {
            criticalMultiple = GetCriticalMultiple((int)criticalRateStatType.Value);
            damage *= criticalMultiple;
            damage *= Managers.Game.IncreaseCriticalDamage;
        }

        // 노말 몬스터 추뎀
        if (monsterType == MonsterType.Normal && Managers.Game.BaseStatDatas[(int)StatType.NormalMonsterDamage] > 0)
            damage *= Managers.Game.BaseStatDatas[(int)StatType.NormalMonsterDamage];

        // 보스 추뎀
        if (monsterType == MonsterType.Boss || monsterType == MonsterType.AllRaidBoss && Managers.Game.BaseStatDatas[(int)StatType.BossMonsterDamage] > 0)
            damage *= Managers.Game.BaseStatDatas[(int)StatType.BossMonsterDamage];

        // 최종데미지 적용
        if (Managers.Game.BaseStatDatas[(int)StatType.FinalDamageIncrease] > 1)
            damage *= Managers.Game.BaseStatDatas[(int)StatType.FinalDamageIncrease];

        return damage;
    }

    public static double CalculateAttackDamage(out double criticalMultiple,
        Dictionary<int, double> statDatas, MonsterType monsterType = MonsterType.Normal, bool isBaseAttack = true)
    {
        if (IsHwasengbangDungeon() || IsWorldCupDungeon())
        {
            criticalMultiple = 1;
            return 1;
        }

        double damage = statDatas[(int)StatType.Attack];
        damage *= statDatas[(int)StatType.AttackPer];

        if (isBaseAttack && statDatas.ContainsKey((int)StatType.NormalAttackDamage))
        {
            damage *= statDatas[(int)StatType.NormalAttackDamage]; // Wood
        }
        

        StatType? criticalRateStatType = null;
        criticalMultiple = 0;

        for (StatType statType = StatType.CriticalRate2; statType <= StatType.CriticalRate9009; statType++)
        {
            if (statDatas[(int)statType] <= 0)
                break;

            double criticalRate = statDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }

        for (StatType statType = StatType.CriticalRate10009; statType <= StatType.CriticalRate14009; statType++)
        {
            if (statDatas[(int)statType] <= 0)
                break;

            double criticalRate = statDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }

        double totalIncreaseCriticalDamage = 0;

        for (int statType = (int)StatType.CriticalDamage2; statType <= (int)StatType.CriticalDamage9009; statType++)
        {
            if (!statDatas.ContainsKey(statType))
                continue;

            if (statDatas[statType] <= 0)
                continue;

            totalIncreaseCriticalDamage += statDatas[statType];
        }
        
        for (int statType = (int)StatType.CriticalDamage10009; statType <= (int)StatType.CriticalDamage14009; statType++)
        {
            if (!statDatas.ContainsKey(statType))
                continue;

            if (statDatas[statType] <= 0)
                continue;

            totalIncreaseCriticalDamage += statDatas[statType];
        }

        if (statDatas.ContainsKey((int)StatType.CriticalDamage))
            totalIncreaseCriticalDamage += statDatas[(int)StatType.CriticalDamage];

        // 크리티컬 적용
        if (criticalRateStatType.HasValue)
        {
            criticalMultiple = GetCriticalMultiple((int)criticalRateStatType.Value);
            damage *= criticalMultiple;
            damage *= totalIncreaseCriticalDamage;
        }

        // 노말 몬스터 추뎀
        if (monsterType == MonsterType.Normal && statDatas[(int)StatType.NormalMonsterDamage] > 0)
            damage *= statDatas[(int)StatType.NormalMonsterDamage];

        // 보스 추뎀
        if (monsterType == MonsterType.Boss || monsterType == MonsterType.AllRaidBoss && statDatas[(int)StatType.BossMonsterDamage] > 0)
            damage *= statDatas[(int)StatType.BossMonsterDamage];

        // 최종데미지 적용
        if (statDatas[(int)StatType.FinalDamageIncrease] > 1)
            damage *= statDatas[(int)StatType.FinalDamageIncrease];

        return damage;
    }

    public static double CalculatePvpAttackDamage(out double criticalMultiple, int skillId = 57, bool isSkill = false)
    {
        double damage = Managers.Game.BaseStatDatas[(int)StatType.Attack];
        damage *= Managers.Game.BaseStatDatas[(int)StatType.AttackPer];
        damage *= Managers.Game.BaseStatDatas[(int)StatType.NormalAttackDamage]; // Wood

        StatType? criticalRateStatType = null;
        criticalMultiple = 0;

        for (StatType statType = StatType.CriticalRate2; statType <= StatType.CriticalRate9009; statType++)
        {
            if (Managers.Game.BaseStatDatas[(int)statType] <= 0)
                break;

            double criticalRate = Managers.Game.BaseStatDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }

        for (StatType statType = StatType.CriticalRate10009; statType <= StatType.CriticalRate14009; statType++)
        {
            if (Managers.Game.BaseStatDatas[(int)statType] <= 0)
                break;

            double criticalRate = Managers.Game.BaseStatDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }

        // 크리티컬 적용
        if (criticalRateStatType.HasValue)
        {
            criticalMultiple = GetCriticalMultiple((int)criticalRateStatType.Value);
            damage *= criticalMultiple;
            damage *= Managers.Game.IncreaseCriticalDamage;
        }

        // 최종데미지 적용
        if (Managers.Game.BaseStatDatas[(int)StatType.FinalDamageIncrease] > 1)
            damage *= Managers.Game.BaseStatDatas[(int)StatType.FinalDamageIncrease];

        if (Managers.Game.SkillDatas[skillId].Level > 0 && isSkill)
        {
            var skillChart = ChartManager.SkillCharts[skillId];
            float skillDamagePercent = skillChart.Value + (skillChart.IncreaseValue * Math.Max(Managers.Game.SkillDatas[skillId].Level - 1, 0));
            damage *= skillDamagePercent;
        }

        return damage;
    }

    public static double CalculatePvpEnemyAttackDamage(out double criticalMultiple, int skillId = 57, bool isPvp = false)
    {
        double damage = Managers.Pvp.EnemyBaseStatDatas[(int)StatType.Attack];
        damage *= Managers.Pvp.EnemyBaseStatDatas[(int)StatType.AttackPer];

        if (Managers.Pvp.EnemyBaseStatDatas.ContainsKey((int)StatType.NormalAttackDamage))
        {
            damage *= Managers.Pvp.EnemyBaseStatDatas[(int)StatType.NormalAttackDamage];
        }

        StatType? criticalRateStatType = null;
        criticalMultiple = 0;

        for (StatType statType = StatType.CriticalRate2; statType <= StatType.CriticalRate9009; statType++)
        {
            if (!Managers.Pvp.EnemyBaseStatDatas.ContainsKey((int)statType))
                continue;

            if (Managers.Pvp.EnemyBaseStatDatas[(int)statType] <= 0)
                break;

            double criticalRate = Managers.Pvp.EnemyBaseStatDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }

        for (StatType statType = StatType.CriticalRate10009; statType <= StatType.CriticalRate14009; statType++)
        {
            if (!Managers.Pvp.EnemyBaseStatDatas.ContainsKey((int)statType))
                continue;

            if (Managers.Pvp.EnemyBaseStatDatas[(int)statType] <= 0)
                break;

            double criticalRate = Managers.Pvp.EnemyBaseStatDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }


        // 크리티컬 적용
        if (criticalRateStatType.HasValue)
        {
            criticalMultiple = GetCriticalMultiple((int)criticalRateStatType.Value);
            damage *= criticalMultiple;

            double totalCriticalDamage = 0;

            for (StatType statType = StatType.CriticalDamage2; statType <= StatType.CriticalDamage9009; statType++)
            {
                if (!Managers.Pvp.EnemyBaseStatDatas.ContainsKey((int)statType))
                    continue;

                totalCriticalDamage += Managers.Pvp.EnemyBaseStatDatas[(int)statType];
            }

            for (StatType statType = StatType.CriticalDamage10009; statType <= StatType.CriticalDamage14009; statType++)
            {
                if (!Managers.Pvp.EnemyBaseStatDatas.ContainsKey((int)statType))
                    continue;

                totalCriticalDamage += Managers.Pvp.EnemyBaseStatDatas[(int)statType];
            }

            if (Managers.Pvp.EnemyBaseStatDatas.ContainsKey((int)StatType.CriticalDamage))
                totalCriticalDamage += Managers.Pvp.EnemyBaseStatDatas[(int)StatType.CriticalDamage];

            damage *= totalCriticalDamage;
        }

        if (Managers.Pvp.EnemyBaseStatDatas.ContainsKey((int)StatType.FinalDamageIncrease))
            damage *= Managers.Pvp.EnemyBaseStatDatas[(int)StatType.FinalDamageIncrease];

        if (Managers.Pvp.EnemyPassiveGodgodLv > 0 && isPvp)
        {
            var skillChart = ChartManager.SkillCharts[skillId];
            float skillDamagePercent = skillChart.Value + (skillChart.IncreaseValue * Math.Max(Managers.Pvp.EnemyPassiveGodgodLv - 1, 0));
            damage *= skillDamagePercent;
        }
            

        return damage;
    }

    public static double CalculatePassiveSkillDamage(int skillId, out double criticalMultiple)
    {
        if (IsHwasengbangDungeon() || IsWorldCupDungeon())
        {
            criticalMultiple = 1;
            return 1;
        }

        int skillLevel = Managers.Game.SkillDatas[skillId].Level;

        var skillChart = ChartManager.SkillCharts[skillId];
        float skillDamagePercent = skillChart.Value + (skillChart.IncreaseValue * Math.Max(skillLevel - 1, 0));

        double damage = Managers.Game.BaseStatDatas[(int)StatType.Attack];
        damage *= Managers.Game.BaseStatDatas[(int)StatType.AttackPer];
        
        StatType? criticalRateStatType = null;
        criticalMultiple = 0;

        for (StatType statType = StatType.CriticalRate2; statType <= StatType.CriticalRate9009; statType++)
        {
            if (Managers.Game.BaseStatDatas[(int)statType] <= 0)
                break;

            double criticalRate = Managers.Game.BaseStatDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }

        for (StatType statType = StatType.CriticalRate10009; statType <= StatType.CriticalRate14009; statType++)
        {
            if (Managers.Game.BaseStatDatas[(int)statType] <= 0)
                break;

            double criticalRate = Managers.Game.BaseStatDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }

        // 크리티컬 적용
        if (criticalRateStatType.HasValue)
        {
            criticalMultiple = GetCriticalMultiple((int)criticalRateStatType.Value);
            damage *= criticalMultiple;
            damage *= Managers.Game.IncreaseCriticalDamage;
        }

        // WorldWood Damage
        if (Managers.Game.BaseStatDatas[(int)StatType.NormalAttackDamage] > 1)
            damage *= Managers.Game.BaseStatDatas[(int)StatType.NormalAttackDamage];

        // 최종데미지 적용
        if (Managers.Game.BaseStatDatas[(int)StatType.FinalDamageIncrease] > 1)
            damage *= Managers.Game.BaseStatDatas[(int)StatType.FinalDamageIncrease];

        // PassiveDamage Commit
        damage *= skillDamagePercent;

        return damage;
    }

    public static double CalculatePassiveSkillDamage(int skillId, int skilllevel, out double criticalMultiple, Dictionary<int, double> statDatas)
    {
        if (IsHwasengbangDungeon() || IsWorldCupDungeon())
        {
            criticalMultiple = 1;
            return 1;
        }

        int skillLevel = skilllevel;

        var skillChart = ChartManager.SkillCharts[skillId];
        float skillDamagePercent = skillChart.Value + (skillChart.IncreaseValue * Math.Max(skillLevel - 1, 0));

        double damage = statDatas[(int)StatType.Attack];
        damage *= statDatas[(int)StatType.AttackPer];

        StatType? criticalRateStatType = null;
        criticalMultiple = 0;

        for (StatType statType = StatType.CriticalRate2; statType <= StatType.CriticalRate9009; statType++)
        {
            if (statDatas[(int)statType] <= 0)
                break;

            double criticalRate = statDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }

        for (StatType statType = StatType.CriticalRate10009; statType <= StatType.CriticalRate14009; statType++)
        {
            if (statDatas[(int)statType] <= 0)
                break;

            double criticalRate = statDatas[(int)statType];

            if (criticalRate >= 1f)
            {
                criticalRateStatType = statType;
            }
            else
            {
                float randRate = Random.Range(0f, 1f);
                if (randRate <= criticalRate)
                    criticalRateStatType = statType;
            }
        }

        // 크리티컬 적용
        if (criticalRateStatType.HasValue)
        {
            criticalMultiple = GetCriticalMultiple((int)criticalRateStatType.Value);
            damage *= criticalMultiple;

            double totalCriticalDamage = 0;

            for (StatType statType = StatType.CriticalDamage2; statType <= StatType.CriticalDamage9009; statType++)
            {
                if (!statDatas.ContainsKey((int)statType))
                    continue;

                totalCriticalDamage += statDatas[(int)statType];
            }

            for (StatType statType = StatType.CriticalDamage10009; statType <= StatType.CriticalDamage14009; statType++)
            {
                if (!statDatas.ContainsKey((int)statType))
                    continue;

                totalCriticalDamage += statDatas[(int)statType];
            }

            if (statDatas.ContainsKey((int)StatType.CriticalDamage))
                totalCriticalDamage += statDatas[(int)StatType.CriticalDamage];

            damage *= totalCriticalDamage; 
        }

        // WorldWood Damage 
        if (statDatas.ContainsKey((int)StatType.NormalAttackDamage))
        {
            if (statDatas[(int)StatType.NormalAttackDamage] > 1)
                damage *= statDatas[(int)StatType.NormalAttackDamage];
        }

        // 최종데미지 적용
        if (statDatas[(int)StatType.FinalDamageIncrease] > 1)
            damage *= statDatas[(int)StatType.FinalDamageIncrease];

        // PassiveDamage Commit
        damage *= skillDamagePercent;

        return damage;
    }

    public static double CalculateSkillDamage(int skillId, out double criticalMultiple, MonsterType monsterType)
    {
        if (IsHwasengbangDungeon())
        {
            criticalMultiple = 1;
            return 1;
        }

        int skillLevel = Managers.Game.SkillDatas[skillId].Level;

        var skillChart = ChartManager.SkillCharts[skillId];
        float skillDamagePercent = skillChart.Value + (skillChart.IncreaseValue * Math.Max(skillLevel - 1, 0));

        if (Managers.Game.LabResearchDatas.TryGetValue(skillChart.LabSkillType, out var labResearchData))
        {
            if (ChartManager.LabSkillLevelCharts.TryGetValue(labResearchData.Level, out var labSkillLevelChart))
            {
                skillDamagePercent += (float)labSkillLevelChart.IncreaseDamagePercent;
            }
        }

        double skillDamage = CalculateAttackDamage(out criticalMultiple, monsterType, false) * skillDamagePercent;

        if (Managers.Game.BaseStatDatas[(int)StatType.SkillDamage] > 0)
            skillDamage *= Managers.Game.BaseStatDatas[(int)StatType.SkillDamage];


        return skillDamage;
    }

    public static double GetItemValue(ItemType itemType, int itemId)
    {
        switch (itemType)
        {
            case ItemType.Goods:
            {
                if (Managers.Game.GoodsDatas.TryGetValue(itemId, out var goodsData))
                    return goodsData.Value;

                return 0;
            }
            case ItemType.Weapon:
            {
                if (Managers.Game.WeaponDatas.TryGetValue(itemId, out var weaponData))
                    return weaponData.Quantity;

                return 0;
            }
            case ItemType.Costume:
            {
                if (Managers.Game.CostumeDatas.TryGetValue(itemId, out var costumeData))
                    return costumeData.Quantity;

                return 0;
            }
            default:
                return 0;
        }
    }

    public static DateTime GetNow()
    {
        return Managers.Game.ServerTime;
    }

    public static DateTime GetDay(int addDay = 0)
    {
        var date = GetNow().AddDays(addDay);
        return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
    }

    // week주 후에 dayOfWeek요일 날짜 반환
    // 기본값 1주뒤 월요일 반환
    public static DateTime GetDayOfWeek(int week = 1, DayOfWeek dayOfWeek = DayOfWeek.Monday)
    {
        // 당일은 체크 안함
        var dateTime = GetDay(1);

        while (dateTime.DayOfWeek != dayOfWeek)
        {
            dateTime = dateTime.AddDays(1);
        }

        return dateTime;
    }

    // 치명타 데미지 합
    public static double GetTotalCriticalDamage()
    {
        double criticalDamage = 0;

        for (int i = (int)StatType.CriticalDamage2; i <= (int)StatType.CriticalDamage512; i++)
            criticalDamage += Managers.Game.BaseStatDatas[i];

        return criticalDamage;
    }

    public static double GetCriticalMultiple(int statType)
    {
        switch (statType)
        {
            case (int)StatType.CriticalDamage2:
            case (int)StatType.CriticalRate2:
                return 2;
            case (int)StatType.CriticalDamage4:
            case (int)StatType.CriticalRate4:
                return 4;
            case (int)StatType.CriticalDamage8:
            case (int)StatType.CriticalRate8:
                return 8;
            case (int)StatType.CriticalDamage16:
            case (int)StatType.CriticalRate16:
                return 16;
            case (int)StatType.CriticalDamage32:
            case (int)StatType.CriticalRate32:
                return 32;
            case (int)StatType.CriticalDamage64:
            case (int)StatType.CriticalRate64:
                return 64;
            case (int)StatType.CriticalDamage128:
            case (int)StatType.CriticalRate128:
                return 128;
            case (int)StatType.CriticalDamage256:
            case (int)StatType.CriticalRate256:
                return 256;
            case (int)StatType.CriticalDamage512:
            case (int)StatType.CriticalRate512:
                return 512;
            case (int)StatType.CriticalDamage1024:
            case (int)StatType.CriticalRate1024:
                return 1024;
            case (int)StatType.CriticalDamage2048:
            case (int)StatType.CriticalRate2048:
                return 2048;
            case (int)StatType.CriticalDamage4096:
            case (int)StatType.CriticalRate4096:
                return 4096;
            case (int)StatType.CriticalDamage5009:
            case (int)StatType.CriticalRate5009:
                return 5009;
            case (int)StatType.CriticalDamage6009:
            case (int)StatType.CriticalRate6009:
                return 6009;
            case (int)StatType.CriticalDamage7009:
            case (int)StatType.CriticalRate7009:
                return 7009;
            case (int)StatType.CriticalDamage8009:
            case (int)StatType.CriticalRate8009:
                return 8009;
            case (int)StatType.CriticalDamage9009:
            case (int)StatType.CriticalRate9009:
                return 9009;
            case (int)StatType.CriticalDamage10009:
            case (int)StatType.CriticalRate10009:
                return 10009;
            case (int)StatType.CriticalDamage11009:
            case (int)StatType.CriticalRate11009:
                return 11009;
            case (int)StatType.CriticalDamage12009:
            case (int)StatType.CriticalRate12009:
                return 12009;
            case (int)StatType.CriticalDamage13009:
            case (int)StatType.CriticalRate13009:
                return 13009;
            case (int)StatType.CriticalDamage14009:
            case (int)StatType.CriticalRate14009:
                return 14009;
            default:
                return 1;
        }
    }

    public static void GetReinforceCriticalRate(out StatType? prevReinforceCriticalRateType,
        out StatType? currentReinforceCriticalRateType)
    {
        prevReinforceCriticalRateType = null;
        currentReinforceCriticalRateType = null;

        for (int promoId = 1; promoId <= ChartManager.SystemCharts[SystemData.MaxPromoLevel].Value; promoId++)
        {
            if (Managers.Game.UserData.PromoGrade < promoId)
                break;

            var statId = ChartManager.PromoDungeonCharts[promoId].ClearRewardStat2Id;

            if (statId == 0)
                continue;

            var maxLevel = ChartManager.StatGoldUpgradeCharts[statId].MaxLevel;

            // 현재 100% 적용중인 치명타 확률
            if (Managers.Game.StatLevelDatas[statId].Value >= maxLevel)
                prevReinforceCriticalRateType = (StatType)statId.GetDecrypted();

            // 현재 강화중인 치명타 확률
            if (Managers.Game.StatLevelDatas[statId].Value < maxLevel)
            {
                currentReinforceCriticalRateType = (StatType)statId.GetDecrypted();
                return;
            }
        }
    }

    public static bool IsContainsNextLevelData()
    {
        return ChartManager.LevelCharts.ContainsKey(Managers.Game.UserData.Level + 1);
    }

    public static bool IsPurchasedProduct(int shopId)
    {
        if (!Managers.Game.ShopDatas.TryGetValue(shopId, out var shopData))
            return false;

        return shopData != 0;
    }

    // 골드 강화 레벨 스탯 증가값 반환
    public static double CalculateGoldUpgradeStat(int statId, long statLv)
    {
        var upgradeCharts =
            ChartManager.StatGoldUpgradeLevelCharts.Values.ToList().FindAll(chart => chart.StatId == statId);

        bool isStop = false;
        int prevLv = 0;

        double statValue = 0;

        foreach (var upgradeChart in upgradeCharts)
        {
            if (isStop)
                break;

            if (upgradeChart.UpgradeLevel > statLv)
            {
                isStop = true;
            }

            long increaseLv = isStop ? statLv - prevLv : upgradeChart.UpgradeLevel - prevLv;
            statValue += increaseLv * upgradeChart.IncreaseValue;

            prevLv = upgradeChart.UpgradeLevel;
        }

        return statValue;
    }

    // 스탯 포인트 강화 레벨 스탯 증가값 반환
    public static double CalculateStatPointUpgradeStat(int statId, long statLv)
    {
        var upgradeCharts =
            ChartManager.StatPointUpgradeLevelCharts.Values.ToList().FindAll(chart => chart.StatId == statId);

        bool isStop = false;
        int prevLv = 0;

        double statValue = 0;

        foreach (var upgradeChart in upgradeCharts)
        {
            if (isStop)
                break;

            if (upgradeChart.UpgradeLevel > statLv)
            {
                isStop = true;
            }

            long increaseLv = isStop ? statLv - prevLv : upgradeChart.UpgradeLevel - prevLv;
            statValue += increaseLv * upgradeChart.IncreaseValue;

            prevLv = upgradeChart.UpgradeLevel;
        }

        return statValue;
    }

    public static double CalculateUnlimitedPointUpgradeStat(int statId, long statLv)
    {
        var upgradeCharts =
            ChartManager.UnlimitedPointUpgradeLevelCharts.Values.ToList().FindAll(chart => chart.StatId == statId);

        bool isStop = false;
        int prevLv = 0;

        double statValue = 0;

        foreach (var upgradeChart in upgradeCharts)
        {
            if (isStop)
                break;

            if (upgradeChart.UpgradeLevel > statLv)
            {
                isStop = true;
            }

            long increaseLv = isStop ? statLv - prevLv : upgradeChart.UpgradeLevel - prevLv;
            statValue += increaseLv * upgradeChart.LevelIncreaseValue;

            prevLv = upgradeChart.UpgradeLevel;
        }

        return statValue;
    }

    public static Color GetRankColor(int rank)
    {
        switch (rank)
        {
            case 1:
                return new Color(0.91f, 0.75f, 0.18f);
            case 2:
                return new Color(0.45f, 0.53f, 0.67f);
            case 3:
                return new Color(0.43f, 0.31f, 0.22f);
            default:
            {
                return rank switch
                {
                    >= 4 and <= 10 => new Color(0.56f, 0.35f, 0.36f),
                    >= 11 and <= 50 => new Color(0.86f, 0.84f, 0.93f),
                    >= 51 and <= 100 => new Color(0.73f, 0.68f, 0.54f),
                    >= 101 and <= 200 => new Color(0.62f, 0.77f, 0.81f),
                    >= 201 and <= 400 => new Color(0.73f, 0.68f, 0.54f),
                    _ => new Color(0.32f, 0.32f, 0.32f)
                };
            }
            // default:
            //     return new Color(0.32f, 0.32f, 0.32f);
        }
    }

    public static void BuyShopItem(int shopId, bool isSaveReward, bool isSaveShop, bool isEventShopSave = false, Action onCompleteCallback = null,
        Action onFailCallback = null)
    {
        if (!ChartManager.ShopCharts.TryGetValue(shopId, out var shopChart))
        {
            Managers.Message.ShowMessage("존재하지 않는 상품 입니다");
            return;
        }

        if (shopChart.LimitType != ShopLimitType.None && Managers.Game.ShopDatas[shopId] >= shopChart.LimitValue)
        {
            Managers.Message.ShowMessage("모두 구매한 상품 입니다.");
            onFailCallback?.Invoke();
            return;
        }

        void GiveReward()
        {
            Managers.Game.ShopDatas[shopId] += 1;

            if (isSaveShop)
                GameDataManager.ShopGameData.SaveGameData();

            if (isEventShopSave)
            {
                GameDataManager.ShopGameData.SaveGameData();
                GameDataManager.GoodsGameData.SaveGameData();
            }
                

            var itemTypes = new List<ItemType>();
            var gainItemDatas = new Dictionary<(ItemType, int), double>();

            for (int i = 0; i < shopChart.RewardItemTypes.Length; i++)
            {
                if (shopChart.RewardItemTypes[i] == ItemType.None)
                    continue;

                Managers.Game.IncreaseItem(shopChart.RewardItemTypes[i], shopChart.RewardItemIds[i],
                    shopChart.RewardItemValues[i]);

                if (!itemTypes.Contains(shopChart.RewardItemTypes[i]))
                    itemTypes.Add(shopChart.RewardItemTypes[i]);

                var gainItemKey = (shopChart.RewardItemTypes[i], shopChart.RewardItemIds[i]);

                if (gainItemDatas.ContainsKey(gainItemKey))
                    gainItemDatas[(shopChart.RewardItemTypes[i], shopChart.RewardItemIds[i])] +=
                        shopChart.RewardItemValues[i];
                else
                    gainItemDatas[(shopChart.RewardItemTypes[i], shopChart.RewardItemIds[i])] =
                        shopChart.RewardItemValues[i];
            }

            Managers.UI.ShowGainItems(gainItemDatas);

            Managers.Sound.PlayUISound(UISoundType.BuyShopItem);

            if (isSaveReward)
                itemTypes.ForEach(GameDataManager.SaveItemData);

            if (isSaveShop)
            {
                Param param = new Param();

                param.Add("ProductName", shopChart.ProductName.GetDecrypted());
                param.Add("GainItem", gainItemDatas);
                GetGoodsLog(ref param);

                Backend.GameLog.InsertLog("Shop", param);
            }

            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.BuyShop, shopId, 1));

            IsBuyingProcess = false;

            onCompleteCallback?.Invoke();
        }

        IsBuyingProcess = true;

        switch (shopChart.PriceType)
        {
            case ShopPriceType.Free:
            {
                GiveReward();
                Managers.Message.ShowMessage("구매 성공");
            }
                break;
            case ShopPriceType.Goods:
            {
                if (!IsEnoughItem(ItemType.Goods, shopChart.PriceId, shopChart.PriceValue))
                {
                    Managers.Message.ShowMessage("재화가 부족합니다.");
                    onFailCallback?.Invoke();
                    return;
                }

                Managers.Game.DecreaseItem(ItemType.Goods, shopChart.PriceId, shopChart.PriceValue);
                GiveReward();
            }
                break;
            case ShopPriceType.Cash:
            {
                Managers.IAP.BuyItem(shopId, GiveReward, () =>
                {
                    Managers.Message.ShowMessage("구매 실패");
                    onFailCallback?.Invoke();
                });
            }
                break;
            case ShopPriceType.AD:
            {
                if (Managers.Ad.Show(GiveReward))
                {
                }
                else
                    onFailCallback?.Invoke();
            }
                break;
            default:
            {
                onFailCallback?.Invoke();
            }
                break;
        }
    }

    public static double GetPower()
    {
        double criticalMultiple = 0;

        for (int promoGrade = Managers.Game.UserData.PromoGrade;
             promoGrade >= 1 && promoGrade <= ChartManager.SystemCharts[SystemData.MaxPromoLevel].Value;
             promoGrade++)
        {
            if (!ChartManager.PromoDungeonCharts.TryGetValue(promoGrade, out var promoDungeonChart))
            {
                if (promoGrade >= ChartManager.SystemCharts[SystemData.MaxPromoLevel].Value)
                {
                    for (int i = promoGrade; i >= 1; i--)
                    {
                        if (!ChartManager.PromoDungeonCharts.TryGetValue(promoGrade, out promoDungeonChart))
                            continue;

                        if (promoDungeonChart.ClearRewardStat2Id == (int)StatType.None)
                            continue;

                        criticalMultiple = promoDungeonChart.ClearRewardStat2Id;

                        break;
                    }

                    break;
                }

                continue;
            }

            if (promoDungeonChart.ClearRewardStat2Id == (int)StatType.None)
                continue;

            criticalMultiple = promoDungeonChart.ClearRewardStat2Id;

            break;
        }

        double power = Managers.Game.BaseStatDatas[(int)StatType.Attack];

        if (Managers.Game.BaseStatDatas[(int)StatType.AttackPer] > 0)
            power *= Managers.Game.BaseStatDatas[(int)StatType.AttackPer];

        if (criticalMultiple > 0)
            power *= criticalMultiple;

        if (Managers.Game.IncreaseCriticalDamage > 0)
            power *= Managers.Game.IncreaseCriticalDamage;

        if (Managers.Game.BaseStatDatas[(int)StatType.FinalDamageIncrease] > 0)
            power *= Managers.Game.BaseStatDatas[(int)StatType.FinalDamageIncrease];

        if (Managers.Game.BaseStatDatas[(int)StatType.SkillDamage] > 0)
            power *= Managers.Game.BaseStatDatas[(int)StatType.SkillDamage];

        if (Managers.Game.BaseStatDatas[(int)StatType.BossMonsterDamage] > 0)
            power *= Managers.Game.BaseStatDatas[(int)StatType.BossMonsterDamage];

        return power * 0.001;
    }

    public static double GetPower(Dictionary<int, double> baseStatDatas, int promoGrade)
    {
        double criticalMultiple = 0;

        for (; promoGrade >= 1 && promoGrade <= ChartManager.SystemCharts[SystemData.MaxPromoLevel].Value; promoGrade++)
        {
            if (!ChartManager.PromoDungeonCharts.TryGetValue(promoGrade, out var promoDungeonChart))
            {
                if (promoGrade >= ChartManager.SystemCharts[SystemData.MaxPromoLevel].Value)
                {
                    for (int i = promoGrade; i >= 1; i--)
                    {
                        if (!ChartManager.PromoDungeonCharts.TryGetValue(promoGrade, out promoDungeonChart))
                            continue;

                        if (promoDungeonChart.ClearRewardStat2Id == (int)StatType.None)
                            continue;

                        criticalMultiple = promoDungeonChart.ClearRewardStat2Id;

                        break;
                    }

                    break;
                }

                continue;
            }

            if (promoDungeonChart.ClearRewardStat2Id == (int)StatType.None)
                continue;

            criticalMultiple = promoDungeonChart.ClearRewardStat2Id;

            break;
        }

        double power = baseStatDatas[(int)StatType.Attack];

        if (baseStatDatas[(int)StatType.AttackPer] > 0)
            power *= baseStatDatas[(int)StatType.AttackPer];

        if (criticalMultiple > 0)
            power *= criticalMultiple;

        double IncreaseCriticalDamage = 0;

        for (int statType = (int)StatType.CriticalDamage2; statType <= (int)StatType.CriticalDamage9009; statType++)
        {
            if (!baseStatDatas.ContainsKey(statType))
                continue;

            if (baseStatDatas[statType] <= 0)
                break;

            IncreaseCriticalDamage += baseStatDatas[statType];
        }

        for (int statType = (int)StatType.CriticalDamage10009;
             statType <= (int)StatType.CriticalDamage14009;
             statType++)
        {
            if (!baseStatDatas.ContainsKey(statType))
                continue;

            if (baseStatDatas[statType] <= 0)
                break;

            IncreaseCriticalDamage += baseStatDatas[statType];
        }

        IncreaseCriticalDamage += baseStatDatas[(int)StatType.CriticalDamage];

        if (IncreaseCriticalDamage > 0)
            power *= IncreaseCriticalDamage;

        if (baseStatDatas[(int)StatType.FinalDamageIncrease] > 0)
            power *= baseStatDatas[(int)StatType.FinalDamageIncrease];

        if (baseStatDatas[(int)StatType.SkillDamage] > 0)
            power *= baseStatDatas[(int)StatType.SkillDamage];

        if (baseStatDatas[(int)StatType.BossMonsterDamage] > 0)
            power *= baseStatDatas[(int)StatType.BossMonsterDamage];

        return power * 0.001;
    }

    public static string GetShopLimitText(ShopLimitType shopLimitType)
    {
        return shopLimitType switch
        {
            ShopLimitType.None => string.Empty,
            ShopLimitType.Daily => "일일 초기화",
            ShopLimitType.Weekly => "주간 초기화",
            ShopLimitType.Monthly => "월간 초기화",
            ShopLimitType.NonReset => string.Empty,
            _ => string.Empty
        };
    }

    public static bool IsCurrentStageIsMaxStage()
    {
        return Managers.Stage.StageId.Value >= ChartManager.SystemCharts[SystemData.MaxStage].Value;
    }

    public static void GetGoodsLog(ref Param param)
    {
        Dictionary<int, double> dic = new();

        foreach (var goodsData in Managers.Game.GoodsDatas)
        {
            dic[goodsData.Key] = goodsData.Value.Value;
        }

        param.Add("Goods", dic);
    }

    public static bool IsDevServer()
    {
        return Managers.Server.CurrentServer == 100;
    }

    public static bool IsWhiteList()
    {
        return ChartManager.WhiteList.Contains(Backend.UserNickName);
    }

    public static bool IsCalculateRankTime()
    {
        var nowTime = GetNow();
        return (nowTime.Hour == 5 && nowTime.Minute <= 5) || nowTime.Hour == 4;
    }

    public static bool IsWeeklyCalculateRankTime()
    {
        return IsCalculateRankTime() && GetNow().DayOfWeek == DayOfWeek.Monday;
    }

    public static bool IsBackendCalculateRankTimeError(BackendReturnObject bro)
    {
        return bro.GetStatusCode().Contains("428") &&
               bro.GetErrorCode().Contains("Precondition") &&
               bro.GetMessage().Contains("ranking");
    }

    public static DateTime GetNextWeeklyDate(DayOfWeek dayOfWeek = DayOfWeek.Monday)
    {
        var dateTime = GetDay(1);

        while (dateTime.DayOfWeek != dayOfWeek)
            dateTime = dateTime.AddDays(1);

        return dateTime;
    }

    public static DateTime GetNextMonthDate()
    {
        var dateTime = GetDay(1);

        while (dateTime.Day != 1)
            dateTime = dateTime.AddDays(1);

        return dateTime;
    }

    public static void GetErrorReason(BackendReturnObject bro, out string statusCode, out string errorCode,
        out string message)
    {
        statusCode = bro.GetStatusCode();
        errorCode = bro.GetErrorCode();
        message = bro.GetMessage();
    }

    public static bool IsCompleteGuideQuest()
    {
        if (!ChartManager.GuideQuestCharts.TryGetValue(Managers.Game.UserData.ProgressGuideQuestId,
                out var guideQuestChart))
            return false;

        return Managers.Game.UserData.ProgressGuideQuestValue >= guideQuestChart.QuestCompleteValue;
    }

    public static bool IsAllClearGuideQuest()
    {
        return !ChartManager.GuideQuestCharts.ContainsKey(Managers.Game.UserData.ProgressGuideQuestId);
    }
}