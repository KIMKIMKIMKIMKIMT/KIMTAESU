using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissileFollower : MonoBehaviour
{
    #region Fields
    private Transform _player;
    private Tweener _tween;

    [SerializeField] private float _delay;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _tween = GetComponent<Tweener>();
        _player = BattleMgr.Instance.Player.transform;
        transform.position = _player.transform.position;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 dir = new Vector3(_player.position.x + (-0.5f), _player.position.y + 1, 0);
        transform.position = Vector3.Lerp(transform.position, dir, _delay);

        //_tween.From.x = transform.position.x;
        //_tween.To.x = transform.position.x;
        //_tween.From.y = transform.position.y;
        //_tween.To.y = transform.position.y - 0.1f;
    }
    #endregion
}
