using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : EnemyBullet
{
    #region Fields
    private eENEMY_TYPE _enemyType;

    private int _reflectionCnt;
    private bool _reflection;
    #endregion

    private void Update()
    {
        transform.position += _dir.normalized * _speed * Time.deltaTime;
    }

    #region Unity Methods
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Fence"))
        {
            if (_reflection)
            {
                if (_reflectionCnt == 2)
                {
                    gameObject.SetActive(false);
                }
                _dir.x = Mathf.Cos((_dir.x * (90)) * Mathf.Deg2Rad);
                _dir.y = Mathf.Cos((_dir.x * (90)) * Mathf.Deg2Rad);
                _reflectionCnt++;
            }
            else
            {
                gameObject.SetActive(false);
            }
            
        }
    }
    #endregion

    #region Public Methods
    public override void SetBullet(Vector3 dir, float dmg, float speed, bool reflection, eENEMY_TYPE type)
    {
        _dir = dir;
        _dmg = dmg;
        _speed = speed;
        _reflection = reflection;
        _reflectionCnt = 0;
        _enemyType = type;
    }
    #endregion
}
