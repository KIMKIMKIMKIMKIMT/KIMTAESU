using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FusionDragObj : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    #region Fields
    private FusionObj _fusionObj;
    private FusionPopup _fusionPopup;
    private RectTransform _rectTransform;

    [SerializeField] private Image _pannel;
    [SerializeField] private Image _icon;
    [SerializeField] private Text _levelTxt;

    public Collider2D LastHitCollider { get; private set; }
    public eATKWEAPON WeaponKey { get; private set; }
    public eHPEQUIP EquipKey { get; private set; }
    public eTYPE Type { get; private set; }
    public int Index { get; private set; }
    public int Level { get; private set; }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _fusionPopup = GetComponentInParent<FusionPopup>();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        LastHitCollider = null;
        WeaponKey = eATKWEAPON.None;
        EquipKey = eHPEQUIP.None;
        Type = eTYPE.None;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FusionSlot") || collision.CompareTag("PartSlot"))
        {
            LastHitCollider = collision;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LastHitCollider == collision)
        {
            LastHitCollider = null;
        }
    }
    #endregion

    #region Public Methods
    public void SetData(eATKWEAPON weapon, int level, int index, eEQUIPGRADE_TYPE grade)
    {
        WeaponKey = weapon;
        _icon.sprite = SpriteMgr.Instance.GetAtkWeaponIcon(WeaponKey);
        _pannel.sprite = SpriteMgr.Instance.GetEquipmentPannel(grade);
        Level = level;
        _levelTxt.text = "Lv." + Level;
        Index = index;
        Type = eTYPE.Weapon;
    }
    public void SetData(eHPEQUIP equip, int level, int index, eEQUIPGRADE_TYPE grade)
    {
        EquipKey = equip;
        _icon.sprite = SpriteMgr.Instance.GetHpEquipmentIcon(EquipKey);
        _pannel.sprite = SpriteMgr.Instance.GetEquipmentPannel(grade);
        Level = level;
        _levelTxt.text = "Lv." + Level;
        Index = index;
        Type = eTYPE.Equip;
    }
    public void OnClickDown()
    {
        _fusionPopup.FusionPartClear();
        _fusionPopup.ShowEquip();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnClickDown();
    }
    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.position = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (LastHitCollider == null)
        {
            _fusionPopup.SetSelectObj(null, null);
            gameObject.SetActive(false);
        }
        else
        {
            if (LastHitCollider.CompareTag("FusionSlot"))
            {
                _fusionPopup.SetFusionObj(this);
                gameObject.SetActive(false);
            }
            else if (LastHitCollider.CompareTag("PartSlot"))
            {
                if (_fusionPopup._fusionSlot.Obj == null || _fusionPopup._fusionSlot.Obj.Type == eTYPE.Weapon ? _fusionPopup._fusionSlot.Obj.WeaponKey != WeaponKey : _fusionPopup._fusionSlot.Obj.EquipKey != EquipKey)
                {
                    _fusionPopup.SetSelectObj(null, null);
                    gameObject.SetActive(false);
                    return;
                }
                _fusionPopup.SetPartObj(this);
                gameObject.SetActive(false);
            }
        }
    }

    
    #endregion
}
