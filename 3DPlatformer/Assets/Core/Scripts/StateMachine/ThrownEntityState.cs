using System;
using Core.Scripts.Effects.Interfaces;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class ThrownEntityState : EntityState, IDisposable
    {
        private readonly EntityState _exitState;
        private readonly IEffectService _effectService;

        public ThrownEntityState(EntityStateMachine entityStateMachine, EntityState exitState, BaseEntity baseEntity, IEffectService effectService) : base(entityStateMachine, baseEntity)
        {
            _exitState = exitState;
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

        private static readonly int Knocked = Animator.StringToHash("Knocked");

        public override void Enter()
        {
            base.Enter();

            _entityCollision.DefaultCollider.excludeLayers = BaseEntity.EntityLayerMask;
            _entityCollision.TriggerCollider.enabled = true;
            _effectService.GetEffect(EVfxType.Hit, true).SetPosition(BaseEntity.transform.position, scale: Vector3.one * 0.5f);
            _animator.SetTrigger(Knocked);
        }

        private void PerformStand()
        {
            _entityCollision.DefaultCollider.excludeLayers = 0;
            _entityCollision.TriggerCollider.enabled = false;
            StateMachine.SetState(_exitState);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (BaseEntity.WaterLayerMask.value.Includes(collider.gameObject.layer))
            {
                BaseEntity.TakeDamage(float.MaxValue);
            }
        }

        public override void Exit()
        {
            base.Exit();

            _entityCollision.DefaultCollider.excludeLayers = 0;
            _entityCollision.TriggerCollider.enabled = false;
        }

        public void Dispose()
        {
            _animatorHelper.OnStand -= PerformStand;
            _entityCollision.TriggerEnter -= OnTriggerEnter;
        }
    }
}