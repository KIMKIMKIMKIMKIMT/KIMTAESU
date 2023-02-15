using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OkCancelPopup : MonoBehaviour
{
    #region Fields
    public delegate void OkDel();
    public OkDel _del;

    [SerializeField] private Text _title;
    [SerializeField] private Text _body;
    #endregion

    #region Unity Methods
    #endregion

    #region Public Methods
    public void SetUI(string title, string body, OkDel del)
    {
        _title.text = title;
        _body.text = body;
        _del = del;
    }

    public void OnClickOk()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PopupMgr.Instance.RemovePopup();
        if (_del != null)
        {
            _del();
        }
        _del = null;
    }

    public void OnClickCancel()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        _del = null;
        PopupMgr.Instance.RemovePopup();
    }
    #endregion
}
