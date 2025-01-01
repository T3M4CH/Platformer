using System;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class FallEntityState : EntityState, IDisposable
    {
        private Vector3 _direction;
        private readonly float _speed;
        private readonly Animator Animator;
        private readonly Rigidbody RigidBody;
        private readonly MonoJoystick _joystick;
        private readonly MonoPlayerController _player;
        private readonly MonoAnimatorHelper _animatorHelper;
        private readonly MonoInteractionSystem _interactionSystem;

        private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public FallEntityState(EntityStateMachine entityStateMachine, MonoInteractionSystem interactionSystem, MonoPlayerController baseEntity, ControlsWindow controlsWindow) : base(entityStateMachine, baseEntity)
        {
            _player = baseEntity;
            _speed = BaseEntity.Speed;
            _interactionSystem = interactionSystem;
            Animator = baseEntity.Animator;
            RigidBody = baseEntity.RigidBody;
            _animatorHelper = baseEntity.AnimatorHelper;
            _joystick = controlsWindow.Joystick;
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Break();

            Animator.speed = 0.75f;
            Animator.SetTrigger("Fall");
            Animator.SetBool("IsGround", false);
        }

        protected void Move()
        {
            RigidBody.MovePosition(RigidBody.position + _direction * (_speed * Time.deltaTime));

            var angle = Math.Sign(_direction.x);
            angle = angle == 0 ? 180 : angle * 90;
            RigidBody.MoveRotation(Quaternion.Euler(0, angle, 0));

            Animator.SetFloat(JoystickOffset, Mathf.Abs(_direction.x));

            if (_player.InteractionSystem.IsGround.Under)
            {
                Animator.SetBool("IsGround", true);
                StateMachine.SetState<PlayerMoveEntityState>();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _direction.x = _joystick.Direction.x;

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