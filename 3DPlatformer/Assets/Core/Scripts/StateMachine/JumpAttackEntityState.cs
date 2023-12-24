using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class JumpAttackEntityState : EntityState
    {
        public JumpAttackEntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity) : base(entityStateMachine, baseEntity)
        {
            _animator = baseEntity.Animator;
            _transform = baseEntity.transform;
        }

        private float _currentTime;
        private Vector3 _endPosition;
        private Vector3 _startPosition;
        private IDamageable _damageable;
        
        private readonly Animator _animator;
        private readonly Transform _transform;
        private static readonly int JumpAttack = Animator.StringToHash("JumpAttack");

        public void SetTarget(IDamageable damageable)
        {
            _damageable = damageable;
            _animator.SetTrigger(JumpAttack);
            _endPosition = damageable.transform.position;
        }

        public override void Enter()
        {
            base.Enter();
            BaseEntity.RigidBody.isKinematic = true;
            _currentTime = 0f;
            _startPosition = _transform.position;
        }

        public override void Update()
        {
            base.Update();
            _currentTime += Time.deltaTime * 5f;
            _transform.position = Vector3.Lerp(_startPosition, _endPosition, _currentTime);

            if (_currentTime >= 1)
            {
                StateMachine.SetState<PlayerMoveEntityState>();
                _damageable.TakeDamage(5, Vector3.right * Mathf.Sign(_endPosition.x - _startPosition.x) + Vector3.up);
            }
        }

        public override void Exit()
        {
            base.Exit();
            BaseEntity.RigidBody.isKinematic = false;
        }
    }
}