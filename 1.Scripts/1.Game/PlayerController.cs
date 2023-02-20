using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Fields
    [SerializeField] private CircleCollider2D _boomTrigger;
    [SerializeField] private CircleCollider2D _magnetTrigger;
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _firePos;
    public PlayerSkillController SkillController { get; private set; }
    private Tweener _fireTween;
    public Transform Target { get; private set; }
    private Rigidbody2D _rigid;

    [Header("HUD")]
    [SerializeField] private Image _hpBar;

    public Transform Head { get { return _head; } }
    public Vector3 GetFirePos { get { return _firePos.position; } }
    public Vector3 HeadDir { get; private set; }

    [SerializeField] private float _speed;
    [SerializeField] private float _range;
    private float _hp;
    private float _currentHp;
    public bool IsDie { get; private set; }
    [SerializeField] private eTANK _tankType;



    #endregion

    #region Unity Methods
    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _fireTween = GetComponentInChildren<Tweener>();
        SkillController = GetComponentInParent<PlayerSkillController>();
        IsDie = false;
        _hp = PlayerDataMgr.Instance.PlayerData.GetCurrentTankData().Hp;
        _currentHp = _hp;


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    private void Update()
    {
        if (JoyStick.Instance.IsDrag)
        {
            Vector3 joystickDir = JoyStick.Instance.GetMoveDir();
            _body.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(joystickDir.y, joystickDir.x) * Mathf.Rad2Deg + 90));
            SoundMgr.Instance.MoveSfxPlay();
        }
        else
        {
            SoundMgr.Instance.MoveSfxStop();
        }
        

        SetHpBarFillAmount(_currentHp / _hp);
    } 

    private void FixedUpdate()
    {
        if (JoyStick.Instance.IsDrag)
        {
            _rigid.AddForce(JoyStick.Instance.GetMoveDir() * _speed * BuffSkillController.Buff_Speed * Time.fixedDeltaTime, ForceMode2D.Force);
        }
        else
        {
            _rigid.velocity = Vector2.zero;
        }
        // 1 << LayerMask.NameToLayer("Enemy")
        Collider2D[] nearestEnemy = Physics2D.OverlapCircleAll(transform.position, _range, LayerMask.GetMask("Enemy", "Enemy1"));

        float min = Mathf.Infinity;

        for (int i = 0; i < nearestEnemy.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, nearestEnemy[i].transform.position);
            if (distance < min)
            {
                min = distance;
                Target = nearestEnemy[i].transform;
            }
        }

        if (Target != null)
        {
            HeadDir = Target.position - transform.position;
            _head.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(HeadDir.y, HeadDir.x) * Mathf.Rad2Deg + 270));
            float dis = Vector3.Distance(transform.position, Target.position);

            if (dis > _range)
            {
                Target = null;
            }
        }
    }
    #endregion

    #region Public Methods
    public void BoomTrigger(float radius)
    {
        _boomTrigger.enabled = true;
        _boomTrigger.radius = radius;
        Invoke("BoomTriggerOff", 0.1f);
    }
    public void BoomTriggerOff()
    {
        _boomTrigger.enabled = false;
    }
    public void MagnetTrigger()
    {
        _magnetTrigger.enabled = true;
        Invoke("MagnetTriggerOff", 0.1f);
    }
    public void MagnetTriggerOff()
    {
        _magnetTrigger.enabled = false;
    }
    public void KnockBack(Vector3 dir)
    {
        _rigid.AddForce(dir * Time.fixedDeltaTime * 2f,ForceMode2D.Force);
    }
    public void AttackTween()
    {
        _fireTween.StartTween();
    } 
    public void SetHpBarFillAmount(float fillAmount)
    {
        _hpBar.fillAmount = fillAmount;
    }
    public void SetPlayerHpHeal(float heal)
    {
        _currentHp += heal;
        if (_currentHp > _hp)
        {
            _currentHp = _hp;
        }
    }
    public void Hit(float dmg)
    {
        _currentHp -= dmg;
        if (_currentHp <= 0 && !IsDie)
        {
            IsDie = true;
            BattleMgr.Instance.GameOver(false);
        }
    }
    #endregion
}
