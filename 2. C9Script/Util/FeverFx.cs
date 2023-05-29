using System;
using UnityEngine;


public class FeverFx : MonoBehaviour
{
    private Camera _camera;
    private Animator _animator;

    private void Awake()
    {
        _camera = Camera.main;
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_animator == null)
            return;

        transform.position = Vector2.zero;

        if (_animator.IsEndCurrentAnimation())
            gameObject.SetActive(false);
    }
}