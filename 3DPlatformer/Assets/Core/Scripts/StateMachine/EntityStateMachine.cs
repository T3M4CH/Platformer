using System;
using System.Collections.Generic;

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

        public T SetState<T>() where T : EntityState
        {
            var type = typeof(T);

            if (CurrentEntityState != null && CurrentEntityState.GetType() == type)
            {
                return (T)CurrentEntityState;
            }

            if (_states.TryGetValue(type, out var state))
            {
                CurrentEntityState?.Exit();
                CurrentEntityState = state;
                CurrentEntityState?.Enter();
            }
            
            return (T)CurrentEntityState;
        }

        public T GetState<T>() where T : EntityState
        {
            var type = typeof(T);
            if (_states.TryGetValue(type, out var state))
            {
                return (T)state;
            }

            return null;
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
    }
}