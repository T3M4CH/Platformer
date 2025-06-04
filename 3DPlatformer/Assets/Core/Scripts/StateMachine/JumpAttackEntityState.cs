using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Entity;
using Core.Sounds.Scripts;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class JumpAttackEntityState : EntityState
    {
        public JumpAttackEntityState(EntityStateMachine entityStateMachine, EntityState exitState, BaseEntity baseEntity) : base(entityStateMachine, baseEntity)
        {
            _exitState = exitState;
            _animator = baseEntity.Animator;
            _transform = baseEntity.transform;
            _rigidBody = baseEntity.RigidBody;
            _collision = baseEntity.EntityCollision;
            _entityLayerMask = baseEntity.EntityLayerMask;
            _flyingAttackSound = baseEntity.FlyingAttackSound;
            _kickSound = baseEntity.KickSound;

            _kickEffect = BaseEntity.KickEffect;
            _kickEffectParticles = _kickEffect.GetComponentsInChildren<ParticleSystem>().Select(el => el.emission).ToArray();

            _animatorHelper = baseEntity.AnimatorHelper;
        }

        private Vector3 _forceImpulse;
        private IDamageable _damageable;

        private readonly Animator _animator;
        private readonly Transform _transform;
        private static readonly int JumpAttack = Animator.StringToHash("JumpAttack");
        private readonly Rigidbody _rigidBody;
        private readonly EntityState _exitState;
        private readonly EntityCollision _collision;
        private readonly LayerMask _entityLayerMask;
        private readonly GameObject _kickEffect;
        private readonly ParticleSystem.EmissionModule[] _kickEffectParticles;
        private readonly SoundAsset _flyingAttackSound;
        private readonly SoundAsset _kickSound;
        private readonly MonoAnimatorHelper _animatorHelper;

        private Queue<IDamageable> _damageableAtState = new();

        public override void Enter()
        {
            base.Enter();

            _animator.SetTrigger(JumpAttack);
            _flyingAttackSound.Play(Random.Range(0.9f, 1.1f));

            SetActiveKickParticle(true);

            _collision.CollisionEnter += OnCollisionEnter;
            _animatorHelper.OnAttackExitEvent += OnLand;
            _collision.TriggerEnter += OnTriggerEnter;

            _forceImpulse = _transform.forward * 10f;

            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.AddForce(_forceImpulse, ForceMode.Impulse);
        }

        public override void Update()
        {
            base.Update();

            _rigidBody.AddForce(_transform.up * 5);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (_entityLayerMask.value.Includes(collision.gameObject.layer) && collision.transform.TryGetComponent(out IDamageable damageable) && damageable is BaseEntity entity)
            {
                if (_damageableAtState.Contains(damageable)) return;
                if (entity.StateMachine.CurrentEntityState is ThrownEntityState)
                {
                    _damageableAtState.Enqueue(damageable);
                    _kickSound.Play(Random.Range(0.9f, 1.1f));
                    damageable.TakeDamage(15f, (_transform.forward + Vector3.up) * 5f);
                }
            }
        }

        private void SetActiveKickParticle(bool value)
        {
            for (var i = 0; i < _kickEffectParticles.Length; i++)
            {
                _kickEffectParticles[i].enabled = value;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_entityLayerMask.value.Includes(collision.gameObject.layer) && collision.transform.TryGetComponent(out IDamageable damageable))
            {
                if (_damageableAtState.Contains(damageable)) return;
                _damageableAtState.Enqueue(damageable);
                _kickSound.Play(Random.Range(0.9f, 1.1f));
                damageable.TakeDamage(15f, (_transform.forward + Vector3.up) * 5f);
            }
        }

        private void OnLand()
        {
            if (!BaseEntity.InteractionSystem.IsGround.Under)
            {
                StateMachine.SetInheritedState<FallEntityState>();
            }
            else
            {
                StateMachine.SetState(_exitState);
            }
        }

        public override void Exit()
        {
            base.Exit();

            _damageableAtState.Clear();
            _collision.CollisionEnter -= OnCollisionEnter;
            _collision.TriggerEnter -= OnTriggerEnter;
            _animatorHelper.OnAttackExitEvent -= OnLand;
            SetActiveKickParticle(false);

            _animator.ResetTrigger(JumpAttack);
        }
    }
}