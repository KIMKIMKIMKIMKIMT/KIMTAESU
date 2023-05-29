using System;
using UnityEngine;

public class GameViewCollider : MonoBehaviour
{
    private Camera _gameCamera;
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _boxCollider;

    private void Start()
    {
        _gameCamera = gameObject.GetOrAddComponent<Camera>();
        _boxCollider = gameObject.GetOrAddComponent<BoxCollider2D>();
        _rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
    }

    private void SetCollider()
    {
        //
    }

    private void SetRigidBody()
    {
        
    }
}