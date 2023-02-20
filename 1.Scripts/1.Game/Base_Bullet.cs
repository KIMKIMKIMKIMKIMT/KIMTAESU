using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Bullet : MonoBehaviour
{
    #region Fields
    [SerializeField] private eTANK _tankType;
    private CircleCollider2D _hitBox;
    private Vector3 _dir;
    private Vector3 _scale;
    private float _speed;
    private float _dmg;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _hitBox = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        transform.position += _dir.normalized * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            if (_tankType == eTANK.Tank_2)
            {
                if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
                {
                    SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.DropMissile, true);
                }
                Effect effect = PoolMgr.Instance.GetEffect(7);
                effect.transform.position = transform.position;
                effect.transform.localScale = Vector3.one * BuffSkillController.Buff_BulletScale;
                _hitBox.radius = 1 * BuffSkillController.Buff_BulletScale;

                Enemy enemy = collision.GetComponent<Enemy>();
                enemy.Hit(_dmg);
                Invoke("InitHitBoxRadius", 0.1f);
            }
            else
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                enemy.Hit(_dmg);
                gameObject.SetActive(false);
            }
        }

        if (collision.CompareTag("ItemBox"))
        {
            if (_tankType == eTANK.Tank_2)
            {
                Debug.Log(collision);
                Effect effect = PoolMgr.Instance.GetEffect(7);
                effect.transform.position = transform.position;
                _hitBox.radius = 1 * BuffSkillController.Buff_BulletScale;
                Invoke("InitHitBoxRadius", 0.1f);
            }
            GameItemBox box = collision.GetComponent<GameItemBox>();
            box.Hit(_dmg);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CameraRange"))
        {
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Public Methods
    public void InitHitBoxRadius()
    {
        _hitBox.radius = 0.3f;
        gameObject.SetActive(false);
    }
    public void SetBullet(Vector3 dir, float speed, float dmg)
    {
        _dir = dir;
        _speed = speed;
        _dmg = dmg;
        transform.localScale = Vector3.one * BuffSkillController.Buff_BulletScale;
        
    }
    #endregion

}
