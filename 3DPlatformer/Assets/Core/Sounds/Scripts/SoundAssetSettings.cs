using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class SoundAssetSettings
{
    [field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }
    [field: SerializeField] public bool IsOneShot { get; private set; }
    [field: SerializeField] public bool IsLoop { get; private set; }
    [field: SerializeField] public float Volume { get; private set; }
    [field: SerializeField] public bool PlayOnEnable { get; private set; }
    [field: SerializeField] public bool WaitPlay { get; private set; }
    [field: SerializeField] public float MinDistance { get; private set; }
    [field: SerializeField] public float MaxDistance { get; private set; }
    [field: SerializeField, Range(0, 1f)] public float SpatialBlend { get; private set; }
}