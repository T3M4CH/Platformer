using Core.Scripts.Levels.Interfaces;
using System;
using Core.Scripts.Extensions;
using Core.Scripts.Levels;
using UnityEngine;
using Object = UnityEngine.Object;

public class LevelManager : ILevelService, IStartable
{
    public event Action<MonoLevelBase> OnLevelChanged = _ => {};

    private int _currentId;
    private MonoLevelBase _levelInstance;
    private readonly LevelsConfig _levelsConfig;

    public LevelManager()
    {
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
            Object.Destroy(_levelInstance.gameObject);
        }

        _levelInstance = Object.Instantiate(_levelsConfig.Levels[_currentId]);
        _levelInstance.Initialize(this);
        OnLevelChanged.Invoke(_levelInstance);
    }

    public void Start()
    {
        ChangeLevel();
    }
}
