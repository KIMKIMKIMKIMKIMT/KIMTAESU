using TMPro;
using UI;
using UnityEngine;


public class UI_DpsResultPopup : UI_Popup
{
    [SerializeField] private TMP_Text DpsValueText;

    public override bool isTop => true;

    public override void Open()
    {
        base.Open();
        DpsValueText.text = Managers.Dps.TotalDps.Value.ToCurrencyString();
    }
}