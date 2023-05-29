using System.Collections.Generic;

public class StatManager
{
    // 연산 공식
    // (BaseStat + UpgradeStat + EquipStat) * (HaveWeaponStat * HavePassiveSKill)
    
    public Dictionary<int, float> BaseStat = new();
    public Dictionary<int, float> UpgradeStat = new();
    public Dictionary<int, float> EquipStat = new();
    public Dictionary<int, float> HaveWeaponStat = new();
    public Dictionary<int, float> HavePassiveSkillStat = new();

    public Dictionary<int, float> StatDatas = new();
    
    //public 

    public void CalculateTotalStat()
    {
        
    }

    // 기본스탯, 캐릭터 레벨에 따른 스탯
    public void CalculateBaseStat()
    {
        
    }

    // 아이템(골드)로 강화한 스탯
    public void CalculateUpgradeStat()
    {
        
    }

    // 장착 아이템 스탯
    public void CalculateEquipStat()
    {
        
    }
    
    // 보유 무기 스탯
    public void CalculateHaveWeaponStat()
    {
        
    }

    // 보유 패시브 스킬
    public void CalculateHavePassiveSkillStat()
    {
        
    }
}