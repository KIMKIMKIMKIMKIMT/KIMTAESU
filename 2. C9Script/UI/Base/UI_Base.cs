using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    // 상단 하단 메뉴보다 위에 오는 팝업인지
    public virtual bool isTop { get; } = false;
    
    public static void BindEvent(GameObject go, Action action, UIEvent type = UIEvent.Click)
    {
        UI_EventHandler evt = go.GetOrAddComponent<UI_EventHandler>();

        switch (type)
        {
            case UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case UIEvent.Pressed:
                evt.OnPressedHandler -= action;
                evt.OnPressedHandler += action;
                break;
            case UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action;
                evt.OnPointerDownHandler += action;
                break;
            case UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action;
                evt.OnPointerUpHandler += action;
                break;
        }
    }

    public static void SetScrollRect(GameObject go, ScrollRect scrollRect)
    {
        UI_EventHandler evt = go.GetOrAddComponent<UI_EventHandler>();
        evt.SetScrollRect(scrollRect);
    }
    
    public static void ClearEvent(GameObject go, UIEvent type = UIEvent.Click)
    {
        UI_EventHandler evt = go.GetOrAddComponent<UI_EventHandler>();

        switch (type)
        {
            case UIEvent.Click:
                evt.OnClickHandler = null;
                break;
            case UIEvent.Pressed:
                evt.OnPressedHandler = null;
                break;
            case UIEvent.PointerDown:
                evt.OnPointerDownHandler = null;
                break;
            case UIEvent.PointerUp:
                evt.OnPointerUpHandler = null;
                break;
        }
    }

    public virtual void Open()
    {
        
    }

    public virtual void Refresh()
    {
    }
}