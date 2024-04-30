using System;
using Core.Scripts.Windows;
using TMPro;
using UnityEngine;

public class TimerWindow : UIWindow
{
    public event Action OnComplete = () => { };

    [SerializeField] private TMP_Text timerText;

    private bool _isRunning;
    private float _currentSeconds;

    public void SetTimer(float seconds)
    {
        Show();
        _isRunning = true;
        _currentSeconds = seconds;
    }

    private void Update()
    {
        if(!_isRunning) return;

        _currentSeconds -= Time.deltaTime;

        if (_currentSeconds < 0)
        {
            _isRunning = false;
            OnComplete.Invoke();
            Hide();
            return;
        }

        var time = TimeSpan.FromSeconds(_currentSeconds);
        timerText.text = time.ToString("mm':'ss");
    }
}
