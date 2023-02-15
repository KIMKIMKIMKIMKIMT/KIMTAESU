using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSlot : FusionSlot
{
    #region Fields
    public Collider2D Collider { get; private set; }
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        Collider = GetComponent<Collider2D>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        ColliderSet(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
    #endregion

    #region Public Methods
    public void ColliderSet(bool enable)
    {
        Collider.enabled = enable;
    }
    public override void ClearObj()
    {
        base.ClearObj();
    }
    #endregion
}
