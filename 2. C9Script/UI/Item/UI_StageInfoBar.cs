using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_StageInfoBar : MonoBehaviour
{
    #region Fields
    [SerializeField] private TMP_Text _stageTxt;
    [SerializeField] private TMP_Text _countTxt;
    [SerializeField] private TMP_Text _dropChanceTxt;
    #endregion

    #region Public Methods
    public void Init(int stage, int count, double dropChance)
    {
        _stageTxt.text = stage.ToString();
        _countTxt.text = count.ToString();
        _dropChanceTxt.text = dropChance.ToString();
    }
    #endregion
}
