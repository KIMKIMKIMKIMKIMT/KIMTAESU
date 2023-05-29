using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class UI_CostumeCollectionQuesionPanel : UI_Panel
{
    #region Fields
    [SerializeField] private Button _close;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _close.BindEvent(Close);
    }
    #endregion

    #region Public Methods
    public override void Open()
    {
        base.Open();
    }
    #endregion
}
