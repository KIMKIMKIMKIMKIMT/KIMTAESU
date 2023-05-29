using System;
using UnityEngine;
using UnityEngine.UI;

public enum DirectionState
{
    Left,
    Right,
}

public class XMasPlayer : MonoBehaviour
{
    [SerializeField] private float MoveSpeed;
    [SerializeField] private Button LeftButton;
    [SerializeField] private Button RightButton;
    [SerializeField] private GameObject PlayerObj;
    [SerializeField] private GameObject SnowManObj;

    private Rigidbody2D _rigidbody2D;
    private Transform _modelTr;
    private Animator _animator;
    private float _minX;
    private float _maxX;

    private DirectionState _directionState = DirectionState.Right;

    private DirectionState DirectionState
    {
        get => _directionState;
        set
        {
            _directionState = value;
            SetDirectionSprite();
        }
    }

    private bool _isLeft;
    private bool _isRight;
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Idle = Animator.StringToHash("Idle");

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _modelTr = _animator.transform;

        SetLeftButton();
        SetRightButton();
    }

    private void Start()
    {
        SetCameraViewPos();
    }

    private void OnEnable()
    {
        PlayerObj.SetActive(true);
        SnowManObj.SetActive(false);
        LeftButton.transform.parent.gameObject.SetActive(true);
        RightButton.transform.parent.gameObject.SetActive(true);
    }

    private void SetLeftButton()
    {
        LeftButton.BindEvent(() =>
        {
            DirectionState = DirectionState.Left;
            _isLeft = true;
            _animator.SetTrigger(Move);
        }, UIEvent.PointerDown);

        LeftButton.BindEvent(() =>
        {
            _isLeft = false;
            if (_isRight)
                DirectionState = DirectionState.Right;
            else
                _animator.SetTrigger(Idle);
        }, UIEvent.PointerUp);
    }

    private void SetRightButton()
    {
        RightButton.BindEvent(() =>
        {
            DirectionState = DirectionState.Right;
            _isRight = true;
            _animator.SetTrigger(Move);
        }, UIEvent.PointerDown);

        RightButton.BindEvent(() =>
        {
            _isRight = false;
            if (_isLeft)
                DirectionState = DirectionState.Left;
            else
                _animator.SetTrigger(Idle);
        }, UIEvent.PointerUp);
    }

    public void SetCameraViewPos()
    {
        var gameCamera = Camera.main;
        if (gameCamera == null) 
            return;
        
        var minPos = gameCamera.ViewportToWorldPoint(gameCamera.rect.min);
        var maxPos = gameCamera.ViewportToWorldPoint(gameCamera.rect.max);

        _minX = minPos.x;
        _maxX = maxPos.x;
    }

    private void SetDirectionSprite()
    {
        var scale = _modelTr.localScale;
        scale.x = DirectionState == DirectionState.Left ? -1 : 1;
        _modelTr.localScale = scale;
    }

    private void Update()
    {
        if (!Managers.XMasEvent.IsPlaying.Value)
            return;

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DirectionState = DirectionState.Left;
            _isLeft = true;
            _animator.SetTrigger(Move);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            _isLeft = false;
            if (_isRight)
                DirectionState = DirectionState.Right;
            else
                _animator.SetTrigger(Idle);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            DirectionState = DirectionState.Right;
            _isRight = true;
            _animator.SetTrigger(Move);
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            _isRight = false;
            if (_isLeft)
                DirectionState = DirectionState.Left;
            else
                _animator.SetTrigger(Idle);
        }

#endif

        if (transform.position.x < _minX)
        {
            var pos = transform.position;
            pos.x = _minX;
            transform.position = pos;
        }

        if (transform.position.x > _maxX)
        {
            var pos = transform.position;
            pos.x = _maxX;
            transform.position = pos;
        }

        if (!_isLeft && !_isRight)
            return;

        if (DirectionState == DirectionState.Left)
            _rigidbody2D.AddForce(Vector3.left * MoveSpeed);
        else
            _rigidbody2D.AddForce(Vector3.right * MoveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.tag.Equals("XMasPoop")) 
            return;
        
        // 게임 종료
        Managers.Resource.Instantiate("ETC/SnowPoopEffect");
        PlayerObj.SetActive(false);
        SnowManObj.SetActive(true);
        _rigidbody2D.velocity = Vector2.zero;
        LeftButton.transform.parent.gameObject.SetActive(false);
        RightButton.transform.parent.gameObject.SetActive(false);
    }
}