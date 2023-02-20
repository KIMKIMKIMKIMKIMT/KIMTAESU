using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PopupMgr : DontDestroy<PopupMgr>
{
    #region Fields
    [SerializeField] private Stack<GameObject> _popups;
    [SerializeField] private OkPopup _okPopup;
    [SerializeField] private OkCancelPopup _okCancelPopup;
    public bool _bPossibleClickBackBtn;

    public int popupsCount
    {
        get { return _popups.Count; }
    }

    public bool IsStackClean
    {
        get
        {
            if (_popups.Count > 0) return false;
            else
                return true;
        }

    }
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        _popups = new Stack<GameObject>();
        _bPossibleClickBackBtn = true;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _popups.Clear();
    }

    private void Update()
    {
        if (_bPossibleClickBackBtn)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (IsStackClean)
                {
                    if (LoadSceneMgr.Instance.SceneState == eSCENESTATE.Main)
                    {
                        ShowOkCancelPopup("알림", "게임을 종료합니다.", () =>
                        {
#if UNITY_EDITOR
                            EditorApplication.isPlaying = false;
#else
                            Application.Quit();       
#endif
                        });
                    }
                }
                else
                {
                    RemovePopup();
                }
            }
        }
    }

    
#endregion

    #region Public Methods
    public void AddPopup(GameObject popup)
    {
        if (_popups.Contains(popup))
        {
            return;
        }

        _popups.Push(popup);
        popup.SetActive(true);
    }
    public void RemovePopup()
    {
        GameObject obj;
        if (_popups.Count > 0)
        {
            obj = _popups.Pop();
            obj.SetActive(false);
        }
    }



    public void ShowOkPopup(string title, string body, OkPopup.OkDel del = null)
    {
        AddPopup(_okPopup.gameObject);
        _okPopup.SetUI(title, body, del);
    }

    public void ShowOkCancelPopup(string title, string body, OkCancelPopup.OkDel del = null)
    { 
        AddPopup(_okCancelPopup.gameObject);
        _okCancelPopup.SetUI(title, body, del);
    }

    #endregion
}
