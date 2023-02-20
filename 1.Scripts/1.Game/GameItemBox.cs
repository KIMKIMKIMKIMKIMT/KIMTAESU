using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemBox : MonoBehaviour
{
    #region Fields
    private ItemPool _pool;
    private float _hp;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _pool = GetComponent<ItemPool>();
    }
    private void OnEnable()
    {
        _hp = 1;
    }
    #endregion

    #region Public Methods
    public void Hit(float dmg)
    {
        _hp -= dmg;
        DmgTxt txt = PoolMgr.Instance.GetDmgTxt(0);
        txt.SetText((int)dmg, transform.position);
        if (_hp <= 0)
        {
            Destruction();
            Invoke("ActiveOff", 0.1f);
        }
    }
    public void Destruction()
    {
        int ran = Random.Range(0, 10);

        if (ran == 0)
        {
            Item bomb = _pool.GetFromPool((int)eITEM_TYPE.Bomb);
            bomb.transform.position = transform.position;
        }
        else if (ran == 1)
        {
            Item magnet = _pool.GetFromPool((int)eITEM_TYPE.Magnet);
            magnet.transform.position = transform.position;
        }
        else if (ran == 2)
        {
            Item potion = _pool.GetFromPool((int)eITEM_TYPE.Potion);
            potion.transform.position = transform.position;
        }
        else if (ran == 3)
        {
            Item goldBundle = _pool.GetFromPool((int)eITEM_TYPE.GoldBundle);
            goldBundle.transform.position = transform.position;
        }
        else
        {
            Item gold = _pool.GetFromPool((int)eITEM_TYPE.Gold);
            gold.transform.position = transform.position;
        }
    }
    public void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
