using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ColorTweener : MonoBehaviour
{
    public enum eTWEENTYPE
    {
        SpriteRenderer,
        Image,
        Text,
        TMP,
    }

    #region Fields
    [SerializeField] private AnimationCurve _animCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public delegate void TweenDel();
    private SpriteRenderer _spriteRenderer;
    private Image _image;
    private Text _text;
    private TextMeshProUGUI _textPro;

    public Color From;
    public Color To;

    [SerializeField] private eTWEENTYPE _type = eTWEENTYPE.SpriteRenderer;

    public bool StartOnEnable = false;
    public bool IsLoop = false;
    public float Duration = 1f;
    private float _time;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (_type == eTWEENTYPE.SpriteRenderer)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        else if (_type == eTWEENTYPE.Image)
            _image = GetComponent<Image>();
        else if (_type == eTWEENTYPE.Text)
            _text = GetComponent<Text>();
        else if (_type == eTWEENTYPE.TMP)
            _textPro = GetComponent<TextMeshProUGUI>();
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
            var result = Color.Lerp(From, To, _animCurve.Evaluate(_time));

            switch (_type)
            {
                case eTWEENTYPE.SpriteRenderer:
                    _spriteRenderer.color = result;
                    break;
                case eTWEENTYPE.Image:
                    _image.color = result;
                    break;
                case eTWEENTYPE.Text:
                    _text.color = result;
                    break;
                case eTWEENTYPE.TMP:
                    _textPro.color = result;
                    break;
                default:
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
