using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DmgTxt : MonoBehaviour
{
    #region Fields
    private Text _txt;
    private UI_Tweener _tween;

    [SerializeField] private Color[] _txtcolors;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _txt = GetComponent<Text>();
        _tween = GetComponent<UI_Tweener>();
    }
    #endregion

    #region Public Methods
    public void SetText(int value, Vector2 position)
    {
        _txt.color = GetValueToColor(value);
        _txt.text = value.ToString();
        position *= 100;
        transform.position = position;
        _tween.From = position;
        _tween.To = position + new Vector2(0, 60f);
        _tween.StartTween(() => { Invoke("ActiveOff", 0.5f); });
    }
    #endregion

    #region Private Methods
    private void ActiveOff()
    {
        gameObject.SetActive(false);
    }

    private Color GetValueToColor(int value)
    {
        if (value < 5000)
        {
            return _txtcolors[0];
        }
        else if (value < 10000)
        {
            return _txtcolors[1];
        }
        else
        {
            return _txtcolors[2];
        }
    }
    #endregion
}
