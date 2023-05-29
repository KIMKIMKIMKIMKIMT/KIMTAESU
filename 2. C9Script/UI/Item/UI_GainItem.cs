using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GainItem : UI_Base
{
    [SerializeField] private TMP_Text ItemValueText;

    [SerializeField] private Image ItemImage;
    [SerializeField] private Image SubGradeImage;

    public void Init(ItemType itemType, int itemId, double itemValue)
    {
        ItemImage.sprite = Managers.Resource.LoadItemIcon(itemType, itemId);

        switch (itemType)
        {
            case ItemType.Goods:
            case ItemType.Costume:
                SubGradeImage.gameObject.SetActive(false);
                break;
            case ItemType.Weapon:
            case ItemType.Pet:
            {
                SubGradeImage.gameObject.SetActive(true);
                SubGradeImage.sprite = Managers.Resource.LoadSubGradeIcon(itemType, itemId);
            }
                break;
        }

        ItemValueText.text = itemValue.ToCurrencyString();
    }
}