using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    #region Fields
    public int RewardIndex { get; private set; }
    #endregion

    #region Public Methods
    public void OnClickOneNormalBox()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PopupMgr.Instance.ShowOkCancelPopup("장비상자 구매", "장비 상자 1개를 보석 50개로 구매하시겠습니까?", () =>
         {
             if (PlayerDataMgr.Instance.PlayerData.Gem >= 50)
             {
                 GameMgr.Instance.QuestAddCnt(eQUEST.GemUse50, 50);
                 PlayerDataMgr.Instance.PlayerData.Gem -= 50;
                 int ran = Random.Range(0, 50);

                 if (ran == 0)
                 {
                     int ran1 = Random.Range(0, 2);

                     if (ran1 == 0)
                     {
                         eATKWEAPON weapon = (eATKWEAPON)Random.Range(5, (int)eATKWEAPON.Gun0_U);
                         PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);
                         

                         UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.BronzeBox);
                         UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                     }
                     else
                     {
                         eHPEQUIP equip = (eHPEQUIP)Random.Range(5, (int)eHPEQUIP.Shield0_U);
                         PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);

                         UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.BronzeBox);
                         UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                     }
                     PlayerDataMgr.Instance.SaveData();
                     UIMgr.Instance.Refresh();

                 }
                 else
                 {
                     int ran2 = Random.Range(0, 2);
                     if (ran2 == 0)
                     {
                         eATKWEAPON weapon = (eATKWEAPON)Random.Range(0, (int)eATKWEAPON.Gun0_E);
                         PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);

                         UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                         UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.BronzeBox);
                     }
                     else
                     {
                         eHPEQUIP equip = (eHPEQUIP)Random.Range(0, (int)eHPEQUIP.Shield0_E);
                         PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);


                         UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                         UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.BronzeBox);
                     }
                     PlayerDataMgr.Instance.SaveData();
                     UIMgr.Instance.Refresh();

                 }
             }
             else
             {
                 PopupMgr.Instance.ShowOkPopup("알림", "보석이 부족합니다.");
             }
             
         });
        
    }
    public void OnClickTenNormalBox()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PopupMgr.Instance.ShowOkCancelPopup("장비상자 구매", "장비 상자 10개를 보석 450개로 구매하시겠습니까?", () =>
         {
             if (PlayerDataMgr.Instance.PlayerData.Gem >= 450)
             {
                 GameMgr.Instance.QuestAddCnt(eQUEST.GemUse50, 450);
                 PlayerDataMgr.Instance.PlayerData.Gem -= 450;

                 for (int i = 0; i < 10; i++)
                 {
                     int ran = Random.Range(0, 50);

                     if (ran == 0)
                     {
                         int ran1 = Random.Range(0, 2);

                         if (ran1 == 0)
                         {
                             eATKWEAPON weapon = (eATKWEAPON)Random.Range(5, (int)eATKWEAPON.Gun0_U);
                             PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);

                             UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                         }
                         else
                         {
                             eHPEQUIP equip = (eHPEQUIP)Random.Range(5, (int)eHPEQUIP.Shield0_U);
                             PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);

                             UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                         }
                         

                     }
                     else
                     {
                         int ran2 = Random.Range(0, 2);
                         if (ran2 == 0)
                         {
                             eATKWEAPON weapon = (eATKWEAPON)Random.Range(0, (int)eATKWEAPON.Gun0_E);
                             PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);

                             UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                         }
                         else
                         {
                             eHPEQUIP equip = (eHPEQUIP)Random.Range(0, (int)eHPEQUIP.Shield0_E);
                             PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);

                             Debug.Log(equip);
                             UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                         }
                         
                     }
                 }
                 UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.BronzeBox);
                 PlayerDataMgr.Instance.SaveData();
                 UIMgr.Instance.Refresh();
             }
             else
             {
                 PopupMgr.Instance.ShowOkPopup("알림", "보석이 부족합니다.");
             }
         });
    }
    public void OnClickOneSuperBox()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PopupMgr.Instance.ShowOkCancelPopup("고급 장비상자 구매", "고급 장비상자 1개를 보석 300개로 구매하시겠습니까?", () =>
        {
            if (PlayerDataMgr.Instance.PlayerData.Gem >= 300)
            {
                GameMgr.Instance.QuestAddCnt(eQUEST.GemUse50, 300);
                PlayerDataMgr.Instance.PlayerData.Gem -= 300;
                int ran = Random.Range(0, 50);

                if (ran == 0)
                {
                    int ran1 = Random.Range(0, 2);

                    if (ran1 == 0)
                    {
                        eATKWEAPON weapon = (eATKWEAPON)Random.Range(10, (int)eATKWEAPON.Max);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);


                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                    }
                    else
                    {
                        eHPEQUIP equip = (eHPEQUIP)Random.Range(10, (int)eHPEQUIP.Max);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);

                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                    }
                    PlayerDataMgr.Instance.SaveData();
                    UIMgr.Instance.Refresh();

                }
                else if(0 < ran && ran <= 10)
                {
                    int ran2 = Random.Range(0, 2);
                    if (ran2 == 0)
                    {
                        eATKWEAPON weapon = (eATKWEAPON)Random.Range(5, (int)eATKWEAPON.Gun0_U);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);

                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                    }
                    else
                    {
                        eHPEQUIP equip = (eHPEQUIP)Random.Range(5, (int)eHPEQUIP.Shield0_U);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);


                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                    }
                    PlayerDataMgr.Instance.SaveData();
                    UIMgr.Instance.Refresh();

                }
                else
                {
                    int ran3 = Random.Range(0, 2);
                    if (ran3 == 0)
                    {
                        eATKWEAPON weapon = (eATKWEAPON)Random.Range(0, (int)eATKWEAPON.Gun0_E);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);

                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                    }
                    else
                    {
                        eHPEQUIP equip = (eHPEQUIP)Random.Range(0, (int)eHPEQUIP.Shield0_E);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);


                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                    }
                    PlayerDataMgr.Instance.SaveData();
                    UIMgr.Instance.Refresh();
                }
            }
            else
            {
                PopupMgr.Instance.ShowOkPopup("알림", "보석이 부족합니다.");
            }

        });

    }
    public void OnClickTenSuperBox()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        PopupMgr.Instance.ShowOkCancelPopup("고급 장비상자 구매", "고급 장비상자 10개를 보석 2700개로 구매하시겠습니까?", () =>
        {
            GameMgr.Instance.QuestAddCnt(eQUEST.GemUse50, 2700);
            if (PlayerDataMgr.Instance.PlayerData.Gem >= 2700)
            {
                PlayerDataMgr.Instance.PlayerData.Gem -= 2700;

                for (int i = 0; i < 10; i++)
                {
                    int ran = Random.Range(0, 50);

                    if (ran == 0)
                    {
                        int ran1 = Random.Range(0, 2);

                        if (ran1 == 0)
                        {
                            eATKWEAPON weapon = (eATKWEAPON)Random.Range(10, (int)eATKWEAPON.Max);
                            PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);

                            UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                        }
                        else
                        {
                            eHPEQUIP equip = (eHPEQUIP)Random.Range(10, (int)eHPEQUIP.Max);
                            PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);

                            UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                        }


                    }
                    else if(0 < ran && ran <= 10)
                    {
                        int ran2 = Random.Range(0, 2);
                        if (ran2 == 0)
                        {
                            eATKWEAPON weapon = (eATKWEAPON)Random.Range(5, (int)eATKWEAPON.Gun0_U);
                            PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);

                            UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                        }
                        else
                        {
                            eHPEQUIP equip = (eHPEQUIP)Random.Range(5, (int)eHPEQUIP.Shield0_U);
                            PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);

                            Debug.Log(equip);
                            UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                        }

                    }
                    else
                    {
                        int ran3 = Random.Range(0, 2);
                        if (ran3 == 0)
                        {
                            eATKWEAPON weapon = (eATKWEAPON)Random.Range(0, (int)eATKWEAPON.Gun0_E);
                            PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);

                            UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                        }
                        else
                        {
                            eHPEQUIP equip = (eHPEQUIP)Random.Range(0, (int)eHPEQUIP.Shield0_E);
                            PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);

                            Debug.Log(equip);
                            UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                        }
                    }
                }
                UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                PlayerDataMgr.Instance.SaveData();
                UIMgr.Instance.Refresh();
            }
            else
            {
                PopupMgr.Instance.ShowOkPopup("알림", "보석이 부족합니다.");
            }
        });
    }
    public void OnClickGem(int index)
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        IAPMgr.Instance.Purchase(index);
    }
    public void OnClickGold(int index)
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);
        }
        switch (index)
        {
            case 0:
                PopupMgr.Instance.ShowOkCancelPopup("골드 구매", "10,000골드를 보석 90개에 구매 하시겠습니까?", () =>
                  {
                      if (PlayerDataMgr.Instance.PlayerData.Gem >= 90)
                      {
                          GameMgr.Instance.QuestAddCnt(eQUEST.GemUse50, 90);
                          PlayerDataMgr.Instance.PlayerData.Gem -= 90;
                          PlayerDataMgr.Instance.PlayerData.Gold += 10000;
                          PlayerDataMgr.Instance.SaveData();
                          UIMgr.Instance.Refresh();
                          PopupMgr.Instance.ShowOkPopup("구매 완료", "골드 구매가 완료되었습니다.");
                      }
                      else
                      {
                          PopupMgr.Instance.ShowOkPopup("구매 실패", "보석이 부족합니다.");
                      }
                  });
                break;
            case 1:
                PopupMgr.Instance.ShowOkCancelPopup("골드 구매", "30,000골드를 보석 250개에 구매 하시겠습니까?", () =>
                {
                    if (PlayerDataMgr.Instance.PlayerData.Gem >= 250)
                    {
                        GameMgr.Instance.QuestAddCnt(eQUEST.GemUse50, 250);
                        PlayerDataMgr.Instance.PlayerData.Gem -= 250;
                        PlayerDataMgr.Instance.PlayerData.Gold += 30000;
                        PlayerDataMgr.Instance.SaveData();
                        UIMgr.Instance.Refresh();
                        PopupMgr.Instance.ShowOkPopup("구매 완료", "골드 구매가 완료되었습니다.");
                    }
                    else
                    {
                        PopupMgr.Instance.ShowOkPopup("구매 실패", "보석이 부족합니다.");
                    }
                });
                break;
            case 2:
                PopupMgr.Instance.ShowOkCancelPopup("골드 구매", "100,000골드를 보석 750개에 구매 하시겠습니까?", () =>
                {
                    if (PlayerDataMgr.Instance.PlayerData.Gem >= 750)
                    {
                        GameMgr.Instance.QuestAddCnt(eQUEST.GemUse50, 750);
                        PlayerDataMgr.Instance.PlayerData.Gem -= 750;
                        PlayerDataMgr.Instance.PlayerData.Gold += 100000;
                        PlayerDataMgr.Instance.SaveData();
                        UIMgr.Instance.Refresh();
                        PopupMgr.Instance.ShowOkPopup("구매 완료", "골드 구매가 완료되었습니다.");
                    }
                    else
                    {
                        PopupMgr.Instance.ShowOkPopup("구매 실패", "보석이 부족합니다.");
                    }
                });
                break;
            case 3:
                PopupMgr.Instance.ShowOkCancelPopup("골드 구매", "500,000골드를 보석 3,500개에 구매 하시겠습니까?", () =>
                {
                    if (PlayerDataMgr.Instance.PlayerData.Gem >= 3500)
                    {
                        GameMgr.Instance.QuestAddCnt(eQUEST.GemUse50, 3500);
                        PlayerDataMgr.Instance.PlayerData.Gem -= 3500;
                        PlayerDataMgr.Instance.PlayerData.Gold += 500000;
                        PlayerDataMgr.Instance.SaveData();
                        UIMgr.Instance.Refresh();
                        PopupMgr.Instance.ShowOkPopup("구매 완료", "골드 구매가 완료되었습니다.");
                    }
                    else
                    {
                        PopupMgr.Instance.ShowOkPopup("구매 실패", "보석이 부족합니다.");
                    }
                });
                break;
        }
    }

    public void OnClickRewardAd(int index)
    {
        switch (index)
        {
            case 0:
                if (!AdmobMgr.Instance.RewardAd.ShowRewardAd())
                {
                    PopupMgr.Instance.ShowOkPopup("알림", "광고가 준비 되지 않았습니다.");
                }
                RewardIndex = 0;
                break;
            case 1:
                if (!AdmobMgr.Instance.RewardAd.ShowRewardAd())
                {
                    PopupMgr.Instance.ShowOkPopup("알림", "광고가 준비 되지 않았습니다.");
                }
                RewardIndex = 1;
                break;
        }
    }
    public void Reward()
    {
        switch (RewardIndex)
        {
            case 0:
                int ran = Random.Range(0, 50);

                if (ran == 0)
                {
                    int ran1 = Random.Range(0, 2);

                    if (ran1 == 0)
                    {
                        eATKWEAPON weapon = (eATKWEAPON)Random.Range(5, (int)eATKWEAPON.Gun0_U);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);


                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.BronzeBox);
                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                    }
                    else
                    {
                        eHPEQUIP equip = (eHPEQUIP)Random.Range(5, (int)eHPEQUIP.Shield0_U);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);

                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.BronzeBox);
                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                    }
                    PlayerDataMgr.Instance.SaveData();
                    UIMgr.Instance.Refresh();

                }
                else
                {
                    int ran2 = Random.Range(0, 2);
                    if (ran2 == 0)
                    {
                        eATKWEAPON weapon = (eATKWEAPON)Random.Range(0, (int)eATKWEAPON.Gun0_E);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);

                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.BronzeBox);
                    }
                    else
                    {
                        eHPEQUIP equip = (eHPEQUIP)Random.Range(0, (int)eHPEQUIP.Shield0_E);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);


                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.BronzeBox);
                    }
                    PlayerDataMgr.Instance.SaveData();
                    UIMgr.Instance.Refresh();

                }
                break;
            case 1:
                int rand = Random.Range(0, 50);

                if (rand == 0)
                {
                    int ran1 = Random.Range(0, 2);

                    if (ran1 == 0)
                    {
                        eATKWEAPON weapon = (eATKWEAPON)Random.Range(10, (int)eATKWEAPON.Max);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);


                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                    }
                    else
                    {
                        eHPEQUIP equip = (eHPEQUIP)Random.Range(10, (int)eHPEQUIP.Max);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);

                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                    }
                    PlayerDataMgr.Instance.SaveData();
                    UIMgr.Instance.Refresh();

                }
                else if (0 < rand && rand <= 10)
                {
                    int ran2 = Random.Range(0, 2);
                    if (ran2 == 0)
                    {
                        eATKWEAPON weapon = (eATKWEAPON)Random.Range(5, (int)eATKWEAPON.Gun0_U);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);

                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                    }
                    else
                    {
                        eHPEQUIP equip = (eHPEQUIP)Random.Range(5, (int)eHPEQUIP.Shield0_U);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);


                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                    }
                    PlayerDataMgr.Instance.SaveData();
                    UIMgr.Instance.Refresh();

                }
                else
                {
                    int ran3 = Random.Range(0, 2);
                    if (ran3 == 0)
                    {
                        eATKWEAPON weapon = (eATKWEAPON)Random.Range(0, (int)eATKWEAPON.Gun0_E);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(weapon, 1);

                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(weapon);
                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                    }
                    else
                    {
                        eHPEQUIP equip = (eHPEQUIP)Random.Range(0, (int)eHPEQUIP.Shield0_E);
                        PlayerDataMgr.Instance.PlayerData.AddEquip(equip, 1);


                        UIMgr.Instance._ui_Popup._boxOpenPopup.AddList(equip);
                        UIMgr.Instance._ui_Popup.ShowBoxOpenPopup(eBOX_TYPE.GoldBox);
                    }
                    PlayerDataMgr.Instance.SaveData();
                    UIMgr.Instance.Refresh();
                }
                break;
        }
    }
    #endregion
}
