using Castle.DynamicProxy.Generators;
using TMPro;
using UI;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_PromoPanel : UI_Panel
{
    [SerializeField] private TMP_Text ClearGradeText;
    [SerializeField] private TMP_Text ClearRewardAttackText;
    [SerializeField] private TMP_Text EntryInfoText;

    [SerializeField] private Image ClearGradeImage;
    [SerializeField] private Image EntryGradeImage;
    [SerializeField] private Image ClearBossImage;
    [SerializeField] private Image EntryBossImage;
    [SerializeField] private Image ClearCriticalRateImage;

    [SerializeField] private Button CloseButton;
    [SerializeField] private Button EntryButton;

    [SerializeField] private GameObject ClearObj;
    [SerializeField] private GameObject ClearRewardCriticalRateObj;
    [SerializeField] private GameObject RewardObj;
    [SerializeField] private GameObject EntryInfoObj;
    [SerializeField] private GameObject EntryCoverObj;
    [SerializeField] private GameObject NonGradeObj;

    [SerializeField] private GameObject EntryNavigationObj;

    private bool isCanEntry = false; 

    private void Start()
    {
        CloseButton.BindEvent(Close);
        EntryButton.BindEvent(OnClickEntry);
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
                guideComposite.Clear();
                return;
            }

            EntryNavigationObj.SetActive(id == 18);
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
        base.Open();
        SetUI();
    }

    private void SetUI()
    {
        SetClearUI();
        SetEntryInfo();
    }

    private void SetClearUI()
    {
        if (Managers.Game.UserData.PromoGrade == 0)
        {
            ClearObj.SetActive(false);
            RewardObj.SetActive(false);
            ClearBossImage.gameObject.SetActive(false);
            NonGradeObj.SetActive(true);
        }
        else
        {
            ClearObj.SetActive(true);
            RewardObj.SetActive(true);
            NonGradeObj.SetActive(false);
            ClearBossImage.gameObject.SetActive(true);
            var promoChart = ChartManager.PromoDungeonCharts[Managers.Game.UserData.PromoGrade];

            ClearGradeText.text = ChartManager.GetString(promoChart.Name);
            ClearGradeImage.sprite = Managers.Resource.LoadPromoIcon(Managers.Game.UserData.PromoGrade);
            ClearRewardAttackText.text = $"{(promoChart.ClearRewardStat1Value * 100).ToCurrencyString()}%";
            ClearBossImage.sprite = Managers.Resource.LoadMonsterIcon(promoChart.BossId);

            if (promoChart.ClearRewardStat2Id == 0)
            {
                ClearRewardCriticalRateObj.SetActive(false);
            }
            else
            {
                ClearRewardCriticalRateObj.SetActive(true);
                var criticalMultiple = Utils.GetCriticalMultiple(promoChart.ClearRewardStat2Id);

                if (ChartManager.StatCharts.TryGetValue(promoChart.ClearRewardStat2Id, out var statChart))
                    ClearCriticalRateImage.sprite = Managers.Resource.LoadStatIcon(statChart.Icon);
            }
        }
    }

    private void SetEntryInfo()
    {
        int entryPromoId = Managers.Game.UserData.PromoGrade + 1;
        int maxPromoId = (int)ChartManager.SystemCharts[SystemData.MaxPromoLevel].Value;
        
        // 최대보다 높거나 다음 등급 데이터가 없다면
        if (entryPromoId > maxPromoId || !ChartManager.PromoDungeonCharts.ContainsKey(entryPromoId))
        {
            isCanEntry = false;
            EntryButton.gameObject.SetActive(false);
            EntryInfoObj.SetActive(false);
            EntryBossImage.gameObject.SetActive(false); 
            EntryCoverObj.SetActive(true);
        }
        else
        {
            EntryCoverObj.SetActive(false);
            isCanEntry = true;
            var promoChart = ChartManager.PromoDungeonCharts[entryPromoId];
            var monsterChart = ChartManager.MonsterCharts[promoChart.BossId];
            
            EntryButton.gameObject.SetActive(true);
            EntryInfoObj.SetActive(true);
            EntryBossImage.gameObject.SetActive(true);

            EntryGradeImage.sprite = Managers.Resource.LoadPromoIcon(entryPromoId);
            EntryInfoText.text = $"{entryPromoId}.{ChartManager.GetString(promoChart.Name)}";
            EntryBossImage.sprite = Managers.Resource.LoadMonsterIcon(monsterChart.Id);
        }
    }

    private void OnClickEntry()
    {
        if (!isCanEntry)
            return;
        
        Managers.Dungeon.StartPromo(Managers.Game.UserData.PromoGrade + 1);
    }
}