using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.TextCore.Text;

public partial class Player
{
    #region 기본 공격

    public void OnAnimationEvent_Attack()
    {
        if (State.Value == CharacterState.None)
            return;

        if (IsMine && IsDead)
            return;

        OnAnimationAttackEvent?.Invoke();
        Managers.Sound.PlayAttackSound();

        double damage;
        double criticalMultiple;

        if (IsMine)
        {
            if (Managers.Game.SkillDatas.ContainsKey(57))
            {
                if (Managers.Game.SkillDatas[57].Level > 0)
                {
                    PassiveGodgodCount++;

                    if (PassiveGodgodCount >= 3)
                    {
                        PassiveGodgodCount = 0;

                        double passiveDamage = Managers.Stage.State.Value != StageState.Pvp ? Utils.CalculatePassiveSkillDamage(57, out criticalMultiple)
                            : Utils.CalculatePvpAttackDamage(out criticalMultiple, 57, true);

                        double passiveDamageNormal = passiveDamage;
                        double passiveDamageBoss = passiveDamage;

                        if (passiveDamage != 1 && Managers.Stage.State.Value != StageState.Pvp)
                        {
                            passiveDamageNormal = passiveDamage * Managers.Game.BaseStatDatas[(int)StatType.NormalMonsterDamage];
                            passiveDamageBoss = passiveDamage * Managers.Game.BaseStatDatas[(int)StatType.BossMonsterDamage];
                        }
                        else if(Managers.Stage.State.Value == StageState.Pvp)
                        {
                            Managers.Pvp.TotalMyDamage.Value += passiveDamage;
                            Managers.Pvp.EnemyPlayer.Damage(passiveDamage, criticalMultiple);
                        }
                            
                        NewEffect effect = PoolManager.Instance.GetEffect(0);
                        effect.GetComponent<Skill_PassiveGodgod>().SetDamage(passiveDamage, passiveDamageNormal, passiveDamageBoss, criticalMultiple, GuildSportsTeamIndex);
                        effect.transform.position = new Vector2(transform.position.x + (Direction.Value == CharacterDirection.Left ?
                            -2 : 2), transform.position.y + 3);

                    }
                }
            }
        }
        else if (IsPvpEnemyPlayer)
        {
            if (Managers.Pvp.EnemyPassiveGodgodLv > 0)
            {
                IsPvpEnemyPlayerPassiveGodgodCount++;

                if (IsPvpEnemyPlayerPassiveGodgodCount >= 3)
                {
                    IsPvpEnemyPlayerPassiveGodgodCount = 0;
                    double pvpDamage = Utils.CalculatePvpEnemyAttackDamage(out criticalMultiple, 57, true);

                    Managers.Game.MainPlayer.Damage(pvpDamage, criticalMultiple);
                    Managers.Pvp.TotalEnemyDamage.Value += pvpDamage;

                    NewEffect effect = PoolManager.Instance.GetEffect(0);
                    //effect.GetComponent<Skill_PassiveGodgod>().SetDamage(pvpDamage, criticalMultiple);
                    effect.transform.position = new Vector2(transform.position.x + (Direction.Value == CharacterDirection.Left ?
                        -3 : 3), transform.position.y + 3);
                }
            }

            
        }
        else
        {
            if (PartyPlayer_Passive_Level > 0)
            {
                PartyPlayerPassiveGodgodCount++;

                if (PartyPlayerPassiveGodgodCount >= 3)
                {
                    PartyPlayerPassiveGodgodCount = 0;

                    double passiveDamage = Utils.CalculatePassiveSkillDamage(57, PartyPlayer_Passive_Level, out criticalMultiple, StatDatas);
                    double passiveDamageNormal = passiveDamage * StatDatas[(int)StatType.NormalMonsterDamage];
                    double passiveDamageBoss = passiveDamage * StatDatas[(int)StatType.BossMonsterDamage];
                    NewEffect effect = PoolManager.Instance.GetEffect(0);
                    effect.GetComponent<Skill_PassiveGodgod>().SetDamage(passiveDamage, passiveDamageNormal, passiveDamageBoss, criticalMultiple, GuildSportsTeamIndex);
                    effect.transform.position = new Vector2(transform.position.x + (Direction.Value == CharacterDirection.Left ?
                        -2 : 2), transform.position.y + 3);
                }
            }
        }

        
        
        

        switch (Managers.Stage.State.Value)
        {
            case StageState.Pvp:
            {
                // 상대 플레이어
                if (IsPvpEnemyPlayer)
                {
                    damage = Utils.CalculatePvpEnemyAttackDamage(out criticalMultiple);
                    Managers.Game.MainPlayer.Damage(damage, criticalMultiple);
                    Managers.Pvp.TotalEnemyDamage.Value += damage;
                }
                // 내 플레이어
                else
                {
                    damage = Utils.CalculatePvpAttackDamage(out criticalMultiple);
                    Managers.Pvp.EnemyPlayer.Damage(damage, criticalMultiple);
                    Managers.Pvp.TotalMyDamage.Value += damage;
                }
            }
                break;
            case StageState.Dps:
            {
                if (Managers.Monster.DpsMonster == null)
                    return;
                
                damage = Utils.CalculateAttackDamage(out criticalMultiple, Managers.Monster.DpsMonster.Type);
                Managers.Monster.DpsMonster.Damage(damage, criticalMultiple);
            }
                break;
            case StageState.GuildSports:
                {
                    if (Managers.Monster.GuildSportMonster == null)
                        return;

                    if (Managers.GuildSports.IsEndFlag)
                        return;

                    if (GuildSportsTeamIndex == 0)
                    {
                        damage =
                                IsMine ? Utils.CalculateAttackDamage(out criticalMultiple, TargetMonster.Type)
                                    : Utils.CalculateAttackDamage(out criticalMultiple, StatDatas, MonsterType.Boss);
                        Managers.Monster.GuildSportMonster[0].Damage(damage, criticalMultiple, GuildSportsTeamIndex);
                    }
                    else
                    {
                        damage = Utils.CalculateAttackDamage(out criticalMultiple , StatDatas, MonsterType.Boss);
                        Managers.Monster.GuildSportMonster[1].Damage(damage, criticalMultiple, GuildSportsTeamIndex);
                    }
                }
                break;
            case StageState.GuildRaid:
            {
                if (IsFeverMode.Value)
                {
                    if (TargetMonster != null)
                    {
                        damage = 
                            IsMine ? Utils.CalculateAttackDamage(out criticalMultiple, TargetMonster.Type) 
                                : Utils.CalculateAttackDamage(out criticalMultiple, StatDatas, TargetMonster.Type);
                        TargetMonster.Damage(damage, criticalMultiple);
                    }

                    Managers.Monster.FindTargetMonsters(CenterPos, Direction.Value, _attackRange).ForEach(monster =>
                    {
                        if (TargetMonster != null && monster == TargetMonster)
                            return;
                        
                        damage = IsMine ? Utils.CalculateAttackDamage(out criticalMultiple, TargetMonster.Type) 
                            : Utils.CalculateAttackDamage(out criticalMultiple, StatDatas, TargetMonster.Type);
                        monster.Damage(damage, criticalMultiple);
                    });
                }
                else
                {
                    if (TargetMonster != null)
                    {
                        damage = IsMine ? Utils.CalculateAttackDamage(out criticalMultiple, TargetMonster.Type) 
                            : Utils.CalculateAttackDamage(out criticalMultiple, StatDatas, TargetMonster.Type);
                        TargetMonster.Damage(damage, criticalMultiple);
                    }
                }

                if (TargetMonster == null)
                    return;

                if (TargetMonster.IsDead)
                    TargetMonster = null;
            }
                break;
            case StageState.GuildAllRaid:
                {
                    if (IsFeverMode.Value)
                    {
                        if (TargetMonster != null)
                        {
                            damage =
                                IsMine ? Utils.CalculateAttackDamage(out criticalMultiple, TargetMonster.Type)
                                    : Utils.CalculateAttackDamage(out criticalMultiple, StatDatas, TargetMonster.Type);
                            TargetMonster.Damage(damage, criticalMultiple);
                        }

                        Managers.Monster.FindTargetMonsters(CenterPos, Direction.Value, _attackRange).ForEach(monster =>
                        {
                            if (TargetMonster != null && monster == TargetMonster)
                                return;

                            damage = IsMine ? Utils.CalculateAttackDamage(out criticalMultiple, TargetMonster.Type)
                                : Utils.CalculateAttackDamage(out criticalMultiple, StatDatas, TargetMonster.Type);
                            monster.Damage(damage, criticalMultiple);
                        });
                    }
                    else
                    {
                        if (TargetMonster != null)
                        {
                            damage = IsMine ? Utils.CalculateAttackDamage(out criticalMultiple, TargetMonster.Type)
                                : Utils.CalculateAttackDamage(out criticalMultiple, StatDatas, TargetMonster.Type);
                            TargetMonster.Damage(damage, criticalMultiple);
                        }
                    }

                    if (TargetMonster == null)
                        return;

                    if (TargetMonster.IsDead)
                        TargetMonster = null;
                }
                break;
            default:
            {
                if (IsFeverMode.Value)
                {
                    if (TargetMonster != null)
                    {
                        damage = Utils.CalculateAttackDamage(out criticalMultiple, TargetMonster.Type);
                        TargetMonster.Damage(damage, criticalMultiple);
                    }

                    Managers.Monster.FindTargetMonsters(CenterPos, Direction.Value, _attackRange).ForEach(monster =>
                    {
                        if (TargetMonster != null && monster == TargetMonster)
                            return;
                        
                        damage = Utils.CalculateAttackDamage(out criticalMultiple, monster.Type);
                        monster.Damage(damage, criticalMultiple);
                    });
                }
                else
                {
                    if (TargetMonster != null)
                    {
                        damage = Utils.CalculateAttackDamage(out criticalMultiple, TargetMonster.Type);
                        TargetMonster.Damage(damage, criticalMultiple);
                    }
                }

                if (TargetMonster == null)
                    return;

                if (TargetMonster.IsDead)
                    TargetMonster = null;
            }
                break;
            
        }
    }

