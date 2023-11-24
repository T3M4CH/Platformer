using System;
using UnityEngine;

namespace Core.Scripts.Healthbars
{
    [Serializable]
    public struct HealthbarManagerSettings
    {
        [field: SerializeField] public RectTransform Canvas { get; private set; }
        [field: SerializeField] public MonoHealthbar HealthBarPrefab { get; private set; }
    }
}