using System;
using Core.Scripts.Entity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Scripts.StatesMachine
{
    public class PlayerMoveEntityState : EntityState, IDisposable
    {
        public PlayerMoveEntityState(EntityStateMachine entityStateMachine, MonoPlayerController baseEntity) : base(entityStateMachine, baseEntity)
        {
            _animatorHelper = baseEntity.AnimatorHelper;
            _rigidBody = baseEntity.RigidBody;
            _speed = baseEntity.Speed;
            _animator = baseEntity.Animator;
            _joystick = baseEntity.Joystick;
            _jumpForce = baseEntity.JumpForce;
            _interactionSystem = baseEntity.InteractionSystem;
            _collision = baseEntity.EntityCollision;

            _animatorHelper.OnLand += PerformLand;
            _collision.TriggerEnter += OnTriggerEnter;
        }


        private Vector3 _direction;
        
        private readonly float _speed;
        private readonly float _jumpForce;
        private readonly Animator _animator;
        private readonly Rigidbody _rigidBody;
        private readonly MonoJoystick _joystick;
        private readonly EntityCollision _collision;
        private readonly MonoAnimatorHelper _animatorHelper;
        private readonly MonoInteractionSystem _interactionSystem;
        private static readonly int JumpAnimation = Animator.StringToHash("Jump");
        private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public override void Enter()
        {
            base.Enter();
            _collision.gameObject.SetActive(false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Move();
        }

        public override void Update()
        {
            base.Update();

            _direction.x = _joystick.Direction.x;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Jump();
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out IDamageable damageable) && !_interactionSystem.IsGround.Under)
            {
                StateMachine.SetState<JumpAttackEntityState>().SetTarget(damageable);
            }
        }

        private void Jump()
        {
            if (!_interactionSystem.IsGround.Under) return;

            _collision.gameObject.SetActive(true);
            _animator.SetTrigger(JumpAnimation);
            _rigidBody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }

        private void Move()
        {
            _rigidBody.MovePosition(_rigidBody.position + _direction * (_speed * Time.deltaTime));
            _rigidBody.MoveRotation(Quaternion.Euler(0, Math.Sign(_direction.x) * -90, 0));

            _animator.SetFloat(JoystickOffset, Mathf.Abs(_direction.x));
        }

        private void PerformLand()
        {
            _collision.gameObject.SetActive(false);
        }

        public override void Exit()
        {
            base.Exit();
            
            _collision.gameObject.SetActive(true);
        }

        public void Dispose()
        {
            _animatorHelper.OnLand -= PerformLand;
            _collision.TriggerEnter -= OnTriggerEnter;
        }
    }
}