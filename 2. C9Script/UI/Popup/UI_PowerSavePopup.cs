using System;
using System.Collections;
using TMPro;
using UI;
using UnityEngine;

public class UI_PowerSavePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text TimeText;
    [SerializeField] private TMP_Text BatteryText;

    [SerializeField] private GameObject Obj;

    private DateTime _dateTime;

    private void Start()
    {
        StartCoroutine(CoTimer());
        
        Obj.BindEvent(() =>
        {
            if (_dateTime != null && (Utils.GetNow() - _dateTime).TotalSeconds < 2)
                Destroy(gameObject);
            else
            {
                _dateTime = Utils.GetNow();
            }
        });
    }

    private IEnumerator CoTimer()
    {
        var delay = new WaitForSeconds(0.5f);
        
        while (true)
        {
            TimeText.text = Utils.GetNow().ToString("HH:mm:ss");
            BatteryText.text = ((int)(SystemInfo.batteryLevel * 100)).ToString();
            yield return delay;
        }
    }
}