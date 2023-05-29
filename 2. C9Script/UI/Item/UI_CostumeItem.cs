using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class UI_CostumeItem : UI_Base
{
    [SerializeField] private TMP_Text CostumeNameText;

    [SerializeField] private Image CostumeImage;
    [SerializeField] private Image GradeImage;

    [SerializeField] private Button CostumeButton;

    [SerializeField] private GameObject NonAcquireObj;
    [SerializeField] private GameObject SelectObj;
    
    public int CostumeId { get; private set; }

    public void Init(int costumeId, Action<UI_CostumeItem> buttonEvent)
    {
        CostumeId = costumeId;
        CostumeButton.BindEvent(() => buttonEvent?.Invoke(this));
        SetUI();
    }

    private void SetUI()
    {
        var costumeChart = ChartManager.CostumeCharts[CostumeId];

        CostumeNameText.text = ChartManager.GetString(costumeChart.Name);
        CostumeImage.sprite = Managers.Resource.LoadCostumeIcon(costumeChart.Icon);
        GradeImage.sprite = Managers.Resource.LoadItemGradeBg(costumeChart.Grade);
        
        Refresh();
    }

    public override void Refresh()
    {
        NonAcquireObj.SetActive(!Managers.Game.CostumeDatas[CostumeId].IsAcquired);
    }

    public void SetActiveSelect(bool isActive)
    {
        SelectObj.SetActive(isActive);
    }

    public Sprite GetCostumeSprite()
    {
        return CostumeImage.sprite;
    }
}