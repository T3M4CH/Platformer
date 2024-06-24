using System;
using System.Collections.Generic;
using System.Linq;
using Lofelt.NiceVibrations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Sounds.Scripts
{
    public class SoundAsset : ScriptableObject
    {
        [SerializeField] private List<AudioClip> _audioClips = new();
        [SerializeField] private HapticPatterns.PresetType hapticPattern;

        private float _maxVolume;
        private bool _hapticEnable;
        private bool _isInitialized;
        private AudioSource _audioSource;
        private Transform _parent;

        public void Initialize(Transform parent)
        {
            if (_audioSource != null)
            {
                Destroy(_audioSource.gameObject);
            }

            if (!_audioClips.Any())
            {
                throw new Exception($"No clips inside {name}");
            }

            _audioSource = new GameObject($"{_audioClips[0].name}").AddComponent<AudioSource>();
            DontDestroyOnLoad(_audioSource.gameObject);
            _audioSource.outputAudioMixerGroup = Settings.MixerGroup;
            _audioSource.loop = Settings.IsLoop;
            _audioSource.volume = Settings.Volume;
            _audioSource.playOnAwake = Settings.PlayOnEnable;
            _audioSource.spatialBlend = Settings.SpatialBlend;
            _audioSource.minDistance = Settings.MinDistance;
            _audioSource.maxDistance = Settings.MaxDistance;
            _maxVolume = _audioSource.volume;
        }

        public void AddClip(AudioClip clip)
        {
            _audioClips.Add(clip);
        }

        public void ChangeHapticState(bool value)
        {
            _hapticEnable = value;
        }

        public void SetVolume(float value)
        {
            _audioSource.volume = Mathf.Min(value, _maxVolume);
        }

        public void Play(float? pitch = null)
        {
            if (_audioSource == null) return;

            if (Settings.WaitPlay && _audioSource.isPlaying) return;

            if (_hapticEnable)
            {
                HapticPatterns.PlayPreset(hapticPattern);
            }

            if (Settings.IsOneShot)
            {
                PlayOneShot(_audioClips[Random.Range(0, _audioClips.Count)], pitch);
            }
            else
            {
                PlayClip(_audioClips[Random.Range(0, _audioClips.Count)], pitch);
            }
        }

        private void PlayOneShot(AudioClip clip, float? pitch = null)
        {
            if (pitch.HasValue)
            {
                _audioSource.pitch = pitch.Value;
            }

            _audioSource.PlayOneShot(clip);
        }

        private void PlayClip(AudioClip clip, float? pitch = null)
        {
            if (pitch.HasValue)
            {
                _audioSource.pitch = pitch.Value;
            }

            _audioSource.clip = clip;
            _audioSource.Play();
        }

        public bool IsPlay => _audioSource != null && _audioSource.isPlaying;
        [field: SerializeField] public SoundAssetSettings Settings { get; private set; }
    }
}