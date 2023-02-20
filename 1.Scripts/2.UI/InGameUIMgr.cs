using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIMgr : SingletonMonoBehaviour<InGameUIMgr>
{
    [SerializeField] public GameUI _gameUI;
    public GameObject _joyStick;
}
