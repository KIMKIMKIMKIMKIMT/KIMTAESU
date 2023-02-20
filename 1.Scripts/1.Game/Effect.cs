using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    #region Fields
    private ParticleSystem _effect;

    private float _duration;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _effect = GetComponent<ParticleSystem>();
        _duration = _effect.main.duration;
    }

    private void OnEnable()
    {
        Invoke("ActiveOff", _duration);
    }
    #endregion

    #region Private Methods
    private void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
