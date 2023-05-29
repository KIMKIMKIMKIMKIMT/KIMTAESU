using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class EffectManager
{
    private readonly Dictionary<string, ObjectPool<Effect>> _effectPool = new();

    public void ShowEffect(string effectName, Vector2 position)
    {
        Effect effect = GetEffect(effectName);

        effect.EffectName = effectName;
        effect.transform.position = position;
    }

    public void ShowEffect(string effectName, Vector2 position, Vector3 scale)
    {
        Effect effect = GetEffect(effectName);

        effect.EffectName = effectName;
        effect.transform.position = position;
        effect.transform.localScale = scale;
    }

    public void ReturnEffect(string effectName, Effect effect)
    {
        _effectPool[effectName].Release(effect);
    }

    private void InitPool(string effectName)
    {
        _effectPool.Add(effectName, new ObjectPool<Effect>(
            () => CreateEffect(effectName),
            effect => effect.gameObject.SetActive(true),
            effect => effect.gameObject.SetActive(false)
            ));
    }

    private static Effect CreateEffect(string effectName)
    {
        return Managers.Resource.Instantiate($"Effect/{effectName}")?.GetComponent<Effect>();
    }

    private Effect GetEffect(string effectName)
    {
        if (!_effectPool.ContainsKey(effectName))
            InitPool(effectName);

        return _effectPool[effectName].Get();
    }
}