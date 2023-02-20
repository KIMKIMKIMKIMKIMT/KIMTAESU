using UnityEngine;
using UnityEngine.Events;

public class CallMultiEvent : MonoBehaviour
{
    public UnityEvent[] Event;

    public void Call(int index)
    {
        Event[index].Invoke();
    }
}
