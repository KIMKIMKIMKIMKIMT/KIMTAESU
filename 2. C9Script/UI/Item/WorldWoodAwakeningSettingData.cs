using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace GameData.Data
{
    public class WorldWoodAwakeningSettingData
    {
        public ReactiveProperty<int> StatId = new(-1);
        public ReactiveProperty<int> Grade = new(-1);

        public void Init()
        {
            StatId.Value = PlayerPrefs.GetInt("WoodAwakeningSetting_Stat", -1);
            Grade.Value = PlayerPrefs.GetInt("WoodAwakeningSetting_Grade", -1);
        }

        public void Save()
        {
            PlayerPrefs.SetInt("WoodAwakeningSetting_Stat", StatId.Value);
            PlayerPrefs.SetInt("WoodAwakeningSetting_Grade", Grade.Value);
        }
    }
}

