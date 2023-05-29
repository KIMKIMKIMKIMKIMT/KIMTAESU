using System.Collections.Generic;
using System.Linq;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

public class DamageTextManager
{
    private ObjectPool<UI_DamageText> _damageTextPool;
    private readonly List<UI_DamageText> _damageTexts = new();

    private Transform Root;

    public void Init()
    {
        _damageTextPool = new ObjectPool<UI_DamageText>(CreateDamageText,
            damageText =>
            {
                damageText.gameObject.SetActive(true);
                _damageTexts.Add(damageText);
            },
            damageText =>
            {
                damageText.gameObject.SetActive(false);
                if (_damageTexts.Contains(damageText))
                    _damageTexts.Remove(damageText);
            });
        
        Root = GameObject.Find("DamageTextCanvas").transform;

        Managers.Game.SettingData.DamageText.Subscribe(damageText =>
        {
            if (!damageText)
                ClearDamageText();
        });
    }

    public void ShowDamageText(Transform target, double damage, double criticalMultiple)
    {
        if (!Managers.Game.SettingData.DamageText.Value)
            return;
        
        var uiDamageText = _damageTextPool.Get();

        float randNum = Random.Range(-1f, 1f);
        var pos = GetTextPosOnTarget(target);
        pos.x += randNum;

        uiDamageText.transform.position = pos;
        uiDamageText.Initialize(damage.ToCurrencyString(), criticalMultiple);
    }

    public void Return(UI_DamageText uiDamageText)
    {
        _damageTextPool.Release(uiDamageText);
    }

    private UI_DamageText CreateDamageText()
    {
        return Managers.Resource.Instantiate("ETC/DamageText", Root)?.GetComponent<UI_DamageText>();
    }

    private Vector3 GetTextPosOnTarget(Transform target)
    {
        return target.position;
    }

    public void ClearDamageText()
    {
        if (_damageTexts == null)
            return;
        
        var list = _damageTexts.ToList();
        list.ForEach(damageText =>
        {
            if (damageText == null)
                return;
            
            damageText.Return();
        });
        _damageTexts.Clear();
    }
}