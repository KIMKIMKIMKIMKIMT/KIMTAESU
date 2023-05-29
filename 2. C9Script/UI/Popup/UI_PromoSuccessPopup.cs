using System.Collections;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_PromoSuccessPopup : UI_Popup
{
    [SerializeField] private TMP_Text AttackValueText;
    [SerializeField] private Image PromoGradeImage;
    [SerializeField] private Button CloseButton;
    [SerializeField] private GameObject CriticalRateObj;
    [SerializeField] private Image CriticalRateImage;

    public override bool isTop => true;

    public override void Open()
    {
        base.Open();

        var promoDungeonChart = ChartManager.PromoDungeonCharts[Managers.Game.UserData.PromoGrade];

        AttackValueText.text = $"{(promoDungeonChart.ClearRewardStat1Value * 100).ToCurrencyString()}%";
        PromoGradeImage.sprite = Managers.Resource.LoadPromoIcon(Managers.Game.UserData.PromoGrade);

        if (promoDungeonChart.ClearRewardStat2Id == 0)
        {
            CriticalRateObj.SetActive(false);
        }
        else
        {
            CriticalRateObj.SetActive(true);
            if (ChartManager.StatCharts.TryGetValue(promoDungeonChart.ClearRewardStat2Id, out var statChart))
                CriticalRateImage.sprite = Managers.Resource.LoadStatIcon(statChart.Icon);
        }

        CloseButton.ClearEvent();
        StartCoroutine(CoReservationButtonEvent());
    }

    private IEnumerator CoReservationButtonEvent()
    {
        yield return new WaitForSeconds(1f);
        
        CloseButton.BindEvent(() =>
        {
            ClosePopup();
            Managers.Dungeon.EndPromo();
        });

        yield return new WaitForSeconds(2f);
        
        ClosePopup();
        Managers.Dungeon.EndPromo();
        
        CloseButton.BindEvent(() =>
        {
            ClosePopup();
            Managers.Dungeon.EndPromo();
        });
    }
}