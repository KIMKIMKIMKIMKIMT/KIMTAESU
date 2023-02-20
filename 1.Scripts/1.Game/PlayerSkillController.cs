using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    #region Fields
    [SerializeField] public AttackSkill[] _atkSkills;
    [SerializeField] public BuffSkill[] _buffSkills;

    public List<eATTACKSKILL_LIST> RemainAttackSkillList;
    public List<eBUFFSKILL_LIST> RemainBuffSkillList;
    public Dictionary<eATTACKSKILL_LIST, AttackSkill> CurrentAttackSkillTable { get; private set; }
    public Dictionary<eBUFFSKILL_LIST, BuffSkill> CurrentBuffSkillTable { get; private set; }
    public List<eATTACKSKILL_LIST> CurrentAttackSkillList { get; private set; }
    public List<eBUFFSKILL_LIST> CurrentBuffSkillList { get; private set; }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        RemainAttackSkillList = new List<eATTACKSKILL_LIST>();
        for (int i = 0; i < _atkSkills.Length; i++)
        {
            RemainAttackSkillList.Add((eATTACKSKILL_LIST)i);
        }
        CurrentAttackSkillTable = new Dictionary<eATTACKSKILL_LIST, AttackSkill>();

        RemainBuffSkillList = new List<eBUFFSKILL_LIST>();
        for (int i = 0; i < _buffSkills.Length; i++)
        {
            RemainBuffSkillList.Add((eBUFFSKILL_LIST)i);
        }
        CurrentBuffSkillTable = new Dictionary<eBUFFSKILL_LIST, BuffSkill>();
        CurrentAttackSkillList = new List<eATTACKSKILL_LIST>();
        CurrentBuffSkillList = new List<eBUFFSKILL_LIST>();

        CurrentAttackSkillTable.Add(eATTACKSKILL_LIST.MainWeapon, _atkSkills[(int)eATTACKSKILL_LIST.MainWeapon]);
        CurrentAttackSkillList.Add(eATTACKSKILL_LIST.MainWeapon);
    }
    #endregion

    #region Public Methods
    public void GetSKill(eATTACKSKILL_LIST key)
    {
        if (CurrentAttackSkillTable.ContainsKey(key))
        {
            CurrentAttackSkillTable[key].Upgrade();
            if (CurrentAttackSkillTable[key].Level >= 5)
            {
                RemainAttackSkillList.Remove(key);
            }
        }
        else
        {
            _atkSkills[(int)key].gameObject.SetActive(true);
            CurrentAttackSkillTable.Add(key, _atkSkills[(int)key]);
            CurrentAttackSkillList.Add(key);
        }
    }

    public void GetSkill(eBUFFSKILL_LIST key)
    {
        if (CurrentBuffSkillTable.ContainsKey(key))
        {
            CurrentBuffSkillTable[key].Upgrade();
            if (CurrentBuffSkillTable[key].Level >= 5)
            {
                RemainBuffSkillList.Remove(key);
            }
        }
        else
        {
            _buffSkills[(int)key].gameObject.SetActive(true);
            CurrentBuffSkillTable.Add(key, _buffSkills[(int)key]);
            CurrentBuffSkillTable[key].Upgrade();
            CurrentBuffSkillList.Add(key);
        }
    }
    #endregion
}
