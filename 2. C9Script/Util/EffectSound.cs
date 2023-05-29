using UnityEngine;

namespace Util
{
    public class EffectSound : MonoBehaviour
    {
        private AudioSource _audioSource;
        private bool _isCheck = false;

        public bool Loop => _audioSource.loop;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClip audioClip)
        {
            _audioSource.PlayOneShot(audioClip);
            _isCheck = true;
        }

        public void SetVolume(float volume)
        {
            if (_audioSource == null)
                return;
            
            _audioSource.volume = volume;
        }

        public void SetLoop(bool isLoop)
        {
            _audioSource.loop = isLoop;
        }

        private void Update()
        {
            if (_audioSource == null)
                return;

            if (!_isCheck)
                return;

            if (!_audioSource.isPlaying)
            {
                _isCheck = false;
                Managers.Sound.ReturnEffect(this);
            }
        }

        public void Stop()
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();
        }
    }
}