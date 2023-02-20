using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjPool<T> : MonoBehaviour where T : Component
{
    #region Fields
    [SerializeField] protected T[] _origin;
    protected List<T>[] _pool;
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        if (_pool == null)
        {
            _pool = new List<T>[_origin.Length];
            for (int i = 0; i < _pool.Length; i++)
            {
                _pool[i] = new List<T>();
            }
        }
    }    
    #endregion

    #region Public Methods
    public virtual T GetFromPool(Transform parent, int index = 0)
    {
        if (_pool == null)
        {
            _pool = new List<T>[_origin.Length];
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

        return MakeNewInstance(parent, index);
    }

    public void UpdateParent(Transform parent, int index = 0)
    {
        for (int i = 0; i < _pool[index].Count; i++)
        {
            if (!_pool[index][i].gameObject.activeInHierarchy)
            {
                _pool[index][i].transform.SetParent(parent);
            }
        }
    }

    public void AddObj(T obj, int index = 0)
    {
        if (!_pool[index].Contains(obj))
        {
            _pool[index].Add(obj);
        }
    }

    public void RemoveObj(T obj, int index = 0)
    {
        if (_pool[index].Contains(obj))
        {
            _pool[index].Remove(obj);
        }
    }

    public virtual T GetFromPool(int index = 0)
    {
        if (_pool == null)
        {
            _pool = new List<T>[_origin.Length];
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

        return MakeNewInstance(index);
    }

    public void AllObjActiveOff()
    {
        if (_pool == null)
        {
            _pool = new List<T>[_origin.Length];
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

    #region Protected Methods
    protected virtual T MakeNewInstance(Transform parent, int index)
    {
        T newObj = Instantiate(_origin[index], parent);
        _pool[index].Add(newObj);
        return newObj;
    }

    protected virtual T MakeNewInstance(int index)
    {
        T newObj = Instantiate(_origin[index]);
        _pool[index].Add(newObj);
        return newObj;
    }
    #endregion
}
