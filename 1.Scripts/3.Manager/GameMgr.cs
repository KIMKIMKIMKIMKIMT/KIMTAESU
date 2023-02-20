using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : DontDestroy<GameMgr>
{
    #region Fields
    public int Chapter;
    public float EnterTime;
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        EnterTime = 0;
    }
    #endregion

    #region Public Methods
    public void GameStart()
    {
        LoadSceneMgr.Instance.LoadSceneAsync(eSCENESTATE.Game);
    }
    public void UserExpCheck()
    {
        Debug.Log(PlayerDataMgr.Instance.PlayerData.UserLevelUpExp);
        Debug.Log(PlayerDataMgr.Instance.PlayerData.UserExp);
        if (PlayerDataMgr.Instance.PlayerData.UserExp >= PlayerDataMgr.Instance.PlayerData.UserLevelUpExp)
        {
            UserLevelUp();
        }
        UIMgr.Instance.TopUI.SetExpFillAmount(PlayerDataMgr.Instance.PlayerData.UserExp / PlayerDataMgr.Instance.PlayerData.UserLevelUpExp);
    }
    public void UserLevelUp()
    {
        PlayerDataMgr.Instance.PlayerData.UserLevel++;
        PlayerDataMgr.Instance.PlayerData.UserExp -= PlayerDataMgr.Instance.PlayerData.UserLevelUpExp;
        PlayerDataMgr.Instance.PlayerData.UserLevelUpExp *= 1.1f;
        UIMgr.Instance.TopUI.InitUserExpFillAmount();
        PlayerDataMgr.Instance.SaveData();
        UIMgr.Instance._ui_Popup.ShowLevelUpPopup();
    }
    public void QuestAddCnt(eQUEST quest, int index = 1)
    {
        PlayerDataMgr.Instance.PlayerData.QuestState[quest] += index;
        if (PlayerDataMgr.Instance.PlayerData.QuestState[quest] > GameDataMgr.Instance.QuestData.QuestDic[quest].ClearCnt)
        {
            PlayerDataMgr.Instance.PlayerData.QuestState[quest] = GameDataMgr.Instance.QuestData.QuestDic[quest].ClearCnt;
        }
        PlayerDataMgr.Instance.SaveData();
    }
    #endregion
}
