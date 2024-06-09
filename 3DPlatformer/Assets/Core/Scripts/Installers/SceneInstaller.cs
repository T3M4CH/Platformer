using Core.Scripts.Cameras;
using Core.Scripts.Entity.Managers;
using Core.Scripts.Entity.Managers.Interfaces;
using Core.Scripts.Extensions;
using Core.Scripts.Healthbars;
using Core.Scripts.Levels.Interfaces;
using Reflex.Core;
using UnityEngine;

public class SceneInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private WindowManager windowManager;
    [SerializeField] private BaseVirtualCamera virtualCamera;
    [SerializeField] private MonoPlayerController playerController;
    [SerializeField] private MonoPortalController portalController;
    [SerializeField] private HealthbarManagerSettings healthbarManagerSettings;

    public void InstallBindings(ContainerBuilder descriptor)
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        descriptor.AddSingleton(healthbarManagerSettings);
        descriptor.AddSingleton(playerController);
        descriptor.AddSingleton(portalController);
        descriptor.AddSingleton(virtualCamera);
        descriptor.AddSingleton(windowManager);

        descriptor.AddSingleton(typeof(HealthbarManager));
        descriptor.AddSingleton(typeof(EnemyManager), typeof(IStartable));
        descriptor.AddSingleton(typeof(LevelManager), typeof(ILevelService), typeof(IStartable));
        descriptor.AddSingleton(typeof(PlayerManager), typeof(IPlayerService), typeof(IStartable));

        descriptor.OnContainerBuilt += PerformContainerBuilt;

        void PerformContainerBuilt(Container container)
        {
            descriptor.OnContainerBuilt -= PerformContainerBuilt;

            foreach (var startable in container.All<IStartable>())
            {
                startable.Start();
            }
        }
    }
}