using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Healthbars;
using Reflex.Core;
using UnityEngine;

public class SceneInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private HealthbarManagerSettings healthbarManagerSettings;
    public void InstallBindings(ContainerDescriptor descriptor)
    {
        descriptor.AddInstance(healthbarManagerSettings);

        descriptor.AddSingleton(typeof(HealthbarManager));
    }
}
