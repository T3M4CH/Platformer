using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class BossJumpEntityState : JumpEntityState
    {
        private readonly BossEntity _baseEntity;

        public BossJumpEntityState(EntityStateMachine entityStateMachine, EntityState exitState, float jumpForce, MonoInteractionSystem interactionSystem, BossEntity baseEntity)
            : base(entityStateMachine, exitState, jumpForce, interactionSystem, baseEntity)
        {
            _baseEntity = baseEntity;
        }

        public override void Enter()
        {
            base.Enter();
        }

        protected override void PerformChangeAbleAttack(bool value)
        {
            base.PerformChangeAbleAttack(value);

            if (value)
            {
                var condition = Random.Range(0, 100f) > 50;

                if (condition)
                {
                    StateMachine.SetState<BowAttackEntityState>();
                }
                else
                {
                    StateMachine.SetState<JumpAttackEntityState>();
                }
            }
        }
    }
}