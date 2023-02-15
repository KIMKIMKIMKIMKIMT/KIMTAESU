using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeMgr : DontDestroy<FadeMgr>
{
    #region Fields
    public delegate void FadeDel();

    [SerializeField] private Canvas _fadeCanvas;
    [SerializeField] private Image _fadeImg;
    #endregion

    #region Unity Methods
    #endregion

    #region Coroutines
    private IEnumerator Cor_Fade(FadeDel start, FadeDel interval, FadeDel end)
    {
        _fadeCanvas.gameObject.SetActive(true);
        _fadeImg.gameObject.SetActive(true);
        if (start != null)
        {
            start();
            start = null;
        }
        float alpha = 0;
        _fadeImg.color = new Color(0, 0, 0, 0);
        while (true)
        {
            yield return null;
            alpha += Time.deltaTime * 5f;
            _fadeImg.color = new Color(0, 0, 0, alpha);
            if (alpha >= 1)
            {
                alpha = 1;
                _fadeImg.color = new Color(0, 0, 0, alpha);
                break;
            }
        }

        if (interval != null)
        {
            interval();
            interval = null;
        }

        yield return new WaitForSeconds(0.3f);

        while (true)
        {
            yield return null;
            alpha -= Time.deltaTime * 5f;
            _fadeImg.color = new Color(0, 0, 0, alpha);
            if (alpha <= 0)
            {
                alpha = 0;
                _fadeImg.color = new Color(0, 0, 0, alpha);
                break;
            }
        }

        if (end != null)
        {
            end();
            end = null;
        }

        _fadeCanvas.gameObject.SetActive(false);
        _fadeImg.gameObject.SetActive(false);
        yield break;
    }
    #endregion

    #region Public Methods
    public void FadeOn(FadeDel start, FadeDel interval, FadeDel end)
    {
        StopAllCoroutines();
        StartCoroutine(Cor_Fade(start, interval, end));
    }
    #endregion
}
