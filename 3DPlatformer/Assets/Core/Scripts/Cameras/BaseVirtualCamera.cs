using Cinemachine;
using UnityEngine;

namespace Core.Scripts.Cameras
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class BaseVirtualCamera : MonoBehaviour
    {
        public BaseVirtualCamera SetFollowAt(Transform target)
        {
            Camera.Follow = target;

            return this;
        }

        public BaseVirtualCamera SetLookAt(Transform target)
        {
            Camera.LookAt = target;

            return this;
        }
        
        [field: SerializeField] public CinemachineVirtualCamera Camera { get; private set; }
    }
}