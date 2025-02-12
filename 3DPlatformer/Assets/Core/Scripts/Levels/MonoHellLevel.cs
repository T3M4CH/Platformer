using System;
using Core.Scripts.Entity;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class MonoHellLevel : MonoLevelBase
{
    [SerializeField] private EntityCollision portalCollision;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private Transform cameraPosition;

    private void Start()
    {
        portalCollision.CollisionEnter += ValidateCollision;
    }

    private void ValidateCollision(Collision other)
    {
        if (other.transform.TryGetComponent(out MonoPlayerController player) && player.enabled)
        {
            PortalController.AppearPortalAtPlayer();
            UniTask.Void(async () =>
            {
                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(1f));

                    var secondCamera = CameraService.ChangeActiveCamera<SecondVirtualCamera>();
                    secondCamera.SetFollowAt(cameraPosition).SetLookAt(cameraPosition);

                    await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

                    CameraService.SetFov(90, 3f);

                    await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

                    cameraPosition.DOMove(targetPosition.position, 3f).SetUpdate(UpdateType.Fixed);

                    await UniTask.Delay(TimeSpan.FromSeconds(2.5f));

                    PortalController.TeleportEntity(player, targetPosition.position);

                    await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

                    CameraService.ChangeActiveCamera<PlayerVirtualCamera>().Camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, 1.5f, -10);
                }
                catch
                {
                    //ignored
                }
            });
        }
    }
}