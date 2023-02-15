using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    #region Fields
    [SerializeField] private WarUI _warUI;
    [SerializeField] private InvenUI _invenUI;
    [SerializeField] private UpgradeUI _upgradeUI;
    [SerializeField] private ShopUI _shopUI;

    public InvenUI InvenUI { get { return _invenUI; } }
    public UpgradeUI UpgradeUI { get { return _upgradeUI; } }
    public ShopUI ShopUI { get { return _shopUI; } }
    #endregion
    private void Start()
    {
        UIMgr.Instance.Refresh();
        if (PlayerDataMgr.Instance.PlayerData.BgmToggle)
        {
            SoundMgr.Instance.PlayBGMPlayer(eAUDIOCLIP_BGM.Main);
        }
        GameMgr.Instance.QuestAddCnt(eQUEST.EnterGame);
        
    }

    #region Public Methods
    public void Refresh()
    {
        _invenUI.Refresh();
        _upgradeUI.Refresh();
    }
    #endregion
}
