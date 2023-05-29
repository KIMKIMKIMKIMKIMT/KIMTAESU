
    using TMPro;
    using UI;
    using UnityEngine;

    public class UI_PvpResultPopup : UI_Popup
    {
        [SerializeField] private TMP_Text TokenValueText;

        public void Init(double token)
        {
            TokenValueText.text = token.ToCurrencyString();
        }
    }
