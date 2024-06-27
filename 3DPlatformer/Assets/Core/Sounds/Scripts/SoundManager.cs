using System;
using Core.Scripts.Extensions;
using Core.Scripts.Windows.Interfaces;
using Core.Sounds.Scripts;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

public class SoundManager : IStartable, IDisposable
{
    public SoundManager(SerializableSoundManagerSettings soundManagerSettings)
    {
        _soundMixer = soundManagerSettings.SoundMixer;
        _musicMuxer = soundManagerSettings.MusicMixer;
    }

    private bool _isInitialized;
    private SoundAsset[] _soundAssets;
    private IGameSettings _gameSettings;

    private readonly AudioMixerGroup _musicMuxer;
    private readonly AudioMixerGroup _soundMixer;

    public void InitializeSettings(IGameSettings gameSettings)
    {
        if (_gameSettings != null)
        {
            _gameSettings.OnChangeMusic -= ValidateMixers;
            _gameSettings.OnChangeSounds -= ValidateMixers;
            _gameSettings.OnChangeHaptic -= ValidateHaptic;
        }
        
        _gameSettings = gameSettings;
        
        _gameSettings.OnChangeMusic += ValidateMixers;
        _gameSettings.OnChangeSounds += ValidateMixers;
        _gameSettings.OnChangeHaptic += ValidateHaptic;
    }

    private void ValidateMixers()
    {
        _soundMixer.audioMixer.SetFloat("VolumeSounds", _gameSettings.Sound ? 0 : -80);
        _musicMuxer.audioMixer.SetFloat("VolumeMusic", _gameSettings.Music ? 0 : -80);
    }

    private void ValidateHaptic()
    {
        for (var i = 0; i < _soundAssets.Length; i++)
        {
            _soundAssets[i].ChangeHapticState(_gameSettings.Haptic);
        }
    }

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

    public void Dispose()
    {
        _gameSettings.OnChangeMusic -= ValidateMixers;
        _gameSettings.OnChangeSounds -= ValidateMixers;
        _gameSettings.OnChangeHaptic -= ValidateHaptic;
    }
}