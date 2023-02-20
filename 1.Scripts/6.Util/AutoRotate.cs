using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public enum eROTATE_DIR
    {
        Left,
        Right
    }

    #region Fields
    [SerializeField] private eROTATE_DIR _rotateDir;
    public eROTATE_DIR RotateDIR { get { return _rotateDir; } }

    private Transform _transfrom;

    public float _speed;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _transfrom = transform;
    }

    private void Update()
    {
        if (_rotateDir == eROTATE_DIR.Left)
        {
            _transfrom.Rotate(Vector3.forward, _speed * Time.deltaTime);
        }
        else if (_rotateDir == eROTATE_DIR.Right)
        {
            _transfrom.Rotate(Vector3.back, _speed * Time.deltaTime);
        }
    }
    #endregion
}
