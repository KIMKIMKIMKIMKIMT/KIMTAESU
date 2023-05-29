using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ClipperLib;
using DefaultNamespace;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using Util;

public class SoundManager
{
    private Transform _root;

    public Transform Root
    {
        get
        {
            if (_root != null)
                return _root;

            GameObject obj = new GameObject { name = "@Sound" };
            _root = obj.transform;
            Object.DontDestroyOnLoad(_root);
            return _root;
        }
    }

    private SoundData _soundData;

    private AudioSource _bgmSource;
    private ObjectPool<EffectSound> _effectSoundPool;

    private readonly List<EffectSound> _playingEffectSource = new();

    public void Init()
    {
        _soundData = Managers.Resource.Load<SoundData>("SoundData");
        
        _bgmSource = CreateBgmSound();
        _bgmSource.loop = true;
        _effectSoundPool = new ObjectPool<EffectSound>(
            CreateEffectSound,
            EffectSound =>
            {
                EffectSound.gameObject.SetActive(true);
                EffectSound.SetVolume(Managers.Game.SettingData.Sfx.Value ? 0.5f : 0);
            } ,
            EffectSound =>
            {
                EffectSound.gameObject.SetActive(false);
            });

        Managers.Game.SettingData.Bgm.Subscribe(bgm =>
        {
            if (_bgmSource == null)
                return;
            
            _bgmSource.volume = bgm ? 1 : 0;
        });
        
        Managers.Game.SettingData.Sfx.Subscribe(sfx =>
        {
            if (_playingEffectSource == null)
                return;
            
            _playingEffectSource.ForEach(effectSound=>
            {
                if (effectSound == null)
                    return;
                
                effectSound.SetVolume(sfx ? 0.5f : 0);
            });
        });
    }

    public void PlayBgm(BgmType bgmType)
    {
        var clip = _soundData.BgmDatas[bgmType].Value;

        if (clip == null)
            return;

        if (_bgmSource.clip == clip)
            return;
        
        if (_bgmSource.isPlaying)
            _bgmSource.Stop();

        _bgmSource.clip = clip;
        _bgmSource.Play();
    }

    private EffectSound PlayEffect(AudioClip clip, bool isLoop = false)
    {
        EffectSound effectSound = _effectSoundPool.Get();
        
        effectSound.SetVolume(Managers.Game.SettingData.Sfx.Value ? 0.5f : 0);
        effectSound.Play(clip);
        _playingEffectSource.Add(effectSound);

        return effectSound;
    }

    public void StopAllEffectSound()
    {
        var playingEffectSource = _playingEffectSource.ToList();
        playingEffectSource.ForEach(effectSound =>
        {
            if (effectSound == null)
                return;
            
            effectSound.Stop();
        });
        
        _playingEffectSource.Clear();
    }

    public void ReturnEffect(EffectSound effectSound)
    {
        _effectSoundPool.Release(effectSound);

        if (_playingEffectSource.Contains(effectSound))
            _playingEffectSource.Remove(effectSound);
    }
    
    private AudioSource CreateBgmSound()
    {
        return Managers.Resource.Instantiate("ETC/BgmSound", Root).GetComponent<AudioSource>();
    }

    private EffectSound CreateEffectSound()
    {
        return Managers.Resource.Instantiate("ETC/EffectSound", Root).GetComponent<EffectSound>();
    }

    public void PlayAttackSound()
    {
        var clip = _soundData.AttackSoundClips[Random.Range(0, _soundData.AttackSoundClips.Count)];
        PlayEffect(clip);
    }
    
    public void PlayMonsterAttackSound(string soundName)
    {
        if (_soundData.MonsterAttackClips.TryGetValue(soundName, out var clip))
            PlayEffect(clip.Value);
    }

    public EffectSound PlaySkillSound(int skillId, bool isLoop = false)
    {
        if (_soundData == null)
            return null;
        
        if (!_soundData.SkillSoundClips.ContainsKey(skillId))
            return null;
        
        var clip = _soundData.SkillSoundClips[skillId].Value;
        return PlayEffect(clip, isLoop);
    }

    public void PlayUISound(UISoundType uiSoundType)
    {
        if (_soundData == null)
            return;

        if (!_soundData.UISoundClips.TryGetValue(uiSoundType, out var clip))
            return;
        
        PlayEffect(clip.Value);
    }

    public void PlaySfxSound(SfxType sfxType)
    {
        if (_soundData == null)
            return;
        
        var clip = _soundData.SfxClips[sfxType].Value;
        PlayEffect(clip);
    }
}