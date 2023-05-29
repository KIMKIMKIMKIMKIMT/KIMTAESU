using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEffect : MonoBehaviour
{
    #region Fields
    private Animator _effectAnim;

    private float _duration;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _effectAnim = GetComponent<Animator>();
    }

    //private void OnEnable()
    //{
    //    Invoke("ActiveOff", _duration);
    //}
    #endregion

    #region Private Methods
    private void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
