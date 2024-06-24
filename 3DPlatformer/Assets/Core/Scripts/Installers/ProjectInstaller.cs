using Core.Scripts.Extensions;
using Core.Sounds.Scripts;
using Reflex.Core;
using UnityEngine;

public class ProjectInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private SerializableSoundManagerSettings soundManagerSettings;

    public void InstallBindings(ContainerBuilder descriptor)
    {
        descriptor.AddSingleton(soundManagerSettings);
        descriptor.AddSingleton(typeof(SoundManager), typeof(IStartable));
    }
}