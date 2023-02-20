using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TankSelectionScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Fields
    [SerializeField] private Scrollbar _scrollBar;
    [SerializeField] private Button[] _tankBtn;
    [SerializeField] private Button[] _rockBtn;
    [SerializeField] private Image[] _description;
    [SerializeField] private Image[] _titleImg;

    private int _size;
    private int _targetIndex;
    private float[] _pos;
    private float _distance;
    private float _targetPos;
    private float _curPos;
    private bool _isDrag;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _size = 3;
        _pos = new float[_size];
        _distance = 1f / (_size - 1);
        for (int i = 0; i < _size; i++)
        {
            _pos[i] = _distance * i;
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < _tankBtn.Length; i++)
        {
            _rockBtn[i].gameObject.SetActive(!PlayerDataMgr.Instance.PlayerData.Tanks[i]);
            _tankBtn[i].interactable = PlayerDataMgr.Instance.PlayerData.Tanks[i];
        }
    }

    private void Update()
    {
        if (!_isDrag)
        {
            _scrollBar.value = Mathf.Lerp(_scrollBar.value, _targetPos, 0.1f);
            if (!_titleImg[_targetIndex].gameObject.activeInHierarchy)
            {
                _titleImg[_targetIndex].gameObject.SetActive(true);
            }
            if (!_description[_targetIndex].gameObject.activeInHierarchy)
            {
                _description[_targetIndex].gameObject.SetActive(true);
            }
        }
    }
    #endregion

    #region Public Methods
    public void OnClickSelectTank(int index)
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PlayerDataMgr.Instance.PlayerData.CurTank = (eTANK)index;
        UIMgr.Instance.MainUI.InvenUI.SetTank((eTANK)index);
        UIMgr.Instance.MainUI.UpgradeUI.SetTank((eTANK)index);
        PlayerDataMgr.Instance.SaveData();
        UIMgr.Instance.Refresh();
        PopupMgr.Instance.RemovePopup();
    }
    public void OnClickTankLock(int index)
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        switch (index)
        {
            case 0:
                PopupMgr.Instance.ShowOkCancelPopup("알림", "K-30 비호를 2,000 보석에 구매하시겠습니까 ?", () =>
                {
                    if (PlayerDataMgr.Instance.PlayerData.Gem < 2000)
                    {
                        PopupMgr.Instance.ShowOkPopup("알림", "보석이 부족합니다.");
                        return;
                    }
                    PopupMgr.Instance.ShowOkPopup("알림", "K-30 비호 구매가 완료 되었습니다.");
                    PlayerDataMgr.Instance.PlayerData.Gem -= 2000;
                    PlayerDataMgr.Instance.SaveData();
                    UIMgr.Instance.Refresh();
                    PlayerDataMgr.Instance.PlayerData.Tanks[index] = true;
                    _rockBtn[index].gameObject.SetActive(false);
                    _tankBtn[index].interactable = true;
                });
                break;
            case 1:
                PopupMgr.Instance.ShowOkCancelPopup("알림", "K-2 흑표를 5,000 보석에 구매하시겠습니까 ?", () =>
                {
                    if (PlayerDataMgr.Instance.PlayerData.Gem < 5000)
                    {
                        PopupMgr.Instance.ShowOkPopup("알림", "보석이 부족합니다.");
                        return;
                    }
                    PopupMgr.Instance.ShowOkPopup("알림", "K-2 흑표 구매가 완료 되었습니다.");
                    PlayerDataMgr.Instance.PlayerData.Gem -= 5000;
                    PlayerDataMgr.Instance.SaveData();
                    UIMgr.Instance.Refresh();
                    PlayerDataMgr.Instance.PlayerData.Tanks[index] = true;
                    _rockBtn[index].gameObject.SetActive(false);
                    _tankBtn[index].interactable = true;
                });
                break;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        for (int i = 0; i < _size; i++)
        {
            if (_scrollBar.value < _pos[i] + _distance * 0.5f && _scrollBar.value > _pos[i] - _distance * 0.5f)
            {
                _curPos = _pos[i];
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _isDrag = true;

        for (int i = 0; i < _size; i++)
        {
            _description[i].gameObject.SetActive(false);
            _titleImg[i].gameObject.SetActive(false);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        _isDrag = false;

        for (int i = 0; i < _size; i++)
        {
            if (_scrollBar.value < _pos[i] + _distance * 0.5f && _scrollBar.value > _pos[i] - _distance * 0.5f)
            {
                _targetPos = _pos[i];
                _targetIndex = i;
            }
        }
        //if (_curPos == _targetPos)
        //{
        //    if (eventData.delta.x > 18 && _curPos - _distance >= 0)
        //    {
        //        _targetPos = _curPos - _distance;
        //    }
        //    else if (eventData.delta.x < -18 && _curPos + _distance <= 1.01f)
        //    {
        //        _targetPos = _curPos + _distance;
        //    }
        //}

        
        
    }
    #endregion
}
