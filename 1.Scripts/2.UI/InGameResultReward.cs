using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eRESULTREWARD_TYPE
{
    Exp,
    Gold,
    Weapon0,
    Weapon1,
    Weapon2,
    Equip0,
    Equip1,
    Equip2,
    WeaponScroll,
    EquipScroll,

    Max
}
public class InGameResultReward : MonoBehaviour
{
    #region Fields
    private AssetObjPool _pool;
    private Dictionary<eRESULTREWARD_TYPE, int> _assetDic;
    [SerializeField] private Transform _gridTransform;
    private WaitForSecondsRealtime _delay = new WaitForSecondsRealtime(0.2f);
    private eRESULTREWARD_TYPE _type;

    [SerializeField] private Text _titleTxt;
    [SerializeField] private Text _recordTimeTxt;
    [SerializeField] private Text _killCntTxt;
    [SerializeField] private Button _ExitBtn;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _assetDic = new Dictionary<eRESULTREWARD_TYPE, int>();
        _pool = GetComponent<AssetObjPool>();
    }

    private void OnEnable()
    {
        InGameUIMgr.Instance._joyStick.SetActive(false);
        JoyStick.Instance.StopDrag();
        Time.timeScale = 0;
    }
    #endregion

    #region Public Methods
    public void OnClickOk()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        LoadSceneMgr.Instance.LoadSceneAsync(eSCENESTATE.Main);
        Time.timeScale = 1;
    }
    public void AddAsset(eRESULTREWARD_TYPE type, int cnt)
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
    public void ClearDic()
    {
        _assetDic.Clear();
    }
    public void SetResult(bool clear)
    {
        if (clear)
        {
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.Victory);
            }
            _titleTxt.text = "성공";
            _recordTimeTxt.text = "15:00";
        }
        else
        {
            if(PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.Lose);
            }
            _titleTxt.text = "실패";
            int minit = (int)BattleMgr.Instance.PlayTime / 60;
            int sec = (int)BattleMgr.Instance.PlayTime % 60;
            _recordTimeTxt.text = string.Format("{0:00}:{1:00}", minit, sec);
        }
        _killCntTxt.text = BattleMgr.Instance.KillCnt.ToString();

        GetReward();
    }
    public void GetReward()
    {
        Debug.Log((int)BattleMgr.Instance.PlayTime);
        if ((int)BattleMgr.Instance.PlayTime < 300)
        {
            AddAsset(eRESULTREWARD_TYPE.Exp, (int)BattleMgr.Instance.PlayTime);
            AddAsset(eRESULTREWARD_TYPE.Gold, (int)BattleMgr.Instance.PlayTime + BattleMgr.Instance.StagaGold);
            Debug.Log("1");
        }
        else if ((int)BattleMgr.Instance.PlayTime < 600)
        {
            AddAsset(eRESULTREWARD_TYPE.Exp, (int)BattleMgr.Instance.PlayTime);
            AddAsset(eRESULTREWARD_TYPE.Gold, (int)BattleMgr.Instance.PlayTime + BattleMgr.Instance.StagaGold);
            AddAsset((eRESULTREWARD_TYPE)Random.Range(8, 10), Random.Range(0, 3));
            AddAsset((eRESULTREWARD_TYPE)Random.Range(8, 10), Random.Range(0, 3));
            Debug.Log("2");
        }
        else if ((int)BattleMgr.Instance.PlayTime <= 900 && !BattleMgr.Instance.IsClear)
        {
            AddAsset(eRESULTREWARD_TYPE.Exp, (int)BattleMgr.Instance.PlayTime);
            AddAsset(eRESULTREWARD_TYPE.Gold, (int)BattleMgr.Instance.PlayTime + BattleMgr.Instance.StagaGold);
            AddAsset((eRESULTREWARD_TYPE)Random.Range(8, 10), Random.Range(0, 5));
            AddAsset((eRESULTREWARD_TYPE)Random.Range(8, 10), Random.Range(0, 5));
            AddAsset((eRESULTREWARD_TYPE)Random.Range(2, 8), 1);
            AddAsset((eRESULTREWARD_TYPE)Random.Range(2, 8), 1);
            Debug.Log("3");
        }
        else
        {
            AddAsset(eRESULTREWARD_TYPE.Exp, (int)BattleMgr.Instance.PlayTime * 2);
            AddAsset(eRESULTREWARD_TYPE.Gold, (int)BattleMgr.Instance.PlayTime + BattleMgr.Instance.StagaGold * 2);
            AddAsset((eRESULTREWARD_TYPE)Random.Range(8, 10), Random.Range(0, 5));
            AddAsset((eRESULTREWARD_TYPE)Random.Range(8, 10), Random.Range(0, 5));
            AddAsset((eRESULTREWARD_TYPE)Random.Range(2, 8), 1);
            AddAsset((eRESULTREWARD_TYPE)Random.Range(2, 8), 1);
            AddAsset((eRESULTREWARD_TYPE)Random.Range(2, 8), 1);
            AddAsset((eRESULTREWARD_TYPE)Random.Range(2, 8), 1);
            AddAsset((eRESULTREWARD_TYPE)Random.Range(2, 8), 1);
            Debug.Log("4");
        }
        GetRewardData();
        ShowAsset();
    }
    public void GetRewardData()
    {
        for (int i = 0; i < (int)eRESULTREWARD_TYPE.Max; i++)
        {
            if (_assetDic.ContainsKey((eRESULTREWARD_TYPE)i))
            {
                _type = (eRESULTREWARD_TYPE)i;

                switch (_type)
                {
                    case eRESULTREWARD_TYPE.Exp:
                        PlayerDataMgr.Instance.PlayerData.UserExp += _assetDic[eRESULTREWARD_TYPE.Exp];
                        Debug.Log(_assetDic[eRESULTREWARD_TYPE.Exp]);
                        break;
                    case eRESULTREWARD_TYPE.Gold:
                        PlayerDataMgr.Instance.PlayerData.Gold += _assetDic[eRESULTREWARD_TYPE.Gold];
                        break;
                    case eRESULTREWARD_TYPE.Weapon0:
                        for (int j = 0; j < _assetDic[eRESULTREWARD_TYPE.Weapon0]; j++)
                        {
                            PlayerDataMgr.Instance.PlayerData.AddEquip(eATKWEAPON.Gun0_N, 1);
                        }
                        break;
                    case eRESULTREWARD_TYPE.Weapon1:
                        for (int j = 0; j < _assetDic[eRESULTREWARD_TYPE.Weapon1]; j++)
                        {
                            PlayerDataMgr.Instance.PlayerData.AddEquip(eATKWEAPON.Gun1_N, 1);
                        }
                        break;
                    case eRESULTREWARD_TYPE.Weapon2:
                        for (int j = 0; j < _assetDic[eRESULTREWARD_TYPE.Weapon2]; j++)
                        {
                            PlayerDataMgr.Instance.PlayerData.AddEquip(eATKWEAPON.Gun2_N, 1);
                        }
                        break;
                    case eRESULTREWARD_TYPE.Equip0:
                        for (int j = 0; j < _assetDic[eRESULTREWARD_TYPE.Equip0]; j++)
                        {
                            PlayerDataMgr.Instance.PlayerData.AddEquip(eHPEQUIP.Shield0_N, 1);
                        }
                        break;
                    case eRESULTREWARD_TYPE.Equip1:
                        for (int j = 0; j < _assetDic[eRESULTREWARD_TYPE.Equip1]; j++)
                        {
                            PlayerDataMgr.Instance.PlayerData.AddEquip(eHPEQUIP.Shield1_N, 1);
                        }
                        break;
                    case eRESULTREWARD_TYPE.Equip2:
                        for (int j = 0; j < _assetDic[eRESULTREWARD_TYPE.Equip2]; j++)
                        {
                            PlayerDataMgr.Instance.PlayerData.AddEquip(eHPEQUIP.Shield2_N, 1);
                        }
                        break;
                    case eRESULTREWARD_TYPE.WeaponScroll:
                        PlayerDataMgr.Instance.PlayerData.AddScroll(eSCROLL_TYPE.Weapon, _assetDic[eRESULTREWARD_TYPE.WeaponScroll]);
                        break;
                    case eRESULTREWARD_TYPE.EquipScroll:
                        PlayerDataMgr.Instance.PlayerData.AddScroll(eSCROLL_TYPE.Equip, _assetDic[eRESULTREWARD_TYPE.EquipScroll]);
                        break;
                }
            }
        }
        PlayerDataMgr.Instance.SaveData();
    }
    public void ShowAsset()
    {
        StopAllCoroutines();
        StartCoroutine(Cor_Show());
    }
    #endregion

    #region Coroutines
    private IEnumerator Cor_Show()
    {
        yield return _delay;
        foreach (KeyValuePair<eRESULTREWARD_TYPE, int> pair in _assetDic)
        {
            if(PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.Reward);
            }
            AssetObj obj = _pool.GetFromPool(_gridTransform);
            obj.SetAssetData(SpriteMgr.Instance.GetResultRewardIcon(pair.Key), pair.Value);
            yield return _delay;
        }

        ClearDic();
        _ExitBtn.gameObject.SetActive(true);
    }
    #endregion
}
