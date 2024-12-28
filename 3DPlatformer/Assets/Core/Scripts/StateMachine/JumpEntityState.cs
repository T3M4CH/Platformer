using UnityEngine.InputSystem;
using Core.Scripts.Entity;
using UnityEngine;
using System;

namespace Core.Scripts.StatesMachine
{
    public class JumpEntityState : EntityState, IDisposable
    {
        public JumpEntityState(EntityStateMachine entityStateMachine, EntityState exitState, float jumpForce, MonoInteractionSystem interactionSystem, BaseEntity baseEntity) : base(entityStateMachine, baseEntity)
        {
            _exitState = exitState;
            JumpForce = jumpForce;
            _interactionSystem = interactionSystem;
            _speed = baseEntity.Speed;
            Animator = baseEntity.Animator;
            RigidBody = baseEntity.RigidBody;
            _animatorHelper = baseEntity.AnimatorHelper;
        }

        protected bool IsAbleAttack;
        protected Vector3 Direction;
        protected readonly float JumpForce;
        protected readonly Animator Animator;
        protected readonly Rigidbody RigidBody;

        private readonly float _speed;
        private readonly EntityState _exitState;
        private readonly EntityCollision _collision;
        private readonly MonoAnimatorHelper _animatorHelper;
        private readonly MonoInteractionSystem _interactionSystem;

        private static readonly int JumpAnimation = Animator.StringToHash("Jump");
        private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public override void Enter()
        {
            base.Enter();

            IsAbleAttack = false;
            RigidBody.linearVelocity = Vector3.zero;
            Animator.CrossFade("Jump", 0f, 0);
            RigidBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);

            _animatorHelper.OnLand += PerformLand;
            _animatorHelper.OnAbleAttack += PerformChangeAbleAttack;
        }

        protected virtual void PerformChangeAbleAttack(bool value)
        {
            IsAbleAttack = value;
        }

        protected void PerformAttack()
        {
            StateMachine.SetState<JumpAttackEntityState>();
        }

        public override void Update()
        {
            base.Update();

            if (Keyboard.current.fKey.wasPressedThisFrame && IsAbleAttack)
            {
                PerformAttack();
            }

            if (Keyboard.current.gKey.wasPressedThisFrame && IsAbleAttack)
            {
                StateMachine.SetState<BowAttackEntityState>();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            Move();
        }

        protected void Move()
        {
            RigidBody.MovePosition(RigidBody.position + Direction * (_speed * Time.deltaTime));

            var angle = Math.Sign(Direction.x);
            angle = angle == 0 ? 180 : angle * 90;
            RigidBody.MoveRotation(Quaternion.Euler(0, angle, 0));

            Animator.SetFloat(JoystickOffset, Mathf.Abs(Direction.x));
            
            if (BaseEntity is MonoPlayerController playerController)
            {
                Animator.SetBool("IsGround", playerController.InteractionSystem.IsGround.Under);
            }
        }

        private void PerformLand()
        {
            if (StateMachine.CurrentEntityState == this)
            {
                var isGround = _interactionSystem.IsGround.Under;

                var velocity = isGround ? Vector3.zero : RigidBody.linearVelocity;
                StateMachine.SetState(_exitState);
                RigidBody.linearVelocity = velocity;
            }
        }

        public override void Exit()
        {
            base.Exit();

            _animatorHelper.OnLand -= PerformLand;
            _animatorHelper.OnAbleAttack -= PerformChangeAbleAttack;
        }

        public void Dispose()
        {
            _animatorHelper.OnLand -= PerformLand;
            _animatorHelper.OnAbleAttack -= PerformChangeAbleAttack;
        }
    }
}