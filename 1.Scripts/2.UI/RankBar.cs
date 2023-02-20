using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RankBar : MonoBehaviour
{
    #region Fields
    [SerializeField] public GameObject[] _medalImgs;
    [SerializeField] private Text indexTxt;
    [SerializeField] private Text _userNameTxt;
    [SerializeField] private Text _timeTxt;
    #endregion


    #region Public Methods
    public void SetRankBar(int index, string name, float time)
    {
        indexTxt.text = index.ToString();
        _userNameTxt.text = name;
        
        TimeSpan timespan = TimeSpan.FromSeconds(time);
        _timeTxt.text = timespan.ToString(@"mm\:ss\:ff");

    }
    #endregion
}
