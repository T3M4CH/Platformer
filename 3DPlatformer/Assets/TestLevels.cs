using System;
using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Levels.Interfaces;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

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
