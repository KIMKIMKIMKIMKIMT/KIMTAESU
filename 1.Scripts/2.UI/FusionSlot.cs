using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionSlot : MonoBehaviour
{
    #region Fields
    public FusionDragObj Obj { get; private set; }

    private FusionPopup _fusionPopup;
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        _fusionPopup = GetComponentInParent<FusionPopup>();
    }
    protected virtual void OnEnable()
    {
        Obj = null;
    }
    protected virtual void OnDisable()
    {
        ClearObj();
    }
    #endregion

    #region Public Methods
    public void SetObj(FusionDragObj obj)
    {
        Obj = obj;
    }
    public virtual void ClearObj()
    {
        Obj = null;
    }
    #endregion

}
