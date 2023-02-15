using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Fields
    [SerializeField] private Transform _player;
    private Rigidbody2D _rigid;

    #endregion

    #region Unity Methods
    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion

    #region Coroutines
    #endregion
}
