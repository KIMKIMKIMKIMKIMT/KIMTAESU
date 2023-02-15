using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    #region Fields
    private CircleCollider2D _hitBox;
    private SpriteRenderer _spriteRenderer;

    private float _maxTime;
    private float _time;
    private float _dmg;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        _hitBox = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _hitBox.enabled = true;
        _hitBox.radius = 0.5f * BuffSkillController.Buff_BulletScale;
        _spriteRenderer.enabled = true;
    }
    private void Update()
    {
        _time += Time.deltaTime;
        if (_time > _maxTime)
        {
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.Mine, true);
            }
            _time = 0;
            _spriteRenderer.enabled = false;
            _hitBox.radius = 1f * BuffSkillController.Buff_BulletScale;

            Effect effect = PoolMgr.Instance.GetEffect(3);
            effect.transform.position = transform.position;
            Invoke("ActiveOff", 0.2f);
            
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.Mine, true);
            }
            _time = 0;
            _spriteRenderer.enabled = false;
            _hitBox.radius = 1f * BuffSkillController.Buff_BulletScale;


            Effect effect = PoolMgr.Instance.GetEffect(3);
            effect.transform.position = transform.position;
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.Hit(_dmg);

            
            Invoke("ActiveOff", 0.2f);
        }
    }
    #endregion

    #region Public Methods
    public void SetBullet(float dmg, float time)
    {
        _dmg = dmg;
        _maxTime = time;
        transform.localScale = Vector3.one * BuffSkillController.Buff_BulletScale;
    }
    public void ActiveOff()
    {
        _hitBox.radius = 0.5f * BuffSkillController.Buff_BulletScale;
        gameObject.SetActive(false);
    }
    #endregion
}
