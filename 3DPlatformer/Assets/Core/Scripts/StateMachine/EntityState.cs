using Core.Scripts.Entity;

namespace Core.Scripts.StatesMachine
{
    public abstract class EntityState
    {
        public EntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity)
        {
            BaseEntity = baseEntity;
            StateMachine = entityStateMachine;
        }
        
        protected readonly EntityStateMachine StateMachine;

        public virtual void Enter()
        {
            
        }

        public virtual void Exit()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual void FixedUpdate()
        {
            
        }
        
        public BaseEntity BaseEntity { get; private set; }
    }
}