using Core.Scripts.Levels.Interfaces;
using UnityEngine.InputSystem;
using Reflex.Attributes;
using UnityEngine;

public class TestLevels : MonoBehaviour
{
    private ILevelService _levelService;

    [Inject]
    private void Construct(ILevelService levelService)
    {
        _levelService = levelService;
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            _levelService.CompleteLevel();
        }
    }
}