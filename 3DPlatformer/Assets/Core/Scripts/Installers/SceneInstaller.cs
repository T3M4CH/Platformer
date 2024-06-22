using Core.Scripts.Cameras;
using Core.Scripts.Effects;
using Core.Scripts.Effects.Interfaces;
using Core.Scripts.Effects.Structs;
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
    [SerializeField] private MonoPlayerController playerController;
    [SerializeField] private MonoPortalController portalController;
    [SerializeField] private HealthbarManagerSettings healthbarManagerSettings;
    [SerializeField] private SerializableCameraSettings camerasSettings;
    [SerializeField] private EffectStruct[] effectStructs;

    public void InstallBindings(ContainerBuilder descriptor)
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        descriptor.AddSingleton(healthbarManagerSettings);
        descriptor.AddSingleton(playerController);
        descriptor.AddSingleton(portalController);
        descriptor.AddSingleton(camerasSettings);
        descriptor.AddSingleton(windowManager);
        descriptor.AddSingleton(effectStructs);

        descriptor.AddSingleton(typeof(HealthbarManager));
        descriptor.AddSingleton(typeof(EnemyManager), typeof(IStartable));
        descriptor.AddSingleton(typeof(LevelManager), typeof(ILevelService), typeof(IStartable));
        descriptor.AddSingleton(typeof(CameraService), typeof(ICameraService));
        descriptor.AddSingleton(typeof(EffectController), typeof(IEffectService));
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