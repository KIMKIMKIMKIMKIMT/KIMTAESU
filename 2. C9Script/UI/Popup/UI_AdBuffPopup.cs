using System;
using CodeStage.AntiCheat.Storage;
using GameData;
using Newtonsoft.Json;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public class UI_AdBuffPopup : UI_Popup
{
    [Serializable]
    public record BuffItem
    {
        public GameObject OffObj;
        public GameObject OnObj;
        public Button ShowButton;
        public TMP_Text ValueText;
        public TMP_Text BuffRemainTimeText;
    }

    [SerializeField] private Button CloseButton;

    [SerializeField] private BuffItem[] BuffItems;

    [SerializeField] private GameObject Buff5OffBackgroundObj;
    [SerializeField] private GameObject Buff5OnBackgroundObj;
    [SerializeField] private GameObject Buff5OffObj;
    [SerializeField] private GameObject Buff5OnObj;
    [SerializeField] private TMP_Text Buff5ValueText;


    public override bool isTop => true;

    private readonly CompositeDisposable _compositeDisposable = new();

    private void Start()
    {
        CloseButton.BindEvent(ClosePopup);

        for (int i = 0; i < BuffItems.Length; i++)
        {
            int buffId = i + 1;

            if (buffId != 5)
                BuffItems[i].ValueText.text = $"X{(ChartManager.AdBuffCharts[buffId].BuffValue + 1):N1}";
            else
                BuffItems[i].ValueText.text = $"{((ChartManager.AdBuffCharts[buffId].BuffValue + 1)*100):N0}%";
            
            BuffItems[i].ShowButton.BindEvent(() =>
            {
                if (Managers.Game.UserData.IsAdSkip())
                    return;
                
                if (buffId == 5)
                    return;

                if (Managers.Game.AdBuffDurationTimes.TryGetValue(buffId, out var buffDurationTime))
                {
                    if (buffDurationTime.Value > 43200)
                        Managers.Message.ShowMessage("최대 누적 시간입니다");
                    else if (ChartManager.AdBuffCharts.TryGetValue(buffId, out var adBuffChart) && buffDurationTime.Value + adBuffChart.Duration > 43200)
                        Managers.Message.ShowMessage("최대 누적 시간입니다");
                    else
                        Managers.Ad.Show(() => AdRewardCallback(buffId));
                }
                else
                    Managers.Ad.Show(() => AdRewardCallback(buffId));
            });

            if (Managers.Game.UserData.IsAdSkip())
            {
                BuffItems[i].OnObj.SetActive(true);
                BuffItems[i].OffObj.SetActive(false);
            }
            else
            {
                if (Managers.Game.AdBuffDurationTimes.ContainsKey(buffId))
                {
                    BuffItems[i].OnObj.SetActive(true);
                    BuffItems[i].OffObj.SetActive(false);
                }
                else
                {
                    BuffItems[i].OnObj.SetActive(false);
                    BuffItems[i].OffObj.SetActive(true);
                }
            }
        }

        Buff5ValueText.text =$"{((ChartManager.AdBuffCharts[5].BuffValue)*100):N0}%";
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();            
        }
    }

    public override void Open()
    {
        base.Open();

        RefreshUI();
    }

    private void OnDisable()
    {
        _compositeDisposable.Clear();
    }

    private void RefreshUI()
    {
        if (Managers.Game.UserData.IsAdSkip())
        {
            foreach (var buffItem in BuffItems)
            {
                buffItem.OnObj.SetActive(true);
                buffItem.OffObj.SetActive(false);
                buffItem.BuffRemainTimeText.text = string.Empty;
            }
            
            Buff5OffObj.SetActive(false);
            Buff5OnObj.SetActive(true);
        }
        else
        {
            _compositeDisposable.Clear();

            int enableBuffCount = 0;

            for (int i = 0; i < BuffItems.Length; i++)
            {
                int buffId = i + 1;

                if (Managers.Game.AdBuffDurationTimes.TryGetValue(buffId, out var remainTimeProperty))
                {
                    int index = i;
                    enableBuffCount++;
                    remainTimeProperty.Subscribe(remainTime =>
                    {
                        BuffItems[index].BuffRemainTimeText.text = Managers.Game.UserData.IsAdSkip() ?
                            string.Empty :
                            remainTime >= 60 ? $"{(int)(remainTime / 60)}분" : $"{remainTime}초";
                    }).AddTo(_compositeDisposable);
                }
                else
                {
                    BuffItems[i].OffObj.SetActive(true);
                    BuffItems[i].OnObj.SetActive(false);
                    BuffItems[i].BuffRemainTimeText.text = string.Empty;
                }
            }

            if (enableBuffCount >= 4)
            {
                Buff5OffBackgroundObj.SetActive(false);
                Buff5OffObj.SetActive(false);
                Buff5OnBackgroundObj.SetActive(true);
                Buff5OnObj.SetActive(true);
            }
            else
            {
                Buff5OffBackgroundObj.SetActive(true);
                Buff5OffObj.SetActive(true);
                Buff5OnBackgroundObj.SetActive(false);
                Buff5OnObj.SetActive(false);
            }
        }
    }

    private void AdRewardCallback(int buffId)
    {
        MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.ShowAdBuff, buffId, 1));
        
        // 광고 보상 콜백후 리워드 지급
        var chartData = ChartManager.AdBuffCharts[buffId];

        if (Managers.Game.AdBuffDurationTimes.ContainsKey(buffId))
            Managers.Game.AdBuffDurationTimes[buffId].Value += chartData.Duration;
        else
        {
            Managers.Game.AdBuffDurationTimes.Add(buffId, new FloatReactiveProperty(chartData.Duration));
            Managers.Game.AdBuffDurationTimes[buffId].Subscribe(remainTime =>
            {
                BuffItems[buffId - 1].BuffRemainTimeText.text =
                    remainTime >= 60 ? $"{(int)(remainTime / 60)}분" : $"{remainTime}초";
            }).AddTo(_compositeDisposable);
        }

        Managers.Game.AdBuffStatDatas[(int)chartData.BuffStatType]
            = chartData.BuffValue;

        BuffItems[buffId - 1].OnObj.SetActive(true);
        BuffItems[buffId - 1].OffObj.SetActive(false);

        // 모든 광고 버프가 활성화 되있다면 5번 버프 활성화
        if (Managers.Game.AdBuffDurationTimes.Keys.Count >= 4)
        {
            chartData = ChartManager.AdBuffCharts[5];

            if (Managers.Game.AdBuffStatDatas.ContainsKey((int)chartData.BuffStatType))
                Managers.Game.AdBuffStatDatas[(int)chartData.BuffStatType] = chartData.BuffValue;
            else
                Managers.Game.AdBuffStatDatas.Add((int)chartData.BuffStatType, chartData.BuffValue);

            Buff5OffBackgroundObj.SetActive(false);
            Buff5OffObj.SetActive(false);
            Buff5OnBackgroundObj.SetActive(true);
            Buff5OnObj.SetActive(true);
        }

        PlayerPrefs.SetString("AdBuffTime", JsonConvert.SerializeObject(Managers.Game.AdBuffDurationTimes));

        Managers.Game.CalculateStat();
    }

    public void DisableBuff(int buffId)
    {
        if (buffId >= 5)
        {
            Buff5OffBackgroundObj.SetActive(true);
            Buff5OffObj.SetActive(true);
            Buff5OnBackgroundObj.SetActive(false);
            Buff5OnObj.SetActive(false);
        }
        else
        {
            BuffItems[buffId - 1].OffObj.SetActive(true);
            BuffItems[buffId - 1].OnObj.SetActive(false);
        }
    }
}