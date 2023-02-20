using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestPopup : MonoBehaviour
{
    #region Fields
    [SerializeField] private QuestBar _questBarPrefab;
    [SerializeField] private Transform _contentsGrid;
    private List<QuestBar> _questList;

    private QuestDataController _questData;
    private PlayerData _playerData;

    private bool _isInit;
    #endregion

    #region Untiy Methods
    private void Awake()
    {
        _questList = new List<QuestBar>();
        _questData = GameDataMgr.Instance.QuestData;
        _playerData = PlayerDataMgr.Instance.PlayerData;

        for (int i = 0; i < _questData.QuestDic.Count; i++)
        {
            if (!_playerData.QuestRewardCheck[(eQUEST)i])
            {
                QuestBar quest = Instantiate(_questBarPrefab, _contentsGrid);

                quest.SetData(_questData.QuestDic[(eQUEST)i]);
                quest.SetUI();
                _questList.Add(quest);
            }
        }
        for (int i = 0; i < _questData.QuestDic.Count; i++)
        {
            if (_playerData.QuestRewardCheck[(eQUEST)i])
            {
                QuestBar quest = Instantiate(_questBarPrefab, _contentsGrid);

                quest.SetData(_questData.QuestDic[(eQUEST)i]);
                quest.SetUI();
                _questList.Add(quest);
            }
        }
        _isInit = true;
    }

    private void OnEnable()
    {
        _playerData.EneterPlayTime = GameMgr.Instance.EnterTime;
        if (_playerData.EneterPlayTime >= 660)
        {
            GameMgr.Instance.QuestAddCnt(eQUEST.Play10);
        }
    }
    #endregion



    #region Public Methods
    public void ShowQuestState()
    {
        if (!_isInit)
        {
            return;
        }
        int index = 0;
        for (int i = 0; i < _questData.QuestDic.Count; i++)
        {
            if (!_playerData.QuestRewardCheck[(eQUEST)i])
            {
                _questList[index].SetData(_questData.QuestDic[(eQUEST)i]);
                _questList[index].SetUI();
                index++;
            }
        }
        for (int i = 0; i < _questData.QuestDic.Count; i++)
        {
            if (_playerData.QuestRewardCheck[(eQUEST)i])
            {
                _questList[index].SetData(_questData.QuestDic[(eQUEST)i]);
                _questList[index].SetUI();
                index++;
            }
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
