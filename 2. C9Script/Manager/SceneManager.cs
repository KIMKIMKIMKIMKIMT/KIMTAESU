using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Title,
    Game,
}

public class SceneManager
{
    private SceneType _currentScene = SceneType.Title;

    public void Init()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoad;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        switch (_currentScene)
        {
            case SceneType.Game:
            {
                FadeScreen.FadeOut(null, 0f);
                Managers.Guild.Init();
                Managers.UI.Clear();
                Managers.Game.Init();
                Managers.Rank.Init();
                Managers.Post.Init();
                Managers.Stage.Init();
                Managers.Dungeon.Init();
                Managers.DamageText.Init();
                FadeScreen.Instance.SetCamera();
                Managers.Raid.Init();
                Managers.GuildRaid.Init();
                Managers.XMasEvent.Init();
                Managers.Backend.SetNotificationHandler();
                Managers.CostumeSet.CostumeCollectActive();

                    FadeScreen.FadeIn(() =>
                {
                    Managers.Game.MainPlayer.State.Value = CharacterState.Idle;
                });
            }
                break;
        }
    }
    

    public void ChangeScene(SceneType sceneType)
    {
        _currentScene = sceneType;
        MainThreadDispatcher.StartCoroutine(LoadScene(sceneType));
    }

    public string GetSceneName(SceneType sceneType)
    {
        return $"{Enum.GetName(typeof(SceneType), sceneType)}Scene";
    }

    public IEnumerator LoadScene(SceneType sceneType)
    {
        var asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GetSceneName(sceneType));
        asyncOperation.allowSceneActivation = false;
        
        while (!asyncOperation.isDone)
        {
            yield return null;
            if (asyncOperation.progress < 0.9f)
            {
                
            }
            else
            {
                asyncOperation.allowSceneActivation = true;
            }
            
        }
    }
}
