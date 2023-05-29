using System;
using Chart;
using NSubstitute.ReceivedExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class UI_DpsItem : UI_Base
{
    [SerializeField] private TMP_Text IdText;
    [SerializeField] private TMP_Text DpsValueText;
    [SerializeField] private TMP_Text ItemValueText;

    [SerializeField] private Image IdBackgroundImage;
    [SerializeField] private Image ItemImage;

    [SerializeField] private Button ReceiveButton;

    [SerializeField] private GameObject CoverImageObj;
    [SerializeField] private GameObject CheckImageObj;
    [SerializeField] private GameObject RedDotImageObj;

    public UI_DungeonDpsPanel UIDungeonDpsPanel;

    private readonly Color IdOnColor = new(0.4f, 0.72f, 0.36f);
    private readonly Color TextOffColor = new(0.34f, 0.34f, 0.34f);
    private readonly Color IdBackgroundOffColor = new(0.31f, 0.31f, 0.31f);

    private int _id;
    private bool _isBoss;
    private double _clearDps;
    private ItemType _rewardItemType;
    private int _rewardItemId;
    private double _rewardItemValue;

    public bool IsSave;
    
    private bool IsReceivedReward 
        => _isBoss ? Managers.Game.UserData.ReceivedDpsBossDungeonReward.Contains(_id) :
            Managers.Game.UserData.ReceivedDpsDungeonReward.Contains(_id);

    private bool IsOverClearDps
        => _isBoss ? Managers.Game.UserData.DpsBossDungeonHighestScore >= _clearDps :
            Managers.Game.UserData.DpsDungeonHighestScore >= _clearDps;
    
    private void Start()
    {
        ReceiveButton.BindEvent(OnClickReceive);
    }
    
    public void Init(int id, bool isBoss, double clearDps, ItemType rewardItemType, int rewardItemId,
        double rewardItemValue)
    {
        _id = id;
        _isBoss = isBoss;
        _clearDps = clearDps;
        _rewardItemType = rewardItemType;
        _rewardItemId = rewardItemId;
        _rewardItemValue = rewardItemValue;
        
        SetUI();
    }

    private void SetUI()
    {
        IdText.text = _id.ToString();
        DpsValueText.text = _clearDps.ToCurrencyString();
        ItemImage.sprite = Managers.Resource.LoadItemIcon(
            _rewardItemType, _rewardItemId);
        ItemValueText.text = _rewardItemValue.ToCurrencyString();
        
        if (IsOverClearDps)
            SetOverScoreUI();
        else
            SetUnderScoreUI();
    }

    private void SetOverScoreUI()
    {
        IdText.color = IdOnColor;
        IdBackgroundImage.color = Color.white;
        DpsValueText.color = Color.white;
        
        bool isReceivedReward = IsReceivedReward;

        CoverImageObj.SetActive(isReceivedReward);
        CheckImageObj.SetActive(isReceivedReward);
        RedDotImageObj.SetActive(!isReceivedReward);
    }

    private void SetUnderScoreUI()
    {
        IdText.color = TextOffColor;
        IdBackgroundImage.color = IdBackgroundOffColor;
        DpsValueText.color = TextOffColor;

        CoverImageObj.SetActive(true);
        CheckImageObj.SetActive(false);
        RedDotImageObj.SetActive(false);
    }

    private void OnClickReceive()
    {
        if (IsReceivedReward)
        {
            Managers.Message.ShowMessage("이미 획득한 보상입니다.");
            return;
        }

        if (!IsOverClearDps)
        {
            Managers.Message.ShowMessage("전투력이 부족합니다.");
            return;
        }
        
        Managers.Game.IncreaseItem(_rewardItemType, _rewardItemId, _rewardItemValue);

        if (_isBoss)
        {
            Managers.Game.UserData.ReceivedDpsBossDungeonReward.Add(_id);
            UIDungeonDpsPanel.BossReceiveIds.Add(_id);
            UIDungeonDpsPanel.BossGainValues += _rewardItemValue;
        }
        else
        {
            Managers.Game.UserData.ReceivedDpsDungeonReward.Add(_id);
            UIDungeonDpsPanel.ReceiveIds.Add(_id);
            UIDungeonDpsPanel.GainValues += _rewardItemValue;
        }

        SetUI();
    }
}