using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Tweener : MonoBehaviour
{
    public enum eTWEENTYPE
    {
        RectTransform,
        AnchoredPosition,
        AnchoredPosition3D,
        Scale,
    }

    #region Fields
    [SerializeField] private AnimationCurve _animCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public delegate void TweenDel();
    private RectTransform _rectTransform;

    public Vector3 From;
    public Vector3 To;

    [SerializeField] private eTWEENTYPE _type = eTWEENTYPE.RectTransform;

    public bool StartOnEnable = false;
    public bool IsLoop = false;
    public float Duration = 1f;
    private float _time;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (StartOnEnable)
        {
            StartTween();
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    #endregion

    #region Private Methods
    private IEnumerator Cor_MoveProcess(TweenDel del = null)
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        while (true)
        {
            _time += Time.deltaTime / Duration;
            var result = Vector3.Lerp(From, To, _animCurve.Evaluate(_time));

            switch (_type)
            {
                case eTWEENTYPE.RectTransform:
                    _rectTransform.localPosition = result;
                    break;

                case eTWEENTYPE.AnchoredPosition:
                    _rectTransform.anchoredPosition = result;
                    break;

                case eTWEENTYPE.AnchoredPosition3D:
                    _rectTransform.anchoredPosition3D = result;
                    break;

                case eTWEENTYPE.Scale:
                    _rectTransform.localScale = result;
                    break;
            }
            if (_time >= 1f)
            {
                if (del != null)
                {
                    del();
                    del = null;
                }
                if (IsLoop)
                {
                    StartTween(del);
                    yield break;
                }
                yield break;
            }
            yield return null;
        }
    }
    #endregion

    #region Public Methods
    public void StartTween(TweenDel del = null)
    {
        _time = 0f;
        StopAllCoroutines();
        StartCoroutine(Cor_MoveProcess(del));
    }
    #endregion
}
