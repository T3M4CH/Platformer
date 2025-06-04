using System;
using System.Collections;
using Core.Scripts.Entity;
using Core.Scripts.Entity.Interfaces;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
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

            StartCoroutine(MoveCamera(player));
        }
    }

    private IEnumerator MoveCamera(MonoPlayerController player)
    {
        yield return new WaitForSeconds(1f);

        var secondCamera = CameraService.ChangeActiveCamera<SecondVirtualCamera>();
        secondCamera.SetFollowAt(cameraPosition).SetLookAt(cameraPosition);

        yield return new WaitForSeconds(1.5f);

        CameraService.SetFov(90, 3f);

        yield return new WaitForSeconds(0.5f);

        cameraPosition.DOMove(targetPosition.position, 3f).SetUpdate(UpdateType.Fixed);

        yield return new WaitForSeconds(2.5f);

        PortalController.TeleportEntity(player.GetComponent<IAuraCharger>(), targetPosition.position);

        yield return new WaitForSeconds(1.5f);

        CameraService.ChangeActiveCamera<PlayerVirtualCamera>().Camera.GetComponent<CinemachineFollow>().FollowOffset = new Vector3(0, 1.5f, -10);
    }
}