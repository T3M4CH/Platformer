using UnityEngine;

namespace Core.Scripts.Levels
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "Config/LevelsConfig", order = 0)]
    public class LevelsConfig : ScriptableObject
    {
        [field: SerializeField] public MonoLevelBase[] Levels { get; private set; }
    }
}