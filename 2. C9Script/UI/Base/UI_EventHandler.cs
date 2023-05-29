using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action OnClickHandler = null;
    public Action OnPressedHandler = null;
    public Action OnPointerDownHandler = null;
    public Action OnPointerUpHandler = null;

    private Coroutine _pressCoroutine;

    private ScrollRect _scrollRect;

    public UISoundType ClickSoundType = UISoundType.DefaultButton; 

    bool _pressed = false;

    private CompositeDisposable _timerComposite = new();

    private void OnDisable()
    {
        if (_scrollRect != null)
            _scrollRect.enabled = true;
        
        _timerComposite.Clear();
        _pressCoroutine = null;
    }

    public void SetScrollRect(ScrollRect scrollRect)
    {
        _scrollRect = scrollRect;
    }

    private IEnumerator PressCoroutine()
    {
        while (true)
        {
            OnPressedHandler?.Invoke();
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_pressed)
            return;
        
        Managers.Sound.PlayUISound(ClickSoundType);
        _pressed = true;
        _timerComposite.Clear();
        Observable.Timer(TimeSpan.FromSeconds(0.15f)).Where(_ => _pressed).Subscribe(_ =>
        {
            if (gameObject != null && gameObject.activeSelf)
                _pressCoroutine ??= StartCoroutine(PressCoroutine());
        }).AddTo(_timerComposite);
        OnPointerDownHandler?.Invoke();
        
        if (_scrollRect != null)
            _scrollRect.enabled = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        if (_pressCoroutine != null)
        {
            StopCoroutine(_pressCoroutine);
            _pressCoroutine = null;
        }

        OnPointerUpHandler?.Invoke();
        
        if (_scrollRect != null)
            _scrollRect.enabled = true;
    }
}