using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    #region Fields
    private Camera _cam;
    private Transform _player;
    private Transform _colliderTransform;

    [SerializeField] private float _delay;
    [SerializeField] private float _zoomSpeed;
    private float _camSize;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _cam = GetComponent<Camera>();
        _colliderTransform = GetComponentInChildren<Collider2D>().transform;

        _player = BattleMgr.Instance.Player.transform;
        _camSize = _cam.orthographicSize;
        SetCameraSize(7);
    }

    private void FixedUpdate()
    {
        Vector3 playerPos = new Vector3(_player.position.x, _player.position.y, this.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, playerPos, _delay);

        _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _camSize, _zoomSpeed);
    }
    #endregion

    #region Public Methods
    public void SetCameraSize(float size)
    {
        float temp = size / _camSize;
        _colliderTransform.localScale *= temp;
        _camSize = size;
    }
    #endregion
}
