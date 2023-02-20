 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissile : MonoBehaviour
{
    #region Fields
    private SpriteRenderer _spriteRenderer;
    private Collider2D _hitBox;
    [SerializeField] private AnimationCurve _animCur;

    private Vector3 _startPos;
    private Vector3 _finalPos;

    private Vector3 _scale = new Vector3(1, 0.5f, 1);
    private Vector3[] point = new Vector3[3];
    [SerializeField] [Range(0, 1)] private float _t = 0;
    [SerializeField] private float spd;
    [SerializeField] private float posA = 2f;
    [SerializeField] private float posB = 2f;

    private float _hoverHeight;
    private float _dmg;

    private float _startDistance;
    private float _endDistance;

    private bool _isAttack;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _hitBox = GetComponent<Collider2D>();

        
    }

    private void Update()
    {
        if (_t < 1)
        {
            _t += Time.deltaTime * spd;
            DrawTrajectory();
        }
        if (_t >= 1)
        {
            transform.position = _finalPos;
        }

        if (transform.position == _finalPos && !_isAttack)
        {
            Attack();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.Hit(_dmg);
        }
        if (collision.CompareTag("ItemBox"))
        {
            GameItemBox box = collision.GetComponent<GameItemBox>();
            box.Hit(_dmg);
        }
    }
    #endregion

    #region Public Methods
    public Quaternion LookAt2D(Vector2 forward)
    {
        float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 20f * Time.deltaTime);
        return rotation;
    }
    public void Attack()
    {
        _isAttack = true;
        _hitBox.enabled = true;
        _spriteRenderer.enabled = false;
        Effect effect = PoolMgr.Instance.GetEffect(5);
        effect.transform.position = transform.position;

        Invoke("ActiveOff", 0.1f);
    }

    


    Vector2 PointSetting(Vector2 origin, Vector3 dir)
    {
        
        float x, y;
        x = posA * Mathf.Cos((dir.x * (_hoverHeight * (40))) * Mathf.Deg2Rad)
                + origin.x;
        y = posB * Mathf.Sin((dir.y * (_hoverHeight * (40))) * Mathf.Deg2Rad)
                + origin.y;
        return new Vector3(x, y, 0);
    }

    private void DrawTrajectory()
    {
        Vector3 a  = new Vector3(
            FourPointBezier(point[0].x, point[1].x, point[2].x),
            FourPointBezier(point[0].y, point[1].y, point[2].y),0
        );
        transform.rotation = LookAt2D(a - transform.position);
        transform.position = a;
    }

    private float FourPointBezier(float a, float b, float c)
    {
        return Mathf.Pow((1 - _t), 3) * a
                + Mathf.Pow((1 - _t), 2) * 3 * _t * b
                + Mathf.Pow(_t, 2) * 3 * (1 - _t) * c
                + Mathf.Pow(_t, 3) * c;
    }


    public void SetBullet(Vector3 finalPos, Vector3 dir, float speed, float dmg, int height)
    {
        _isAttack = false;
        _hitBox.enabled = false;
        _spriteRenderer.enabled = true;
        _startPos = transform.position;
        _finalPos = finalPos;
        _dmg = dmg;
        _hoverHeight = height;
        transform.localScale = _scale * BuffSkillController.Buff_BulletScale;
        spd = speed;
        _t = 0;
        point[0] = _startPos; // P0
        point[1] = PointSetting(_startPos, dir);
        point[2] = finalPos; // P3
    }

    public void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
