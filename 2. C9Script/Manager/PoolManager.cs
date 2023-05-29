using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonoBehaviour<PoolManager>
{
    #region Fields
    [SerializeField] private PlayerPool _playerPool;
    [SerializeField] private EffectPool _effectPool;
    #endregion

    #region Unity Methods
    #endregion

    #region Public Methods
    public Player GetPlayer(int index)
    {
        return _playerPool.GetFromPool(index, _playerPool.transform);
    }

    public NewEffect GetEffect(int index)
    {
        return _effectPool.GetFromPool(index, _effectPool.transform);
    }

    public void PlayerAllOffObj()
    {
        _playerPool.AllObjActiveOff();
    }

    public void EffectAllOffObj()
    {
        _effectPool.AllObjActiveOff();
    }
    #endregion
}
