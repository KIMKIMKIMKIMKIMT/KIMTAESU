using System;
using System.Collections;
using BackEnd;
using CodeStage.AntiCheat.Genuine.CodeHash;
using UniRx;
using UnityEngine;

public class AntiCheatManager : MonoBehaviour
{
    public static AntiCheatManager Instance;

    private bool _cheatDetected;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(CoSetRandomizeCryptoKey());
    }

    private IEnumerator CoSetRandomizeCryptoKey()
    {
        var delay = new WaitForSeconds(3f);

        while (true)
        {
            yield return delay;
            
            foreach (var goodsData in Managers.Game.GoodsDatas)
                goodsData.Value.Value.RandomizeCryptoKey();
        }
    }

    public void OnInjectionDetected(string reason)
    {
#if DEV
        UnityEngine.Debug.LogError("InjectionDetected");

#endif

        if (Application.isEditor)
            return;

        var param = new Param
        {
            { "CheatType", "Injection" },
            { "Reason", reason }
        };
        
        if (Managers.Manager.ProjectType == ProjectType.Dev)
            return;

        GameDataManager.GetAllDataLog(ref param);

        Backend.GameLog.InsertLog("CheatLog", param, bro =>
        {
            if (!bro.IsSuccess())
            {
                Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => { OnInjectionDetected(reason); });
            }
        });
    }

    public void OnCheatDetected()
    {
#if DEV
        UnityEngine.Debug.LogError("MemoryModulationDetected");

#endif
        
        if (Application.isEditor)
            return;
        
        if (Managers.Manager.ProjectType == ProjectType.Dev)
            return;

        if (_cheatDetected)
            return;

        var param = new Param
        {
            { "CheatType", "MemoryModulation" }
        };

        GameDataManager.GetAllDataLog(ref param);

        Backend.GameLog.InsertLog("CheatLog", param, bro =>
        {
            if (!bro.IsSuccess())
            {
                Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => { OnCheatDetected(); });
            }

            _cheatDetected = true;
            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => _cheatDetected = false);
        });
    }

    public void OnSpeedCheatDetected()
    {
#if DEV
        UnityEngine.Debug.LogError("SpeedDetected");

#endif

        if (Managers.Manager.ProjectType == ProjectType.Dev)
            return;

        if (Application.isEditor)
            return;
        
        var param = new Param
        {
            { "CheatType", "Speed" },
            { "Speed", Time.timeScale }
        };

        GameDataManager.GetAllDataLog(ref param);

        Backend.GameLog.InsertLog("CheatLog", param, bro =>
        {
            if (!bro.IsSuccess())
            {
                Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => { OnSpeedCheatDetected(); });
            }
        });
    }
}