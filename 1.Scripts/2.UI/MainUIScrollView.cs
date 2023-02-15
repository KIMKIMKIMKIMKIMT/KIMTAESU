using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainUIScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Fields
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private Slider _tabSlider;
    [SerializeField] private RectTransform[] _tabBtnRect;
    [SerializeField] private RectTransform[] _tabBtnImgRect;

    private const int SIZE = 5;
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
        _pos = new float[SIZE];
        _distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++)
        {
            _pos[i] = _distance * i;
        }
    }
    private void Start()
    {
        _targetIndex = 2;
        _targetPos = _pos[_targetIndex];
    }

    private void Update()
    {
        _tabSlider.value = _scrollbar.value;

        if (!_isDrag)
        {
            _scrollbar.value = Mathf.Lerp(_scrollbar.value, _targetPos, 0.1f);

            for (int i = 0; i < SIZE; i++)
            {
                _tabBtnRect[i].sizeDelta = new Vector2(i == _targetIndex ? 360 : 180, _tabBtnRect[i].sizeDelta.y);
            }
        }

        for (int i = 0; i < SIZE; i++)
        {
            Vector3 btnImgTargetPos = _tabBtnRect[i].anchoredPosition3D;
            Vector3 btnImgLocalScale = Vector3.one;
            bool textActive = false;

            if (i == _targetIndex)
            {
                btnImgTargetPos.y = -57f;
                btnImgLocalScale = new Vector3(1.3f, 1.3f, 1);
                textActive = true;
            }

            _tabBtnImgRect[i].anchoredPosition3D = Vector3.Lerp(_tabBtnImgRect[i].anchoredPosition3D, btnImgTargetPos, 0.25f);
            _tabBtnImgRect[i].localScale = Vector3.Lerp(_tabBtnImgRect[i].localScale, btnImgLocalScale, 0.25f);
            _tabBtnImgRect[i].GetChild(0).gameObject.SetActive(textActive);
        }
    }
    #endregion

    #region Public Methods
    
    public void OnClickTab(int index)
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        _targetIndex = index;
        _targetPos = _pos[index];
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        for (int i = 0; i < SIZE; i++)
        {
            if (_scrollbar.value < _pos[i] + _distance * 0.5f && _scrollbar.value > _pos[i] - _distance * 0.5f)
            {
                _curPos = _pos[i];
                _targetIndex = i;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _isDrag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDrag = false;

        for (int i = 0; i < SIZE; i++)
        {
            if (_scrollbar.value < _pos[i] + _distance * 0.5f && _scrollbar.value > _pos[i] - _distance * 0.5f)
            {
                _targetPos = _pos[i];
                _targetIndex = i;
            }
        }

        if (_curPos == _targetPos)
        {
            if (eventData.delta.x > 18 && _curPos - _distance >= 0)
            {
                if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
                {
                    SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
                }
                --_targetIndex;
                _targetPos = _curPos - _distance;
            }
            else if(eventData.delta.x < -18 && _curPos + _distance <= 1.01f)
            {
                if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
                {
                    SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
                }
                ++_targetIndex;
                _targetPos = _curPos + _distance;
            }
        }
        
    }
    #endregion
}
