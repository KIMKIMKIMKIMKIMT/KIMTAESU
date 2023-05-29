using System;

[Serializable]
public abstract class BaseLog
{
    
}

[Serializable]
public class ResetStatLog : BaseLog
{
    //public 
}

[Serializable]
public class AwakenCostumeLog : BaseLog
{
    public string CostumeId;
    public string AwakenLv;
    public string UseCount;
    
    public AwakenCostumeLog(string costumeId, string awaken, string useCount)
    {
        CostumeId = costumeId;
        AwakenLv = awaken;
        UseCount = useCount;
    }
}

[Serializable]
public class CombineWeaponLog : BaseLog
{
    public int CombineWeaponId;
    public int CombineCount;
    public int GainWeaponId;
    public int GainCount;

    public CombineWeaponLog(int combineWeaponId, int combineCount, int gainWeaponId, int gainCount)
    {
        CombineWeaponId = combineWeaponId;
        CombineCount = combineCount;
        GainWeaponId = gainWeaponId;
        GainCount = gainCount;
    }
}

[Serializable]
public class CombinePetLog : BaseLog
{
    public int CombinePetId;
    public int CombineCount;
    public int GainPetId;
    public int GainCount;

    public CombinePetLog(int combinePetId, int combineCount, int gainPetId, int gainCount)
    {
        CombinePetId = combinePetId;
        CombineCount = combineCount;
        GainPetId = gainPetId;
        GainCount = gainCount;
    }
}

[Serializable]
public class ReinforcePetLog : BaseLog
{
    public int IncreaseLv;
    public double UsingGoldBar;
}

[Serializable]
public class UpgradeSkillLog : BaseLog
{
    public int IncreaseLv;
    public double UsingSkillGem;
}

[Serializable]
public class UpgradeWoodLog : BaseLog
{
    public int IncreaseLv;
    public double UsingGoldbar;
}

[Serializable]
public class UpgradeWoodAwakeningLog : BaseLog
{
    public int StatId;
    public Grade Grade;
    public double UsingGoods;
}

[Serializable]
public class DungeonSkipLog : BaseLog
{
    public int SkipCount;
    public ItemType ItemType;
    public int ItemId;
    public double GainValue;
}

[Serializable]
public class RaidSkipLog : BaseLog
{
    public int SkipCount;
    public double GainGoldValue;
    public double GainGoldBarValue;
    public double GainSkillStoneValue;
}

[Serializable]
public class ReceiveDpsRewardLog
{
    public int RewardId;
    public double GainCount;
}