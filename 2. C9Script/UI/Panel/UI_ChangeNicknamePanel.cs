using System;
using System.Text.RegularExpressions;
using BackEnd;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class UI_ChangeNicknamePanel : UI_Panel
{
    [SerializeField] private TMP_InputField NicknameInput;
    [SerializeField] private TMP_Text PriceText;
    [SerializeField] private Image GoodsImage;

    [SerializeField] private Button ChangeButton;

    [SerializeField] private Button CloseButton;

    public Action OnChangeNickname;

    private void Start()
    {
        NicknameInput.onValueChanged.AddListener(text =>
        {
            text = Regex.Replace(text, @"[^0-9a-zA-Z가-힣ㄱ-ㅎㅏ-ㅣ]", "");
            NicknameInput.text = text;
        });
        
        NicknameInput.onDeselect.AddListener(text =>
        {
            text = Regex.Replace(text, @"[^0-9a-zA-Z가-힣ㄱ-ㅎㅏ-ㅣ]", "");
            NicknameInput.text = text;
        });

        if (ChartManager.GoodsCharts.TryGetValue((int)Goods.ChangeNicknameTicket, out var goodsChart))
        {
            GoodsImage.sprite = Managers.Resource.LoadGoodsIcon(goodsChart.Icon);
        }

        ChangeButton.BindEvent(OnClickChange);
        CloseButton.BindEvent(Close);
    }

    public override void Open()
    {
        base.Open();

        PriceText.text = $"{Managers.Game.GoodsDatas[(int)Goods.ChangeNicknameTicket]}/1";
        PriceText.color = !Utils.IsEnoughItem(ItemType.Goods, (int)Goods.ChangeNicknameTicket, 1) 
            ? Color.red
            : Color.white;
        NicknameInput.text = string.Empty;
    }

    private void OnClickChange()
    {
        if (!Utils.IsEnoughItem(ItemType.Goods, (int)Goods.ChangeNicknameTicket,
                1))
            return;

        string nickname = NicknameInput.text;

        if (string.IsNullOrEmpty(nickname))
        {
            Managers.Message.ShowMessage("닉네임을 입력하세요");
            return;
        }

        if (nickname.Length < 2 || nickname.Length > 8)
        {
            Managers.Message.ShowMessage("2자 이상 8자 이하로 입력해주세요");
            return;
        }

        NicknameInput.text = string.Empty;
        
        Backend.BMember.UpdateNickname(nickname, bro =>
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

                return;
            }
            
            Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.ChangeNicknameTicket,  1);
            GameDataManager.GoodsGameData.SaveGameData();
            Managers.Message.ShowMessage("변경 완료");
            Close();
            OnChangeNickname?.Invoke();
        });
    }
}