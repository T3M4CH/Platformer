using System;
using UnityEngine;

namespace Core.Scripts.Effects.Structs
{
    [Serializable]
    public struct EffectStruct
    {
        [field: SerializeField] public EVfxType Type { get; private set; }
        [field: SerializeField] public MonoEffect Effect { get; private set; }
    }
}