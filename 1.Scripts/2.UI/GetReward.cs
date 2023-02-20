using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eASSET_TYPE
{
    Gem,
    Gold,
    
}

public class GetReward : MonoBehaviour
{
    #region Fields
    [SerializeField] private AssetObjPool _pool;
    [SerializeField] private Transform _gridTransform;
    [SerializeField] private Button _quitBtn;
    [SerializeField] private GameObject _quitMsgObj;
    [SerializeField] private AssetEffect[] _effects;
    private Dictionary<eASSET_TYPE, int> _assetDic = new Dictionary<eASSET_TYPE, int>();

    private WaitForSeconds _delay = new WaitForSeconds(0.2f);
    
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        _pool.AllObjActiveOff();
        _quitBtn.interactable = false;
        _quitMsgObj.SetActive(false);
        for (int i = 0; i < _effects.Length; i++)
        {
            _effects[i].gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        GameMgr.Instance.UserExpCheck();
        UIMgr.Instance.Refresh();
    }
    #endregion

    #region Public Methods
    public void ShowAsset()
    {
        StartCoroutine(Cor_Show());
    }

    public void AddAsset(eASSET_TYPE type, int cnt)
    {
        if (_assetDic.ContainsKey(type))
        {
            _assetDic[type] += cnt;
        }
        else
        {
            _assetDic.Add(type, cnt);
        }
    }

    public void OnClickExit()
    {
        _quitMsgObj.SetActive(false);
        _quitBtn.interactable = false;
        _pool.AllObjActiveOff();
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        if (_assetDic.ContainsKey(eASSET_TYPE.Gem) && !_assetDic.ContainsKey(eASSET_TYPE.Gold))
        {
            for (int i = 0; i < _effects.Length; i++)
            {
                _effects[i].gameObject.SetActive(true);
                _effects[i].SetIcon(SpriteMgr.Instance.GetAssetIcon(eASSET_TYPE.Gem));
                _effects[i].SetTweenPosition(UIMgr.Instance.TopUI.GetGemPosition);
            }
            _assetDic.Clear();
            Invoke("ActiveOff", 1.3f);
        }
        else if (!_assetDic.ContainsKey(eASSET_TYPE.Gem) && _assetDic.ContainsKey(eASSET_TYPE.Gold))
        {
            for (int i = 0; i < _effects.Length; i++)
            {
                _effects[i].gameObject.SetActive(true);
                _effects[i].SetIcon(SpriteMgr.Instance.GetAssetIcon(eASSET_TYPE.Gold));
                _effects[i].SetTweenPosition(UIMgr.Instance.TopUI.GetGoldPosition);
            }
            _assetDic.Clear();
            Invoke("ActiveOff", 1.3f);
        }
        else if (_assetDic.ContainsKey(eASSET_TYPE.Gem) && _assetDic.ContainsKey(eASSET_TYPE.Gold))
        {
            int length = _effects.Length / 2;
            int index = 0;
            for (int i = 0; i < length; i++)
            {
                _effects[index].gameObject.SetActive(true);
                _effects[index].SetIcon(SpriteMgr.Instance.GetAssetIcon(eASSET_TYPE.Gem));
                _effects[index].SetTweenPosition(UIMgr.Instance.TopUI.GetGemPosition);
                index++;
            }

            length = _effects.Length - length;
            for (int i = 0; i < length; i++)
            {
                _effects[index].gameObject.SetActive(true);
                _effects[index].SetIcon(SpriteMgr.Instance.GetAssetIcon(eASSET_TYPE.Gold));
                _effects[index].SetTweenPosition(UIMgr.Instance.TopUI.GetGoldPosition);
                index++;
            }
            _assetDic.Clear();
            Invoke("ActiveOff", 1.3f);
        }
        else
        {

            _assetDic.Clear();
            ActiveOff();
        }
    }

    public void ActiveOff()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Coroutines
    private IEnumerator Cor_Show()
    {
        foreach (KeyValuePair<eASSET_TYPE, int> pair in _assetDic)
        {
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.Reward);
            }
            AssetObj obj = _pool.GetFromPool(_gridTransform);
            obj.SetAssetData(SpriteMgr.Instance.GetAssetIcon(pair.Key), pair.Value);
            yield return _delay;
        }
        _quitBtn.interactable = true;
        _quitMsgObj.SetActive(true);
    }
    #endregion
}
