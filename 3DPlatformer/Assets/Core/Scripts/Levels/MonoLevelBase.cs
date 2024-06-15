using Core.Scripts.Entity;
using Core.Scripts.Levels.Interfaces;
using UnityEngine;

public class MonoLevelBase : MonoBehaviour
{
    protected ILevelService LevelService;
    protected WindowManager WindowManager;
    protected ICameraService CameraService;
    protected MonoPortalController PortalController;

    public void Initialize(ILevelService levelService, WindowManager windowManager, MonoPortalController portalController, ICameraService cameraService)
    {
        LevelService = levelService;
        WindowManager = windowManager;
        PortalController = portalController;
        CameraService = cameraService;
    }
    
    [field: SerializeField] public BaseEntity[] Enemies { get; private set; }
    [field: SerializeField] public Transform[] EnemiesSpawnPoints { get; private set; }
    [field: SerializeField] public Transform PlayerSpawnPoint { get; private set; }
    
}
