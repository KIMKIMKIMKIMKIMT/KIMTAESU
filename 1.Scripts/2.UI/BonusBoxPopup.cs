using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusBoxPopup : MonoBehaviour
{
    #region Fields
    [SerializeField] private BonusSkill[] _skills;
    [SerializeField] private GameObject[] _light;


    [SerializeField] private Image _upgradeSkill;
    [SerializeField] private Button _startBtn;
    [SerializeField] private Button _quitBtn;
    [SerializeField] private Text _stopTxt;

    private float _time;
    private float _realTime;
    private bool _isStart;
    private bool _isSkip;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        InGameUIMgr.Instance._joyStick.SetActive(false);
        JoyStick.Instance.StopDrag();
        _upgradeSkill.gameObject.SetActive(false);
        _quitBtn.gameObject.SetActive(false);
        _stopTxt.gameObject.SetActive(false);
        _startBtn.gameObject.SetActive(true);
        _time = 0;
        _realTime = 0;
        _isSkip = false;
        Time.timeScale = 0;
        
        for (int i = 0; i < _light.Length; i++)
        {
            _light[i].SetActive(false);
        }

        for (int i = 0; i < _skills.Length; i++)
        {
            int ran = Random.Range(0, 2);
            if (ran == 0)
            {
                int ranSkill = Random.Range(0, BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList.Count);
                _skills[i].SetSkill(BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList[ranSkill]);
                if (BattleMgr.Instance.Player.SkillController.CurrentAttackSkillTable[_skills[i]._attackSkill].Level == 5)
                {
                    i--;
                }
            }
            else
            {
                if (BattleMgr.Instance.Player.SkillController.CurrentBuffSkillList.Count == 0)
                {
                    i--;
                }
                else
                {
                    int ranSkill = Random.Range(0, BattleMgr.Instance.Player.SkillController.CurrentBuffSkillList.Count);
                    _skills[i].SetSkill(BattleMgr.Instance.Player.SkillController.CurrentBuffSkillList[ranSkill]);
                    if (BattleMgr.Instance.Player.SkillController.CurrentBuffSkillTable[_skills[i]._buffSkill].Level == 5)
                    {
                        i--;
                    }
                }
            }
        }
    }

    private void Update()
    {
        _realTime += Time.unscaledDeltaTime;
        if (_isStart)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isSkip = true;
            }
            _time += Time.unscaledDeltaTime / 10;
        }
    }
    private void OnDisable()
    {
        InGameUIMgr.Instance._joyStick.SetActive(true);
        Time.timeScale = 1;
    }
    #endregion

    #region Public Methods
    public void OnClickStart()
    {
        _isStart = true;
        _startBtn.gameObject.SetActive(false);
        _stopTxt.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(Cor_Start());
    }
    public void UpgradeSkill(int index)
    {
        _upgradeSkill.gameObject.SetActive(true);
        if (_skills[index]._buffSkill == eBUFFSKILL_LIST.None)
        {
            BattleMgr.Instance.Player.SkillController.GetSKill(_skills[index]._attackSkill);
            _upgradeSkill.sprite = SpriteMgr.Instance._atkSkillkImg[(int)_skills[index]._attackSkill];
        }
        else
        {
            BattleMgr.Instance.Player.SkillController.GetSkill(_skills[index]._buffSkill);
            _upgradeSkill.sprite = SpriteMgr.Instance._buffSkillkImg[(int)_skills[index]._buffSkill];
        }
        
        QuitBtnActiveOn();
    }
    public void QuitBtnActiveOn()
    {
        _stopTxt.gameObject.SetActive(false);
        _quitBtn.gameObject.SetActive(true);
    }
    public void OnClickQuit()
    {
        PopupMgr.Instance.RemovePopup();
    }
    #endregion

    #region Coroutines
    private IEnumerator Cor_Start()
    {
        int index = Random.Range(0, 16);
        while (true)
        {
            if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
            {
                SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.BonusBox);
            }
            _light[index].SetActive(true);
            _light[index == 0 ? 15 : index - 1].SetActive(false);

            if (_isSkip && _realTime > 2)
            {
                _isStart = false;
                _light[index].SetActive(true);
                _light[index == 0 ? 15 : index - 1].SetActive(false);
                UpgradeSkill(index);
                yield break;
            }
            if (_realTime > 5)
            {
                _isStart = false;
                _light[index].SetActive(true);
                _light[index == 0 ? 15 : index - 1].SetActive(false);
                UpgradeSkill(index);
                yield break;
            }

            index++;
            if (index == 16)
            {
                index = 0;
            }
            

            yield return new WaitForSecondsRealtime(_time / 10);
        }
    }
    #endregion
}
