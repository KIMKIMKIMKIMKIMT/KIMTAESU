using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;


public class UI_SummonRatePopup : UI_Popup
{
    [SerializeField] private TMP_Text SummonLvText;
    [SerializeField] private TMP_Text NormalRateText;
    [SerializeField] private TMP_Text RareRateText;
    [SerializeField] private TMP_Text UniqueRateText;
    [SerializeField] private TMP_Text LegendRateText;
    [SerializeField] private TMP_Text LegenoRateText;
    [SerializeField] private TMP_Text LvRewardValueText;

    [SerializeField] private Image RewardItemGradeBackgroundImage;
    [SerializeField] private Image RewardItemSubGradeImage;
    [SerializeField] private Image RewardItemImage;

    [SerializeField] private Button BackgroundButton;
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button PrevButton;
    [SerializeField] private Button NextButton;
    [SerializeField] private Button ReceiveLvRewardButton;

    [SerializeField] private GameObject ReceiveRewardObj;
    
    public override bool isTop => true;

    private SummonType _summonType;
    private int _summonLv;

    private void Start()
    {
        BackgroundButton.BindEvent(ClosePopup);
        CloseButton.BindEvent(ClosePopup);
        PrevButton.BindEvent(OnClickPrev);
        NextButton.BindEvent(OnClickNext);
        ReceiveLvRewardButton.BindEvent(OnClickReceiveLvReward);
    }

    public void Init(SummonType summonType)
    {
        _summonType = summonType;
        
        switch (summonType)
        {
            case SummonType.Weapon:
                _summonLv = Managers.Game.UserData.SummonWeaponLv;
                break;
            case SummonType.Pet:
                _summonLv = Managers.Game.UserData.SummonPetLv;
                break;
        }
        
        SetUI();
    }

    private void SetUI()
    {
        SummonLvText.text = $"소환레벨 {_summonLv}";
        
        switch (_summonType)
        {
            case SummonType.Weapon:
            {
                PrevButton.gameObject.SetActive(ChartManager.WeaponSummonLevelCharts.ContainsKey(_summonLv - 1));
                NextButton.gameObject.SetActive(ChartManager.WeaponSummonLevelCharts.ContainsKey(_summonLv + 1));

                var weaponSummonLvChart = ChartManager.WeaponSummonLevelCharts[_summonLv];

                NormalRateText.text = $"{weaponSummonLvChart.NormalRate}%";
                RareRateText.text = $"{weaponSummonLvChart.RareRate}%";
                UniqueRateText.text = $"{weaponSummonLvChart.UniqueRate}%";
                LegendRateText.text = $"{weaponSummonLvChart.LegendRate}%";
                LegenoRateText.text = $"{weaponSummonLvChart.LegenoRate}%";

                LvRewardValueText.text = $"X {weaponSummonLvChart.RewardItemValue}";

                ReceiveRewardObj.SetActive(Managers.Game.UserData.ReceivedSummonWeaponReward.Contains(_summonLv));

                if (weaponSummonLvChart.RewardItemType == ItemType.Weapon)
                {
                    var weaponChart = ChartManager.WeaponCharts[weaponSummonLvChart.RewardItemId];

                    RewardItemImage.sprite = Managers.Resource.LoadWeaponIcon(weaponChart.Icon);
                    RewardItemGradeBackgroundImage.gameObject.SetActive(true);
                    RewardItemGradeBackgroundImage.sprite = Managers.Resource.LoadItemGradeBg(weaponChart.Grade);
                    RewardItemSubGradeImage.gameObject.SetActive(true);
                    RewardItemSubGradeImage.sprite = Managers.Resource.LoadSubGradeIcon(weaponChart.SubGrade);
                }
                else if (weaponSummonLvChart.RewardItemType == ItemType.Goods)
                {
                    var goodsChart = ChartManager.GoodsCharts[weaponSummonLvChart.RewardItemId];
                    RewardItemImage.sprite = Managers.Resource.LoadGoodsIcon(goodsChart.Icon);
                    RewardItemGradeBackgroundImage.gameObject.SetActive(false);
                    RewardItemSubGradeImage.gameObject.SetActive(false);
                }
            }
                break;
            case SummonType.Pet:
            {
                PrevButton.gameObject.SetActive(ChartManager.PetSummonLevelCharts.ContainsKey(_summonLv - 1));
                NextButton.gameObject.SetActive(ChartManager.PetSummonLevelCharts.ContainsKey(_summonLv + 1));
                                                             
                var petSummonLvChart = ChartManager.PetSummonLevelCharts[_summonLv];

                NormalRateText.text = $"{petSummonLvChart.NormalRate}%";
                RareRateText.text = $"{petSummonLvChart.RareRate}%";
                UniqueRateText.text = $"{petSummonLvChart.UniqueRate}%";
                LegendRateText.text = $"{petSummonLvChart.LegendRate}%";
                LegenoRateText.text = $"{petSummonLvChart.LegenoRate}%";

                LvRewardValueText.text = $"X {petSummonLvChart.RewardItemValue}";

                ReceiveRewardObj.SetActive(Managers.Game.UserData.ReceivedSummonPetReward.Contains(_summonLv));

                if (petSummonLvChart.RewardItemType == ItemType.Pet)
                {
                    var petChart = ChartManager.PetCharts[petSummonLvChart.RewardItemId];

                    RewardItemImage.sprite = Managers.Resource.LoadPetIcon(petChart.Icon);
                    RewardItemGradeBackgroundImage.gameObject.SetActive(true);
                    RewardItemGradeBackgroundImage.sprite = Managers.Resource.LoadItemGradeBg(petChart.Grade);
                    RewardItemSubGradeImage.gameObject.SetActive(true);
                    RewardItemSubGradeImage.sprite = Managers.Resource.LoadSubGradeIcon(petChart.SubGrade);
                }
                else if (petSummonLvChart.RewardItemType == ItemType.Goods)
                {
                    var goodsChart = ChartManager.GoodsCharts[petSummonLvChart.RewardItemId];
                    RewardItemImage.sprite = Managers.Resource.LoadGoodsIcon(goodsChart.Icon);
                    RewardItemGradeBackgroundImage.gameObject.SetActive(false);
                    RewardItemSubGradeImage.gameObject.SetActive(false);
                }
            }
                break;
        }
    }

