using UnityEngine;

public class MonoAnimatorHelper : MonoBehaviour
{
    [SerializeField] private MonoPlayerController playerController;
    
    public void PerformAttackEvent()
    {
        playerController.PerformAttackEvent();
    }
}
