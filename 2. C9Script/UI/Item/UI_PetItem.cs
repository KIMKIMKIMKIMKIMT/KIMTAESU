using System;
using Chart;
using GameData;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_PetItem : UI_Base
{
    [SerializeField] private TMP_Text NameText;
    [SerializeField] private TMP_Text CountText; 

    [SerializeField] private Image PetImage;
    [SerializeField] private Image GradeImage;

    [SerializeField] private Button PetButton;

    [SerializeField] private GameObject NonAcquireObj;
    [SerializeField] private GameObject SelectObj;

    [SerializeField] private GameObject[] SubGradeObjs;

    public int PetId { get; private set; }

    private PetChart _petChart;
    private PetData _petData;

    public void Init(int petId, Action<UI_PetItem> buttonEvent)
    {
        PetId = petId;
        
        _petChart = ChartManager.PetCharts[PetId];
        _petData = Managers.Game.PetDatas[PetId];
        
        PetButton.BindEvent(() => buttonEvent?.Invoke(this));
        SetUI();
    }

    private void SetUI()
    {
        NameText.text = ChartManager.GetString(_petChart.PetName);
        PetImage.sprite = Managers.Resource.LoadPetIcon(_petChart.Icon);
        GradeImage.sprite = Managers.Resource.LoadItemGradeBg(_petChart.Grade);
        for (var i = 0; i < SubGradeObjs.Length; i++)
            SubGradeObjs[i].SetActive(_petChart.SubGrade > i);
        
        Refresh();
    }

    public override void Refresh()
    {
        NonAcquireObj.SetActive(!_petData.IsAcquired);
        if (!_petData.IsAcquired)
            CountText.text = string.Empty;
        else if (ChartManager.PetCharts.ContainsKey(_petChart.CombineResult))
            CountText.text = $"{_petData.Quantity}/{_petChart.CombineCount}";
        else
            CountText.text = $"{_petData.Quantity}";
    }

    public void SetActiveSelect(bool isActive)
    {
        SelectObj.SetActive(isActive);
    }
}
