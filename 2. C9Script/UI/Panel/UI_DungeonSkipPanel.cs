using System;
using BackEnd;
using Chart;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_DungeonSkipPanel : UI_Panel
{
    [SerializeField] private TMP_Text DungeonNameText;
    [SerializeField] private TMP_Text MaterialText;
    [SerializeField] private TMP_Text SkipButtonText;

    [SerializeField] private Image MaterialImage;

    [SerializeField] private Button CloseButton;
    [SerializeField] private Button PrevDungeonButton;
    [SerializeField] private Button NextDungeonButton;
    [SerializeField] private Button MinButton;
    [SerializeField] private Button MaxButton;
    [SerializeField] private Button PlusButton;
    [SerializeField] private Button MinusButton;
    [SerializeField] private Button SkipButton;

    [SerializeField] private GameObject ShadowBackgroundObj;

    [SerializeField] private UI_DungeonSkipResultPanel UIDungeonSkipResultPanel;

    private int _dungeonId;
    private int _skipCount = 1;
    private DungeonChart _dungeonChart;

    private bool _isSkip;

    public void Start()
    {
        CloseButton.BindEvent(OnClickClose);
        PrevDungeonButton.BindEvent(OnClickPrevDungeon);
        NextDungeonButton.BindEvent(OnClickNextDungeon);
        MinButton.BindEvent(OnClickMin);
        MaxButton.BindEvent(OnClickMax);
        PlusButton.BindEvent(OnClickPlus);
        PlusButton.BindEvent(OnClickPlus, UIEvent.Pressed);
        MinusButton.BindEvent(OnClickMinus);
        MinusButton.BindEvent(OnClickMinus, UIEvent.Pressed);
        SkipButton.BindEvent(OnClickSkip);
    }

    public override void Open()
    {
        base.Open();
        _isSkip = false;
    }

    private void OnClickClose()
    {
        ShadowBackgroundObj.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnClickPrevDungeon()
    {
        if (_dungeonId - 1 < (int)DungeonType.Hwasengbang)
            return;
        
        SetPanel(_dungeonId - 1);
    }

    private void OnClickNextDungeon()
    {
        if (_dungeonId + 1 > (int)DungeonType.March)
            return;
        
        SetPanel(_dungeonId + 1);
    }

    private void OnClickMin()
    {
        _skipCount = Managers.Game.GoodsDatas[_dungeonChart.EntryItemId].Value > 0 ? 1 : 0;
        Refresh();
    }

    private void OnClickMax()
    {
        _skipCount = Managers.Game.GoodsDatas[_dungeonChart.EntryItemId].Value > 0 ? 
            (int)Managers.Game.GoodsDatas[_dungeonChart.EntryItemId].Value : 0;
        Refresh();
    }

    private void OnClickPlus()
    {
        _skipCount = Mathf.Min((int)Managers.Game.GoodsDatas[_dungeonChart.EntryItemId].Value, _skipCount + 1);
        Refresh();
    }

    private void OnClickMinus()
    {
        _skipCount = Mathf.Max(0, _skipCount - 1);
        Refresh();
    }

    private void OnClickSkip()
    {
        if (_skipCount <= 0)
            return;

        if (Managers.Game.UserData.GetDungeonClearStep(_dungeonId) <= 0 ||
            Managers.Game.UserData.GetDungeonHighestValue(_dungeonId) <= 0)
        {
            Managers.Message.ShowMessage("해당 컨텐츠 플레이 전적이 없는 상태에서는 스킵이 불가능합니다!");
            return;
        }

        if (!Utils.IsEnoughItem(ItemType.Goods, _dungeonChart.EntryItemId, _skipCount))
        {
            Managers.Message.ShowMessage("재화가 부족합니다.");
            return;
        }
        
        if (_isSkip)
            return;

        _isSkip = true;

        Managers.Game.DecreaseItem(ItemType.Goods, _dungeonChart.EntryItemId, _skipCount);
        Managers.Game.IncreaseItem(ItemType.Goods, _dungeonChart.RewardItemId,
            Managers.Game.UserData.GetDungeonHighestValue(_dungeonId) * _skipCount);
        
        GameDataManager.UserGameData.SaveGameData();
        GameDataManager.GoodsGameData.SaveGameData();
        
        ShadowBackgroundObj.SetActive(false);

        UIDungeonSkipResultPanel.Init(Managers.Game.UserData.GetDungeonClearStep(_dungeonId), _dungeonChart.RewardItemId,
            Managers.Game.UserData.GetDungeonHighestValue(_dungeonId) * _skipCount, _skipCount);

        Param param = new();
        
        param.Add(((DungeonType)_dungeonId).ToString(), new DungeonSkipLog()
        {
            GainValue = Managers.Game.UserData.GetDungeonHighestValue(_dungeonId) * _skipCount,
            ItemId = _dungeonChart.RewardItemId,
            ItemType = ItemType.Goods,
            SkipCount = _skipCount
        });
        Utils.GetGoodsLog(ref param);
        
        Backend.GameLog.InsertLog("Dungeon", param);
        
        Close();
    }

    public void SetPanel(int dungeonId)
    {
        ShadowBackgroundObj.SetActive(true);

        _dungeonId = dungeonId;
        _isSkip = false;
        SetUI();
    }

    private void SetUI()
    {
        if (!ChartManager.DungeonCharts.TryGetValue(_dungeonId, out  _dungeonChart))
        {
            Debug.LogError($"Can't Find {_dungeonId} DungeonChart!!");
            return;
        }

        if (!ChartManager.GoodsCharts.TryGetValue(_dungeonChart.EntryItemId, out var entryItemChart))
        {
            Debug.LogError($"Can't Find {_dungeonChart.EntryItemId} ItemChart!!");
            return;
        }

        DungeonNameText.text = ChartManager.GetString(_dungeonChart.Name);
        DungeonNameText.color = Utils.DungeonColor[_dungeonId];

        MaterialImage.sprite = Managers.Resource.LoadGoodsIcon(entryItemChart.Icon);

        _skipCount = Managers.Game.GoodsDatas[_dungeonChart.EntryItemId].Value > 0 ? 1 : 0;

        PrevDungeonButton.gameObject.SetActive(ChartManager.DungeonCharts.ContainsKey(_dungeonId - 1));
        NextDungeonButton.gameObject.SetActive(ChartManager.DungeonCharts.ContainsKey(_dungeonId + 1) && _dungeonId <= (int)DungeonType.March);
        
        Refresh();
    }

    public override void Refresh()
    {
        MaterialText.text = $"{_skipCount} / " +
                            $"{Managers.Game.GoodsDatas[_dungeonChart.EntryItemId]}";
        SkipButtonText.text = $"{_skipCount}회 스킵";
    }
}