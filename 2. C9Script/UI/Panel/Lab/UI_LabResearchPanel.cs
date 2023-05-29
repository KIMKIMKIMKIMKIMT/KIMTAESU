using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class UI_LabResearchPanel : UI_Panel
{
    [SerializeField] private Transform UISkillResearchItemRoot;

    private readonly List<UI_LabResearchItem> _uiSkillResearchItems = new();

    private void Awake()
    {
        SetItems();
    }

    public override void Open()
    {
        base.Open();
        UpdateItems();
    }

    private void SetItems()
    {
        _uiSkillResearchItems.Clear();
        UISkillResearchItemRoot.DestroyInChildren();

        foreach (var labResearchData in Managers.Game.LabResearchDatas.Values)
        {
            var uiLabResearchItem = Managers.UI.MakeSubItem<UI_LabResearchItem>(UISkillResearchItemRoot);
            uiLabResearchItem.name += $"_{_uiSkillResearchItems.Count}";
            _uiSkillResearchItems.Add(uiLabResearchItem);

            uiLabResearchItem.gameObject.SetActive(true);
            uiLabResearchItem.Init(labResearchData);
        }
    }

    private void UpdateItems()
    {
        _uiSkillResearchItems.ForEach(uiSkillResearchItem =>
        {
            if (uiSkillResearchItem == null)
                return;

            uiSkillResearchItem.RefreshUI();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI();
        }
    }
}