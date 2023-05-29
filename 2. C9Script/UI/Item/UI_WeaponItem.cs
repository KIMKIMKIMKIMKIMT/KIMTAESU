using System;
using Chart;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponItem : UI_Base
{
    [SerializeField] private Button WeaponButton;

    [SerializeField] private TMP_Text WeaponInfoText;
    
    [SerializeField] private Image WeaponImage;
    [SerializeField] private Image GradeImage;
    [SerializeField] private Image SubGradeImage;
    [SerializeField] private Image BackGroundImage;

    [SerializeField] private GameObject SelectObj;
    [SerializeField] private GameObject NoAcquiredObj;

    public int ItemIndex { get; set; }

    private readonly CompositeDisposable _composite = new CompositeDisposable();

    public void SetItem(int itemIndex, Action<UI_WeaponItem> callback)
    {
        ItemIndex = itemIndex;
        
        var weaponChart = ChartManager.WeaponCharts[ItemIndex];
        
        WeaponImage.sprite = Managers.Resource.LoadWeaponIcon(weaponChart.Icon);
        GradeImage.sprite = Managers.Resource.LoadItemGradeBg(weaponChart.Grade);
        SubGradeImage.sprite = Managers.Resource.LoadSubGradeIcon(weaponChart.SubGrade);
        if (weaponChart.Grade == Grade.Godgod)
            BackGroundImage.sprite = Managers.Resource.LoadBackGroundIconImage(weaponChart.Grade);

        if (SubGradeImage.sprite == null)
            SubGradeImage.gameObject.SetActive(false);
        else
            SubGradeImage.gameObject.SetActive(true);

        _composite.Clear();

        WeaponInfoText.text = $"({Managers.Game.WeaponDatas[itemIndex].Quantity.ToString()}/{weaponChart.CombineCount.ToString()})";
        Managers.Game.WeaponDatas[itemIndex].OnChangeQuantity.Subscribe(quantity =>
        {
            WeaponInfoText.text = $"({quantity}/{weaponChart.CombineCount})";
        }).AddTo(_composite);

        gameObject.SetActive(true);

        if (Managers.Game.WeaponDatas[itemIndex].IsAcquired)
        {
            NoAcquiredObj.SetActive(false);
            WeaponInfoText.gameObject.SetActive(true);
        }
        else
        {
            NoAcquiredObj.SetActive(true);
            WeaponInfoText.gameObject.SetActive(false);
            Managers.Game.WeaponDatas[itemIndex].OnAcquired.Subscribe(_ =>
            {
                NoAcquiredObj.SetActive(false);
                WeaponInfoText.gameObject.SetActive(true);
            }).AddTo(_composite);
        }
        
        WeaponButton.BindEvent(() => callback?.Invoke(this));
    }

    public void OnSelect()
    {
        SelectObj.SetActive(true);
    }

    public void OffSelect()
    {
        SelectObj.SetActive(false);
    }
}