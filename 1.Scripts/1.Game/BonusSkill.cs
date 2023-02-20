using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusSkill : MonoBehaviour
{
    #region Fields
    [SerializeField] private Image _icon;

    public eATTACKSKILL_LIST _attackSkill { get; private set; }
    public eBUFFSKILL_LIST _buffSkill { get; private set; }
    #endregion

    #region Unity Methods
    private void OnDisable()
    {
        _attackSkill = eATTACKSKILL_LIST.None;
        _buffSkill = eBUFFSKILL_LIST.None;
    }
    #endregion

    #region Public Methods
    public void SetSkill(eATTACKSKILL_LIST type)
    {
        _attackSkill = type;
        _buffSkill = eBUFFSKILL_LIST.None;
        _icon.sprite = SpriteMgr.Instance._atkSkillkImg[(int)_attackSkill];
    }
    public void SetSkill(eBUFFSKILL_LIST type)
    {
        _buffSkill = type;
        _attackSkill = eATTACKSKILL_LIST.None;
        _icon.sprite = SpriteMgr.Instance._buffSkillkImg[(int)_buffSkill];
    }
    #endregion
}
