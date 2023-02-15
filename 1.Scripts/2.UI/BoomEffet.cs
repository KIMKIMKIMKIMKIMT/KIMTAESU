using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomEffet : MonoBehaviour
{
    #region Fields
    private ColorTweener _tween;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _tween = GetComponent<ColorTweener>();
    }
    private void OnEnable()
    {
        Invoke("ActiveOff", _tween.Duration);
    }
    #endregion

    #region Public Methods
    public void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
