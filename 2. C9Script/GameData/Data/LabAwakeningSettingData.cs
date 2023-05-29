using UniRx;
using UnityEngine;

namespace GameData.Data
{
    public class LabAwakeningSettingData
    {
        public ReactiveProperty<int> PatternId = new(-1);
        public ReactiveProperty<int> StatId = new(-1);
        public ReactiveProperty<int> Grade = new(-1);

        public void Init()
        {
            PatternId.Value = PlayerPrefs.GetInt("LabAwakeningSetting_Pattern", -1);
            StatId.Value = PlayerPrefs.GetInt("LabAwakeningSetting_Stat", -1);
            Grade.Value = PlayerPrefs.GetInt("LabAwakeningSetting_Grade", -1);
        }

        public void Save()
        {
            PlayerPrefs.SetInt("LabAwakeningSetting_Pattern", PatternId.Value);
            PlayerPrefs.SetInt("LabAwakeningSetting_Stat", StatId.Value);
            PlayerPrefs.SetInt("LabAwakeningSetting_Grade", Grade.Value);
        }
    }
}