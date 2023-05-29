using UniRx;
using UnityEngine;

namespace GameData.Data
{
    public class SettingData
    {
        public ReactiveProperty<bool> Bgm = new(true);
        public ReactiveProperty<bool> Sfx = new(true);
        public ReactiveProperty<bool> PowerSave = new(true);
        public ReactiveProperty<bool> DamageText = new(true);
        public ReactiveProperty<bool> Push = new(true);
        public ReactiveProperty<bool> SkillEffect = new(true);

        public void Init()
        {
            int bgm =PlayerPrefs.GetInt("Bgm", 1);
            Bgm.Value = bgm == 1;
            Bgm.Subscribe(bgm => { PlayerPrefs.SetInt("Bgm", bgm ? 1 : 0); });
            
            int sfx =PlayerPrefs.GetInt("Sfx", 1);
            Sfx.Value = sfx == 1;
            Sfx.Subscribe(sfx => { PlayerPrefs.SetInt("Sfx", sfx ? 1 : 0); });
            
            int powerSave =PlayerPrefs.GetInt("PowerSave", 1);
            PowerSave.Value = powerSave == 1;
            PowerSave.Subscribe(powerSave => { PlayerPrefs.SetInt("PowerSave", powerSave ? 1 : 0); });
            
            int damageText =PlayerPrefs.GetInt("DamageText", 1);
            DamageText.Value = damageText == 1;
            DamageText.Subscribe(damageText => { PlayerPrefs.SetInt("DamageText", damageText ? 1 : 0); });
            
            int push = PlayerPrefs.GetInt("Push", 1);
            Push.Value = push == 1;
            Push.Subscribe(push => { PlayerPrefs.SetInt("Push", push ? 1 : 0); });

            int skillEffect = PlayerPrefs.GetInt("SkillEffect", 1);
            SkillEffect.Value = skillEffect == 1;
            SkillEffect.Subscribe(skillEffect => { PlayerPrefs.SetInt("SkillEffect", skillEffect ? 1 : 0); });
        }

        public void Save()
        {
            PlayerPrefs.SetInt("Bgm", Bgm.Value ? 1 : 0);
            PlayerPrefs.SetInt("Sfx", Sfx.Value ? 1 : 0);
            PlayerPrefs.SetInt("PowerSave", PowerSave.Value ? 1 : 0);
            PlayerPrefs.SetInt("DamageText", DamageText.Value ? 1 : 0);
            PlayerPrefs.SetInt("Push", Push.Value ? 1 : 0);
            PlayerPrefs.SetInt("SkillEffect", SkillEffect.Value ? 1 : 0);
        }
    }
}