using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eBOX_TYPE
{
    BronzeBox,
    GoldBox
}
public class BoxOpenPopup : MonoBehaviour
{
    #region Fields
    private UI_Tweener[] _tweens;
    private GetEquipmentPool _pool;
    private List<eATKWEAPON> _weaponList = new List<eATKWEAPON>();
    private List<eHPEQUIP> _equipList = new List<eHPEQUIP>();
    private WaitForSeconds _delay = new WaitForSeconds(0.1f);

    [SerializeField] private RectTransform _boxRectTransform;
    [SerializeField] private Image[] _box;
    [SerializeField] private GameObject _lockEffect;
    [SerializeField] private GameObject _openEffect;
    [SerializeField] private GameObject _quitTxt;
    [SerializeField] private Button _quitBtn;

    #endregion

    #region Unity Methods
    private void Awake()
    {
        _tweens = GetComponentsInChildren<UI_Tweener>();
        _pool = GetComponentInChildren<GetEquipmentPool>();
    }
    private void OnEnable()
    {
        _pool.AllObjActiveOff();
        _boxRectTransform.position = Vector3.zero;
        _boxRectTransform.localScale = Vector3.one;
        _quitTxt.SetActive(false);
        _box[(int)eBOX_STRUCT.Cover].gameObject.SetActive(true);
        _box[(int)eBOX_STRUCT.OpenCover].gameObject.SetActive(false);
        _lockEffect.SetActive(true);
        _openEffect.SetActive(false);
        _quitBtn.interactable = false;
        Invoke("SwitchBox", _tweens[0].Duration);
        
    }

    private void OnDisable()
    {
        _weaponList.Clear();
        _equipList.Clear();
    }
    #endregion

    #region Public Methods
    public void StartShowReward()
    {
        StopAllCoroutines();
        StartCoroutine(Cor_ShowReward());
    }
    public void SetBoxSprite(eBOX_TYPE type)
    {
        Sprite[] sprite = SpriteMgr.Instance.GetShopBoxSprite(type);

        for (int i = 0; i < (int)eBOX_STRUCT.Max; i++)
        {
            _box[i].sprite = sprite[i];
        }
    }
    public void SwitchBox()
    {
        _box[(int)eBOX_STRUCT.Cover].gameObject.SetActive(false);
        _lockEffect.SetActive(false);
        _box[(int)eBOX_STRUCT.OpenCover].gameObject.SetActive(true);
        _openEffect.SetActive(true);
        Invoke("BoxMove", 0.1f);
    }
    public void BoxMove()
    {
        for (int i = 1; i < _tweens.Length; i++)
        {
            _tweens[i].StartTween();
        }
        Invoke("StartShowReward", 0.6f);
    }

    public void AddList(eATKWEAPON weapon)
    {
        Debug.Log(_weaponList);
        _weaponList.Add(weapon);
    }
    public void AddList(eHPEQUIP equip)
    {
        Debug.Log(_equipList);
        _equipList.Add(equip);
    }
    public void OnClickQuit()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PopupMgr.Instance.RemovePopup();
    }
    #endregion

    #region Coroutines
    private IEnumerator Cor_ShowReward()
    {
        for (int i = 0; i < _weaponList.Count; i++)
        {
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.Reward);
            }
            GetEquipment equip = _pool.GetFromPool(_pool.transform);
            equip.SetData(_weaponList[i], GameDataMgr.Instance.AtkWeaponData.AtkWeaponDic[_weaponList[i]].Grade);
            yield return _delay;
        }
        for (int i = 0; i < _equipList.Count; i++)
        {
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.Reward);
            }
            GetEquipment equip = _pool.GetFromPool(_pool.transform);
            equip.SetData(_equipList[i], GameDataMgr.Instance.HpEquipData.HpEquipmentDic[_equipList[i]].Grade);
            yield return _delay;
        }
        _quitTxt.SetActive(true);
        _quitBtn.interactable = true;
    }
    #endregion
}
