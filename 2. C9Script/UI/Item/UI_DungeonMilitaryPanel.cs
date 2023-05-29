using System.Collections.Generic;
using UI;
using UnityEngine;

public class UI_DungeonMilitaryPanel : UI_Panel
{
    [SerializeField] private Transform ContentTr;
    [SerializeField] private UI_DungeonSkipPanel DungeonSkipPanel;
    
    private readonly List<UI_DungeonItem> _uiDungeonItems = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (DungeonSkipPanel.gameObject.activeSelf)
            {
                DungeonSkipPanel.Close();
                return;
            }
            
            Managers.UI.ClosePopupUI();
        }
    }

    public override void Open()
    {
        base.Open();
        
        if (_uiDungeonItems.Count <= 0)
            MakeDungeonItems();
        else
            _uiDungeonItems.ForEach(uiDungeonItem => uiDungeonItem.Open());
    }
    
    private void MakeDungeonItems()
    {
        ContentTr.DestroyInChildren();

        for (int i = (int)DungeonType.Hwasengbang; i <= (int)DungeonType.March; i++)
        {
            Debug.Log("UI_DungeonItem Create");
            var item = Managers.UI.MakeSubItem<UI_DungeonItem>(ContentTr);
            if (item == null)
                continue;

            Debug.Log("UI_DungeonItem Set ID");
            item.SetItem(i, OnSkipCallback);
            _uiDungeonItems.Add(item);
        }
        
        _uiDungeonItems.ForEach(uiDungeonItem =>
        {
            uiDungeonItem.Open();
        });
    }
    
    private void OnSkipCallback(int dungeonId)
    {
        if (Managers.Game.UserData.GetDungeonClearStep(dungeonId) <= 0 ||
            Managers.Game.UserData.GetDungeonHighestValue(dungeonId) <= 0)
        {
            Managers.Message.ShowMessage("해당 컨텐츠 플레이 전적이 없는 상태에서는 스킵이 불가능합니다!");
            return;
        }
        
        DungeonSkipPanel.Open();
        DungeonSkipPanel.SetPanel(dungeonId);
    }
}