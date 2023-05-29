using System;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LabAwakeningItem : UI_Base
{
    [SerializeField] private TMP_Text GradeText;
    [SerializeField] private TMP_Text EffectText;
    [SerializeField] private TMP_Text OpenText;
    
    [SerializeField] private Image PatternImage;
    [SerializeField] private Image BgImage;

    [SerializeField] private Toggle LockToggle;

    [SerializeField] private GameObject CoverObj;

    [SerializeField] private Sprite[] BgSprites;

    private int _awakeningId;
    private bool _isOpen;
    private LabAwakeningData _labAwakeningData;
    private Action _lockCallback;

    private void Start()
    {
        LockToggle.onValueChanged.AddListener(isOn =>
        {
            _labAwakeningData.IsLock = isOn;
            _lockCallback?.Invoke();
        });
    }

    public void Init(int awakeningId, bool isOpen, Action lockCallback)
    {
        _awakeningId = awakeningId;
        _isOpen = isOpen;
        _lockCallback = lockCallback;
        
        if (!Managers.Game.LabAwakeningDatas.TryGetValue(Managers.Game.LabEquipPresetId, out var labAwakeningDatas))
            return;
        
        if (!labAwakeningDatas.TryGetValue(_awakeningId, out _labAwakeningData))
            return;
        
        SetUI();
    }

    private void SetUI()
    {
        CoverObj.SetActive(!_isOpen);

        if (_isOpen && _labAwakeningData.StatId != 0)
        {
            if (!ChartManager.LabPatternCharts.TryGetValue(_labAwakeningData.PatternId, out var labPatternChart))
                return;

            if (!ChartManager.StatCharts.TryGetValue(_labAwakeningData.StatId, out var statChart))
                return;

            PatternImage.gameObject.SetActive(true);
            PatternImage.sprite = Managers.Resource.LoadLabIcon(labPatternChart.Icon);
            GradeText.text = $"[{ChartManager.GetString(_labAwakeningData.Grade.ToString())}]";
            EffectText.text =
                statChart.ValueType == ValueType.Percent
                    ? $"{ChartManager.GetString(statChart.Name)} {(_labAwakeningData.StatValue * 100).ToCurrencyString()}%"
                    : $"{ChartManager.GetString(statChart.Name)} {_labAwakeningData.StatValue.ToCurrencyString()}";
            LockToggle.isOn = _labAwakeningData.IsLock;

            Sprite bgSprite = null;

            switch (_labAwakeningData.Grade)
            {
                case Grade.Normal:
                    bgSprite = BgSprites[0];
                    break;
                case Grade.Rare:
                    bgSprite = BgSprites[1];
                    break;
                case Grade.Unique:
                    bgSprite = BgSprites[2];
                    break;
                case Grade.Legend:
                    bgSprite = BgSprites[3];
                    break;
                case Grade.Legeno:
                    bgSprite = BgSprites[4];
                    break;
            }

            BgImage.sprite = bgSprite;
        }
        else
        {
            if (!ChartManager.LabAwakeningCharts.TryGetValue(_awakeningId, out var labAwakeningChart))
                return;
            
            OpenText.text = $"연구 {labAwakeningChart.OpenLv}레벨 해금";
            GradeText.text = "-";
            EffectText.text = "-";
            PatternImage.gameObject.SetActive(false);
            LockToggle.isOn = false;
            BgImage.sprite = BgSprites[0];
        }
    }
}