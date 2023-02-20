using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBomb : Item
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
        InGameUIMgr.Instance._gameUI.ShowBoomEffect();
        BattleMgr.Instance.Player.BoomTrigger(10);
    }
    #endregion
}
