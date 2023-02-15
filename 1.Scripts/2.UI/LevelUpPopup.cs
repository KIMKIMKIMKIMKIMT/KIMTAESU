using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPopup : MonoBehaviour
{
    #region Fields
    [SerializeField] private Text _levelTxt;
    [SerializeField] private Button _quitBtn;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        _levelTxt.text = PlayerDataMgr.Instance.PlayerData.UserLevel.ToString();
        _quitBtn.interactable = false;
        Invoke("ButtonActive", 1);
    }
    #endregion

    #region Public Methods
    public void ButtonActive()
    {
        _quitBtn.interactable = true;
    }
    public void OnClickBtn()
    {
        UIMgr.Instance.Reward.gameObject.SetActive(true);
        UIMgr.Instance.Reward.AddAsset(eASSET_TYPE.Gem, 50);
        UIMgr.Instance.Reward.ShowAsset();

        PopupMgr.Instance.RemovePopup();
    }
    #endregion
}
