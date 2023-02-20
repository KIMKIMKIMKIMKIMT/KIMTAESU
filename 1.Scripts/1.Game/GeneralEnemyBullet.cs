using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEnemyBullet : EnemyBullet
{
    #region Unity Methods
    private void OnEnable()
    {
        Invoke("ActiveOff", 3f);
    }

    private void Update()
    {
        transform.position += _dir.normalized * _speed * Time.deltaTime;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
    #endregion

    #region Public Methods
    public override void SetBullet(Vector3 dir, float dmg, float speed, bool reflection, eENEMY_TYPE type)
    {
        _dir = dir;
        _dmg = dmg;
        _speed = speed;
    }
    #endregion
}
