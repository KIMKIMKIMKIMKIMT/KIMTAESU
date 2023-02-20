using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InvenScroll : ScrollRect
{
    #region Fields
    private MainUIScrollView _mainUIScrollView;
    private ScrollRect _mainScrollRect;

    private bool _isParent;
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        base.Start();
        _mainUIScrollView = GameObject.FindWithTag("MainScrollView").GetComponent<MainUIScrollView>();
        _mainScrollRect = GameObject.FindWithTag("MainScrollView").GetComponent<ScrollRect>();
    }
    #endregion

    #region Public Methods
    public override void OnBeginDrag(PointerEventData eventData)
    {
        _isParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

        if (_isParent)
        {
            _mainUIScrollView.OnBeginDrag(eventData);
            _mainScrollRect.OnBeginDrag(eventData);
        }
        else
        {
            base.OnBeginDrag(eventData);
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (_isParent)
        {
            _mainUIScrollView.OnDrag(eventData);
            _mainScrollRect.OnDrag(eventData);
        }
        else
        {
            base.OnDrag(eventData);
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (_isParent)
        {
            _mainUIScrollView.OnEndDrag(eventData);
            _mainScrollRect.OnEndDrag(eventData);
        }
        else
        {
            base.OnEndDrag(eventData);
        }
    }
    #endregion
}
