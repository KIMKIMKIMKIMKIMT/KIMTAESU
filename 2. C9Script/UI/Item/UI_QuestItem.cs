using System;
using Chart;
using GameData;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestItem : UI_Base
{
    [SerializeField] private TMP_Text QuestText;
    [SerializeField] private TMP_Text QuestValueText;
    [SerializeField] private TMP_Text RewardValueText;
    [SerializeField] private TMP_Text CompleteButtonText;

    [SerializeField] private Image RewardImage;
    
    [SerializeField] private Sprite ProgressButtonSprite;
    [SerializeField] private Sprite ReceiveButtonSprite;
    [SerializeField] private Sprite CompleteButtonSprite;

    [SerializeField] private Button CompleteButton;
    
    [SerializeField] private Slider QuestProgressSlider;
    
    private QuestChart _questChart;
    private QuestData _questData;

    public QuestData QuestData => _questData;

    private CompositeDisposable _compositeDisposable = new();

    private Action<bool> _onCompleteCallback;
    private Action _onReceiveCallback;

    public UI_QuestPopup UIQuestPopup;
    //private bool isSaveFlag;

    private void Start()
    {
        CompleteButton.BindEvent(OnClickReceive);
    }
    
    public void Init(int questId, Action<bool> onCompleteCallback, Action onReceiveCallback)
    {
        _questChart = ChartManager.QuestCharts[questId];
        _questData = Managers.Game.QuestDatas[questId];
        _onCompleteCallback = onCompleteCallback;
        _onReceiveCallback = onReceiveCallback;
        
        _compositeDisposable.Clear();
        
        SetUI();
    }

    private void SetUI()
    {
        QuestText.text = ChartManager.GetString(_questChart.Name);
        
        RewardImage.sprite = Managers.Resource.LoadItemIcon(_questChart.RewardType, _questChart.RewardId);

        RefreshUI();
        
        _questData.OnChangeProgressValue.Subscribe(_ =>
        {
            RefreshUI();
        }).AddTo(_compositeDisposable);
    }

    private void OnDisable()
    {
        _compositeDisposable.Clear();
    }

    private void RefreshUI()
    {
        QuestValueText.text = $"{_questData.ProgressValue}/{_questChart.CompleteValue}";
        QuestProgressSlider.value = _questData.ProgressValue / (float)_questChart.CompleteValue;
        
        RewardValueText.text = 
            _questChart.Type == QuestType.Repeat ? 
                (_questChart.RewardValue * Math.Max(1, _questData.ProgressValue / _questChart.CompleteValue)).ToCurrencyString() : 
                _questChart.RewardValue.ToCurrencyString();
        
        // 이미 보상을 받고 완료한 퀘스트
        if (_questData.IsReceiveReward)
        {
            CompleteButton.image.sprite = CompleteButtonSprite;
            CompleteButtonText.text = "완료";
        }
        else
        {
            // 완료가 가능한 퀘스트
            if (_questData.IsComplete)
            {
                CompleteButton.image.sprite = ReceiveButtonSprite;
                CompleteButtonText.text = "보상받기";
                _onCompleteCallback?.Invoke(true);
            }
            else
            {
                CompleteButton.image.sprite = ProgressButtonSprite;
                CompleteButtonText.text = "진행중";
            }
        }
    }

    public void OnClickReceive()
    {
        if (_questData.IsReceiveReward)
            return;

        if (!_questData.IsComplete)
            return;

        if (_questChart.Type == QuestType.Repeat)
        {
            long completeCount = _questData.ProgressValue / _questChart.CompleteValue;
            long progressValue = _questData.ProgressValue % _questChart.CompleteValue;
            
            Managers.Game.IncreaseItem(_questChart.RewardType, _questChart.RewardId, _questChart.RewardValue * completeCount);
            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.QuestComplete, _questChart.Id, completeCount));

            _questData.ProgressValue = progressValue;

            if (UIQuestPopup.CompleteQuests.ContainsKey(_questChart.Id))
                UIQuestPopup.CompleteQuests[_questChart.Id] += completeCount;
            else
                UIQuestPopup.CompleteQuests[_questChart.Id] = completeCount;

            if (UIQuestPopup.GainItems.ContainsKey(_questChart.RewardId))
                UIQuestPopup.GainItems[_questChart.RewardId] += _questChart.RewardValue * completeCount;
            else
                UIQuestPopup.GainItems[_questChart.RewardId] = _questChart.RewardValue * completeCount;
        }
        else
        {
            _questData.IsReceiveReward = true;
            Managers.Game.IncreaseItem(_questChart.RewardType, _questChart.RewardId, _questChart.RewardValue);
            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.QuestComplete, _questChart.Id, 1));
            
            UIQuestPopup.CompleteQuests[_questChart.Id] = 1;
            UIQuestPopup.GainItems[_questChart.RewardId] = _questChart.RewardValue;
        }

        _onReceiveCallback?.Invoke();

        RefreshUI();
    }

    public void AllComplete()
    {
        if (_questChart.Type != QuestType.Repeat)
            OnClickReceive();
        else
        {
            if (_questData.IsReceiveReward)
                return;

            if (!_questData.IsComplete)
                return;

            //isSaveFlag = true;

            int completeCount = 0;

            var progressValue = _questData.ProgressValue;

            while (progressValue >= _questChart.CompleteValue)
            {
                progressValue -= _questChart.CompleteValue;
                completeCount++;
            }

            _questData.ProgressValue = progressValue;

            Managers.Game.IncreaseItem(_questChart.RewardType, _questChart.RewardId, _questChart.RewardValue * completeCount);
            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.QuestComplete, _questChart.Id, completeCount));
            
            if (UIQuestPopup.CompleteQuests.ContainsKey(_questChart.Id))
                UIQuestPopup.CompleteQuests[_questChart.Id] += completeCount;
            else
                UIQuestPopup.CompleteQuests[_questChart.Id] = completeCount;

            if (UIQuestPopup.GainItems.ContainsKey(_questChart.RewardId))
                UIQuestPopup.GainItems[_questChart.RewardId] += _questChart.RewardValue * completeCount;
            else
                UIQuestPopup.GainItems[_questChart.RewardId] = _questChart.RewardValue * completeCount;
            
            RefreshUI();
        }
    }
}