using System;
using System.Linq;
using GameData;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatGoldUpgradePanel : UI_Panel
{
    [Serializable]
    public struct UpgradeStatItem
    {
        public TMP_Text MaxLevelText;
        public TMP_Text LevelText;
        public TMP_Text StatDescText;
        public TMP_Text ReinforcePriceText;

        public Image StatIconImage;
        public Image PromoGradeImage;

        public Button ReinforceButton;

        public GameObject LockObj;
        public GameObject ReinforceLockObj;

        public UpgradeEffect UpgradeEffect;
    }
    
    [SerializeField] private UpgradeStatItem AttackUpgradeItem;
    [SerializeField] private UpgradeStatItem CriticalRateUpgradeItem;
    [SerializeField] private UpgradeStatItem CriticalDamageUpgradeItem;

    [SerializeField] private ScrollRect ScrollRect;

    private bool _saveFlag;

    private int _multiple = 1;

    public int Multiple
    {
        get => _multiple;
        set
        {
            _multiple = value;
            _compositeDisposable.Clear();
            SetAttackUpgradeUI();
            SetCriticalUpgradeUI(StatType.CriticalRate2, StatType.CriticalRate9009,
                StatType.CriticalRate10009, StatType.CriticalRate14009, ref CriticalRateUpgradeItem);
            SetCriticalUpgradeUI(StatType.CriticalDamage2, StatType.CriticalDamage9009, 
                StatType.CriticalDamage10009, StatType.CriticalDamage14009, ref CriticalDamageUpgradeItem);
        }
    }

    private readonly CompositeDisposable _compositeDisposable = new();


    private void Start()
    {
        AttackUpgradeItem.ReinforceButton.SetScrollRect(ScrollRect);
        AttackUpgradeItem.ReinforceButton.BindEvent(OnClickReinforceAttack);
        AttackUpgradeItem.ReinforceButton.BindEvent(OnClickReinforceAttack, UIEvent.Pressed);

        CriticalRateUpgradeItem.ReinforceButton.SetScrollRect(ScrollRect);
        CriticalRateUpgradeItem.ReinforceButton.BindEvent(OnClickReinforceCriticalRate);
        CriticalRateUpgradeItem.ReinforceButton.BindEvent(OnClickReinforceCriticalRate, UIEvent.Pressed);

        CriticalDamageUpgradeItem.ReinforceButton.SetScrollRect(ScrollRect);
        CriticalDamageUpgradeItem.ReinforceButton.BindEvent(OnClickReinforceCriticalDamage);
        CriticalDamageUpgradeItem.ReinforceButton.BindEvent(OnClickReinforceCriticalDamage, UIEvent.Pressed);

        Managers.Game.GoodsDatas[(int)Goods.Gold].Subscribe(gold =>
        {
            AttackUpgradeItem.ReinforcePriceText.color = Utils.IsEnoughItem(ItemType.Goods, (int)Goods.Gold,
                Utils.GetStatGoldUpgradePrice((int)StatType.Attack, Multiple))
                ? Color.white
                : Color.red;
        });
    }
    
    public override void Open()
    {
        base.Open();

        _compositeDisposable.Clear();
        
        SetAttackUpgradeUI();
        SetCriticalUpgradeUI(StatType.CriticalRate2, StatType.CriticalRate9009,
            StatType.CriticalRate10009, StatType.CriticalRate14009,ref CriticalRateUpgradeItem);
        SetCriticalUpgradeUI(StatType.CriticalDamage2, StatType.CriticalDamage9009, 
            StatType.CriticalDamage10009, StatType.CriticalDamage14009, ref CriticalDamageUpgradeItem);
    }

    private void OnDisable()
    {
        Save();
    }

    private void SetAttackUpgradeUI()
    {
        long statLevel = Managers.Game.StatLevelDatas[(int)StatType.Attack].Value;
        var statChart = ChartManager.StatCharts[(int)StatType.Attack];
        var statUpgradeChart = ChartManager.StatGoldUpgradeCharts[(int)StatType.Attack];

        AttackUpgradeItem.LevelText.text = $"Lv.{statLevel}";
        AttackUpgradeItem.StatDescText.text =
            $"{ChartManager.GetString(statChart.Name)} {Utils.CalculateGoldUpgradeStat(statChart.Id, statLevel).ToCurrencyString()} 상승";

        AttackUpgradeItem.MaxLevelText.text = $"(Max Lv.{statUpgradeChart.MaxLevel})";
        AttackUpgradeItem.ReinforcePriceText.text = statLevel >= statUpgradeChart.MaxLevel ? "Max" : (Utils.GetStatGoldUpgradePrice((int)StatType.Attack, Multiple)).ToCurrencyString();
        AttackUpgradeItem.ReinforcePriceText.color = Utils.IsEnoughItem(ItemType.Goods, (int)Goods.Gold, Utils.GetStatGoldUpgradePrice((int)StatType.Attack, Multiple))
            ? Color.white
            : Color.red;

        AttackUpgradeItem.ReinforceLockObj.SetActive(false);
    }


    void SetCriticalUpgradeUI(StatType startStat, StatType endStat, StatType startStat2, StatType endStat2, ref UpgradeStatItem upgradeStatItem)
    {
        int promoMaxGrade = (int)ChartManager.SystemCharts[SystemData.MaxPromoLevel].Value;

        bool isNextCheck = false;

        for (StatType statType = startStat; statType <= endStat; statType++)
        {
            var statUpgradeChart = ChartManager.StatGoldUpgradeCharts[(int)statType];
            var maxLevel = statUpgradeChart.MaxLevel;

            int needPromoGrade = GetPromoGrade(statType);

            // 최대 레벨인지
            if (Managers.Game.StatLevelDatas[(int)statType].Value >= maxLevel)
            {
                if (needPromoGrade >= promoMaxGrade)
                {
                    // 데이터상 최대치를 찍은 스탯
                    upgradeStatItem.LevelText.text = "Lv.Max";
                    upgradeStatItem.MaxLevelText.text = $"(Max Lv.{maxLevel})";
                    upgradeStatItem.StatIconImage.sprite =
                        Managers.Resource.LoadStatIcon(ChartManager.StatCharts[(int)statType].Icon);
                    upgradeStatItem.StatDescText.text =
                        $"{ChartManager.GetString(ChartManager.StatCharts[(int)statType].Name)} {(Utils.CalculateGoldUpgradeStat(ChartManager.StatCharts[(int)statType].Id, Managers.Game.StatLevelDatas[(int)statType].Value) * 100).ToCurrencyString()}% 상승";
                    upgradeStatItem.LockObj.SetActive(false);
                    upgradeStatItem.ReinforceLockObj.SetActive(false);
                    upgradeStatItem.ReinforcePriceText.text = "-";
                    isNextCheck = true;
                    break;
                }
                
                // 최대 등급
                if (Managers.Game.UserData.PromoGrade <= promoMaxGrade)
                {
                    if (statType == endStat)
                        isNextCheck = true;
                    
                    continue;
                }

                // 데이터상 최대치를 찍은 스탯
                upgradeStatItem.LevelText.text = "Lv.Max";
                upgradeStatItem.MaxLevelText.text = $"(Max Lv.{maxLevel})";
                upgradeStatItem.StatIconImage.sprite =
                    Managers.Resource.LoadStatIcon(ChartManager.StatCharts[(int)statType].Icon);
                upgradeStatItem.StatDescText.text =
                    $"{ChartManager.GetString(ChartManager.StatCharts[(int)statType].Name)} {(Utils.CalculateGoldUpgradeStat(ChartManager.StatCharts[(int)statType].Id, Managers.Game.StatLevelDatas[(int)statType].Value) * 100).ToCurrencyString()}% 상승";
                upgradeStatItem.ReinforceLockObj.SetActive(true);
                upgradeStatItem.ReinforcePriceText.text = "-";
                isNextCheck = true;
                break;
            }

            // 승급 단계 부족
            if (Managers.Game.UserData.PromoGrade < needPromoGrade)
            {
                upgradeStatItem.LockObj.SetActive(true);
                upgradeStatItem.ReinforceButton.gameObject.SetActive(false);
                upgradeStatItem.ReinforceLockObj.SetActive(true);
                upgradeStatItem.ReinforcePriceText.text = "-";
                for (int i = Managers.Game.UserData.PromoGrade + 1; i <= ChartManager.SystemCharts[SystemData.MaxPromoLevel].Value; i++)
                {
                    if (!ChartManager.PromoDungeonCharts.TryGetValue(i, out var promoDungeonChart))
                        continue;

                    if (promoDungeonChart.ClearRewardStat2Id == (int)StatType.None)
                        continue;

                    upgradeStatItem.PromoGradeImage.sprite = Managers.Resource.LoadPromoIcon(i);

                    if (startStat == StatType.CriticalRate2)
                    {
                        if (ChartManager.StatCharts.TryGetValue(promoDungeonChart.ClearRewardStat2Id,
                                out var statChart))
                            upgradeStatItem.StatIconImage.sprite = Managers.Resource.LoadStatIcon(statChart.Icon);
                    }
                    
                    if (startStat == StatType.CriticalDamage2)
                    {
                        if (ChartManager.StatCharts.TryGetValue(promoDungeonChart.ClearRewardStat3Id,
                                out var statChart))
                            upgradeStatItem.StatIconImage.sprite = Managers.Resource.LoadStatIcon(statChart.Icon);
                    }

                    break;
                }
            }
            // 강화 가능
            else
            {
                upgradeStatItem.LockObj.SetActive(false);
                upgradeStatItem.StatIconImage.sprite =
                    Managers.Resource.LoadStatIcon(ChartManager.StatCharts[(int)statType].Icon);
                upgradeStatItem.ReinforceButton.gameObject.SetActive(true);
                upgradeStatItem.ReinforceLockObj.SetActive(false);
                upgradeStatItem.ReinforcePriceText.text = (Utils.GetStatGoldUpgradePrice((int)statType, Multiple)).ToCurrencyString();
                upgradeStatItem.ReinforcePriceText.color = Utils.IsEnoughItem(ItemType.Goods, (int)Goods.Gold,
                    Utils.GetStatGoldUpgradePrice((int)statType, Multiple))
                    ? Color.white
                    : Color.red;
            }

            string statName = ChartManager.GetString(ChartManager.StatCharts[(int)statType].Name);

            upgradeStatItem.StatDescText.text =
                $"{statName} {(Utils.CalculateGoldUpgradeStat(ChartManager.StatCharts[(int)statType].Id, Managers.Game.StatLevelDatas[(int)statType].Value) * 100).ToCurrencyString()}% 상승";
            upgradeStatItem.LevelText.text = $"Lv.{Managers.Game.StatLevelDatas[(int)statType].Value}";
            upgradeStatItem.MaxLevelText.text = $"(Max Lv.{maxLevel})";

            break;
        }

        if (!isNextCheck)
            return;
        
        for (StatType statType = startStat2; statType <= endStat2; statType++)
        {
            var statUpgradeChart = ChartManager.StatGoldUpgradeCharts[(int)statType];
            var maxLevel = statUpgradeChart.MaxLevel;

            int needPromoGrade = GetPromoGrade(statType);

            // 최대 레벨인지
            if (Managers.Game.StatLevelDatas[(int)statType].Value >= maxLevel)
            {
                if (needPromoGrade >= promoMaxGrade)
                {
                    // 데이터상 최대치를 찍은 스탯
                    upgradeStatItem.LevelText.text = "Lv.Max";
                    upgradeStatItem.MaxLevelText.text = $"(Max Lv.{maxLevel})";
                    upgradeStatItem.StatIconImage.sprite =
                        Managers.Resource.LoadStatIcon(ChartManager.StatCharts[(int)statType].Icon);
                    upgradeStatItem.StatDescText.text =
                        $"{ChartManager.GetString(ChartManager.StatCharts[(int)statType].Name)} {(Utils.CalculateGoldUpgradeStat(ChartManager.StatCharts[(int)statType].Id, Managers.Game.StatLevelDatas[(int)statType].Value) * 100).ToCurrencyString()}% 상승";
                    upgradeStatItem.LockObj.SetActive(false);
                    upgradeStatItem.ReinforceLockObj.SetActive(false);
                    upgradeStatItem.ReinforcePriceText.text = "-";
                    break;
                }
                
                // 최대 등급
                if (Managers.Game.UserData.PromoGrade <= promoMaxGrade)
                    continue;

                // 데이터상 최대치를 찍은 스탯
                upgradeStatItem.LevelText.text = "Lv.Max";
                upgradeStatItem.MaxLevelText.text = $"(Max Lv.{maxLevel})";
                upgradeStatItem.StatIconImage.sprite =
                    Managers.Resource.LoadStatIcon(ChartManager.StatCharts[(int)statType].Icon);
                upgradeStatItem.StatDescText.text =
                    $"{ChartManager.GetString(ChartManager.StatCharts[(int)statType].Name)} {(Utils.CalculateGoldUpgradeStat(ChartManager.StatCharts[(int)statType].Id, Managers.Game.StatLevelDatas[(int)statType].Value) * 100).ToCurrencyString()}% 상승";
                upgradeStatItem.ReinforceLockObj.SetActive(true);
                upgradeStatItem.ReinforcePriceText.text = "-";
                break;
            }

            // 승급 단계 부족
            if (Managers.Game.UserData.PromoGrade < needPromoGrade)
            {
                upgradeStatItem.LockObj.SetActive(true);
                upgradeStatItem.ReinforceButton.gameObject.SetActive(false);
                upgradeStatItem.ReinforceLockObj.SetActive(true);
                upgradeStatItem.ReinforcePriceText.text = "-";
                for (int i = Managers.Game.UserData.PromoGrade + 1; i <= ChartManager.SystemCharts[SystemData.MaxPromoLevel].Value; i++)
                {
                    if (!ChartManager.PromoDungeonCharts.TryGetValue(i, out var promoDungeonChart))
                        continue;

                    if (promoDungeonChart.ClearRewardStat2Id == (int)StatType.None)
                        continue;

                    upgradeStatItem.PromoGradeImage.sprite = Managers.Resource.LoadPromoIcon(i);

                    if (startStat == StatType.CriticalRate2)
                    {
                        if (ChartManager.StatCharts.TryGetValue(promoDungeonChart.ClearRewardStat2Id,
                                out var statChart))
                            upgradeStatItem.StatIconImage.sprite = Managers.Resource.LoadStatIcon(statChart.Icon);
                    }
                    
                    if (startStat == StatType.CriticalDamage2)
                    {
                        if (ChartManager.StatCharts.TryGetValue(promoDungeonChart.ClearRewardStat3Id,
                                out var statChart))
                            upgradeStatItem.StatIconImage.sprite = Managers.Resource.LoadStatIcon(statChart.Icon);
                    }

                    break;
                }
            }
            // 강화 가능
            else
            {
                upgradeStatItem.LockObj.SetActive(false);
                upgradeStatItem.StatIconImage.sprite =
                    Managers.Resource.LoadStatIcon(ChartManager.StatCharts[(int)statType].Icon);
                upgradeStatItem.ReinforceButton.gameObject.SetActive(true);
                upgradeStatItem.ReinforceLockObj.SetActive(false);
                upgradeStatItem.ReinforcePriceText.text = (Utils.GetStatGoldUpgradePrice((int)statType, Multiple)).ToCurrencyString();
                upgradeStatItem.ReinforcePriceText.color = Utils.IsEnoughItem(ItemType.Goods, (int)Goods.Gold,
                    Utils.GetStatGoldUpgradePrice((int)statType, Multiple))
                    ? Color.white
                    : Color.red;
            }

            string statName = ChartManager.GetString(ChartManager.StatCharts[(int)statType].Name);

            upgradeStatItem.StatDescText.text =
                $"{statName} {(Utils.CalculateGoldUpgradeStat(ChartManager.StatCharts[(int)statType].Id, Managers.Game.StatLevelDatas[(int)statType].Value) * 100).ToCurrencyString()}% 상승";
            upgradeStatItem.LevelText.text = $"Lv.{Managers.Game.StatLevelDatas[(int)statType].Value}";
            upgradeStatItem.MaxLevelText.text = $"(Max Lv.{maxLevel})";

            break;
        }
    }
    
    private int GetPromoGrade(StatType statType)
    {
        return ChartManager.PromoDungeonCharts.Values.ToList().Find(chartData =>
            chartData.ClearRewardStat2Id == (int)statType || chartData.ClearRewardStat3Id == (int)statType).Id;
    }

    private void ReinforceStat(StatType statType, Action successCallback = null)
    {
        var price = Utils.GetStatGoldUpgradePrice((int)statType, Multiple);

        if (!Utils.IsEnoughItem(ItemType.Goods, (int)Goods.Gold, price))
        {
            Managers.Message.ShowMessage(MessageType.LackReinforceMaterial);
            return;
        }

        if (Managers.Game.StatLevelDatas[(int)statType].Value + Multiple > ChartManager.StatGoldUpgradeCharts[(int)statType].MaxLevel)
            return;

        Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.Gold, price);
        Managers.Game.StatLevelDatas[(int)statType].Value += Multiple;
        Managers.Game.CalculateStat();
        successCallback?.Invoke();

        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.UpgradeStat, (int)QuestUpgradeStatType.GoldReinforce, Multiple));
        
        _saveFlag = true;
    }
    
    private StatType? GetCurrentReinforceCriticalRate()
    {
        for (int promoId = 1; promoId <= ChartManager.SystemCharts[SystemData.MaxPromoLevel].Value; promoId++)
        {
            if (Managers.Game.UserData.PromoGrade < promoId)
                break;

            var statId = ChartManager.PromoDungeonCharts[promoId].ClearRewardStat2Id;
            if (statId == 0)
                continue;
            
            var maxLevel = ChartManager.StatGoldUpgradeCharts[statId].MaxLevel;
            
            if (Managers.Game.StatLevelDatas[statId].Value >= maxLevel)
                continue;
            
            if (Managers.Game.StatLevelDatas[statId].Value < maxLevel)
            {
                return (StatType)statId.GetDecrypted();
            }
        }

        return null;
    }
    
    private StatType? GetCurrentReinforceCriticalDamage()
    {
        for (int promoId = 1; promoId <= ChartManager.SystemCharts[SystemData.MaxPromoLevel].Value; promoId++)
        {
            if (Managers.Game.UserData.PromoGrade < promoId)
                break;

            var statId = ChartManager.PromoDungeonCharts[promoId].ClearRewardStat3Id;
            if (statId == 0)
                continue;
            
            var maxLevel = ChartManager.StatGoldUpgradeCharts[statId].MaxLevel;
            
            if (Managers.Game.StatLevelDatas[statId].Value >= maxLevel)
                continue;
            
            if (Managers.Game.StatLevelDatas[statId].Value < maxLevel)
            {
                return (StatType)statId.GetDecrypted();
            }
        }

        return null;
    }
    
    private void OnClickReinforceAttack()
    {
        ReinforceStat(StatType.Attack, () =>
        {
            AttackUpgradeItem.UpgradeEffect.Play();
            SetAttackUpgradeUI();
        });
    }

    private void OnClickReinforceCriticalRate()
    {
        var statType = GetCurrentReinforceCriticalRate();
        if (!statType.HasValue)
            return;
        
        ReinforceStat(statType.Value, 
            () =>
            {
                CriticalRateUpgradeItem.UpgradeEffect.Play();
                SetCriticalUpgradeUI(StatType.CriticalRate2, StatType.CriticalRate9009,
                    StatType.CriticalRate10009, StatType.CriticalRate14009, ref CriticalRateUpgradeItem);
            });
    }

    private void OnClickReinforceCriticalDamage()
    {
        var statType = GetCurrentReinforceCriticalDamage();
        if (!statType.HasValue)
            return;
        ReinforceStat(statType.Value,
            () =>
            {
                CriticalDamageUpgradeItem.UpgradeEffect.Play();
                SetCriticalUpgradeUI(StatType.CriticalDamage2, StatType.CriticalDamage9009,
                    StatType.CriticalDamage10009, StatType.CriticalDamage14009, ref CriticalDamageUpgradeItem);
            });
    }
    
    private void Save()
    {
        if (!_saveFlag)
            return;

        _saveFlag = false;
        GameDataManager.StatLevelGameData.SaveGameData();
        GameDataManager.GoodsGameData.SaveGameData();
    }
}