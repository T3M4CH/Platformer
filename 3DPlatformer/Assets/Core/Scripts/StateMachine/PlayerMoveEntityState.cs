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
            _collision = baseEntity.EntityCollision;
            _entityLayerMask = baseEntity.EntityLayerMask;
            _idleBehaviour = baseEntity.Animator.GetBehaviour<MoveBehaviour>();
            _interactionSystem = baseEntity.InteractionSystem;
        }

        private bool _isJumping;
        private bool _isCloseToEnemy;
        private Vector3 _direction;

        private readonly float _speed;
        private readonly float _jumpForce;
        private readonly Button _jumpButton;
        private readonly Animator _animator;
        private readonly Button _attackButton;
        private readonly Rigidbody _rigidBody;
        private readonly MonoJoystick _joystick;
        private readonly LayerMask _entityLayerMask;
        private readonly EntityCollision _collision;
        private readonly MoveBehaviour _idleBehaviour;
        private readonly MonoInteractionSystem _interactionSystem;
        private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public override void Enter()
        {
            base.Enter();

            _isCloseToEnemy = false;
            _rigidBody.velocity = Vector3.zero;
            _attackButton.interactable = true;
            _jumpButton.interactable = true;
            _jumpButton.onClick.AddListener(Jump);
            _attackButton.onClick.AddListener(Attack);

            _collision.CollisionStay += OnCollisionStay;
            _collision.CollisionExit += OnCollisionExit;
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_entityLayerMask.value.Includes(collision.gameObject.layer))
            {
                _isCloseToEnemy = false;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (_entityLayerMask.value.Includes(collision.gameObject.layer))
            {
                _isCloseToEnemy = true;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _jumpButton.interactable = _interactionSystem.IsGround.Under && !_isCloseToEnemy;

            Move();
        }

        public override void Update()
        {
            base.Update();

            _direction.x = _joystick.Direction.x;

            if (Keyboard.current.spaceKey.wasPressedThisFrame && _jumpButton.interactable)
            {
                Jump();
            }

            if (Keyboard.current.fKey.wasPressedThisFrame && _attackButton.interactable)
            {
                Attack();
            }
        }

        private void Attack()
        {
            if (!_interactionSystem.IsGround.Under || _direction.x == 0) return;

            StateMachine.SetState<MeleeAttackEntityState>();
        }

        private void Jump()
        {
            StateMachine.SetState<JumpEntityState>();
        }

        private void Move()
        {
            if (_idleBehaviour.isIdle)
            {
                _rigidBody.MovePosition(_rigidBody.position + _direction * (_speed * Time.deltaTime));
            }

            var angle = Math.Sign(_direction.x);
            angle = angle == 0 ? 180 : angle * 90;
            _rigidBody.MoveRotation(Quaternion.Euler(0, angle, 0));

            _animator.SetFloat(JoystickOffset, Mathf.Abs(_direction.x));
        }

        public override void Exit()
        {
            base.Exit();

            _jumpButton.onClick.RemoveListener(Jump);
            _attackButton.onClick.RemoveListener(Attack);
            
            _collision.CollisionStay -= OnCollisionStay;
            _collision.CollisionExit -= OnCollisionExit;
        }

        public void Dispose()
        {
            _jumpButton.onClick.RemoveListener(Jump);
            _attackButton.onClick.RemoveListener(Attack);
            
            _collision.CollisionStay -= OnCollisionStay;
            _collision.CollisionExit -= OnCollisionExit;
        }
    }
}