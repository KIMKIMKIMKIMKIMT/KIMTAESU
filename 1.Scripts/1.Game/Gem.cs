using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    #region Unity Methods
    private Tweener _tween;

    private Vector3 _dir;

    [SerializeField] private int _exp;
    [SerializeField] private float _moveSpeed;
    private bool _moveTrigger;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _tween = GetComponent<Tweener>();
        _moveSpeed = 10f;
    }

    private void OnEnable()
    {
        _moveTrigger = false;
    }

    private void Update()
    {
        if (_moveTrigger)
        {
            float t = Time.deltaTime * (_moveSpeed / 5);
            Vector3 dir = BattleMgr.Instance.Player.transform.position - transform.position;
            transform.position += dir.normalized * _moveSpeed * t;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Magnetic") && !_moveTrigger || collision.CompareTag("MagnetTrigger") && !_moveTrigger)
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
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.Gem);
            }
            BattleMgr.Instance.IncreaseEXP(_exp);
            gameObject.SetActive(false);
        }
    }
    #endregion
}