    public void OnAnimationEvent_EndAttack()
    {
        if (EndEvent != null)
        {
            EndEvent.Invoke();
            EndEvent = null;
        }
        StartCoroutine(CoAttackDelay());
    }

    private IEnumerator CoAttackDelay()
    {
        yield return new WaitForSeconds((1 / AttackSpeed.Value) + 0.25f);

        if (State.Value != CharacterState.Attack)
            yield break;

        State.Value = CharacterState.Idle;
    }

    #endregion

    #region 스킬(2, 7)

    public void OnEndSkillAnimation(int skillId)
    {
        TargetMonster = null;
        
        if (UsingSkillIds.Contains(skillId))
            UsingSkillIds.Remove(skillId);

        if (EndEvent != null)
        {
            EndEvent.Invoke();
            EndEvent = null;
        }

        if (_nextSkillAnimations.Count > 0)
        {
            var nextAnimation = _nextSkillAnimations.Dequeue();

            if (nextAnimation != null)
                nextAnimation.Invoke();
            else if (State.Value == CharacterState.Skill)
                State.Value = CharacterState.Idle;
        }
        else if (State.Value == CharacterState.Skill)
        {
            State.Value = CharacterState.Idle;
        }
            
    }

    public void OnSkill2_Move()
    {
        if (TargetMonster == null)
            TargetMonster = Managers.Monster.FindTargetMonster(CenterPos);

        if (TargetMonster == null)
            return;

        Vector3 movePos = TargetMonster.CenterPos;

        if (Direction.Value == CharacterDirection.Left)
        {
            movePos.x += 2f;
        }
        else
        {
            movePos.x -= 2;
        }

        transform.position = movePos;
    }

    public void OnSkill2Damage()
    {
        if (TargetMonster == null)
            return;

        var damage = Utils.CalculateSkillDamage(2, out var criticalMultiple, TargetMonster.Type);
        
        TargetMonster.Damage(damage, criticalMultiple);
    }

    public void PlaySkillSound(int skillId)
    {
        Managers.Sound.PlaySkillSound(2);
    }

    #endregion
}