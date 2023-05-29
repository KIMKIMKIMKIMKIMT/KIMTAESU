using System;
using System.Linq;
using Chart;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_RaidPanel : UI_Panel
{
    [SerializeField] private TMP_Text RankText;
    [SerializeField] private TMP_Text ClearStepText;
    [SerializeField] private TMP_Text ClearTimeText;
    [SerializeField] private TMP_Text HighestGoldText;
    [SerializeField] private TMP_Text HighestGoldBarText;
    [SerializeField] private TMP_Text HighestSkillStoneText;
    [SerializeField] private TMP_Text EntryGoodsText;
    [SerializeField] private TMP_Text StepText;
    [SerializeField] private TMP_Text MonsterNameText;

    [SerializeField] private Image RankImage;
    [SerializeField] private Image MonsterImage;

    [SerializeField] private Button PrevButton;
    [SerializeField] private Button NextButton;
    [SerializeField] private Button EntryButton;
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button SkipButton;

    [SerializeField] private UI_RaidSkipPanel UIRaidSkipPanel;

    private DungeonChart _dungeonChart;

    private readonly CompositeDisposable _compositeDisposable = new();

    private void Start()
    {
        Managers.Raid.Step.Subscribe(step =>
        {
            StepText.text = $"{step}단계";
        }); 
        
        PrevButton.BindEvent(OnClickPrev);
        NextButton.BindEvent(OnClickNext);
        EntryButton.BindEvent(OnClickEntry);
        SkipButton.BindEvent(OnClickSkip);
        CloseButton.BindEvent(Close);
    }

    private void OnDisable()
    {
        _compositeDisposable.Clear();
    }

    public bool CloseSkipPanel()
    {
        if (UIRaidSkipPanel.gameObject.activeSelf)
        {
            UIRaidSkipPanel.Close();
            return true;
        }

        return false;
    }
    
    
    public override void Open()
    {
        base.Open();
        
        UIRaidSkipPanel.Close();
        
        if (_dungeonChart == null && !ChartManager.DungeonCharts.TryGetValue((int)DungeonType.Raid, out _dungeonChart))
            Debug.LogError("Fail Load DungeonChart Raid(7)");
        
        Managers.Game.GoodsDatas[_dungeonChart.EntryItemId].Subscribe(goodsValue =>
        {
            EntryGoodsText.text = $"{goodsValue}/{ChartManager.SystemCharts[SystemData.Raid_Ticket_Daily_Count].Value}";
        }).AddTo(_compositeDisposable);

        if (Managers.Raid.RaidClearInfo != null)
        {
            if (ChartManager.RaidDungeonCharts.ContainsKey(Managers.Raid.RaidClearInfo.ClearStep + 1))
                Managers.Raid.Step.Value = Managers.Raid.RaidClearInfo.ClearStep + 1;
            else
                Managers.Raid.Step.Value = Managers.Raid.RaidClearInfo.ClearStep;
        }
        else
            Managers.Raid.Step.Value = 1;
        
        SetUI();
    }

    private void SetUI()
    {
        if (Managers.Raid.RaidClearInfo != null)
        {
            var clearInfo = Managers.Raid.RaidClearInfo;

            int rank = Managers.Rank.MyRankDatas.ContainsKey(RankType.Raid) ? Managers.Rank.MyRankDatas[RankType.Raid].Rank : 0;

            RankImage.gameObject.SetActive(true);
            RankImage.sprite = Managers.Resource.LoadRankIcon(rank);
            RankText.text = $"{rank}위";
            ClearStepText.text = $"{clearInfo.ClearStep}단계";
            ClearTimeText.text = $"{clearInfo.ClearTime:N02}초";
            HighestGoldText.text = clearInfo.HighestGold.ToCurrencyString();
            HighestGoldBarText.text = clearInfo.HighestGoldBar.ToCurrencyString();
            HighestSkillStoneText.text = clearInfo.HighestSkillStone.ToCurrencyString();
        }
        // 클리어 정보 없음
        else
        {
            RankImage.gameObject.SetActive(false);
            RankText.text = "-";
            ClearStepText.text = "-단계";
            ClearTimeText.text = "-";
            HighestGoldText.text = "-";
            HighestGoldBarText.text = "-";
            HighestSkillStoneText.text = "-";
        }
        
        //EntryGoodsText.text = $"{Managers.Game.GoodsDatas[_dungeonChart.EntryItemId].Value}/{ChartManager.SystemCharts[SystemData.Raid_Ticket_Daily_Count].Value}";

        if (ChartManager.RaidDungeonCharts.TryGetValue(Managers.Raid.Step.Value, out var raidDungeonChart))
        {
            if (ChartManager.MonsterCharts.TryGetValue(raidDungeonChart.Wave3MonsterId, out var monsterChart))
            {
                MonsterNameText.text = $"<{ChartManager.GetString(monsterChart.Name)}>";
                MonsterImage.sprite = Managers.Resource.LoadMonsterIcon(monsterChart.Id);
            }
            else
            {
                Debug.LogError($"Fail Load MonsterChart : {raidDungeonChart.Wave3MonsterId}");
                MonsterNameText.text = string.Empty;
                MonsterImage.sprite = null;
            }
        }
        else
        {
            Debug.LogError($"Fail Load RaidDungeonChart : {Managers.Raid.Step.Value}");
            MonsterNameText.text = string.Empty;
            MonsterImage.sprite = null;
        }

        PrevButton.gameObject.SetActive(ChartManager.RaidDungeonCharts.ContainsKey(Managers.Raid.Step.Value - 1));
        NextButton.gameObject.SetActive(
            (Managers.Raid.RaidClearInfo != null && (Managers.Raid.RaidClearInfo.ClearStep + 1) > Managers.Raid.Step.Value)
            && ChartManager.RaidDungeonCharts.ContainsKey(Managers.Raid.Step.Value + 1));
    }

    private void OnClickPrev()
    {
        int prevIndex = Managers.Raid.Step.Value - 1;

        if (!ChartManager.RaidDungeonCharts.ContainsKey(prevIndex))
            return;

        Managers.Raid.Step.Value = prevIndex;
        SetUI();
    }

    private void OnClickNext()
    {
        int nextIndex = Managers.Raid.Step.Value + 1;

        if (!ChartManager.RaidDungeonCharts.ContainsKey(nextIndex))
            return;

        Managers.Raid.Step.Value = nextIndex;
        SetUI();
    }

    private void OnClickEntry()
    {
        if (!Utils.IsEnoughItem(ItemType.Goods, _dungeonChart.EntryItemId, 1))
        {
            Managers.Message.ShowMessage("재화가 부족합니다.");
            return;
        }
        
        Managers.Raid.Start();
    }

    private void OnClickSkip()
    {
        if (Managers.Raid.RaidClearInfo == null || Managers.Raid.RaidClearInfo.ClearStep <= 0)
        {
            Managers.Message.ShowMessage("해당 컨텐츠 플레이 전적이 없는 상태에서는 스킵이 불가능 합니다.");
            return;
        }
        
        UIRaidSkipPanel.Open();
    }
}