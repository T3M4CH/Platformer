using System;
using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Windows;
using Core.Scripts.Windows.Interfaces;
using Core.Sounds.Scripts;
using Reflex.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class SettingsWindow : UIWindow, IGameSettings
{
    public event Action OnChangeMusic = () => { };
    public event Action OnChangeSounds = () => { };
    public event Action OnChangeHaptic = () => { };

    [SerializeField] private SoundAsset interactionSound;
    [SerializeField] private Toggle toggleSound;
    [SerializeField] private Toggle toggleMusic;
    [SerializeField] private Toggle toggleHaptic;

    private bool _isMusic;
    private bool _isSound;
    private bool _isHaptic;

    [Inject]
    private void Construct(SoundManager soundManager)
    {
        soundManager.InitializeSettings(this);
    }

    private void Awake()
    {
        Hide();
        OnChangeMusic.Invoke();
        OnChangeSounds.Invoke();
        OnChangeHaptic.Invoke();

        toggleSound.OnStateChanged += PerformToggleSoundChanged;
        toggleMusic.OnStateChanged += PerformToggleMusicChanged;
        toggleHaptic.OnStateChanged += PerformToggleHapticChanged;
    }

    private void Start()
    {
        Music = PlayerPrefs.GetInt(SaveConstants.MusicSettings, 1) == 1;
        Sound = PlayerPrefs.GetInt(SaveConstants.SoundSettings, 1) == 1;
        Haptic = PlayerPrefs.GetInt(SaveConstants.HapticSettings, 1) == 1;

        toggleSound.SetupToggle(Sound);
        toggleMusic.SetupToggle(Music);
        toggleHaptic.SetupToggle(Haptic);
    }

    public override void Hide()
    {
        base.Hide();

        Time.timeScale = 1;
    }

    public override void Show()
    {
        base.Show();

        Time.timeScale = 0;

        interactionSound.Play(Random.Range(0.95f, 1.1f));
    }

    private void PerformToggleSoundChanged(bool value)
    {
        Sound = value;
        interactionSound.Play(Random.Range(0.95f, 1.1f));
    }

    private void PerformToggleMusicChanged(bool value)
    {
        Music = value;
        interactionSound.Play(Random.Range(0.95f, 1.1f));
    }

    private void PerformToggleHapticChanged(bool value)
    {
        Haptic = value;
        interactionSound.Play(Random.Range(0.95f, 1.1f));
    }

    private void OnDestroy()
    {
        toggleSound.OnStateChanged -= PerformToggleSoundChanged;
        toggleMusic.OnStateChanged -= PerformToggleMusicChanged;
        toggleHaptic.OnStateChanged -= PerformToggleHapticChanged;
    }

    public bool Haptic
    {
        get => _isHaptic;
        private set
        {
            _isHaptic = value;
            PlayerPrefs.SetInt(SaveConstants.HapticSettings, value ? 1 : 0);
            OnChangeHaptic.Invoke();
        }
    }

    public bool Sound
    {
        get => _isSound;
        private set
        {
            _isSound = value;
            PlayerPrefs.SetInt(SaveConstants.SoundSettings, value ? 1 : 0);
            OnChangeSounds.Invoke();
        }
    }

    public bool Music
    {
        get => _isMusic;
        private set
        {
            _isMusic = value;
            PlayerPrefs.SetInt(SaveConstants.MusicSettings, value ? 1 : 0);
            OnChangeMusic.Invoke();
        }
    }
}