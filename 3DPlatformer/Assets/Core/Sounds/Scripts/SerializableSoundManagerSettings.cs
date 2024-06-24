using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Sounds.Scripts
{
    [Serializable]
    public struct SerializableSoundManagerSettings
    {
        [field: SerializeField] public AudioMixerGroup SoundMixer { get; private set; }
        [field: SerializeField] public AudioMixerGroup MusicMixer { get; private set; }
    }
}