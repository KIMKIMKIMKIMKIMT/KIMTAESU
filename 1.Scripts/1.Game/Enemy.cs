using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eENEMY_TYPE
{
    ShortDistanceEnemy1,
    ShortDistanceEnemy2,
    ShortDistanceEnemy3,
    LongDistanceEnemy1,
    LongDistanceEnemy2,
    MiniBoss1,
    Boss1,
    Boss2,
    Boss3,

    Max
}
public abstract class Enemy : MonoBehaviour
{
    #region Fields
    protected EnemyData _enemyData;

    protected PlayerController _player;
    protected Rigidbody2D _rigid;
    protected Collider2D _hitBox;

    [SerializeField] protected float _speed;
    [SerializeField] protected float _hp;
    [SerializeField] public float _atk;
    #endregion

    #region Unity Methods
    protected virtual void Start()
    {
        _player = BattleMgr.Instance.Player;
        _rigid = GetComponent<Rigidbody2D>();
        _hitBox = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        transform.rotation = LookAt2D(_player.transform.position - transform.position);
    }
    protected virtual void FixedUpdate()
    {
        Vector3 direction = _player.transform.position - transform.position;
        _rigid.AddForce(direction.normalized * _speed, ForceMode2D.Force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HitTrigger"))
        {
            _hitBox.enabled = false;
            Invoke("HitBoxTrigger", 0.1f);
            _player.Hit(_atk);
        }
        if (collision.CompareTag("BoomTrigger"))
        {
            Hit(100000);
        }
    }

    
    #endregion

    #region Public Methods
    public Quaternion LookAt2D(Vector2 forward)
    {
        float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 10f * Time.deltaTime);
        return rotation;
    }

    public abstract void Hit(float dmg);
    public abstract void KnockBack(Vector3 dir, float power);

    public void EnemyDataUpgrade(float index)
    {
        _hp = _enemyData.Hp * index;
        _atk = _enemyData.Atk * index;
    }

    #endregion

    #region Protected Methods
    protected abstract void Die();
    protected abstract void SetEnemyData(EnemyData enemyData);

    protected void HitBoxTrigger()
    {
        _hitBox.enabled = true;
    }
    #endregion


}
