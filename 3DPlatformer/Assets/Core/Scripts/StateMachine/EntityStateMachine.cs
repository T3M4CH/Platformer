using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class EntityStateMachine
    {
        private readonly Dictionary<Type, EntityState> _states = new();

        public void AddState(EntityState entityState)
        {
            if (_states.ContainsKey(entityState.GetType()))
            {
                return;
            }

            _states.Add(entityState.GetType(), entityState);
        }

        public EntityState SetState(EntityState entityState)
        {
            var type = entityState.GetType();


            if (_states.TryGetValue(type, out var state))
            {
                PreviousEntityState = CurrentEntityState;
                PreviousEntityState?.Exit();
                CurrentEntityState = state;
                CurrentEntityState?.Enter();
                return CurrentEntityState;
            }

            throw new Exception();
        }

        public T SetState<T>() where T : EntityState
        {
            var type = typeof(T);

            if (_states.TryGetValue(type, out var state))
            {
                PreviousEntityState = CurrentEntityState;
                PreviousEntityState?.Exit();
                CurrentEntityState = state;
                CurrentEntityState?.Enter();
                return (T)CurrentEntityState;
            }

            throw new Exception();
        }

        public T SetInheritedState<T>() where T : EntityState
        {
            var type = typeof(T);
            foreach (var kvp in _states)
            {
                if (type.IsAssignableFrom(kvp.Key))
                {
                    return (T)SetState(kvp.Value);
                }
            }

            throw new Exception();
        }

        public T GetState<T>() where T : EntityState
        {
            var type = typeof(T);
            if (_states.TryGetValue(type, out var state))
            {
                return (T)state;
            }

            throw new Exception();
        }

        public void Update()
        {
            CurrentEntityState?.Update();
        }

        public void FixedUpdate()
        {
            CurrentEntityState?.FixedUpdate();
        }

        public EntityState CurrentEntityState { get; private set; }
        public EntityState PreviousEntityState { get; private set; }
    }
}