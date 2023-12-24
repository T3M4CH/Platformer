using System;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class ThrownEntityState : EntityState, IDisposable
    {
        public ThrownEntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity) : base(entityStateMachine, baseEntity)
        {
            _animator = baseEntity.Animator;
            _animatorHelper = baseEntity.AnimatorHelper;
            _entityCollision = baseEntity.EntityCollision;

            _animatorHelper.OnStand += PerformStand;
            _animatorHelper.OnStanding += PerformStanding;
            _entityCollision.TriggerEnter += OnTriggerEnter;
        }

        private float _initialSize;
        private CapsuleCollider _capsuleCollider;
        
        private readonly Animator _animator;
        private readonly EntityCollision _entityCollision;
        private readonly MonoAnimatorHelper _animatorHelper;
        
        private static readonly int Fall = Animator.StringToHash("Fall");

        public override void Enter()
        {
            base.Enter();
            
            _capsuleCollider = _entityCollision.Collider as CapsuleCollider;

            if (_capsuleCollider)
            {
                _initialSize = _capsuleCollider.height;
                _capsuleCollider.height = 1f;
            }
            
            _animator.SetTrigger(Fall);
        }

        private void PerformStand()
        {
            StateMachine.SetState<PatrolMoveEntityState>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (BaseEntity.WaterLayerMask.value.Includes(collider.gameObject.layer))
            {
                BaseEntity.TakeDamage(float.MaxValue);
            }
        }

        private void PerformStanding()
        {
            _capsuleCollider.height = _initialSize;
        }

        public void Dispose()
        {
            _animatorHelper.OnStand -= PerformStand;
            _animatorHelper.OnStanding -= PerformStanding;
            _entityCollision.TriggerEnter -= OnTriggerEnter;
        }
    }
}