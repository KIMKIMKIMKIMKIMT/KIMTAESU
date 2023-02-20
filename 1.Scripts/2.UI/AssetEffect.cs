using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetEffect : MonoBehaviour
{
    #region Fields
    [SerializeField] private Image _icon;
    [SerializeField] private Tweener _tweener;

    private Vector3 _final;
    #endregion

    #region Public Methods
    public void SetIcon(Sprite sprite)
    {
        _icon.sprite = sprite;
    }

    public void SetTweenPosition(Vector3 final)
    {
        _final = final;
        Vector3 pos = Camera.main.WorldToScreenPoint(Vector3.zero);
        _tweener.Duration = Random.Range(0.1f, 0.5f);
        _tweener.From = pos;
        _tweener.To = pos + (Vector3)Random.insideUnitCircle * 200;
        _tweener.StartTween(() =>
        {
            Invoke("MoveToFinal", 0.25f);
        });
    }

    public void MoveToFinal()
    {
        _tweener.Duration = Random.Range(0.1f, 0.5f);
        _tweener.From = _tweener.To;
        _tweener.To = _final;
        _tweener.StartTween(() => { gameObject.SetActive(false); });
    }
    #endregion
}
