using Core.Scripts.Extensions;
using Core.Sounds.Scripts;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : IStartable
{
    public SoundManager(SerializableSoundManagerSettings soundManagerSettings)
    {
        _soundMixer = soundManagerSettings.SoundMixer;
        _musicMuxer = soundManagerSettings.MusicMixer;
    }

    private bool _isInitialized;
    private SoundAsset[] _soundAssets;

    private readonly AudioMixerGroup _musicMuxer;
    private readonly AudioMixerGroup _soundMixer;

    private void Execute()
    {
        if (_isInitialized) return;

        _isInitialized = true;

        _soundAssets = Resources.LoadAll<SoundAsset>("SoundAssets");
        var parent = new GameObject("SoundAssets").transform;
        Object.DontDestroyOnLoad(parent);

        foreach (var soundAsset in _soundAssets)
        {
            soundAsset.Initialize(parent);
            if (soundAsset.Settings.PlayOnEnable)
            {
                soundAsset.Play();
            }
        }
    }

    public void Start()
    {
        Execute();
    }
}