using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_CostumCollectItem : UI_Base
{
    #region Fields

    private const string SET_NAME_KEY = "Costume_Collection_Name_";
    private const string COSTUMEICON_KEY = "Chulgu_Costume_Png_";

    [SerializeField] private TMP_Text _titleTxt;
    [SerializeField] private Image[] _costumImg;
    [SerializeField] private Image _statIconImg;
    [SerializeField] private TMP_Text _statEffectTxt;

    [SerializeField] private GameObject[] _figureObjs;

    public int SetCount;
    private int _flagCount;
    #endregion

    #region Unity Methods
    private void Awake()
    {

    }
    #endregion

    #region Public Methods
    public void Init(int title, int id, int[] costumeId, string[] costumeIconId, int statId, double statValue)
    {
        _flagCount = 0;

        for (int i = 0; i < _costumImg.Length; i++)
        {
            _costumImg[i].gameObject.SetActive(false);
            _figureObjs[i].SetActive(false);
        }

        _titleTxt.text = ChartManager.GetString(SET_NAME_KEY + title.ToString());

        for (int i = 0; i < costumeIconId.Length; i++)
        {
            _figureObjs[i].SetActive(true);
            _costumImg[i].gameObject.SetActive(true);
            _costumImg[i].sprite = Managers.Resource.LoadCostumeIcon(COSTUMEICON_KEY + costumeIconId[i]);
            if (Managers.Game.CostumeDatas[costumeId[i]].Awakening > -1)
            {
                _costumImg[i].color = Color.white;
            }
            else
            {
                _costumImg[i].color = Color.gray;
                _flagCount++;
            }
        }

        _statIconImg.sprite = ChartManager.StatCharts.TryGetValue(statId, out var statChart) ?
                Managers.Resource.LoadStatIcon(statChart.Icon) : null;

        _statEffectTxt.text =  statChart.ValueType == ValueType.Percent
            ? ChartManager.GetString(statChart.Name) + " " + $" { Math.Round(statValue * 100, 7).ToCurrencyString()}%"
            : ChartManager.GetString(statChart.Name) + " " + Math.Round(statValue, 7).ToCurrencyString();

        if (_flagCount == 0)
        {
            _statIconImg.color = Color.white;
            _statEffectTxt.color = Color.white;
            _titleTxt.color = new Color(255/ 255f, 180/255f, 61/255f);
        }
        else
        {
            _statIconImg.color = Color.gray;
            _statEffectTxt.color = Color.gray;
            _titleTxt.color = Color.gray;
        }
        
    }
    #endregion
}
