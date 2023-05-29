using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Skill_PassiveGodgod : MonoBehaviour
{
    #region Fields
    private AnimationEventReceiver _animationEventReceiver;
    private CircleCollider2D _hitCollider;

    private double _originDamage;
    private double _normalDamage;
    private double _bossDamage;
    private double _criticalMultiple;
    private double _bossCriticalMultiple;
    private int _teamIndex;

    private Dictionary<int, double> StatDatas = new();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _hitCollider = GetComponent<CircleCollider2D>();
        _animationEventReceiver = GetComponent<AnimationEventReceiver>();

        
        _animationEventReceiver.OnAnimationEventAttack += Attack;
        _animationEventReceiver.OnAnimationEventEndAttack += EndAttack;
    }

    private void OnEnable()
    {
        _hitCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            Monster monster = collision.GetComponent<Monster>();
            if (monster.Type == MonsterType.Normal)
            {
                if (Managers.Stage.State.Value == StageState.WorldCupEvent || Utils.IsHwasengbangDungeon())
                {
                    monster.Damage(1);
                    return;
                }
                monster.Damage(_normalDamage, _criticalMultiple);
            }
            else if (monster.Type == MonsterType.Boss || monster.Type == MonsterType.AllRaidBoss)
            {
                monster.Damage(_bossDamage, _criticalMultiple, _teamIndex);
            }
            else if (monster.Type == MonsterType.StageSpecial)
            {
                monster.Damage(_originDamage, _criticalMultiple);
            }
        }
        //else if(collision.gameObject.CompareTag("Player") && Managers.Stage.State.Value == StageState.Pvp)
        //{
        //    Player player = collision.GetComponent<Player>();
        //    Debug.Log(player.IsPvpEnemyPlayer);

        //    if (_isEnemy && player.IsMine)
        //        player.Damage(_normalDamage, _criticalMultiple);
        //    else if (!_isEnemy && player.IsPvpEnemyPlayer)
        //        player.Damage(_normalDamage, _criticalMultiple);
        //}
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Public Methods
    public void SetDamage(double originDamage, double normalDamage, double bossDamage, double criticalMultiple, int teamIndex = -1)
    {
        _originDamage = originDamage;
        _normalDamage = normalDamage;
        _bossDamage = bossDamage;
        _criticalMultiple = criticalMultiple;
        _teamIndex = teamIndex;
    }

    public void SetDamage(double damage, double criticalMultiple, int teamIndex = -1)
    {
        _normalDamage = damage;
        _criticalMultiple = criticalMultiple;
        _teamIndex = teamIndex;
    }
    #endregion

    #region Private Methods
    private void Attack(float percent)
    {
        _hitCollider.enabled = true;
    }

    private void EndAttack()
    {
        _hitCollider.enabled = false;
        gameObject.SetActive(false);
    }
    #endregion
}
