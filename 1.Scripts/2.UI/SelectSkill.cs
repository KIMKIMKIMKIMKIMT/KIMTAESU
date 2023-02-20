using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSkill : MonoBehaviour
{
    #region Fields
    private AttackSkillData _atkData;
    private BuffSkillData _buffData;
    [SerializeField] private Image _skillSprite;
    [SerializeField] private Text _nameTxt;
    [SerializeField] private Text _descriptionTxt;
    [SerializeField] private Text _levelTxt;

    private eSKILL_TYPE eSKILL_TYPE;
    #endregion

    #region Public Methods
    public void SetSkillData(AttackSkillData atkSkillData)
    {
        _atkData = atkSkillData;
        _skillSprite.sprite = SpriteMgr.Instance._atkSkillkImg[_atkData.Index];
        _nameTxt.text = _atkData.Name.ToString();
        _descriptionTxt.text = _atkData.Descriptions[BattleMgr.Instance.Player.SkillController._atkSkills[_atkData.Index].Level];
        int level = BattleMgr.Instance.Player.SkillController._atkSkills[_atkData.Index].Level;
        _levelTxt.text = "다음 레벨 : " + (level + 1);
        eSKILL_TYPE = eSKILL_TYPE.Attack;
    }

    public void SetSkillData(BuffSkillData buffSKillData)
    {
        _buffData = buffSKillData;
        _skillSprite.sprite = SpriteMgr.Instance._buffSkillkImg[_buffData.Index];
        _nameTxt.text = _buffData.Name.ToString();
        _descriptionTxt.text = _buffData.Descriptions[BattleMgr.Instance.Player.SkillController._buffSkills[_buffData.Index].Level];
        int level = BattleMgr.Instance.Player.SkillController._buffSkills[_buffData.Index].Level;
        _levelTxt.text = "다음 레벨 : " + (level + 1);
        eSKILL_TYPE = eSKILL_TYPE.Buff;
    }

    public void OnClickSelect()
    {
        if (PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            SoundMgr.Instance.PlaySFXSound(eAUDIOCLIP_SFX.SkillSelect);
        }
        switch (eSKILL_TYPE)
        {
            case eSKILL_TYPE.Attack:
                BattleMgr.Instance.Player.SkillController.GetSKill(_atkData.Key);
                break;
            case eSKILL_TYPE.Buff:
                BattleMgr.Instance.Player.SkillController.GetSkill(_buffData.Key);
                break;
        }
        InGameUIMgr.Instance._gameUI.SetSkillSelection(false);
    }
    #endregion
}
