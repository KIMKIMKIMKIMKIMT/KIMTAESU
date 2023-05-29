using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class UI_GainDropItem : UI_Base
{
    [SerializeField] private TMP_Text ItemValueText1;
    [SerializeField] private TMP_Text ItemValueText2;

    [SerializeField] private Image ItemImage1;
    [SerializeField] private Image ItemImage2;

    [SerializeField] private GameObject ItemObj1;
    [SerializeField] private GameObject ItemObj2;

    [SerializeField] private CanvasGroup CanvasGroup;

    private Sequence _sequence;

    public void ResetScale()
    {
        transform.localScale = Vector3.one;
    }

    public void SetFadeSequence()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence().Append(CanvasGroup.DOFade(0f, 0.7f * (transform.GetSiblingIndex() + 1)));
    }

    public void Set(UI_GainDropItemsPanel.GainDropItemData gainDropItemData)
    {
        CanvasGroup.alpha = 1f; 
        _sequence?.Kill();
        _sequence = DOTween.Sequence().SetAutoKill(true)
            //.Append(CanvasGroup.DOFade(1f, 0f))
            .Append(transform.DOScale(1.2f, 0.1f))
            .Append(transform.DOScale(1f, 0.1f))
            .AppendInterval(1f).Append(CanvasGroup.DOFade(0f, 2f));


        if (gainDropItemData.GoodsId1 == 0)
        {
            ItemObj1.SetActive(false);
        }
        else
        {
            ItemObj1.SetActive(true);
            ItemImage1.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, gainDropItemData.GoodsId1);
            ItemValueText1.text = gainDropItemData.GoodsValue1.ToCurrencyString();
        }

        if (gainDropItemData.GoodsId2 == 0)
        {
            ItemObj2.SetActive(false);
        }
        else
        {
            ItemObj2.SetActive(true);
            ItemImage2.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, gainDropItemData.GoodsId2);
            ItemValueText2.text = gainDropItemData.GoodsValue2.ToCurrencyString();
        }
    }
}