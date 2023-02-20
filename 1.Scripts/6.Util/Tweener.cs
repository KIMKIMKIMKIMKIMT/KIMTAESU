using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    public enum eTWEENTYPE
    {
        Transform,
        LocalTransform,
        Scale,
    }

    #region Fields
    [SerializeField] private AnimationCurve _animCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public delegate void TweenDel();
    [SerializeField] private Transform _transform;

    public Vector3 From;
    public Vector3 To;

    [SerializeField] private eTWEENTYPE _type = eTWEENTYPE.Transform;

    public bool StartOnEnable = false;
    public bool IsLoop = false;
    public float Duration = 1f;
    private float _time;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _transform = transform;
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
    private IEnumerator Cor_MoveProcess(TweenDel del)
    {
        while (true)
        {
            _time += Time.deltaTime / Duration;
            var result = Vector3.Lerp(From, To, _animCurve.Evaluate(_time));
            
            switch (_type)
            {
                case eTWEENTYPE.Transform:
                    _transform.position = result;
                    break;

                case eTWEENTYPE.LocalTransform:
                    _transform.localPosition = result;
                    break;

                case eTWEENTYPE.Scale:
                    _transform.localScale = result;
                    break;
            }
            if (_time >= 1f)
            {
                if (del != null)
                {
                    del();
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
