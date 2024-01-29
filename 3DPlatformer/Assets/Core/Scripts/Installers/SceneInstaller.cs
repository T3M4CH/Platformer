using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Healthbars;
using Reflex.Core;
using UnityEngine;

public class SceneInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private WindowManager windowManager;
    [SerializeField] private HealthbarManagerSettings healthbarManagerSettings;
    public void InstallBindings(ContainerDescriptor descriptor)
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        descriptor.AddInstance(healthbarManagerSettings);
        descriptor.AddInstance(windowManager);

        descriptor.AddSingleton(typeof(HealthbarManager));
    }
}
