using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterMgr : SingletonMonoBehaviour<ChapterMgr>
{
    #region Fields
    [SerializeField] private Chapter1 _chapter1;
    [SerializeField] private Chapter2 _chapter2;
    [SerializeField] private Chapter3 _chapter3;
    [SerializeField] private ChapterSurvival _chapterSurvival;

    private int _chapterIndex;
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        _chapterIndex = GameMgr.Instance.Chapter;
    }

    private void Start()
    {
        ChapterActive(_chapterIndex);
    }
    #endregion

    #region Public Methods
    public void ChapterActive(int index)
    {
        switch (index)
        {
            case 0:
                _chapter1.gameObject.SetActive(true);
                break;
            case 1:
                _chapter2.gameObject.SetActive(true);
                break;
            case 2:
                _chapter3.gameObject.SetActive(true);
                break;
            case 3:
                _chapterSurvival.gameObject.SetActive(true);
                break;
        }
    }
    #endregion
}
