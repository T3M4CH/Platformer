using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class BossJumpEntityState : JumpEntityState
    {
        private float _distance;
        private bool _isBowAttack;
        private Transform _targetTransform;
        private readonly BossEntity _baseEntity;

        public BossJumpEntityState(EntityStateMachine entityStateMachine, EntityState exitState, float jumpForce, MonoInteractionSystem interactionSystem, BossEntity baseEntity)
            : base(entityStateMachine, exitState, jumpForce, interactionSystem, baseEntity)
        {
            _baseEntity = baseEntity;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log(Time.time + "ВХОД");
        }

        public void SetAimTarget(Transform target, bool isBowAttack = false)
        {
            _targetTransform = target;
            var direction = (target.position - BaseEntity.transform.position).normalized;

            Direction.x = direction.x;

            _isBowAttack = isBowAttack;
        }

        protected override void PerformChangeAbleAttack(bool value)
        {
            base.PerformChangeAbleAttack(value);

            if (value)
            {
                _distance = Vector3.Distance(_targetTransform.position, BaseEntity.transform.position);

                diff = Time.time - diff;
                Debug.LogWarning(RigidBody.transform.name + " " + diff);
                
                StateMachine.SetState<JumpAttackEntityState>();
                return;
                if (_distance > 4 || _isBowAttack)
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