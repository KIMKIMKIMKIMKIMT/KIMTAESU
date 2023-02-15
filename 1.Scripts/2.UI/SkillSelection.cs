using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelection : MonoBehaviour
{
    #region Fields
    [SerializeField] private UT_UI_Tweener _newSelectTextTween;
    [SerializeField] private UT_UI_Tweener _skillSelectionEnableTween;
    [SerializeField] private Image[] _currentAttackSkillsImg;
    [SerializeField] private Image[] _currentBuffSkillsImg;
    [SerializeField] private Text _levelTxt;
    

    private SelectSkill[] _skillSelects;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _skillSelects = GetComponentsInChildren<SelectSkill>();
    }

    private void OnEnable()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.SkillSelect);
        }
        InGameUIMgr.Instance._joyStick.SetActive(false);
        JoyStick.Instance.StopDrag();
        _levelTxt.text = BattleMgr.Instance.Level.ToString();
        Time.timeScale = 0;
        _skillSelectionEnableTween.StartTween();
        _newSelectTextTween.StartTween();
        SetSKillPriority();
        SetCurrentAttackSkillImage();
    }

    private void OnDisable()
    {
        InGameUIMgr.Instance._joyStick.SetActive(true);
        transform.localScale = Vector3.zero;
        Time.timeScale = 1;
    }
    #endregion

    #region Public Methods
    public void SetCurrentAttackSkillImage()
    {
        for (int i = 0; i < BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList.Count; i++)
        {
            _currentAttackSkillsImg[i].sprite = SpriteMgr.Instance._atkSkillkImg[(int)BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList[i]];
        }
        for (int i = 0; i < BattleMgr.Instance.Player.SkillController.CurrentBuffSkillList.Count; i++)
        {
            _currentBuffSkillsImg[i].sprite = SpriteMgr.Instance._buffSkillkImg[(int)BattleMgr.Instance.Player.SkillController.CurrentBuffSkillList[i]];
        }
    }
    public void SetSKillPriority()
    {
        for (int i = 0; i < _skillSelects.Length; i++)
        {
            _skillSelects[i].gameObject.SetActive(false);
        }
        List<eATTACKSKILL_LIST> tempAtk = new List<eATTACKSKILL_LIST>();
        List<eBUFFSKILL_LIST> tempBuff = new List<eBUFFSKILL_LIST>();

        for (int i = 0; i < BattleMgr.Instance.Player.SkillController.RemainAttackSkillList.Count; i++)
        {
            tempAtk.Add(BattleMgr.Instance.Player.SkillController.RemainAttackSkillList[i]);
        }
        for (int i = 0; i < BattleMgr.Instance.Player.SkillController.RemainBuffSkillList.Count; i++)
        {
            tempBuff.Add(BattleMgr.Instance.Player.SkillController.RemainBuffSkillList[i]);
        }

        if (tempAtk.Count >= 3)
        {
            eATTACKSKILL_LIST key = tempAtk[Random.Range(0, tempAtk.Count)];
            tempAtk.Remove(key);
            _skillSelects[0].SetSkillData(GameDataMgr.Instance.AttackSKillDataController.AttackSkillTable[key]);
            _skillSelects[0].gameObject.SetActive(true);

            for (int i = 1; i < _skillSelects.Length; i++)
            {
                eSKILL_TYPE randType = (eSKILL_TYPE)Random.Range(0, (int)eSKILL_TYPE.Max);
                if (randType == eSKILL_TYPE.Attack)
                {
                    eATTACKSKILL_LIST atk = tempAtk[Random.Range(0, tempAtk.Count)];
                    tempAtk.Remove(atk);
                    _skillSelects[i].SetSkillData(GameDataMgr.Instance.AttackSKillDataController.AttackSkillTable[atk]);
                    _skillSelects[i].gameObject.SetActive(true);
                }
                else
                {
                    if (tempBuff.Count >= 1)
                    {
                        eBUFFSKILL_LIST buff = tempBuff[Random.Range(0, tempBuff.Count)];
                        tempBuff.Remove(buff);
                        _skillSelects[i].SetSkillData(GameDataMgr.Instance.BuffSkillDataController.BuffSkillTable[buff]);
                        _skillSelects[i].gameObject.SetActive(true);
                    }
                }
            }
        }
        else if (tempAtk.Count == 2)
        {
            eATTACKSKILL_LIST key = tempAtk[Random.Range(0, tempAtk.Count)];
            tempAtk.Remove(key);
            _skillSelects[0].SetSkillData(GameDataMgr.Instance.AttackSKillDataController.AttackSkillTable[key]);
            _skillSelects[0].gameObject.SetActive(true);

            for (int i = 1; i < _skillSelects.Length; i++)
            {
                eSKILL_TYPE randType = (eSKILL_TYPE)Random.Range(0, (int)eSKILL_TYPE.Max);
                if (randType == eSKILL_TYPE.Attack)
                {
                    eATTACKSKILL_LIST atk = tempAtk[Random.Range(0, tempAtk.Count)];
                    tempAtk.Remove(atk);
                    _skillSelects[i].SetSkillData(GameDataMgr.Instance.AttackSKillDataController.AttackSkillTable[atk]);
                    _skillSelects[i].gameObject.SetActive(true);
                    if (i == 1)
                    {
                        eBUFFSKILL_LIST buff = tempBuff[Random.Range(0, tempBuff.Count)];
                        tempBuff.Remove(buff);
                        _skillSelects[2].SetSkillData(GameDataMgr.Instance.BuffSkillDataController.BuffSkillTable[buff]);
                        _skillSelects[2].gameObject.SetActive(true);
                        break;
                    }
                }
                else
                {
                    if (tempBuff.Count >= 1)
                    {
                        eBUFFSKILL_LIST buff = tempBuff[Random.Range(0, tempBuff.Count)];
                        tempBuff.Remove(buff);
                        _skillSelects[i].SetSkillData(GameDataMgr.Instance.BuffSkillDataController.BuffSkillTable[buff]);
                        _skillSelects[i].gameObject.SetActive(true);
                    }
                }
            }
        }
        else if(tempAtk.Count == 1)
        {
            eATTACKSKILL_LIST key = tempAtk[Random.Range(0, tempAtk.Count)];
            tempAtk.Remove(key);
            _skillSelects[0].SetSkillData(GameDataMgr.Instance.AttackSKillDataController.AttackSkillTable[key]);
            _skillSelects[0].gameObject.SetActive(true);

            if (tempBuff.Count >= 2)
            {
                eBUFFSKILL_LIST buff1 = tempBuff[Random.Range(0, tempBuff.Count)];
                tempBuff.Remove(buff1);
                _skillSelects[1].SetSkillData(GameDataMgr.Instance.BuffSkillDataController.BuffSkillTable[buff1]);
                _skillSelects[1].gameObject.SetActive(true);

                eBUFFSKILL_LIST buff2 = tempBuff[Random.Range(0, tempBuff.Count)];
                tempBuff.Remove(buff2);
                _skillSelects[2].SetSkillData(GameDataMgr.Instance.BuffSkillDataController.BuffSkillTable[buff2]);
                _skillSelects[2].gameObject.SetActive(true);
            }
        }
        else if (tempBuff.Count > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (tempBuff.Count > i)
                {
                    eBUFFSKILL_LIST buff2 = tempBuff[Random.Range(0, tempBuff.Count)];
                    tempBuff.Remove(buff2);
                    _skillSelects[i].SetSkillData(GameDataMgr.Instance.BuffSkillDataController.BuffSkillTable[buff2]);
                    _skillSelects[i].gameObject.SetActive(true);
                }
            }
        }
    }
    #endregion
}