using System;
using UnityEngine;

public class RaidPortalSpawner : MonoBehaviour
{
    private Action _createCallback;
    public void Init(Action createCallback)
    {
        _createCallback = createCallback;
    }
}