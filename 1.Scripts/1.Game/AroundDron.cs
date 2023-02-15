using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AroundDron : MonoBehaviour
{
    #region Fields
    [SerializeField] private Tweener[] _tweens;
    [SerializeField] private AutoRotate _autoRotate;

    private float _defaultSpeed;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _tweens = GetComponentsInChildren<Tweener>();
        _defaultSpeed = _autoRotate._speed;
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one * BuffSkillController.Buff_BulletScale;
        _autoRotate._speed = _defaultSpeed * BuffSkillController.Buff_BulletSpeed;
        for (int i = 0; i < _tweens.Length; i++)
        {
            _tweens[i].From = Vector3.zero;
            _tweens[i].To = Vector3.one;
            _tweens[i].StartTween();
        }
    }
    private void OnDisable()
    {
        _autoRotate._speed = _defaultSpeed;
    }
    #endregion

    #region Public Methods
    public void ActiveOff()
    {
        gameObject.SetActive(false);
    }

    public void ActiveOffTweens()
    {
        for (int i = 0; i < _tweens.Length; i++)
        {
            _tweens[i].From = Vector3.one;
            _tweens[i].To = Vector3.zero;
            _tweens[i].StartTween();
        }
        Invoke("ActiveOff", 0.5f);
    }

    public void SetSpeed()
    {
        
    }
    #endregion
}
