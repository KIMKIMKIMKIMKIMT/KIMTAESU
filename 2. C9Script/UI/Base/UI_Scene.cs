using System;

namespace UI
{
    public class UI_Scene : UI_Base
    {
        private void Awake()
        {
            Managers.UI.SetCanvas(this, false);
            Managers.UI.SetScene(this);
        }
    }
}