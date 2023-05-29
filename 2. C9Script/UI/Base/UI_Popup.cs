using System;

namespace UI
{
    public class UI_Popup : UI_Base
    {
        public override void Open()
        {
            Managers.UI.SetCanvas(this);
        }

        protected void ClosePopup()
        {
            Managers.UI.ClosePopupUI(this);
        }
    }
}