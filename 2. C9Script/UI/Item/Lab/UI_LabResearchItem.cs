using System;
using System.Collections;
using System.Text;
using BackEnd;
using GameData;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LabResearchItem : UI_Base
{
    [SerializeField] private Image SkillTypeImage;
    [SerializeField] private Image BgImage;

    [SerializeField] private TMP_Text LevelText;
    [SerializeField] private TMP_Text SkillTypeText;
    [SerializeField] private TMP_Text EffectValueText;
    [SerializeField] private TMP_Text ResearchProgressText;
    [SerializeField] private TMP_Text ResearchCostText;
    [SerializeField] private TMP_Text FinishCostText;
    [SerializeField] private TMP_Text CoolTimeText;

    [SerializeField] private Slider ResearchProgressSlider;

    [SerializeField] private Button StartResearchButton;
    [SerializeField] private Button FinishResearchButton;

    [SerializeField] private GameObject CompleteImmediatelyObj;
    [SerializeField] private GameObject CompleteObj;
    [SerializeField] private GameObject CoolTimeCover;

    [SerializeField] private Toggle AutoToggle;

    private LabResearchData _labResearchData;

    private Coroutine _refreshCoolTimeCoroutine;
    private Coroutine _refreshProgressCoroutine;

    private Coroutine _autoCoroutine;

    private void Awake()
    {
        StartResearchButton.BindEvent(OnClickStartResearch);
        FinishResearchButton.BindEvent(OnClickFinishResearch);
        AutoToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
                InvokeRepeating(nameof(Invoke_AutoLabResearch), 0, 1);
            else
                CancelInvoke(nameof(Invoke_AutoLabResearch));

            // if (isOn)
            //     _autoCoroutine = Managers.Manager.StartCoroutine(CoAutoResearch());
            // else
            // {
            //     if (_autoCoroutine != null)
            //         Managers.Manager.StopCoroutine(_autoCoroutine);
            // }
        });
    }

    private void CheckRefresh()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (CoolTimeCover.activeSelf)
        {
            if (_refreshCoolTimeCoroutine != null)
                StopCoroutine(_refreshCoolTimeCoroutine);

            _refreshCoolTimeCoroutine = StartCoroutine(CoRefreshCoolTime());
        }

        if (_labResearchData.IsResearch)
        {
            if (_refreshProgressCoroutine != null)
                StopCoroutine(_refreshProgressCoroutine);

            _refreshProgressCoroutine = StartCoroutine(CoRefreshProgress());
        }
    }

    public void Init(LabResearchData labResearchData)
    {
        _labResearchData = labResearchData;

        SetUI();
    }

    public void RefreshUI()
    {
        SetUI();
    }

    private void SetUI()
    {
        var labSkillChart = ChartManager.LabSkillCharts[_labResearchData.LabSkillType];

        SkillTypeImage.sprite = Managers.Resource.LoadLabIcon(labSkillChart.Icon);
        SkillTypeText.text = ChartManager.GetString(labSkillChart.LabSkillType.ToString());
        BgImage.sprite = Managers.Resource.LoadLabIcon(labSkillChart.Bg);
        LevelText.text = $"Lv.{_labResearchData.Level}";

        StartResearchButton.gameObject.SetActive(!_labResearchData.IsResearch &&
                                                 ChartManager.LabSkillLevelCharts.ContainsKey(
                                                     _labResearchData.Level + 1));
        FinishResearchButton.gameObject.SetActive(_labResearchData.IsResearch);
        if (ChartManager.LabSkillLevelCharts.TryGetValue(_labResearchData.Level + 1, out var labSkillLevelChart))
        {
            ResearchCostText.text = labSkillLevelChart.LabCost.ToString();
            FinishCostText.text = labSkillLevelChart.FinishCost.ToString();
        }

        EffectValueText.text =
            ChartManager.LabSkillLevelCharts.TryGetValue(_labResearchData.Level, out labSkillLevelChart)
                ? $"{(labSkillLevelChart.IncreaseDamagePercent * 100).ToCurrencyString()}% x 스킬 타격 횟수"
                : "0%";

        CoolTimeCover.SetActive(Utils.GetNow() < _labResearchData.CoolTime);
        CoolTimeText.text = GetCoolTimeString();

        RefreshProgressUI();

        CheckRefresh();
    }

    private string GetProgressString()
    {
        if (!_labResearchData.IsResearch)
            return string.Empty;

        if (Utils.GetNow() >= _labResearchData.EndResearchTime)
            return string.Empty;

        var gap = _labResearchData.EndResearchTime - Utils.GetNow();

        if (gap.Days > 0)
            return $"{gap.Days}일 {gap.Hours}시간";

        if (gap.Hours > 0)
            return $"{gap.Hours}시간 {gap.Minutes}분";

        if (gap.Minutes > 0)
            return $"{gap.Minutes}분 {gap.Seconds}초";

        return $"{gap.Seconds}초";
    }

    private string GetCoolTimeString()
    {
        if (_labResearchData.IsResearch)
            return string.Empty;

        if (Utils.GetNow() >= _labResearchData.CoolTime)
            return string.Empty;

        var gap = _labResearchData.CoolTime - Utils.GetNow();

        if (gap.Days > 0)
            return $"{gap.Days}일:{gap.Hours:00}:{gap.Minutes:00}:{gap.Seconds:00}";

        if (gap.Hours > 0)
            return $"{gap.Hours:00}:{gap.Minutes:00}:{gap.Seconds:00}";

        if (gap.Minutes > 0)
            return $"{gap.Minutes:00}:{gap.Seconds:00}";

        return $"{gap.Seconds}초";
    }

    private float GetProgressRatio()
    {
        if (!_labResearchData.IsResearch)
            return 0;

        if (Utils.GetNow() >= _labResearchData.EndResearchTime)
            return 1;

        var completeProgressTime = _labResearchData.EndResearchTime - _labResearchData.StartResearchTime;
        var progressTime = _labResearchData.EndResearchTime - Utils.GetNow();

        if (completeProgressTime.TotalSeconds == 0)
            return 1;

        return 1 - (float)(progressTime.TotalSeconds / completeProgressTime.TotalSeconds);
    }

    private void OnClickStartResearch()
    {
        if (!ChartManager.LabSkillLevelCharts.ContainsKey(_labResearchData.Level + 1))
            return;

        if (_labResearchData.IsResearch)
            return;

        if (!ChartManager.LabSkillLevelCharts.TryGetValue(_labResearchData.Level + 1, out var labSkillLevelChart))
            return;

        if (!Utils.IsEnoughItem(ItemType.Goods, (int)Goods.SkillEnhancementStone, labSkillLevelChart.LabCost))
        {
            Managers.Message.ShowMessage("재화가 부족합니다.");
            return;
        }

        if (Utils.GetNow() < _labResearchData.CoolTime)
        {
            Managers.Message.ShowMessage("정비 시간 입니다.");
            return;
        }

        _labResearchData.IsResearch = true;
        _labResearchData.StartResearchTime = Utils.GetNow();
        _labResearchData.EndResearchTime = _labResearchData.StartResearchTime.AddSeconds(labSkillLevelChart.LabTime);

        Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.SkillEnhancementStone, labSkillLevelChart.LabCost);

        GameDataManager.LabGameData.SaveResearchGameData(_labResearchData.LabSkillType, true);
        GameDataManager.GoodsGameData.SaveGameData();

        Managers.Message.ShowMessage("연구 시작");

        var param = new Param()
        {
            { "LabType", _labResearchData.LabSkillType.ToString() },
            { "Type", "Start" },
            { "Lv", _labResearchData.Level },
            { "SkillStoneCost", labSkillLevelChart.LabCost.GetDecrypted() }
        };

        Debug.Log($"Lab Start Log Size : {Encoding.UTF8.GetByteCount(JsonMapper.ToJson(param))}");

        Backend.GameLog.InsertLog("Lab", param);

        SetUI();
    }

    private void OnClickFinishResearch()
    {
        if (!_labResearchData.IsResearch)
            return;

        if (!ChartManager.LabSkillLevelCharts.TryGetValue(_labResearchData.Level + 1, out var labSkillLevelChart))
            return;

        if (Utils.GetNow() < _labResearchData.EndResearchTime)
        {
            var popup = Managers.UI.ShowPopupUI<UI_YesNoPopup>();
            popup.Init($"연구가 완료되지 않았습니다.\n{labSkillLevelChart.FinishCost}다이아로 즉시 완료 하시겠습니까?", ImmediatelyComplete);
            return;
        }

        CompleteResearch();
    }

    private void ImmediatelyComplete()
    {
        if (!ChartManager.LabSkillLevelCharts.TryGetValue(_labResearchData.Level + 1, out var labSkillLevelChart))
            return;

        Managers.UI.ClosePopupUI();

        if (!Utils.IsEnoughItem(ItemType.Goods, (int)Goods.StarBalloon, labSkillLevelChart.FinishCost))
        {
            Managers.Message.ShowMessage("재화가 부족합니다.");
        }
        else
        {
            Managers.Game.DecreaseItem(ItemType.Goods, (int)Goods.StarBalloon, labSkillLevelChart.FinishCost);
            GameDataManager.GoodsGameData.SaveGameData();

            CompleteResearch();

            var param = new Param()
            {
                { "LabType", _labResearchData.LabSkillType.ToString() },
                { "Type", "ImmediatelyComplete" },
                { "Lv", _labResearchData.Level },
                { "CashCost", labSkillLevelChart.FinishCost.GetDecrypted() }
            };

            Debug.Log($"Lab ImmediatelyComplete Log Size : {Encoding.UTF8.GetByteCount(JsonMapper.ToJson(param))}");

            Backend.GameLog.InsertLog("Lab", param);
        }
    }

    private void CompleteResearch()
    {
        if (!ChartManager.LabSkillLevelCharts.TryGetValue(_labResearchData.Level + 1, out var labSkillLevelChart))
            return;

        Managers.Message.ShowMessage("연구 완료");

        _labResearchData.Level += 1;
        _labResearchData.IsResearch = false;
        _labResearchData.StartResearchTime = new DateTime();
        _labResearchData.EndResearchTime = new DateTime();
        _labResearchData.CoolTime = Utils.GetNow().AddSeconds(labSkillLevelChart.NextLabCoolTime);

        GameDataManager.LabGameData.SaveResearchGameData(_labResearchData.LabSkillType, true);

        SetUI();
    }

    private IEnumerator CoRefreshProgress()
    {
        var delay = new WaitForSeconds(1f);

        while (true)
        {
            if (!_labResearchData.IsResearch)
                yield break;

            if (Utils.GetNow() >= _labResearchData.EndResearchTime)
            {
                RefreshProgressUI();
                yield break;
            }

            RefreshProgressUI();

            yield return delay;
        }
    }

    private IEnumerator CoRefreshCoolTime()
    {
        var delay = new WaitForSeconds(1f);

        while (true)
        {
            if (_labResearchData.IsResearch)
                yield break;

            if (Utils.GetNow() >= _labResearchData.CoolTime)
            {
                SetUI();
                yield break;
            }

            CoolTimeText.text = GetCoolTimeString();

            yield return delay;
        }
    }

    private void RefreshProgressUI()
    {
        ResearchProgressText.text = GetProgressString();
        ResearchProgressSlider.value = GetProgressRatio();
        CompleteImmediatelyObj.SetActive(_labResearchData.IsResearch &&
                                         Utils.GetNow() < _labResearchData.EndResearchTime);
        CompleteObj.SetActive(_labResearchData.IsResearch && Utils.GetNow() >= _labResearchData.EndResearchTime);
    }

    private void Invoke_AutoLabResearch()
    {
        if (!AutoToggle.isOn)
        {
            CancelInvoke(nameof(Invoke_AutoLabResearch));
            return;
        }

        // 연구중
        if (_labResearchData.IsResearch)
        {
            // 연구 완료
            if (Utils.GetNow() >= _labResearchData.EndResearchTime)
            {
                OnClickFinishResearch();
            }
        }
        else
        {
            // 정비시간이 끝났을 때
            if (Utils.GetNow() >= _labResearchData.CoolTime)
            {
                if (ChartManager.LabSkillLevelCharts.TryGetValue(_labResearchData.Level + 1,
                        out var labSkillLevelChart))
                {
                    // 연구 시작
                    if (Utils.IsEnoughItem(ItemType.Goods, (int)Goods.SkillEnhancementStone,
                            labSkillLevelChart.LabCost))
                    {
                        OnClickStartResearch();
                    }
                    // 재료 부족으로 종료
                    else
                    {
                        AutoToggle.isOn = false;
                        CancelInvoke(nameof(Invoke_AutoLabResearch));
                    }
                }
                else
                {
                    AutoToggle.isOn = false;
                    CancelInvoke(nameof(Invoke_AutoLabResearch));
                }
            }
        }
    }
}