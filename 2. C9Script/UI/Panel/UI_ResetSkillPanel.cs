using BackEnd;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResetSkillPanel : UI_Panel
{
    [SerializeField] private TMP_Text CostValueText;
    [SerializeField] private GameObject BackgroundObj;
    [SerializeField] private Button ResetButton;

    private void Start()
    {
        CostValueText.text = ChartManager.SystemCharts[SystemData.SkillResetCost].Value.ToCurrencyString();
        
        BackgroundObj.BindEvent(Close);
        ResetButton.BindEvent(OnClickReset);
    }

    private void OnClickReset()
    {
        var skillResetCost = ChartManager.SystemCharts[SystemData.SkillResetCost].Value;

        if (!Utils.IsEnoughItem(ItemType.Goods, (int)Goods.StarBalloon, skillResetCost))
        {
            Managers.Message.ShowMessage("재화가 부족합니다.");
            return;
        }

        double totalPrice = 0;
        
        foreach (var skillData in Managers.Game.SkillDatas.Values)
        {
            if (skillData.Level <= 1)
                continue;
            
            if (!ChartManager.SkillCharts.TryGetValue(skillData.Id, out var skillChart))
                continue;

            for (int skillLv = 1; skillLv < skillData.Level; skillLv++)
                totalPrice += Utils.CalculateItemValue(skillChart.LevelUpItemValue, skillChart.LevelUpItemIncreaseValue,
                    skillLv);

            skillData.Level = 1;
        }

        Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.StarBalloon, skillResetCost);
        Managers.Game.IncreaseItem(ItemType.Goods, (int)Goods.SkillEnhancementStone, totalPrice);
        
        GameDataManager.GoodsGameData.SaveGameData();
        GameDataManager.SkillGameData.SaveGameData();

        var param = new Param();
        
        param.Add("Return SkillEnhancementStone", totalPrice);
        Utils.GetGoodsLog(ref param);

        Backend.GameLog.InsertLog("ResetSkill", param);
        
        Managers.Game.CalculateStat();
        
        Managers.Message.ShowMessage("초기화 완료");
        
        Close();
    }
}