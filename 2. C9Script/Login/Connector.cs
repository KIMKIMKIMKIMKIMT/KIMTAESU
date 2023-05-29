using System;
using UnityEngine;

public abstract class Connector : MonoBehaviour
{
    public abstract LoginType LoginType { get; }
    public bool IsInit { get; set; }
    public Action OnSuccessLoginCallback { get; set; }
    public Action OnFailLoginCallback { get; set; }
    public abstract void Login();
}