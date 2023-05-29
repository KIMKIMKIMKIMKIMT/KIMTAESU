using System;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_DungeonItem : UI_Base
{
    [SerializeField] private TMP_Text NameText;
    [SerializeField] private TMP_Text DescText;
    [SerializeField] private TMP_Text EntryItemText;
    [SerializeField] private TMP_Text EntryButtonText;
    [SerializeField] private TMP_Text SkipButtonText;
    [SerializeField] private TMP_Text HighestItemText;
    [SerializeField] private TMP_Text HighestItemValueText;
    [SerializeField] private TMP_Text DungeonStepText;

    [SerializeField] private Image DungeonBackgroundImage;
    [SerializeField] private Image EntryItemImage;
    [SerializeField] private Image EntryButtonItemImage;
    [SerializeField] private Image SkipButtonItemImage;
    [SerializeField] private Image HighestItemImage;

    [SerializeField] private Button EntryButton;
    [SerializeField] private Button SkipButton;
    [SerializeField] private Button PrevStepButton;
    [SerializeField] private Button NextStepButton;
    [SerializeField] private Button ShopButton;

    [SerializeField] private GameObject NavigationObj;

    private int _dungeonId;
    private int _dungeonStep;
    private Action<int> _onSkillCallback;
    private CompositeDisposable _guideComposite = new();

    private void Start()
    {
        Color color = Utils.DungeonColor[_dungeonId];

        var dungeonChart = ChartManager.DungeonCharts[_dungeonId];

        NameText.text = ChartManager.GetString(dungeonChart.Name);
        NameText.color = color;
        DescText.text = ChartManager.GetString(dungeonChart.Desc);

        EntryItemText.color = color;

        EntryButtonText.text = "입 장";
        EntryButtonText.color = color;

        SkipButtonText.text = "스 킵";
        SkipButtonText.color = color;

        HighestItemText.text =
            string.Format(ChartManager.GetString("Dungeon_BestRecord"),
                Utils.ItemColor[dungeonChart.RewardItemId].ToHexString(),
                ChartManager.GetString(ChartManager.GoodsCharts[dungeonChart.RewardItemId].Name));

        Sprite entryItemSprite =
            Managers.Resource.LoadGoodsIcon(ChartManager.GoodsCharts[dungeonChart.EntryItemId].Icon);

        DungeonBackgroundImage.sprite = Managers.Resource.LoadBg(dungeonChart.WorldId);
        EntryItemImage.sprite = entryItemSprite;
        EntryButtonItemImage.sprite = entryItemSprite;
        SkipButtonItemImage.sprite = entryItemSprite;

        HighestItemImage.sprite =
            Managers.Resource.LoadGoodsIcon(ChartManager.GoodsCharts[dungeonChart.RewardItemId].Icon);

        EntryButton.BindEvent(OnClickEntry);
        SkipButton.BindEvent(() => _onSkillCallback?.Invoke(_dungeonId));

        PrevStepButton.BindEvent(OnClickPrevStep);
        NextStepButton.BindEvent(OnClickNextStep);
        ShopButton.BindEvent(OnClickShop);

        Managers.Game.GoodsDatas[dungeonChart.EntryItemId].Subscribe(goodsCount =>
        {
            EntryItemText.text =
                $"{goodsCount}/" +
                $"{ChartManager.SystemCharts[SystemData.Dungeon_Ticket_Daily_Maxcount].Value}";
        });

        if (!Utils.IsAllClearGuideQuest())
        {
            Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigation).AddTo(_guideComposite);
            SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);
        }
    }

    public void SetItem(int dungeonId, Action<int> onSkipCallback)
    {
        _dungeonId = dungeonId;
        _onSkillCallback = onSkipCallback;
    }

    public override void Open()
    {
        base.Open();
        var dungeonChart = ChartManager.DungeonCharts[_dungeonId];
        if (dungeonChart.MaxStep < Managers.Game.UserData.GetDungeonClearStep(_dungeonId) + 1)
            _dungeonStep = Managers.Game.UserData.GetDungeonClearStep(_dungeonId);
        else
            _dungeonStep = Managers.Game.UserData.GetDungeonClearStep(_dungeonId) + 1;
        SetUI();
    }

    private void SetUI()
    {
        var dungeonChart = ChartManager.DungeonCharts[_dungeonId];

        // EntryItemText.text =
        //     $"{Managers.Game.GoodsDatas[dungeonChart.EntryItemId]}/" +
        //     $"{ChartManager.SystemCharts[SystemData.Dungeon_Ticket_Daily_Maxcount].Value}";

        DungeonStepText.text = $"{_dungeonStep}단계";
        HighestItemValueText.text = Managers.Game.UserData.GetDungeonHighestValue(_dungeonId).ToCurrencyString();

        PrevStepButton.gameObject.SetActive(_dungeonStep > 1);
        NextStepButton.gameObject.SetActive(_dungeonStep < dungeonChart.MaxStep &&
                                            _dungeonStep <= Managers.Game.UserData.GetDungeonClearStep(_dungeonId));

        if (!Utils.IsAllClearGuideQuest())
            SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);
    }
    
    void SetNavigation(int id)
    {
        if (Utils.IsAllClearGuideQuest())
        {
            _guideComposite.Clear();
            return;
        }
        
        if (Utils.IsCompleteGuideQuest())
        {
            NavigationObj.SetActive(false);
            return;
        }
            
        switch (_dungeonId)
        {
            case 1 :
                NavigationObj.SetActive(id == 20);
                break;
            case 2:
                NavigationObj.SetActive(id == 21);
                break;
            case 3:
                NavigationObj.SetActive(id == 22);
                break;
            default:
                NavigationObj.SetActive(false);
                break;
        }
    }

    private void OnClickEntry()
    {
        var dungeonChart = ChartManager.DungeonCharts[_dungeonId];

        if (Managers.Game.GoodsDatas[dungeonChart.EntryItemId].Value <= 0)
        {
            Managers.Message.ShowMessage("입장권이 부족합니다!");
            return;
        }

        Managers.Dungeon.StartDungeon(_dungeonId, _dungeonStep);
    }

    private void OnClickPrevStep()
    {
        if (_dungeonStep <= 1)
            return;

        _dungeonStep--;
        SetUI();
    }

    private void OnClickNextStep()
    {
        int clearStep = 0;

        switch (_dungeonId)
        {
            case (int)DungeonType.Hwasengbang:
                if (!ChartManager.HwasengbangDungeonCharts.ContainsKey(_dungeonStep + 1))
                    return;
                clearStep = Managers.Game.UserData.Dungeon1ClearStep;
                break;
            case (int)DungeonType.MarinCamp:
                if (!ChartManager.MarinCampDungeonCharts.ContainsKey(_dungeonStep + 1))
                    return;
                clearStep = Managers.Game.UserData.Dungeon2ClearStep;
                break;
            case (int)DungeonType.March:
                if (!ChartManager.MarchDungeonCharts.ContainsKey(_dungeonStep + 1))
                    return;
                clearStep = Managers.Game.UserData.Dungeon3ClearStep;
                break;
        }

        if (_dungeonStep >= clearStep + 1)
            return;

        _dungeonStep++;
        SetUI();
    }

    private void OnClickShop()
    {
        Managers.UI.ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_ShopPopup>();
    }
}