    private void OnClickPrev()
    {
        _summonLv--;
        if (!ChartManager.WeaponSummonLevelCharts.ContainsKey(_summonLv))
            _summonLv = 1;
        
        SetUI();
    }

    private void OnClickNext()
    {
        _summonLv++;
        if (!ChartManager.WeaponSummonLevelCharts.ContainsKey(_summonLv))
            _summonLv = ChartManager.WeaponSummonLevelCharts.Max(data => data.Key);
        
        SetUI();
    }

    private void OnClickReceiveLvReward()
    {
        switch (_summonType)
        {
            case SummonType.Weapon:
            {
                if (Managers.Game.UserData.SummonWeaponLv < _summonLv)
                    return;

                if (Managers.Game.UserData.ReceivedSummonWeaponReward.Contains(_summonLv))
                    return;

                var weaponSummonLvChart = ChartManager.WeaponSummonLevelCharts[_summonLv];
                Managers.Game.IncreaseItem(weaponSummonLvChart.RewardItemType, weaponSummonLvChart.RewardItemId, weaponSummonLvChart.RewardItemValue);
                Managers.Game.UserData.ReceivedSummonWeaponReward.Add(_summonLv);
                
                GameDataManager.SaveItemData(weaponSummonLvChart.RewardItemType);
                GameDataManager.UserGameData.SaveGameData();
                
                SetUI();
            }
                break;
            case SummonType.Pet:
            {
                if (Managers.Game.UserData.SummonPetLv < _summonLv)
                    return;

                if (Managers.Game.UserData.ReceivedSummonPetReward.Contains(_summonLv))
                    return;

                var petSummonLvChart = ChartManager.PetSummonLevelCharts[_summonLv];
                Managers.Game.IncreaseItem(petSummonLvChart.RewardItemType, petSummonLvChart.RewardItemId, petSummonLvChart.RewardItemValue);
                Managers.Game.UserData.ReceivedSummonPetReward.Add(_summonLv);
                
                GameDataManager.SaveItemData(petSummonLvChart.RewardItemType);
                GameDataManager.UserGameData.SaveGameData();
                
                SetUI();
            }
                break;
        }
    }
}