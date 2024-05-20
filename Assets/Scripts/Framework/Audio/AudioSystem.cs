using Random = UnityEngine.Random;
using DG.Tweening;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.Utils;

namespace DerailedDeliveries.Framework.Audio
{
    /// <summary>
    /// Class responsible for managing general audio effect.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioSystem : AbstractSingleton<AudioSystem>
    {
        [Header("Sounds")]
        [SerializeField]
        private AudioCollection[] _audioCollections;

        [SerializeField]
        private float _songFadeDuration = .5f;

        [SerializeField]
        private float _songStopFadeDuration = .1f;

        /// <summary>
        /// Current active audio source for playing songs.
        /// </summary>
        public AudioSource CurrentActiveSongSource { get; private set; }
        
        private bool _isCrossFading;
        
        private int _soundLenghtCounter = 0;
        private int _songLenghtCounter = 0;
        private int _crossFadeSongLenghtCounter = 0;

        private void Awake() => CurrentActiveSongSource = GetComponent<AudioSource>();

        /// <summary>
        /// Method for playing a random sound effect.
        /// </summary>
        public void PlayRandomSoundEffect()
        {
            Array availableAudioTypes = Enum.GetValues(typeof(AudioCollectionTypes));   
            int randIndex = Random.Range(0, availableAudioTypes.Length);

            PlayRandomSoundEffectOfType((AudioCollectionTypes)availableAudioTypes.GetValue(randIndex));
        }

        /// <summary>
        /// Method for playing a random sound effect based on a specified type.
        /// </summary>
        /// <param name="audioCollectionType">Audio collection type.</param>
        /// <param name="volume">Optional volume param.</param>
        public void PlayRandomSoundEffectOfType(AudioCollectionTypes audioCollectionType, float volume = .75f)
        {
            for (int i = 0; i < _audioCollections.Length; i++)
            {
                if (_audioCollections[i].AudioCollectionType != audioCollectionType)
                    continue;

                int randIndex = Random.Range(0, _audioCollections[i].audioClipList.Count);
                PlaySound(_audioCollections[i].audioClipList[randIndex], volume);
            }
        }

        private void PlaySound(AudioClip clip, float volume = .75f)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();

            source.clip = clip;
            source.volume = volume;

            source.Play();

            float audioClipLenght = clip.length;
            DOTween.To(() => _soundLenghtCounter, x => _soundLenghtCounter = x, 1, audioClipLenght)
                .OnComplete(() => Destroy(source));
        }

        /// <summary>
        /// Method for playing a song. Applies automatic crossfading if needed.
        /// </summary>
        /// <param name="clip">Song to be played.</param>
        public void PlaySong(AudioClip clip, float volume = .75f, float startTime = 0)
        {
            if (CurrentActiveSongSource.clip == clip)
                return;

            if (_isCrossFading)
                return;

            // Crossfading logic.
            if (CurrentActiveSongSource.clip != null) 
            {
                _isCrossFading = true;
                AudioSource crossFadeSource = CreateCrossFadeSource(clip, volume, startTime);

                CurrentActiveSongSource.DOFade(0, _songFadeDuration).OnComplete(() 
                    => DestroyCrossFadeSource(crossFadeSource));
                
                return;
            }
         
            CurrentActiveSongSource.volume = 0;
            CurrentActiveSongSource.clip = clip;

            CurrentActiveSongSource.Play();

            CurrentActiveSongSource.time = startTime;
            CurrentActiveSongSource.DOFade(volume, _songFadeDuration);

            float audioClipLenght = clip.length;
            DOTween.To(() => _songLenghtCounter, x => _songLenghtCounter = x, 1, audioClipLenght)
                .OnComplete(() => CurrentActiveSongSource.clip = null);
        }

        /// <summary>
        /// Helper method which stops and fades the current active played song.
        /// </summary>
        public void StopCurrentActiveSong()
        {
            if (CurrentActiveSongSource.clip == null)
                return;

            CurrentActiveSongSource.DOFade(0, _songStopFadeDuration)
                .OnComplete(() => CurrentActiveSongSource.clip = null);
        }

        private AudioSource CreateCrossFadeSource(AudioClip clip, float volume = .75f, float startTime = 0)
        {
            AudioSource crossFadeSource = gameObject.AddComponent<AudioSource>();

            crossFadeSource.clip = clip;
            crossFadeSource.volume = 0;

            crossFadeSource.Play();
            crossFadeSource.time = startTime;

            crossFadeSource.DOFade(volume, _songFadeDuration);

            return crossFadeSource;
        }

        private void DestroyCrossFadeSource(AudioSource source)
        {
            Destroy(CurrentActiveSongSource);
            CurrentActiveSongSource = source;

            _isCrossFading = false;
            float audioClipLenght = CurrentActiveSongSource.clip.length;

            DOTween.To(() => _crossFadeSongLenghtCounter, x => _crossFadeSongLenghtCounter = x, 1, audioClipLenght)
                .OnComplete(() => CurrentActiveSongSource.clip = null);
        }
    }
}
