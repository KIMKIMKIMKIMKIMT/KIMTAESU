using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_DropItem : MonoBehaviour
    {
        [SerializeField] private Image _itemImage;

        public void Init(ItemType itemType, int itemId)
        {
            _itemImage.sprite = Managers.Resource.LoadItemIcon(itemType, itemId);

            transform.DOMoveY(transform.position.y + 0.5f, 0.5f).onComplete += () =>
            {
                //Managers.Drop.Return(this);
            };
        }
    }
}