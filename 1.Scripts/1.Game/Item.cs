using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eITEM_TYPE
{
    Bomb,
    Magnet,
    Potion,
    GoldBundle,
    Gold
}
public abstract class Item : MonoBehaviour
{
    #region Fields
    protected Tweener _tween;

    protected Vector3 _dir;

    [SerializeField] protected float _moveSpeed;
    protected bool _moveTrigger;
    #endregion

    #region Unity Methods
    protected abstract void Awake();
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
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.Item, true);
            }
            ItemTrigger();
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Protected Methods
    protected abstract void ItemTrigger();
    #endregion
}
