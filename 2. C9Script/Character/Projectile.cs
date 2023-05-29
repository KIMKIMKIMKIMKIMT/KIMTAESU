using System;
using UniRx;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private CircleCollider2D _circleCollider;
    
    private Vector3 _direction;
    private float _radius;
    private float _speed;
    private float _damage;

    private float _destroyTime = 5f;

    private bool _isFire;

    private void Awake()
    {
        _rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
        _circleCollider = gameObject.GetOrAddComponent<CircleCollider2D>();

        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.simulated = true;

        _circleCollider.isTrigger = true;
    }

    public void Fire(Vector3 direction, float speed, float radius, float damage, bool isLookAtDirection = false, float destroyTime = 5f)
    {
        _direction = direction;
        _radius = radius;
        _speed = speed;
        _damage = damage;
        _destroyTime = destroyTime;

        if (isLookAtDirection)
        {
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            transform.rotation = angleAxis;
        }

        _isFire = true;
        
        Observable.Timer(TimeSpan.FromSeconds(_destroyTime)).Subscribe(_ =>
        {
            _isFire = false;
            gameObject.SetActive(false);
        }).AddTo(gameObject);
    }

    private void Update()
    {
        if (!_isFire)
            return;
        
        transform.Translate(_direction * (_speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Monster monster = col.GetComponent<Monster>();
        
        if (monster == null)
            return;
        
        //monster.Damage(_damage);
    }
}