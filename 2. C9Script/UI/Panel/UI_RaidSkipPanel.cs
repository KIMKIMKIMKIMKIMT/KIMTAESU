using System;
using BackEnd;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_RaidSkipPanel : UI_Panel
{
    [SerializeField] private TMP_Text MaterialText;
    [SerializeField] private TMP_Text SkipButtonText;

    [SerializeField] private GameObject CloseObj;
    [SerializeField] private Button MinButton;
    [SerializeField] private Button MaxButton;
    [SerializeField] private Button MinusButton;
    [SerializeField] private Button PlusButton;
    [SerializeField] private Button SkipButton;

    [SerializeField] private UI_RaidSkipResultPanel UIRaidSkipResultPanel;

    private double _skipCount;

    private void Start()
    {
        CloseObj.BindEvent(Close);
        MinButton.BindEvent(OnClickMin);
        MaxButton.BindEvent(OnClickMax);
        MinusButton.BindEvent(OnClickMinus);
        MinusButton.BindEvent(OnClickMinus, UIEvent.Pressed);
        PlusButton.BindEvent(OnClickPlus);
        PlusButton.BindEvent(OnClickPlus, UIEvent.Pressed);
        SkipButton.BindEvent(OnClickSkip);
    }

    public override void Open()
    {
        base.Open();
        _skipCount = Managers.Game.GoodsDatas[(int)Goods.RaidTicket].Value > 0 ? 1 : 0;
        SetUI();
    }

    private void SetUI()
    {
        MaterialText.text = $"{_skipCount} / {Managers.Game.GoodsDatas[(int)Goods.RaidTicket].Value}";
        SkipButtonText.text = $"{_skipCount}회 스킵";
    }

    private void OnClickMin()
    {
        _skipCount = Math.Min(1, Managers.Game.GoodsDatas[(int)Goods.RaidTicket].Value);
        SetUI();
    }

    private void OnClickMax()
    {
        _skipCount = Managers.Game.GoodsDatas[(int)Goods.RaidTicket].Value;
        SetUI();
    }

    private void OnClickMinus()
    {
        _skipCount = _skipCount - 1 >= 0 ? _skipCount - 1 : 0;
        SetUI();
    }

    private void OnClickPlus()
    {
        _skipCount = Math.Min(_skipCount + 1, Managers.Game.GoodsDatas[(int)Goods.RaidTicket].Value);
        SetUI();
    }

    private void OnClickSkip()
    {
        if (_skipCount <= 0)
            return;

        if (!Utils.IsEnoughItem(ItemType.Goods, (int)Goods.RaidTicket, _skipCount))
        {
            Managers.Message.ShowMessage("티켓이 부족합니다");
            return;
        }

        var gainGold = Managers.Raid.RaidClearInfo.HighestGold * _skipCount;
        var gainGoldBar = Managers.Raid.RaidClearInfo.HighestGoldBar * _skipCount;
        var gainSkillStone = Managers.Raid.RaidClearInfo.HighestSkillStone * _skipCount;

        Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.RaidTicket, _skipCount);
        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.Gold, gainGold);
        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.GoldBar, gainGoldBar);
        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.SkillEnhancementStone, gainSkillStone);
        
        Param param = new();
        
        param.Add("SoloRaid", new RaidSkipLog()
        {
            SkipCount = (int)_skipCount,
            GainGoldValue = gainGold,
            GainGoldBarValue = gainGoldBar,
            GainSkillStoneValue = gainSkillStone
        });
        Utils.GetGoodsLog(ref param);
        
        Backend.GameLog.InsertLog("Dungeon", param);

        GameDataManager.GoodsGameData.SaveGameData();

        UIRaidSkipResultPanel.Open();
        UIRaidSkipResultPanel.Init(Managers.Raid.RaidClearInfo.ClearStep, gainGold, gainGoldBar, gainSkillStone,
            _skipCount);

        Close();
    }
}