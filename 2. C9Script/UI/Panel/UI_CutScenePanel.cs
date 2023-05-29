using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_CutScenePanel : UI_Panel
{
    [SerializeField] private GameObject[] CutSceneObjs;
    [SerializeField] private GameObject NextObj;
    [SerializeField] private Button SkipButton;

    private int _currentCutSceneIndex;

    private void Start()
    {
        NextObj.BindEvent(NextCutScene);
        SkipButton.BindEvent(OnClickSkip);

        for (int i = 0; i < CutSceneObjs.Length; i++)
        {
            if (_currentCutSceneIndex == i)
                CutSceneObjs[i].SetActive(true);
            else
                CutSceneObjs[i].SetActive(false);
        }
    }

    private void NextCutScene()
    {
        if (_currentCutSceneIndex + 1 >= CutSceneObjs.Length)
        {
            FadeScreen.FadeOut(() =>
            {
                Managers.GameData.LoadComplete();
            });
            
            return;
        }

        CutSceneObjs[_currentCutSceneIndex].SetActive(false);
        _currentCutSceneIndex += 1;
        CutSceneObjs[_currentCutSceneIndex].SetActive(true);
    }

    private void OnClickSkip()
    {
        FadeScreen.FadeOut(() =>
        {
            Managers.GameData.LoadComplete();
        });
    }
}