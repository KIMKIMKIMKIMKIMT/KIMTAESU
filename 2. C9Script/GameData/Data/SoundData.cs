using System.Collections.Generic;
using NativeSerializableDictionary;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Object/Sound Data", order = int.MaxValue)]
public class SoundData : ScriptableObject
{
    [SerializeField] public SerializableDictionary<BgmType, AudioClip> BgmDatas;

    [SerializeField] public List<AudioClip> AttackSoundClips;

    [SerializeField] public SerializableDictionary<int, AudioClip> SkillSoundClips;

    [SerializeField] public SerializableDictionary<UISoundType, AudioClip> UISoundClips;

    [SerializeField] public SerializableDictionary<SfxType, AudioClip> SfxClips;

    [SerializeField] public SerializableDictionary<string, AudioClip> MonsterAttackClips;
}   