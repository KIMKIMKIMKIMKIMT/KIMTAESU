using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RankData
{
    
    public string UserNickname;
    public float Time;

    public RankData(string nickname = "", float time = 0)
    {
        UserNickname = nickname;
        Time = time;
    }
}
