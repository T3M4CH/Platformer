using Core.Scripts.Bow;
using Core.Scripts.Entity;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class BowAttackEntityState : EntityState
    {
        private bool _isShooting;
        private float _estimatedTime;
        private float _currentTime;
        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private readonly BowController _bowController;
        private readonly EntityState _exitState;
        private readonly Rigidbody _rigidBody;
        private readonly Animator _animator;

        public BowAttackEntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity, BowController bowController, EntityState exitState) : base(entityStateMachine, baseEntity)
        {
            _rigidBody = baseEntity.RigidBody;
            _animator = baseEntity.Animator;
            _bowController = bowController;
            _exitState = exitState;
        }

        public void SetAimTarget(Transform targetPosition)
        {
            _endPosition = targetPosition.position + Vector3.up * 1.5f;
        }

        public override void Enter()
        {
            base.Enter();

            _rigidBody.isKinematic = true;
            _bowController.BowInHands = true;
            _animator.speed = 0.07f;

            _currentTime = 0f;
            _estimatedTime = 1.5f;

            // var transform = BaseEntity.transform;
            // var position = transform.position + Vector3.down;
            // var forward = transform.forward;
            //
            // _startPosition = position + forward * 5f;
            // _endPosition = position + forward * 1.5f;

            _bowController.PerformBowAnim();

            _isShooting = true;
        }

        public override void Update()
        {
            base.Update();

            if (_isShooting)
            {
                _currentTime += Time.deltaTime;

                var ratio = _currentTime / _estimatedTime;
                _bowController.PerformMoveBowAlongBody(ratio, _endPosition);

                // var directionToTarget = targetPosition - _bowController.BowTransform.position;
                // var lookRotation = Quaternion.LookRotation(directionToTarget);
                //
                // _bowController.BowTransform.rotation = lookRotation * Quaternion.Euler(-90, 0, 0);

                if (_currentTime > _estimatedTime)
                {
                    _bowController.Shot();
                    _isShooting = false;
                    StateMachine.SetState(_exitState);
                }
            }
        }

        public override void Exit()
        {
            base.Exit();

            _animator.speed = 1;
            _rigidBody.isKinematic = false;
            _bowController.BowInHands = false;
        }
    }
}