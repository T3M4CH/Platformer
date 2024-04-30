using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class PatrolMoveEntityState : EntityState
    {
        public PatrolMoveEntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity, MonoInteractionSystem interactionSystem) : base(entityStateMachine, baseEntity)
        {
            _animator = baseEntity.Animator;
            _rigidBody = baseEntity.RigidBody;
            _interactionSystem = interactionSystem;
        }

        private Vector3 _savedPosition;
        private Vector3 _direction = Vector3.right;

        private readonly Animator _animator;
        private readonly Rigidbody _rigidBody;
        private readonly MonoInteractionSystem _interactionSystem;

        private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

        public override void Enter()
        {
            base.Enter();

            _rigidBody.velocity = Vector3.zero;

            if (_interactionSystem.IsGround.Under)
            {
                _savedPosition = BaseEntity.transform.position;
            }

            _animator.SetFloat(JoystickOffset, 0.5f);
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

            _rigidBody.MovePosition(_rigidBody.position + _direction * (Time.fixedDeltaTime * BaseEntity.Speed));
        }
    }
}