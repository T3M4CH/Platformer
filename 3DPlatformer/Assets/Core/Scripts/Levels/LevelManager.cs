using Core.Scripts.Levels.Interfaces;
using System;
using System.Collections;
using Core.Scripts.Extensions;
using Core.Scripts.Levels;
using UnityEngine;
using Object = UnityEngine.Object;

public class LevelManager : ILevelService, IStartable
{
    private readonly WindowManager _windowManager;
    private readonly MonoPortalController _portalController;
    private readonly ICameraService _cameraService;
    public event Action<MonoLevelBase> OnLevelChanged = _ => { };

    private int _currentId;
    private MonoLevelBase _levelInstance;
    private readonly LevelsConfig _levelsConfig;

    public LevelManager(WindowManager windowManager, MonoPortalController portalController, ICameraService cameraService)
    {
        _windowManager = windowManager;
        _portalController = portalController;
        _cameraService = cameraService;
        _levelsConfig = Resources.Load<LevelsConfig>("LevelsConfig");
        _currentId = PlayerPrefs.GetInt(SaveConstants.CurrentLevel, 0);
    }

    public void CompleteLevel()
    {
        _currentId += 1;

        if (_currentId > _levelsConfig.Levels.Length - 1)
        {
            _currentId = 0;
        }

        PlayerPrefs.SetInt(SaveConstants.CurrentLevel, _currentId);
        ChangeLevel();
    }

    public void ChangeLevel()
    {
        if (_levelInstance)
        {
            CoroutineRunner.Instance.StartCoroutine(ChangeLevelCoroutine());
        }
        else
        {
            _levelInstance = Object.Instantiate(_levelsConfig.Levels[_currentId]);
            _levelInstance.Initialize(this, _windowManager, _portalController, _cameraService);
            OnLevelChanged.Invoke(_levelInstance);
        }
    }

    private IEnumerator ChangeLevelCoroutine()
    {
        _portalController.AppearPortalAtPlayer();

        yield return new WaitForSeconds(2.5f);

        Object.Destroy(_levelInstance.gameObject);

        _levelInstance = Object.Instantiate(_levelsConfig.Levels[_currentId]);
        _levelInstance.Initialize(this, _windowManager, _portalController, _cameraService);
        OnLevelChanged.Invoke(_levelInstance);
    }

    public void Start()
    {
        ChangeLevel();
    }
}