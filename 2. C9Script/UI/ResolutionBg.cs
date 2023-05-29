using System;
using UnityEngine;

public class ResolutionBg : MonoBehaviour
{
    private Camera _uiCamera;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
        _rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (_rectTransform == null)
            return;
        
        _rectTransform.sizeDelta = new Vector2(_uiCamera.rect.height,  _uiCamera.rect.height);
    }
}
