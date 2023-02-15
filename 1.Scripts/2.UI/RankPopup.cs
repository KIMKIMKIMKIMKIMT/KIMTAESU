using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

public class RankPopup : MonoBehaviour
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
        SetRankingList();
    }
    #endregion

    #region Public Methods
    public async void SetRankingList()
    {
        _pool.AllObjActiveOff();
        List<RankData> rankList = await PlayerDataMgr.Instance.GetRankingData();

        List<RankData> newlist = rankList.OrderByDescending(num => num.Time).ToList();

        for (int i = 0; i < newlist.Count; i++)
        {
            RankBar bar = _pool.GetFromPool(_pool.transform);
            bar.SetRankBar(i + 1, newlist[i].UserNickname, newlist[i].Time);
        }
    }
    
    public void OnClickQuit()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PopupMgr.Instance.RemovePopup();
    }
    #endregion
}
