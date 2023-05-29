using UnityEngine;
using UnityEngine.UI;

public class UI_SetItem : UI_Base
{
    [SerializeField] private Image GradeImage;
    [SerializeField] private Image SubGradeImage;
    [SerializeField] private Image ItemImage;

    [SerializeField] private GameObject HaveEffectObj;

    public bool IsHave;

    public void Init(ItemType itemType, int itemId)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
            {
                var weaponChart = ChartManager.WeaponCharts[itemId];

                if (weaponChart.SubGrade != 0 && weaponChart.SubGrade < (int)Grade.Godgod)
                {
                    SubGradeImage.gameObject.SetActive(true);
                    SubGradeImage.sprite = Managers.Resource.LoadSubGradeIcon(weaponChart.SubGrade);
                }
                else
                    SubGradeImage.gameObject.SetActive(false);

                GradeImage.sprite = Managers.Resource.LoadItemGradeBg(weaponChart.Grade);
                ItemImage.sprite = Managers.Resource.LoadWeaponIcon(weaponChart.Icon);

                IsHave = Managers.Game.WeaponDatas[itemId].IsAcquired;
            }
                break;
            case ItemType.Pet:
            {
                var petChart = ChartManager.PetCharts[itemId];

                SubGradeImage.gameObject.SetActive(true);
                GradeImage.sprite = Managers.Resource.LoadItemGradeBg(petChart.Grade);
                SubGradeImage.sprite = Managers.Resource.LoadSubGradeIcon(petChart.SubGrade);
                ItemImage.sprite = Managers.Resource.LoadPetIcon(petChart.Icon);
                
                IsHave = Managers.Game.PetDatas[itemId].IsAcquired;
            }
                break;
            case ItemType.Costume:
            {
                var costumeChart = ChartManager.CostumeCharts[itemId];

                SubGradeImage.gameObject.SetActive(false);

                GradeImage.sprite = Managers.Resource.LoadItemGradeBg(costumeChart.Grade);
                ItemImage.sprite = Managers.Resource.LoadCostumeIcon(costumeChart.Icon);
                
                IsHave = Managers.Game.CostumeDatas[itemId].IsAcquired;
            }
                break;
        }
        
        HaveEffectObj.SetActive(IsHave);
    }
}