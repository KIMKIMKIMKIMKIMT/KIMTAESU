using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBullet : MonoBehaviour
{
    #region Fields
    protected Vector3 _dir;

    protected float _dmg;
    protected float _speed;
    #endregion

    #region Unity Methods
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.Hit(_dmg);
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Public Methods
    public void ActiveOff()
    {
        gameObject.SetActive(false);
    }

    public abstract void SetBullet(Vector3 dir, float dmg, float speed, bool reflection, eENEMY_TYPE type);
    #endregion
}
