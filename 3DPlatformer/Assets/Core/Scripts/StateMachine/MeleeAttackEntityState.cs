using System;
using System.Linq;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class MeleeAttackEntityState : EntityState, IDisposable
    {
        public MeleeAttackEntityState(EntityStateMachine entityStateMachine, EntityState exitState, GameObject weapon, BaseEntity baseEntity) : base(entityStateMachine, baseEntity)
        {
            _animator = baseEntity.Animator;
            _animatorHelper = baseEntity.AnimatorHelper;
            _entityLayerMask = baseEntity.EntityLayerMask;
            _exitState = exitState;
            _weapon = weapon;

            _animatorHelper.OnAttacked += PerformDamageEvent;
            _animatorHelper.OnAttackExitEvent += PerformAttackExit;
        }

        private Vector3 _relativePosition;

        private readonly Animator _animator;
        private readonly GameObject _weapon;
        private readonly EntityState _exitState;
        private readonly LayerMask _entityLayerMask;
        private readonly MonoAnimatorHelper _animatorHelper;

        private static readonly int Attack = Animator.StringToHash("Attack");

        public override void Enter()
        {
            base.Enter();

            _animator.SetTrigger(Attack);
            _weapon.SetActive(true);
        }

        public void SetAimTarget(IDamageable target)
        {
            var relativePosition = target.transform.position - BaseEntity.transform.position;
            relativePosition.y = 0;
            BaseEntity.RigidBody.MoveRotation(Quaternion.LookRotation(relativePosition));
        }

        private void PerformDamageEvent()
        {
            var damageables = Physics.OverlapSphere(BaseEntity.AttackTransform.position, 1f, _entityLayerMask).Select(coll => coll.GetComponent<IDamageable>()).ToArray();

            foreach (var target in damageables)
            {
                if (ReferenceEquals(target, BaseEntity))
                {
                    continue;
                }

                if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Kick")
                {
                    target?.TakeDamage(15f, (Vector3.right * Mathf.Sign(_relativePosition.x) + Vector3.up) * 5f);
                    continue;
                }

                target?.TakeDamage(10f);
            }
        }

        private void PerformAttackExit()
        {
            StateMachine.SetState(_exitState);
        }

        public override void Exit()
        {
            base.Exit();
            
            _weapon.SetActive(false);
        }

        public void Dispose()
        {
            _animatorHelper.OnAttacked -= PerformDamageEvent;
            _animatorHelper.OnAttackExitEvent -= PerformAttackExit;
        }
    }
}