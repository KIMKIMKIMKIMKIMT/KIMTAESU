using System;
using System.Collections;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public class FadeScreen : MonoBehaviour
{
    public static FadeScreen Instance;

    [SerializeField] private Canvas FadeCanvas;
    [SerializeField] private Image FadeImage;
    [SerializeField] private Image _whiteFadeImage;

    [SerializeField] private Canvas SummonCanvas;
    [SerializeField] private Image SummonFadeImage;

    [SerializeField] private GameObject LoadingObj;
    [SerializeField] private GameObject LoadingIconObj;

    private Camera _gameCamera;
    private Camera _uiCamera;

    public Tween CurrentTween;

    public Sequence Sequence;

    public Sequence SummonSequence;

    public CompositeDisposable CompositeDisposable = new();

    private void Awake()
    {
        Instance = this;
    }

    public void SetCamera()
    {
        _gameCamera = Camera.main;
        _uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
    }

    public static void FadeOut(Action callback = null, float fadeTime = 1f, float callbackDelay = 0)
    {
        if (Instance.Sequence != null)
        {
            Instance.Sequence.Kill();
            Instance.Sequence.onComplete = null;
        }
        
        Instance.CompositeDisposable.Clear();

        Instance.FadeCanvas.worldCamera = Instance._uiCamera;
        Instance.FadeCanvas.sortingLayerName = "UI";
        Instance.FadeCanvas.sortingOrder = 200;
        
        Instance.Sequence = DOTween.Sequence().Append(Instance.FadeImage.DOFade(1f, fadeTime));
        Instance.Sequence.onComplete = () =>
        {
            Managers.Sound.StopAllEffectSound();
            Managers.DamageText.ClearDamageText();
            Instance.CurrentTween = null;
            Observable.Timer(TimeSpan.FromSeconds(callbackDelay)).Subscribe(_ =>
            {
                callback?.Invoke();
            });
        };
    }

    public static void WhiteFadeOut(Action callback = null, float fadeTime = 1f, float callbackDelay = 0)
    {
        if (Instance.Sequence != null)
        {
            Instance.Sequence.Kill();
            Instance.Sequence.onComplete = null;
        }

        Instance.CompositeDisposable.Clear();

        Instance.FadeCanvas.worldCamera = Instance._uiCamera;
        Instance.FadeCanvas.sortingLayerName = "UI";
        Instance.FadeCanvas.sortingOrder = 200;

        Instance.Sequence = DOTween.Sequence().Append(Instance._whiteFadeImage.DOFade(1f, fadeTime));
        Instance.Sequence.onComplete = () =>
        {
            Managers.Sound.StopAllEffectSound();
            Managers.DamageText.ClearDamageText();
            Instance.CurrentTween = null;
            Observable.Timer(TimeSpan.FromSeconds(callbackDelay)).Subscribe(_ =>
            {
                callback?.Invoke();
            });
        };
    }

    public static void FadeIn(Action callback = null, float fadeTime = 1f)
    {
        if (Instance.Sequence != null)
        {
            Instance.Sequence.onComplete = null;
            Instance.CompositeDisposable.Clear();
        }

        Instance.FadeCanvas.worldCamera = Instance._uiCamera;
        Instance.Sequence = DOTween.Sequence().Append(Instance.FadeImage.DOFade(0f, fadeTime));
        Instance.Sequence.onComplete = () =>
        {
            Instance.CurrentTween = null;
            callback?.Invoke();
            if (Managers.Stage.State.Value == StageState.Normal)
                Managers.Stage.CheckBoss();
        };
    }

    public static void WhiteFadeIn(Action callback = null, float fadeTime = 1f)
    {
        if (Instance.Sequence != null)
        {
            Instance.Sequence.onComplete = null;
            Instance.CompositeDisposable.Clear();
        }

        Instance.FadeCanvas.worldCamera = Instance._uiCamera;
        Instance.Sequence = DOTween.Sequence().Append(Instance._whiteFadeImage.DOFade(0f, fadeTime));
        Instance.Sequence.onComplete = () =>
        {
            Instance.CurrentTween = null;
            callback?.Invoke();
            if (Managers.Stage.State.Value == StageState.Normal)
                Managers.Stage.CheckBoss();
        };
    }

    public static void GameViewFadeOut(Action callback = null, float fadeTime = 1f)
    {
        if (Instance.Sequence != null)
        {
            Instance.Sequence.Kill();
            Instance.Sequence.onComplete = null;
            Instance.CompositeDisposable.Clear();
        }

        Instance.FadeCanvas.worldCamera = Instance._gameCamera;
        Instance.FadeCanvas.sortingLayerName = "GameViewFade";
        Instance.FadeCanvas.sortingOrder = 200;
        
        Instance.Sequence = DOTween.Sequence().Append(Instance.FadeImage.DOFade(1f, fadeTime));
        Instance.Sequence.onComplete = () =>
        {
            Managers.Sound.StopAllEffectSound();
            Managers.DamageText.ClearDamageText();
            Instance.CurrentTween = null;
            callback?.Invoke();
        };
    }

    public static void GameViewFadeIn(Action callback = null, float fadeTime = 1f)
    {
        if (Instance.Sequence != null)
        {
            Instance.Sequence.onComplete = null;
            Instance.CompositeDisposable.Clear();
        }

        Instance.Sequence = DOTween.Sequence().Append(Instance.FadeImage.DOFade(0f, fadeTime));
        Instance.Sequence.onComplete = () =>
        {
            Managers.DamageText.ClearDamageText();
            Instance.CurrentTween = null;
            callback?.Invoke();
            if (Managers.Stage.State.Value == StageState.Normal)
                Managers.Stage.CheckBoss();
        };
    }

    public static void SummonFadeOut(Action callback = null, float fadeTime = 1f)
    {
        Instance.SummonCanvas.worldCamera = Instance._uiCamera;
        Instance.SummonSequence = DOTween.Sequence().Append(Instance.SummonFadeImage.DOFade(1f, fadeTime));
        Instance.SummonSequence.onComplete = () =>
        {
            Instance.CurrentTween = null;
            callback?.Invoke();
        };
    }

    public static void SummonFadeIn(Action callback = null, float fadeTime = 1f)
    {
        Instance.SummonCanvas.worldCamera = Instance._uiCamera;

        Instance.SummonSequence = DOTween.Sequence().Append(Instance.SummonFadeImage.DOFade(0f, fadeTime));
        Instance.SummonSequence .onComplete = () =>
        {
            Instance.CurrentTween = null;
            callback?.Invoke();
        };
    }

    public static void StopSummonFade()
    {
        if(Instance.SummonSequence != null)
            Instance.SummonSequence.Kill();

        Instance.SummonFadeImage.DOFade(0f, 0f);
    }

    public void OnLoadingScreen()
    {
        Instance.LoadingObj.SetActive(true);
        Instance.LoadingIconObj.transform.DOLocalRotate(new Vector3(0, 0, -360), 0.8f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental);
    }

    public void OffLoadingScreen()
    {
        LoadingObj.SetActive(false);
    }
}