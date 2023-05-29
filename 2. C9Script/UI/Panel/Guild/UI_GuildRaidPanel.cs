using NSubstitute.ClearExtensions;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuildRaidPanel : UI_Panel
{
    [SerializeField] private TMP_Text ClearStepText;
    [SerializeField] private TMP_Text ClearTimeText;
    [SerializeField] private TMP_Text HighestGainGoldText;
    [SerializeField] private TMP_Text HighestGainGoldBarText;
    [SerializeField] private TMP_Text HighestGainGuildPointText;
    [SerializeField] private TMP_Text MonsterNameText;
    [SerializeField] private TMP_Text StepText;
    [SerializeField] private TMP_Text EntryCountText;

    [SerializeField] private Image MonsterImage;
    [SerializeField] private Image _goodsImg;

    [SerializeField] private Button PrevButton;
    [SerializeField] private Button NextButton;
    [SerializeField] private Button EntryButton;
    [SerializeField] private Button CloseButton;

    private void Start()
    {
        PrevButton.BindEvent(OnClickPrev);
        NextButton.BindEvent(OnClickNext);
        EntryButton.BindEvent(OnClickEntry);
        CloseButton.BindEvent(Close);

        _goodsImg.sprite = Managers.Resource.LoadGoodsIcon(ChartManager.GoodsCharts[(int)Goods.GuildRaidTicket].Icon);

        Managers.GuildRaid.Step.Subscribe(SetStepUI);
    }

    public override void Open()
    {
        base.Open();
        
        SetUI();
    }

    private void SetUI()
    {
        // 클리어 정보가 있을때.
        if (Managers.GuildRaid.GuildRaidClearInfo != null && Managers.GuildRaid.GuildRaidClearInfo.ClearStep >= 1)
        {
            var guildRaidClearInfo = Managers.GuildRaid.GuildRaidClearInfo;
            
            ClearStepText.text = $"{guildRaidClearInfo.ClearStep}단계";
            ClearTimeText.text = $"{(guildRaidClearInfo.ClearTime / 60):00}:{(guildRaidClearInfo.ClearTime % 60):00}";
            HighestGainGoldText.text = guildRaidClearInfo.HighestGold.ToCurrencyString();
            HighestGainGoldBarText.text = guildRaidClearInfo.HighestGoldBar.ToCurrencyString();
            HighestGainGuildPointText.text = guildRaidClearInfo.HighestGuildPoint.ToCurrencyString();
            
            // 단계 최고단계로 설정
            Managers.GuildRaid.Step.Value = ChartManager.GuildRaidDungeonCharts.ContainsKey(guildRaidClearInfo.ClearStep + 1) ? guildRaidClearInfo.ClearStep + 1 : guildRaidClearInfo.ClearStep;
        }
        // 클리어 정보가 없을때.
        else
        {
            ClearStepText.text = "-단계";
            ClearTimeText.text = "-:-";
            HighestGainGoldText.text = "-";
            HighestGainGoldBarText.text = "-";
            HighestGainGuildPointText.text = "-";
            
            Managers.GuildRaid.Step.Value = 1;
        }

        EntryCountText.text = $"{Managers.Game.GoodsDatas[(int)Goods.GuildRaidTicket].Value}/1";
    }

    private void SetStepUI(int step)
    {
        if (!ChartManager.GuildRaidDungeonCharts.TryGetValue(step, out var guildRaidDungeonChart))
            return;

        if (!ChartManager.MonsterCharts.TryGetValue(guildRaidDungeonChart.Wave3MonsterId, out var monsterChart))
            return;

        StepText.text = $"{step}단계";
        MonsterImage.sprite = Managers.Resource.LoadMonsterIcon(guildRaidDungeonChart.Wave3MonsterId);
        MonsterNameText.text = $"<{ChartManager.GetString(monsterChart.Name)}>";
        
        PrevButton.gameObject.SetActive(ChartManager.GuildRaidDungeonCharts.ContainsKey(step - 1));
        NextButton.gameObject.SetActive(Managers.GuildRaid.GuildRaidClearInfo != null &&
                                        Managers.GuildRaid.GuildRaidClearInfo.ClearStep + 1 > step &&
                                        ChartManager.GuildRaidDungeonCharts.ContainsKey(step + 1));
    }

    private void OnClickPrev()
    {
        if (!ChartManager.GuildRaidDungeonCharts.ContainsKey(Managers.GuildRaid.Step.Value - 1))
            return;

        Managers.GuildRaid.Step.Value -= 1;
    }

    private void OnClickNext()
    {
        if (!ChartManager.GuildRaidDungeonCharts.ContainsKey(Managers.GuildRaid.Step.Value + 1))
            return;

        if (Managers.GuildRaid.GuildRaidClearInfo != null &&
            Managers.GuildRaid.GuildRaidClearInfo.ClearStep + 1 < Managers.GuildRaid.Step.Value + 1)
            return;

        Managers.GuildRaid.Step.Value += 1;
    }

    private void OnClickEntry()
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

        if (Managers.Game.GoodsDatas[(int)Goods.GuildRaidTicket].Value <= 0)
        {
            Managers.Message.ShowMessage(ChartManager.GetString("Ticket_Condition_not_have"));
            return;
        }
        
        Managers.GuildRaid.Start();
    }
}