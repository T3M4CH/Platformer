using System;
using System.Collections.Generic;
using Core.Scripts.Effects.Interfaces;
using Core.Scripts.Effects.Structs;
using UnityEngine;

namespace Core.Scripts.Effects
{
    public class EffectController : IEffectService
    {
        private Dictionary<EVfxType, PoolObjects<MonoEffect>> _effects = new();
        
        public EffectController(EffectStruct[] effectStructs)
        {
            foreach (var effectStruct in effectStructs)
            {
                _effects.Add(effectStruct.Type, new PoolObjects<MonoEffect>(effectStruct.Effect, new GameObject(effectStruct.Type + " Holder").transform, 5));
            }
        }
        
        public MonoEffect GetEffect(EVfxType vfxType, bool returnActive = false)
        {
            if (_effects.TryGetValue(vfxType, out var pool))
            {
                return pool.GetFree(returnActive);
            }

            throw new Exception($"Effect {vfxType} is not found");
        }
    }
}