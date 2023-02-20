using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBar : MonoBehaviour
{
    #region Fields
    [SerializeField] private Text _header;
    [SerializeField] private Image _gaugeFillAmount;
    [SerializeField] private Image _icon;
    [SerializeField] private Text _cntTxt;
    [SerializeField] private Text _rewardTxt;
    [SerializeField] private GameObject _lockObj;
    [SerializeField] private GameObject _clearObj;
    private DailyQuestPopup _ctrl;
    private QuestData _data;
    private PlayerData _playerData;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _playerData = PlayerDataMgr.Instance.PlayerData;
        _ctrl = GetComponentInParent<DailyQuestPopup>();
    }
    #endregion

    #region Public Methods
    public void SetData(QuestData data)
    {
        _data = data;
    }

    public void SetUI()
    {
        if (_data == null)
        {
            return;
        }

        _header.text = _data.Header;
        _cntTxt.text = string.Format("{0} / {1}", _playerData.QuestState[_data.Key], _data.ClearCnt);
        _gaugeFillAmount.fillAmount = (float)_playerData.QuestState[_data.Key] / _data.ClearCnt;
        _rewardTxt.text = _data.RewardCnt.ToString("N0");


        _icon.sprite = SpriteMgr.Instance.GetAssetIcon(_data.RewardType);

        _lockObj.SetActive(_playerData.QuestState[_data.Key] < _data.ClearCnt);
        _clearObj.SetActive(_playerData.QuestRewardCheck[_data.Key]);
    }

    public void OnClickGetReward()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        if (_playerData.QuestState[_data.Key] >= _data.ClearCnt && !_playerData.QuestRewardCheck[_data.Key])
        {
            _playerData.QuestRewardCheck[_data.Key] = true;
            UIMgr.Instance.Reward.gameObject.SetActive(true);

            UIMgr.Instance.Reward.AddAsset(_data.RewardType, _data.RewardCnt);
            UIMgr.Instance.Reward.ShowAsset();

            _ctrl.ShowQuestState();
        }
    }
    #endregion
}
