using System;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;

namespace Chart
{
    public class SkillChart : IChart<int>
    {
        public ObscuredInt Id;
        public ObscuredString Name;
        public SkillTabType TabType;
        public LabSkillType LabSkillType;
        public ObscuredString Desc;
        public Grade Grade;
        public ObscuredFloat Value;
        public ObscuredFloat IncreaseValue;
        public ObscuredFloat CoolTime;
        public ItemType UnlockItemType;
        public ObscuredInt UnlockItemId;
        public ObscuredInt UnlockItemValue;
        public ItemType LevelUpItemType;
        public ObscuredInt LevelUpItemId;
        public ObscuredInt LevelUpItemValue;
        public ObscuredFloat LevelUpItemIncreaseValue;
        public ObscuredInt MaxLevel;
        public ObscuredString Icon;
        
        public int GetID()
        {
            return Id;
        }

        public void SetData(JsonData jsonData)
        {
            int.TryParse(jsonData["Skill_Id"].ToString(), out int id);
            Id = id;
            
            Name = jsonData["Skill_Name"].ToString();
            
            Enum.TryParse(jsonData["Skill_Type"].ToString(), out TabType);
            
            Enum.TryParse(jsonData["Skill_Lab_Type"].ToString(), out LabSkillType);
            
            Desc = jsonData["Skill_Desc"].ToString();
            
            Enum.TryParse(jsonData["Skill_Grade"].ToString(), out Grade);
            
            float.TryParse(jsonData["Skill_Value"].ToString(), out float value);
            Value = value;
            
            float.TryParse(jsonData["Skill_Increase_Value"].ToString(), out float increaseValue);
            IncreaseValue = increaseValue;
            
            float.TryParse(jsonData["Skill_Cooltime"].ToString(), out float coolTime);
            CoolTime = coolTime;
            
            Enum.TryParse(jsonData["Unlock_Item_Type"].ToString(), out UnlockItemType);
            
            int.TryParse(jsonData["Unlock_Item_Id"].ToString(), out int unlockItemId);
            UnlockItemId = unlockItemId;
            
            int.TryParse(jsonData["Unlock_Item_Value"].ToString(), out int unlockItemValue);
            UnlockItemValue = unlockItemValue;
            
            Icon = jsonData["Icon"].ToString();
            
            Enum.TryParse(jsonData["Level_Up_Item_Type"].ToString(), out LevelUpItemType);
            
            int.TryParse(jsonData["Level_Up_Item_Id"].ToString(), out int levelUpItemId);
            LevelUpItemId = levelUpItemId;
            
            int.TryParse(jsonData["Level_Up_Item_Value"].ToString(), out int levelUpItemValue);
            LevelUpItemValue = levelUpItemValue;
            
            float.TryParse(jsonData["Level_Up_Item_Increase_Value"].ToString(), out float levelUpItemIncreaseValue);
            LevelUpItemIncreaseValue = levelUpItemIncreaseValue;
            
            int.TryParse(jsonData["Max_Level"].ToString(), out int maxLevel);
            MaxLevel = maxLevel;

            ChartManager.SkillCharts[GetID()] = this;
        }
    }
}