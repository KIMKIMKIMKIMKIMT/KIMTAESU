using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpeed : BuffSkill
{
    public override void Upgrade()
    {
        if (Level < 5)
        {
            Level++;
            switch (Level)
            {
                case 1:
                    BuffSkillController.Buff_BulletSpeed = 1.1f;
                    break;
                case 2:
                    BuffSkillController.Buff_BulletSpeed = 1.2f;
                    break;
                case 3:
                    BuffSkillController.Buff_BulletSpeed = 1.3f;
                    break;
                case 4:
                    BuffSkillController.Buff_BulletSpeed = 1.4f;
                    break;
                case 5:
                    BuffSkillController.Buff_BulletSpeed = 1.5f;
                    break;

            }
        }
        
    }

    protected override void SetSkillData(BuffSkillData skillData)
    {
        
    }
}
