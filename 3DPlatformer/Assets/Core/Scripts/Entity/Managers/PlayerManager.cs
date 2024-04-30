using System;
using Core.Scripts.Cameras;
using Core.Scripts.Entity.Managers.Interfaces;
using Core.Scripts.Extensions;
using Core.Scripts.Healthbars;
using Core.Scripts.Levels.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Core.Scripts.Entity.Managers
{
    public class PlayerManager : IPlayerService, IStartable, IDisposable
    {
        private readonly ILevelService _levelService;
        private readonly WindowManager _windowManager;
        private readonly HealthbarManager _healthbarManager;
        private readonly MonoPlayerController _playerPrefab;
        private readonly BaseVirtualCamera _virtualCamera;

        public PlayerManager(ILevelService levelService, WindowManager windowManager, HealthbarManager healthbarManager, MonoPlayerController playerPrefab, BaseVirtualCamera virtualCamera)
        {
            _levelService = levelService;
            _windowManager = windowManager;
            _healthbarManager = healthbarManager;
            _playerPrefab = playerPrefab;
            _virtualCamera = virtualCamera;

            _levelService.OnLevelChanged += PerformSpawn;
        }

        private void PerformSpawn(MonoLevelBase levelBase)
        {
            if (!PlayerInstance)
            {
                PlayerInstance = Object.Instantiate(_playerPrefab, levelBase.PlayerSpawnPoint.position, Quaternion.identity);
                PlayerInstance.Construct(_windowManager);
                PlayerInstance.Construct(_healthbarManager);

                _virtualCamera.SetFollowAt(PlayerInstance.LookAtPosition).SetLookAt(PlayerInstance.LookAtPosition);
            }
            else
            {
                var position = levelBase.PlayerSpawnPoint.position;
                PlayerInstance.transform.position = position;
                position.z -= 10;
                _virtualCamera.Camera.ForceCameraPosition(position, Quaternion.identity);
            }

            PlayerInstance.OnDead += Restart;
        }

        private void Restart()
        {
            PlayerInstance.OnDead -= Restart;
            SceneManager.LoadScene(0);
        }

        public void Start()
        {
        }

        public void Dispose()
        {
            _levelService.OnLevelChanged -= PerformSpawn;
        }

        public MonoPlayerController PlayerInstance { get; private set; }
    }
}