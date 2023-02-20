using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMissile : MonoBehaviour
{
    #region Fields

    private Collider2D _hitBox;

    private float _dmg;
    private float _speed;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _hitBox = GetComponent<Collider2D>();
    }
    private void OnEnable()
    {
        Invoke("Attack", 0.1f);
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
    public void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    public void Attack()
    {
        Effect effect = PoolMgr.Instance.GetEffect((int)eEFFECT_LIST.RangeExplosion);
        effect.transform.position = transform.position;

        Invoke("ActiveOff", 0.2f);
    }
    public void SetBullet(float dmg, float speed)
    {
        _dmg = dmg;
        _speed = speed;
        transform.localScale = Vector3.one * BuffSkillController.Buff_BulletScale;
    }
    #endregion
}
