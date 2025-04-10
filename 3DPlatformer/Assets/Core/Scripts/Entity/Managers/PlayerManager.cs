using System;
using System.Collections;
using Core.Scripts.Effects.Interfaces;
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
        private readonly MonoPortalController _portalController;
        private readonly MonoPlayerController _playerPrefab;
        private readonly ICameraService _cameraService;
        private readonly IEffectService _effectService;
        private readonly PlayerVirtualCamera _playerCamera;

        public PlayerManager
        (
            ILevelService levelService,
            WindowManager windowManager,
            HealthbarManager healthbarManager,
            MonoPortalController portalController,
            MonoPlayerController playerPrefab,
            ICameraService cameraService,
            IEffectService effectService
        )
        {
            _levelService = levelService;
            _windowManager = windowManager;
            _healthbarManager = healthbarManager;
            _portalController = portalController;
            _playerPrefab = playerPrefab;
            _cameraService = cameraService;
            _effectService = effectService;
            _playerCamera = _cameraService.ChangeActiveCamera<PlayerVirtualCamera>();

            _levelService.OnLevelChanged += PerformSpawn;
        }

        private void PerformSpawn(MonoLevelBase levelBase)
        {
            if (!PlayerInstance)
            {
                PlayerInstance = Object.Instantiate(_playerPrefab, levelBase.PlayerSpawnPoint.position, Quaternion.identity);
                PlayerInstance.Construct(_healthbarManager, _effectService, _windowManager);

                _playerCamera.SetFollowAt(PlayerInstance.LookAtPosition).SetLookAt(PlayerInstance.LookAtPosition);
            }
            else
            {
                var position = levelBase.PlayerSpawnPoint.position;
                position.z -= 10;
                _playerCamera.Camera.ForceCameraPosition(position, Quaternion.identity);
            }

            _portalController.TeleportEntity(PlayerInstance, levelBase.PlayerSpawnPoint.position);

            PlayerInstance.OnDead += Restart;
        }

        private void Restart()
        {
            PlayerInstance.OnDead -= Restart;

            CoroutineRunner.Instance.StartCoroutine(RestartScene());
        }

        private IEnumerator RestartScene()
        {
            yield return new WaitForSeconds(1.5f);
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