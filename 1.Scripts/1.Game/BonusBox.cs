using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusBox : MonoBehaviour
{
    #region Fields
    private Tweener _tween;
    private Vector3 _dir;
    [SerializeField] private float _moveSpeed;
    private bool _moveTrigger;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _tween = GetComponent<Tweener>();
    }
    protected virtual void OnEnable()
    {
        _moveTrigger = false;
    }
    protected virtual void Update()
    {
        if (_moveTrigger)
        {
            Vector3 dir = BattleMgr.Instance.Player.transform.position - transform.position;
            transform.position += dir.normalized * _moveSpeed * Time.deltaTime;
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Magnetic") && !_moveTrigger)
        {
            _dir = transform.position - BattleMgr.Instance.Player.transform.position;
            _tween.From = transform.position;
            _tween.To = transform.position + (_dir.normalized * 2f);
            _tween.StartTween(() => { _moveTrigger = true; });
        }
        if (collision.CompareTag("Player") && _moveTrigger)
        {
            ItemTrigger();
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Public Methods
    public void ItemTrigger()
    {
        InGameUIMgr.Instance._gameUI.ShowBonusPopup();
    }
    #endregion
}
