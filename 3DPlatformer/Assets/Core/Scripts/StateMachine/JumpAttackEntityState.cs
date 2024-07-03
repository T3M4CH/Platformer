using System.Linq;
using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts.StatesMachine
{
    public class JumpAttackEntityState : EntityState
    {
        public JumpAttackEntityState(EntityStateMachine entityStateMachine, BaseEntity baseEntity) : base(entityStateMachine, baseEntity)
        {
            _animator = baseEntity.Animator;
            _transform = baseEntity.transform;
            _rigidBody = baseEntity.RigidBody;
            _collision = baseEntity.EntityCollision;
            _entityLayerMask = baseEntity.EntityLayerMask;

            _kickEffect = BaseEntity.KickEffect;
            _kickEffectParticles = _kickEffect.GetComponentsInChildren<ParticleSystem>().Select(el => el.emission).ToArray();
        }

        private float _currentTime;
        private Vector3 _forceImpulse;
        private IDamageable _damageable;

        private readonly Animator _animator;
        private readonly Transform _transform;
        private static readonly int JumpAttack = Animator.StringToHash("JumpAttack");
        private readonly Rigidbody _rigidBody;
        private readonly EntityCollision _collision;
        private readonly LayerMask _entityLayerMask;
        private readonly GameObject _kickEffect;
        private readonly ParticleSystem.EmissionModule[] _kickEffectParticles;

        public override void Enter()
        {
            base.Enter();
            _currentTime = 0f;
            _animator.SetTrigger(JumpAttack);
            
            SetActiveKickParticle(true);
            
            _collision.CollisionEnter += OnCollisionEnter;
            
            _forceImpulse = _transform.forward * 10f;
            
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(_forceImpulse, ForceMode.Impulse);
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
                damageable.TakeDamage(15f, (_transform.forward + Vector3.up) * 5f);
            }
        }

        public override void Update()
        {
            base.Update();
            _currentTime += Time.deltaTime * 2f;

            if (_currentTime >= 1)
            {
                StateMachine.SetState<PlayerMoveEntityState>();
            }
        }

        public override void Exit()
        {
            base.Exit();
            
            _collision.CollisionEnter -= OnCollisionEnter;
            SetActiveKickParticle(false);
        }
    }
}