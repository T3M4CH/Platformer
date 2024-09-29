using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class BossJumpEntityState : JumpEntityState
    {
        private readonly BossEntity _baseEntity;
        private Transform _targetTransform;

        public BossJumpEntityState(EntityStateMachine entityStateMachine, EntityState exitState, float jumpForce, MonoInteractionSystem interactionSystem, BossEntity baseEntity)
            : base(entityStateMachine, exitState, jumpForce, interactionSystem, baseEntity)
        {
            _baseEntity = baseEntity;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public void SetAimTarget(Transform target)
        {
            _targetTransform = target;
            var direction = (target.position - BaseEntity.transform.position).normalized;

            Direction.x = direction.x;
        }

        protected override void PerformChangeAbleAttack(bool value)
        {
            base.PerformChangeAbleAttack(value);

            if (value)
            {
                var condition = true; //Random.Range(0, 100f) >= 0;

                if (condition)
                {
                    StateMachine.SetState<BowAttackEntityState>().SetAimTarget(_targetTransform);
                }
                else
                {
                    StateMachine.SetState<JumpAttackEntityState>();
                }
            }
        }

        public override void Exit()
        {
            base.Exit();

            Direction = Vector3.zero;
        }
    }
}