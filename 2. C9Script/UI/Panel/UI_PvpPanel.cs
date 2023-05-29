using System;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_PvpPanel : UI_Panel
{
    [SerializeField] private TMP_Text RankValueText;
    [SerializeField] private TMP_Text MaterialText;
    [SerializeField] private TMP_Text GuideText;

    [SerializeField] private Toggle AutoMatchToggle;

    [SerializeField] private Image MaterialImage;

    [SerializeField] private Button CloseButton;
    [SerializeField] private Button EntryButton;

    [SerializeField] private GameObject EntryNavigationObj;

    private void Start()
    {
        CloseButton.BindEvent(() => gameObject.SetActive(false));
        AutoMatchToggle.onValueChanged.AddListener(value => Managers.Pvp.IsAutoMatch.Value = value);
        EntryButton.BindEvent(OnClickMatch);

        Managers.Pvp.IsAutoMatch.Subscribe(isAutoMatch =>
        {
            AutoMatchToggle.isOn = isAutoMatch;
        });

        SetGuideEvent();
    }
    
    private void SetGuideEvent()
    {
        if (Utils.IsAllClearGuideQuest())
            return;

        var guideComposite = new CompositeDisposable();
        
        Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigationId).AddTo(guideComposite);
        SetNavigationId(Managers.Game.UserData.ProgressGuideQuestId);

        Managers.Game.UserData.OnChangeGuideQuestProgressValue.Subscribe(SetNavigationValue).AddTo(guideComposite);
        SetNavigationValue(Managers.Game.UserData.ProgressGuideQuestValue);

        void SetNavigationId(int id)
        {
            if (Utils.IsAllClearGuideQuest())
            {
                EntryNavigationObj.SetActive(false);
                guideComposite.Clear();
                return;
            }

            EntryNavigationObj.SetActive(id == 19);
        }
        void SetNavigationValue(long value)
        {
            if (!Utils.IsCompleteGuideQuest())
                return;
            
            EntryNavigationObj.SetActive(false);
        }
    }

    public override void Open()
    {
        Managers.Pvp.IsPvp = false;

        // 내 랭킹 정보 갱신

        base.Open();

        var dungeonChart = ChartManager.DungeonCharts[(int)DungeonType.Pvp];

        RankValueText.text = $"{Managers.Rank.MyRankDatas[RankType.Pvp].Rank}위";
        MaterialImage.sprite = Managers.Resource.LoadItemIcon(ItemType.Goods, dungeonChart.EntryItemId);
        MaterialText.text =
            $"{(int)Managers.Game.GoodsDatas[dungeonChart.EntryItemId].Value}/{(int)ChartManager.SystemCharts[SystemData.Pvp_Ticket_Daily_Maxcount].Value}";
    }

    private void OnClickMatch()
    {
        Managers.Pvp.StartMatch();
    }

    private void CloseMatch()
    {
    }
}