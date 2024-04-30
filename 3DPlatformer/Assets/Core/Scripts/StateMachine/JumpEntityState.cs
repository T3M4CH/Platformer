using System;
using Core.Scripts.Entity;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.StatesMachine
{
    public class JumpEntityState : EntityState, IDisposable
    {
        public JumpEntityState(EntityStateMachine entityStateMachine, MonoPlayerController baseEntity, ControlsWindow controlsWindow) : base(entityStateMachine, baseEntity)
        {
            _joystick = controlsWindow.Joystick;
            _jumpButton = controlsWindow.JumpButton;

            _speed = baseEntity.Speed;
            _animator = baseEntity.Animator;
            _rigidBody = baseEntity.RigidBody;
            _jumpForce = baseEntity.JumpForce;
            _collision = baseEntity.EntityCollision;
            _animatorHelper = baseEntity.AnimatorHelper;
        }

        private Vector3 _direction;

        private readonly float _speed;
        private readonly float _jumpForce;
        private readonly Button _jumpButton;
        private readonly Animator _animator;
        private readonly Rigidbody _rigidBody;
        private readonly MonoJoystick _joystick;
        private readonly EntityCollision _collision;
        private readonly MonoAnimatorHelper _animatorHelper;
        
        private static readonly int JumpAnimation = Animator.StringToHash("Jump");
        private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public override void Enter()
        {
            base.Enter();
            
            _collision.gameObject.SetActive(true);
            _rigidBody.velocity = Vector3.zero;
            _jumpButton.interactable = false;
            _animator.SetTrigger(JumpAnimation);
            _rigidBody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

            _animatorHelper.OnLand += PerformLand;
            _collision.TriggerEnter += OnTriggerEnter;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _direction.x = _joystick.Direction.x;
            Move();
        }

        private void Move()
        {
            _rigidBody.MovePosition(_rigidBody.position + _direction * (_speed * Time.deltaTime));
            _rigidBody.MoveRotation(Quaternion.Euler(0, Math.Sign(_direction.x) * -90, 0));

            _animator.SetFloat(JoystickOffset, Mathf.Abs(_direction.x));
        }
        private void OnTriggerEnter(Collider collider)
        {
            if (BaseEntity.EntityLayerMask.value.Includes(collider.gameObject.layer))
            {
                if (collider.gameObject.TryGetComponent(out IDamageable damageable))
                {
                    StateMachine.SetState<JumpAttackEntityState>().SetTarget(damageable);
                }
            }
        }

        private void PerformLand()
        {
            StateMachine.SetState<PlayerMoveEntityState>();
        }
        

        public override void Exit()
        {
            base.Exit();
            
            _collision.gameObject.SetActive(false);
            
            _animatorHelper.OnLand -= PerformLand;
            _collision.TriggerEnter -= OnTriggerEnter;
        }

        public void Dispose()
        {
            _animatorHelper.OnLand -= PerformLand;
            _collision.TriggerEnter -= OnTriggerEnter;
        }
    }
}