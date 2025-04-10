using System;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class FallEntityState : EntityState, IDisposable
    {
        protected Vector3 Direction;
        
        private readonly float _speed;
        private readonly Animator Animator;
        private readonly Rigidbody RigidBody;
        private readonly BaseEntity _entity;

        private readonly MonoInteractionSystem _interactionSystem;
        private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public FallEntityState(EntityStateMachine entityStateMachine, MonoInteractionSystem interactionSystem, BaseEntity baseEntity) : base(entityStateMachine, baseEntity)
        {
            _entity = baseEntity;
            _speed = BaseEntity.Speed;
            _interactionSystem = interactionSystem;
            Animator = baseEntity.Animator;
            RigidBody = baseEntity.RigidBody;
        }

        public override void Enter()
        {
            base.Enter();

            Animator.speed = 0.75f;
            Animator.SetTrigger("Fall");
            Animator.SetBool("IsGround", false);
        }

        protected void Move()
        {
            var targetPosition = RigidBody.position + Direction * (5 * Time.deltaTime);
            var targetVelocity = (targetPosition - BaseEntity.transform.position) / Time.deltaTime;

            targetVelocity.y = RigidBody.linearVelocity.y;
            RigidBody.linearVelocity = Vector3.Lerp(RigidBody.linearVelocity, targetVelocity, 6 * Time.deltaTime);


            var angle = Math.Sign(Direction.x);
            angle = angle == 0 ? 180 : angle * 90;
            RigidBody.MoveRotation(Quaternion.Euler(0, angle, 0));

            Animator.SetFloat(JoystickOffset, Mathf.Abs(Direction.x));

            if (_interactionSystem.IsGround.Under)
            {
                Animator.SetBool("IsGround", true);
                StateMachine.SetInheritedState<MoveEntityState>();
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            Move();
        }

        public override void Exit()
        {
            base.Exit();

            Animator.speed = 1f;
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }
    }
}