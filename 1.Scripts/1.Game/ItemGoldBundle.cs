using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGoldBundle : Item
{
    #region Unity Methods
    protected override void Awake()
    {
        _tween = GetComponent<Tweener>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
    #endregion

    #region Protected Methods
    protected override void ItemTrigger()
    {
        BattleMgr.Instance.StagaGold += 3000;
    }
    #endregion
}
