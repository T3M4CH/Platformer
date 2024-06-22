using System;
using Core.Scripts.Effects.Interfaces;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class ThrownEntityState : EntityState, IDisposable
    {
        private readonly IEffectService _effectService;

        public ThrownEntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity, IEffectService effectService) : base(entityStateMachine, baseEntity)
        {
            _effectService = effectService;
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
            
            _effectService.GetEffect(EVfxType.Hit, true).SetPosition(BaseEntity.transform.position, scale: Vector3.one * 0.5f);
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