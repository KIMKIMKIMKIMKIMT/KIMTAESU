using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    #region Fields
    [SerializeField] private UI_Tweener _playBtnTween;
    [SerializeField] private GameObject _emailLoginBox;
    [SerializeField] private InputField _inputEmail;
    [SerializeField] private Text _loginWait;
    #endregion

    #region Unity Methods
    private void Start()
    {
        SoundMgr.Instance.PlayBGMPlayer(eAUDIOCLIP_BGM.Title);
    
#if UNITY_EDITOR
        _emailLoginBox.SetActive(true);
        _loginWait.gameObject.SetActive(false);
#else
        _emailLoginBox.SetActive(false);
        ShowLoginWait();
#endif
    }

    #endregion

    #region Public Methods
    public void OnClickEmailLogin()
    {
        if (string.IsNullOrEmpty(_inputEmail.text))
        {
            return;
        }
        string email = _inputEmail.text.Trim();
        AccountMgr.Instance.EmailLogin(email);
        _loginWait.gameObject.SetActive(true);
        ShowLoginWait();
    }
    public void OnClickPlay()
    {
        SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.UI_Button);

        FadeMgr.Instance.FadeOn(null, () => { UIMgr.Instance.SetCanvasState(eCANVAS_STATE.Main); }, null);
    }
    public void ShowLoginWait()
    {
        StopAllCoroutines();
        StartCoroutine(Cor_LoginWait());
    }
    #endregion

    #region Coroutines
    private IEnumerator Cor_LoginWait()
    {
        WaitForSeconds delay = new WaitForSeconds(0.5f);

        string a = ".";
        int cnt = 0;
        while (!PlayerDataMgr.Instance.IsLogin)
        {
            _loginWait.text = "로그인 중" + a;

            a += ".";
            cnt++;
            if (cnt > 5)
            {
                cnt = 0;
                a = ".";
            }
            yield return delay;
        }
        if (string.IsNullOrEmpty(PlayerDataMgr.Instance.PlayerData.UserNickname))
        {
            PlayerDataMgr.Instance.PlayerData.UserNickname = "플레이어" + Random.Range(0, 99999);
        }
        _loginWait.gameObject.SetActive(false);
        _playBtnTween.StartTween();
    }
    #endregion
}
