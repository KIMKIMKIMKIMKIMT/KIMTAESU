using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class DpsMonster : Monster
{
    [SerializeField] private SpriteRenderer ModelSpriteRenderer;
    [SerializeField] private Sprite[] HitSprite;
    [SerializeField] private Sprite BaseSprite;

    [SerializeField] private Sprite[] BossHitSprite;
    [SerializeField] private Sprite BossBaseSprite;

    private CompositeDisposable _compositeDisposable = new();
    public bool IsBoss;

    private void Start()
    {
        IsBoss = Managers.Dps.IsBoss;
        ModelSpriteRenderer.sprite = IsBoss ? BossBaseSprite : BaseSprite;
    }

    private void OnDisable()
    {
        _compositeDisposable.Clear();
    }

    protected override void HitEffect()
    {
        ModelSpriteRenderer.sprite = IsBoss ? BossHitSprite[Random.Range(0, BossHitSprite.Length)] : HitSprite[Random.Range(0, HitSprite.Length)];
        _compositeDisposable.Clear();
        Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ =>
        {
            if (ModelSpriteRenderer == null)
                return;

            ModelSpriteRenderer.sprite = IsBoss ? BossBaseSprite : BaseSprite;
        }).AddTo(_compositeDisposable);
    }
}