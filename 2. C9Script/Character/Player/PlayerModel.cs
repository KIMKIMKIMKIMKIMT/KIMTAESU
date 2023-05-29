using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private SpriteLibraryAsset SpriteLibraryAsset;
    [SerializeField] private SpriteResolver[] CharacterSpriteResolvers;
    [SerializeField] private SpriteResolver WeaponSpriteResolver;
    [SerializeField] private Animator Animator;
    
    [SerializeField] private GameObject CostumeEffectObj;
    [SerializeField] private GameObject CostumeLegenoEffectObj;

    public void SetModel(bool isAnimation = false)
    {
        Animator.enabled = isAnimation;
    }

    public void SetCostume(int costumeId)
    {
        if (ChartManager.CostumeCharts.TryGetValue(costumeId, out var costumeChart))
        {
            CostumeEffectObj.SetActive(costumeChart.Grade == Grade.Legend);
            CostumeLegenoEffectObj.SetActive(costumeChart.Grade == Grade.Legeno);
        }
        else
        {
            CostumeEffectObj.SetActive(costumeId == 5006 || costumeId == 5007 || costumeId == 5008);
            CostumeLegenoEffectObj.SetActive(costumeId == 5009 || costumeId == 5010 || costumeId == 5011 || costumeId == 5012 || costumeId == 5013 || costumeId == 5014);
        }

        foreach (var spriteResolver in CharacterSpriteResolvers)
        {
            if (!SpriteLibraryAsset.GetCategoryLabelNames(spriteResolver.GetCategory())
                    .Contains(costumeId.ToString()))
            {
                if (!SpriteLibraryAsset.GetCategoryLabelNames(spriteResolver.name)
                    .Contains(costumeId.ToString()))
                    continue;
            }
                
            
            spriteResolver.SetCategoryAndLabel(spriteResolver.GetCategory() != null ? spriteResolver.GetCategory() : spriteResolver.name, costumeId.ToString());
        }
    }

    public void SetWeapon(int weaponId)
    {
        WeaponSpriteResolver.SetCategoryAndLabel(WeaponSpriteResolver.GetCategory(), weaponId.ToString());
    }

    public void SetAnimation(string animationName)
    {
        Animator.SetTrigger(animationName);
    }
}