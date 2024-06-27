using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;

public class Toggle : MonoBehaviour
{
    public event Action<bool> OnStateChanged = _ => { };

    [SerializeField] private ToggleSettings enabledToggleSettings;
    [SerializeField] private ToggleSettings disabledToggleSettings;

    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image switchImage;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private float duration;
    [SerializeField] private Ease ease;

    private bool _isOn;
    private RectTransform _rect;
    private Vector2 _handlePosition;

    private void Awake()
    {
        _rect = switchImage.rectTransform;
        button.onClick.AddListener(PerformButtonClick);
    }

    public void SetupToggle(bool isOn)
    {
        _isOn = isOn;

        ChangeToggleState();
    }

    private void PerformButtonClick()
    {
        if (!_rect) return;

        _isOn = !_isOn;

        ChangeToggleState();
        OnStateChanged.Invoke(_isOn);
    }

    private void ChangeToggleState()
    {
        if (_isOn)
        {
            switchImage.sprite = enabledToggleSettings.SwitchSprite;
            iconImage.sprite = enabledToggleSettings.IconSprite;
            backgroundImage.color = enabledToggleSettings.BackgroundColor;
            _handlePosition.x = 20f;
        }
        else
        {
            switchImage.sprite = disabledToggleSettings.SwitchSprite;
            iconImage.sprite = disabledToggleSettings.IconSprite;
            backgroundImage.color = disabledToggleSettings.BackgroundColor;
            _handlePosition.x = -20f;
        }

        _rect.DOKill();
        _rect.DOAnchorPos(_handlePosition, duration).SetEase(ease).SetUpdate(true).SetLink(gameObject);
    }
}