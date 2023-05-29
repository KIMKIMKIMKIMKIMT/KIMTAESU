using System.Collections.Generic;
using System.Data;
using GameData;
using UniRx;
using UnityEditor;
using UnityEngine;

public partial class Player
{
    public Dictionary<int, ReactiveProperty<float>> SkillCoolTimes = new();

    private readonly Dictionary<int, BaseSkill> _skills = new();

    public List<SkillInfo> SkillInfos = new()
    {
        new SkillInfo(),
        new SkillInfo(),
        new SkillInfo(),
        new SkillInfo(),
        new SkillInfo()
    };

    // 현재 사용, 적용중인 스킬
    public readonly List<int> UsingSkillIds = new();

    public void AllStopSkill()
    {
        foreach (var skill in _skills)
        {
            skill.Value.StopSkill();
        }
    }

    public void StartSkill(int slotIndex, bool isAuto = true)
    {
        if (State.Value == CharacterState.None)
            return;
        
        int id = Managers.Game.EquipSkillList[Managers.Game.SkillQuickSlotIndex.Value][slotIndex].Value;

        if (id <= 0)
            return;

        if (!ChartManager.SkillCharts.ContainsKey(id))
            return;

        if (SkillInfos[slotIndex].IsCoolTime.Value)
            return;

        if (State.Value == CharacterState.RaidPortalMove)
            return;
        
        if (UsingSkillIds.Contains(id))
            return;
        
        if (IsPvp)
            return;

        if (Utils.IsWorldCupDungeon())
            return;

        if (Utils.IsGuildAllRaidDungeon())
            return;

        if (isAuto && TargetMonster == null)
            return;
        
        if (IsSkillAnimation(SkillInfos[slotIndex].SkillIndex))
        {
            if (IsSkillAnimationUsing())
            {
                return;
            }
        }

        if (Managers.Stage.State.Value == StageState.Promo && !Managers.Dungeon.StartPromoBattle)
            return;

        if (Managers.Stage.State.Value == StageState.GuildAllRaid && Managers.AllRaid.IsAllRaidSkillLock)
            return;

        if (Managers.Stage.State.Value == StageState.GuildSports)
            return;

        if (!_skills.TryGetValue(id, out var skill))
        {
            string skillPath = $"Skill/Skill_{id}";
            GameObject obj = Managers.Resource.Instantiate(skillPath);
            if (obj == null)
            {
                Debug.LogError($"Can't Find Skill Prefab!! : {skillPath}");
                return;
            }

            skill = obj.GetComponent<BaseSkill>();
            if (skill == null)
            {
                Debug.LogError($"Can't Fiend Skill Component!! : {skillPath}");
                return;
            }

            _skills.Add(id, skill);
        }

        skill.StartSkill();

        var coolTime = (float)(ChartManager.SkillCharts[id].CoolTime - ChartManager.SkillCharts[id].CoolTime * Managers.Game.BaseStatDatas[(int)StatType.SkillCoolTimeReduce]);

        if (SkillCoolTimes.ContainsKey(id))
            SkillCoolTimes[id].Value = coolTime;
        else
            SkillCoolTimes.Add(id, new ReactiveProperty<float>(coolTime));
    }
    
    public void StartSkillBySkillIndex(int id)
    {
        if (State.Value == CharacterState.None)
            return;
        
        if (id <= 0)
            return;

        if (!ChartManager.SkillCharts.ContainsKey(id))
            return;

        if (!_skills.TryGetValue(id, out var skill))
        {
            string skillPath = $"Skill/Skill_{id}";
            GameObject obj = Managers.Resource.Instantiate(skillPath);
            if (obj == null)
            {
                Debug.LogError($"Can't Find Skill Prefab!! : {skillPath}");
                return;
            }

            skill = obj.GetComponent<BaseSkill>();
            if (skill == null)
            {
                Debug.LogError($"Can't Fiend Skill Component!! : {skillPath}");
                return;
            }

            _skills.Add(id, skill);
        }

        skill.Id = id;

        skill.StartSkill();
    }

    private void StartAutoSkill(int slotIndex)
    {
        if (UsingSkillIds.Contains(SkillInfos[slotIndex].SkillIndex))
            return;

        if (IsSkillAnimation(SkillInfos[slotIndex].SkillIndex))
        {
            if (IsSkillAnimationUsing())
            {
                return;
            }
        }

        StartSkill(slotIndex);
    }
} 