using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;

public class UI_GuildSportsEndPopup : UI_Popup
{
    [SerializeField] private TMP_Text TitleTxt;
    [SerializeField] private TMP_Text GoodsValueText;
    [SerializeField] private TMP_Text GuildGoodsValueText;

    public void Init(string title, double gem, int guildGoods)
    {
        TitleTxt.text = $"{title}!";

        GoodsValueText.text = gem.ToCurrencyString();
        GuildGoodsValueText.text = guildGoods.ToString();
    }
}
