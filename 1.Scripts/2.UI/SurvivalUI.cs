using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SurvivalUI : MonoBehaviour
{
    #region Fields
    private RankBarPool _pool;
    
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _pool = GetComponentInChildren<RankBarPool>();
    }
    private void OnEnable()
    {
        SetRankerList();
    }
    #endregion

    #region Public Methods
    public async void SetRankerList()
    {
        _pool.AllObjActiveOff();
        List<RankData> data = await PlayerDataMgr.Instance.GetRankingData();
        List<RankData> newData = data.OrderByDescending(num => num.Time).ToList();

        
        for (int i = 0; i < 3; i++)
        {
            RankBar rankbar = _pool.GetFromPool(_pool.transform);
            
            rankbar.SetRankBar(i + 1, newData[i].UserNickname, newData[i].Time);
            rankbar._medalImgs[i].SetActive(true);
        }
    }
    public void OnClickRankPopup()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        UIMgr.Instance._ui_Popup.ShowRankPopup();
    }
    public void OnClickPlay()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        GameMgr.Instance.Chapter = 3;
        GameMgr.Instance.GameStart();
    }
    #endregion
}
