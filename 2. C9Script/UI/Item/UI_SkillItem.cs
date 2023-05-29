using System;
using Chart;
using GameData;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillItem : UI_Base
{
    [SerializeField] private TMP_Text SkillNameText;
    [SerializeField] private TMP_Text SkillLevelText;
    [SerializeField] private TMP_Text PriceText; 
    [SerializeField] private TMP_Text ValueTypeText;
    [SerializeField] private TMP_Text SkillValueText;
    [SerializeField] private TMP_Text CoolTimeValueText;
    [SerializeField] private TMP_Text SkillDescText;
    [SerializeField] private TMP_Text OpenMaterialCountText;
    [SerializeField] private TMP_Text OpenButtonText;
    [SerializeField] private TMP_Text ReinforceButtonText;
    [SerializeField] private TMP_Text OpenMaterialNameText;

    [SerializeField] private Image IconImage;
    [SerializeField] private Image SlotImage;
    [SerializeField] private Image SlotBackgroundImage;
    [SerializeField] private Image ProgressImage;
    [SerializeField] private Image FullLevelImage;
    [SerializeField] private Image OpenMaterialImage;
    [SerializeField] private Image OpenMaterialGradeImage;
    [SerializeField] private Image SkillTypeImage;

    [SerializeField] private Button SkillButton;
    [SerializeField] private Button ReinforceButton;
    [SerializeField] private Button OpenButton;

    [SerializeField] private GameObject LockObj;
    [SerializeField] private GameObject CoolTimeObj;

    [SerializeField] private UpgradeEffect UpgradeEffect;

    [NonSerialized] public UI_SkillPopup UISkillPopup;
    
    public int SkillId { get; private set; }
    private readonly CompositeDisposable _compositeDisposable = new();
    
    private string SkillDamagePercentText => ((_skillChart.Value * 100 + Mathf.Max(0, _skillData.Level - 1) * _skillChart.IncreaseValue * 100) * ChartManager.SkillPolicyCharts[(SkillId, SkillPolicyProperty.Hit_Count)].Value).ToCurrencyString();

    private string SkillPassivePercentText
        => ((_skillChart.Value + Math.Max(0, _skillData.Level - 1) * _skillChart.IncreaseValue) * 100d)
            .ToCurrencyString();

    private bool IsMaxLevel => _skillData.Level >= _skillChart.MaxLevel;

    private Action<int> _itemCallback;

    private SkillChart _skillChart;
    private SkillData _skillData;
    public bool IsSave;

    public void Init(int skillId, Action<int> itemCallback)
    {
        SkillId = skillId;
        _itemCallback = itemCallback;
        
        ChartManager.SkillCharts.TryGetValue(skillId, out _skillChart);
        Managers.Game.SkillDatas.TryGetValue(SkillId, out _skillData);

        SetUI();
    }

    private void SetUI()
    {
        var icon = Managers.Resource.LoadSkillIcon(_skillChart.Icon);
        if (icon == null)
            icon = Managers.Resource.LoadSkillIcon("Skill_Icon_Dummy");

        IconImage.sprite = icon;
        SlotImage.sprite = Managers.Resource.LoadSkillSlot(_skillChart.Grade);
        SlotBackgroundImage.sprite = Managers.Resource.LoadSkillSlotBg(_skillChart.Grade);
        SkillNameText.text = ChartManager.GetString(_skillChart.Name);
        SkillDescText.text = ChartManager.GetString(_skillChart.Desc);
        CoolTimeValueText.text = $"{_skillChart.CoolTime}초";
        CoolTimeObj.SetActive(_skillChart.TabType == SkillTabType.Active);
        ValueTypeText.text = _skillChart.TabType == SkillTabType.Active ? "데미지 : " : "효과 : ";

        if (_skillChart.TabType == SkillTabType.Active)
        {
            SkillTypeImage.sprite =
                ChartManager.LabSkillCharts.TryGetValue(_skillChart.LabSkillType, out var labSkillChart)
                    ? Managers.Resource.LoadLabIcon(labSkillChart.Icon)
                    : null;
        }
        else if (_skillChart.TabType == SkillTabType.Passive)
        {
            SkillTypeImage.sprite = Managers.Resource.LoadLabIcon("Skilllab_Bg_015");
        }

        if (_skillData.Level > 0)
        {
            LockObj.SetActive(false);
            
            _compositeDisposable.Clear();
            _skillData.OnChangeLevel.Subscribe(_ => RefreshUI()).AddTo(_compositeDisposable);
            SkillButton.BindEvent(OnClickSkill);
        
            ReinforceButton.gameObject.SetActive(_skillData.Level > 0);
            ReinforceButton.BindEvent(OnClickReinforce);
            ReinforceButton.BindEvent(OnClickReinforce, UIEvent.Pressed);
        }
        else
        {
            LockObj.SetActive(true);
            OpenMaterialImage.sprite =
                Managers.Resource.LoadItemIcon(_skillChart.UnlockItemType, _skillChart.UnlockItemId);
            OpenMaterialCountText.text = $"{Utils.GetItemValue(_skillChart.UnlockItemType, _skillChart.UnlockItemId)}/{_skillChart.UnlockItemValue}";

            if (ChartManager.WeaponCharts.TryGetValue(_skillChart.UnlockItemId, out var weaponChart))
            {
                OpenMaterialGradeImage.sprite = Managers.Resource.LoadItemGradeBg(weaponChart.Grade);
                OpenMaterialNameText.text = ChartManager.GetString(weaponChart.Name);
            }

            if (Utils.IsEnoughItem(_skillChart.UnlockItemType, _skillChart.UnlockItemId, _skillChart.UnlockItemValue))
            {
                OpenButtonText.color = Color.white;
                OpenMaterialCountText.color = Color.white;
                OpenButtonText.text = "스킬해방";
                OpenButton.BindEvent(() =>
                {
                    if (!Utils.IsEnoughItem(_skillChart.UnlockItemType, _skillChart.UnlockItemId,
                            _skillChart.UnlockItemValue))
                    {
                        Managers.Message.ShowMessage("장비가 부족합니다");
                        return;
                    }
                    
                    Managers.Game.DecreaseItem(_skillChart.UnlockItemType, _skillChart.UnlockItemId, _skillChart.UnlockItemValue);
                    _skillData.Level = 1;
                    
                    GameDataManager.SaveItemData(_skillChart.UnlockItemType);
                    GameDataManager.SkillGameData.SaveGameData();
                    MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.OpenSkill, _skillChart.Id, 1));
                    IsSave = true;
                    if (_skillChart.TabType == SkillTabType.Passive)
                        Managers.Game.CalculateStat();
                    
                    UISkillPopup.OpenSkillLog.Add(_skillChart.Id);
                    
                    SetUI();
                });
            }
            else
            {
                OpenButton.ClearEvent();
                OpenButtonText.color = Color.red;
                OpenMaterialCountText.color = Color.red;
                OpenButtonText.text = "장비부족";
            }
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_skillChart.TabType == SkillTabType.Active)
        {
            if (Managers.Game.LabResearchDatas.TryGetValue(_skillChart.LabSkillType, out var labResearchData))
            {
                SkillValueText.text = 
                    ChartManager.LabSkillLevelCharts.TryGetValue(labResearchData.Level, out var labSkillLevelChart) ? 
                        $"{SkillDamagePercentText}%<color=#EBE444>(+{(labSkillLevelChart.IncreaseDamagePercent * 100 * ChartManager.SkillPolicyCharts[(SkillId, SkillPolicyProperty.Hit_Count)].Value).ToCurrencyString()}%)</color>" :
                        $"{SkillDamagePercentText}%";
            }
            else
                SkillValueText.text = $"{SkillDamagePercentText}%";
        }
        else
        {
            SkillValueText.text = _skillChart.TabType == SkillTabType.Active
                ? $"{SkillDamagePercentText}%"
                : $"{SkillPassivePercentText}%";
        }

        SkillLevelText.text = $"Lv.{_skillData.Level}";
        PriceText.text = IsMaxLevel ? 
            string.Empty : 
            $"{Utils.CalculateItemValue(_skillChart.LevelUpItemValue, _skillChart.LevelUpItemIncreaseValue, _skillData.Level).ToCurrencyString()}";
        ReinforceButtonText.text = IsMaxLevel ? "Max" : "강 화";

        if (IsMaxLevel)
        {
            ProgressImage.gameObject.SetActive(false);
            FullLevelImage.gameObject.SetActive(true);
        }
        else
        {
            ProgressImage.gameObject.SetActive(true);
            FullLevelImage.gameObject.SetActive(false);

            ProgressImage.fillAmount = 1f - (float)_skillData.Level / _skillChart.MaxLevel;
        }
    }

    private void OnClickSkill()
    {
        if (_skillChart.TabType == SkillTabType.Passive)
            return;
        
        _itemCallback?.Invoke(SkillId);
    }

    private void OnClickReinforce()
    {
        if (!Utils.IsEnoughItem(_skillChart.LevelUpItemType, _skillChart.LevelUpItemId, _skillChart.LevelUpItemValue,
                _skillChart.LevelUpItemIncreaseValue, _skillData.Level))
        {
            Managers.Message.ShowMessage(MessageType.LackReinforceMaterial);
            return;
        }

        if (IsMaxLevel)
            return;

        IsSave = true;

        var price = Utils.CalculateItemValue(_skillChart.LevelUpItemValue, _skillChart.LevelUpItemIncreaseValue,
            _skillData.Level);
        
        UpgradeEffect.Play();
        Managers.Game.DecreaseItem(_skillChart.LevelUpItemType, _skillChart.LevelUpItemId, price);
        _skillData.Level++;
        RefreshUI();

        if (UISkillPopup.UpgradeSkillLogs.ContainsKey(_skillChart.Id))
        {
            UISkillPopup.UpgradeSkillLogs[_skillChart.Id].IncreaseLv += 1;
            UISkillPopup.UpgradeSkillLogs[_skillChart.Id].UsingSkillGem += price;
        }
        else
        {
            UISkillPopup.UpgradeSkillLogs.Add(_skillChart.Id, new UpgradeSkillLog());
            UISkillPopup.UpgradeSkillLogs[_skillChart.Id].IncreaseLv = 1;
            UISkillPopup.UpgradeSkillLogs[_skillChart.Id].UsingSkillGem = price;
        }

        if (_skillChart.TabType == SkillTabType.Passive)
            Managers.Game.CalculateStat();
    }
}