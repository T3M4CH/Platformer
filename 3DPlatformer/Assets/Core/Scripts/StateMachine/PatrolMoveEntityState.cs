using System;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class PatrolMoveEntityState : EntityState, IDisposable
    {
        public PatrolMoveEntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity, MonoInteractionSystem interactionSystem) : base(entityStateMachine, baseEntity)
        {
            _animator = baseEntity.Animator;
            _rigidBody = baseEntity.RigidBody;
            _interactionSystem = interactionSystem;
            _collision = baseEntity.EntityCollision;
            _entityLayerMask = baseEntity.EntityLayerMask;
        }

        private float _currentTime;
        private float _speedMultiplier;
        private Vector3 _savedPosition;
        private Vector3 _direction = Vector3.right;

        private readonly Animator _animator;
        private readonly Rigidbody _rigidBody;
        private readonly float _attackDelay = 0.4f;
        private readonly EntityCollision _collision;
        private readonly LayerMask _entityLayerMask;
        private readonly MonoInteractionSystem _interactionSystem;

        private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public override void Enter()
        {
            base.Enter();

            _currentTime = _attackDelay;
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.MoveRotation(Quaternion.LookRotation(_direction));

            if (_interactionSystem.IsGround.Under)
            {
                _savedPosition = BaseEntity.transform.position;
            }

            _speedMultiplier = 1;
            _animator.SetFloat(JoystickOffset, 0.5f);

            _collision.CollisionStay += OnCollisionStay;
            _collision.CollisionExit += OnCollisionExit;
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_entityLayerMask.value.Includes(collision.gameObject.layer))
            {
                _speedMultiplier = 1;
                _animator.SetFloat(JoystickOffset, 0.5f);
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (_entityLayerMask.value.Includes(other.gameObject.layer))
            {
                _speedMultiplier = 0;
                _currentTime -= Time.deltaTime;
                _animator.SetFloat(JoystickOffset, 0f);

                if (other.transform.TryGetComponent(out IDamageable damageable) && _currentTime < 0)
                {
                    _currentTime = _attackDelay;
                    StateMachine.SetState<MeleeAttackEntityState>().SetAimTarget(damageable);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!_interactionSystem.IsGround.Under)
            {
                _rigidBody.position = _savedPosition;
                _direction *= -1;
                _rigidBody.MoveRotation(Quaternion.LookRotation(_direction));
            }
            else
            {
                _savedPosition = _rigidBody.position;
            }

            _rigidBody.MovePosition(_rigidBody.position + _direction * (Time.fixedDeltaTime * BaseEntity.Speed * _speedMultiplier));
        }

        public override void Exit()
        {
            base.Exit();

            _collision.CollisionExit -= OnCollisionExit;
            _collision.CollisionStay -= OnCollisionStay;
        }

        public void Dispose()
        {
            _collision.CollisionExit -= OnCollisionExit;
            _collision.CollisionStay -= OnCollisionStay;
        }
    }
}