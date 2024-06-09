using Core.Scripts.Entity;
using UnityEngine;

public class MonoLevelPortal : MonoLevelBase
{
    [SerializeField] private EntityCollision entityCollision;

    private void PerformTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out MonoPlayerController player) && player.enabled)
        {
            LevelService.CompleteLevel();
        }
    }
    
    private void Start()
    {
        entityCollision.TriggerEnter += PerformTriggerEnter;
    }

    private void OnDestroy()
    {
        entityCollision.TriggerEnter -= PerformTriggerEnter;
    }
}
