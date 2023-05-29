using System;
using BackEnd;
using UnityEngine;

namespace Login
{
    public class GuestConnector : Connector
    {
        [SerializeField] private bool IsGuestDelete;

        private void Start()
        {
            Debug.Log($"Guest ID : {Backend.BMember.GetGuestID()}");
            
            if (IsGuestDelete)
                Backend.BMember.DeleteGuestInfo();
        }

        public override LoginType LoginType => LoginType.Guest;
        public override void Login()
        {
            if (Managers.Manager.ProjectType == ProjectType.Dev && Application.platform == RuntimePlatform.Android)
            {
                var popup = Managers.UI.ShowPopupUI<UI_CustomLoginPopup>();
                popup.OnSuccessLoginCallback = OnSuccessLoginCallback;
            }
            else
            {
                Backend.BMember.GuestLogin("GUEST", bro =>
                {
                    if (!bro.IsSuccess())
                    {
                        Managers.Backend.FailLog("Fail Guest Login", bro);
                        OnFailLoginCallback?.Invoke();
                        return;
                    }

                    Managers.Server.UserId = Backend.BMember.GetGuestID();

                    OnSuccessLoginCallback?.Invoke();
                });
            }
        }
    }
}