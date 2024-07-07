using System;
using Core.Scripts.Effects.Interfaces;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class DamagedEntityState : EntityState, IDisposable
    {
        public DamagedEntityState(EntityStateMachine entityStateMachine, EntityState exitState, BaseEntity baseEntity, IEffectService effectService) : base(entityStateMachine, baseEntity)
        {
            _exitState = exitState;
            _effectService = effectService;
            _animator = baseEntity.Animator;
            _animatorHelper = baseEntity.AnimatorHelper;
        }

        private readonly Animator _animator;
        private readonly EntityState _exitState;
        private readonly IEffectService _effectService;
        private readonly MonoAnimatorHelper _animatorHelper;

        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int GetHit = Animator.StringToHash("GetHit");

        public override void Enter()
        {
            base.Enter();

            _effectService.GetEffect(EVfxType.Hit, true).SetPosition(BaseEntity.transform.position, scale: Vector3.one * 0.25f);
            
            _animator.ResetTrigger(Attack);
            _animator.SetTrigger(GetHit);

            _animatorHelper.OnDamageExit += PerformStand;
        }

        private void PerformStand()
        {
            StateMachine.SetState(_exitState);
        }

        public override void Exit()
        {
            base.Exit();

            _animatorHelper.OnDamageExit -= PerformStand;
        }

        public void Dispose()
        {
            _animatorHelper.OnDamageExit -= PerformStand;
        }
    }
}