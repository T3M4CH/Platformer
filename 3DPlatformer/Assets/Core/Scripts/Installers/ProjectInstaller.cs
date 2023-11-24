using Reflex.Core;
using UnityEngine;

public class ProjectInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerDescriptor descriptor)
    {
        descriptor.AddInstance("Hello ");
    }
}
