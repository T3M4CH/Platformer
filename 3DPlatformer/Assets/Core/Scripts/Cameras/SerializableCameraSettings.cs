using Unity.Cinemachine;
using UnityEngine;
using System;

namespace Core.Scripts.Cameras
{
    [Serializable]
    public struct SerializableCameraSettings
    {
        [field: SerializeField] public CinemachineBrain CinemachineBrain { get; private set; }
        [field: SerializeField] public BaseVirtualCamera[] Cameras { get; private set; }
    }
}