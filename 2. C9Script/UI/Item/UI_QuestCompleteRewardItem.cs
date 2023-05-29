using System.Linq;
using Chart;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestCompleteRewardItem : UI_Base
{
    [SerializeField] private Image RewardImage;
    [SerializeField] private TMP_Text CompleteValueText;
    [SerializeField] private Button CompleteButton;
    [SerializeField] private GameObject ClearObj;

    private readonly Color ClearRewardImageColor = new Color(30 / 255f, 30 / 255f, 30 / 255f);
    
    private QuestChart _questChart;
    private QuestData _questData;
    private QuestType _checkQuestType;

    public UI_QuestPopup UIQuestPopup;

    private void Start()
    {
        CompleteButton.BindEvent(OnClickComplete);
    }
    
    public void Init(int questId)
    {
        _questChart = ChartManager.QuestCharts[questId];
        _questData = Managers.Game.QuestDatas[questId];
        switch (_questChart.Type)
        {
            case QuestType.DailyComplete:
                _checkQuestType = QuestType.Daily;
                break;
            case QuestType.WeeklyComplete:
                _checkQuestType = QuestType.Weekly;
                break;
        }

        SetUI();
    }

    private void SetUI()
    {
        RewardImage.sprite = Managers.Resource.LoadItemIcon(_questChart.RewardType, _questChart.RewardId);
        CompleteValueText.text = _questChart.CompleteValue.ToString();
        
        if (_questData.IsReceiveReward)
        {
            ClearObj.SetActive(true);
            RewardImage.color = ClearRewardImageColor;
        }
        else
        {
            ClearObj.SetActive(false);
            RewardImage.color = Color.white;
        }
    }

    private void OnClickComplete()
    {
        Complete();
    }

    public void Complete()
    {
        if (_questData.IsReceiveReward)
            return;
        
        var completeQuestCount = Managers.Game.QuestDatas.Values.ToList()
            .FindAll(questData => ChartManager.QuestCharts[questData.Id].Type == _checkQuestType && questData.IsReceiveReward).Count;

        if (completeQuestCount < _questChart.CompleteValue)
            return;

        _questData.IsReceiveReward = true;
        Managers.Game.IncreaseItem(_questChart.RewardType, _questChart.RewardId, _questChart.RewardValue);
        RewardImage.color = ClearRewardImageColor;
        ClearObj.SetActive(true);
        
        UIQuestPopup.CompleteQuests[_questData.Id] = 1;

        if (UIQuestPopup.GainItems.ContainsKey(_questChart.RewardId))
            UIQuestPopup.GainItems[_questChart.RewardId] += _questChart.RewardValue;
        else
            UIQuestPopup.GainItems[_questChart.RewardId] = _questChart.RewardValue;
    }
}