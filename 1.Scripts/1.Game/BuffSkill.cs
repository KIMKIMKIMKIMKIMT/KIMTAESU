using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffSkill : Skill
{
    #region Fields
    protected BuffSkillData _skillData;
    #endregion

    #region Protected Methods
    protected abstract void SetSkillData(BuffSkillData skillData);
    #endregion
}
