using System;
using Core.Scripts.Entity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Core.Scripts.StatesMachine
{
    public class PlayerMoveEntityState : EntityState, IDisposable
    {
        public PlayerMoveEntityState(EntityStateMachine entityStateMachine, MonoPlayerController baseEntity, ControlsWindow controlsWindow) : base(entityStateMachine, baseEntity)
        {
            _joystick = controlsWindow.Joystick;
            _jumpButton = controlsWindow.JumpButton;
            _attackButton = controlsWindow.AttackButton;

            _rigidBody = baseEntity.RigidBody;
            _speed = baseEntity.Speed;
            _animator = baseEntity.Animator;
            _interactionSystem = baseEntity.InteractionSystem;
            _collision = baseEntity.EntityCollision;
        }

        private bool _isJumping;
        private Vector3 _direction;
        private IDamageable _target;

        private readonly float _speed;
        private readonly float _jumpForce;
        private readonly Button _jumpButton;
        private readonly Animator _animator;
        private readonly Button _attackButton;
        private readonly Rigidbody _rigidBody;
        private readonly MonoJoystick _joystick;
        private readonly EntityCollision _collision;
        private readonly MonoInteractionSystem _interactionSystem;
        private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public override void Enter()
        {
            base.Enter();

            _target = null;
            _attackButton.interactable = false;
            _jumpButton.onClick.AddListener(Jump);
            _attackButton.onClick.AddListener(Attack);

            _collision.TriggerEnter += OnTriggerEnter;
            _collision.TriggerExit += OnTriggerExit;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _jumpButton.interactable = _interactionSystem.IsGround.Under;

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

            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                Attack();
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out IDamageable damageable))
            {
                _target = damageable;

                _attackButton.interactable = true;
            }
        }

        private void OnTriggerExit(Collider coll)
        {
            if (coll.TryGetComponent(out IDamageable _))
            {
                _target = null;
                _attackButton.interactable = false;
            }
        }

        private void Attack()
        {
            if (!_interactionSystem.IsGround.Under || _target == null) return;

            StateMachine.SetState<MeleeAttackEntityState>().SetAimTarget(_target);
        }

        private void Jump()
        {
            StateMachine.SetState<JumpEntityState>();
        }

        private void Move()
        {
            _rigidBody.MovePosition(_rigidBody.position + _direction * (_speed * Time.deltaTime));
            _rigidBody.MoveRotation(Quaternion.Euler(0, Math.Sign(_direction.x) * -90, 0));

            _animator.SetFloat(JoystickOffset, Mathf.Abs(_direction.x));
        }

        public override void Exit()
        {
            base.Exit();

            _attackButton.interactable = false;
            _jumpButton.onClick.RemoveListener(Jump);
            _attackButton.onClick.RemoveListener(Attack);
            _collision.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            _jumpButton.onClick.RemoveListener(Jump);
            _attackButton.onClick.RemoveListener(Attack);

            _collision.TriggerEnter -= OnTriggerEnter;
            _collision.TriggerExit -= OnTriggerExit;
        }
    }
}