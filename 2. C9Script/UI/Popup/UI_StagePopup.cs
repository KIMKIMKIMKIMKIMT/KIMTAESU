using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_StagePopup : UI_Popup
{
    [SerializeField] private TMP_Text WorldText;
    [SerializeField] private TMP_Text StageText;

    [SerializeField] private Image WorldImage;

    [SerializeField] private Button PrevWorldButton;
    [SerializeField] private Button NextWorldButton;
    [SerializeField] private Button PrevStageButton;
    [SerializeField] private Button Prev10StageButton;
    [SerializeField] private Button NextStageButton;
    [SerializeField] private Button Next10StageButton;
    [SerializeField] private Button MoveButton;
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button _stageInfoBtn;

    [SerializeField] private UI_StageRewardItem DropItem;
    [SerializeField] private UI_Panel _ui_stageInfoPanel;

    private int _world;
    private int _stage;

    private void Start()
    {
        PrevWorldButton.BindEvent(OnClickPrevWorld);
        PrevWorldButton.BindEvent(OnClickPrevWorld, UIEvent.Pressed);
        NextWorldButton.BindEvent(OnClickNextWorld);
        NextWorldButton.BindEvent(OnClickNextWorld, UIEvent.Pressed);
        PrevStageButton.BindEvent(OnClickPrevStage);
        PrevStageButton.BindEvent(OnClickPrevStage, UIEvent.Pressed);
        Prev10StageButton.BindEvent(OnClickPrev10Stage);
        Prev10StageButton.BindEvent(OnClickPrev10Stage, UIEvent.Pressed);
        NextStageButton.BindEvent(OnClickNextStage);
        NextStageButton.BindEvent(OnClickNextStage, UIEvent.Pressed);
        Next10StageButton.BindEvent(OnClickNext10Stage);
        Next10StageButton.BindEvent(OnClickNext10Stage, UIEvent.Pressed);
        MoveButton.BindEvent(OnClickMove);
        CloseButton.BindEvent(ClosePopup);
        _stageInfoBtn.BindEvent(() => _ui_stageInfoPanel.Open());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Managers.UI.FindPopup<UI_YesNoPopup>() != null)
                return;

            if (_ui_stageInfoPanel.gameObject.activeInHierarchy)
            {
                _ui_stageInfoPanel.gameObject.SetActive(false);
                return;
            }

            Managers.UI.ClosePopupUI();
        }
    }

    public override void Open()
    {
        base.Open();

        _world = ChartManager.StageDataController.StageDataTable[Managers.Stage.StageId.Value].WorldIndex;
        _stage = Managers.Stage.StageId.Value;

        SetUI();
    }

    private void SetUI()
    {
        var stageChart = ChartManager.StageDataController.StageDataTable[_stage];

        _world = stageChart.WorldIndex;
        
        var worldChart = ChartManager.WorldCharts[_world];
        var maxStageChart = ChartManager.StageDataController.StageDataTable[Managers.Game.UserData.MaxReachStage];

        WorldImage.sprite = Managers.Resource.LoadBg(_world);
        WorldText.text = ChartManager.GetString(worldChart.BgDesc);

        PrevWorldButton.gameObject.SetActive(CheckPrevWorld());
        NextWorldButton.gameObject.SetActive(CheckNextWorld());
        
        StageText.text = $"Stage - {_stage}";

        PrevStageButton.gameObject.SetActive(ChartManager.StageDataController.StageDataTable.ContainsKey(_stage - 1));
        Prev10StageButton.gameObject.SetActive(ChartManager.StageDataController.StageDataTable.ContainsKey(_stage - 10));

        NextStageButton.gameObject.SetActive(ChartManager.StageDataController.StageDataTable.ContainsKey(_stage + 1) &&
                                             _stage < Managers.Game.UserData.MaxReachStage);
        Next10StageButton.gameObject.SetActive(ChartManager.StageDataController.StageDataTable.ContainsKey(_stage + 10) &&
                                               _stage < Managers.Game.UserData.MaxReachStage);
        
        MoveButton.gameObject.SetActive(_stage != Managers.Stage.StageId.Value);
        
        if (stageChart.DropItemId == 0)
            DropItem.gameObject.SetActive(false);
        else
        {
            DropItem.gameObject.SetActive(true);
            DropItem.Init(ItemType.Goods, stageChart.DropItemId);
        }
    }

    private void OnClickPrevWorld()
    {
        for (int i = _stage; i >= 1; i--)
        {
            if (ChartManager.StageDataController.StageDataTable[i].WorldIndex != _world)
            {
                _world = ChartManager.StageDataController.StageDataTable[i].WorldIndex;
                _stage = i;
                break;
            }
        }
        
        SetUI();
    }

    private void OnClickNextWorld()
    {
        for (int i = _stage; i <= Managers.Game.UserData.MaxReachStage; i++)
        {
            if (ChartManager.StageDataController.StageDataTable[i].WorldIndex != _world)
            {
                _world = ChartManager.StageDataController.StageDataTable[i].WorldIndex;
                _stage = i;
                break;
            }
        }
        
        SetUI();
    }

    private void OnClickPrevStage()
    {
        if (!ChartManager.StageDataController.StageDataTable.ContainsKey(_stage - 1))
            return;

        _stage -= 1;
        SetUI();
    }

    private void OnClickPrev10Stage()
    {
        if (!ChartManager.StageDataController.StageDataTable.ContainsKey(_stage - 10))
            return;

        _stage -= 10;
        SetUI();
    }

    private void OnClickNextStage()
    {
        if (!ChartManager.StageDataController.StageDataTable.ContainsKey(_stage + 1))
            return;

        if (_stage + 1 > Managers.Game.UserData.MaxReachStage)
            return;

        _stage += 1;
        SetUI();
    }

    private void OnClickNext10Stage()
    {
        if (!ChartManager.StageDataController.StageDataTable.ContainsKey(_stage + 10))
            return;

        if (_stage + 10 > Managers.Game.UserData.MaxReachStage)
            return;

        _stage += 10;
        SetUI();
    }

    private void OnClickMove()
    {
        Managers.Game.MainPlayer.State.Value = CharacterState.None;
        
        FadeScreen.FadeOut(() =>
        {
            Managers.UI.CloseAllPopupUI();
            Managers.Stage.State.Value = StageState.Normal;
            Managers.Stage.KillCount.Value = 0;
            Managers.Stage.StageId.Value = _stage;
            Managers.Game.MainPlayer.transform.position = Vector3.zero;
            
            Managers.Game.MainPlayer.SetAllSkillCoolTime();
            
            FadeScreen.FadeIn(() =>
            {
                Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
            }, 0.5f);
        }, 0f);
    }

    private bool CheckPrevWorld()
    {
        for (int stage = _stage; stage > 0; stage--)
        {
            if (ChartManager.StageDataController.StageDataTable[stage].WorldIndex != _world)
                return true;
        }

        return false;
    }

    private bool CheckNextWorld()
    {
        for (int stage = _stage; stage <= Managers.Game.UserData.MaxReachStage; stage++)
        {
            if (ChartManager.StageDataController.StageDataTable[stage].WorldIndex != _world)
                return true;
        }

        return false;
    }

    private void OnClickStageInfo()
    {
        
    }
}