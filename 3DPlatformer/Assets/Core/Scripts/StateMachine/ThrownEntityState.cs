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
            _entityCollision.TriggerEnter += OnTriggerEnter;
        }

        private float _initialSize;
        
        private readonly Animator _animator;
        private readonly EntityCollision _entityCollision;
        private readonly MonoAnimatorHelper _animatorHelper;
        
        private static readonly int Fall = Animator.StringToHash("Fall");

        public override void Enter()
        {
            base.Enter();
            
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

        public void Dispose()
        {
            _animatorHelper.OnStand -= PerformStand;
            _entityCollision.TriggerEnter -= OnTriggerEnter;
        }
    }
}