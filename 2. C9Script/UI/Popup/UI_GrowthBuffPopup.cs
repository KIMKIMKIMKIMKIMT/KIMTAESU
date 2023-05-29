using System;
using System.Collections.Generic;
using Chart;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_GrowthBuffPopup : UI_Popup
{
    [Serializable]
    public record BuffItem
    {
        public GameObject Obj;
        public Image StatImage;
        public TMP_Text StatText;
        public TMP_Text ValueText;
    }

    [SerializeField] private TMP_Text BuffNameText;
    [SerializeField] private TMP_Text LevelText;

    [SerializeField] private Button PrevButton;
    [SerializeField] private Button NextButton;
    [SerializeField] private Button CloseButton;

    [SerializeField] private BuffItem[] BuffItems;

    private int _currentId;

    private int CurrentId
    {
        get => _currentId;
        set
        {
            _currentId = value;
            SetUI();
        }
    }

    private void Start()
    {
        PrevButton.BindEvent(OnClickPrev);
        NextButton.BindEvent(OnClickNext);
        CloseButton.BindEvent(ClosePopup);

        CurrentId = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI();
        }
    }

    private void SetUI()
    {
        if (!ChartManager.GrowthBuffCharts.TryGetValue(CurrentId, out var growthBuffChart))
            return;

        BuffNameText.text = ChartManager.GetString(growthBuffChart.BuffName);

        int prevLv = ChartManager.GrowthBuffCharts.TryGetValue(CurrentId - 1, out var prevGrowthBuffChart)
            ? prevGrowthBuffChart.ApplyMaxLv + 1
            : 1;

        LevelText.text = $"Lv.{prevLv}~{growthBuffChart.ApplyMaxLv}";

        PrevButton.gameObject.SetActive(ChartManager.GrowthBuffCharts.ContainsKey(CurrentId - 1));
        NextButton.gameObject.SetActive(ChartManager.GrowthBuffCharts.ContainsKey(CurrentId + 1));

        for (int i = 0; i < 3; i++)
        {
            if (BuffItems.Length < i)
                return;

            if (growthBuffChart.BuffStatIds.Length <= i)
                BuffItems[i].Obj.SetActive(false);
            else
            {
                BuffItems[i].Obj.SetActive(true);

                if (ChartManager.StatCharts.TryGetValue(growthBuffChart.BuffStatIds[i], out var statChart))
                {
                    BuffItems[i].StatImage.sprite = Managers.Resource.LoadStatIcon(statChart.Icon);
                    BuffItems[i].StatText.text = ChartManager.GetString(statChart.Name);
                }
                else
                {
                    BuffItems[i].StatImage.sprite = null;
                    BuffItems[i].StatText.text = string.Empty;
                }

                BuffItems[i].ValueText.text = $"+{(growthBuffChart.BuffStatValues[i] * 100).ToCurrencyString()}%";
            }
        }
    }

    private void OnClickPrev()
    {
        if (!ChartManager.GrowthBuffCharts.ContainsKey(CurrentId - 1))
            return;

        CurrentId -= 1;
    }

    private void OnClickNext()
    {
        if (!ChartManager.GrowthBuffCharts.ContainsKey(CurrentId + 1))
            return;

        CurrentId += 1;
    }
}