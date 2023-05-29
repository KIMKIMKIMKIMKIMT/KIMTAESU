using UnityEngine;
using UnityEngine.UI;

public class UI_StageRewardItem : UI_Base
{
    [SerializeField] private Image ItemImage;
    
    public void Init(ItemType itemType, int itemId)
    {
        ItemImage.sprite = Managers.Resource.LoadItemIcon(itemType, itemId);
    }
}