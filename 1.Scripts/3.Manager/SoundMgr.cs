using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eAUDIOTYPE
{
    Bgm,
    Sfx,
    Max
}

public enum eAUDIOCLIP_BGM
{
    None = -1,

    Title,
    Main,
    Game,

    Max
}

public enum eAUDIOCLIP_SFX
{
    None = -1,

    UI_Button,
    EnemyDie,
    BossDie,
    EnemyHit,
    BaseAttack0,
    BaseAttack1,
    BaseAttack2,
    GuidedMissile,
    DropMissile,
    RangeMissile,
    AroundDron,
    Mine,
    Warnning,
    Gem,
    BonusBox,
    ItemBoom,
    Item,
    Reward,
    Victory,
    Lose,
    SkillSelect,


    Max
}

public class SoundMgr : DontDestroy<SoundMgr>
{
    #region Fields
    public AudioSource _bgmPlayer;
    public AudioSource _sfxPlayer;
    public AudioSource _moveSfxPlayer;

    [SerializeField] private AudioClip[] _bgmClips;
    [SerializeField] private AudioClip[] _sfxClips;
    [SerializeField] private AudioClip _moveClips;

    private Dictionary<eAUDIOCLIP_SFX, int> SFX_BUFFER = new Dictionary<eAUDIOCLIP_SFX, int>();

    private const int MAX_TM_COUNT = 6;
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        _bgmPlayer.rolloffMode = AudioRolloffMode.Linear;
        _sfxPlayer.rolloffMode = AudioRolloffMode.Linear;
        _moveSfxPlayer.rolloffMode = AudioRolloffMode.Linear;
    }
    #endregion

    #region Coroutines
    private IEnumerator Cor_RemoveSfx(eAUDIOCLIP_SFX sfx, float length)
    {
        yield return new WaitForSeconds(length);
        SFX_BUFFER[sfx]--;
    }
    #endregion

    #region Public Methods
    public void PlayBGMPlayer(eAUDIOCLIP_BGM clipKey)
    {
        if (_bgmPlayer.clip == _bgmClips[(int)clipKey] && _bgmPlayer.isPlaying)
            return;

        _bgmPlayer.clip = _bgmClips[(int)clipKey];
        _bgmPlayer.Play();
        _bgmPlayer.pitch = 1;
    }

    public void StopBGMPlayer()
    {
        _bgmPlayer.Stop();
    }

    public void PlaySFXSound(eAUDIOCLIP_SFX clipKey, bool bOverlap)
    {
        if (clipKey == eAUDIOCLIP_SFX.None)
        {
            return;
        }
        if (!SFX_BUFFER.ContainsKey(clipKey))
        {
            SFX_BUFFER.Add(clipKey, 1);
            StartCoroutine(Cor_RemoveSfx(clipKey, _sfxClips[(int)clipKey].length));
            _sfxPlayer.PlayOneShot(_sfxClips[(int)clipKey]);
        }
        else
        {
            SFX_BUFFER[clipKey]++;
            if (SFX_BUFFER[clipKey] > MAX_TM_COUNT)
            {
                SFX_BUFFER[clipKey] = MAX_TM_COUNT;
                return;
            }
            if (bOverlap)
            {
                StartCoroutine(Cor_RemoveSfx(clipKey, _sfxClips[(int)clipKey].length));
                _sfxPlayer.PlayOneShot(_sfxClips[(int)clipKey]);
            }
            else
            {
                _sfxPlayer.clip = _sfxClips[(int)clipKey];
                _sfxPlayer.Play();

            }
        }
    }

    public void PlaySFXSound(eAUDIOCLIP_SFX clipKey)
    {
        if (clipKey == eAUDIOCLIP_SFX.None)
        {
            return;
        }
        _sfxPlayer.PlayOneShot(_sfxClips[(int)clipKey]);
    }

    public void StopEffectSound()
    {
        _sfxPlayer.Stop();
        _sfxPlayer.loop = false;
    }

    public void MoveSfxPlay()
    {
        if (!PlayerDataMgr.Instance.PlayerData.SfxToggle)
        {
            return;
        }
        if (!_moveSfxPlayer.isPlaying)
        {
            _moveSfxPlayer.PlayOneShot(_moveClips);
        }
    }
    public void MoveSfxStop()
    {
        if (_moveSfxPlayer.isPlaying)
        {
            _moveSfxPlayer.Stop();
        }
    }

    public void SoundPause()
    {
        _bgmPlayer.volume = 0;
        _sfxPlayer.volume = 0;
    }



    #endregion
}