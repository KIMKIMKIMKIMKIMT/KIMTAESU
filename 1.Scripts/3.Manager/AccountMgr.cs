using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Auth;
using Firebase.Extensions;

public class AccountMgr : DontDestroy<AccountMgr>
{
    #region Fields
    private FirebaseAuth _auth;

    public string UID { get { return _auth.CurrentUser.UserId; } }
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        _auth = FirebaseAuth.DefaultInstance;
    }
    private void Start()
    {
#if UNITY_EDITOR

#else
        GPGSLogin();
#endif
    }
    #endregion

    #region Public Methods
    public void GPGSLogin()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .RequestIdToken()
            .RequestEmail()
            .RequestServerAuthCode(false)
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                FireBaseLogin();
            }
            else
            {
                //todo okpopup
            }
        });
    }
    public async void FireBaseLogin()
    {
        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
        Credential credential = GoogleAuthProvider.GetCredential(idToken, "");

        bool success = false;

        await _auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    success = true;
                    FirebaseUser user = task.Result;
                }
            }
        });

        if (success)
        {
            PlayerDataMgr.Instance.GetPlayerData();
        }
    }
    public async void EmailLogin(string email)
    {
        bool isSuccess = true;
        await _auth.SignInWithEmailAndPasswordAsync(email, "123123").ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                try
                {
                    FirebaseUser newuser = task.Result;
                }
                catch (System.Exception ex)
                {
                    isSuccess = false;
                    CreateEmailAuth(email);
                }
            }
        });
        if (isSuccess)
        {
            PlayerDataMgr.Instance.GetPlayerData();
        }
    }
    public async void CreateEmailAuth(string email)
    {
        bool isSuccess = false;
        await _auth.CreateUserWithEmailAndPasswordAsync(email, "123123").ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    isSuccess = true;
                    FirebaseUser user = task.Result;
                }
            }
        });
        if (isSuccess)
        {
            EmailLogin(email);
        }
    }
    #endregion
}
