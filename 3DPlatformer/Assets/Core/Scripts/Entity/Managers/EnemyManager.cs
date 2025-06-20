using System;
using Core.Scripts.Effects.Interfaces;
using Core.Scripts.Entity.Interfaces;
using Core.Scripts.Entity.Managers.Interfaces;
using Core.Scripts.Extensions;
using Core.Scripts.Healthbars;
using Core.Scripts.Levels.Interfaces;
using Object = UnityEngine.Object;

public class EnemyManager : IStartable, IDisposable
{
    private readonly HealthbarManager _healthbarManager;
    private readonly IPlayerService _playerService;
    private readonly ILevelService _levelService;
    private readonly IEffectService _effectService;

    public EnemyManager(HealthbarManager healthbarManager, IPlayerService playerService, ILevelService levelService, IEffectService effectService)
    {
        _healthbarManager = healthbarManager;
        _playerService = playerService;
        _levelService = levelService;
        _effectService = effectService;

        _levelService.OnLevelChanged += PerformSpawn;
    }

    private void PerformSpawn(MonoLevelBase levelBase)
    {
        if (levelBase.Enemies.Length != levelBase.EnemiesSpawnPoints.Length)
        {
            throw new Exception("Enemies count isn't equals they're spawn points");
        }

        var enemies = levelBase.Enemies;
        var positions = levelBase.EnemiesSpawnPoints;

        for (var i = 0; i < enemies.Length; i++)
        {
            var enemy = Object.Instantiate(enemies[i], levelBase.transform);
            enemy.transform.position = positions[i].position;
            enemy.Construct(_healthbarManager, _effectService);

            if (enemy is IPlayerConsumer playerConsumer)
            {
                playerConsumer.Initialize(_playerService);
            }
        }
    }

    public void Start()
    {
    }

    public void Dispose()
    {
        _levelService.OnLevelChanged -= PerformSpawn;
    }
}