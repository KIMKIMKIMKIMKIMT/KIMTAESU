using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChpaterScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Fields
    [SerializeField] private WarUI _warUI;
    [SerializeField] private Scrollbar _scrollBar;
    [SerializeField] private Text[] _description;
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

    private void Update()
    {
        if (!_isDrag)
        {
            _scrollBar.value = Mathf.Lerp(_scrollBar.value, _targetPos, 0.1f);
            if (!_description[_targetIndex].gameObject.activeInHierarchy)
            {
                _description[_targetIndex].gameObject.SetActive(true);
            }
            if (!_titleImg[_targetIndex].gameObject.activeInHierarchy)
            {
                _titleImg[_targetIndex].gameObject.SetActive(true);
            }
            
        }
    }
    #endregion

    #region Public Methods
    public void OnClickSelectChapter(int index)
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        GameMgr.Instance.Chapter = index;
        _warUI.SetChapterImg(index);
        PopupMgr.Instance.RemovePopup();
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
        //        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        //        {
        //            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        //        }
        //        _targetPos = _curPos - _distance;
        //    }
        //    else if (eventData.delta.x < -18 && _curPos + _distance <= 1.01f)
        //    {
        //        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        //        {
        //            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        //        }
        //        _targetPos = _curPos + _distance;
        //    }
        //}

        _description[_targetIndex].gameObject.SetActive(true);
        _titleImg[_targetIndex].gameObject.SetActive(true);
    }
    #endregion
}
