using System;
using System.Threading;
using Core.Scripts.Entity;
using Cysharp.Threading.Tasks;
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

        private bool _isAbleToAttack;
        private float _speedMultiplier;
        private Vector3 _savedPosition;
        private Vector3 _direction = Vector3.right;

        private readonly Animator _animator;
        private readonly Rigidbody _rigidBody;
        private readonly EntityCollision _collision;
        private CancellationTokenSource _tokenSource;
        private readonly LayerMask _entityLayerMask;
        private readonly MonoInteractionSystem _interactionSystem;

        private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public override void Enter()
        {
            base.Enter();

            _rigidBody.velocity = Vector3.zero;
            _rigidBody.MoveRotation(Quaternion.LookRotation(_direction));

            if (_interactionSystem.IsGround.Under)
            {
                _savedPosition = BaseEntity.transform.position;
            }

            _tokenSource = new CancellationTokenSource();
            _collision.CollisionStay += OnCollisionStay;

            UniTask.Void(async () =>
            {
                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: _tokenSource.Token);

                    _isAbleToAttack = true;
                }
                catch
                {
                    // ignored
                }
            });
        }

        private void OnCollisionStay(Collision other)
        {
            if (_entityLayerMask.value.Includes(other.gameObject.layer))
            {
                _speedMultiplier = 0;
                _animator.SetFloat(JoystickOffset, 0f);
                
                if (other.transform.TryGetComponent(out IDamageable damageable) && _isAbleToAttack)
                {
                    StateMachine.SetState<MeleeAttackEntityState>().SetAimTarget(damageable);
                }
            }
            else
            {
                _speedMultiplier = 1;
                _animator.SetFloat(JoystickOffset, 0.5f);
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

            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }

            _isAbleToAttack = false;
            _collision.CollisionStay -= OnCollisionStay;
        }

        public void Dispose()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }

            _collision.CollisionStay -= OnCollisionStay;
        }
    }
}