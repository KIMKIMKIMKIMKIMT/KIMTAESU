using System;
using BackEnd;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_CustomLoginPopup : UI_Popup
{
    [SerializeField] private TMP_InputField IDInputField;
    [SerializeField] private TMP_InputField PWInputField;

    [SerializeField] private Button SignUpButton;
    [SerializeField] private Button LoginButton;
    [SerializeField] private Button CloseButton;

    public Action OnSuccessLoginCallback;

    private void Start()
    {
        IDInputField.text = PlayerPrefs.GetString("LastLoginId", string.Empty);
        PWInputField.text = PlayerPrefs.GetString("LastLoginPw", string.Empty);
        
        SignUpButton.BindEvent(() =>
        {
            string id = IDInputField.text;
            if (string.IsNullOrEmpty(id))
            {
                Managers.Message.ShowMessage("아이디를 입력하세요");
                return;
            }

            string pw = PWInputField.text;
            if (string.IsNullOrEmpty(pw))
            {
                Managers.Message.ShowMessage("비밀번호를 입력하세요");
                return;
            }

            var bro = Backend.BMember.CustomSignUp(id, pw);

            if (bro.IsSuccess())
            {
                Managers.Message.ShowMessage("회원가입 성공");
            }
            else
            {
                if (bro.GetStatusCode() == "409" && bro.GetErrorCode() == "DuplicatedParameterException" &&
                    bro.GetMessage().Contains("중복된 customId 입니다"))
                {
                    Managers.Message.ShowMessage("사용중인 ID 입니다");
                    return;
                }

                UnityEngine.Debug.LogError($"StatusCode : {bro.GetStatusCode()}\nErrorCode : {bro.GetErrorCode()}\nMessage : {bro.GetMessage()}");
            }
        });
        
        LoginButton.BindEvent(() =>
        {
            string id = IDInputField.text;
            if (string.IsNullOrEmpty(id))
            {
                Managers.Message.ShowMessage("아이디를 입력하세요");
                return;
            }

            string pw = PWInputField.text;
            if (string.IsNullOrEmpty(pw))
            {
                Managers.Message.ShowMessage("비밀번호를 입력하세요");
                return;
            }

            var bro = Backend.BMember.CustomLogin(id, pw, "GOOGLE");

            if (bro.IsSuccess())
            {
                PlayerPrefs.SetString("LastLoginId", id);
                PlayerPrefs.SetString("LastLoginPw", pw);
                Managers.Message.ShowMessage("로그인 성공");
                OnSuccessLoginCallback?.Invoke();
                ClosePopup();
            }
            else
            {
                if (bro.GetStatusCode() == "401" && bro.GetErrorCode() == "BadUnauthorizedException" &&
                    bro.GetMessage().Contains("bad customId"))
                {
                    Managers.Message.ShowMessage("존재하지 않는 ID 입니다");
                    return;
                }

                if (bro.GetStatusCode() == "401" && bro.GetErrorCode() == "BadUnauthorizedException" &&
                    bro.GetMessage().Contains("bad customPassword"))
                {
                    Managers.Message.ShowMessage("잘못된 비밀번호 입니다.");
                    return;
                }

                UnityEngine.Debug.LogError($"StatusCode : {bro.GetStatusCode()}\nErrorCode : {bro.GetErrorCode()}\nMessage : {bro.GetMessage()}");
            }
        });
        
        CloseButton.BindEvent(ClosePopup);
    }
}