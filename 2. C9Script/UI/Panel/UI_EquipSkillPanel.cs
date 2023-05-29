using System;
using GameData;
using TMPro;
using UI;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipSkillPanel : UI_Panel
{
    [Serializable]
    public struct SkillSlot
    {
        public Image SlotBackgroundImage;
        public Image BackgroundImage;
        public Image SkillIconImage;
        public Button SlotButton;
        public GameObject LockObj;
        public TMP_Text OpenLvText;
    }
    
    [SerializeField] private Button[] QuickSlotButtons;
    [SerializeField] private Button[] CloseButtons;

    [SerializeField] private SkillSlot[] SkillSlots;

    private int _clickSkillSlotIndex;
    private int _clickQuickSlotIndex;

    [HideInInspector]
    public int SelectSkillId;

    public bool isChangeEquipSkill;

    private Color _nonSelectColor = new Color(100 / 255f, 100 / 255f, 100 / 255f);

    public void Start()
    {
        for (int i = 0; i < SkillSlots.Length; i++)
        {
            int index = i;
            
            SkillSlots[i].SlotButton.BindEvent(() =>
            {
                _clickSkillSlotIndex = index;
                OnClickSkillSlot();
            });
        }

        for (int i = 0; i < QuickSlotButtons.Length; i++)
        {
            int index = i;

            if (i != _clickQuickSlotIndex)
                QuickSlotButtons[i].image.color = _nonSelectColor;
            else
                QuickSlotButtons[_clickQuickSlotIndex].image.color = Color.white;
            
            QuickSlotButtons[i].BindEvent(() =>
            {
                if (_clickQuickSlotIndex != index)
                    QuickSlotButtons[_clickQuickSlotIndex].image.color = _nonSelectColor;
                
                if (_clickQuickSlotIndex == index)
                    return;
                
                _clickQuickSlotIndex = index;
                QuickSlotButtons[_clickQuickSlotIndex].image.color = Color.white;
                
                OnClickQuickSlot();
            });
        }

        foreach (var closeButton in CloseButtons)
            closeButton.BindEvent(() => gameObject.SetActive(false));
    }

    public override void Open()
    {
        base.Open();

        SetSkillIcon();
    }

    private void SetSkillIcon()
    {
        for (int i = 0; i < Managers.Game.EquipSkillList[_clickQuickSlotIndex].Length; i++)
        {
            SetSkillIcon(i);
        }
    }

    private void SetSkillIcon(int index)
    {
        int equipSkillId = Managers.Game.EquipSkillList[_clickQuickSlotIndex][index].Value;

        int openLv;

        switch (index)
        {
            case 0:
                openLv = (int)ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_1].Value;
                break;
            case 1:
                openLv = (int)ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_2].Value;
                break;
            case 2:
                openLv = (int)ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_3].Value;
                break;
            case 3:
                openLv = (int)ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_4].Value;
                break;
            case 4:
                openLv = (int)ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_5].Value;
                break;
            default:
                openLv = 0;
                break;
        }

        if (!IsOpenSkillSlot(index))
        {
            SkillSlots[index].LockObj.SetActive(true);
            SkillSlots[index].OpenLvText.text = $"Lv.{openLv}";
        }
        else
            SkillSlots[index].LockObj.SetActive(false);

        if (!IsOpenSkillSlot(index) || equipSkillId == 0 || !ChartManager.SkillCharts.TryGetValue(equipSkillId, out var skillChart))
        {
            SkillSlots[index].BackgroundImage.sprite = Managers.Resource.LoadSkillSlotBg(Grade.Normal);
            SkillSlots[index].SlotBackgroundImage.sprite = Managers.Resource.LoadSkillSlot(Grade.Normal);
            SkillSlots[index].SkillIconImage.gameObject.SetActive(false);
        }
        else
        {
            SkillSlots[index].BackgroundImage.sprite = Managers.Resource.LoadSkillSlotBg(skillChart.Grade);
            SkillSlots[index].SlotBackgroundImage.sprite = Managers.Resource.LoadSkillSlot(skillChart.Grade);
            SkillSlots[index].SkillIconImage.gameObject.SetActive(true);
            SkillSlots[index].SkillIconImage.sprite = Managers.Resource.LoadSkillIcon(skillChart.Icon);
        }
    }

    private void OnClickSkillSlot()
    {
        if (!IsOpenSkillSlot(_clickSkillSlotIndex))
            return;
        
        if (Managers.Game.EquipSkillList[_clickQuickSlotIndex][_clickSkillSlotIndex].Value == SelectSkillId)
        {
            Managers.Game.EquipSkillList[_clickQuickSlotIndex][_clickSkillSlotIndex].Value = 0;
            SetSkillIcon(_clickSkillSlotIndex);
        }
        else
        {
            for (int i = 0; i < Managers.Game.EquipSkillList[_clickQuickSlotIndex].Length; i++)
            {
                if (Managers.Game.EquipSkillList[_clickQuickSlotIndex][i].Value != SelectSkillId)
                    continue;

                Managers.Game.EquipSkillList[_clickQuickSlotIndex][i].Value = 0;
                SetSkillIcon(i);
            }

            Managers.Game.EquipSkillList[_clickQuickSlotIndex][_clickSkillSlotIndex].Value = SelectSkillId;
            SetSkillIcon(_clickSkillSlotIndex);
            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.EquipSkill, SelectSkillId, 1));
        }

        isChangeEquipSkill = true;
    }

    private void OnClickQuickSlot()
    {
        SetSkillIcon();
    }

    private bool IsOpenSkillSlot(int slotIndex)
    {
        int openLv;

        switch (slotIndex)
        {
            case 0:
                openLv = (int)ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_1].Value;
                break;
            case 1:
                openLv = (int)ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_2].Value;
                break;
            case 2:
                openLv = (int)ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_3].Value;
                break;
            case 3:
                openLv = (int)ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_4].Value;
                break;
            case 4:
                openLv = (int)ChartManager.SystemCharts[SystemData.SkillSlot_OpenLevel_5].Value;
                break;
            default:
                openLv = 0;
                break;
        }

        return Managers.Game.UserData.Level >= openLv;
    }
}