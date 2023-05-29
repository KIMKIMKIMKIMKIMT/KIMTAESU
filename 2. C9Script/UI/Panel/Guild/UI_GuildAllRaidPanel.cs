using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class UI_GuildAllRaidPanel : UI_Panel
{
    #region Fields
    [SerializeField] private TMP_Text _clearStepText;
    [SerializeField] private TMP_Text _clearTimeText;
    [SerializeField] private TMP_Text _highestGainGoldText;
    [SerializeField] private TMP_Text _highestGainGoldBarText;
    [SerializeField] private TMP_Text _highestGainGuildPointText;
    [SerializeField] private TMP_Text _monsterNameText;
    [SerializeField] private TMP_Text _stepText;
    [SerializeField] private TMP_Text _entryCountText;

    [SerializeField] private Image _monsterImage;
    [SerializeField] private Image _goodsImg;

    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _startBtn;
    [SerializeField] private Button _closeBtn;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _prevButton.BindEvent(OnClickPrev);
        _nextButton.BindEvent(OnClickNext);
        _startBtn.BindEvent(OnClickAllRaidStart);
        _closeBtn.BindEvent(Close);

        _goodsImg.sprite = Managers.Resource.LoadGoodsIcon(ChartManager.GoodsCharts[(int)Goods.GuildAllRaidTicket].Icon);

        Managers.AllRaid.Step.Subscribe(SetAllRaidStepUI);
    }
    #endregion

    #region Public Methods
    public override void Open()
    {
        base.Open();

        SetUI();
    }
    #endregion

    #region Private Methods
    private void SetUI()
    {
        // 클리어 정보가 있을때.
        if (Managers.AllRaid.GuildAllRaidClearInfo != null && Managers.AllRaid.GuildAllRaidClearInfo.ClearStep >= 1)
        {
            var guildAllRaidClearInfo = Managers.AllRaid.GuildAllRaidClearInfo;

            _clearStepText.text = $"{guildAllRaidClearInfo.ClearStep}단계";
            _clearTimeText.text = $"{(int)(guildAllRaidClearInfo.ClearTime / 60):00}:{(guildAllRaidClearInfo.ClearTime % 60):00}";
            _highestGainGoldText.text = guildAllRaidClearInfo.HighestGold.ToCurrencyString();
            _highestGainGoldBarText.text = guildAllRaidClearInfo.HighestGoldBar.ToCurrencyString();
            _highestGainGuildPointText.text = guildAllRaidClearInfo.HighestGuildPoint.ToCurrencyString();

            // 단계 최고단계로 설정
            Managers.AllRaid.Step.Value = ChartManager.AllGuildRaidDungeonCharts
                .ContainsKey(guildAllRaidClearInfo.ClearStep + 1) ?
                guildAllRaidClearInfo.ClearStep + 1 : guildAllRaidClearInfo.ClearStep;
        }
        // 클리어 정보가 없을때.
        else
        {
            _clearStepText.text = "-단계";
            _clearTimeText.text = "-:-";
            _highestGainGoldText.text = "-";
            _highestGainGoldBarText.text = "-";
            _highestGainGuildPointText.text = "-";

            Managers.AllRaid.Step.Value = 1;
        }

        _entryCountText.text = $"{Managers.Game.GoodsDatas[(int)Goods.GuildAllRaidTicket].Value}/1";
    }

    private void SetAllRaidStepUI(int step)
    {
        if (!ChartManager.AllGuildRaidDungeonCharts.TryGetValue(step, out var guildAllRaidDungeonChart))
            return;

        if (!ChartManager.MonsterCharts.TryGetValue(guildAllRaidDungeonChart.MonsterId, out var monsterChart))
            return;

        _stepText.text = $"{step}단계";
        _monsterImage.sprite = Managers.Resource.LoadMonsterIcon(guildAllRaidDungeonChart.MonsterId);
        _monsterNameText.text = $"<{ChartManager.GetString(monsterChart.Name)}>";

        _prevButton.gameObject.SetActive(ChartManager.AllGuildRaidDungeonCharts.ContainsKey(step - 1));
        _nextButton.gameObject.SetActive(Managers.AllRaid.GuildAllRaidClearInfo != null &&
                                        Managers.AllRaid.GuildAllRaidClearInfo.ClearStep + 1 > step &&
                                        ChartManager.AllGuildRaidDungeonCharts.ContainsKey(step + 1));
    }

    private void OnClickAllRaidStart()
    {
        var afterGuildJoinTime = Utils.GetNow() - Managers.Guild.MyGuildMemberData.GuildJoinTime;

        if (afterGuildJoinTime.TotalSeconds < 60 * 60 * 24)
        {
            var openRaidTime = Managers.Guild.MyGuildMemberData.GuildJoinTime.AddHours(24);
            var openGapTime = openRaidTime - Utils.GetNow();

            if (openGapTime.Hours > 0)
                Managers.Message.ShowMessage($"{openGapTime.Hours}시간 후 참여할 수 있습니다.");
            else if (openGapTime.Minutes > 0)
                Managers.Message.ShowMessage($"{openGapTime.Minutes}분 후 참여할 수 있습니다.");
            else
                Managers.Message.ShowMessage($"{openGapTime.Seconds}초 후 참여할 수 있습니다.");

            return;
        }

        if (Managers.Game.GoodsDatas[(int)Goods.GuildAllRaidTicket].Value <= 0)
        {
            Managers.Message.ShowMessage(ChartManager.GetString("Ticket_Condition_not_have"));
            return;
        }

        Managers.AllRaid.AllRaidStart();
    }

    private void OnClickPrev()
    {
        if (!ChartManager.AllGuildRaidDungeonCharts.ContainsKey(Managers.AllRaid.Step.Value - 1))
            return;

        Managers.AllRaid.Step.Value -= 1;
    }

    private void OnClickNext()
    {
        if (!ChartManager.AllGuildRaidDungeonCharts.ContainsKey(Managers.AllRaid.Step.Value + 1))
            return;

        if (Managers.AllRaid.GuildAllRaidClearInfo != null &&
            Managers.AllRaid.GuildAllRaidClearInfo.ClearStep + 1 < Managers.AllRaid.Step.Value + 1)
            return;

        Managers.AllRaid.Step.Value += 1;
    }
    #endregion
}
