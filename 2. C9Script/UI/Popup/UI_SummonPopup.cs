using System;
using System.Collections.Generic;
using BackEnd;
using BackEnd.Tcp;
using DG.Tweening;
using GameData;
using LitJson;
using Newtonsoft.Json;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_SummonPopup : UI_Popup
{
    [Serializable]
    public class Tab
    {
        public SummonType SummonType;
        public GameObject SelectObj;
        public Button TabButton;
        public UI_Panel SummonPanel;
    }

    [Serializable]
    public record SummonLvUIBundle
    {
        public Image SummonTypeIconImage;
        public TMP_Text SummonLvText;
        public TMP_Text SummonExpText;
        public Slider SummonExpSlider;

        public Sprite WeaponSprite;
        public Sprite PetSprite;
        public Sprite CollectionSprite;
    }

    [SerializeField] private UI_WeaponSummonPanel UIWeaponSummonPanel;
    [SerializeField] private UI_PetSummonPanel UIPetSummonPanel;
    [SerializeField] private UI_CollectionSummonPanel UICollectionSummonPanel;

    [SerializeField] private UI_SummonResultPanel UISummonResultPanel;

    [SerializeField] private TMP_Text SummonLvText;
    [SerializeField] private Toggle SkipEffectToggle;

    [SerializeField] private Button SummonRatePanelButton;

    [SerializeField] private Image SummonBackground;
    [SerializeField] private Animator SummonAnimator;

    [SerializeField] private GameObject SummonPanelObj;

    [SerializeField] private Tab[] SummonTypeTabs;

    [SerializeField] private SummonLvUIBundle SummonLvUI;
    
    [SerializeField] private GameObject NavigationObj;
    [SerializeField] private GameObject PetTabNavigationObj;
    [SerializeField] private GameObject CollectionTabNavigationObj;

    private bool _isSkipEffect;

    private Tab _selectSummonTab;

    private bool _isSummoning;

    private CompositeDisposable _compositeDisposable = new();

    private Dictionary<int, double> _weaponSummonLog = new();
    private Dictionary<int, double> _petSummonLog = new();
    private Dictionary<int, double> _collectionSummonLog = new();
    private double _summonUsingJewel;

    private readonly CompositeDisposable _guideComposite = new();

    private Tab SelectSummonTab
    {
        get => _selectSummonTab;
        set
        {
            if (_selectSummonTab != null)
            {
                _selectSummonTab.SelectObj.SetActive(false);
                _selectSummonTab.SummonPanel.Close();
            }

            _selectSummonTab = value;

            SummonRatePanelButton.gameObject.SetActive(_selectSummonTab.SummonType != SummonType.Collection);
            SummonPanelObj.SetActive(_selectSummonTab.SummonType != SummonType.Costume);
            _selectSummonTab.SelectObj.SetActive(true);
            _selectSummonTab.SummonPanel.Open();

            if (!Utils.IsAllClearGuideQuest())
                SetNavigation(Managers.Game.UserData.ProgressGuideQuestId);

            SetUI();
        }
    }

    private void Start()
    {
        foreach (var tab in SummonTypeTabs)
        {
            tab.TabButton.BindEvent(() => SelectSummonTab = tab);
        }

        UIWeaponSummonPanel.SummonEvent = SummonItem;
        UIPetSummonPanel.SummonEvent = SummonItem;
        UICollectionSummonPanel.SummonEvent = SummonItem;

        UISummonResultPanel.OnCloseCallback = () =>
        {
            SummonAnimator.gameObject.SetActive(true);
            _isSkipEffect = SkipEffectToggle.isOn;
        };

        SkipEffectToggle.onValueChanged.AddListener(value => _isSkipEffect = value);
        SummonRatePanelButton.BindEvent(OnClickSummonRate);

        Managers.Game.UserData.OnChangeGuideQuestId.Subscribe(SetNavigation).AddTo(_guideComposite);
        Managers.Game.UserData.OnChangeGuideQuestProgressValue.Subscribe(SetNavigationValue).AddTo(_guideComposite);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UISummonResultPanel.gameObject.activeSelf)
            {
                UISummonResultPanel.Close();
                return;
            }

            Managers.UI.ClosePopupUI();
        }
    }

    public override void Open()
    {
        base.Open();

        SummonBackground.DOFade(0, 0);
        UISummonResultPanel.Close();
        SummonAnimator.gameObject.SetActive(true);

        if (SelectSummonTab == null)
            SelectSummonTab = SummonTypeTabs[0];
        else
            SetUI();

        SummonAnimator.transform.DOLocalMoveY(250f, 0);
    }

    private void OnDisable()
    {
        _compositeDisposable.Clear();
        StopAllCoroutines();
        FadeScreen.StopSummonFade();
        _isSummoning = false;

        Param param = new();

        foreach (var weaponSummonLog in _weaponSummonLog)
            param.Add($"Weapon_{weaponSummonLog.Key}", weaponSummonLog.Value);
        _weaponSummonLog.Clear();

        foreach (var petSummonLog in _petSummonLog)
            param.Add($"Pet_{petSummonLog.Key}", petSummonLog.Value);
        _petSummonLog.Clear();

        foreach (var collectionSummonLog in _collectionSummonLog)
            param.Add($"Collection_{collectionSummonLog.Key}", collectionSummonLog.Value);
        _collectionSummonLog.Clear();

        if (_summonUsingJewel > 0)
        {
            param.Add("UsingJewel", _summonUsingJewel);
            _summonUsingJewel = 0;
        }

        if (param.Count > 0)
        {
#if !UNITY_EDITOR
            Backend.GameLog.InsertLog("Summon", param);
#endif
        }
    }

    private void SetNavigation(int id)
    {
        if (Utils.IsAllClearGuideQuest())
        {
            _guideComposite.Clear();
            return;
        }
        
        if (Utils.IsCompleteGuideQuest())
            return;

        switch (id)
        {
            case 5 when SelectSummonTab != null && SelectSummonTab.SummonType == SummonType.Weapon:
            case 12 when SelectSummonTab != null && SelectSummonTab.SummonType == SummonType.Pet:
            case 16 when SelectSummonTab != null && SelectSummonTab.SummonType == SummonType.Collection:
                NavigationObj.SetActive(true);
                break;
            default:
                NavigationObj.SetActive(false);
                break;
        }
        
        PetTabNavigationObj.SetActive(id == 12 && SelectSummonTab != null && SelectSummonTab.SummonType != SummonType.Pet);
        CollectionTabNavigationObj.SetActive(id == 16 && SelectSummonTab != null && SelectSummonTab.SummonType != SummonType.Collection);
    }

    private void SetNavigationValue(long value)
    {
        if (!Utils.IsCompleteGuideQuest())
            return;
        
        NavigationObj.SetActive(false);
        PetTabNavigationObj.SetActive(false);
        CollectionTabNavigationObj.SetActive(false);
    }

    private void SummonItem(SummonType summonType, string probabilityId, int count, double cost,
        Action autoSummonCallback)
    {
        if (_isSummoning)
            return;

        if (!Utils.IsEnoughItem(ItemType.Goods, (int)Goods.StarBalloon, cost))
        {
            Managers.Message.ShowMessage("보석이 부족합니다!!");
            UISummonResultPanel.StopAutoSummon();
            return;
        }

        _isSummoning = true;


        Debug.Log($"확률 차트 Id : {probabilityId}");

        Backend.Probability.GetProbabilitys(probabilityId, count, bro =>
        {
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog("Fail GetProbabilitys", bro);
                return;
            }

            JsonData jsonData = bro.GetFlattenJSON()["elements"];

            List<int> probabilityIds = new();

            if (count > 80 || jsonData.Count > 80)
            {
                Managers.Message.ShowMessage("재시도 해 주세요.");
                return;
            }

            foreach (JsonData itemJson in jsonData)
            {
                int itemId = 0;

                if (itemJson.ContainsKey("Weapon_Id"))
                    itemId = int.Parse(itemJson["Weapon_Id"].ToString());
                else if (itemJson.ContainsKey("Pet_Id"))
                    itemId = int.Parse(itemJson["Pet_Id"].ToString());
                else if (itemJson.ContainsKey("Collection_Id"))
                    itemId = int.Parse(itemJson["Collection_Id"].ToString());
                else
                    continue;

                probabilityIds.Add(itemId);
            }

            probabilityIds.ForEach(id =>
            {
                switch (summonType)
                {
                    case SummonType.Weapon:
                    {
                        Managers.Game.IncreaseItem(ItemType.Weapon, id, 1);
                        if (_weaponSummonLog.ContainsKey(id))
                            _weaponSummonLog[id] += 1;
                        else
                            _weaponSummonLog.Add(id, 1);

                        if (ChartManager.WeaponCharts.TryGetValue(id, out var weaponChart) && weaponChart.Grade >= Grade.Legeno &&  Backend.Chat.IsChatConnect(ChannelType.Public))
                        {
                            ChatData chatData = new ChatData()
                            {
                                Rank =  Managers.Rank.MyRankDatas.ContainsKey(RankType.Stage) ? Managers.Rank.MyRankDatas[RankType.Stage].Rank : 0,
                                CostumeId = Managers.Game.EquipDatas[EquipType.Costume],
                                Nickname = Backend.UserNickName,
                                ChatMessage = $"{Backend.UserNickName}님께서 <color=#BC1C1E>{ChartManager.GetString(weaponChart.Name)}</color>를 소환하였습니다."
                            };
                        
                            Backend.Chat.ChatToChannel(ChannelType.Public, JsonConvert.SerializeObject(chatData));
                        }
                    }
                        break;
                    case SummonType.Pet:
                    {
                        Managers.Game.PetDatas[id].Quantity =
                            Mathf.Min(Managers.Game.PetDatas[id].Quantity + 1, int.MaxValue);
                        if (_petSummonLog.ContainsKey(id))
                            _petSummonLog[id] += 1;
                        else
                            _petSummonLog.Add(id, 1);
                    }
                        break;
                    case SummonType.Collection:
                    {
                        Managers.Game.CollectionDatas[id].Quantity =
                            Mathf.Min(Managers.Game.CollectionDatas[id].Quantity + 1, int.MaxValue);
                        if (_collectionSummonLog.ContainsKey(id))
                            _collectionSummonLog[id] += 1;
                        else
                            _collectionSummonLog.Add(id, 1);
                    }
                        break;
                }
            });

            Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.StarBalloon, cost);
            
            GameDataManager.GoodsGameData.SaveGameData();

            _summonUsingJewel += cost;

            switch (summonType)
            {
                case SummonType.Weapon:
                    Managers.Game.UserData.SummonWeaponCount += count;
                    break;
                case SummonType.Pet:
                    Managers.Game.UserData.SummonPetCount += count;
                    break;
            }

            ShowSummonEffect(summonType, probabilityIds, () =>
            {
                _isSkipEffect = true;
                autoSummonCallback?.Invoke();
            });

            switch (summonType)
            {
                case SummonType.Weapon:
                    GameDataManager.WeaponGameData.SaveGameData();
                    break;
                case SummonType.Pet:
                    GameDataManager.PetGameData.SaveGameData();
                    break;
                case SummonType.Collection:
                    GameDataManager.CollectionGameData.SaveGameData();
                    break;
            }

            GameDataManager.UserGameData.SaveGameData();
            
            Managers.Game.CalculateStat(true);
            MessageBroker.Default.Publish(new QuestMessage(QuestProgressType.Summon, (int)summonType, count));
            GameDataManager.QuestGameData.SaveSummonQuestData();
        });
    }

    public void ShowSummonEffect(SummonType summonType, List<int> probabilityIds, Action autoSummonCallback)
    {
        if (_isSkipEffect)
        {
            SummonBackground.DOFade(0f, 0f);
            SummonAnimator.gameObject.SetActive(false);
            UISummonResultPanel.Init(summonType, probabilityIds, _isSkipEffect, autoSummonCallback);
            UISummonResultPanel.Open();
            _isSummoning = false;
            SetUI();
        }
        else
        {
            SummonAnimator.transform.DOLocalMoveY(0f, 0.5f);
            SummonBackground.DOFade(1f, 0.5f).onComplete = SetUI;
            SummonAnimator.SetTrigger("Summon");
            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
            {
                _isSummoning = false;
                FadeScreen.SummonFadeOut(() =>
                {
                    SummonBackground.DOFade(0f, 0f);
                    SummonAnimator.gameObject.SetActive(false);
                    SummonAnimator.transform.DOLocalMoveY(250f, 0);

                    UISummonResultPanel.Init(summonType, probabilityIds, _isSkipEffect, autoSummonCallback);
                    UISummonResultPanel.Open();
                    FadeScreen.SummonFadeIn(null, 0);
                }, 0.5f);
            }).AddTo(_compositeDisposable);
        }
    }

    private void SetUI()
    {
        var lv = 0;
        var exp = 0;
        var needExp = 0;

        switch (_selectSummonTab.SummonType)
        {
            case SummonType.Weapon:
            {
                SummonLvText.text = $"소환레벨 {Managers.Game.UserData.SummonWeaponLv}";
                SummonLvUI.SummonTypeIconImage.sprite = SummonLvUI.WeaponSprite;
                exp = Managers.Game.UserData.CurrentLevelSummonWeaponCount;
                needExp = ChartManager.WeaponSummonLevelCharts.TryGetValue(Managers.Game.UserData.SummonWeaponLv,
                    out var weaponSummonLevelChart)
                    ? weaponSummonLevelChart.Exp
                    : 0;
                lv = Managers.Game.UserData.SummonWeaponLv;
            }
                break;
            case SummonType.Pet:
            {
                SummonLvText.text = $"소환레벨 {Managers.Game.UserData.SummonPetLv}";
                SummonLvUI.SummonTypeIconImage.sprite = SummonLvUI.PetSprite;
                exp = Managers.Game.UserData.CurrentLevelSummonPetCount;
                needExp = ChartManager.PetSummonLevelCharts.TryGetValue(Managers.Game.UserData.SummonPetLv,
                    out var petSummonLevelChart)
                    ? petSummonLevelChart.Exp
                    : 0;
                lv = Managers.Game.UserData.SummonPetLv;
            }
                break;
            case SummonType.Collection:
            {
                SummonLvUI.SummonTypeIconImage.sprite = SummonLvUI.CollectionSprite;
                SummonLvUI.SummonExpText.text = "Max";
                SummonLvUI.SummonExpSlider.value = 1f;
            }
                break;
        }

        SummonLvUI.SummonLvText.text = lv == 0 ? string.Empty : $"Lv.{lv}";
        SummonLvUI.SummonExpSlider.value = needExp == 0 ? 1f : exp / (float)needExp;
        SummonLvUI.SummonExpText.text = needExp == 0 ? "Max" : $"{exp.ToString()} / {needExp.ToString()}";
    }

    private void OnClickSummonRate()
    {
        var uiSummonRatePopup = Managers.UI.ShowPopupUI<UI_SummonRatePopup>();

        if (uiSummonRatePopup == null)
            return;

        uiSummonRatePopup.Init(_selectSummonTab.SummonType);
    }
}