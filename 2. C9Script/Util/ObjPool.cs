using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool<T> : MonoBehaviour where T : Component
{
    #region Fields
    [SerializeField] protected T[] _origins;
    protected List<T>[] _pool;
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        if (_pool == null)
        {
            _pool = new List<T>[_origins.Length];

            for (int i = 0; i < _pool.Length; i++)
            {
                _pool[i] = new List<T>();
            }
        }
    }
    #endregion

    #region Protected Methods
    protected virtual T MakeNewInstance(int index, Transform parent)
    {
        T newObj = Instantiate(_origins[index], parent);
        _pool[index].Add(newObj);
        return newObj;
    }

    protected virtual T MakeNewInstance(int index)
    {
        T newObj = Instantiate(_origins[index]);
        _pool[index].Add(newObj);
        return newObj;
    }
    #endregion

    #region Public Methods
    public virtual T GetFromPool(int index, Transform parent)
    {
        if (_pool == null)
        {
            _pool = new List<T>[_origins.Length];

            for (int i = 0; i < _pool.Length; i++)
            {
                _pool[i] = new List<T>();
            }
        }

        for (int i = 0; i < _pool[index].Count; i++)
        {
            if (!_pool[index][i].gameObject.activeInHierarchy)
            {
                _pool[index][i].gameObject.SetActive(true);
                return _pool[index][i];
            }
        }

        return MakeNewInstance(index, parent);
    }

    public void AllObjActiveOff()
    {
        if (_pool == null)
        {
            _pool = new List<T>[_origins.Length];
            for (int i = 0; i < _pool.Length; i++)
            {
                _pool[i] = new List<T>();
            }
        }
        for (int q = 0; q < _pool.Length; q++)
        {
            for (int i = 0; i < _pool[q].Count; i++)
            {
                _pool[q][i].gameObject.SetActive(false);
            }
        }
    }
    #endregion
}
