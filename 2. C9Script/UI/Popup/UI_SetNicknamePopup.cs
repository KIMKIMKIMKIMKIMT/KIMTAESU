using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using AppsFlyerSDK;
using BackEnd;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_SetNicknamePopup : UI_Popup
{
    [SerializeField] private TMP_InputField NicknameInput;
    [SerializeField] private Button SetNicknameButton;

    public Action OnSuccessCallback;

    private void Start()
    {
        SetNicknameButton.BindEvent(OnClickSetNickname);

        NicknameInput.onDeselect.AddListener(text =>
        {
            text = Regex.Replace(text, @"[^0-9a-zA-Z가-힣]", "");
            NicknameInput.text = text;
        });
    }

    private void OnClickSetNickname()
    {
        SetNicknameButton.interactable = false;

        string nickname = Regex.Replace(NicknameInput.text, @"[^0-9a-zA-Z가-힣]", "");
        if (string.IsNullOrEmpty(nickname))
        {
            Managers.Message.ShowMessage("닉네임을 입력해주세요");
            SetNicknameButton.interactable = true;
            return;
        }

        if (nickname.Length < 2 || nickname.Length > 8)
        {
            Managers.Message.ShowMessage("2자 이상 8자 이하로 입력해주세요");
            SetNicknameButton.interactable = true;
            return;
        }

        string checkNickname = nickname.ToLower();

        if (checkNickname.Contains("gm") || checkNickname.Contains("dwgame") || checkNickname.Contains("운영자"))
        {
            Managers.Message.ShowMessage("금칙어가 포함되어 있습니다.");
            return;
        }

        Backend.BMember.CreateNickname(nickname, bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog(bro);

                if (bro.GetStatusCode().Equals("409") &&
                    bro.GetErrorCode().Equals("DuplicatedParameterException") &&
                    bro.GetMessage().Contains("중복된 nickname 입니다"))
                {
                    Managers.Message.ShowMessage("이미 사용중인 닉네임 입니다");
                }

                SetNicknameButton.interactable = true;
                return;
            }
            
            InAppActivity.SendEvent("nickname");
            Managers.UI.ClosePopupUI(this);
            OnSuccessCallback?.Invoke();
        });
    }
}