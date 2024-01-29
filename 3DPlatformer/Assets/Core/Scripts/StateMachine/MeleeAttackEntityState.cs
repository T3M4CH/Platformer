using System;
using System.Linq;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class MeleeAttackEntityState : EntityState, IDisposable
    {
        private readonly Animator _animator;
        private readonly LayerMask _entityLayerMask;
        private readonly MonoAnimatorHelper _animatorHelper;
        
        private static readonly int Attack = Animator.StringToHash("Attack");

        public MeleeAttackEntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity) : base(entityStateMachine, baseEntity)
        {
            _animator = baseEntity.Animator;
            _animatorHelper = baseEntity.AnimatorHelper;
            _entityLayerMask = baseEntity.EntityLayerMask;

            _animatorHelper.OnAttacked += PerformDamageEvent;
        }

        public override void Enter()
        {
            base.Enter();
            
            _animator.SetTrigger(Attack);
        }

        public void SetAimTarget(IDamageable target)
        {
            var relativePosition = BaseEntity.transform.position - target.transform.position;
            BaseEntity.RigidBody.MoveRotation(Quaternion.LookRotation(relativePosition));
        }

        private void PerformDamageEvent()
        {
            var damageables = Physics.OverlapSphere(BaseEntity.transform.position, 2f, _entityLayerMask).Select(coll => coll.GetComponent<IDamageable>()).ToArray();

            foreach (var target in damageables)
            {
                if (ReferenceEquals(target, BaseEntity))
                {
                    continue;    
                }
                
                target?.TakeDamage(10f);
            }

            StateMachine.SetState<PlayerMoveEntityState>();
        }

        public void Dispose()
        {
            _animatorHelper.OnAttacked -= PerformDamageEvent;
        }
    }
}