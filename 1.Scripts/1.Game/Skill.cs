using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSKILL_TYPE
{
    Attack,
    Buff,

    Max
}

public abstract class Skill : MonoBehaviour
{
    #region Fields
    protected PlayerController _player;

    public int Level;
    #endregion

    #region Unity Method
    protected virtual void Start()
    {
        _player = BattleMgr.Instance.Player;
    }
    #endregion

    #region Public Methods
    public abstract void Upgrade();
    #endregion
}
