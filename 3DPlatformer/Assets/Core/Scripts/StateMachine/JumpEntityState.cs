using System;
using Core.Scripts.Entity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Core.Scripts.StatesMachine
{
    public class JumpEntityState : EntityState, IDisposable
    {
        public JumpEntityState(EntityStateMachine entityStateMachine, MonoPlayerController baseEntity, ControlsWindow controlsWindow) : base(entityStateMachine, baseEntity)
        {
            _joystick = controlsWindow.Joystick;
            _jumpButton = controlsWindow.JumpButton;
            _attackButton = controlsWindow.AttackButton;

            _speed = baseEntity.Speed;
            _animator = baseEntity.Animator;
            _rigidBody = baseEntity.RigidBody;
            _jumpForce = baseEntity.JumpForce;
            _animatorHelper = baseEntity.AnimatorHelper;
        }

        private Vector3 _direction;
        private Vector3 _jumpStartPosition;

        private readonly float _speed;
        private readonly float _jumpForce;
        private readonly Button _jumpButton;
        private readonly Animator _animator;
        private readonly Button _attackButton;
        private readonly Rigidbody _rigidBody;
        private readonly MonoJoystick _joystick;
        private readonly EntityCollision _collision;
        private readonly MonoAnimatorHelper _animatorHelper;

        private static readonly int JumpAnimation = Animator.StringToHash("Jump");
        private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public override void Enter()
        {
            base.Enter();

            _jumpStartPosition = _rigidBody.position;
            _rigidBody.velocity = Vector3.zero;
            _jumpButton.interactable = false;
            _attackButton.interactable = true;
            _animator.SetTrigger(JumpAnimation);
            _rigidBody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

            _attackButton.onClick.AddListener(PerformAttack);

            _animatorHelper.OnLand += PerformLand;
        }

        private void PerformAttack()
        {
            StateMachine.SetState<JumpAttackEntityState>().SetTarget(_jumpStartPosition);
        }

        public override void Update()
        {
            base.Update();

            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                PerformAttack();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _direction.x = _joystick.Direction.x;
            _attackButton.interactable = _direction.x != 0;
            Move();
        }

        private void Move()
        {
            _rigidBody.MovePosition(_rigidBody.position + _direction * (_speed * Time.deltaTime));

            var angle = Math.Sign(_direction.x);
            angle = angle == 0 ? 180 : angle * 90;
            _rigidBody.MoveRotation(Quaternion.Euler(0, angle, 0));

            _animator.SetFloat(JoystickOffset, Mathf.Abs(_direction.x));
        }

        private void PerformLand()
        {
            if (StateMachine.CurrentEntityState == this)
            {
                StateMachine.SetState<PlayerMoveEntityState>();
            }
        }

        public override void Exit()
        {
            base.Exit();

            _attackButton.onClick.RemoveListener(PerformAttack);

            _animatorHelper.OnLand -= PerformLand;
        }

        public void Dispose()
        {
            _animatorHelper.OnLand -= PerformLand;
        }
    }
}