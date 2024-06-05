using System;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class DamagedEntityState : EntityState, IDisposable
    {
        public DamagedEntityState(EntityStateMachine entityStateMachine, EntityState exitState, BaseEntity baseEntity) : base(entityStateMachine, baseEntity)
        {
            _exitState = exitState;
            _animator = baseEntity.Animator;
            _animatorHelper = baseEntity.AnimatorHelper;
        }

        private readonly Animator _animator;
        private readonly EntityState _exitState;
        private readonly MonoAnimatorHelper _animatorHelper;

        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int GetHit = Animator.StringToHash("GetHit");

        public override void Enter()
        {
            base.Enter();

            _animator.ResetTrigger(Attack);
            _animator.SetTrigger(GetHit);

            _animatorHelper.OnStand += PerformStand;
        }

        private void PerformStand()
        {
            StateMachine.SetState(_exitState);
        }

        public override void Exit()
        {
            base.Exit();

            _animatorHelper.OnStand -= PerformStand;
        }

        public void Dispose()
        {
            _animatorHelper.OnStand -= PerformStand;
        }
    }
}