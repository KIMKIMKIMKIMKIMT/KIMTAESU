using System;
using Cinemachine.Utility;
using UnityEngine;

public class FixEffectPosition : MonoBehaviour
{
    [SerializeField] private Vector3 StartPosition;
    private Vector3 _prevPosition;
    
    private void OnEnable()
    {
        transform.localPosition = StartPosition;
        _prevPosition = transform.position;
    }

    private void Update()
    {
        transform.position = _prevPosition;
    }
}