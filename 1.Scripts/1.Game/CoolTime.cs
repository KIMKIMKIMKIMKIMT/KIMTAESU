using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolTime : BuffSkill
{
    public override void Upgrade()
    {
        if (Level < 5)
        {
            Level++;
            switch (Level)
            {
                case 1:
                    BuffSkillController.Buff_CoolTime = 0.9f;
                    for (int i = 0; i < BattleMgr.Instance.Player.SkillController.CurrentBuffSkillTable.Count; i++)
                    {
                        BattleMgr.Instance.Player.SkillController.CurrentAttackSkillTable[BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList[i]].SetCoolTime();
                    }
                    break;
                case 2:
                    BuffSkillController.Buff_CoolTime = 0.85f;
                    for (int i = 0; i < BattleMgr.Instance.Player.SkillController.CurrentBuffSkillTable.Count; i++)
                    {
                        BattleMgr.Instance.Player.SkillController.CurrentAttackSkillTable[BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList[i]].SetCoolTime();
                    }
                    break;
                case 3:
                    BuffSkillController.Buff_CoolTime = 0.8f;
                    for (int i = 0; i < BattleMgr.Instance.Player.SkillController.CurrentBuffSkillTable.Count; i++)
                    {
                        BattleMgr.Instance.Player.SkillController.CurrentAttackSkillTable[BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList[i]].SetCoolTime();
                    }
                    break;
                case 4:
                    BuffSkillController.Buff_CoolTime = 0.75f;
                    for (int i = 0; i < BattleMgr.Instance.Player.SkillController.CurrentBuffSkillTable.Count; i++)
                    {
                        BattleMgr.Instance.Player.SkillController.CurrentAttackSkillTable[BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList[i]].SetCoolTime();
                    }
                    break;
                case 5:
                    BuffSkillController.Buff_CoolTime = 0.7f;
                    for (int i = 0; i < BattleMgr.Instance.Player.SkillController.CurrentBuffSkillTable.Count; i++)
                    {
                        BattleMgr.Instance.Player.SkillController.CurrentAttackSkillTable[BattleMgr.Instance.Player.SkillController.CurrentAttackSkillList[i]].SetCoolTime();
                    }
                    break;
            }
        }
        
    }

    protected override void SetSkillData(BuffSkillData skillData)
    {
        
    }
}
