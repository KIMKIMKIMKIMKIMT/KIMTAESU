using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_PromoFailPopup : UI_Popup
{
    [SerializeField] private Button CloseButton;

    public override bool isTop => true;

    public override void Open()
    {
        base.Open();
        CloseButton.ClearEvent();
        StartCoroutine(CoReservationButtonEvent());
    }

    private IEnumerator CoReservationButtonEvent()
    {
        yield return new WaitForSeconds(1f);
        
        CloseButton.BindEvent(() =>
        {
            ClosePopup();
            Managers.Dungeon.EndPromo(true);
        });

        yield return new WaitForSeconds(2f);
        
        ClosePopup();
        Managers.Dungeon.EndPromo(true);
    }
}