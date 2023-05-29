using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_SummonResultPanel : UI_Panel
{
    [SerializeField] private TMP_Text SummonLvText;
    [SerializeField] private TMP_Text SummonLvValueText;
    [SerializeField] private TMP_Text ProgressText;
    [SerializeField] private Slider SummonExpSlider;
    [SerializeField] private Image SummonIconImage;
    [SerializeField] private ScrollRect UISummonScrollView;
    [SerializeField] private Transform UISummonItemScrollViewTr;
    [SerializeField] private Transform UISummonItemRoot;
    [SerializeField] private Button CloseButton;
    [SerializeField] private Toggle AutoSummonToggle;

    [SerializeField] private Sprite WeaponSprite;
    [SerializeField] private Sprite PetSprite;
    [SerializeField] private Sprite CollectionSprite;


    private SummonType _summonType;
    private List<int> _probabilityIds;
    private bool _isSkip;

    private readonly List<UI_SummonItem> _uiSummonItems = new();

    public Action OnCloseCallback;

    private Action _summonAction;
    private Coroutine _autoSummonCoroutine;

    private void Start()
    {
        CloseButton.BindEvent(() =>
        {
            Close();
            OnCloseCallback?.Invoke();
        });

        AutoSummonToggle.onValueChanged.AddListener(isAutoSummon =>
        {
            if (isAutoSummon)
            {
                StopAllCoroutines();
                _autoSummonCoroutine = StartCoroutine(CoAutoSummon());
            }
            else
            {
                if (_autoSummonCoroutine != null)
                {
                    StopCoroutine(_autoSummonCoroutine);
                    _autoSummonCoroutine = null;
                }
            }
        });
    }

    private void OnDisable()
    {
        AutoSummonToggle.isOn = false;
        OnCloseCallback?.Invoke();
    }

    public void Init(SummonType summonType, List<int> probabilityIds, bool isSkip, Action summonAction)
    {
        _summonType = summonType;
        _probabilityIds = probabilityIds;
        _isSkip = isSkip;
        _summonAction = summonAction;
    }

    public override void Open()
    {
        base.Open();
        SetUI();

        if (_isSkip)
        {
            MakeSummonItems();
        }
        else
            StartCoroutine(CoMakeSummonItems());
    }

    private void SetUI()
    {
        string summonLvString = string.Empty;
        float exp = 0;
        float needExp = 0;

        switch (_summonType)
        {
            case SummonType.Weapon:
            {
                SummonIconImage.sprite = WeaponSprite;
                SummonLvText.text = "무기 소환 레벨";
                summonLvString = Managers.Game.UserData.SummonWeaponLv.ToString();
                exp = Managers.Game.UserData.CurrentLevelSummonWeaponCount;
                needExp = ChartManager.WeaponSummonLevelCharts[Managers.Game.UserData.SummonWeaponLv].Exp;
            }
                break;
            case SummonType.Pet:
            {
                SummonIconImage.sprite = PetSprite;
                SummonLvText.text = "펫 소환 레벨";
                summonLvString = Managers.Game.UserData.SummonPetLv.ToString();
                exp = Managers.Game.UserData.CurrentLevelSummonPetCount;
                needExp = ChartManager.PetSummonLevelCharts.TryGetValue(Managers.Game.UserData.SummonPetLv, out var petSummonLevelChart) ?
                    petSummonLevelChart.Exp : 0;
            }
                break;
            case SummonType.Collection:
            {
                SummonIconImage.sprite = CollectionSprite;
                SummonLvText.text = "유물 소환";
                summonLvString = string.Empty;
                exp = 0;
                needExp = 0;
            }
                break;
        }

        SummonLvValueText.text = string.IsNullOrEmpty(summonLvString) ? string.Empty : $"Lv.{summonLvString}";
        SummonExpSlider.value = needExp == 0 ? 1f : exp / needExp;
        ProgressText.text = needExp == 0 ? "Max" : $"{exp.ToString()} / {needExp.ToString()}";
    }

    private void MakeSummonItems()
    {
        UISummonItemScrollViewTr.DOScale(2f, 0);

        if (_uiSummonItems.Count <= 0)
            UISummonItemRoot.DestroyInChildren();

        _uiSummonItems.ForEach(uiSummonItem => uiSummonItem.gameObject.SetActive(false));

        var uiSummonItems = _uiSummonItems.ToList();

        for (int i = 0; i < _probabilityIds.Count; i++)
        {
            UI_SummonItem uiSummonItem;

            if (uiSummonItems.Count > i)
                uiSummonItem = uiSummonItems[i];
            else
            {
                uiSummonItem = Managers.UI.MakeSubItem<UI_SummonItem>(UISummonItemRoot);
                _uiSummonItems.Add(uiSummonItem);
            }

            uiSummonItem.Init(_summonType, _probabilityIds[i]);
        }
        
        UISummonItemScrollViewTr.DOScale(1, 0.1f).onComplete += () =>
        {
            UISummonItemScrollViewTr.DOShakePosition(0.3f, 5f);
        };
        
        UISummonScrollView.verticalNormalizedPosition = 0;
    }


    private IEnumerator CoMakeSummonItems()
    {
        if (_uiSummonItems.Count <= 0)
            UISummonItemRoot.DestroyInChildren();

        _uiSummonItems.ForEach(uiSummonItem => uiSummonItem.gameObject.SetActive(false));

        var uiSummonItems = _uiSummonItems.ToList();

        for (int i = 0; i < _probabilityIds.Count; i++)
        {
            UI_SummonItem uiSummonItem;

            if (uiSummonItems.Count > i)
                uiSummonItem = uiSummonItems[i];
            else
            {
                uiSummonItem = Managers.UI.MakeSubItem<UI_SummonItem>(UISummonItemRoot);
                _uiSummonItems.Add(uiSummonItem);
            }

            uiSummonItem.Init(_summonType, _probabilityIds[i]);
            UISummonScrollView.verticalNormalizedPosition = 0;
            yield return null;
        }
    }

    private IEnumerator CoAutoSummon()
    {
        if (_summonAction == null)
            yield break;

        var delay = new WaitForSeconds(1.5f);

        while (true)
        {
            _summonAction?.Invoke();
            
            yield return delay;
        }
    }

    public void StopAutoSummon()
    {
        if (_autoSummonCoroutine != null)
        {
            StopCoroutine(_autoSummonCoroutine);
            _autoSummonCoroutine = null;
            AutoSummonToggle.isOn = false;
        }
    }
}