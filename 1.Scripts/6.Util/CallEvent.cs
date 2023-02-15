using UnityEngine;
using UnityEngine.Events;

public class CallEvent : MonoBehaviour
{
    public UnityEvent Event;

    public void Call()
    {
        Event.Invoke();
    }
}
