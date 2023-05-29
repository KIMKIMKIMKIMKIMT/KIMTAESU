using System;
using System.Collections;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_SummonItem : UI_Base
{
    [SerializeField] private Image ItemImage;
    [SerializeField] private Image GradeImage;

    [SerializeField] private Animator RareAnimator;
    [SerializeField] private Animator UniqueAnimator;
    [SerializeField] private Animator LegendAnimator;
    
    public void Init(SummonType summonType, int id)
    {
        RareAnimator.gameObject.SetActive(false);
        UniqueAnimator.gameObject.SetActive(false);
        LegendAnimator.gameObject.SetActive(false);
        
        Sprite sprite;
        Grade? grade = null;

        switch (summonType)
        {
            case SummonType.Weapon:
            {
                var weaponChart = ChartManager.WeaponCharts[id];
                sprite = Managers.Resource.LoadWeaponIcon(weaponChart.Icon);
                grade = weaponChart.Grade;
            }
                break;
            case SummonType.Pet:
            {
                var petChart = ChartManager.PetCharts[id];
                sprite = Managers.Resource.LoadPetIcon(petChart.Icon);
                grade = petChart.Grade;
            }
                break;
            case SummonType.Collection:
            {
                var collectionChart = ChartManager.CollectionCharts[id];
                sprite = Managers.Resource.LoadCollectionIcon(collectionChart.Icon);
            }
                break;
            default:
                return;
        }

        ItemImage.sprite = sprite;

        if (grade.HasValue)
        {
            GradeImage.gameObject.SetActive(true); 
            GradeImage.sprite = Managers.Resource.LoadItemGradeBg(grade.Value);
        }
        else
            GradeImage.gameObject.SetActive(false); 
        
        gameObject.SetActive(true);

        StartCoroutine(CoGradeEffect(grade));
    }

    private IEnumerator CoGradeEffect(Grade? grade)
    {
        if (!grade.HasValue)
            yield break;
        
        if (grade >= Grade.Rare)
        {
            RareAnimator.gameObject.SetActive(true);
            yield return null;

            if (RareAnimator.GetCurrentAnimatorClipInfo(0).Length <= 0)
            {
                RareAnimator.gameObject.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(RareAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
                RareAnimator.gameObject.SetActive(false);
            }
        }

        if (grade >= Grade.Unique)
        {
            UniqueAnimator.gameObject.SetActive(true);
            yield return null;

            if (UniqueAnimator.GetCurrentAnimatorClipInfo(0).Length <= 0)
            {
                UniqueAnimator.gameObject.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(UniqueAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
                UniqueAnimator.gameObject.SetActive(false);
            }
        }
        
        if (grade >= Grade.Legend)
        {
            LegendAnimator.gameObject.SetActive(true);
            yield return null;

            if (LegendAnimator.GetCurrentAnimatorClipInfo(0).Length <= 0)
            {
                LegendAnimator.gameObject.SetActive(false);    
            }
            else
            {
                yield return new WaitForSeconds(LegendAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
                LegendAnimator.gameObject.SetActive(false);
            }
        }
    }
}