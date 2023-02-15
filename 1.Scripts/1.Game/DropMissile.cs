using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMissile : MonoBehaviour
{
    #region Fields
    private SpriteRenderer _spriteRenderer;
    private CapsuleCollider2D _hitBox;

    private Vector3 _targetPosition;

    private float _speed;
    private float _dmg;
    private bool _isAttack;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _hitBox = GetComponent<CapsuleCollider2D>();
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

    private void Update()
    {
        if (_targetPosition.y >= transform.position.y && !_isAttack)
        {
            _isAttack = true;
            Attack();
        }
        else
            transform.position += Vector3.down * _speed * Time.deltaTime;
    }
    #endregion

    #region Public Methods
    public void SetBullet(Vector2 pos ,float dmg, float speed)
    {
        _isAttack = false;
        _spriteRenderer.enabled = true;
        _hitBox.enabled = false;

        _targetPosition = pos;
        _dmg = dmg;
        _speed = speed;
        transform.localScale = Vector3.one * BuffSkillController.Buff_BulletScale;
    }

    public void Attack()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.DropMissile, true);
        }
        Effect effect = PoolMgr.Instance.GetEffect((int)eEFFECT_LIST.DropMissile);
        effect.transform.position = transform.position;
        effect.transform.localScale = Vector3.one * BuffSkillController.Buff_BulletScale;

        _spriteRenderer.enabled = false;
        _hitBox.enabled = true;
        Invoke("ActiveOff", 0.2f);
    }

    public void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
