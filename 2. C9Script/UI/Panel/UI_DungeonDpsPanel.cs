using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_DungeonDpsPanel : UI_Panel
{
    [SerializeField] private TMP_Text DungeonText;
    [SerializeField] private TMP_Text HighestScoreText;
    
    [FormerlySerializedAs("BossImage")] [SerializeField] private Image MonsterImage;
    [SerializeField] private Sprite NormalSprite;
    [SerializeField] private Sprite BossSprite;

    [SerializeField] private Button EntryButton;
    [SerializeField] private Button PrevButton;
    [SerializeField] private Button NextButton;

    [SerializeField] private Transform DpsItemRootTr;

    private readonly List<UI_DpsItem> _uiDpsItems = new();

    [HideInInspector] public List<int> ReceiveIds;
    [HideInInspector] public double GainValues;

    [HideInInspector] public List<int> BossReceiveIds;
    [HideInInspector] public double BossGainValues;

    private bool IsBoss => Managers.Dps.IsBoss;

    public void Start()
    {
        EntryButton.BindEvent(OnClickEntry);
        PrevButton.BindEvent(OnClickPrev);
        NextButton.BindEvent(OnClickNext);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI();
        }
    }
    
    public override void Open()
    {
        base.Open();

        SetUI();
        SetDpsItems();
    }

    private void OnDisable()
    {
        if (ReceiveIds.Count <= 0 && BossReceiveIds.Count <= 0)
            return;
        
        GameDataManager.GoodsGameData.SaveGameData();
        GameDataManager.UserGameData.SaveGameData();

        Param param = new Param();

        if (ReceiveIds.Count > 0)
        {
            param.Add("ReceiveIds", ReceiveIds);
            param.Add("GainJewel", GainValues);
        }

        if (BossReceiveIds.Count > 0)
        {
            param.Add("BossReceiveIds", BossReceiveIds);
            param.Add("BossGainJewel", BossGainValues);
        }

#if !UNITY_EDITOR
            Backend.GameLog.InsertLog("DpsReward", param);
#endif
            
        ReceiveIds.Clear();
        GainValues = 0;
            
        BossReceiveIds.Clear();
        BossGainValues = 0;
    }

    private void SetUI()
    {
        DungeonText.text = IsBoss ? "전투력 측정(보스)" : "전투력 측정(일반)";
        DungeonText.color = IsBoss ? new Color(0.85f, 0.4f, 0.4f) : new Color(0.35f, 0.35f, 0.69f);

        MonsterImage.sprite = IsBoss ? BossSprite : NormalSprite;
        
        HighestScoreText.text = IsBoss ? Managers.Game.UserData.DpsBossDungeonHighestScore.ToCurrencyString() :
            Managers.Game.UserData.DpsDungeonHighestScore.ToCurrencyString();
        
        PrevButton.gameObject.SetActive(IsBoss);
        NextButton.gameObject.SetActive(!IsBoss);
    }

    private void SetDpsItems()
    {
        if (_uiDpsItems.Count <= 0)
            DpsItemRootTr.DestroyInChildren();
        else
            _uiDpsItems.ForEach(uiDpsItem => uiDpsItem.gameObject.SetActive(false));

        int index = 0;

        List<UI_DpsItem> uiDpsItems = new();

        if (IsBoss)
        {
            var sortDpsBossDungeonCharts = ChartManager.DpsBossDungeonCharts.Values
                .OrderByDescending(dpsDungeonChart =>
                    Managers.Game.UserData.ReceivedDpsDungeonReward.Contains(dpsDungeonChart.Id))
                .ThenBy(dpsDungeonChart => dpsDungeonChart.Id);

            foreach (var dpsBossDungeonChart in sortDpsBossDungeonCharts)
            {
                UI_DpsItem uiDpsItem;

                if (index < _uiDpsItems.Count)
                    uiDpsItem = _uiDpsItems[index++];
                else
                {
                    uiDpsItem = Managers.UI.MakeSubItem<UI_DpsItem>(DpsItemRootTr);
                    uiDpsItems.Add(uiDpsItem);
                }

                uiDpsItem.UIDungeonDpsPanel = this;
                uiDpsItem.gameObject.SetActive(true);
                uiDpsItem.Init(dpsBossDungeonChart.Id, true, dpsBossDungeonChart.StageClearDps,
                    dpsBossDungeonChart.ClearRewardItemType,
                    dpsBossDungeonChart.ClearRewardItemId, dpsBossDungeonChart.ClearRewardItemValue);
            }
        }
        else
        {
            var sortDpsDungeonCharts = ChartManager.DpsDungeonCharts.Values
                .OrderByDescending(dpsDungeonChart =>
                    Managers.Game.UserData.ReceivedDpsDungeonReward.Contains(dpsDungeonChart.Id))
                .ThenBy(dpsDungeonChart => dpsDungeonChart.Id);

            foreach (var dpsDungeonChart in sortDpsDungeonCharts)
            {
                UI_DpsItem uiDpsItem;

                if (index < _uiDpsItems.Count)
                    uiDpsItem = _uiDpsItems[index++];
                else
                {
                    uiDpsItem = Managers.UI.MakeSubItem<UI_DpsItem>(DpsItemRootTr);
                    uiDpsItems.Add(uiDpsItem);
                }

                uiDpsItem.UIDungeonDpsPanel = this;
                uiDpsItem.gameObject.SetActive(true);
                uiDpsItem.Init(dpsDungeonChart.Id, false, dpsDungeonChart.StageClearDps,
                    dpsDungeonChart.ClearRewardItemType,
                    dpsDungeonChart.ClearRewardItemId, dpsDungeonChart.ClearRewardItemValue);
            }
        }

        if (uiDpsItems.Count > 0)
            uiDpsItems.ForEach(uiDpsItem => _uiDpsItems.Add(uiDpsItem));
    }

    private void OnClickEntry()
    {
        Managers.Dps.StartDps();
    }

    private void OnClickPrev()
    {
        if (!IsBoss)
            return;

        Managers.Dps.IsBoss = false;
        
        SetUI();
        SetDpsItems();
    }

    private void OnClickNext()
    {
        if (IsBoss)
            return;

        Managers.Dps.IsBoss = true;
        
        SetUI();
        SetDpsItems();
    }
}