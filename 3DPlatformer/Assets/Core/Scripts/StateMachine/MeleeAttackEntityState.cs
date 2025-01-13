using System;
using System.Linq;
using Core.Scripts.Entity;
using Core.Sounds.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

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

            _attackSound = baseEntity.AttackSound;
            _kickSound = baseEntity.KickSound;
        }

        private bool isKeepAttacking;
        private Vector3 _relativePosition;

        private readonly Animator _animator;
        private readonly GameObject _weapon;
        private readonly EntityState _exitState;
        private readonly LayerMask _entityLayerMask;
        private readonly MonoAnimatorHelper _animatorHelper;

        private static readonly int Attack = Animator.StringToHash("Attack");
        private readonly SoundAsset _kickSound;
        private readonly SoundAsset _attackSound;

        public override void Enter()
        {
            base.Enter();

            _animator.SetTrigger(Attack);
            _weapon.SetActive(true);

            _animatorHelper.OnAttacked += PerformDamageEvent;
            _animatorHelper.OnAttackExitEvent += PerformAttackExit;
        }

        public void SetAimTarget(IDamageable target)
        {
            _relativePosition = target.transform.position - BaseEntity.transform.position;
            _relativePosition.y = 0;
            BaseEntity.RigidBody.MoveRotation(Quaternion.LookRotation(_relativePosition));
        }

        private void PerformDamageEvent()
        {
            //TODO: Выход в мили у игрока, а нахуя??
            isKeepAttacking = false;

            var damageables = Enumerable.ToHashSet(Physics.OverlapSphere(BaseEntity.AttackTransform.position, 2f, _entityLayerMask)
                .Select(coll => coll.GetComponent<IDamageable>())
                .Where(target => target != null && !ReferenceEquals(target, BaseEntity)));

            foreach (var target in damageables)
            {
                Debug.LogWarning(target.transform.name);

                if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Kick")
                {
                    _relativePosition = target.transform.position - BaseEntity.transform.position;
                    _relativePosition.y = 0;
                    _kickSound.Play(Random.Range(0.9f, 1.1f));
                    Debug.LogWarning($"Relative position is {_relativePosition} + Mathf.Sign is{Mathf.Sign(_relativePosition.x)}");
                    target.TakeDamage(15f, (Vector3.right * Mathf.Sign(_relativePosition.x) + Vector3.up) * 5f);
                    continue;
                }

                _attackSound.Play(Random.Range(0.9f, 1.1f));
                target.TakeDamage(10f);

                if (BaseEntity is not MonoPlayerController)
                {
                    isKeepAttacking = true;
                    _animator.SetTrigger(Attack);
                }
            }
        }

        private void PerformAttackExit()
        {
            Debug.LogWarning("2");
            if (!isKeepAttacking)
            {
                StateMachine.SetState(_exitState);
            }
        }

        public override void Exit()
        {
            base.Exit();

            _weapon.SetActive(false);

            _animator.ResetTrigger(Attack);
            _animatorHelper.OnAttacked -= PerformDamageEvent;
            _animatorHelper.OnAttackExitEvent -= PerformAttackExit;
        }

        public void Dispose()
        {
            _animatorHelper.OnAttacked -= PerformDamageEvent;
            _animatorHelper.OnAttackExitEvent -= PerformAttackExit;
        }
    }
}