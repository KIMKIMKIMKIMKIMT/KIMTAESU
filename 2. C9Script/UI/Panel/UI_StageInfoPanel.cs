using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class UI_StageInfoPanel : UI_Panel
{
    #region Fields
    [SerializeField] private Button _closeBtn;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _closeBtn.BindEvent(Close);
    }
    #endregion
}
