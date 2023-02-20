using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum eSCENESTATE
{
    None = -1,
    
    Main,
    Game,

    Max
}

public class LoadSceneMgr : DontDestroy<LoadSceneMgr>
{
    #region Fields
    public delegate void SceneChangeDel();

    private AsyncOperation _LoadingInfo;

    private eSCENESTATE _sceneState;
    public eSCENESTATE SceneState
    {
        get { return _sceneState; }
    }
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
    }
    #endregion

    #region Coroutines
    IEnumerator Cor_LoadSceneProcess(eSCENESTATE state, FadeMgr.FadeDel start = null, FadeMgr.FadeDel interval = null, FadeMgr.FadeDel end = null)
    {
        _LoadingInfo = SceneManager.LoadSceneAsync((int)state);
        _LoadingInfo.allowSceneActivation = false;
        _sceneState = state;

        while (!_LoadingInfo.isDone)
        {
            if (_LoadingInfo.progress >= 0.9f)
            {
                FadeMgr.Instance.FadeOn(start, interval += () => { _LoadingInfo.allowSceneActivation = true; }, end);
                yield break;
            }
            yield return null;
        }
    }
    #endregion

    #region Public Methods
    public void LoadSceneAsync(eSCENESTATE state, FadeMgr.FadeDel start = null, FadeMgr.FadeDel interval = null, FadeMgr.FadeDel end = null)
    {
        StartCoroutine(Cor_LoadSceneProcess(state, start, interval, end));
    }
    #endregion
}
