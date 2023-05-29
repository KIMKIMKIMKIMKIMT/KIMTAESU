using System;
using System.Threading.Tasks;
using BackEnd;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using UnityEngine;

[Serializable]
public class GoogleConnector : Connector
{
    [SerializeField] private string _webClientId_AOS;

    private FirebaseApp _app;
    private FirebaseAuth _auth;
    public override LoginType LoginType => LoginType.Google;

    private Task<GoogleSignInUser> _signIn;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var status = task.Result;

            if (status == DependencyStatus.Available)
            {
                _app = FirebaseApp.DefaultInstance;
                _auth = FirebaseAuth.DefaultInstance;

                FirebaseApp.LogLevel = LogLevel.Debug;

                IsInit = true;
            }
        });
    }

    public override void Login()
    {
        if (Application.isEditor)
        {
            var popup = Managers.UI.ShowPopupUI<UI_CustomLoginPopup>();
            popup.OnSuccessLoginCallback = OnSuccessLoginCallback;
            return;
        }

        Debug.Log("Success Firebase Init");

        if (GoogleSignIn.Configuration == null)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration()
            {
                RequestIdToken = true,
                RequestEmail = true,
                WebClientId = _webClientId_AOS
            };
        }

        _signIn = GoogleSignIn.DefaultInstance.SignIn();

        var signinCompleted = new TaskCompletionSource<FirebaseUser>();

        Debug.Log("Start Google SignIn");

        _signIn.ContinueWithOnMainThread(task =>
        {
            Debug.Log("SignIn Result");

            if (task.IsCanceled)
            {
                Debug.Log("Cancel Google Login");
                signinCompleted.SetCanceled();
                OnFailLoginCallback?.Invoke();
                return;
            }

            if (task.IsFaulted)
            {
                if (task.Exception == null)
                    return;
                Debug.Log($"Faulted Google Login : {task.Exception.InnerException?.Message}");
                OnFailLoginCallback?.Invoke();
                return;
            }

            Debug.Log("Success Google SignIn");
            var credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
            
            Debug.Log($"Google UserID : {task.Result.UserId}");

            Managers.Server.UserId = task.Result.Email;

            _auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(authTask =>
            {
                if (authTask.IsCanceled)
                {
                    Debug.Log("authTask Cancel");
                    OnFailLoginCallback?.Invoke();
                    return;
                }

                if (authTask.IsFaulted)
                {
                    if (authTask.Exception != null)
                        Debug.Log($"authTask Faulted : {authTask.Exception.Message}");
                    OnFailLoginCallback?.Invoke();
                    return;
                }
                
                Debug.Log($"Auth UserID : {_auth.CurrentUser.UserId}");

                Debug.Log("authTask Success");

                if (string.IsNullOrEmpty(Backend.UserInDate))
                    BackendLogin(task.Result.IdToken);
                else
                {
                    InAppActivity.SendEvent("login");
                    OnSuccessLoginCallback?.Invoke();
                }
            });
        });
    }

    private void BackendLogin(string idToken)
    {
        string etcData = string.Empty;

        switch (Managers.Manager.StoreType)
        {
            case StoreType.GoogleStore:
                etcData = "GOOGLE";
                break;
            case StoreType.OneStore:
                etcData = "OneStore";
                break;
        }

        Backend.BMember.AuthorizeFederation(idToken, FederationType.Google, etcData, bro =>
        {
            Managers.Server.IdToken = idToken;
            Debug.Log("Google Check - 6");
            if (!bro.IsSuccess())
            {
                Managers.Backend.FailLog("Fail Google Federation", bro);
                OnFailLoginCallback?.Invoke();
                return;
            }

            var status = bro.GetStatusCode();

            bool isSuccess = false;

            switch (status)
            {
                case "200":
                {
                    isSuccess = true;
                }
                    break;
                case "201":
                {
                    isSuccess = true;
                }
                    break;
            }

            // Where where = new Where();
            // where.Equal("owner_inDate", Backend.UserInDate);
            //
            // Backend.GameData.Get("Cheat", where, bro =>
            // {
            //     if (!bro.IsSuccess())
            //     {
            //         Managers.Backend.FailLog("Fail Google Federation", bro);
            //         OnFailLoginCallback?.Invoke();
            //         return;
            //     }
            // });

            InAppActivity.SendEvent("login");

            if (isSuccess)
            {
                PlayerPrefs.SetInt("AutoLogin", 1);
                OnSuccessLoginCallback?.Invoke();
            }
        });
    }

    private void OnApplicationQuit()
    {
        if (_app != null)
            FirebaseApp.DefaultInstance.Dispose();

        if (_auth != null)
            FirebaseAuth.DefaultInstance.Dispose();
    }
}