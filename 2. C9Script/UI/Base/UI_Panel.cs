
using UniRx;

namespace UI
{
    public abstract class UI_Panel : UI_Base
    {
        public override void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}