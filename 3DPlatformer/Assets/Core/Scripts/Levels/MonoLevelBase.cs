using Core.Scripts.Entity;
using Core.Scripts.Levels.Interfaces;
using UnityEngine;

public class MonoLevelBase : MonoBehaviour
{
    protected ILevelService LevelService;

    public void Initialize(ILevelService levelService)
    {
        LevelService = levelService;
    }
    
    [field: SerializeField] public BaseEntity[] Enemies { get; private set; }
    [field: SerializeField] public Transform[] EnemiesSpawnPoints { get; private set; }
    [field: SerializeField] public Transform PlayerSpawnPoint { get; private set; }
    
}
