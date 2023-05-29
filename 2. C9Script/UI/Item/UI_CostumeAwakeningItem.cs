using TMPro;
using UnityEngine;

public class UI_CostumeAwakeningItem : UI_Base
{
    [SerializeField] private TMP_Text AwakeningText;
    [SerializeField] private UI_StatEffectItem UIStatEffectItem;
    [SerializeField] private GameObject NonAwakeningObj;

    public void Init(int awakeningLevel, int statId, double statValue, bool isAwakening)
    {
        AwakeningText.text = $"{awakeningLevel}각성";
        UIStatEffectItem.Init(statId, statValue);
        NonAwakeningObj.SetActive(!isAwakening);
    }
}