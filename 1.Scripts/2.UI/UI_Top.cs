using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Top : MonoBehaviour
{
    #region Fields
    [SerializeField] private Image _goldIcon;
    [SerializeField] private Image _gemIcon;

    [SerializeField] private Text _gemTxt;
    [SerializeField] private Text _goldTxt;

    [SerializeField] private Text _userNickname;
    [SerializeField] private Text _userLevel;
    [SerializeField] private Image _expBar;
    public Vector3 GetGoldPosition => _goldIcon.transform.position;
    public Vector3 GetGemPosition => _gemIcon.transform.position;
    #endregion

    #region Unity Methods
    private void Start()
    {
        GameMgr.Instance.UserExpCheck();
        Refresh();
    }
    #endregion

    #region Public Methods
    public void SetExpFillAmount(float fillamount)
    {
        _expBar.fillAmount = fillamount;
    }
    public void InitUserExpFillAmount()
    {
        _expBar.fillAmount = 0;
    }
    public void Refresh()
    {
        _userNickname.text = PlayerDataMgr.Instance.PlayerData.UserNickname;
        _userLevel.text = PlayerDataMgr.Instance.PlayerData.UserLevel.ToString();
        _goldTxt.text = PlayerDataMgr.Instance.PlayerData.Gold.ToString();
        _gemTxt.text = PlayerDataMgr.Instance.PlayerData.Gem.ToString();
    }
    #endregion
}
