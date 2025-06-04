using System;
using Core.Scripts.Effects.Interfaces;
using Core.Scripts.Entity.Interfaces;
using Core.Scripts.Healthbars;
using Core.Scripts.StatesMachine;
using Core.Sounds.Scripts;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Scripts.Entity
{
    public abstract class BaseEntity : MonoBehaviour, IDamageable
    {
        public event Action OnDead = () => { };
        protected HealthbarManager HealthBarManager;

        [SerializeField] private float maxHealth;
        [SerializeField] private SoundAsset deathSound;
        [SerializeField] private Renderer skinRenderer;
        [SerializeField] private Material dissolvePrefab;

        private float _health;
        private Material _dissolveMaterial;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

        public void Construct(HealthbarManager healthbarManager, IEffectService effectService)
        {
            _health = maxHealth;
            HealthBarManager = healthbarManager;
            EffectService = effectService;

            _dissolveMaterial = new Material(dissolvePrefab);
            var texture = skinRenderer.material.GetTexture(MainTex);
            _dissolveMaterial.SetTexture(MainTex, texture);
        }

        public abstract void UpdateHp(float health, float maxHealth);

        public virtual bool TakeDamage(float damage, Vector3? force = null)
        {
            if (_health < 0) return false;

            _health -= damage;
            UpdateHp(_health, maxHealth);
            skinRenderer.material.DOColor(Color.red, 0.2f).OnKill(() => skinRenderer.material.SetColor(BaseColor, Color.white));

            if (_health < 0)
            {
                skinRenderer.materials = new[] { _dissolveMaterial };
                Animator.speed = 0;

                RigidBody.isKinematic = true;
                var colliders = GetComponentsInChildren<Collider>();
                foreach (var entityCollider in colliders)
                {
                    entityCollider.enabled = false;
                }

                DOTween.To(t => skinRenderer.material.SetFloat(DissolveAmount, t), 0, 1, 1).SetLink(gameObject).OnKill(() => { Destroy(gameObject); });
                EffectService.GetEffect(EVfxType.Die, true).SetPosition(transform.position);

                enabled = false;

                deathSound.Play(Random.Range(0.95f, 1.1f));
                OnDead.Invoke();
                return false;
            }

            return true;
        }

        private void OnDestroy()
        {
            skinRenderer.material.DOKill();
        }

        public abstract EntityStateMachine StateMachine { get; protected set; }

        public IEffectService EffectService { get; private set; }

        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float JumpForce { get; private set; }
        [field: SerializeField] public Vector3 RotationOffset { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public Transform AttackTransform { get; private set; }
        [field: SerializeField] public Rigidbody RigidBody { get; private set; }
        [field: SerializeField] public GameObject KickEffect { get; private set; }
        [field: SerializeField] public EntityCollision EntityCollision { get; private set; }
        [field: SerializeField] public MonoAnimatorHelper AnimatorHelper { get; private set; }
        [field: SerializeField] public SoundAsset FlyingAttackSound { get; private set; }
        [field: SerializeField] public SoundAsset AttackSound { get; private set; }
        [field: SerializeField] public SoundAsset KickSound { get; private set; }
        [field: SerializeField] public MonoInteractionSystem InteractionSystem { get; private set; }
        [field: SerializeField] public LayerMask WaterLayerMask { get; private set; }
        [field: SerializeField] public LayerMask EntityLayerMask { get; private set; }
    }
}