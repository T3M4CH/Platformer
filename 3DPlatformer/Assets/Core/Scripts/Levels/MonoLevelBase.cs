using Core.Scripts.Entity;
using Core.Scripts.Levels.Interfaces;
using UnityEngine;

public class MonoLevelBase : MonoBehaviour
{
    protected ILevelService LevelService;
    protected WindowManager WindowManager;

    public void Initialize(ILevelService levelService, WindowManager windowManager)
    {
        LevelService = levelService;
        WindowManager = windowManager;
    }
    
    [field: SerializeField] public BaseEntity[] Enemies { get; private set; }
    [field: SerializeField] public Transform[] EnemiesSpawnPoints { get; private set; }
    [field: SerializeField] public Transform PlayerSpawnPoint { get; private set; }
    
}
