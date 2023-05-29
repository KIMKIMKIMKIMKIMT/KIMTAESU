using System;
using NSubstitute.Exceptions;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_ChulwadaePopup : UI_Popup
{
    enum PanelType
    {
        Promo,
        Pvp,
        Raid,
    }

    [SerializeField] private UI_Panel[] Panels;

    [SerializeField] private Button[] Buttons;

    [SerializeField] private TMP_Text PromoGradeText;
    [SerializeField] private TMP_Text PvpRankText;
    [SerializeField] private TMP_Text RaidRankText;

    [SerializeField] private Image PromoGradeImage;
    [SerializeField] private Image RaidRankImage;

    [SerializeField] private GameObject NonGradeObj;

    [SerializeField] private GameObject PromoNavigationObj;
    [SerializeField] private GameObject PvpNavigationObj;

    private void Start()
    {
        foreach (PanelType panelType in Enum.GetValues(typeof(PanelType)))
        {
            int index = (int)panelType;

            if (index >= Panels.Length || index >= Buttons.Length)
                continue;

            Buttons[index].BindEvent(() => Panels[index].Open());
        }

        SetGuideEvent();
    }

    private void SetGuideEvent()
    {
        if (Utils.IsAllClearGuideQuest())
            return;

        CompositeDisposable guideComposite = new();
        
        Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigation).AddTo(guideComposite);
        SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);

        void SetNavigation(int id)
        {
            if (Utils.IsAllClearGuideQuest())
            {
                guideComposite.Clear();
                PromoNavigationObj.SetActive(false);
                PvpNavigationObj.SetActive(false);
                return;
            }
            
            PromoNavigationObj.SetActive(id == 18);
            PvpNavigationObj.SetActive(id == 19);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (PanelType panelType in Enum.GetValues(typeof(PanelType)))
            {
                int index = (int)panelType;

                if (index >= Panels.Length || index >= Buttons.Length)
                    continue;

                if (Panels[index].gameObject.activeSelf)
                {
                    UI_RaidPanel uiRaidPanel = Panels[index] as UI_RaidPanel;

                    if (uiRaidPanel != null && uiRaidPanel.CloseSkipPanel())
                    {
                        return;
                    }


                    Panels[index].Close();
                    return;
                }
            }

            ClosePopup();
        }
    }

    public override void Open()
    {
        base.Open();

        foreach (var uiPanel in Panels)
            uiPanel.Close();

        SetPromoUI();
        SetPvpUI();
        SetRaidUI();
    }

    private void SetPromoUI()
    {
        if (Managers.Game.UserData.PromoGrade == 0)
        {
            PromoGradeImage.gameObject.SetActive(false);
            PromoGradeText.gameObject.SetActive(false);
            NonGradeObj.SetActive(true);
        }
        else
        {
            PromoGradeImage.gameObject.SetActive(true);
            PromoGradeText.gameObject.SetActive(true);
            NonGradeObj.SetActive(false);

            PromoGradeImage.sprite = Managers.Resource.LoadPromoIcon(Managers.Game.UserData.PromoGrade);
            PromoGradeText.text =
                ChartManager.GetString(ChartManager.PromoDungeonCharts[Managers.Game.UserData.PromoGrade].Name);
        }
    }

    private void SetPvpUI()
    {
        PvpRankText.text = Managers.Rank.MyRankDatas.ContainsKey(RankType.Pvp)
            ? $"{Managers.Rank.MyRankDatas[RankType.Pvp].Rank}위"
            : "-위";
    }

    private void SetRaidUI()
    {
        RaidRankText.text = Managers.Rank.MyRankDatas.ContainsKey(RankType.Raid)
            ? $"{Managers.Rank.MyRankDatas[RankType.Raid].Rank}위"
            : "-위";
        RaidRankImage.sprite = Managers.Rank.MyRankDatas.ContainsKey(RankType.Raid)
            ? Managers.Resource.LoadRankIcon(Managers.Rank.MyRankDatas[RankType.Raid].Rank)
            : Managers.Resource.LoadRankIcon(1);
    }
}