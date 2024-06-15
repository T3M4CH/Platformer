using Core.Scripts.Levels.Interfaces;
using System;
using Core.Scripts.Extensions;
using Core.Scripts.Levels;
using Cysharp.Threading.Tasks;
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
        _currentId = ES3.Load(SaveConstants.CurrentLevel, 0);
    }

    public void CompleteLevel()
    {
        _currentId += 1;

        if (_currentId > _levelsConfig.Levels.Length - 1)
        {
            _currentId = 0;
        }

        ES3.Save(SaveConstants.CurrentLevel, _currentId);
        ChangeLevel();
    }

    public void ChangeLevel()
    {
        if (_levelInstance)
        {
            UniTask.Void(async () =>
            {
                _portalController.AppearPortalAtPlayer();
                
                await UniTask.Delay(TimeSpan.FromSeconds(2.5f));

                Object.Destroy(_levelInstance.gameObject);
                
                _levelInstance = Object.Instantiate(_levelsConfig.Levels[_currentId]);
                _levelInstance.Initialize(this, _windowManager, _portalController, _cameraService);
                OnLevelChanged.Invoke(_levelInstance);
            });
        }
        else
        {
            _levelInstance = Object.Instantiate(_levelsConfig.Levels[_currentId]);
            _levelInstance.Initialize(this, _windowManager, _portalController, _cameraService);
            OnLevelChanged.Invoke(_levelInstance);
        }
    }

    public void Start()
    {
        ChangeLevel();
    }
}