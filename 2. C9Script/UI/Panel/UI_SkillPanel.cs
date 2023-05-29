using System;
using Firebase;
using GameData;
using TMPro;
using UI;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillPanel : UI_Panel
{
    [Serializable]
    struct UISkill
    {
        public Button SkillButton;
        public GameObject CoolTimeObj;
        public Image SkillIconImage;
        public Image CoolTimeImage;
        public TMP_Text CoolTimeText;
        public Image BackgroundImage;
        public Image SlotBackgroundImage;
        public TMP_Text OpenLvText;
        public GameObject LockObj;
    }

    [SerializeField] private UISkill[] Skills;

    [SerializeField] private TMP_Text SkillPresetButtonText;

    [SerializeField] private Button SkillPresetButton;
    [SerializeField] private Button AutoSkillButton;
    [SerializeField] private Button FeverButton;
    [SerializeField] private Button AutoFeverButton;

    [SerializeField] private Image FeverGageImage;

    [SerializeField] private GameObject OnFeverObj;
    [SerializeField] private GameObject OffFeverObj;
    [SerializeField] private GameObject OnAutoFeverObj;
    [SerializeField] private GameObject OffAutoFeverObj;

    [SerializeField] private GameObject FeverLockObj;
    
    [SerializeField] private GameObject OnAutoSkillObj;
    [SerializeField] private GameObject OffAutoSkillObj;

    [SerializeField] private GameObject UseSkillNavigationObj;
    [SerializeField] private GameObject AutoSkillNavigationObj;
    
    private readonly Color _nonSkillSlotColor = new(99f / 255f, 96f / 255f, 92f / 255f);
    
    private void Start()
    {
        SetButtonEvent();
        SetPropertyEvent();
    }

    private void SetButtonEvent()
    {
        for (int i = 0; i < Skills.Length; i++)
        {
            int index = i;
            Skills[index].SkillButton.BindEvent(() =>
            {
                Managers.Game.MainPlayer.StartSkill(index, false);
            });
        }

        AutoSkillButton.BindEvent(() =>
        {
            Managers.Game.MainPlayer.AutoSkillMode.Value = !Managers.Game.MainPlayer.AutoSkillMode.Value;
            
            if (Managers.Game.MainPlayer.AutoSkillMode.Value)
                MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.UseAuto, (int)QuestUseAutoType.Skill, 1));
        });
        
        SkillPresetButton.BindEvent(() =>
        {
            Managers.Game.SkillQuickSlotIndex.Value =
                Managers.Game.SkillQuickSlotIndex.Value + 1 >= Managers.Game.EquipSkillList.Count
                    ? 0
                    : Managers.Game.SkillQuickSlotIndex.Value + 1;
        });
        
        FeverButton.BindEvent(() =>
        {
            Managers.Game.MainPlayer.EnableFeverMode();
        });

       AutoFeverButton.BindEvent(() =>
       {
           Managers.Game.MainPlayer.AutoFeverMode.Value = !Managers.Game.MainPlayer.AutoFeverMode.Value;
           
           if (Managers.Game.MainPlayer.AutoFeverMode.Value)
               MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.UseAuto, (int)QuestUseAutoType.Fever, 1));
       });
    }

    private void SetPropertyEvent()
    {
        for (int i = 0; i < Skills.Length; i++)
        {
            int index = i;
            
            Managers.Game.MainPlayer.SkillInfos[index].RemainSkillCoolTime.Subscribe(remainCoolTime =>
            {
                if (Managers.Game.MainPlayer.SkillInfos[index].SkillCoolTime == 0)
                    return;

                float ratio = remainCoolTime / Managers.Game.MainPlayer.SkillInfos[index].SkillCoolTime;
                Skills[index].CoolTimeImage.fillAmount = ratio;

                Skills[index].CoolTimeText.text = remainCoolTime.ToString("N1");
            });

            Managers.Game.MainPlayer.SkillInfos[index].IsCoolTime.Subscribe(isCoolTime =>
            {
                Skills[index].CoolTimeObj.SetActive(isCoolTime);
            });

            Managers.Game.SkillQuickSlotIndex.Subscribe(quickSlotIndex =>
            {
                SkillPresetButtonText.text = (quickSlotIndex + 1).ToString();
            });
        }

        Managers.Game.MainPlayer.AutoSkillMode.Subscribe(isAuto =>
        {
            OnAutoSkillObj.SetActive(isAuto);
            OffAutoSkillObj.SetActive(!isAuto);
        });

        for (int i = 0; i < Managers.Game.EquipSkillList.Count; i++)
        {
            for (int j = 0; j < Managers.Game.EquipSkillList[Managers.Game.SkillQuickSlotIndex.Value].Length; j++)
            {
                int quickSlotIndex = i;
                int skillSlotIndex = j;

                Managers.Game.EquipSkillList[quickSlotIndex][skillSlotIndex].Subscribe(_ =>
                {
                    if (quickSlotIndex != Managers.Game.SkillQuickSlotIndex.Value)
                        return;

                    SetSkillSlotIcon(skillSlotIndex);
                });
            }
        }

        Managers.Game.SkillQuickSlotIndex.Subscribe(_ =>
        {
            for (int i = 0; i < 5; i++)
            {
                SetSkillSlotIcon(i);
            }
        });
        
        Managers.Game.MainPlayer.FeverGage.Subscribe(value =>
        {
            FeverGageImage.fillAmount = value / 100f;
        });
        
        Managers.Game.MainPlayer.IsFeverMode.Subscribe(feverMode =>
        {
            OnFeverObj.SetActive(feverMode);
            OffFeverObj.SetActive(!feverMode);
        }).AddTo(gameObject);

        Managers.Game.MainPlayer.AutoFeverMode.Subscribe(isAuto =>
        {
            OnAutoFeverObj.SetActive(isAuto);
            OffAutoFeverObj.SetActive(!isAuto);
        }).AddTo(gameObject);

        Managers.Stage.State.Subscribe(state =>
        {
            FeverLockObj.SetActive(state != StageState.Normal && state != StageState.StageBoss);
            
            var level = Managers.Game.UserData.Level;

            Skills[0].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_1].Value ||
                                        Utils.IsWorldCupDungeon() || Utils.IsGuildAllRaidDungeon());
            Skills[1].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_2].Value ||
                                        Utils.IsWorldCupDungeon() || Utils.IsGuildAllRaidDungeon());
            Skills[2].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_3].Value ||
                                        Utils.IsWorldCupDungeon() || Utils.IsGuildAllRaidDungeon());
            Skills[3].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_4].Value ||
                                        Utils.IsWorldCupDungeon() || Utils.IsGuildAllRaidDungeon());
            Skills[4].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_5].Value ||
                                        Utils.IsWorldCupDungeon() || Utils.IsGuildAllRaidDungeon());
        }).AddTo(gameObject);

        {
            var level = Managers.Game.UserData.Level;
            
            Skills[0].OpenLvText.text = level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_1].Value
                ? $"Lv.{ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_1].Value:N0}"
                : string.Empty;
            
            //Skills[1].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_2].Value);
            Skills[1].OpenLvText.text = level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_2].Value ?
                $"Lv.{ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_2].Value:N0}" :
                string.Empty;
            
            //Skills[2].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_3].Value);
            Skills[2].OpenLvText.text = level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_3].Value ?
                $"Lv.{ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_3].Value:N0}" : 
                string.Empty;
            
            //Skills[3].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_4].Value);
            Skills[3].OpenLvText.text = level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_4].Value ?
                $"Lv.{ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_4].Value:N0}" : 
                string.Empty;
            
            //Skills[4].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_5].Value);
            Skills[4].OpenLvText.text = level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_5].Value ? 
                $"Lv.{ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_5].Value:N0}" :
                string.Empty;
        }

        Managers.Game.UserData.OnChangeLevel.Subscribe(level =>
        {
            Skills[0].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_1].Value);
            Skills[1].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_2].Value);
            Skills[2].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_3].Value);
            Skills[3].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_4].Value);
            Skills[4].LockObj.SetActive(level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_5].Value);

            Skills[0].OpenLvText.text = level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_1].Value
                ? $"Lv.{ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_1].Value:N0}"
                : string.Empty;

            Skills[1].OpenLvText.text = level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_2].Value
                ? $"Lv.{ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_2].Value:N0}"
                : string.Empty;

            Skills[2].OpenLvText.text = level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_3].Value
                ? $"Lv.{ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_3].Value:N0}"
                : string.Empty;

            Skills[3].OpenLvText.text = level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_4].Value
                ? $"Lv.{ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_4].Value:N0}"
                : string.Empty;

            Skills[4].OpenLvText.text = level < ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_5].Value
                ? $"Lv.{ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_5].Value:N0}"
                : string.Empty;
        }).AddTo(gameObject);

        SetGuideEvent();
    }

    private void SetGuideEvent()
    {
        if (Utils.IsAllClearGuideQuest())
            return;

        var guideComposite = new CompositeDisposable();
        
        Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigationId).AddTo(guideComposite);
        SetNavigationId(Managers.Game.UserData.ProgressGuideQuestId);

        Managers.Game.UserData.OnChangeGuideQuestProgressValue.Subscribe(SetNavigationValue).AddTo(guideComposite);
        SetNavigationValue(Managers.Game.UserData.ProgressGuideQuestValue);

        void SetNavigationId(int id)
        {
            if (Utils.IsAllClearGuideQuest())
            {
                guideComposite.Clear();
                return;
            }
            
            UseSkillNavigationObj.SetActive(id == 9);
            AutoSkillNavigationObj.SetActive(id == 10);
        }

        void SetNavigationValue(long value)
        {
            if (!Utils.IsCompleteGuideQuest())
                return;
            
            UseSkillNavigationObj.SetActive(false);
            AutoSkillNavigationObj.SetActive(false);
        }
    }

    private void SetSkillSlotIcon(int slotIndex)
    {
        int skillId = Managers.Game.EquipSkillList[Managers.Game.SkillQuickSlotIndex.Value][slotIndex].Value;
        
        if (ChartManager.SkillCharts.TryGetValue(skillId, out var skillChart))
        {
            Skills[slotIndex].SkillIconImage.color = Color.white;
            Skills[slotIndex].SkillIconImage.sprite =
                Managers.Resource.LoadSkillIcon(skillChart.Icon);
            Skills[slotIndex].SlotBackgroundImage.sprite = Managers.Resource.LoadSkillSlot(skillChart.Grade);
            Skills[slotIndex].BackgroundImage.sprite = Managers.Resource.LoadSkillSlotBg(skillChart.Grade);
        }
        else
        {
            Skills[slotIndex].SkillIconImage.color = _nonSkillSlotColor;
            Skills[slotIndex].SkillIconImage.sprite = null;
            Skills[slotIndex].SlotBackgroundImage.sprite = Managers.Resource.LoadSkillSlot(Grade.Normal);
            Skills[slotIndex].BackgroundImage.sprite = Managers.Resource.LoadSkillSlotBg(Grade.Normal);
        }
    }
}