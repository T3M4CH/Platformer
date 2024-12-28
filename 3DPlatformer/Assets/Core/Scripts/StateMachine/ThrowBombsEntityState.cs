using System;
using Core.Scripts.Entity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Scripts.StatesMachine
{
    public class ThrowBombsEntityState : EntityState, IDisposable
    {
        private float _currentTime;
        
        private readonly float _delay;
        private readonly Rigidbody _bomb;
        private readonly Animator _animator;
        private readonly Transform _transform;
        private readonly Transform _playerTransform;
        private readonly MonoAnimatorHelper _animatorHelper;
        private static readonly int Throw = Animator.StringToHash("Throw");

        public ThrowBombsEntityState(EntityStateMachine entityStateMachine, MonoBomberEnemy baseEntity, MonoPlayerController playerController) : base(entityStateMachine, baseEntity)
        {
            _bomb = baseEntity.Bomb;
            _delay = baseEntity.Delay;
            _animator = baseEntity.Animator;
            _animatorHelper = baseEntity.AnimatorHelper;
            _transform = baseEntity.transform;
            _playerTransform = playerController.transform;

            _animatorHelper.OnAttacked += Attack;
        }

        public override void Enter()
        {
            base.Enter();

            var direction = _playerTransform.position - _transform.position;
            direction.y = 0;

            var rotation = Quaternion.LookRotation(direction);
            _transform.rotation = rotation;

            _currentTime = 2f;
        }

        public override void Update()
        {
            base.Update();

            _currentTime -= Time.deltaTime;

            if (_currentTime < 0 && Vector3.Distance(_playerTransform.position, _transform.position) > 7)
            {
                _currentTime = _delay;
                
                _animator.SetTrigger(Throw);
            }
        }

        private void Attack()
        {
            var playerPos = _playerTransform.position;
            var position = _transform.position;

            var displacementY = playerPos.y - position.y;
            var displacementXZ = new Vector3(playerPos.x - position.x, 0, playerPos.z - position.z);
            var height = 2.5f;
            var gravity = Physics.gravity.y;

            var time = Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity);
            var velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
            var velocityXZ = displacementXZ / time;

            var bombInstance = Object.Instantiate(_bomb, _transform.position + Vector3.up, Quaternion.identity);
            bombInstance.transform.SetParent(_transform.parent);
            bombInstance.linearVelocity = velocityY + velocityXZ;
        }

        public void Dispose()
        {
            _animatorHelper.OnAttacked -= Attack;
        }
    }
}