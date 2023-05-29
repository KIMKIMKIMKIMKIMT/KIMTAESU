using System;
using System.Linq;
using BackEnd;
using GameData;
using TMPro;
using UI;
using UI.Panel;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_CharacterInfoPanel : UI_Panel
{
    public enum TabType
    {
        Gold,
        StatPoint,
        UnlimitedPoint,
    }

    [Serializable]
    public record Tab
    {
        public TabType TabType;
        public Button TabButton;
        public GameObject SelectObj;
        public UI_Panel UIPanel;

        public void Off()
        {
            SelectObj.SetActive(false);
            UIPanel.Close();
        }

        public void On()
        {
            SelectObj.SetActive(true);
            UIPanel.Open();
        }
    }

    [Serializable]
    public record MultipleTab
    {
        public Button TabButton;
        public int MultipleValue;
        public GameObject SelectObj;

        public void Off()
        {
            SelectObj.SetActive(false);
        }

        public void On()
        {
            SelectObj.SetActive(true);
        }
    }

    #region SerializeField

    [SerializeField] private TMP_Text NicknameText;
    [SerializeField] private TMP_Text GuildText;
    [SerializeField] private TMP_Text GoldBarValueText;
    [SerializeField] private TMP_Text MileageValueText;
    [SerializeField] private TMP_Text AttackValueText;
    [SerializeField] private TMP_Text CriticalRate1Text;
    [SerializeField] private TMP_Text CriticalRate1ValueText;
    [SerializeField] private TMP_Text CriticalRate2Text;
    [SerializeField] private TMP_Text CriticalRate2ValueText;
    [SerializeField] private TMP_Text CriticalDamageValueText;
    [SerializeField] private TMP_Text AttackSpeedValueText;
    [SerializeField] private TMP_Text MoveSpeedValueText;
    [SerializeField] private TMP_Text HpValueText;
    [SerializeField] private TMP_Text DefenceValueText;
    [SerializeField] private TMP_Text ServerText;

    [SerializeField] private Image EquipWeaponImage;
    [SerializeField] private Image EquipPetImage;
    [SerializeField] private Image EquipCostumeImage;
    [SerializeField] private Image EquipWeaponGradeImage;
    [SerializeField] private Image EquipPetGradeImage;
    [SerializeField] private Image EquipCostumeGradeImage;
    [SerializeField] private Image EquipWeaponSubGradeImage;
    [SerializeField] private Image EquipPetSubGradeImage;
    [SerializeField] private Image EquipCostumeSubGradeImage;

    [SerializeField] private Button AutoEquipButton;
    [SerializeField] private Button ShowDetailStatButton;
    [SerializeField] private Button ChangeNicknameButton;

    [SerializeField] private UI_DetailStatPanel UIDetailStatPanel;

    [SerializeField] private Tab[] Tabs;
    [SerializeField] private MultipleTab[] MultipleTabs;

    [SerializeField] private UI_StatGoldUpgradePanel UIStatGoldUpgradePanel;
    [SerializeField] private UI_StatPointUpgradePanel UIStatPointUpgradePanel;
    [SerializeField] private UI_UnlimitedPointUpgradePanel UIUnlimitedPointUpgradePanel;
    [SerializeField] private UI_ChangeNicknamePanel UIChangeNicknamePanel;

    [SerializeField] private GameObject UnlimitedPointLockObj;
    [SerializeField] private TMP_Text UnlimitedPointLockGuideText;

    [SerializeField] private GameObject AutoEquipNavigationObj;

    [FormerlySerializedAs("UpgradeAttackObj")] [SerializeField]
    private GameObject UpgradeAttackNavigationObj;

    #endregion

    private Tab _currentTab;

    private Tab CurrentTab
    {
        get => _currentTab;
        set
        {
            _currentTab?.Off();
            _currentTab = value;
            _currentTab?.On();
        }
    }

    private MultipleTab _currentMultipleTab;

    private MultipleTab CurrentMultipleTab
    {
        get => _currentMultipleTab;
        set
        {
            _currentMultipleTab?.Off();
            _currentMultipleTab = value;
            _currentMultipleTab?.On();

            if (_currentMultipleTab != null)
                UIStatGoldUpgradePanel.Multiple = _currentMultipleTab.MultipleValue;

            if (_currentMultipleTab != null)
                UIStatPointUpgradePanel.Multiple = _currentMultipleTab.MultipleValue;

            if (_currentMultipleTab != null)
                UIUnlimitedPointUpgradePanel.Multiple = _currentMultipleTab.MultipleValue;
        }
    }

    private readonly CompositeDisposable _levelCheckComposite = new();

    private void Start()
    {
        SetButton();

        Managers.Game.GoodsDatas[(int)Goods.Mileage].Subscribe(mileage =>
        {
            MileageValueText.text = mileage.ToCurrencyString();
        });

        Managers.Game.GoodsDatas[(int)Goods.GoldBar].Subscribe(goldBar =>
        {
            GoldBarValueText.text = goldBar.ToCurrencyString();
        });

        UIChangeNicknamePanel.OnChangeNickname = () => { NicknameText.text = $"< {Backend.UserNickName} >"; };

        if (Managers.Game.UserData.Level <= ChartManager.SystemCharts[SystemData.StatPoint1_MaxLevel].Value)
        {
            UnlimitedPointLockObj.SetActive(true);
            UnlimitedPointLockGuideText.text
                = $"<color=#5842A2>{ChartManager.SystemCharts[SystemData.StatPoint1_MaxLevel].Value + 1}</color>레벨 달성 시 해금";
            Managers.Game.UserData.OnChangeLevel.Subscribe(level =>
            {
                if (!UnlimitedPointLockObj.activeSelf)
                    return;

                if (level <= ChartManager.SystemCharts[SystemData.StatPoint1_MaxLevel].Value)
                    return;

                UnlimitedPointLockObj.SetActive(false);
                _levelCheckComposite?.Clear();
            }).AddTo(_levelCheckComposite);
        }
        else
        {
            UnlimitedPointLockObj.SetActive(false);
        }

        ServerText.text = $"{Managers.Server.CurrentServer}서버";

        SetGuideEvent();
    }

    private void SetGuideEvent()
    {
        if (Utils.IsAllClearGuideQuest()) 
            return;
        
        var guideQuestComposite = new CompositeDisposable();
            
        Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigationUI).AddTo(guideQuestComposite);
        SetNavigationUI(Managers.Game.UserData.ProgressGuideQuestId);

        void SetNavigationUI(int guideQuestId)
        {
            if (Utils.IsAllClearGuideQuest())
            {
                guideQuestComposite.Clear();
                return;
            }
                
            AutoEquipNavigationObj.SetActive(guideQuestId == 6 || guideQuestId == 13 || guideQuestId == 15);
            UpgradeAttackNavigationObj.SetActive(guideQuestId == 2);
        }

        Managers.Game.UserData.OnChangeGuideQuestProgressValue.Subscribe(SetNavigationValue).AddTo(guideQuestComposite);
        SetNavigationValue(Managers.Game.UserData.ProgressGuideQuestValue);
            
        void SetNavigationValue(long value)
        {
            if (!Utils.IsCompleteGuideQuest())
                return;

            AutoEquipNavigationObj.SetActive(false);
            UpgradeAttackNavigationObj.SetActive(false);
        }
    }

    private void OnEnable()
    {
        Managers.Game.OnRefreshStat += SetStat;
    }

    private void OnDisable()
    {
        UIChangeNicknamePanel.Close();
        Managers.Game.OnRefreshStat -= SetStat;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIDetailStatPanel.gameObject.activeSelf)
            {
                UIDetailStatPanel.Close();
                return;
            }

            if (UIChangeNicknamePanel.gameObject.activeSelf)
            {
                UIChangeNicknamePanel.Close();
                return;
            }

            if (UIStatPointUpgradePanel.IsOnPanel)
            {
                UIStatPointUpgradePanel.CloseResetPanel();
                return;
            }

            if (UIUnlimitedPointUpgradePanel.IsOnPanel)
            {
                UIUnlimitedPointUpgradePanel.CloseResetPanel();
                return;
            }

            Managers.UI.ClosePopupUI();
        }
    }

    public override void Open()
    {
        base.Open();

        if (CurrentTab == null)
        {
            foreach (var tab in Tabs)
                tab.UIPanel.Close();
            CurrentTab = Tabs[0];
        }
        else
            CurrentTab.UIPanel.Open();

        if (CurrentMultipleTab == null)
        {
            foreach (var tab in MultipleTabs)
                tab.Off();

            CurrentMultipleTab = MultipleTabs[0];
        }

        Refresh();
    }

    public override void Refresh()
    {
        UIDetailStatPanel.Close();

        SetCharacter();
        SetStat();
        SetEquipInfo();
    }

    private void SetButton()
    {
        AutoEquipButton.BindEvent(OnClickAutoEquip);
        ShowDetailStatButton.BindEvent(OnClickShowDetailStat);
        ChangeNicknameButton.BindEvent(() => UIChangeNicknamePanel.Open());

        foreach (var tab in Tabs)
            tab.TabButton.BindEvent(() =>
            {
                if (tab.TabType == TabType.UnlimitedPoint)
                {
                    if (Managers.Game.UserData.Level <= ChartManager.SystemCharts[SystemData.StatPoint1_MaxLevel].Value)
                        return;
                }

                CurrentTab = tab;
            });

        foreach (var tab in MultipleTabs)
            tab.TabButton.BindEvent(() => CurrentMultipleTab = tab);
    }


    private void OnClickAutoEquip()
    {
        // 무기
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

        // 코스튬
        var acquiredCostumes =
            Managers.Game.CostumeDatas.Values.ToList().FindAll(costumeData => costumeData.IsAcquired);

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

        // 펫

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

        GameDataManager.EquipGameData.SaveGameData();
        Managers.Game.SetEquip();

        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EquipItem, (int)QuestEquipItemType.Weapon, 1));
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EquipItem, (int)QuestEquipItemType.Pet, 1));
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EquipItem, (int)QuestEquipItemType.Costume,
            1));

        Managers.Game.CalculateStat();
        Refresh();
    }

    private void OnClickShowDetailStat()
    {
        UIDetailStatPanel.Open();
    }

    private void SetCharacter()
    {
        Managers.Model.ResetPlayerModel();
        NicknameText.text = $"< {Backend.UserNickName} >";
        GuildText.text = Managers.Guild.GuildData != null ? $"<{Managers.Guild.GuildData.GuildName}>" : string.Empty;
    }

    private void SetStat()
    {
        AttackValueText.text =
            (Managers.Game.BaseStatDatas[(int)StatType.Attack] * Managers.Game.BaseStatDatas[(int)StatType.AttackPer])
            .ToCurrencyString();

        // 승급을 아예 안햇을때 - - 표기
        if (Managers.Game.UserData.PromoGrade == 0)
        {
            CriticalRate1Text.text = "-";
            CriticalRate2Text.text = "-";

            CriticalRate1ValueText.text = "-";
            CriticalRate2ValueText.text = "-";
        }
        else
        {
            Utils.GetReinforceCriticalRate(out var prevReinforceCriticalRateType,
                out var currentReinforceCriticalRateType);

            CriticalRate1Text.text = prevReinforceCriticalRateType.HasValue
                ? ChartManager.GetString(ChartManager.StatCharts[(int)prevReinforceCriticalRateType.Value].Name)
                : "-";
            CriticalRate1ValueText.text = prevReinforceCriticalRateType.HasValue
                ? $"{Managers.Game.BaseStatDatas[(int)prevReinforceCriticalRateType.Value] * 100:F2}%"
                : "-";

            CriticalRate2Text.text = currentReinforceCriticalRateType.HasValue
                ? ChartManager.GetString(ChartManager.StatCharts[(int)currentReinforceCriticalRateType.Value].Name)
                : "-";
            CriticalRate2ValueText.text = currentReinforceCriticalRateType.HasValue
                ? $"{Managers.Game.BaseStatDatas[(int)currentReinforceCriticalRateType.Value] * 100:F2}%"
                : "-";
        }

        CriticalDamageValueText.text = $"{(Managers.Game.IncreaseCriticalDamage * 100):F0}%";

        AttackSpeedValueText.text =
            $"{(Math.Round(Managers.Game.BaseStatDatas[(int)StatType.AttackSpeedPer], 7) * 100).ToCurrencyString()}%";
        MoveSpeedValueText.text =
            $"{(Math.Round(Managers.Game.BaseStatDatas[(int)StatType.MoveSpeedPer], 7) * 100).ToCurrencyString()}%";
        HpValueText.text =
            Math.Round(Managers.Game.BaseStatDatas[(int)StatType.Hp] * Managers.Game.BaseStatDatas[(int)StatType.HpPer],
                7).ToCurrencyString();
        DefenceValueText.text =
            Math.Round(
                Managers.Game.BaseStatDatas[(int)StatType.Defence] *
                Managers.Game.BaseStatDatas[(int)StatType.DefencePer], 7).ToCurrencyString();
    }

    private void SetEquipInfo()
    {
        var weaponChart = ChartManager.WeaponCharts[Managers.Game.EquipDatas[EquipType.Weapon]];
        EquipWeaponImage.sprite = Managers.Resource.LoadWeaponIcon(weaponChart.Icon);
        EquipWeaponGradeImage.sprite = Managers.Resource.LoadItemGradeBg(weaponChart.Grade);
        EquipWeaponSubGradeImage.gameObject.SetActive(weaponChart.Grade != Grade.Godgod);
        EquipWeaponSubGradeImage.sprite = Managers.Resource.LoadSubGradeIcon(weaponChart.SubGrade);

        var petChart = ChartManager.PetCharts[Managers.Game.EquipDatas[EquipType.Pet]];
        EquipPetImage.sprite = Managers.Resource.LoadPetIcon(petChart.Icon);
        EquipPetGradeImage.sprite = Managers.Resource.LoadItemGradeBg(petChart.Grade);
        EquipPetSubGradeImage.sprite = Managers.Resource.LoadSubGradeIcon(petChart.SubGrade);

        var costumeChart = ChartManager.CostumeCharts[Managers.Game.EquipDatas[EquipType.Costume]];
        EquipCostumeImage.sprite = Managers.Resource.LoadCostumeIcon(costumeChart.Icon);
        EquipCostumeGradeImage.sprite = Managers.Resource.LoadItemGradeBg(costumeChart.Grade);
        EquipCostumeSubGradeImage.gameObject.SetActive(false);
    }
}